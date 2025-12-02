using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NppDB.Core
{
    
    public struct PromptPlaceholder
    {
        public string Name;
        public string Description;
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
        
        public FrmPromptLibrary()
        {
            InitializeComponent();
            
            promptsListView.View = View.Details;
            
            if (_prompts.Count > 0)
            {
                noPromptsFoundLabel.Visible = false;
            }
            
            promptsListView.Columns.Clear();
            promptsListView.Columns.Add("Prompt Name", 200);
            promptsListView.Columns.Add("Description", 300);
            
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
                var selectedItem = promptsListView.SelectedItems[0];
                var prompt = (PromptItem)selectedItem.Tag;
                promptTextBox.Text = prompt.Text;
            }
            else
            {
                promptTextBox.Text = string.Empty;
            }
        }
    }
}