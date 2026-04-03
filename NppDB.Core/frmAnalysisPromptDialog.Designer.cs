using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NppDB.Core
{
    partial class frmAnalysisPromptDialog
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.txtPrompt = new System.Windows.Forms.RichTextBox();
            this.lnkTemplateHint = new System.Windows.Forms.LinkLabel();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnOpenLlm = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(16, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(288, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Review and adjust the generated analysis prompt.";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(16, 39);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(300, 13);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "You can edit the text before copying it or opening your LLM.";
            // 
            // txtPrompt
            // 
            this.txtPrompt.AcceptsTab = true;
            this.txtPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrompt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrompt.DetectUrls = false;
            this.txtPrompt.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrompt.Location = new System.Drawing.Point(16, 68);
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.txtPrompt.Size = new System.Drawing.Size(1120, 560);
            this.txtPrompt.TabIndex = 2;
            this.txtPrompt.Text = "";
            this.txtPrompt.WordWrap = false;
            // 
            // lnkTemplateHint
            // 
            this.lnkTemplateHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkTemplateHint.AutoSize = true;
            this.lnkTemplateHint.Location = new System.Drawing.Point(16, 647);
            this.lnkTemplateHint.Name = "lnkTemplateHint";
            this.lnkTemplateHint.Size = new System.Drawing.Size(345, 13);
            this.lnkTemplateHint.TabIndex = 3;
            this.lnkTemplateHint.TabStop = true;
            this.lnkTemplateHint.Text = "Template can be edited in NppDB Settings -> Analysis Prompt Template";
            this.lnkTemplateHint.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTemplateHint_LinkClicked);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(590, 634);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(170, 38);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "Copy to Clipboard";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnOpenLlm
            // 
            this.btnOpenLlm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenLlm.Location = new System.Drawing.Point(772, 634);
            this.btnOpenLlm.Name = "btnOpenLlm";
            this.btnOpenLlm.Size = new System.Drawing.Size(170, 38);
            this.btnOpenLlm.TabIndex = 5;
            this.btnOpenLlm.Text = "Open LLM";
            this.btnOpenLlm.UseVisualStyleBackColor = true;
            this.btnOpenLlm.Click += new System.EventHandler(this.btnOpenLlm_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(966, 634);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(170, 38);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // AnalysisPromptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1152, 684);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpenLlm);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lnkTemplateHint);
            this.Controls.Add(this.txtPrompt);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(1040, 620);
            this.Name = "AnalysisPromptDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Analysis Prompt";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnOpenLlm;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.LinkLabel lnkTemplateHint;
        private System.Windows.Forms.RichTextBox txtPrompt;

        #endregion
    }
}
