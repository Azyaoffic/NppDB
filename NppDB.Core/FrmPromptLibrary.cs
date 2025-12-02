using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NppDB.Core
{
    public struct PromptPlaceholder
    {
        public string Name;
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
        private readonly Func<string> _getSelectedSql;
        
        public static readonly Dictionary<string, string> Placeholders = new Dictionary<string, string>();

        public FrmPromptLibrary(Func<string> getSelectedSql)
        {
            _getSelectedSql = getSelectedSql;

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
                    promptTextBox.Text = prompt.Text;
                }
                else
                {
                    promptTextBox.Text = SubstitutePlaceholders(prompt.Text);
                }

                // placeholders
                placeholderListView.Items.Clear();
                foreach (var placeholder in prompt.Placeholders)
                {
                    var item = new ListViewItem(placeholder.Name);
                    try
                    {
                        item.SubItems.Add(Placeholders[placeholder.Name]);
                    } catch (KeyNotFoundException)
                    {
                        item.SubItems.Add("<this placeholder is invalid>");
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
             * Supported placeholders:
             * * {selected_sql} - the currently selected SQL in the editor
             */
            var selectedSql = _getSelectedSql();
            text = text.Replace("{{selected_sql}}", selectedSql);
            return text;
        }

        private void disableTemplatingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            promptsListView_SelectedIndexChanged(this, EventArgs.Empty);
        }
    }
}