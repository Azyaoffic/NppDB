using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace NppDB.Core
{
    public class BehaviorSettings
    {
        public bool EnableDestructiveSelectInto { get; set; }
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
                var readData = File.ReadAllText(_preferencesFilePath);
                if (!string.IsNullOrEmpty(readData))
                {
                    var value = JsonConvert.DeserializeObject<BehaviorSettings>(readData);
                    return value;
                }
            }

            return new BehaviorSettings();
        }
    }
}