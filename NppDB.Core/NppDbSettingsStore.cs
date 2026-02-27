using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using BehaviorSettings = NppDB.Core.FrmSettings.BehaviorSettings;
using PromptPreferences = NppDB.Core.FrmSettings.PromptPreferences;

namespace NppDB.Core
{
    public class NppDbSettings
    {
        public BehaviorSettings Behavior { get; set; }
        public PromptPreferences Prompt { get; set; }
        public string AiPromptTemplate { get; set; }

        public static readonly string[] RequiredAiTemplatePlaceholders =
        {
            "{DATABASE_DIALECT}",
            "{SQL_QUERY}",
            "{ANALYSIS_ISSUES_WITH_DETAILS_LIST}"
        };

        public const string DefaultAiPromptTemplate = @"**Role**
You are an expert {DATABASE_DIALECT} SQL developer and troubleshooter.

**Task**
- Diagnose and fix problems in the provided SQL query using the detected issues list.
- For each issue, explain what it means, why it happens, and how to fix it.

**Constraints**
- Use the {DATABASE_DIALECT} dialect only.
- Preserve the query’s intended result; prefer minimal, targeted changes.
- If critical context is missing (schema, sample data, constraints), state assumptions explicitly.
- Any SQL you propose must be valid and runnable for this dialect.

**Response**
For each issue (in the same order as provided), output:
1) Meaning (1–3 sentences)
2) Likely cause(s) (bullets)
3) Fix (final SQL or snippet in one `sql` code block)
4) Best-practice note (optional, 1–2 bullets)

**Database Dialect**
`{DATABASE_DIALECT}`

**SQL Query**
```sql
{SQL_QUERY}
```
**Detected Issues**
```
{ANALYSIS_ISSUES_WITH_DETAILS_LIST}
```";

        public static NppDbSettings CreateDefault()
        {
            return new NppDbSettings
            {
                Behavior = new BehaviorSettings
                {
                    EnableDestructiveSelectInto = false,
                    EnableNewTabCreation = false,
                    DbManagerFontScale = 1.0f
                },
                Prompt = new PromptPreferences
                {
                    ResponseLanguage = "English",
                    CustomInstructions = "",
                    OpenLlmUrl = "https://chatgpt.com/"
                },
                AiPromptTemplate = DefaultAiPromptTemplate
            };
        }

        public void EnsureDefaults()
        {
            if (Behavior == null) Behavior = new BehaviorSettings();
            if (Prompt == null) Prompt = new PromptPreferences();

            if (Behavior.DbManagerFontScale < 0.75f || Behavior.DbManagerFontScale > 2.0f)
                Behavior.DbManagerFontScale = 1.0f;

            if (string.IsNullOrWhiteSpace(Prompt.ResponseLanguage))
                Prompt.ResponseLanguage = "English";
            if (Prompt.CustomInstructions == null)
                Prompt.CustomInstructions = "";
            if (string.IsNullOrWhiteSpace(Prompt.OpenLlmUrl))
                Prompt.OpenLlmUrl = "https://chatgpt.com/";

            if (string.IsNullOrWhiteSpace(AiPromptTemplate))
                AiPromptTemplate = DefaultAiPromptTemplate;
        }
    }

    public static class NppDbSettingsStore
    {
        private static readonly object _lock = new object();

        private static bool _initialized;
        private static string _settingsFilePath;
        private static string _configDir;

        private static NppDbSettings _cached;

        public static void Initialize(string settingsFilePath, string nppDbConfigDir)
        {
            lock (_lock)
            {
                _settingsFilePath = settingsFilePath;
                _configDir = nppDbConfigDir;
                _initialized = true;

                if (string.IsNullOrWhiteSpace(_settingsFilePath))
                    throw new ArgumentException("settingsFilePath cannot be empty.", nameof(settingsFilePath));

                if (string.IsNullOrWhiteSpace(_configDir))
                    _configDir = Path.GetDirectoryName(_settingsFilePath);

                if (!File.Exists(_settingsFilePath))
                {
                    _cached = NppDbSettings.CreateDefault();
                    TryWriteToDisk(_cached);
                }
                else
                {
                    _cached = TryReadFromDisk() ?? NppDbSettings.CreateDefault();
                    _cached.EnsureDefaults();
                }
            }
        }

        public static NppDbSettings Get()
        {
            EnsureInitialized();

            lock (_lock)
            {
                if (_cached == null)
                {
                    _cached = TryReadFromDisk() ?? NppDbSettings.CreateDefault();
                    _cached.EnsureDefaults();
                }

                return DeepClone(_cached);
            }
        }

        public static void Save(NppDbSettings settings)
        {
            EnsureInitialized();

            if (settings == null) settings = NppDbSettings.CreateDefault();
            settings.EnsureDefaults();

            lock (_lock)
            {
                TryWriteToDisk(settings);
                _cached = DeepClone(settings);
            }
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            throw new InvalidOperationException("NppDbSettingsStore.Initialize(...) must be called first.");
        }

        private static NppDbSettings TryReadFromDisk()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return null;

                var json = File.ReadAllText(_settingsFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                var value = JsonConvert.DeserializeObject<NppDbSettings>(json);
                if (value == null) return null;

                value.EnsureDefaults();
                return value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error reading settings.json:\n" + ex.Message,
                    "NppDB Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return null;
            }
        }

        private static void TryWriteToDisk(NppDbSettings settings)
        {
            try
            {
                var dir = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsFilePath, json, new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error writing settings.json:\n" + ex.Message,
                    "NppDB Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static NppDbSettings DeepClone(NppDbSettings settings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.None);
                return JsonConvert.DeserializeObject<NppDbSettings>(json) ?? NppDbSettings.CreateDefault();
            }
            catch
            {
                return NppDbSettings.CreateDefault();
            }
        }
    }
}