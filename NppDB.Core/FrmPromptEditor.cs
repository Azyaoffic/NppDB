using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NppDB.Core
{
    public partial class FrmPromptEditor : Form
    {
        public PromptItem SelectedPromptItem { get; private set; }
        
        private string _selectedTypePretty;

        public FrmPromptEditor()
        {
            InitializeComponent();
            LoadPromptTypes();
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public PromptItem CreatePromptItem()
        {
            var name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Prompt name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default;
            }

            var id = lblIdValue.Text.Trim();
            var description = txtDescription.Text.Trim();
            var promptText = txtPrompt.Text.Trim();

            if (string.IsNullOrEmpty(promptText))
            {
                MessageBox.Show("Prompt text cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default;
            }
            
            // find placeholders using {{placeholder}} syntax
            var placeholders = new List<PromptPlaceholder>();
            const string pattern = @"\{\{(.*?)\}\}";
            var matches = Regex.Matches(promptText, pattern);
            foreach (Match match in matches)
            {
                var placeholderName = match.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(placeholderName))
                {
                    placeholders.Add(new PromptPlaceholder { Name = placeholderName, IsEditable = true });
                }
            }
            
            string type;
            
            var selectedPretty = _selectedTypePretty ?? comboBoxType.SelectedItem?.ToString();
            switch (selectedPretty)
            {
                case "Prompt Library Prompt":
                    type = "LlmPrompt";
                    break;
                case "Table Prompt":
                    type = "TablePrompt";
                    break;
                default:
                    MessageBox.Show("Please select a valid prompt type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return default;
            }

            return new PromptItem
            {
                Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id,
                Title = name,
                Description = description,
                Type = type,
                Text = promptText,
                Placeholders = placeholders.ToArray()
            };
        }
        
        public void LoadPromptItem(PromptItem promptItem)
        {
            lblIdValue.Text = promptItem.Id;
            txtName.Text = promptItem.Title;
            txtDescription.Text = promptItem.Description;
            txtPrompt.Text = promptItem.Text;

            var type = (promptItem.Type ?? string.Empty).Trim();
            string pretty = null;

            if (type.Equals("LlmPrompt", StringComparison.OrdinalIgnoreCase))
                pretty = "Prompt Library Prompt";
            else if (type.Equals("TablePrompt", StringComparison.OrdinalIgnoreCase))
                pretty = "Table Prompt";

            if (pretty != null)
            {
                comboBoxType.SelectedItem = pretty;
                _selectedTypePretty = pretty;
            }
            else
            {
                comboBoxType.SelectedIndex = -1;
                _selectedTypePretty = null;
            }
        }


        public void LoadPlaceholders(List<string> placeholders)
        {
            if (placeholders == null || placeholders.Count == 0)
            {
                lblPlaceholders.Text = "No system placeholders available.";
                return;
            }

            StringBuilder sb = new StringBuilder("System Placeholders: ");
            sb.Append(string.Join(", ", placeholders.Select(p => $"{{{{{p}}}}}")));
            lblPlaceholders.Text = sb.ToString();
        }
        
        private void LoadPromptTypes()
        {
            comboBoxType.Items.Clear();
            comboBoxType.Items.Add("Prompt Library Prompt");
            comboBoxType.Items.Add("Table Prompt");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var promptItem = CreatePromptItem();
            if (string.IsNullOrEmpty(promptItem.Title)) return;

            SelectedPromptItem = promptItem;
            SaveNewPromptToFile(promptItem);
            DialogResult = DialogResult.OK;
            Close();
        }

        public static void SaveNewPromptToFile(PromptItem promptItem)
        {
            string promptLibraryPath = FrmPromptLibrary.PromptLibraryPath;
            if (!File.Exists(promptLibraryPath))
            {
                throw new FileNotFoundException("Prompt library file not found.", promptLibraryPath);
            }
            
            XDocument doc = XDocument.Load(promptLibraryPath);
            XElement root = doc.Element("Prompts");
            if (root == null) return;
            
            var exists = false;
            
            // find if prompt with same name exists
            foreach (XElement prompt in root.Elements("Prompt"))
            {
                if (prompt.Element("Id")?.Value == promptItem.Id)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                var prompt = root.Elements("Prompt")
                    .FirstOrDefault(p => (string) p.Element("Id") == promptItem.Id);
                if (prompt == null) throw new FileNotFoundException("Edited prompt not found but should've been!", promptLibraryPath);
                
                prompt.SetElementValue("Description", promptItem.Description);
                prompt.SetElementValue("Text", promptItem.Text);
                prompt.SetElementValue("Title", promptItem.Title);
                
                prompt.SetAttributeValue("type", promptItem.Type);
                
                var existingPlaceholders = prompt.Element("Placeholders");
                // check whether existing placeholders are editable
                Dictionary<string, bool> existingPlaceholderDict = new Dictionary<string, bool>();
                
                if (existingPlaceholders != null)
                {
                    foreach (XElement placeholder in existingPlaceholders.Elements("Placeholder"))
                    {
                        var name = placeholder.Attribute("name")?.Value;
                        var isEditable = placeholder.Attribute("editable")?.Value == "true";
                        if (!string.IsNullOrEmpty(name) && !isEditable)
                        {
                            // if not editable, keep it
                            existingPlaceholderDict[name] = false;
                        }
                    }
                }
                
                // create new placeholders element
                List<XElement> placeholderElements = new List<XElement>();
                foreach (var p in promptItem.Placeholders)
                {
                    if (existingPlaceholderDict.ContainsKey(p.Name) && !existingPlaceholderDict[p.Name])
                    {
                        // keep non-editable placeholder
                        placeholderElements.Add(new XElement("Placeholder", new XAttribute("name", p.Name)));
                    }
                    else
                    {
                        // add new editable placeholder
                        placeholderElements.Add(new XElement("Placeholder", new XAttribute("name", p.Name), new XAttribute("editable", "true")));
                    }
                }
                
                XElement placeholdersElement = new XElement("Placeholders", placeholderElements);
                
                // add new placeholders
                prompt.Element("Placeholders")?.ReplaceWith(placeholdersElement);

                doc.Save(promptLibraryPath);
            }
            else
            {
                XElement promptElement = new XElement("Prompt",
                    new XAttribute("type", promptItem.Type),
                    new XElement("Id", promptItem.Id),
                    new XElement("Title", promptItem.Title),
                    new XElement("Description", promptItem.Description),
                    new XElement("Text", promptItem.Text),
                    new XElement("Placeholders",
                        new List<XElement>(
                            Array.ConvertAll(promptItem.Placeholders, p =>
                                new XElement("Placeholder", new XAttribute("name", p.Name), new XAttribute("editable", "true"))
                            )
                        )
                    )
                );
                root.Add(promptElement);
                doc.Save(promptLibraryPath);
            }
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedTypePretty = comboBoxType.SelectedItem?.ToString();
        }
    }
}