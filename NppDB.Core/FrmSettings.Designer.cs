namespace NppDB.Core
{
    partial class FrmSettings
    {
        private System.ComponentModel.IContainer components = null;

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
            this.tableRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabBehavior = new System.Windows.Forms.TabPage();
            this.tableBehavior = new System.Windows.Forms.TableLayoutPanel();
            this.chkEnableDestructiveSelectInto = new System.Windows.Forms.CheckBox();
            this.chkEnableNewTabCreation = new System.Windows.Forms.CheckBox();
            this.tabLlm = new System.Windows.Forms.TabPage();
            this.tableLlm = new System.Windows.Forms.TableLayoutPanel();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.comboResponseLanguage = new System.Windows.Forms.ComboBox();
            this.lblOpenUrl = new System.Windows.Forms.Label();
            this.txtOpenLlmUrl = new System.Windows.Forms.TextBox();
            this.lblCustomInstructions = new System.Windows.Forms.Label();
            this.txtCustomInstructions = new System.Windows.Forms.RichTextBox();
            this.tabAiTemplate = new System.Windows.Forms.TabPage();
            this.tableAi = new System.Windows.Forms.TableLayoutPanel();
            this.lblRequiredPlaceholders = new System.Windows.Forms.Label();
            this.panelInsert = new System.Windows.Forms.FlowLayoutPanel();
            this.comboInsertPlaceholder = new System.Windows.Forms.ComboBox();
            this.btnInsertPlaceholder = new System.Windows.Forms.Button();
            this.btnRestoreDefaultTemplate = new System.Windows.Forms.Button();
            this.txtAiTemplate = new System.Windows.Forms.TextBox();
            this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableRoot.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabBehavior.SuspendLayout();
            this.tableBehavior.SuspendLayout();
            this.tabLlm.SuspendLayout();
            this.tableLlm.SuspendLayout();
            this.tabAiTemplate.SuspendLayout();
            this.tableAi.SuspendLayout();
            this.panelInsert.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableRoot
            // 
            this.tableRoot.ColumnCount = 1;
            this.tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRoot.Controls.Add(this.tabControlMain, 0, 0);
            this.tableRoot.Controls.Add(this.panelButtons, 0, 1);
            this.tableRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableRoot.Location = new System.Drawing.Point(0, 0);
            this.tableRoot.Name = "tableRoot";
            this.tableRoot.RowCount = 2;
            this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableRoot.Size = new System.Drawing.Size(780, 520);
            this.tableRoot.TabIndex = 0;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabBehavior);
            this.tabControlMain.Controls.Add(this.tabLlm);
            this.tabControlMain.Controls.Add(this.tabAiTemplate);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(8, 8);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(8);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(764, 460);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabBehavior
            // 
            this.tabBehavior.Controls.Add(this.tableBehavior);
            this.tabBehavior.Location = new System.Drawing.Point(4, 24);
            this.tabBehavior.Name = "tabBehavior";
            this.tabBehavior.Padding = new System.Windows.Forms.Padding(8);
            this.tabBehavior.Size = new System.Drawing.Size(756, 432);
            this.tabBehavior.TabIndex = 0;
            this.tabBehavior.Text = "Behavior";
            this.tabBehavior.UseVisualStyleBackColor = true;
            // 
            // tableBehavior
            // 
            this.tableBehavior.ColumnCount = 1;
            this.tableBehavior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableBehavior.Controls.Add(this.chkEnableDestructiveSelectInto, 0, 0);
            this.tableBehavior.Controls.Add(this.chkEnableNewTabCreation, 0, 1);
            this.tableBehavior.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableBehavior.Location = new System.Drawing.Point(8, 8);
            this.tableBehavior.Name = "tableBehavior";
            this.tableBehavior.RowCount = 3;
            this.tableBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableBehavior.Size = new System.Drawing.Size(740, 120);
            this.tableBehavior.TabIndex = 0;
            // 
            // chkEnableDestructiveSelectInto
            // 
            this.chkEnableDestructiveSelectInto.AutoSize = true;
            this.chkEnableDestructiveSelectInto.Location = new System.Drawing.Point(3, 3);
            this.chkEnableDestructiveSelectInto.Name = "chkEnableDestructiveSelectInto";
            this.chkEnableDestructiveSelectInto.Size = new System.Drawing.Size(262, 19);
            this.chkEnableDestructiveSelectInto.TabIndex = 0;
            this.chkEnableDestructiveSelectInto.Text = "Enable destructive SELECT INTO operations";
            this.chkEnableDestructiveSelectInto.UseVisualStyleBackColor = true;
            // 
            // chkEnableNewTabCreation
            // 
            this.chkEnableNewTabCreation.AutoSize = true;
            this.chkEnableNewTabCreation.Location = new System.Drawing.Point(3, 28);
            this.chkEnableNewTabCreation.Name = "chkEnableNewTabCreation";
            this.chkEnableNewTabCreation.Size = new System.Drawing.Size(180, 19);
            this.chkEnableNewTabCreation.TabIndex = 1;
            this.chkEnableNewTabCreation.Text = "Create a new tab when needed";
            this.chkEnableNewTabCreation.UseVisualStyleBackColor = true;
            // 
            // tabLlm
            // 
            this.tabLlm.Controls.Add(this.tableLlm);
            this.tabLlm.Location = new System.Drawing.Point(4, 24);
            this.tabLlm.Name = "tabLlm";
            this.tabLlm.Padding = new System.Windows.Forms.Padding(8);
            this.tabLlm.Size = new System.Drawing.Size(756, 432);
            this.tabLlm.TabIndex = 1;
            this.tabLlm.Text = "LLM";
            this.tabLlm.UseVisualStyleBackColor = true;
            // 
            // tableLlm
            // 
            this.tableLlm.ColumnCount = 2;
            this.tableLlm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLlm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLlm.Controls.Add(this.lblLanguage, 0, 0);
            this.tableLlm.Controls.Add(this.comboResponseLanguage, 1, 0);
            this.tableLlm.Controls.Add(this.lblOpenUrl, 0, 1);
            this.tableLlm.Controls.Add(this.txtOpenLlmUrl, 1, 1);
            this.tableLlm.Controls.Add(this.lblCustomInstructions, 0, 2);
            this.tableLlm.Controls.Add(this.txtCustomInstructions, 0, 3);
            this.tableLlm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLlm.Location = new System.Drawing.Point(8, 8);
            this.tableLlm.Name = "tableLlm";
            this.tableLlm.RowCount = 4;
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLlm.Size = new System.Drawing.Size(740, 416);
            this.tableLlm.TabIndex = 0;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLanguage.Location = new System.Drawing.Point(3, 0);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblLanguage.Size = new System.Drawing.Size(154, 34);
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Text = "Response language";
            // 
            // comboResponseLanguage
            // 
            this.comboResponseLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboResponseLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboResponseLanguage.FormattingEnabled = true;
            this.comboResponseLanguage.Location = new System.Drawing.Point(163, 5);
            this.comboResponseLanguage.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.comboResponseLanguage.Name = "comboResponseLanguage";
            this.comboResponseLanguage.Size = new System.Drawing.Size(574, 23);
            this.comboResponseLanguage.TabIndex = 1;
            // 
            // lblOpenUrl
            // 
            this.lblOpenUrl.AutoSize = true;
            this.lblOpenUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenUrl.Location = new System.Drawing.Point(3, 34);
            this.lblOpenUrl.Name = "lblOpenUrl";
            this.lblOpenUrl.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblOpenUrl.Size = new System.Drawing.Size(154, 34);
            this.lblOpenUrl.TabIndex = 2;
            this.lblOpenUrl.Text = "Open LLM URL";
            // 
            // txtOpenLlmUrl
            // 
            this.txtOpenLlmUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOpenLlmUrl.Location = new System.Drawing.Point(163, 39);
            this.txtOpenLlmUrl.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.txtOpenLlmUrl.Name = "txtOpenLlmUrl";
            this.txtOpenLlmUrl.Size = new System.Drawing.Size(574, 23);
            this.txtOpenLlmUrl.TabIndex = 3;
            // 
            // lblCustomInstructions
            // 
            this.lblCustomInstructions.AutoSize = true;
            this.tableLlm.SetColumnSpan(this.lblCustomInstructions, 2);
            this.lblCustomInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCustomInstructions.Location = new System.Drawing.Point(3, 68);
            this.lblCustomInstructions.Name = "lblCustomInstructions";
            this.lblCustomInstructions.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblCustomInstructions.Size = new System.Drawing.Size(734, 26);
            this.lblCustomInstructions.TabIndex = 4;
            this.lblCustomInstructions.Text = "Custom instructions";
            // 
            // txtCustomInstructions
            // 
            this.tableLlm.SetColumnSpan(this.txtCustomInstructions, 2);
            this.txtCustomInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCustomInstructions.Location = new System.Drawing.Point(3, 97);
            this.txtCustomInstructions.Name = "txtCustomInstructions";
            this.txtCustomInstructions.Size = new System.Drawing.Size(734, 316);
            this.txtCustomInstructions.TabIndex = 5;
            this.txtCustomInstructions.Text = "";
            // 
            // tabAiTemplate
            // 
            this.tabAiTemplate.Controls.Add(this.tableAi);
            this.tabAiTemplate.Location = new System.Drawing.Point(4, 24);
            this.tabAiTemplate.Name = "tabAiTemplate";
            this.tabAiTemplate.Padding = new System.Windows.Forms.Padding(8);
            this.tabAiTemplate.Size = new System.Drawing.Size(756, 432);
            this.tabAiTemplate.TabIndex = 2;
            this.tabAiTemplate.Text = "AI Prompt Template";
            this.tabAiTemplate.UseVisualStyleBackColor = true;
            // 
            // tableAi
            // 
            this.tableAi.ColumnCount = 1;
            this.tableAi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableAi.Controls.Add(this.lblRequiredPlaceholders, 0, 0);
            this.tableAi.Controls.Add(this.panelInsert, 0, 1);
            this.tableAi.Controls.Add(this.txtAiTemplate, 0, 2);
            this.tableAi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableAi.Location = new System.Drawing.Point(8, 8);
            this.tableAi.Name = "tableAi";
            this.tableAi.RowCount = 3;
            this.tableAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableAi.Size = new System.Drawing.Size(740, 416);
            this.tableAi.TabIndex = 0;
            // 
            // lblRequiredPlaceholders
            // 
            this.lblRequiredPlaceholders.AutoSize = true;
            this.lblRequiredPlaceholders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRequiredPlaceholders.Location = new System.Drawing.Point(3, 0);
            this.lblRequiredPlaceholders.Name = "lblRequiredPlaceholders";
            this.lblRequiredPlaceholders.Padding = new System.Windows.Forms.Padding(0, 2, 0, 6);
            this.lblRequiredPlaceholders.Size = new System.Drawing.Size(734, 23);
            this.lblRequiredPlaceholders.TabIndex = 0;
            this.lblRequiredPlaceholders.Text = "Required placeholders:";
            // 
            // panelInsert
            // 
            this.panelInsert.Controls.Add(this.comboInsertPlaceholder);
            this.panelInsert.Controls.Add(this.btnInsertPlaceholder);
            this.panelInsert.Controls.Add(this.btnRestoreDefaultTemplate);
            this.panelInsert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInsert.Location = new System.Drawing.Point(0, 23);
            this.panelInsert.Margin = new System.Windows.Forms.Padding(0);
            this.panelInsert.Name = "panelInsert";
            this.panelInsert.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.panelInsert.Size = new System.Drawing.Size(740, 36);
            this.panelInsert.TabIndex = 1;
            // 
            // comboInsertPlaceholder
            // 
            this.comboInsertPlaceholder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInsertPlaceholder.FormattingEnabled = true;
            this.comboInsertPlaceholder.Location = new System.Drawing.Point(3, 9);
            this.comboInsertPlaceholder.Name = "comboInsertPlaceholder";
            this.comboInsertPlaceholder.Size = new System.Drawing.Size(240, 23);
            this.comboInsertPlaceholder.TabIndex = 0;
            // 
            // btnInsertPlaceholder
            // 
            this.btnInsertPlaceholder.Location = new System.Drawing.Point(249, 8);
            this.btnInsertPlaceholder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.btnInsertPlaceholder.Name = "btnInsertPlaceholder";
            this.btnInsertPlaceholder.Size = new System.Drawing.Size(96, 23);
            this.btnInsertPlaceholder.TabIndex = 1;
            this.btnInsertPlaceholder.Text = "Insert";
            this.btnInsertPlaceholder.UseVisualStyleBackColor = true;
            this.btnInsertPlaceholder.Click += new System.EventHandler(this.btnInsertPlaceholder_Click);
            // 
            // btnRestoreDefaultTemplate
            // 
            this.btnRestoreDefaultTemplate.Location = new System.Drawing.Point(351, 8);
            this.btnRestoreDefaultTemplate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.btnRestoreDefaultTemplate.Name = "btnRestoreDefaultTemplate";
            this.btnRestoreDefaultTemplate.Size = new System.Drawing.Size(140, 23);
            this.btnRestoreDefaultTemplate.TabIndex = 2;
            this.btnRestoreDefaultTemplate.Text = "Restore default";
            this.btnRestoreDefaultTemplate.UseVisualStyleBackColor = true;
            this.btnRestoreDefaultTemplate.Click += new System.EventHandler(this.btnRestoreDefaultTemplate_Click);
            // 
            // txtAiTemplate
            // 
            this.txtAiTemplate.AcceptsReturn = true;
            this.txtAiTemplate.AcceptsTab = true;
            this.txtAiTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAiTemplate.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtAiTemplate.Location = new System.Drawing.Point(3, 62);
            this.txtAiTemplate.Multiline = true;
            this.txtAiTemplate.Name = "txtAiTemplate";
            this.txtAiTemplate.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAiTemplate.Size = new System.Drawing.Size(734, 351);
            this.txtAiTemplate.TabIndex = 2;
            this.txtAiTemplate.WordWrap = false;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panelButtons.Location = new System.Drawing.Point(8, 476);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(764, 36);
            this.panelButtons.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(686, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(605, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 520);
            this.Controls.Add(this.tableRoot);
            this.MinimumSize = new System.Drawing.Size(720, 480);
            this.Name = "FrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NppDB Settings";
            this.tableRoot.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabBehavior.ResumeLayout(false);
            this.tableBehavior.ResumeLayout(false);
            this.tableBehavior.PerformLayout();
            this.tabLlm.ResumeLayout(false);
            this.tableLlm.ResumeLayout(false);
            this.tableLlm.PerformLayout();
            this.tabAiTemplate.ResumeLayout(false);
            this.tableAi.ResumeLayout(false);
            this.tableAi.PerformLayout();
            this.panelInsert.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableRoot;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabBehavior;
        private System.Windows.Forms.TableLayoutPanel tableBehavior;
        private System.Windows.Forms.CheckBox chkEnableDestructiveSelectInto;
        private System.Windows.Forms.CheckBox chkEnableNewTabCreation;
        private System.Windows.Forms.TabPage tabLlm;
        private System.Windows.Forms.TableLayoutPanel tableLlm;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox comboResponseLanguage;
        private System.Windows.Forms.Label lblOpenUrl;
        private System.Windows.Forms.TextBox txtOpenLlmUrl;
        private System.Windows.Forms.Label lblCustomInstructions;
        private System.Windows.Forms.RichTextBox txtCustomInstructions;
        private System.Windows.Forms.TabPage tabAiTemplate;
        private System.Windows.Forms.TableLayoutPanel tableAi;
        private System.Windows.Forms.Label lblRequiredPlaceholders;
        private System.Windows.Forms.FlowLayoutPanel panelInsert;
        private System.Windows.Forms.ComboBox comboInsertPlaceholder;
        private System.Windows.Forms.Button btnInsertPlaceholder;
        private System.Windows.Forms.Button btnRestoreDefaultTemplate;
        private System.Windows.Forms.TextBox txtAiTemplate;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}