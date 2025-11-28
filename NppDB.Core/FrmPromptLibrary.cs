using System;
using System.Windows.Forms;

namespace NppDB.Core
{
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