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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.promptsListView = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colDesc = new System.Windows.Forms.ColumnHeader();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.showTablePromptsCheckbox = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.panelLeftActions = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDuplicate = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.promptTextBox = new System.Windows.Forms.RichTextBox();
            this.splitterPreview = new System.Windows.Forms.Splitter();
            this.panelPreviewBottom = new System.Windows.Forms.Panel();
            this.flowLayoutPanelPlaceholders = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPlaceholders = new System.Windows.Forms.Label();
            this.panelRightActions = new System.Windows.Forms.Panel();
            this.disableTemplatingCheckbox = new System.Windows.Forms.CheckBox();
            this.buttonCopy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.panelLeftActions.SuspendLayout();
            this.grpPreview.SuspendLayout();
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
            this.splitContainerMain.Panel1.Controls.Add(this.promptsListView);
            this.splitContainerMain.Panel1.Controls.Add(this.panelSearch);
            this.splitContainerMain.Panel1.Controls.Add(this.panelLeftActions);
            this.splitContainerMain.Panel1MinSize = 250;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.grpPreview);
            this.splitContainerMain.Panel2.Controls.Add(this.panelRightActions);
            this.splitContainerMain.Panel2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.splitContainerMain.Panel2MinSize = 430;
            this.splitContainerMain.Size = new System.Drawing.Size(969, 500);
            this.splitContainerMain.SplitterDistance = 328;
            this.splitContainerMain.TabIndex = 0;
            // 
            // promptsListView
            // 
            this.promptsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.colName, this.colDesc });
            this.promptsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.promptsListView.FullRowSelect = true;
            this.promptsListView.GridLines = true;
            this.promptsListView.HideSelection = false;
            this.promptsListView.Location = new System.Drawing.Point(0, 65);
            this.promptsListView.MinimumSize = new System.Drawing.Size(200, 120);
            this.promptsListView.MultiSelect = false;
            this.promptsListView.Name = "promptsListView";
            this.promptsListView.Size = new System.Drawing.Size(328, 395);
            this.promptsListView.TabIndex = 1;
            this.promptsListView.UseCompatibleStateImageBehavior = false;
            this.promptsListView.View = System.Windows.Forms.View.Details;
            this.promptsListView.SelectedIndexChanged += new System.EventHandler(this.promptsListView_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Text = "Prompt Name";
            this.colName.Width = 140;
            // 
            // colDesc
            // 
            this.colDesc.Text = "Description";
            this.colDesc.Width = 160;
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.showTablePromptsCheckbox);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.lblSearch);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(328, 65);
            this.panelSearch.TabIndex = 0;
            // 
            // showTablePromptsCheckbox
            // 
            this.showTablePromptsCheckbox.Location = new System.Drawing.Point(4, 35);
            this.showTablePromptsCheckbox.Name = "showTablePromptsCheckbox";
            this.showTablePromptsCheckbox.Size = new System.Drawing.Size(313, 24);
            this.showTablePromptsCheckbox.TabIndex = 2;
            this.showTablePromptsCheckbox.Text = "Show Schema-Aware Prompts (from DB Manager)\r\n";
            this.showTablePromptsCheckbox.UseVisualStyleBackColor = true;
            this.showTablePromptsCheckbox.CheckedChanged += new System.EventHandler(this.ShowTablePromptCheckbox_CheckedChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(55, 7);
            this.txtSearch.MinimumSize = new System.Drawing.Size(100, 22);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(270, 22);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(4, 10);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            // 
            // panelLeftActions
            // 
            this.panelLeftActions.Controls.Add(this.buttonAdd);
            this.panelLeftActions.Controls.Add(this.buttonDuplicate);
            this.panelLeftActions.Controls.Add(this.buttonEdit);
            this.panelLeftActions.Controls.Add(this.buttonDelete);
            this.panelLeftActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLeftActions.Location = new System.Drawing.Point(0, 460);
            this.panelLeftActions.MinimumSize = new System.Drawing.Size(250, 40);
            this.panelLeftActions.Name = "panelLeftActions";
            this.panelLeftActions.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panelLeftActions.Size = new System.Drawing.Size(328, 40);
            this.panelLeftActions.TabIndex = 2;
            this.panelLeftActions.WrapContents = false;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(3, 8);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 28);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add New";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Location = new System.Drawing.Point(84, 8);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(75, 28);
            this.buttonDuplicate.TabIndex = 3;
            this.buttonDuplicate.Text = "Duplicate";
            this.buttonDuplicate.UseVisualStyleBackColor = true;
            this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(165, 8);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(75, 28);
            this.buttonEdit.TabIndex = 0;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(246, 8);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 28);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // grpPreview
            // 
            this.grpPreview.Controls.Add(this.promptTextBox);
            this.grpPreview.Controls.Add(this.splitterPreview);
            this.grpPreview.Controls.Add(this.panelPreviewBottom);
            this.grpPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPreview.Location = new System.Drawing.Point(5, 0);
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.Size = new System.Drawing.Size(632, 460);
            this.grpPreview.TabIndex = 0;
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = "Prompt Preview";
            // 
            // promptTextBox
            // 
            this.promptTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.promptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.promptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.promptTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptTextBox.Location = new System.Drawing.Point(3, 18);
            this.promptTextBox.Name = "promptTextBox";
            this.promptTextBox.ReadOnly = true;
            this.promptTextBox.Size = new System.Drawing.Size(626, 295);
            this.promptTextBox.TabIndex = 0;
            this.promptTextBox.Text = "";
            // 
            // splitterPreview
            // 
            this.splitterPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterPreview.Location = new System.Drawing.Point(3, 313);
            this.splitterPreview.MinExtra = 120;
            this.splitterPreview.MinSize = 80;
            this.splitterPreview.Name = "splitterPreview";
            this.splitterPreview.Size = new System.Drawing.Size(626, 6);
            this.splitterPreview.TabIndex = 1;
            this.splitterPreview.TabStop = false;
            // 
            // panelPreviewBottom
            // 
            this.panelPreviewBottom.Controls.Add(this.flowLayoutPanelPlaceholders);
            this.panelPreviewBottom.Controls.Add(this.lblPlaceholders);
            this.panelPreviewBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPreviewBottom.Location = new System.Drawing.Point(3, 319);
            this.panelPreviewBottom.Name = "panelPreviewBottom";
            this.panelPreviewBottom.Size = new System.Drawing.Size(626, 138);
            this.panelPreviewBottom.TabIndex = 1;
            // 
            // flowLayoutPanelPlaceholders
            // 
            this.flowLayoutPanelPlaceholders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelPlaceholders.AutoScroll = true;
            this.flowLayoutPanelPlaceholders.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelPlaceholders.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanelPlaceholders.MinimumSize = new System.Drawing.Size(200, 80);
            this.flowLayoutPanelPlaceholders.Name = "flowLayoutPanelPlaceholders";
            this.flowLayoutPanelPlaceholders.Size = new System.Drawing.Size(620, 113);
            this.flowLayoutPanelPlaceholders.TabIndex = 6;
            this.flowLayoutPanelPlaceholders.WrapContents = false;
            // 
            // lblPlaceholders
            // 
            this.lblPlaceholders.AutoSize = true;
            this.lblPlaceholders.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPlaceholders.Location = new System.Drawing.Point(3, 3);
            this.lblPlaceholders.Name = "lblPlaceholders";
            this.lblPlaceholders.Size = new System.Drawing.Size(93, 13);
            this.lblPlaceholders.TabIndex = 5;
            this.lblPlaceholders.Text = "Fill Placeholders:";
            // 
            // panelRightActions
            // 
            this.panelRightActions.Controls.Add(this.disableTemplatingCheckbox);
            this.panelRightActions.Controls.Add(this.buttonCopy);
            this.panelRightActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRightActions.Location = new System.Drawing.Point(5, 460);
            this.panelRightActions.MinimumSize = new System.Drawing.Size(430, 40);
            this.panelRightActions.Name = "panelRightActions";
            this.panelRightActions.Size = new System.Drawing.Size(632, 40);
            this.panelRightActions.TabIndex = 1;
            // 
            // disableTemplatingCheckbox
            // 
            this.disableTemplatingCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.disableTemplatingCheckbox.AutoEllipsis = true;
            this.disableTemplatingCheckbox.Location = new System.Drawing.Point(6, 11);
            this.disableTemplatingCheckbox.Name = "disableTemplatingCheckbox";
            this.disableTemplatingCheckbox.Size = new System.Drawing.Size(260, 17);
            this.disableTemplatingCheckbox.TabIndex = 5;
            this.disableTemplatingCheckbox.Text = "Show template names instead of contents";
            this.disableTemplatingCheckbox.UseVisualStyleBackColor = true;
            this.disableTemplatingCheckbox.CheckedChanged += new System.EventHandler(this.disableTemplatingCheckbox_CheckedChanged);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopy.AutoEllipsis = true;
            this.buttonCopy.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonCopy.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonCopy.Location = new System.Drawing.Point(275, 6);
            this.buttonCopy.MinimumSize = new System.Drawing.Size(140, 28);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(354, 28);
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
            this.ClientSize = new System.Drawing.Size(989, 520);
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(740, 460);
            this.Name = "FrmPromptLibrary";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Prompt Manager";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelLeftActions.ResumeLayout(false);
            this.grpPreview.ResumeLayout(false);
            this.panelPreviewBottom.ResumeLayout(false);
            this.panelPreviewBottom.PerformLayout();
            this.panelRightActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button buttonDuplicate;

        private System.Windows.Forms.CheckBox showTablePromptsCheckbox;

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.ListView promptsListView;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colDesc;
        private System.Windows.Forms.FlowLayoutPanel panelLeftActions;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;

        private System.Windows.Forms.GroupBox grpPreview;
        private System.Windows.Forms.RichTextBox promptTextBox;
        private System.Windows.Forms.Splitter splitterPreview;
        private System.Windows.Forms.Panel panelPreviewBottom;
        private System.Windows.Forms.Label lblPlaceholders;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelPlaceholders;

        private System.Windows.Forms.Panel panelRightActions;
        private System.Windows.Forms.CheckBox disableTemplatingCheckbox;
        private System.Windows.Forms.Button buttonCopy;

        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
    }
}