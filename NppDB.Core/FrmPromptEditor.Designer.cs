using System.ComponentModel;

namespace NppDB.Core
{
    partial class FrmPromptEditor
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
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonDiscard = new System.Windows.Forms.Button();
            this.richTextBoxName = new System.Windows.Forms.RichTextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.richTextBoxPrompt = new System.Windows.Forms.RichTextBox();
            this.labelPromptId = new System.Windows.Forms.Label();
            this.labelPlaceholders = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 701);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(79, 44);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonDiscard
            // 
            this.buttonDiscard.Location = new System.Drawing.Point(709, 701);
            this.buttonDiscard.Name = "buttonDiscard";
            this.buttonDiscard.Size = new System.Drawing.Size(79, 44);
            this.buttonDiscard.TabIndex = 1;
            this.buttonDiscard.Text = "Discard";
            this.buttonDiscard.UseVisualStyleBackColor = true;
            this.buttonDiscard.Click += new System.EventHandler(this.buttonDiscard_Click);
            // 
            // richTextBoxName
            // 
            this.richTextBoxName.Location = new System.Drawing.Point(87, 12);
            this.richTextBoxName.Name = "richTextBoxName";
            this.richTextBoxName.Size = new System.Drawing.Size(701, 43);
            this.richTextBoxName.TabIndex = 2;
            this.richTextBoxName.Text = "";
            // 
            // labelName
            // 
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(12, 13);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(64, 41);
            this.labelName.TabIndex = 3;
            this.labelName.Text = "Name:";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBoxDescription
            // 
            this.richTextBoxDescription.Location = new System.Drawing.Point(113, 84);
            this.richTextBoxDescription.Name = "richTextBoxDescription";
            this.richTextBoxDescription.Size = new System.Drawing.Size(675, 70);
            this.richTextBoxDescription.TabIndex = 4;
            this.richTextBoxDescription.Text = "";
            // 
            // labelDescription
            // 
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(12, 100);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(100, 41);
            this.labelDescription.TabIndex = 5;
            this.labelDescription.Text = "Description:";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBoxPrompt
            // 
            this.richTextBoxPrompt.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxPrompt.Location = new System.Drawing.Point(11, 216);
            this.richTextBoxPrompt.Name = "richTextBoxPrompt";
            this.richTextBoxPrompt.Size = new System.Drawing.Size(775, 479);
            this.richTextBoxPrompt.TabIndex = 6;
            this.richTextBoxPrompt.Text = "";
            // 
            // labelPromptId
            // 
            this.labelPromptId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPromptId.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelPromptId.Location = new System.Drawing.Point(12, 172);
            this.labelPromptId.Name = "labelPromptId";
            this.labelPromptId.Size = new System.Drawing.Size(238, 41);
            this.labelPromptId.TabIndex = 7;
            this.labelPromptId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelPlaceholders
            // 
            this.labelPlaceholders.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlaceholders.Location = new System.Drawing.Point(332, 172);
            this.labelPlaceholders.Name = "labelPlaceholders";
            this.labelPlaceholders.Size = new System.Drawing.Size(455, 41);
            this.labelPlaceholders.TabIndex = 8;
            this.labelPlaceholders.Text = "No placeholders available";
            this.labelPlaceholders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmPromptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 757);
            this.Controls.Add(this.labelPlaceholders);
            this.Controls.Add(this.labelPromptId);
            this.Controls.Add(this.richTextBoxPrompt);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.richTextBoxDescription);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.richTextBoxName);
            this.Controls.Add(this.buttonDiscard);
            this.Controls.Add(this.buttonSave);
            this.Name = "FrmPromptEditor";
            this.ShowIcon = false;
            this.Text = "Prompt Editor";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label labelPlaceholders;

        private System.Windows.Forms.RichTextBox richTextBoxDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.RichTextBox richTextBoxPrompt;
        private System.Windows.Forms.Label labelPromptId;

        private System.Windows.Forms.Label labelName;

        private System.Windows.Forms.RichTextBox richTextBoxName;

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonDiscard;

        #endregion
    }
}