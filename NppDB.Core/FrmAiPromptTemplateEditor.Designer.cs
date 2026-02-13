using System.ComponentModel;
using System.Windows.Forms;

namespace NppDB.Core
{
    partial class FrmAiPromptTemplateEditor
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnRestoreDefault = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.lblPathValue = new System.Windows.Forms.Label();
            this.txtTemplate = new System.Windows.Forms.RichTextBox();
            this.lblPlaceholders = new System.Windows.Forms.Label();
            this.comboInsert = new System.Windows.Forms.ComboBox();
            this.btnInsert = new System.Windows.Forms.Button();
            this.lblInsert = new System.Windows.Forms.Label();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panelBottom.Controls.Add(this.btnRestoreDefault);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 505);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(784, 45);
            this.panelBottom.TabIndex = 10;
            // 
            // btnRestoreDefault
            // 
            this.btnRestoreDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestoreDefault.Location = new System.Drawing.Point(12, 8);
            this.btnRestoreDefault.Name = "btnRestoreDefault";
            this.btnRestoreDefault.Size = new System.Drawing.Size(120, 27);
            this.btnRestoreDefault.TabIndex = 0;
            this.btnRestoreDefault.Text = "Restore Default";
            this.btnRestoreDefault.UseVisualStyleBackColor = true;
            this.btnRestoreDefault.Click += new System.EventHandler(this.btnRestoreDefault_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(697, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(616, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(145, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "AI Prompt Template Editor";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPath.Location = new System.Drawing.Point(12, 33);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(33, 15);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "Path:";
            // 
            // lblPathValue
            // 
            this.lblPathValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPathValue.AutoEllipsis = true;
            this.lblPathValue.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPathValue.Location = new System.Drawing.Point(51, 33);
            this.lblPathValue.Name = "lblPathValue";
            this.lblPathValue.Size = new System.Drawing.Size(721, 15);
            this.lblPathValue.TabIndex = 2;
            this.lblPathValue.Text = "";
            // 
            // txtTemplate
            // 
            this.txtTemplate.AcceptsTab = true;
            this.txtTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTemplate.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTemplate.Location = new System.Drawing.Point(15, 90);
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(757, 387);
            this.txtTemplate.TabIndex = 6;
            this.txtTemplate.Text = "";
            // 
            // lblPlaceholders
            // 
            this.lblPlaceholders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPlaceholders.AutoSize = true;
            this.lblPlaceholders.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPlaceholders.Location = new System.Drawing.Point(15, 482);
            this.lblPlaceholders.Name = "lblPlaceholders";
            this.lblPlaceholders.Size = new System.Drawing.Size(0, 15);
            this.lblPlaceholders.TabIndex = 7;
            // 
            // comboInsert
            // 
            this.comboInsert.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInsert.FormattingEnabled = true;
            this.comboInsert.Location = new System.Drawing.Point(115, 58);
            this.comboInsert.Name = "comboInsert";
            this.comboInsert.Size = new System.Drawing.Size(283, 23);
            this.comboInsert.TabIndex = 4;
            // 
            // btnInsert
            // 
            this.btnInsert.Location = new System.Drawing.Point(404, 58);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(75, 23);
            this.btnInsert.TabIndex = 5;
            this.btnInsert.Text = "Insert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // lblInsert
            // 
            this.lblInsert.AutoSize = true;
            this.lblInsert.Location = new System.Drawing.Point(12, 61);
            this.lblInsert.Name = "lblInsert";
            this.lblInsert.Size = new System.Drawing.Size(97, 15);
            this.lblInsert.TabIndex = 3;
            this.lblInsert.Text = "Insert placeholder:";
            // 
            // FrmAiPromptTemplateEditor
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 550);
            this.Controls.Add(this.lblInsert);
            this.Controls.Add(this.btnInsert);
            this.Controls.Add(this.comboInsert);
            this.Controls.Add(this.lblPlaceholders);
            this.Controls.Add(this.txtTemplate);
            this.Controls.Add(this.lblPathValue);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 450);
            this.Name = "FrmAiPromptTemplateEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit AI Prompt Template";
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnRestoreDefault;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblPathValue;

        private System.Windows.Forms.Label lblInsert;
        private System.Windows.Forms.ComboBox comboInsert;
        private System.Windows.Forms.Button btnInsert;

        private System.Windows.Forms.RichTextBox txtTemplate;
        private System.Windows.Forms.Label lblPlaceholders;
    }
}
