using System;
using System.Windows.Forms;

namespace NppDB.Core
{
    public partial class frmAnalysisPromptDialog : Form
    {
        private readonly Action<string> _openLlmAction;
        private readonly Action _editTemplateAction;

        public string PromptText => txtPrompt.Text;

        public frmAnalysisPromptDialog(string promptText, Action<string> openLlmAction, Action editTemplateAction)
        {
            _openLlmAction = openLlmAction;
            _editTemplateAction = editTemplateAction;

            InitializeComponent();
            UiThemeManager.Register(this);

            txtPrompt.Text = promptText ?? string.Empty;
            btnOpenLlm.Enabled = _openLlmAction != null;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            CopyPromptToClipboard();
        }

        private void btnOpenLlm_Click(object sender, EventArgs e)
        {
            CopyPromptToClipboard();
            _openLlmAction?.Invoke(PromptText);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lnkTemplateHint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _editTemplateAction?.Invoke();
        }

        private void CopyPromptToClipboard()
        {
            if (!string.IsNullOrWhiteSpace(PromptText))
            {
                Clipboard.SetText(PromptText);
            }
        }
    }
}
