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

        private const int BasePreferredWidth = 1152;
        private const int BasePreferredHeight = 684;
        private const int BaseMinimumWidth = 1040;
        private const int BaseMinimumHeight = 620;
        private const int BaseScreenMargin = 60;
        private float _uiScale = 1f;

        public string PromptText => txtPrompt.Text;

        public frmAnalysisPromptDialog(string promptText, Action<string> openLlmAction, Action editTemplateAction)
        {
            _openLlmAction = openLlmAction;
            _editTemplateAction = editTemplateAction;

            InitializeComponent();
            _uiScale = GetUiScale();
            ApplyScaledWindowBounds();
            UiThemeManager.Register(this);

            _defaultCopyButtonText = btnCopy.Text;
            _defaultCopyButtonBackColor = btnCopy.BackColor;
            _defaultCopyButtonForeColor = btnCopy.ForeColor;

            SetPromptText(promptText ?? string.Empty);
            btnOpenLlm.Enabled = _openLlmAction != null;
        }

        private float GetUiScale()
        {
            try
            {
                using (var graphics = CreateGraphics())
                {
                    var scale = graphics.DpiY / 96f;
                    return scale > 0.1f ? scale : 1f;
                }
            }
            catch
            {
                return 1f;
            }
        }

        private int ScaleUi(int value)
        {
            return Math.Max(1, (int)Math.Round(value * _uiScale));
        }

        private void ApplyScaledWindowBounds()
        {
            var workingArea = Screen.FromControl(this).WorkingArea;
            var preferredWidth = Math.Min(workingArea.Width, Math.Max(ScaleUi(BaseMinimumWidth), Math.Min(ScaleUi(BasePreferredWidth), workingArea.Width - ScaleUi(BaseScreenMargin))));
            var preferredHeight = Math.Min(workingArea.Height, Math.Max(ScaleUi(BaseMinimumHeight), Math.Min(ScaleUi(BasePreferredHeight), workingArea.Height - ScaleUi(BaseScreenMargin))));

            MinimumSize = new Size(Math.Min(workingArea.Width, ScaleUi(BaseMinimumWidth)), Math.Min(workingArea.Height, ScaleUi(BaseMinimumHeight)));
            Size = new Size(preferredWidth, preferredHeight);
            StartPosition = FormStartPosition.CenterScreen;
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
