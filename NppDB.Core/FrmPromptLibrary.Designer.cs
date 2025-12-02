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
            this.promptTextBox = new System.Windows.Forms.RichTextBox();
            this.promptPreviewLabel = new System.Windows.Forms.Label();
            this.noPromptsFoundLabel = new System.Windows.Forms.Label();
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
            this.promptsListView.SelectedIndexChanged += new System.EventHandler(this.promptsListView_SelectedIndexChanged);
            // 
            // promptTextBox
            // 
            this.promptTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptTextBox.Location = new System.Drawing.Point(397, 66);
            this.promptTextBox.Name = "promptTextBox";
            this.promptTextBox.ReadOnly = true;
            this.promptTextBox.Size = new System.Drawing.Size(403, 384);
            this.promptTextBox.TabIndex = 1;
            this.promptTextBox.Text = "";
            // 
            // promptPreviewLabel
            // 
            this.promptPreviewLabel.Font = new System.Drawing.Font("Times New Roman", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptPreviewLabel.Location = new System.Drawing.Point(397, 8);
            this.promptPreviewLabel.Name = "promptPreviewLabel";
            this.promptPreviewLabel.Size = new System.Drawing.Size(403, 55);
            this.promptPreviewLabel.TabIndex = 2;
            this.promptPreviewLabel.Text = "Prompt Preview";
            this.promptPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noPromptsFoundLabel
            // 
            this.noPromptsFoundLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noPromptsFoundLabel.Location = new System.Drawing.Point(12, 12);
            this.noPromptsFoundLabel.Name = "noPromptsFoundLabel";
            this.noPromptsFoundLabel.Size = new System.Drawing.Size(367, 426);
            this.noPromptsFoundLabel.TabIndex = 3;
            this.noPromptsFoundLabel.Text = "No Prompts Found!";
            this.noPromptsFoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmPromptLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.noPromptsFoundLabel);
            this.Controls.Add(this.promptPreviewLabel);
            this.Controls.Add(this.promptTextBox);
            this.Controls.Add(this.promptsListView);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "FrmPromptLibrary";
            this.ShowIcon = false;
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label noPromptsFoundLabel;

        private System.Windows.Forms.RichTextBox promptTextBox;
        private System.Windows.Forms.Label promptPreviewLabel;

        private System.Windows.Forms.ListView promptsListView;

        #endregion
    }
}