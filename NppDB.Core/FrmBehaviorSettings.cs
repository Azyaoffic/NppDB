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
    }

    public partial class FrmBehaviorSettings : Form
    {
        private string _preferencesFilePath;

        public FrmBehaviorSettings(String preferencesFilePath)
        {
            _preferencesFilePath = preferencesFilePath;
            
            InitializeComponent();
            
            var existingSettings = loadExistingSettings();
            destructiveSelectIntoCheckbox.Checked = existingSettings.EnableDestructiveSelectInto;
            newTabCheckbox.Checked = existingSettings.EnableNewTabCreation;
        }

        private void destructiveSelectIntoCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var settings = new BehaviorSettings
            {
                EnableDestructiveSelectInto = destructiveSelectIntoCheckbox.Checked
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
                        return value;
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

        private void newTabCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var settings = new BehaviorSettings
            {
                EnableNewTabCreation = newTabCheckbox.Checked
            };

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);

            File.WriteAllText(_preferencesFilePath, json);
        }
    }
}