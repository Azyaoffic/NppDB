using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace NppDB.Core
{

    public class PromptPreferences // also in PostgreSQLPromptReading.cs
    {
        public string ResponseLanguage {get; set;}
        public string CustomInstructions {get; set;}
    }
    
    public partial class FrmPromptPreferences : Form
    {
        public static Dictionary<string, string> LanguageCodeDict = new Dictionary<string, string>();

        private string _selectedLanguage;
        private string _preferencesFilePath;
        public static string PreferencesFilePath; 
        
        public FrmPromptPreferences(string preferencesFilePath)
        {
            _preferencesFilePath = preferencesFilePath;
            
            
            InitializeComponent();
            
            // Load languages into combo box
            comboBoxResponseLanguage.Items.Clear();

            foreach (var lang in LanguageCodeDict.Values)
            {
                comboBoxResponseLanguage.Items.Add(lang);
            }

            // try to load existing file (json)
            if (File.Exists(_preferencesFilePath))
            {
                var readData = File.ReadAllText(_preferencesFilePath);
                if (!string.IsNullOrEmpty(readData))
                {
                    var value = JsonConvert.DeserializeObject<PromptPreferences>(readData);
                    if (value.ResponseLanguage != null)
                    {
                        comboBoxResponseLanguage.SelectedItem = value.ResponseLanguage;
                    }

                    if (value.CustomInstructions != null)
                    {
                        richTextBoxCustomInstructions.Text = value.CustomInstructions;
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var preferences = new PromptPreferences
            {
                ResponseLanguage = _selectedLanguage,
                CustomInstructions = richTextBoxCustomInstructions.Text
            };

            var jsonData = JsonConvert.SerializeObject(preferences, Formatting.Indented);
            File.WriteAllText(_preferencesFilePath, jsonData);

            MessageBox.Show("Preferences saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void comboBoxResponseLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedLanguage = comboBoxResponseLanguage.SelectedItem.ToString();
        }

        public static PromptPreferences ReadUserPreferences()
        {
            try
            {
                if (File.Exists(PreferencesFilePath))
                {
                    var readData = File.ReadAllText(PreferencesFilePath);
                    if (!string.IsNullOrEmpty(readData))
                    {
                        return JsonConvert.DeserializeObject<PromptPreferences>(readData);
                    }
                }

                return new PromptPreferences
                {
                    ResponseLanguage = "English",
                    CustomInstructions = ""
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading preferences: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return new PromptPreferences
                {
                    ResponseLanguage = "English",
                    CustomInstructions = ""
                };
            }
        }
    }
}