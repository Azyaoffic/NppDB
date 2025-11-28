using System.ComponentModel;

namespace NppDB.Core
{
    partial class FrmPromptLibrary
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
            this.promptsListView = new System.Windows.Forms.ListView();
            this.promptTextbox = new System.Windows.Forms.RichTextBox();
            this.promptPreviewLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // promptsListView
            // 
            this.promptsListView.HideSelection = false;
            this.promptsListView.Location = new System.Drawing.Point(12, 12);
            this.promptsListView.Name = "promptsListView";
            this.promptsListView.Size = new System.Drawing.Size(367, 426);
            this.promptsListView.TabIndex = 0;
            this.promptsListView.UseCompatibleStateImageBehavior = false;
            // 
            // promptTextbox
            // 
            this.promptTextbox.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptTextbox.Location = new System.Drawing.Point(386, 79);
            this.promptTextbox.Name = "promptTextbox";
            this.promptTextbox.ReadOnly = true;
            this.promptTextbox.Size = new System.Drawing.Size(403, 359);
            this.promptTextbox.TabIndex = 1;
            this.promptTextbox.Text = "";
            // 
            // promptPreviewLabel
            // 
            this.promptPreviewLabel.Font = new System.Drawing.Font("Times New Roman", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptPreviewLabel.Location = new System.Drawing.Point(385, 12);
            this.promptPreviewLabel.Name = "promptPreviewLabel";
            this.promptPreviewLabel.Size = new System.Drawing.Size(403, 63);
            this.promptPreviewLabel.TabIndex = 2;
            this.promptPreviewLabel.Text = "Prompt Preview";
            this.promptPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmPromptLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.promptPreviewLabel);
            this.Controls.Add(this.promptTextbox);
            this.Controls.Add(this.promptsListView);
            this.Name = "FrmPromptLibrary";
            this.ShowIcon = false;
            this.Text = "Prompt Library";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.RichTextBox promptTextbox;
        private System.Windows.Forms.Label promptPreviewLabel;

        private System.Windows.Forms.ListView promptsListView;

        #endregion
    }
}