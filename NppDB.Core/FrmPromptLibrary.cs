using System;
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
        public FrmPromptLibrary()
        {
            InitializeComponent();
            
            promptsListView.View = View.Details;
            
            promptsListView.Columns.Clear();
            promptsListView.Columns.Add("Prompt Name", 200);
            promptsListView.Columns.Add("Description", 300);
            
            
        }

    }
}