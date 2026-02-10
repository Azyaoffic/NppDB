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
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelBottomActions = new System.Windows.Forms.Panel();
            this.flowLayoutBottomActions = new System.Windows.Forms.FlowLayoutPanel();
            this.labelResponseLanguage = new System.Windows.Forms.Label();
            this.comboBoxResponseLanguage = new System.Windows.Forms.ComboBox();
            this.labelCustomInstructions = new System.Windows.Forms.Label();
            this.richTextBoxCustomInstructions = new System.Windows.Forms.RichTextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tableLayoutMain.SuspendLayout();
            this.panelBottomActions.SuspendLayout();
            this.flowLayoutBottomActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Controls.Add(this.labelResponseLanguage, 0, 0);
            this.tableLayoutMain.Controls.Add(this.comboBoxResponseLanguage, 1, 0);
            this.tableLayoutMain.Controls.Add(this.labelCustomInstructions, 0, 1);
            this.tableLayoutMain.Controls.Add(this.richTextBoxCustomInstructions, 0, 2);
            this.tableLayoutMain.Controls.Add(this.panelBottomActions, 0, 3);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 4;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutMain.Size = new System.Drawing.Size(520, 420);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // panelBottomActions
            // 
            this.tableLayoutMain.SetColumnSpan(this.panelBottomActions, 2);
            this.panelBottomActions.Controls.Add(this.flowLayoutBottomActions);
            this.panelBottomActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottomActions.Location = new System.Drawing.Point(0, 380);
            this.panelBottomActions.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.panelBottomActions.MinimumSize = new System.Drawing.Size(0, 40);
            this.panelBottomActions.Name = "panelBottomActions";
            this.panelBottomActions.Size = new System.Drawing.Size(520, 40);
            this.panelBottomActions.TabIndex = 10;
            // 
            // flowLayoutBottomActions
            // 
            this.flowLayoutBottomActions.Controls.Add(this.buttonSave);
            this.flowLayoutBottomActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutBottomActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutBottomActions.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutBottomActions.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutBottomActions.Name = "flowLayoutBottomActions";
            this.flowLayoutBottomActions.Padding = new System.Windows.Forms.Padding(0);
            this.flowLayoutBottomActions.Size = new System.Drawing.Size(520, 40);
            this.flowLayoutBottomActions.TabIndex = 0;
            this.flowLayoutBottomActions.WrapContents = false;
            // 
            // labelResponseLanguage
            // 
            this.labelResponseLanguage.AutoSize = true;
            this.labelResponseLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelResponseLanguage.Location = new System.Drawing.Point(0, 0);
            this.labelResponseLanguage.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.labelResponseLanguage.Name = "labelResponseLanguage";
            this.labelResponseLanguage.Size = new System.Drawing.Size(160, 19);
            this.labelResponseLanguage.TabIndex = 0;
            this.labelResponseLanguage.Text = "Response Language";
            this.labelResponseLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxResponseLanguage
            // 
            this.comboBoxResponseLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxResponseLanguage.FormattingEnabled = true;
            this.comboBoxResponseLanguage.Location = new System.Drawing.Point(170, 3);
            this.comboBoxResponseLanguage.MinimumSize = new System.Drawing.Size(160, 0);
            this.comboBoxResponseLanguage.Name = "comboBoxResponseLanguage";
            this.comboBoxResponseLanguage.Size = new System.Drawing.Size(347, 21);
            this.comboBoxResponseLanguage.TabIndex = 1;
            this.comboBoxResponseLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxResponseLanguage_SelectedIndexChanged);
            // 
            // labelCustomInstructions
            // 
            this.labelCustomInstructions.AutoSize = true;
            this.tableLayoutMain.SetColumnSpan(this.labelCustomInstructions, 2);
            this.labelCustomInstructions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCustomInstructions.Location = new System.Drawing.Point(0, 34);
            this.labelCustomInstructions.Margin = new System.Windows.Forms.Padding(0, 10, 0, 4);
            this.labelCustomInstructions.Name = "labelCustomInstructions";
            this.labelCustomInstructions.Size = new System.Drawing.Size(117, 15);
            this.labelCustomInstructions.TabIndex = 2;
            this.labelCustomInstructions.Text = "Custom Instructions";
            // 
            // richTextBoxCustomInstructions
            // 
            this.richTextBoxCustomInstructions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutMain.SetColumnSpan(this.richTextBoxCustomInstructions, 2);
            this.richTextBoxCustomInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxCustomInstructions.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxCustomInstructions.Location = new System.Drawing.Point(0, 53);
            this.richTextBoxCustomInstructions.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxCustomInstructions.MinimumSize = new System.Drawing.Size(200, 120);
            this.richTextBoxCustomInstructions.Name = "richTextBoxCustomInstructions";
            this.richTextBoxCustomInstructions.Size = new System.Drawing.Size(520, 317);
            this.richTextBoxCustomInstructions.TabIndex = 3;
            this.richTextBoxCustomInstructions.Text = "";
            // 
            // buttonSave
            // 
            this.buttonSave.AutoEllipsis = true;
            this.buttonSave.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonSave.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonSave.Location = new System.Drawing.Point(377, 6);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(6, 6, 0, 6);
            this.buttonSave.MinimumSize = new System.Drawing.Size(140, 28);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(143, 28);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FrmPromptPreferences
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(540, 440);
            this.Controls.Add(this.tableLayoutMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(460, 420);
            this.Name = "FrmPromptPreferences";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LLM Response Preferences";
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutMain.PerformLayout();
            this.panelBottomActions.ResumeLayout(false);
            this.flowLayoutBottomActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.Panel panelBottomActions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutBottomActions;

        private System.Windows.Forms.Button buttonSave;

        private System.Windows.Forms.Label labelCustomInstructions;
        private System.Windows.Forms.RichTextBox richTextBoxCustomInstructions;

        private System.Windows.Forms.ComboBox comboBoxResponseLanguage;

        private System.Windows.Forms.Label labelResponseLanguage;

        #endregion
    }
}