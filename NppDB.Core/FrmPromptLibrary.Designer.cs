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
            this.placeholderListView = new System.Windows.Forms.ListView();
            this.disableTemplatingCheckbox = new System.Windows.Forms.CheckBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // promptsListView
            // 
            this.promptsListView.HideSelection = false;
            this.promptsListView.Location = new System.Drawing.Point(12, 12);
            this.promptsListView.Name = "promptsListView";
            this.promptsListView.Size = new System.Drawing.Size(367, 418);
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
            this.promptTextBox.Size = new System.Drawing.Size(423, 246);
            this.promptTextBox.TabIndex = 1;
            this.promptTextBox.Text = "";
            // 
            // promptPreviewLabel
            // 
            this.promptPreviewLabel.Font = new System.Drawing.Font("Times New Roman", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptPreviewLabel.Location = new System.Drawing.Point(397, 8);
            this.promptPreviewLabel.Name = "promptPreviewLabel";
            this.promptPreviewLabel.Size = new System.Drawing.Size(423, 55);
            this.promptPreviewLabel.TabIndex = 2;
            this.promptPreviewLabel.Text = "Prompt Preview";
            this.promptPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noPromptsFoundLabel
            // 
            this.noPromptsFoundLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noPromptsFoundLabel.Location = new System.Drawing.Point(12, 12);
            this.noPromptsFoundLabel.Name = "noPromptsFoundLabel";
            this.noPromptsFoundLabel.Size = new System.Drawing.Size(367, 432);
            this.noPromptsFoundLabel.TabIndex = 3;
            this.noPromptsFoundLabel.Text = "No Prompts Found!";
            this.noPromptsFoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // placeholderListView
            // 
            this.placeholderListView.HideSelection = false;
            this.placeholderListView.Location = new System.Drawing.Point(397, 318);
            this.placeholderListView.Name = "placeholderListView";
            this.placeholderListView.Size = new System.Drawing.Size(423, 112);
            this.placeholderListView.TabIndex = 4;
            this.placeholderListView.UseCompatibleStateImageBehavior = false;
            this.placeholderListView.DoubleClick += new System.EventHandler(this.placeholderListView_DoubleClick);
            // 
            // disableTemplatingCheckbox
            // 
            this.disableTemplatingCheckbox.Location = new System.Drawing.Point(397, 436);
            this.disableTemplatingCheckbox.Name = "disableTemplatingCheckbox";
            this.disableTemplatingCheckbox.Size = new System.Drawing.Size(403, 33);
            this.disableTemplatingCheckbox.TabIndex = 5;
            this.disableTemplatingCheckbox.Text = "Show template names instead of contents";
            this.disableTemplatingCheckbox.UseVisualStyleBackColor = true;
            this.disableTemplatingCheckbox.CheckedChanged += new System.EventHandler(this.disableTemplatingCheckbox_CheckedChanged);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(12, 436);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(121, 34);
            this.buttonEdit.TabIndex = 6;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(139, 436);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(113, 33);
            this.buttonAdd.TabIndex = 7;
            this.buttonAdd.Text = "Add New";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(258, 435);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(121, 33);
            this.buttonDelete.TabIndex = 8;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(699, 437);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(121, 33);
            this.buttonCopy.TabIndex = 9;
            this.buttonCopy.Text = "Copy Prompt";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // FrmPromptLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(832, 475);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.disableTemplatingCheckbox);
            this.Controls.Add(this.placeholderListView);
            this.Controls.Add(this.promptPreviewLabel);
            this.Controls.Add(this.promptTextBox);
            this.Controls.Add(this.noPromptsFoundLabel);
            this.Controls.Add(this.promptsListView);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "FrmPromptLibrary";
            this.ShowIcon = false;
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button buttonCopy;

        private System.Windows.Forms.Button buttonDelete;

        private System.Windows.Forms.Button buttonAdd;

        private System.Windows.Forms.Button buttonEdit;

        private System.Windows.Forms.CheckBox disableTemplatingCheckbox;

        private System.Windows.Forms.ListView placeholderListView;

        private System.Windows.Forms.Label noPromptsFoundLabel;

        private System.Windows.Forms.RichTextBox promptTextBox;
        private System.Windows.Forms.Label promptPreviewLabel;

        private System.Windows.Forms.ListView promptsListView;

        #endregion
    }
}