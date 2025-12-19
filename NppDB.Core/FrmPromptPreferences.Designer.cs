using System.ComponentModel;

namespace NppDB.Core
{
    partial class FrmPromptPreferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelResponseLanguage = new System.Windows.Forms.Label();
            this.comboBoxResponseLanguage = new System.Windows.Forms.ComboBox();
            this.labelCustomInstructions = new System.Windows.Forms.Label();
            this.richTextBoxCustomInstructions = new System.Windows.Forms.RichTextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelResponseLanguage
            // 
            this.labelResponseLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResponseLanguage.Location = new System.Drawing.Point(8, 35);
            this.labelResponseLanguage.Name = "labelResponseLanguage";
            this.labelResponseLanguage.Size = new System.Drawing.Size(168, 21);
            this.labelResponseLanguage.TabIndex = 0;
            this.labelResponseLanguage.Text = "Response Language";
            // 
            // comboBoxResponseLanguage
            // 
            this.comboBoxResponseLanguage.FormattingEnabled = true;
            this.comboBoxResponseLanguage.Location = new System.Drawing.Point(182, 37);
            this.comboBoxResponseLanguage.Name = "comboBoxResponseLanguage";
            this.comboBoxResponseLanguage.Size = new System.Drawing.Size(217, 21);
            this.comboBoxResponseLanguage.TabIndex = 1;
            this.comboBoxResponseLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxResponseLanguage_SelectedIndexChanged);
            // 
            // labelCustomInstructions
            // 
            this.labelCustomInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCustomInstructions.Location = new System.Drawing.Point(8, 82);
            this.labelCustomInstructions.Name = "labelCustomInstructions";
            this.labelCustomInstructions.Size = new System.Drawing.Size(190, 22);
            this.labelCustomInstructions.TabIndex = 2;
            this.labelCustomInstructions.Text = "Custom Instructions";
            // 
            // richTextBoxCustomInstructions
            // 
            this.richTextBoxCustomInstructions.Location = new System.Drawing.Point(8, 107);
            this.richTextBoxCustomInstructions.Name = "richTextBoxCustomInstructions";
            this.richTextBoxCustomInstructions.Size = new System.Drawing.Size(391, 232);
            this.richTextBoxCustomInstructions.TabIndex = 3;
            this.richTextBoxCustomInstructions.Text = "";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(8, 345);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(390, 50);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FrmPromptPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 398);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.richTextBoxCustomInstructions);
            this.Controls.Add(this.labelCustomInstructions);
            this.Controls.Add(this.comboBoxResponseLanguage);
            this.Controls.Add(this.labelResponseLanguage);
            this.Name = "FrmPromptPreferences";
            this.ShowIcon = false;
            this.Text = "LLM Response Preferences";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button buttonSave;

        private System.Windows.Forms.Label labelCustomInstructions;
        private System.Windows.Forms.RichTextBox richTextBoxCustomInstructions;

        private System.Windows.Forms.ComboBox comboBoxResponseLanguage;

        private System.Windows.Forms.Label labelResponseLanguage;

        #endregion
    }
}