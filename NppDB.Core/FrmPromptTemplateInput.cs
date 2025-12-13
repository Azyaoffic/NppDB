using System.Windows.Forms;

namespace NppDB.Core
{
    public partial class FrmPromptTemplateInput : Form
    {
        private readonly TextBoxBase _textBox;
        private readonly Button _okButton;
        private readonly Button _cancelButton;
        
        public string InputText => _textBox.Text;


        public FrmPromptTemplateInput(string title, string prompt, string initialValue = "", bool isRichText = false)
        {
            Text = title;
            Width = 400;
            Height = isRichText ? 180 : 150;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var label = new Label
            {
                Text = prompt,
                Left = 10,
                Top = 10,
                AutoSize = true
            };
            Controls.Add(label);

            if (isRichText)
            {
                _textBox = new RichTextBox
                {
                    Left = 10,
                    Top = 30,
                    Width = 360,
                    Height = 50,
                    Text = initialValue
                };
            }
            else
            {
                _textBox = new TextBox
                {
                    Left = 10,
                    Top = 30,
                    Width = 360,
                    Text = initialValue
                };
            }

            Controls.Add(_textBox);
            
            // button height depends on textbox type
            var buttonTop = isRichText ? 90 : 65;

            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 210,
                Top = buttonTop,
                Width = 75
            };
            Controls.Add(_okButton);

            _cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 295,
                Top = buttonTop,
                Width = 75
            };
            Controls.Add(_cancelButton);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }
    }
}