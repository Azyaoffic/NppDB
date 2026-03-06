using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

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
        public string[] Tags;
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

        private bool _isEditingTemplate;
        private bool _suppressPromptTextBoxChange;
        private bool _suppressTagsTextBoxChange;
        private Timer _templateAutoSaveTimer;
        private PromptItem? _pendingAutoSavePrompt;
        private bool _autoSaveErrorShown;
        private bool _canCopy;
        private string _copyDisabledReason;

        private const int TemplateAutoSaveDebounceMs = 650;

        public static string PromptLibraryPath { get; set; }

        public static Dictionary<string, string> Placeholders;
        
        private Control _resizingControl;
        private int _resizeStartY;

        // TODO: Maybe there is a better way to manage supported placeholders?
        public static readonly List<string> SupportedPlaceholders = new List<string>
        {
            "selected_sql"
        };

        public FrmPromptLibrary(Dictionary<string, string> placeholders)
        {
            Placeholders = new Dictionary<string, string>(placeholders);

            InitializeComponent();
            UiThemeManager.Register(this);
            
            promptsGridView.CellDoubleClick += promptsGridView_CellDoubleClick; 
            promptsGridView.RowPostPaint += promptsGridView_RowPostPaint;
            promptTextBox.TextChanged += promptTextBox_TextChanged;
            txtTags.TextChanged += txtTags_TextChanged;
            promptsGridView.CellEndEdit += promptsGridView_CellEndEdit;
            promptsGridView.EditingControlShowing += promptsGridView_EditingControlShowing;

            _templateAutoSaveTimer = new Timer { Interval = TemplateAutoSaveDebounceMs };
            _templateAutoSaveTimer.Tick += TemplateAutoSaveTimer_Tick;

            SetEditingMode(false);
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
            lblPromptType.Text = "Type: -";
            lblPromptCapabilities.Text = "Capabilities: -";
            _actionToolTip.SetToolTip(buttonDuplicate, "Select a prompt to duplicate.");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RestoreLayoutFromSettings();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            FlushPendingAutoSave();
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

        private static string[] ParseTags(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return Array.Empty<string>();

            return raw
                .Split(new[] { ',', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => (t ?? string.Empty).Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string FormatTags(string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return string.Empty;

            return string.Join(", ", tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()));
        }

        private static bool ContainsIgnoreCase(string haystack, string needle)
        {
            if (string.IsNullOrEmpty(needle))
                return true;
            if (string.IsNullOrEmpty(haystack))
                return false;
            return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool PromptMatchesSearch(PromptItem prompt, string rawSearchText)
        {
            var searchText = (rawSearchText ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(searchText))
                return true;

            var tokens = searchText
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => (t ?? string.Empty).Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();

            if (tokens.Length == 0)
                return true;

            var tags = prompt.Tags ?? Array.Empty<string>();

            bool tagHas(string needle)
            {
                if (string.IsNullOrWhiteSpace(needle))
                    return true;
                return tags.Any(t => ContainsIgnoreCase(t, needle));
            }

            bool textHas(string needle)
            {
                return ContainsIgnoreCase(prompt.Title, needle)
                       || ContainsIgnoreCase(prompt.Description, needle)
                       || tags.Any(t => ContainsIgnoreCase(t, needle));
            }

            foreach (var token in tokens)
            {
                if (token.StartsWith("#") && token.Length > 1)
                {
                    if (!tagHas(token.Substring(1)))
                        return false;
                }
                else if (token.StartsWith("tag:", StringComparison.OrdinalIgnoreCase) && token.Length > 4)
                {
                    if (!tagHas(token.Substring(4)))
                        return false;
                }
                else
                {
                    if (!textHas(token))
                        return false;
                }
            }

            return true;
        }

        private List<PromptItem> GetFilteredPrompts(string rawSearchText)
        {
            IEnumerable<PromptItem> query = _prompts ?? new List<PromptItem>();

            query = query.Where(p => PromptMatchesSearch(p, rawSearchText));

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
            var pal = UiThemeManager.Current;

            if (pal.IsDark)
            {
                row.DefaultCellStyle.BackColor = isSchemaAware ? pal.SofterBackground : pal.PureBackground;
                row.DefaultCellStyle.SelectionBackColor = pal.HotBackground;
                row.DefaultCellStyle.SelectionForeColor = pal.Text;
                row.Cells[colPromptType.Index].Style.ForeColor = isSchemaAware ? pal.LinkText : pal.Text;
                return;
            }

            // old light behavior
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
                    row.DividerHeight = (prompt.Tags != null && prompt.Tags.Length > 0) ? 16 : 0; // “mini row” height
                    ApplyRowStyling(row, prompt);
                }

                promptsGridView.ClearSelection();
                if (promptsGridView.Rows.Count > 0)
                    promptsGridView.CurrentCell = null;

                UpdatePromptMeta(null);
                UpdateTagsBox(null);

                if (promptsGridView.Rows.Count == 0)
                {
                    promptTextBox.Text = string.Empty;
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateTagsBox(null);
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

        private void promptsGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var row = promptsGridView.Rows[e.RowIndex];
            if (row.DividerHeight > 0 && row.Tag is PromptItem prompt)
            {
                var tagsText = FormatTags(prompt.Tags);
                if (!string.IsNullOrEmpty(tagsText))
                {
                    var startX = e.RowBounds.Left;
                    if (promptsGridView.RowHeadersVisible)
                        startX += promptsGridView.RowHeadersWidth;

                    var rect = new Rectangle(startX, e.RowBounds.Bottom - row.DividerHeight, e.RowBounds.Width - (startX - e.RowBounds.Left), row.DividerHeight);

                    var isSelected = row.Selected;
                    var pal = UiThemeManager.Current;

                    var bgColor = isSelected ? row.DefaultCellStyle.SelectionBackColor : row.DefaultCellStyle.BackColor;
                    var textColor = isSelected
                        ? row.DefaultCellStyle.SelectionForeColor
                        : pal.IsDark ? pal.DarkerText : Color.DimGray;

                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillRectangle(bgBrush, rect);
                    }

                    var fontSize = 8f;
                    using (var font = new Font(promptsGridView.Font.FontFamily, fontSize, FontStyle.Regular))
                    using (var textBrush = new SolidBrush(textColor))
                    {
                        e.Graphics.DrawString("Tags: " + tagsText, font, textBrush, startX + 5, rect.Top + 1);
                    }

                    using (var linePen = new Pen(promptsGridView.GridColor))
                    {
                        e.Graphics.DrawLine(linePen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FlushPendingAutoSave();
            RefreshPromptList();
        }

        private void cmbPromptSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            FlushPendingAutoSave();
            _currentSourceFilter = (PromptSourceFilter)cmbPromptSource.SelectedIndex;
            RefreshPromptList();
        }
        
        private void promptsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_isEditingTemplate) return;

            promptsGridView.ClearSelection();
            var row = promptsGridView.Rows[e.RowIndex];
            row.Selected = true;

            if (e.ColumnIndex >= 0 && e.ColumnIndex < row.Cells.Count)
                promptsGridView.CurrentCell = row.Cells[e.ColumnIndex];

            editingModeCheckbox.Checked = true;
            promptTextBox.Focus();
        }


        private void UpdatePromptMeta(PromptItem? prompt)
        {
            var pal = UiThemeManager.Current;

            if (!prompt.HasValue)
            {
                lblPromptType.Text = "Type: —";
                lblPromptType.ForeColor = pal.IsDark ? pal.DarkerText : SystemColors.GrayText;
                lblPromptCapabilities.Text = "Capabilities: —";
                panelSchemaBanner.Visible = false;
                _actionToolTip.SetToolTip(buttonDuplicate, "Select a prompt to duplicate.");
                return;
            }

            var selectedPrompt = prompt.Value;
            var isSchemaAware = IsSchemaAwarePrompt(selectedPrompt);

            lblPromptType.Text = isSchemaAware ? "Type: SCHEMA-AWARE" : "Type: LIBRARY";
            lblPromptType.ForeColor = pal.IsDark
                ? isSchemaAware ? pal.LinkText : pal.Text
                : isSchemaAware ? Color.FromArgb(86, 55, 141) : Color.FromArgb(18, 111, 49);
            lblPromptCapabilities.Text = isSchemaAware
                ? "Cannot be used outside of Database Manager context. Can contain DB Schema placeholders."
                : "Cannot contain DB Schema placeholders. Can contain general placeholders.";

            panelSchemaBanner.Visible = isSchemaAware;
        }

        private void promptsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressPromptGridSelectionChanged)
                return;

            FlushPendingAutoSave();

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var selectedRow = promptsGridView.SelectedRows[0];

                if (!(selectedRow.Tag is PromptItem prompt))
                    return;

                GeneratePlaceholderControls(prompt);
                UpdatePromptTextBoxForCurrentMode(prompt);
                UpdatePromptMeta(prompt);
                UpdateTagsBox(prompt);
            }
            else
            {
                SetPreviewText(string.Empty);
                flowLayoutPanelPlaceholders.Controls.Clear();
                UpdateTagsBox(null);
                UpdateCopyButtonState(false, "No prompt selected");
                UpdatePromptMeta(null);
            }
        }

        private void UpdateTagsBox(PromptItem? prompt)
        {
            _suppressTagsTextBoxChange = true;
            try
            {
                if (!prompt.HasValue)
                {
                    txtTags.Text = string.Empty;
                    return;
                }

                txtTags.Text = FormatTags(prompt.Value.Tags);
            }
            finally
            {
                _suppressTagsTextBoxChange = false;
            }
        }

        private void GeneratePlaceholderControls(PromptItem prompt)
        {
            var pal = UiThemeManager.Current;

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
                        label.ForeColor = pal.IsDark ? pal.Text : Color.Black;
                        
                        var tip = new ToolTip();
                        tip.SetToolTip(label, "This field is required.");
                    }
                    else
                    {
                        label.Text = $"{placeholder.Name} (Auto-filled)";
                        label.ForeColor = pal.IsDark ? pal.DarkerText : Color.DimGray;
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
                                Height = 80,
                                Width = container.Width,
                                Text = initialValue,
                                Tag = placeholder.Name,
                                BorderStyle = BorderStyle.FixedSingle,
                                ScrollBars = RichTextBoxScrollBars.Vertical
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
                                Tag = placeholder.Name,
                                Multiline = true,
                                Height = 60,
                                ScrollBars = ScrollBars.Vertical
                            };
                            txtBox.TextChanged += InputControl_TextChanged;
                            inputControl = txtBox;
                        }
                        container.Controls.Add(inputControl);

                        // vertical resize grip
                        var grip = new Panel
                        {
                            Height = 5,
                            Dock = DockStyle.Bottom,
                            Cursor = Cursors.SizeNS,
                            BackColor = pal.IsDark ? pal.Edge : SystemColors.ControlDark
                        };
                        grip.MouseDown += Grip_MouseDown;
                        grip.MouseMove += Grip_MouseMove;
                        grip.MouseUp += Grip_MouseUp;
                        grip.Tag = inputControl;
                        container.Controls.Add(grip);
                    }
                    else
                    {
                        // read-only placeholders
                        var lblValue = new RichTextBox
                        {
                            Width = container.Width,
                            Text = string.IsNullOrEmpty(initialValue) ? "<No context available>" : initialValue,
                            ReadOnly = true,
                            BackColor = pal.IsDark ? pal.Background : SystemColors.Control,
                            ForeColor = pal.IsDark ? pal.Text : SystemColors.WindowText
                        };
                        container.Controls.Add(lblValue);
                    }

                    flowLayoutPanelPlaceholders.Controls.Add(container);
                }
            }
            UiThemeManager.Apply(flowLayoutPanelPlaceholders);

            flowLayoutPanelPlaceholders.ResumeLayout();
            
            ResizePlaceholderControlWidths();

            ValidateInputs();
        }
        
        private void Grip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _resizingControl = (Control)((Panel)sender).Tag;
                _resizeStartY = Cursor.Position.Y;
                ((Panel)sender).Capture = true;
            }
        }

        private void Grip_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizingControl != null && e.Button == MouseButtons.Left)
            {
                int delta = Cursor.Position.Y - _resizeStartY;
                _resizingControl.Height = Math.Max(40, _resizingControl.Height + delta);
                _resizeStartY = Cursor.Position.Y;
                flowLayoutPanelPlaceholders.PerformLayout();
            }
        }

        private void Grip_MouseUp(object sender, MouseEventArgs e)
        {
            _resizingControl = null;
            if (sender is Panel p) p.Capture = false;
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
                if (!_isEditingTemplate) UpdatePreviewText(prompt);
            }

            ValidateInputs();
        }
        
        private void SetPreviewText(string text)
        {
            promptTextBox.SuspendLayout();

            _suppressPromptTextBoxChange = true;
            try
            {
                promptTextBox.Clear();
                promptTextBox.Text = text;

                promptTextBox.SelectAll();
                promptTextBox.SelectionFont = promptTextBox.Font;
                promptTextBox.SelectionColor = promptTextBox.ForeColor;

                promptTextBox.SelectionStart = 0;
                promptTextBox.SelectionLength = 0;
                promptTextBox.ScrollToCaret();
            }
            finally
            {
                _suppressPromptTextBoxChange = false;
                promptTextBox.ResumeLayout();
            }
        }


        private void UpdatePreviewText(PromptItem prompt)
        {
            if (_isEditingTemplate)
            {
                SetPreviewText(prompt.Text ?? string.Empty);
                return;
            }

            var baseText = ConstructPromptPreview(SubstitutePlaceholders(prompt.Text));
            SetPreviewText(baseText);
        }
        
        private void SetEditingMode(bool isEditing)
        {
            FlushPendingAutoSave();

            _isEditingTemplate = isEditing;

            grpPreview.Text = _isEditingTemplate ? "Edit Template (Auto-Save)" : "Prompt Preview";
            promptTextBox.BorderStyle = BorderStyle.FixedSingle;
            txtTags.BorderStyle = BorderStyle.FixedSingle;
            
            promptTextBox.ReadOnly = !isEditing;
            txtTags.ReadOnly = !isEditing;
            promptsGridView.ReadOnly = !isEditing;
            
            promptTextBox.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;
            txtTags.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;

            lblEditingBadge.Text = _isEditingTemplate ? "EDITING" : "VIEW";

            colPromptType.ReadOnly = true;
            colPromptName.ReadOnly = !isEditing;
            colPromptDesc.ReadOnly = !isEditing;

            var pal = UiThemeManager.Current;

            if (pal.IsDark)
            {
                promptTextBox.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
                txtTags.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
            }
            else
            {
                promptTextBox.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
                txtTags.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
            }

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                UpdatePromptTextBoxForCurrentMode(prompt);
            }
        }
        
        private void promptsGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isEditingTemplate) return;
            if (e.RowIndex < 0) return;

            var row = promptsGridView.Rows[e.RowIndex];
            if (!(row.Tag is PromptItem prompt)) return;

            bool changed = false;

            if (e.ColumnIndex == colPromptName.Index)
            {
                var v = Convert.ToString(row.Cells[colPromptName.Index].Value) ?? "";
                v = v.Trim();
                if (!string.IsNullOrWhiteSpace(v) && v != prompt.Title)
                {
                    prompt.Title = v;
                    changed = true;
                }
                else
                {
                    row.Cells[colPromptName.Index].Value = prompt.Title;
                }
            }
            else if (e.ColumnIndex == colPromptDesc.Index)
            {
                var v = Convert.ToString(row.Cells[colPromptDesc.Index].Value) ?? "";
                v = v.Trim();
                if (v != (prompt.Description ?? ""))
                {
                    prompt.Description = v;
                    changed = true;
                }
            }

            if (!changed) return;

            row.Tag = prompt;
            ReplacePromptInCollections(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }
        
        private void promptsGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl tb)
            {
                var pal = UiThemeManager.Current;

                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.BackColor = pal.IsDark ? pal.HotBackground : Color.White;
                tb.ForeColor = pal.IsDark ? pal.Text : SystemColors.ControlText;
            }
        }

        private void txtTags_TextChanged(object sender, EventArgs e)
        {
            if (_suppressTagsTextBoxChange)
                return;

            if (!_isEditingTemplate)
                return;

            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            if (!(selectedRow.Tag is PromptItem prompt))
                return;

            prompt.Tags = ParseTags(txtTags.Text);

            selectedRow.Tag = prompt;
            ReplacePromptInCollections(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }

        private void UpdatePromptTextBoxForCurrentMode(PromptItem prompt)
        {
            if (_isEditingTemplate)
                SetPreviewText(prompt.Text ?? string.Empty);
            else
                UpdatePreviewText(prompt);
        }

        private void promptTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suppressPromptTextBoxChange)
                return;

            if (!_isEditingTemplate)
                return;

            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            if (!(selectedRow.Tag is PromptItem prompt))
                return;

            prompt.Text = promptTextBox.Text;
            prompt.Placeholders = BuildPlaceholdersFromText(prompt.Text, prompt.Placeholders);

            selectedRow.Tag = prompt;
            ReplacePromptInCollections(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }

        private void TemplateAutoSaveTimer_Tick(object sender, EventArgs e)
        {
            _templateAutoSaveTimer.Stop();

            if (!_pendingAutoSavePrompt.HasValue)
                return;

            var promptToSave = _pendingAutoSavePrompt.Value;
            _pendingAutoSavePrompt = null;

            try
            {
                FrmPromptEditor.SaveNewPromptToFile(promptToSave);
                _autoSaveErrorShown = false;

                if (promptsGridView.SelectedRows.Count > 0)
                {
                    var selectedPrompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                    if (selectedPrompt.Id == promptToSave.Id)
                    {
                        GeneratePlaceholderControls(promptToSave);
                        ValidateInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_autoSaveErrorShown)
                {
                    _autoSaveErrorShown = true;
                    MessageBox.Show("Auto-save failed: " + ex.Message, "Prompt Auto-Save",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void FlushPendingAutoSave()
        {
            if (_templateAutoSaveTimer != null && _templateAutoSaveTimer.Enabled)
            {
                TemplateAutoSaveTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void ReplacePromptInCollections(PromptItem updatedPrompt)
        {
            if (_prompts != null)
            {
                var index = _prompts.FindIndex(p => p.Id == updatedPrompt.Id);
                if (index >= 0)
                    _prompts[index] = updatedPrompt;
            }

            if (_filteredPrompts != null)
            {
                var index = _filteredPrompts.FindIndex(p => p.Id == updatedPrompt.Id);
                if (index >= 0)
                    _filteredPrompts[index] = updatedPrompt;
            }
        }

        private PromptPlaceholder[] BuildPlaceholdersFromText(string promptText, PromptPlaceholder[] existingPlaceholders)
        {
            var extracted = new List<PromptPlaceholder>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            const string pattern = @"\{\{(.*?)\}\}";
            foreach (System.Text.RegularExpressions.Match match in
                     System.Text.RegularExpressions.Regex.Matches(promptText ?? string.Empty, pattern))
            {
                var name = match.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(name))
                    continue;

                if (!seen.Add(name))
                    continue;

                var placeholder = new PromptPlaceholder { Name = name, IsEditable = true, IsRichText = false };

                if (existingPlaceholders != null)
                {
                    for (int i = 0; i < existingPlaceholders.Length; i++)
                    {
                        if (!string.Equals(existingPlaceholders[i].Name, name, StringComparison.OrdinalIgnoreCase))
                            continue;

                        placeholder.IsEditable = existingPlaceholders[i].IsEditable;
                        placeholder.IsRichText = existingPlaceholders[i].IsRichText;
                        break;
                    }
                }

                extracted.Add(placeholder);
            }

            return extracted.ToArray();
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
            var pal = UiThemeManager.Current;

            _canCopy = enabled;
            _copyDisabledReason = reason ?? string.Empty;

            buttonCopy.Enabled = true;
            buttonCopy.Text = "Copy Prompt";

            if (enabled)
            {
                buttonCopy.BackColor = pal.IsDark ? pal.HotBackground : SystemColors.Highlight;
                buttonCopy.ForeColor = pal.IsDark ? pal.Text : SystemColors.HighlightText;
                _actionToolTip.SetToolTip(buttonCopy, "Copy prompt to clipboard");
            }
            else
            {
                buttonCopy.BackColor = pal.IsDark ? pal.SofterBackground : Color.Gainsboro;
                buttonCopy.ForeColor = pal.IsDark ? pal.DarkerText : Color.DimGray;
                _actionToolTip.SetToolTip(buttonCopy, _copyDisabledReason);
            }
        }

        private string SubstitutePlaceholders(string text)
        {
            text = text ?? string.Empty;

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
        
        private void flowLayoutPanelPlaceholders_SizeChanged(object sender, EventArgs e)
        {
            ResizePlaceholderControlWidths();
        }

        private void ResizePlaceholderControlWidths()
        {
            var targetWidth = flowLayoutPanelPlaceholders.ClientSize.Width - 10 - SystemInformation.VerticalScrollBarWidth;
            if (targetWidth < 200) targetWidth = 200;

            foreach (Control c in flowLayoutPanelPlaceholders.Controls)
            {
                if (!(c is FlowLayoutPanel container))
                    continue;

                container.Width = targetWidth;

                foreach (Control inner in container.Controls)
                {
                    if (inner is TextBox || inner is RichTextBox || inner is Panel)
                        inner.Width = targetWidth;
                }
            }
        }

        private string LoadUserPromptPreferences()
        {
            var preferences = NppDbSettingsStore.Get().Prompt;
            return $"\nRespond in the following language: {preferences.ResponseLanguage}." +
                   $"\nAlso follow user's custom instructions: {preferences.CustomInstructions}.";
        }

        private void editingModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            SetEditingMode(editingModeCheckbox.Checked);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FlushPendingAutoSave();
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
            FlushPendingAutoSave();
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
                    _prompts.RemoveAll(p => p.Id == prompt.Id);
                    
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

            var doc = XDocument.Load(promptLibraryPath);
            var root = doc.Element("Prompts");
            if (root == null) return;

            var promptElement = Enumerable.FirstOrDefault(root.Elements("Prompt"), e =>
                (string)e.Element("Id") == promptItem.Id);
            if (promptElement != null)
            {
                promptElement.Remove();
                doc.Save(promptLibraryPath);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (!_canCopy)
            {
                if (!string.IsNullOrWhiteSpace(_copyDisabledReason))
                    _actionToolTip.Show(_copyDisabledReason, buttonCopy, buttonCopy.Width / 2, -18, 2000);
                return;
            }

            var pal = UiThemeManager.Current;

            var textToCopy = promptTextBox.Text;

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                textToCopy = ConstructPromptPreview(SubstitutePlaceholders(prompt.Text));
            }

            if (!string.IsNullOrEmpty(textToCopy))
            {
                Clipboard.SetText(textToCopy);
                buttonCopy.BackColor = pal.IsDark ? pal.SofterBackground : Color.LightGreen;
                buttonCopy.ForeColor = pal.IsDark ? pal.Text : buttonCopy.ForeColor;
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
            FlushPendingAutoSave();
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
                var url = NppDbSettingsStore.Get().Prompt.OpenLlmUrl?.Trim() ?? "https://chatgpt.com/";
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
