using System.ComponentModel;
using System.Windows.Forms;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.promptsGridView = new System.Windows.Forms.DataGridView();
            this.colPromptName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromptDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromptType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.cmbPromptSource = new System.Windows.Forms.ComboBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.buttonClearSearch = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.panelLeftActions = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDuplicate = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.promptTextBox = new System.Windows.Forms.RichTextBox();
            this.panelEditFields = new System.Windows.Forms.Panel();
            this.txtPromptDescription = new System.Windows.Forms.TextBox();
            this.lblPromptDescription = new System.Windows.Forms.Label();
            this.txtPromptName = new System.Windows.Forms.TextBox();
            this.lblPromptName = new System.Windows.Forms.Label();
            this.panelPromptTags = new System.Windows.Forms.Panel();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.panelPromptMeta = new System.Windows.Forms.Panel();
            this.panelWorkflowHint = new System.Windows.Forms.Panel();
            this.lblWorkflowHint = new System.Windows.Forms.Label();
            this.panelMetaRight = new System.Windows.Forms.Panel();
            this.editingModeCheckbox = new System.Windows.Forms.CheckBox();
            this.lblEditingBadge = new System.Windows.Forms.Label();
            this.buttonAiStudio = new System.Windows.Forms.Button();
            this.buttonTogglePreview = new System.Windows.Forms.Button();
            this.lblPromptCapabilities = new System.Windows.Forms.Label();
            this.lblPromptType = new System.Windows.Forms.Label();
            this.splitterPreview = new System.Windows.Forms.Splitter();
            this.panelPreviewBottom = new System.Windows.Forms.Panel();
            this.flowLayoutPanelPlaceholders = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPlaceholders = new System.Windows.Forms.Label();
            this.panelRightActions = new System.Windows.Forms.Panel();
            this.buttonCopy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.promptsGridView)).BeginInit();
            this.panelSearch.SuspendLayout();
            this.panelLeftActions.SuspendLayout();
            this.grpPreview.SuspendLayout();
            this.panelEditFields.SuspendLayout();
            this.panelPromptTags.SuspendLayout();
            this.panelPromptMeta.SuspendLayout();
            this.panelWorkflowHint.SuspendLayout();
            this.panelMetaRight.SuspendLayout();
            this.panelPreviewBottom.SuspendLayout();
            this.panelRightActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(10, 10);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.promptsGridView);
            this.splitContainerMain.Panel1.Controls.Add(this.panelSearch);
            this.splitContainerMain.Panel1.Controls.Add(this.panelLeftActions);
            this.splitContainerMain.Panel1MinSize = 300;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.grpPreview);
            this.splitContainerMain.Panel2.Controls.Add(this.panelRightActions);
            this.splitContainerMain.Panel2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.splitContainerMain.Panel2MinSize = 450;
            this.splitContainerMain.Size = new System.Drawing.Size(1250, 735);
            this.splitContainerMain.SplitterDistance = 380;
            this.splitContainerMain.TabIndex = 0;
            // 
            // promptsGridView
            // 
            this.promptsGridView.AllowUserToAddRows = false;
            this.promptsGridView.AllowUserToDeleteRows = false;
            this.promptsGridView.AllowUserToResizeRows = false;
            this.promptsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.promptsGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.promptsGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.promptsGridView.ColumnHeadersHeight = 30;
            this.promptsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.promptsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colPromptName, this.colPromptDesc, this.colPromptType });
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(10, 8, 10, 6);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.promptsGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.promptsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.promptsGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.promptsGridView.Location = new System.Drawing.Point(0, 70);
            this.promptsGridView.MinimumSize = new System.Drawing.Size(200, 120);
            this.promptsGridView.MultiSelect = false;
            this.promptsGridView.Name = "promptsGridView";
            this.promptsGridView.ReadOnly = true;
            this.promptsGridView.RowHeadersVisible = false;
            this.promptsGridView.RowTemplate.Height = 58;
            this.promptsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.promptsGridView.Size = new System.Drawing.Size(380, 617);
            this.promptsGridView.TabIndex = 1;
            this.promptsGridView.SelectionChanged += new System.EventHandler(this.promptsListView_SelectedIndexChanged);
            // 
            // colPromptName
            // 
            this.colPromptName.HeaderText = "Prompt Name ↕";
            this.colPromptName.MinimumWidth = 160;
            this.colPromptName.Name = "colPromptName";
            this.colPromptName.ReadOnly = true;
            this.colPromptName.Width = 170;
            // 
            // colPromptDesc
            // 
            this.colPromptDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPromptDesc.HeaderText = "Description ↕";
            this.colPromptDesc.Name = "colPromptDesc";
            this.colPromptDesc.ReadOnly = true;
            // 
            // colPromptType
            // 
            this.colPromptType.HeaderText = "Type";
            this.colPromptType.Name = "colPromptType";
            this.colPromptType.ReadOnly = true;
            this.colPromptType.Width = 95;
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.cmbPromptSource);
            this.panelSearch.Controls.Add(this.lblSource);
            this.panelSearch.Controls.Add(this.buttonClearSearch);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.lblSearch);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.panelSearch.Size = new System.Drawing.Size(380, 70);
            this.panelSearch.TabIndex = 0;
            // 
            // cmbPromptSource
            // 
            this.cmbPromptSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPromptSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPromptSource.FormattingEnabled = true;
            this.cmbPromptSource.Location = new System.Drawing.Point(60, 40);
            this.cmbPromptSource.Name = "cmbPromptSource";
            this.cmbPromptSource.Size = new System.Drawing.Size(320, 21);
            this.cmbPromptSource.TabIndex = 3;
            this.cmbPromptSource.SelectedIndexChanged += new System.EventHandler(this.cmbPromptSource_SelectedIndexChanged);
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(4, 43);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(45, 13);
            this.lblSource.TabIndex = 2;
            this.lblSource.Text = "Source:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(60, 10);
            this.txtSearch.MinimumSize = new System.Drawing.Size(100, 22);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(294, 22);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // buttonClearSearch
            // 
            this.buttonClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearSearch.FlatAppearance.BorderSize = 0;
            this.buttonClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClearSearch.Location = new System.Drawing.Point(357, 10);
            this.buttonClearSearch.BackgroundImage = global::NppDB.Core.Properties.Resources.x_letter1;
            this.buttonClearSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonClearSearch.Margin = new System.Windows.Forms.Padding(0);
            this.buttonClearSearch.Name = "buttonClearSearch";
            this.buttonClearSearch.Size = new System.Drawing.Size(23, 22);
            this.buttonClearSearch.TabIndex = 4;
            this.buttonClearSearch.TabStop = false;
            this.buttonClearSearch.UseVisualStyleBackColor = true;
            this.buttonClearSearch.Visible = false;
            this.buttonClearSearch.Click += new System.EventHandler(this.buttonClearSearch_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(4, 13);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            // 
            // panelLeftActions
            // 
            this.panelLeftActions.Controls.Add(this.buttonAdd);
            this.panelLeftActions.Controls.Add(this.buttonDuplicate);
            this.panelLeftActions.Controls.Add(this.buttonDelete);
            this.panelLeftActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLeftActions.Location = new System.Drawing.Point(0, 687);
            this.panelLeftActions.MinimumSize = new System.Drawing.Size(300, 48);
            this.panelLeftActions.Name = "panelLeftActions";
            this.panelLeftActions.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.panelLeftActions.Size = new System.Drawing.Size(380, 48);
            this.panelLeftActions.TabIndex = 2;
            this.panelLeftActions.WrapContents = false;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(0, 8);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(88, 30);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add New";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Location = new System.Drawing.Point(96, 8);
            this.buttonDuplicate.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(88, 30);
            this.buttonDuplicate.TabIndex = 3;
            this.buttonDuplicate.Text = "Duplicate";
            this.buttonDuplicate.UseVisualStyleBackColor = true;
            this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(192, 8);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(88, 30);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // grpPreview
            // 
            this.grpPreview.Controls.Add(this.promptTextBox);
            this.grpPreview.Controls.Add(this.panelPromptTags);
            this.grpPreview.Controls.Add(this.panelEditFields);
            this.grpPreview.Controls.Add(this.panelWorkflowHint);
            this.grpPreview.Controls.Add(this.panelPromptMeta);
            this.grpPreview.Controls.Add(this.splitterPreview);
            this.grpPreview.Controls.Add(this.panelPreviewBottom);
            this.grpPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPreview.Location = new System.Drawing.Point(5, 0);
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.Size = new System.Drawing.Size(861, 687);
            this.grpPreview.TabIndex = 0;
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = "Prompt Details";
            // 
            // promptTextBox
            // 
            this.promptTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.promptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.promptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.promptTextBox.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptTextBox.Location = new System.Drawing.Point(3, 158);
            this.promptTextBox.Name = "promptTextBox";
            this.promptTextBox.ReadOnly = true;
            this.promptTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.promptTextBox.Size = new System.Drawing.Size(855, 274);
            this.promptTextBox.TabIndex = 0;
            this.promptTextBox.Text = "";
            // 
            // 
            // panelEditFields
            // 
            this.panelEditFields.Controls.Add(this.txtPromptDescription);
            this.panelEditFields.Controls.Add(this.lblPromptDescription);
            this.panelEditFields.Controls.Add(this.txtPromptName);
            this.panelEditFields.Controls.Add(this.lblPromptName);
            this.panelEditFields.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEditFields.Location = new System.Drawing.Point(3, 66);
            this.panelEditFields.Name = "panelEditFields";
            this.panelEditFields.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.panelEditFields.Size = new System.Drawing.Size(855, 58);
            this.panelEditFields.TabIndex = 7;
            this.panelEditFields.Visible = false;
            // 
            // txtPromptDescription
            // 
            this.txtPromptDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPromptDescription.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtPromptDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPromptDescription.Location = new System.Drawing.Point(78, 31);
            this.txtPromptDescription.Name = "txtPromptDescription";
            this.txtPromptDescription.Size = new System.Drawing.Size(777, 22);
            this.txtPromptDescription.TabIndex = 3;
            // 
            // lblPromptDescription
            // 
            this.lblPromptDescription.AutoSize = true;
            this.lblPromptDescription.Location = new System.Drawing.Point(0, 34);
            this.lblPromptDescription.Name = "lblPromptDescription";
            this.lblPromptDescription.Size = new System.Drawing.Size(63, 13);
            this.lblPromptDescription.TabIndex = 2;
            this.lblPromptDescription.Text = "Description";
            // 
            // txtPromptName
            // 
            this.txtPromptName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPromptName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtPromptName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPromptName.Location = new System.Drawing.Point(78, 3);
            this.txtPromptName.Name = "txtPromptName";
            this.txtPromptName.Size = new System.Drawing.Size(777, 22);
            this.txtPromptName.TabIndex = 1;
            // 
            // lblPromptName
            // 
            this.lblPromptName.AutoSize = true;
            this.lblPromptName.Location = new System.Drawing.Point(0, 6);
            this.lblPromptName.Name = "lblPromptName";
            this.lblPromptName.Size = new System.Drawing.Size(35, 13);
            this.lblPromptName.TabIndex = 0;
            this.lblPromptName.Text = "Name";
            // 
            // panelPromptTags
            // 
            this.panelPromptTags.Controls.Add(this.txtTags);
            this.panelPromptTags.Controls.Add(this.lblTags);
            this.panelPromptTags.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPromptTags.Location = new System.Drawing.Point(3, 124);
            this.panelPromptTags.Name = "panelPromptTags";
            this.panelPromptTags.Size = new System.Drawing.Size(855, 34);
            this.panelPromptTags.TabIndex = 6;
            // 
            // txtTags
            // 
            this.txtTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTags.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtTags.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTags.Location = new System.Drawing.Point(47, 7);
            this.txtTags.Name = "txtTags";
            this.txtTags.ReadOnly = true;
            this.txtTags.Size = new System.Drawing.Size(685, 15);
            this.txtTags.TabIndex = 1;
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Location = new System.Drawing.Point(0, 7);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(32, 13);
            this.lblTags.TabIndex = 0;
            this.lblTags.Text = "Tags:";
            // 
            // panelWorkflowHint
            // 
            this.panelWorkflowHint.Controls.Add(this.lblWorkflowHint);
            this.panelWorkflowHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWorkflowHint.Location = new System.Drawing.Point(3, 42);
            this.panelWorkflowHint.Name = "panelWorkflowHint";
            this.panelWorkflowHint.Padding = new System.Windows.Forms.Padding(0, 3, 0, 1);
            this.panelWorkflowHint.Size = new System.Drawing.Size(855, 24);
            this.panelWorkflowHint.TabIndex = 8;
            // 
            // lblWorkflowHint
            // 
            this.lblWorkflowHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWorkflowHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblWorkflowHint.Location = new System.Drawing.Point(0, 3);
            this.lblWorkflowHint.Name = "lblWorkflowHint";
            this.lblWorkflowHint.Size = new System.Drawing.Size(855, 20);
            this.lblWorkflowHint.TabIndex = 0;
            this.lblWorkflowHint.Text = "1. Select a prompt   2. Fill missing inputs   3. Copy prompt / Open AI Chat";
            this.lblWorkflowHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelPromptMeta
            // 
            this.panelPromptMeta.Controls.Add(this.panelMetaRight);
            this.panelPromptMeta.Controls.Add(this.lblPromptCapabilities);
            this.panelPromptMeta.Controls.Add(this.lblPromptType);
            this.panelPromptMeta.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPromptMeta.Location = new System.Drawing.Point(3, 18);
            this.panelPromptMeta.Name = "panelPromptMeta";
            this.panelPromptMeta.Size = new System.Drawing.Size(855, 24);
            this.panelPromptMeta.TabIndex = 5;
            // 
            // panelMetaRight
            // 
            this.panelMetaRight.Controls.Add(this.editingModeCheckbox);
            this.panelMetaRight.Controls.Add(this.lblEditingBadge);
            this.panelMetaRight.Controls.Add(this.buttonTogglePreview);
            this.panelMetaRight.Controls.Add(this.buttonAiStudio);
            this.panelMetaRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelMetaRight.Location = new System.Drawing.Point(435, 0);
            this.panelMetaRight.Name = "panelMetaRight";
            this.panelMetaRight.Size = new System.Drawing.Size(420, 24);
            this.panelMetaRight.TabIndex = 10;
            // 
            // editingModeCheckbox
            // 
            this.editingModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.editingModeCheckbox.AutoEllipsis = true;
            this.editingModeCheckbox.Dock = System.Windows.Forms.DockStyle.Left;
            this.editingModeCheckbox.Location = new System.Drawing.Point(78, 0);
            this.editingModeCheckbox.Margin = new System.Windows.Forms.Padding(0);
            this.editingModeCheckbox.Name = "editingModeCheckbox";
            this.editingModeCheckbox.Size = new System.Drawing.Size(90, 24);
            this.editingModeCheckbox.TabIndex = 3;
            this.editingModeCheckbox.Text = "Edit";
            this.editingModeCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.editingModeCheckbox.UseVisualStyleBackColor = true;
            this.editingModeCheckbox.CheckedChanged += new System.EventHandler(this.editingModeCheckbox_CheckedChanged);
            // 
            // lblEditingBadge
            // 
            this.lblEditingBadge.BackColor = System.Drawing.SystemColors.Highlight;
            this.lblEditingBadge.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblEditingBadge.ForeColor = System.Drawing.Color.White;
            this.lblEditingBadge.Location = new System.Drawing.Point(0, 0);
            this.lblEditingBadge.Margin = new System.Windows.Forms.Padding(0);
            this.lblEditingBadge.Name = "lblEditingBadge";
            this.lblEditingBadge.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblEditingBadge.Size = new System.Drawing.Size(78, 24);
            this.lblEditingBadge.TabIndex = 999;
            this.lblEditingBadge.Text = "Preview";
            this.lblEditingBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEditingBadge.ForeColorChanged += new System.EventHandler(this.lblEditingBadge_forceWhiteText);
            // 
            // buttonAiStudio
            // 
            this.buttonAiStudio.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonAiStudio.Location = new System.Drawing.Point(300, 0);
            this.buttonAiStudio.Name = "buttonAiStudio";
            this.buttonAiStudio.Size = new System.Drawing.Size(120, 24);
            this.buttonAiStudio.TabIndex = 2;
            this.buttonAiStudio.Text = "Open AI Chat";
            this.buttonAiStudio.UseVisualStyleBackColor = true;
            this.buttonAiStudio.Click += new System.EventHandler(this.buttonAiStudio_Click);
            // 
            // buttonTogglePreview
            // 
            this.buttonTogglePreview.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonTogglePreview.Location = new System.Drawing.Point(190, 0);
            this.buttonTogglePreview.Name = "buttonTogglePreview";
            this.buttonTogglePreview.Size = new System.Drawing.Size(110, 24);
            this.buttonTogglePreview.TabIndex = 4;
            this.buttonTogglePreview.Text = "Collapse Preview";
            this.buttonTogglePreview.UseVisualStyleBackColor = true;
            this.buttonTogglePreview.Click += new System.EventHandler(this.buttonTogglePreview_Click);
            // 
            // lblPromptCapabilities
            // 
            this.lblPromptCapabilities.AutoEllipsis = true;
            this.lblPromptCapabilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPromptCapabilities.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPromptCapabilities.Location = new System.Drawing.Point(190, 0);
            this.lblPromptCapabilities.Name = "lblPromptCapabilities";
            this.lblPromptCapabilities.Size = new System.Drawing.Size(665, 24);
            this.lblPromptCapabilities.TabIndex = 1;
            this.lblPromptCapabilities.Text = "No table selected";
            this.lblPromptCapabilities.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPromptCapabilities.Visible = false;
            // 
            // lblPromptType
            // 
            this.lblPromptType.AutoEllipsis = true;
            this.lblPromptType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPromptType.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPromptType.Location = new System.Drawing.Point(0, 0);
            this.lblPromptType.Name = "lblPromptType";
            this.lblPromptType.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblPromptType.Size = new System.Drawing.Size(435, 24);
            this.lblPromptType.TabIndex = 0;
            this.lblPromptType.Text = "No database selected";
            this.lblPromptType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPromptType.AutoEllipsis = true;
            // 
            // splitterPreview
            // 
            this.splitterPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterPreview.Location = new System.Drawing.Point(3, 428);
            this.splitterPreview.MinExtra = 120;
            this.splitterPreview.MinSize = 80;
            this.splitterPreview.Name = "splitterPreview";
            this.splitterPreview.Size = new System.Drawing.Size(855, 10);
            this.splitterPreview.TabIndex = 1;
            this.splitterPreview.TabStop = false;
            // 
            // panelPreviewBottom
            // 
            this.panelPreviewBottom.Controls.Add(this.flowLayoutPanelPlaceholders);
            this.panelPreviewBottom.Controls.Add(this.lblPlaceholders);
            this.panelPreviewBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPreviewBottom.Location = new System.Drawing.Point(3, 438);
            this.panelPreviewBottom.Name = "panelPreviewBottom";
            this.panelPreviewBottom.Size = new System.Drawing.Size(855, 246);
            this.panelPreviewBottom.TabIndex = 1;
            // 
            // flowLayoutPanelPlaceholders
            // 
            this.flowLayoutPanelPlaceholders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelPlaceholders.AutoScroll = true;
            this.flowLayoutPanelPlaceholders.BackColor = System.Drawing.SystemColors.Window;
            this.flowLayoutPanelPlaceholders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanelPlaceholders.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelPlaceholders.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanelPlaceholders.MinimumSize = new System.Drawing.Size(200, 80);
            this.flowLayoutPanelPlaceholders.Name = "flowLayoutPanelPlaceholders";
            this.flowLayoutPanelPlaceholders.Padding = new System.Windows.Forms.Padding(6);
            this.flowLayoutPanelPlaceholders.Size = new System.Drawing.Size(849, 221);
            this.flowLayoutPanelPlaceholders.TabIndex = 6;
            this.flowLayoutPanelPlaceholders.WrapContents = false;
            this.flowLayoutPanelPlaceholders.SizeChanged += new System.EventHandler(this.flowLayoutPanelPlaceholders_SizeChanged);
            // 
            // lblPlaceholders
            // 
            this.lblPlaceholders.AutoSize = true;
            this.lblPlaceholders.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaceholders.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPlaceholders.Location = new System.Drawing.Point(3, 3);
            this.lblPlaceholders.Name = "lblPlaceholders";
            this.lblPlaceholders.Size = new System.Drawing.Size(297, 17);
            this.lblPlaceholders.TabIndex = 5;
            this.lblPlaceholders.Text = "Prompt Inputs (all required, scroll if needed).";
            // 
            // panelRightActions
            // 
            this.panelRightActions.Controls.Add(this.buttonCopy);
            this.panelRightActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRightActions.Location = new System.Drawing.Point(5, 687);
            this.panelRightActions.MinimumSize = new System.Drawing.Size(430, 48);
            this.panelRightActions.Name = "panelRightActions";
            this.panelRightActions.Padding = new System.Windows.Forms.Padding(6);
            this.panelRightActions.Size = new System.Drawing.Size(861, 48);
            this.panelRightActions.TabIndex = 1;
            // 
            // buttonCopy
            // 
            this.buttonCopy.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonCopy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCopy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopy.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonCopy.Location = new System.Drawing.Point(6, 6);
            this.buttonCopy.MinimumSize = new System.Drawing.Size(140, 34);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(849, 36);
            this.buttonCopy.TabIndex = 9;
            this.buttonCopy.Text = "Copy Prompt";
            this.buttonCopy.UseVisualStyleBackColor = false;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // FrmPromptLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1320, 780);
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(15, 15);
            this.MinimumSize = new System.Drawing.Size(980, 620);
            this.Name = "FrmPromptLibrary";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Prompt Library";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.promptsGridView)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelLeftActions.ResumeLayout(false);
            this.panelEditFields.ResumeLayout(false);
            this.panelEditFields.PerformLayout();
            this.grpPreview.ResumeLayout(false);
            this.panelPromptTags.ResumeLayout(false);
            this.panelPromptTags.PerformLayout();
            this.panelPromptMeta.ResumeLayout(false);
            this.panelWorkflowHint.ResumeLayout(false);
            this.panelMetaRight.ResumeLayout(false);
            this.panelPreviewBottom.ResumeLayout(false);
            this.panelPreviewBottom.PerformLayout();
            this.panelRightActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.ComboBox cmbPromptSource;

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.DataGridView promptsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromptName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromptDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromptType;
        private System.Windows.Forms.FlowLayoutPanel panelLeftActions;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDuplicate;
        private System.Windows.Forms.Button buttonDelete;

        private System.Windows.Forms.GroupBox grpPreview;

        private System.Windows.Forms.Panel panelPromptMeta;
        private System.Windows.Forms.Panel panelWorkflowHint;
        private System.Windows.Forms.Panel panelMetaRight;
        private System.Windows.Forms.Button buttonAiStudio;
        private System.Windows.Forms.Button buttonTogglePreview;
        private System.Windows.Forms.CheckBox editingModeCheckbox;

        private System.Windows.Forms.Label lblPromptCapabilities;
        private System.Windows.Forms.Label lblPromptType;
        private System.Windows.Forms.Label lblEditingBadge;
        private System.Windows.Forms.Label lblWorkflowHint;

        private System.Windows.Forms.Panel panelPromptTags;
        private System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.Label lblTags;

        private System.Windows.Forms.RichTextBox promptTextBox;
        private System.Windows.Forms.Panel panelEditFields;
        private System.Windows.Forms.TextBox txtPromptDescription;
        private System.Windows.Forms.Label lblPromptDescription;
        private System.Windows.Forms.TextBox txtPromptName;
        private System.Windows.Forms.Label lblPromptName;

        private System.Windows.Forms.Splitter splitterPreview;
        private System.Windows.Forms.Panel panelPreviewBottom;
        private System.Windows.Forms.Label lblPlaceholders;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelPlaceholders;

        private System.Windows.Forms.Panel panelRightActions;
        private System.Windows.Forms.Button buttonCopy;

        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button buttonClearSearch;
        private System.Windows.Forms.Label lblSearch;
    }
}