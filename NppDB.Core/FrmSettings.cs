using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NppDB.Core
{
    public partial class FrmSettings : Form
    {
        private NppDbSettings _settings;

        public enum SettingsTab
        {
            Behavior = 0,
            LlmResponse = 1,
            AiPromptTemplate = 2
        }
        
        // also needs to be updated in MSAccessTable.cs
        public class BehaviorSettings
        {
            public bool EnableDestructiveSelectInto { get; set; }
            public bool EnableNewTabCreation { get; set; }
            public bool EnableSqlAutocomplete { get; set; } = true;
            public float DbManagerFontScale { get; set; } = 1.0f;
            public string ThemeMode { get; set; } = "FollowNotepadPlusPlus";
        }
        
        public class PromptPreferences // also in PostgreSQLPromptReading.cs
        {
            public string ResponseLanguage {get; set;}
            public string CustomInstructions {get; set;}
            public string OpenLlmUrl { get; set; }
        }

        public static Dictionary<string, string> LanguageCodeDict = new Dictionary<string, string>();

        public FrmSettings(string settingsFilePath, SettingsTab initialTab = SettingsTab.Behavior)
        {
            NppDbSettingsStore.Initialize(settingsFilePath, null);

            InitializeComponent();
            UiThemeManager.Register(this);

            LoadLanguages();
            LoadSettingsToUi();

            tabControlMain.SelectedIndex = (int)initialTab;
        }

        private void LoadLanguages()
        {
            comboResponseLanguage.Items.Clear();

            if (LanguageCodeDict != null && LanguageCodeDict.Count > 0)
            {
                var sortedLanguages = LanguageCodeDict.Values.OrderBy(l => l).ToList();
                foreach (var lang in sortedLanguages)
                    comboResponseLanguage.Items.Add(lang);
            }
            else
            {
                comboResponseLanguage.Items.Add("English");
            }
        }

        private void LoadSettingsToUi()
        {
            _settings = NppDbSettingsStore.Get();

            // behavior settings
            chkEnableDestructiveSelectInto.Checked = _settings.Behavior != null && _settings.Behavior.EnableDestructiveSelectInto;
            chkEnableNewTabCreation.Checked = _settings.Behavior != null && _settings.Behavior.EnableNewTabCreation;
            chkEnableSqlAutocomplete.Checked = _settings.Behavior == null || _settings.Behavior.EnableSqlAutocomplete;

            var scale = (_settings.Behavior != null) ? _settings.Behavior.DbManagerFontScale : 1.0f;
            if (scale < 0.75f || scale > 2.5f) scale = 1.0f;
            numDbManagerFontScale.Value = (decimal)scale;
            
            var themeMode = (_settings.Behavior != null && !string.IsNullOrWhiteSpace(_settings.Behavior.ThemeMode))
                ? _settings.Behavior.ThemeMode
                : "FollowNotepadPlusPlus";

            if (comboThemeMode.Items.Contains(themeMode))
                comboThemeMode.SelectedItem = themeMode;
            else
                comboThemeMode.SelectedIndex = comboThemeMode.Items.Count > 0 ? 0 : -1;

            // llm preferences
            var lang = (_settings.Prompt != null && !string.IsNullOrWhiteSpace(_settings.Prompt.ResponseLanguage))
                ? _settings.Prompt.ResponseLanguage
                : "English";

            if (comboResponseLanguage.Items.Contains(lang))
                comboResponseLanguage.SelectedItem = lang;
            else
                comboResponseLanguage.SelectedIndex = comboResponseLanguage.Items.Count > 0 ? 0 : -1;

            txtOpenLlmUrl.Text = (_settings.Prompt != null && !string.IsNullOrWhiteSpace(_settings.Prompt.OpenLlmUrl))
                ? _settings.Prompt.OpenLlmUrl.Trim()
                : "https://chatgpt.com/";

            txtCustomInstructions.Text = (_settings.Prompt != null && _settings.Prompt.CustomInstructions != null)
                ? _settings.Prompt.CustomInstructions
                : string.Empty;

            // analysis template
            lblRequiredPlaceholders.Text = "Required placeholders: " + string.Join(", ", NppDbSettings.RequiredAiTemplatePlaceholders);

            comboInsertPlaceholder.Items.Clear();
            foreach (var p in NppDbSettings.RequiredAiTemplatePlaceholders)
                comboInsertPlaceholder.Items.Add(p);
            if (comboInsertPlaceholder.Items.Count > 0) comboInsertPlaceholder.SelectedIndex = 0;

            txtAiTemplate.Text = string.IsNullOrWhiteSpace(_settings.AiPromptTemplate)
                ? NppDbSettings.DefaultAiPromptTemplate
                : _settings.AiPromptTemplate;
        }

        private void btnInsertPlaceholder_Click(object sender, EventArgs e)
        {
            var ph = comboInsertPlaceholder.SelectedItem as string;
            if (string.IsNullOrEmpty(ph)) return;

            var start = txtAiTemplate.SelectionStart;
            txtAiTemplate.SelectedText = ph;
            txtAiTemplate.SelectionStart = start + ph.Length;
            txtAiTemplate.Focus();
        }
        
        private void btnCustomInstructionsInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Custom instructions let you guide how the AI responds in general.\n\n" +
                "You can use them to control:\n" +
                "- explanation length\n" +
                "- tone\n" +
                "- level of detail\n" +
                "- formatting style\n" +
                "- whether code should be minimal or more commented\n\n" +
                "Examples:\n" +
                "- Keep explanations concise and to the point.\n" +
                "- Explain things step by step for a beginner.\n" +
                "- Prefer practical examples over theory.\n" +
                "- When giving code, keep changes minimal.\n" +
                "- Use short answers unless more detail is requested.\n\n" +
                "These instructions are applied generally, so they should describe how you usually want responses to look.",
                "Custom instructions help",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnRestoreDefaultTemplate_Click(object sender, EventArgs e)
        {
            txtAiTemplate.Text = NppDbSettings.DefaultAiPromptTemplate;
            txtAiTemplate.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var openUrl = (txtOpenLlmUrl.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(openUrl))
            {
                MessageBox.Show(this, "URL must not be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControlMain.SelectedIndex = (int)SettingsTab.LlmResponse;
                txtOpenLlmUrl.Focus();
                return;
            }

            var templateText = txtAiTemplate.Text ?? string.Empty;
            var missing = NppDbSettings.RequiredAiTemplatePlaceholders.Where(p => !templateText.Contains(p)).ToArray();
            if (missing.Length > 0)
            {
                MessageBox.Show(this,
                    "AI prompt template is missing required placeholders:\n- " + string.Join("\n- ", missing),
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                tabControlMain.SelectedIndex = (int)SettingsTab.AiPromptTemplate;
                txtAiTemplate.Focus();
                return;
            }

            var selectedLang = comboResponseLanguage.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedLang))
                selectedLang = "English";

            if (_settings == null) _settings = new NppDbSettings();
            if (_settings.Behavior == null) _settings.Behavior = new BehaviorSettings();
            if (_settings.Prompt == null) _settings.Prompt = new PromptPreferences();

            _settings.Behavior.EnableDestructiveSelectInto = chkEnableDestructiveSelectInto.Checked;
            _settings.Behavior.EnableNewTabCreation = chkEnableNewTabCreation.Checked;
            _settings.Behavior.DbManagerFontScale = (float)numDbManagerFontScale.Value;
            _settings.Behavior.EnableSqlAutocomplete = chkEnableSqlAutocomplete.Checked;
            
            var selectedTheme = comboThemeMode.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTheme))
                selectedTheme = "FollowNotepadPlusPlus";
            _settings.Behavior.ThemeMode = selectedTheme;

            _settings.Prompt.ResponseLanguage = selectedLang;
            _settings.Prompt.CustomInstructions = txtCustomInstructions.Text ?? string.Empty;
            _settings.Prompt.OpenLlmUrl = openUrl;

            _settings.AiPromptTemplate = templateText;

            NppDbSettingsStore.Save(_settings);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}