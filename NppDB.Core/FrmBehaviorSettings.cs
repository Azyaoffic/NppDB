using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace NppDB.Core
{
    // also needs to be updated in MSAccessTable.cs
    public class BehaviorSettings
    {
        public bool EnableDestructiveSelectInto { get; set; }
        public bool EnableNewTabCreation { get; set; }
        public float DbManagerFontScale { get; set; } = 1.0f;
    }

    public partial class FrmBehaviorSettings : Form
    {
        private string _preferencesFilePath;
        private bool _loading;

        public FrmBehaviorSettings(string preferencesFilePath)
        {
            _preferencesFilePath = preferencesFilePath;

            InitializeComponent();

            _loading = true;
            try
            {
                var existingSettings = loadExistingSettings();
                destructiveSelectIntoCheckbox.Checked = existingSettings.EnableDestructiveSelectInto;
                newTabCheckbox.Checked = existingSettings.EnableNewTabCreation;

                var scale = existingSettings.DbManagerFontScale;
                if (scale < 0.75f || scale > 2.5f) scale = 1.0f;
                numDbManagerFontScale.Value = (decimal)scale;
            }
            finally
            {
                _loading = false;
            }
        }

        private void destructiveSelectIntoCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            SaveCurrentSettings();
        }

        private void newTabCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            SaveCurrentSettings();
        }

        private void numDbManagerFontScale_ValueChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            SaveCurrentSettings();
        }

        private void SaveCurrentSettings()
        {
            var settings = new BehaviorSettings
            {
                EnableDestructiveSelectInto = destructiveSelectIntoCheckbox.Checked,
                EnableNewTabCreation = newTabCheckbox.Checked,
                DbManagerFontScale = (float)numDbManagerFontScale.Value
            };

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_preferencesFilePath, json);
        }

        private BehaviorSettings loadExistingSettings()
        {
            if (File.Exists(_preferencesFilePath))
            {
                try
                {
                    var readData = File.ReadAllText(_preferencesFilePath);
                    if (!string.IsNullOrEmpty(readData))
                    {
                        var value = JsonConvert.DeserializeObject<BehaviorSettings>(readData);
                        return value ?? new BehaviorSettings();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new BehaviorSettings
                    {
                        EnableDestructiveSelectInto = false,
                        EnableNewTabCreation = false
                    };
                }

            }

            return new BehaviorSettings();
        }
    }
}