using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            placeholderListView.View = View.Details;

            if (_prompts.Count > 0)
            {
                noPromptsFoundLabel.Visible = false;
            }

            promptsListView.Columns.Clear();
            promptsListView.Columns.Add("Prompt Name", 200);
            promptsListView.Columns.Add("Description", 300);

            placeholderListView.Columns.Clear();
            placeholderListView.Columns.Add("Placeholder", 150);
            placeholderListView.Columns.Add("Value", 300);

            foreach (var prompt in _prompts)
            {
                var item = new ListViewItem(prompt.Title);
                item.SubItems.Add(prompt.Description);
                item.Tag = prompt;
                promptsListView.Items.Add(item);
            }
        }

        public static void SetPrompts(List<PromptItem> promptItems)
        {
            _prompts = promptItems;
        }

        private void promptsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (promptsListView.SelectedItems.Count > 0)
            {
                // main selection
                var selectedItem = promptsListView.SelectedItems[0];
                var prompt = (PromptItem)selectedItem.Tag;
                if (disableTemplatingCheckbox.Checked)
                {
                    promptTextBox.Text = ConstructPromptPreview(prompt.Text);
                }
                else
                {
                    promptTextBox.Text = ConstructPromptPreview(SubstitutePlaceholders(prompt.Text));
                }

                // placeholders
                placeholderListView.Items.Clear();
                foreach (var placeholder in prompt.Placeholders)
                {
                    var item = new ListViewItem(placeholder.Name)
                    {
                        Tag = placeholder
                    };
                    try
                    {
                        item.SubItems.Add(Placeholders[placeholder.Name]);
                        if (!placeholder.IsEditable)
                        {
                            item.BackColor = Color.Gray;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        if (placeholder.IsEditable)
                        {
                            item.SubItems.Add(string.Empty);
                        }
                        else
                        {
                            item.SubItems.Add("<no value can be assigned>");
                            item.BackColor = Color.Gray;
                        }
                    }

                    placeholderListView.Items.Add(item);
                }
            }
            else
            {
                promptTextBox.Text = string.Empty;
            }
        }

        private string SubstitutePlaceholders(string text)
        {
            /*
             * Supported default placeholders:
             * * {selected_sql} - the currently selected SQL in the editor
             */
            var selectedSql = Placeholders.TryGetValue("selected_sql", out var plh)
                ? plh
                : string.Empty;

            text = text.Replace("{{selected_sql}}", selectedSql);
            
            // Custom placeholders
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
            promptsListView_SelectedIndexChanged(this, EventArgs.Empty);
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
                        selectedItem.SubItems[0].Text = updatedPrompt.Title;
                        selectedItem.SubItems[1].Text = updatedPrompt.Description;
                        selectedItem.Tag = updatedPrompt;

                        // refresh the display
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

                    var item = new ListViewItem(newPrompt.Title);
                    item.SubItems.Add(newPrompt.Description);
                    item.Tag = newPrompt;
                    promptsListView.Items.Add(item);

                    noPromptsFoundLabel.Visible = false;
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
                    // remove from the in-memory list
                    _prompts.RemoveAll(p => p.Title == prompt.Title);

                    // remove from the ListView
                    promptsListView.Items.Remove(selectedItem);

                    // remove from the file
                    ErasePromptFromFile(prompt);

                    // Clear the preview and placeholders
                    promptTextBox.Text = string.Empty;
                    placeholderListView.Items.Clear();

                    if (_prompts.Count == 0)
                    {
                        noPromptsFoundLabel.Visible = true;
                    }
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

        private void placeholderListView_DoubleClick(object sender, EventArgs e)
        {
            if (placeholderListView.SelectedItems.Count > 0)
            {
                var item = placeholderListView.SelectedItems[0];

                if (item.BackColor == Color.Gray) return;
                
                var placeholderName = item.Text;
                var currentValue = item.SubItems.Count > 1 ? item.SubItems[1].Text : string.Empty;
                
                var placeholder = (PromptPlaceholder)item.Tag;
                var isRichText = placeholder.IsRichText;


                using (var inputDialog = new FrmPromptTemplateInput(
                           "Edit Placeholder",
                           $"Enter value for {{{{{placeholderName}}}}}:",
                           currentValue,
                           isRichText))
                {
                    if (inputDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        var newValue = inputDialog.InputText;

                        // Update ListView displayed value
                        if (item.SubItems.Count > 1)
                            item.SubItems[1].Text = newValue;
                        else
                            item.SubItems.Add(newValue);

                        Placeholders[placeholderName] = newValue;

                        // Refresh prompt preview
                        if (promptsListView.SelectedItems.Count > 0)
                        {
                            var selectedPrompt = (PromptItem)promptsListView.SelectedItems[0].Tag;
                            promptTextBox.Text = disableTemplatingCheckbox.Checked
                                ? selectedPrompt.Text
                                : SubstitutePlaceholders(selectedPrompt.Text);
                        }
                    }
                }

            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(promptTextBox.Text))
            {
                Clipboard.SetText(promptTextBox.Text);
                buttonCopy.BackColor = Color.LightGreen;
                var timer = new Timer { Interval = 500 };
                timer.Tick += (s, args) =>
                {
                    buttonCopy.BackColor = SystemColors.Control;
                    timer.Stop();
                };
                timer.Start();
            }
        }
    }
}