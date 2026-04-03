using System;
using System.Drawing;
using System.Windows.Forms;

namespace NppDB.Core
{
    public partial class frmAnalysisPromptDialog : Form
    {
        private readonly Action<string> _openLlmAction;
        private readonly Action _editTemplateAction;
        private readonly string _defaultCopyButtonText;
        private readonly Color _defaultCopyButtonBackColor;
        private readonly Color _defaultCopyButtonForeColor;

        public string PromptText => txtPrompt.Text;

        public frmAnalysisPromptDialog(string promptText, Action<string> openLlmAction, Action editTemplateAction)
        {
            _openLlmAction = openLlmAction;
            _editTemplateAction = editTemplateAction;

            InitializeComponent();
            UiThemeManager.Register(this);

            _defaultCopyButtonText = btnCopy.Text;
            _defaultCopyButtonBackColor = btnCopy.BackColor;
            _defaultCopyButtonForeColor = btnCopy.ForeColor;

            SetPromptText(promptText ?? string.Empty);
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

        private void SetPromptText(string text)
        {
            txtPrompt.SuspendLayout();
            try
            {
                txtPrompt.Clear();
                txtPrompt.Text = text;
                txtPrompt.SelectAll();
                txtPrompt.SelectionFont = txtPrompt.Font;
                txtPrompt.SelectionColor = UiThemeManager.Current.IsDark
                    ? UiThemeManager.Current.Text
                    : SystemColors.ControlText;
                txtPrompt.SelectionStart = 0;
                txtPrompt.SelectionLength = 0;
                txtPrompt.ScrollToCaret();
            }
            finally
            {
                txtPrompt.ResumeLayout();
            }
        }

        private void CopyPromptToClipboard()
        {
            if (string.IsNullOrWhiteSpace(PromptText))
                return;

            Clipboard.SetText(PromptText);
            ShowCopyFeedback();
        }

        private void ShowCopyFeedback()
        {
            var pal = UiThemeManager.Current;

            btnCopy.BackColor = pal.IsDark ? pal.SofterBackground : Color.DarkBlue;
            btnCopy.ForeColor = pal.IsDark ? pal.Text : Color.White;
            btnCopy.Text = "Copied!";

            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, args) =>
            {
                ResetCopyButtonState();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ResetCopyButtonState()
        {
            btnCopy.Text = _defaultCopyButtonText;
            btnCopy.BackColor = _defaultCopyButtonBackColor;
            btnCopy.ForeColor = _defaultCopyButtonForeColor;
        }
    }
}
