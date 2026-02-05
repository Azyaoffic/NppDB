﻿using System;
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
        public string Type; // "SqlTemplate", "LlmPrompt"
        public string Text;
        public PromptPlaceholder[] Placeholders;
    }

    public partial class FrmPromptLibrary : Form
    {
        private static List<PromptItem> _prompts;
        private List<PromptItem> _filteredPrompts;

        private const int MinPlaceholdersHeight = 80;
        private const int MinPreviewTextHeight = 120;

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

            promptsListView.View = View.Details;

            promptsListView.Columns.Clear();
            promptsListView.Columns.Add("Prompt Name", 200);
            promptsListView.Columns.Add("Description", 300);

            _filteredPrompts = new List<PromptItem>(_prompts);
            PopulatePromptList();
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

        private void PopulatePromptList()
        {
            promptsListView.Items.Clear();
            foreach (var prompt in _filteredPrompts)
            {
                var item = new ListViewItem(prompt.Title);
                item.SubItems.Add(prompt.Description);
                item.Tag = prompt;
                promptsListView.Items.Add(item);
            }
            
            if (promptsListView.Items.Count == 0)
            {
                promptTextBox.Text = string.Empty;
                flowLayoutPanelPlaceholders.Controls.Clear();
                UpdateCopyButtonState(false);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var searchText = txtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                _filteredPrompts = new List<PromptItem>(_prompts);
            }
            else
            {
                _filteredPrompts = _prompts.Where(p => 
                    (p.Title != null && p.Title.ToLower().Contains(searchText)) || 
                    (p.Description != null && p.Description.ToLower().Contains(searchText))
                ).ToList();
            }

            PopulatePromptList();
        }

        private void promptsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (promptsListView.SelectedItems.Count > 0)
            {
                var selectedItem = promptsListView.SelectedItems[0];
                var prompt = (PromptItem)selectedItem.Tag;

                GeneratePlaceholderControls(prompt);
                UpdatePreviewText(prompt);
            }
            else
            {
                promptTextBox.Text = string.Empty;
                flowLayoutPanelPlaceholders.Controls.Clear();
                UpdateCopyButtonState(false);
            }
        }

        private void GeneratePlaceholderControls(PromptItem prompt)
        {
            flowLayoutPanelPlaceholders.Controls.Clear();
            flowLayoutPanelPlaceholders.SuspendLayout();

            if (prompt.Placeholders != null)
            {
                foreach (var placeholder in prompt.Placeholders)
                {
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

            if (promptsListView.SelectedItems.Count > 0)
            {
                var prompt = (PromptItem)promptsListView.SelectedItems[0].Tag;
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
            if (promptsListView.SelectedItems.Count == 0)
            {
                UpdateCopyButtonState(false);
                return;
            }

            var prompt = (PromptItem)promptsListView.SelectedItems[0].Tag;
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

            UpdateCopyButtonState(isValid);
        }

        private void UpdateCopyButtonState(bool enabled)
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
                buttonCopy.Text = "Fill required fields (*) to Copy";
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
            if (promptsListView.SelectedItems.Count > 0)
            {
                var prompt = (PromptItem)promptsListView.SelectedItems[0].Tag;
                UpdatePreviewText(prompt);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (promptsListView.SelectedItems.Count > 0)
            {
                var selectedItem = promptsListView.SelectedItems[0];
                var prompt = (PromptItem)selectedItem.Tag;

                using (var promptEditor = new FrmPromptEditor())
                {
                    promptEditor.LoadPromptItem(prompt);
                    promptEditor.LoadPlaceholders(SupportedPlaceholders);

                    var result = promptEditor.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var updatedPrompt = promptEditor.SelectedPromptItem;
                        // update the prompt in the list
                        selectedItem.Text = updatedPrompt.Title;
                        selectedItem.SubItems[1].Text = updatedPrompt.Description;
                        selectedItem.Tag = updatedPrompt;

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
                    txtSearch_TextChanged(this, EventArgs.Empty);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (promptsListView.SelectedItems.Count > 0)
            {
                var selectedItem = promptsListView.SelectedItems[0];
                var prompt = (PromptItem)selectedItem.Tag;

                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the prompt '{prompt.Title}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    _prompts.RemoveAll(p => p.Title == prompt.Title);
                    
                    // Update UI via search logic
                    txtSearch_TextChanged(this, EventArgs.Empty);

                    ErasePromptFromFile(prompt);

                    promptTextBox.Text = string.Empty;
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateCopyButtonState(false);
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
    }
}
