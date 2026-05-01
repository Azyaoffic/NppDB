using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NppDB.Core
{
    public static class DiagnosticMessageBox
    {
        private static readonly Regex UrlRegex = new Regex(@"https?://\S+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void Show(IWin32Window owner, string message, string title, MessageBoxIcon icon)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(string.Empty, title, MessageBoxButtons.OK, icon);
                return;
            }

            var urlMatch = UrlRegex.Match(message);
            if (!urlMatch.Success)
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
                return;
            }

            var url = urlMatch.Value.Trim();
            var text = message.Replace(urlMatch.Value, string.Empty).Trim();
            text = Regex.Replace(text, @"(?:\r?\n){2,}Download:\s*$", string.Empty, RegexOptions.IgnoreCase).Trim();

            var prompt = string.Format("{0}{1}{1}Open download page now?", text, Environment.NewLine);
            var result = MessageBox.Show(prompt, title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                OpenUrl(url);
            }
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
            }
        }
    }
}