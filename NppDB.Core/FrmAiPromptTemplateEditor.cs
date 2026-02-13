using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NppDB.Core
{
    public partial class FrmAiPromptTemplateEditor : Form
    {
        private readonly string _templateFilePath;

        private static readonly string[] RequiredPlaceholders =
        {
            "{DATABASE_DIALECT}",
            "{SQL_QUERY}",
            "{ANALYSIS_ISSUES_WITH_DETAILS_LIST}"
        };

        private const string DefaultTemplate =
@"You are an expert {DATABASE_DIALECT} SQL developer and troubleshooter.
I need help understanding and fixing issues with my SQL query.

Here is the information:
1.  Database Dialect: {DATABASE_DIALECT}
2.  SQL Query:
{SQL_QUERY}
3.  Detected Issues (Errors/Warnings):
{ANALYSIS_ISSUES_WITH_DETAILS_LIST}

Please, for each issue detailed in point 3 above:
a. Explain what the feedback message means in the context of my query and the {DATABASE_DIALECT} dialect.
b. Identify the most likely cause(s) of this issue in my query.
c. Provide specific, corrected SQL code snippet(s) to resolve the issue, or indicate how the query structure should change for that specific issue.
d. If applicable, suggest any SQL best practices related to this problem to avoid it in the future.
";

        public FrmAiPromptTemplateEditor(string templateFilePath)
        {
            if (string.IsNullOrWhiteSpace(templateFilePath))
                throw new ArgumentException("Template file path cannot be empty.", nameof(templateFilePath));

            _templateFilePath = templateFilePath;

            InitializeComponent();
            LoadPlaceholders();
            LoadTemplate();
        }

        private void LoadTemplate()
        {
            lblPathValue.Text = _templateFilePath;

            try
            {
                if (File.Exists(_templateFilePath))
                {
                    txtTemplate.Text = File.ReadAllText(_templateFilePath);
                }
                else
                {
                    txtTemplate.Text = DefaultTemplate;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Failed to load AI prompt template:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtTemplate.Text = DefaultTemplate;
            }
        }

        private void LoadPlaceholders()
        {
            var placeholders = string.Join(", ", RequiredPlaceholders);
            lblPlaceholders.Text = "Required placeholders: " + placeholders;

            comboInsert.Items.Clear();
            foreach (var p in RequiredPlaceholders) comboInsert.Items.Add(p);
            if (comboInsert.Items.Count > 0) comboInsert.SelectedIndex = 0;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var ph = comboInsert.SelectedItem as string;
            if (string.IsNullOrEmpty(ph)) return;

            var start = txtTemplate.SelectionStart;
            txtTemplate.SelectedText = ph;
            txtTemplate.SelectionStart = start + ph.Length;
            txtTemplate.Focus();
        }

        private void btnRestoreDefault_Click(object sender, EventArgs e)
        {
            txtTemplate.Text = DefaultTemplate;
            txtTemplate.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var text = txtTemplate.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(this,
                    "Template cannot be empty.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var missing = RequiredPlaceholders.Where(p => !text.Contains(p)).ToArray();
            if (missing.Length > 0)
            {
                MessageBox.Show(this,
                    "Template is missing required placeholders:\n- " + string.Join("\n- ", missing),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                var dir = Path.GetDirectoryName(_templateFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(_templateFilePath, text, new UTF8Encoding(false));

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Failed to save AI prompt template:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
