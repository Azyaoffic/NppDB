namespace NppDB.Core
{
    partial class FrmSettings
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabBehavior = new System.Windows.Forms.TabPage();
            this.grpBehavior = new System.Windows.Forms.GroupBox();
            this.layoutBehaviorOptions = new System.Windows.Forms.TableLayoutPanel();
            this.chkEnableDestructiveSelectInto = new System.Windows.Forms.CheckBox();
            this.chkEnableNewTabCreation = new System.Windows.Forms.CheckBox();
            this.pnlDbManagerFont = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDbManagerFontScale = new System.Windows.Forms.Label();
            this.numDbManagerFontScale = new System.Windows.Forms.NumericUpDown();
            this.tabLlm = new System.Windows.Forms.TabPage();
            this.grpLlm = new System.Windows.Forms.GroupBox();
            this.tableLlm = new System.Windows.Forms.TableLayoutPanel();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.comboResponseLanguage = new System.Windows.Forms.ComboBox();
            this.lblOpenUrl = new System.Windows.Forms.Label();
            this.txtOpenLlmUrl = new System.Windows.Forms.TextBox();
            this.lblCustomInstructions = new System.Windows.Forms.Label();
            this.txtCustomInstructions = new System.Windows.Forms.RichTextBox();
            this.tabAiTemplate = new System.Windows.Forms.TabPage();
            this.grpAiTemplate = new System.Windows.Forms.GroupBox();
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
            this.grpBehavior.SuspendLayout();
            this.layoutBehaviorOptions.SuspendLayout();
            this.pnlDbManagerFont.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDbManagerFontScale)).BeginInit();
            this.tabLlm.SuspendLayout();
            this.grpLlm.SuspendLayout();
            this.tableLlm.SuspendLayout();
            this.tabAiTemplate.SuspendLayout();
            this.grpAiTemplate.SuspendLayout();
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
            this.tableRoot.Location = new System.Drawing.Point(10, 10);
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
            this.tabControlMain.Location = new System.Drawing.Point(3, 3);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(774, 470);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabBehavior
            // 
            this.tabBehavior.Controls.Add(this.grpBehavior);
            this.tabBehavior.Location = new System.Drawing.Point(4, 22);
            this.tabBehavior.Name = "tabBehavior";
            this.tabBehavior.Padding = new System.Windows.Forms.Padding(10);
            this.tabBehavior.Size = new System.Drawing.Size(766, 444);
            this.tabBehavior.TabIndex = 0;
            this.tabBehavior.Text = "Behavior";
            this.tabBehavior.UseVisualStyleBackColor = true;
            // 
            // grpBehavior
            // 
            this.grpBehavior.Controls.Add(this.layoutBehaviorOptions);
            this.grpBehavior.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBehavior.Location = new System.Drawing.Point(10, 10);
            this.grpBehavior.Name = "grpBehavior";
            this.grpBehavior.Padding = new System.Windows.Forms.Padding(10);
            this.grpBehavior.Size = new System.Drawing.Size(746, 424);
            this.grpBehavior.TabIndex = 0;
            this.grpBehavior.TabStop = false;
            this.grpBehavior.Text = "Behavior";
            // 
            // layoutBehaviorOptions
            // 
            this.layoutBehaviorOptions.ColumnCount = 1;
            this.layoutBehaviorOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutBehaviorOptions.Controls.Add(this.chkEnableDestructiveSelectInto, 0, 0);
            this.layoutBehaviorOptions.Controls.Add(this.chkEnableNewTabCreation, 0, 1);
            this.layoutBehaviorOptions.Controls.Add(this.pnlDbManagerFont, 0, 2);
            this.layoutBehaviorOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutBehaviorOptions.Location = new System.Drawing.Point(10, 25);
            this.layoutBehaviorOptions.Name = "layoutBehaviorOptions";
            this.layoutBehaviorOptions.RowCount = 4;
            this.layoutBehaviorOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutBehaviorOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutBehaviorOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutBehaviorOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutBehaviorOptions.Size = new System.Drawing.Size(726, 140);
            this.layoutBehaviorOptions.TabIndex = 0;
            // 
            // chkEnableDestructiveSelectInto
            // 
            this.chkEnableDestructiveSelectInto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEnableDestructiveSelectInto.AutoEllipsis = true;
            this.chkEnableDestructiveSelectInto.Location = new System.Drawing.Point(3, 3);
            this.chkEnableDestructiveSelectInto.MinimumSize = new System.Drawing.Size(0, 28);
            this.chkEnableDestructiveSelectInto.Name = "chkEnableDestructiveSelectInto";
            this.chkEnableDestructiveSelectInto.Size = new System.Drawing.Size(720, 34);
            this.chkEnableDestructiveSelectInto.TabIndex = 0;
            this.chkEnableDestructiveSelectInto.Text = "Enable destructive SELECT INTO (MSAccess)";
            this.chkEnableDestructiveSelectInto.UseVisualStyleBackColor = true;
            // 
            // chkEnableNewTabCreation
            // 
            this.chkEnableNewTabCreation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEnableNewTabCreation.AutoEllipsis = true;
            this.chkEnableNewTabCreation.Location = new System.Drawing.Point(3, 43);
            this.chkEnableNewTabCreation.MinimumSize = new System.Drawing.Size(0, 28);
            this.chkEnableNewTabCreation.Name = "chkEnableNewTabCreation";
            this.chkEnableNewTabCreation.Size = new System.Drawing.Size(720, 34);
            this.chkEnableNewTabCreation.TabIndex = 1;
            this.chkEnableNewTabCreation.Text = "Create new tab every time for queries";
            this.chkEnableNewTabCreation.UseVisualStyleBackColor = true;
            // 
            // pnlDbManagerFont
            // 
            this.pnlDbManagerFont.AutoSize = true;
            this.pnlDbManagerFont.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlDbManagerFont.Controls.Add(this.lblDbManagerFontScale);
            this.pnlDbManagerFont.Controls.Add(this.numDbManagerFontScale);
            this.pnlDbManagerFont.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDbManagerFont.Location = new System.Drawing.Point(3, 83);
            this.pnlDbManagerFont.Name = "pnlDbManagerFont";
            this.pnlDbManagerFont.Size = new System.Drawing.Size(720, 28);
            this.pnlDbManagerFont.TabIndex = 2;
            this.pnlDbManagerFont.WrapContents = false;
            // 
            // lblDbManagerFontScale
            // 
            this.lblDbManagerFontScale.AutoSize = true;
            this.lblDbManagerFontScale.Location = new System.Drawing.Point(0, 6);
            this.lblDbManagerFontScale.Margin = new System.Windows.Forms.Padding(0, 6, 8, 0);
            this.lblDbManagerFontScale.Name = "lblDbManagerFontScale";
            this.lblDbManagerFontScale.Size = new System.Drawing.Size(214, 13);
            this.lblDbManagerFontScale.TabIndex = 0;
            this.lblDbManagerFontScale.Text = "DB Manager text scale (requires restart): ";
            // 
            // numDbManagerFontScale
            // 
            this.numDbManagerFontScale.DecimalPlaces = 2;
            this.numDbManagerFontScale.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            this.numDbManagerFontScale.Location = new System.Drawing.Point(225, 3);
            this.numDbManagerFontScale.Maximum = new decimal(new int[] { 250, 0, 0, 131072 });
            this.numDbManagerFontScale.Minimum = new decimal(new int[] { 75, 0, 0, 131072 });
            this.numDbManagerFontScale.Name = "numDbManagerFontScale";
            this.numDbManagerFontScale.Size = new System.Drawing.Size(70, 22);
            this.numDbManagerFontScale.TabIndex = 1;
            this.numDbManagerFontScale.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tabLlm
            // 
            this.tabLlm.Controls.Add(this.grpLlm);
            this.tabLlm.Location = new System.Drawing.Point(4, 22);
            this.tabLlm.Name = "tabLlm";
            this.tabLlm.Padding = new System.Windows.Forms.Padding(10);
            this.tabLlm.Size = new System.Drawing.Size(766, 444);
            this.tabLlm.TabIndex = 1;
            this.tabLlm.Text = "LLM";
            this.tabLlm.UseVisualStyleBackColor = true;
            // 
            // grpLlm
            // 
            this.grpLlm.Controls.Add(this.tableLlm);
            this.grpLlm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLlm.Location = new System.Drawing.Point(10, 10);
            this.grpLlm.Name = "grpLlm";
            this.grpLlm.Padding = new System.Windows.Forms.Padding(10);
            this.grpLlm.Size = new System.Drawing.Size(746, 424);
            this.grpLlm.TabIndex = 0;
            this.grpLlm.TabStop = false;
            this.grpLlm.Text = "LLM Response";
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
            this.tableLlm.Location = new System.Drawing.Point(10, 25);
            this.tableLlm.Name = "tableLlm";
            this.tableLlm.RowCount = 4;
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLlm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLlm.Size = new System.Drawing.Size(726, 389);
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
            this.comboResponseLanguage.Size = new System.Drawing.Size(560, 21);
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
            this.txtOpenLlmUrl.Size = new System.Drawing.Size(560, 22);
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
            this.lblCustomInstructions.Size = new System.Drawing.Size(720, 26);
            this.lblCustomInstructions.TabIndex = 4;
            this.lblCustomInstructions.Text = "Custom instructions";
            // 
            // txtCustomInstructions
            // 
            this.tableLlm.SetColumnSpan(this.txtCustomInstructions, 2);
            this.txtCustomInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCustomInstructions.Location = new System.Drawing.Point(3, 97);
            this.txtCustomInstructions.Name = "txtCustomInstructions";
            this.txtCustomInstructions.Size = new System.Drawing.Size(720, 289);
            this.txtCustomInstructions.TabIndex = 5;
            this.txtCustomInstructions.Text = "";
            // 
            // tabAiTemplate
            // 
            this.tabAiTemplate.Controls.Add(this.grpAiTemplate);
            this.tabAiTemplate.Location = new System.Drawing.Point(4, 22);
            this.tabAiTemplate.Name = "tabAiTemplate";
            this.tabAiTemplate.Padding = new System.Windows.Forms.Padding(10);
            this.tabAiTemplate.Size = new System.Drawing.Size(766, 444);
            this.tabAiTemplate.TabIndex = 2;
            this.tabAiTemplate.Text = "AI Prompt Template";
            this.tabAiTemplate.UseVisualStyleBackColor = true;
            // 
            // grpAiTemplate
            // 
            this.grpAiTemplate.Controls.Add(this.tableAi);
            this.grpAiTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAiTemplate.Location = new System.Drawing.Point(10, 10);
            this.grpAiTemplate.Name = "grpAiTemplate";
            this.grpAiTemplate.Padding = new System.Windows.Forms.Padding(10);
            this.grpAiTemplate.Size = new System.Drawing.Size(746, 424);
            this.grpAiTemplate.TabIndex = 0;
            this.grpAiTemplate.TabStop = false;
            this.grpAiTemplate.Text = "AI Prompt Template";
            // 
            // tableAi
            // 
            this.tableAi.ColumnCount = 1;
            this.tableAi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableAi.Controls.Add(this.lblRequiredPlaceholders, 0, 0);
            this.tableAi.Controls.Add(this.panelInsert, 0, 1);
            this.tableAi.Controls.Add(this.txtAiTemplate, 0, 2);
            this.tableAi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableAi.Location = new System.Drawing.Point(10, 25);
            this.tableAi.Name = "tableAi";
            this.tableAi.RowCount = 3;
            this.tableAi.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableAi.Size = new System.Drawing.Size(726, 389);
            this.tableAi.TabIndex = 0;
            // 
            // lblRequiredPlaceholders
            // 
            this.lblRequiredPlaceholders.AutoSize = true;
            this.lblRequiredPlaceholders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRequiredPlaceholders.Location = new System.Drawing.Point(3, 0);
            this.lblRequiredPlaceholders.Name = "lblRequiredPlaceholders";
            this.lblRequiredPlaceholders.Padding = new System.Windows.Forms.Padding(0, 2, 0, 6);
            this.lblRequiredPlaceholders.Size = new System.Drawing.Size(720, 21);
            this.lblRequiredPlaceholders.TabIndex = 0;
            this.lblRequiredPlaceholders.Text = "Required placeholders:";
            // 
            // panelInsert
            // 
            this.panelInsert.Controls.Add(this.comboInsertPlaceholder);
            this.panelInsert.Controls.Add(this.btnInsertPlaceholder);
            this.panelInsert.Controls.Add(this.btnRestoreDefaultTemplate);
            this.panelInsert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInsert.Location = new System.Drawing.Point(0, 21);
            this.panelInsert.Margin = new System.Windows.Forms.Padding(0);
            this.panelInsert.Name = "panelInsert";
            this.panelInsert.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.panelInsert.Size = new System.Drawing.Size(726, 36);
            this.panelInsert.TabIndex = 1;
            // 
            // comboInsertPlaceholder
            // 
            this.comboInsertPlaceholder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInsertPlaceholder.FormattingEnabled = true;
            this.comboInsertPlaceholder.Location = new System.Drawing.Point(3, 9);
            this.comboInsertPlaceholder.Name = "comboInsertPlaceholder";
            this.comboInsertPlaceholder.Size = new System.Drawing.Size(240, 21);
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
            this.txtAiTemplate.Location = new System.Drawing.Point(3, 60);
            this.txtAiTemplate.Multiline = true;
            this.txtAiTemplate.Name = "txtAiTemplate";
            this.txtAiTemplate.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAiTemplate.Size = new System.Drawing.Size(720, 326);
            this.txtAiTemplate.TabIndex = 2;
            this.txtAiTemplate.WordWrap = false;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panelButtons.Location = new System.Drawing.Point(3, 479);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(774, 38);
            this.panelButtons.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(696, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(615, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 540);
            this.Controls.Add(this.tableRoot);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(720, 480);
            this.Name = "FrmSettings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NppDB Settings";
            this.tableRoot.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabBehavior.ResumeLayout(false);
            this.grpBehavior.ResumeLayout(false);
            this.layoutBehaviorOptions.ResumeLayout(false);
            this.layoutBehaviorOptions.PerformLayout();
            this.pnlDbManagerFont.ResumeLayout(false);
            this.pnlDbManagerFont.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDbManagerFontScale)).EndInit();
            this.tabLlm.ResumeLayout(false);
            this.grpLlm.ResumeLayout(false);
            this.tableLlm.ResumeLayout(false);
            this.tableLlm.PerformLayout();
            this.tabAiTemplate.ResumeLayout(false);
            this.grpAiTemplate.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox grpBehavior;
        private System.Windows.Forms.TableLayoutPanel layoutBehaviorOptions;
        private System.Windows.Forms.CheckBox chkEnableDestructiveSelectInto;
        private System.Windows.Forms.CheckBox chkEnableNewTabCreation;
        private System.Windows.Forms.FlowLayoutPanel pnlDbManagerFont;
        private System.Windows.Forms.Label lblDbManagerFontScale;
        private System.Windows.Forms.NumericUpDown numDbManagerFontScale;

        private System.Windows.Forms.TabPage tabLlm;
        private System.Windows.Forms.GroupBox grpLlm;
        private System.Windows.Forms.TableLayoutPanel tableLlm;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox comboResponseLanguage;
        private System.Windows.Forms.Label lblOpenUrl;
        private System.Windows.Forms.TextBox txtOpenLlmUrl;
        private System.Windows.Forms.Label lblCustomInstructions;
        private System.Windows.Forms.RichTextBox txtCustomInstructions;

        private System.Windows.Forms.TabPage tabAiTemplate;
        private System.Windows.Forms.GroupBox grpAiTemplate;
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