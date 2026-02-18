using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NppDB.Core
{
    public struct PromptPlaceholder
    {
        public string Name;
        public bool IsEditable;
        public bool IsRichText;
    }

    public struct PromptItem
    {
        public string Id;
        public string Title;
        public string Description;
        public string Type; // "TablePrompt", "LlmPrompt"
        public string Text;
        public PromptPlaceholder[] Placeholders;
    }

    public partial class FrmPromptLibrary : Form
    {
        private enum PromptSourceFilter
        {
            Library = 0,
            SchemaAware = 1,
            All = 2
        }

        private static List<PromptItem> _prompts;
        private List<PromptItem> _filteredPrompts;

        private HashSet<string> _placeholdersInCurrentView = new HashSet<string>();
        
        private const int MinPlaceholdersHeight = 80;
        private const int MinPreviewTextHeight = 120;

        private PromptSourceFilter _currentSourceFilter = PromptSourceFilter.Library;

        private bool _suppressPromptGridSelectionChanged;

        private readonly ToolTip _actionToolTip = new ToolTip();

        public static string PromptLibraryPath { get; set; }

        public static Dictionary<string, string> Placeholders;

        // TODO: Maybe there is a better way to manage supported placeholders?
        public static readonly List<string> SupportedPlaceholders = new List<string>
        {
            "selected_sql"
        };

        public FrmPromptLibrary(Dictionary<string, string> placeholders)
        {
            Placeholders = new Dictionary<string, string>(placeholders);

            InitializeComponent();
            
            promptsGridView.CellDoubleClick += promptsGridView_CellDoubleClick; 

            ConfigureSourceFilter();
            RefreshPromptList();
        }

        private void ConfigureSourceFilter()
        {
            cmbPromptSource.Items.Clear();
            cmbPromptSource.Items.Add("Library");
            cmbPromptSource.Items.Add("Schema-Aware");
            cmbPromptSource.Items.Add("All");
            cmbPromptSource.SelectedIndex = (int)PromptSourceFilter.Library;

            panelSchemaBanner.Visible = false;
            lblPromptType.Text = "Type: —";
            lblPromptCapabilities.Text = "Capabilities: —";
            _actionToolTip.SetToolTip(buttonDuplicate, "Select a prompt to duplicate.");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RestoreLayoutFromSettings();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveLayoutToSettings();
            base.OnFormClosing(e);
        }

        private void RestoreLayoutFromSettings()
        {
            var rawSize = Properties.Settings.Default["PromptLibrary_Size"];
            var savedSize = rawSize is Size s ? s : Size.Empty;
            if (savedSize.Width > 0 && savedSize.Height > 0)
            {
                Size = savedSize;
            }

            var rawLocation = Properties.Settings.Default["PromptLibrary_Location"];
            var savedLocation = rawLocation is Point p ? p : Point.Empty;
            if (!savedLocation.IsEmpty)
            {
                StartPosition = FormStartPosition.Manual;
                Location = savedLocation;
            }

            var rawHeight = Properties.Settings.Default["PromptLibrary_PlaceholdersHeight"];
            var savedPlaceholdersHeight = rawHeight is int h && h > 0 ? h : 138;

            var maxBottom = Math.Max(MinPlaceholdersHeight, grpPreview.ClientSize.Height - MinPreviewTextHeight);
            panelPreviewBottom.Height = Math.Max(MinPlaceholdersHeight, Math.Min(savedPlaceholdersHeight, maxBottom));
        }


        private void SaveLayoutToSettings()
        {
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.PromptLibrary_Size = Size;
                Properties.Settings.Default.PromptLibrary_Location = Location;
            }
            else
            {
                Properties.Settings.Default.PromptLibrary_Size = RestoreBounds.Size;
                Properties.Settings.Default.PromptLibrary_Location = RestoreBounds.Location;
            }

            Properties.Settings.Default.PromptLibrary_PlaceholdersHeight = panelPreviewBottom.Height;
            Properties.Settings.Default.Save();
        }

        public static void SetPrompts(List<PromptItem> promptItems)
        {
            _prompts = promptItems;
        }

        private bool IsSchemaAwarePrompt(PromptItem prompt)
        {
            return string.Equals(prompt.Type, "TablePrompt", StringComparison.OrdinalIgnoreCase);
        }

        private List<PromptItem> GetFilteredPrompts(string rawSearchText)
        {
            IEnumerable<PromptItem> query = _prompts ?? new List<PromptItem>();
            var searchText = (rawSearchText ?? string.Empty).Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.Title) && p.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            switch (_currentSourceFilter)
            {
                case PromptSourceFilter.Library:
                    query = query.Where(p => !IsSchemaAwarePrompt(p));
                    break;
                case PromptSourceFilter.SchemaAware:
                    query = query.Where(IsSchemaAwarePrompt);
                    break;
                case PromptSourceFilter.All:
                    break;
            }

            return query.ToList();
        }

        private void RefreshPromptList()
        {
            _filteredPrompts = GetFilteredPrompts(txtSearch.Text);
            PopulatePromptList();
        }

        private void ApplyRowStyling(DataGridViewRow row, PromptItem prompt)
        {
            var isSchemaAware = IsSchemaAwarePrompt(prompt);

            if (isSchemaAware)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(246, 241, 255);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(86, 55, 141);
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                row.Cells[colPromptType.Index].Style.ForeColor = Color.FromArgb(86, 55, 141);
            }
            else
            {
                row.DefaultCellStyle.BackColor = SystemColors.Window;
                row.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                row.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                row.Cells[colPromptType.Index].Style.ForeColor = Color.FromArgb(18, 111, 49);
            }
        }

        private void PopulatePromptList()
        {
            _suppressPromptGridSelectionChanged = true;
            try
            {
                promptsGridView.Rows.Clear();

                foreach (var prompt in _filteredPrompts)
                {
                    var promptKind = IsSchemaAwarePrompt(prompt) ? "SCHEMA-AWARE" : "LIBRARY";
                    var rowIndex = promptsGridView.Rows.Add(prompt.Title, prompt.Description, promptKind);
                    var row = promptsGridView.Rows[rowIndex];
                    row.Tag = prompt;
                    ApplyRowStyling(row, prompt);
                }

                promptsGridView.ClearSelection();
                if (promptsGridView.Rows.Count > 0)
                    promptsGridView.CurrentCell = null;

                UpdatePromptMeta(null);

                if (promptsGridView.Rows.Count == 0)
                {
                    promptTextBox.Text = string.Empty;
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateCopyButtonState(false, "No prompts match the search criteria.");
                }
                else
                {
                    UpdateCopyButtonState(false, "No prompt selected");
                }
            }
            finally
            {
                _suppressPromptGridSelectionChanged = false;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshPromptList();
        }

        private void cmbPromptSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentSourceFilter = (PromptSourceFilter)cmbPromptSource.SelectedIndex;
            RefreshPromptList();
        }
        
        private void promptsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            promptsGridView.ClearSelection();
            var row = promptsGridView.Rows[e.RowIndex];
            row.Selected = true;

            if (e.ColumnIndex >= 0 && e.ColumnIndex < row.Cells.Count)
                promptsGridView.CurrentCell = row.Cells[e.ColumnIndex];

            buttonEdit_Click(buttonEdit, EventArgs.Empty);
        }


        private void UpdatePromptMeta(PromptItem? prompt)
        {
            if (!prompt.HasValue)
            {
                lblPromptType.Text = "Type: —";
                lblPromptType.ForeColor = SystemColors.GrayText;
                lblPromptCapabilities.Text = "Capabilities: —";
                panelSchemaBanner.Visible = false;
                _actionToolTip.SetToolTip(buttonDuplicate, "Select a prompt to duplicate.");
                return;
            }

            var selectedPrompt = prompt.Value;
            var isSchemaAware = IsSchemaAwarePrompt(selectedPrompt);

            lblPromptType.Text = isSchemaAware ? "Type: SCHEMA-AWARE" : "Type: LIBRARY";
            lblPromptType.ForeColor = isSchemaAware
                ? Color.FromArgb(86, 55, 141)
                : Color.FromArgb(18, 111, 49);
            lblPromptCapabilities.Text = isSchemaAware
                ? "Cannot be used outside of Database Manager context. Can contain DB Schema placeholders."
                : "Cannot contain DB Schema placeholders. Can contain general placeholders.";

            panelSchemaBanner.Visible = isSchemaAware;
        }

        private void promptsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressPromptGridSelectionChanged)
                return;

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var selectedRow = promptsGridView.SelectedRows[0];

                if (!(selectedRow.Tag is PromptItem prompt))
                    return;

                GeneratePlaceholderControls(prompt);
                UpdatePreviewText(prompt);
                UpdatePromptMeta(prompt);
            }
            else
            {
                promptTextBox.Text = string.Empty;
                flowLayoutPanelPlaceholders.Controls.Clear();
                UpdateCopyButtonState(false, "No prompt selected");
                UpdatePromptMeta(null);
            }
        }

        private void GeneratePlaceholderControls(PromptItem prompt)
        {
            flowLayoutPanelPlaceholders.Controls.Clear();
            flowLayoutPanelPlaceholders.SuspendLayout();
            
            _placeholdersInCurrentView.Clear();

            if (prompt.Placeholders != null)
            {
                foreach (var placeholder in prompt.Placeholders)
                {
                    if (string.IsNullOrEmpty(placeholder.Name) || _placeholdersInCurrentView.Contains(placeholder.Name))
                        continue;
                    
                    _placeholdersInCurrentView.Add(placeholder.Name);
                    
                    var container = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        AutoSize = true,
                        Width = flowLayoutPanelPlaceholders.Width - 25,
                        Margin = new Padding(0, 0, 0, 10)
                    };

                    var label = new Label
                    {
                        AutoSize = true,
                        Margin = new Padding(0, 0, 0, 2)
                    };

                    if (placeholder.IsEditable)
                    {
                        label.Text = $"{placeholder.Name} *";
                        label.Font = new Font(label.Font, FontStyle.Bold);
                        label.ForeColor = Color.Black; 
                        
                        var tip = new ToolTip();
                        tip.SetToolTip(label, "This field is required.");
                    }
                    else
                    {
                        label.Text = $"{placeholder.Name} (Auto-filled)";
                        label.ForeColor = Color.DimGray;
                    }

                    container.Controls.Add(label);

                    // input control
                    var initialValue = "";
                    if (Placeholders.ContainsKey(placeholder.Name))
                    {
                        initialValue = Placeholders[placeholder.Name];
                    }

                    if (placeholder.IsEditable)
                    {
                        Control inputControl;
                        if (placeholder.IsRichText)
                        {
                            var rtBox = new RichTextBox
                            {
                                Height = 60,
                                Width = container.Width,
                                Text = initialValue,
                                Tag = placeholder.Name,
                                BorderStyle = BorderStyle.FixedSingle
                            };
                            rtBox.TextChanged += InputControl_TextChanged;
                            inputControl = rtBox;
                        }
                        else
                        {
                            var txtBox = new TextBox
                            {
                                Width = container.Width,
                                Text = initialValue,
                                Tag = placeholder.Name
                            };
                            txtBox.TextChanged += InputControl_TextChanged;
                            inputControl = txtBox;
                        }
                        container.Controls.Add(inputControl);
                    }
                    else
                    {
                        // read-only placeholders
                        var lblValue = new TextBox
                        {
                            Width = container.Width,
                            Text = string.IsNullOrEmpty(initialValue) ? "<No context available>" : initialValue,
                            ReadOnly = true,
                            BackColor = SystemColors.Control,
                            ForeColor = SystemColors.WindowText
                        };
                        container.Controls.Add(lblValue);
                    }

                    flowLayoutPanelPlaceholders.Controls.Add(container);
                }
            }

            flowLayoutPanelPlaceholders.ResumeLayout();
            
            ValidateInputs();
        }

        private void InputControl_TextChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var key = (string)control.Tag;
            var value = control.Text;

            Placeholders[key] = value;

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                UpdatePreviewText(prompt);
            }

            ValidateInputs();
        }

        private void UpdatePreviewText(PromptItem prompt)
        {
            if (disableTemplatingCheckbox.Checked)
            {
                promptTextBox.Text = ConstructPromptPreview(prompt.Text);
            }
            else
            {
                promptTextBox.Text = ConstructPromptPreview(SubstitutePlaceholders(prompt.Text));
            }
        }

        private void ValidateInputs()
        {
            if (promptsGridView.SelectedRows.Count == 0)
            {
                UpdateCopyButtonState(false, "No prompt selected");
                return;
            }

            var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
            if (prompt.Type == "TablePrompt")
            {
                UpdateCopyButtonState(false, "Schema-Aware prompt: use it from Database Manager (F10), not via copy");
                return;
            }
            
            var isValid = true;

            if (prompt.Placeholders != null)
            {
                foreach (var ph in prompt.Placeholders)
                {
                    if (ph.IsEditable)
                    {
                        if (!Placeholders.TryGetValue(ph.Name, out var val) || string.IsNullOrWhiteSpace(val))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
            }

            UpdateCopyButtonState(isValid, "Fill required fields (*) to Copy");
        }

        private void UpdateCopyButtonState(bool enabled, string reason)
        {
            buttonCopy.Enabled = enabled;
            if (enabled)
            {
                buttonCopy.Text = "Copy Prompt";
                buttonCopy.BackColor = SystemColors.Highlight;
                buttonCopy.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                buttonCopy.Text = reason;
                buttonCopy.BackColor = Color.LightGray;
                buttonCopy.ForeColor = Color.DimGray;
            }
        }

        private string SubstitutePlaceholders(string text)
        {
            var selectedSql = Placeholders.TryGetValue("selected_sql", out var plh)
                ? plh
                : string.Empty;

            text = text.Replace("{{selected_sql}}", selectedSql);
            
            foreach (var placeholder in Placeholders)
            {
                if (!string.IsNullOrEmpty(placeholder.Value))
                {
                    text = text.Replace("{{" + placeholder.Key + "}}", placeholder.Value);
                }
            }
            
            return text;
        }
        
        private string ConstructPromptPreview(string promptBase)
        {
            var PromptStringBuilder = new StringBuilder();
            PromptStringBuilder.Append(promptBase);
            PromptStringBuilder.Append(LoadUserPromptPreferences());
            return PromptStringBuilder.ToString();
        }

        private string LoadUserPromptPreferences()
        {
            var preferences = FrmPromptPreferences.ReadUserPreferences();
            return $"\nRespond in the following language: {preferences.ResponseLanguage}." +
                   $"\nAlso follow user's custom instructions: {preferences.CustomInstructions}.";
        }

        private void disableTemplatingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                UpdatePreviewText(prompt);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (promptsGridView.SelectedRows.Count > 0)
            {
                var selectedRow = promptsGridView.SelectedRows[0];
                var prompt = (PromptItem)selectedRow.Tag;

                using (var promptEditor = new FrmPromptEditor())
                {
                    promptEditor.LoadPromptItem(prompt);
                    promptEditor.LoadPlaceholders(SupportedPlaceholders);

                    var result = promptEditor.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var updatedPrompt = promptEditor.SelectedPromptItem;

                        // update the prompt in the grid
                        selectedRow.Cells[0].Value = updatedPrompt.Title;
                        selectedRow.Cells[1].Value = updatedPrompt.Description;
                        selectedRow.Cells[colPromptType.Index].Value = IsSchemaAwarePrompt(updatedPrompt) ? "SCHEMA-AWARE" : "LIBRARY";
                        selectedRow.Tag = updatedPrompt;
                        ApplyRowStyling(selectedRow, updatedPrompt);

                        var index = _prompts.FindIndex(p => p.Title == prompt.Title);
                        if (index >= 0) _prompts[index] = updatedPrompt;

                        // refresh
                        promptsListView_SelectedIndexChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var promptEditor = new FrmPromptEditor())
            {
                promptEditor.LoadPlaceholders(SupportedPlaceholders);

                var result = promptEditor.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var newPrompt = promptEditor.SelectedPromptItem;
                    _prompts.Add(newPrompt);
                    
                    // refresh search results to possibly include the new prompt
                    RefreshPromptList();
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (promptsGridView.SelectedRows.Count > 0)
            {
                var selectedRow = promptsGridView.SelectedRows[0];
                var prompt = (PromptItem)selectedRow.Tag;

                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the prompt '{prompt.Title}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    _prompts.RemoveAll(p => p.Title == prompt.Title);
                    
                    // Update UI via search logic
                    RefreshPromptList();

                    ErasePromptFromFile(prompt);

                    promptTextBox.Text = string.Empty;
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateCopyButtonState(false, "No prompt selected");
                }
            }
        }

        private void ErasePromptFromFile(PromptItem promptItem)
        {
            string promptLibraryPath = PromptLibraryPath;
            if (!File.Exists(promptLibraryPath))
            {
                throw new FileNotFoundException("Prompt library file not found.", promptLibraryPath);
            }

            var doc = System.Xml.Linq.XDocument.Load(promptLibraryPath);
            var root = doc.Element("Prompts");
            if (root == null) return;

            var promptElement = System.Linq.Enumerable.FirstOrDefault(root.Elements("Prompt"), e =>
                (string)e.Element("Title") == promptItem.Title);
            if (promptElement != null)
            {
                promptElement.Remove();
                doc.Save(promptLibraryPath);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (!buttonCopy.Enabled) return;

            if (!string.IsNullOrEmpty(promptTextBox.Text))
            {
                Clipboard.SetText(promptTextBox.Text);
                buttonCopy.BackColor = Color.LightGreen;
                buttonCopy.Text = "Copied!";
                var timer = new Timer { Interval = 1000 };
                timer.Tick += (s, args) =>
                {
                    ValidateInputs(); 
                    timer.Stop();
                };
                timer.Start();
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            var prompt = (PromptItem)selectedRow.Tag;

            var newPrompt = prompt;
            newPrompt.Id = Guid.NewGuid().ToString();
            newPrompt.Title += " (Copy)";

            _prompts.Add(newPrompt);
            FrmPromptEditor.SaveNewPromptToFile(newPrompt);

            RefreshPromptList();
        }
        
        private void buttonAiStudio_Click(object sender, EventArgs e)
        {
            try
            {
                var url = FrmPromptPreferences.ReadUserPreferences().OpenLlmUrl;
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open the browser: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
