using System.ComponentModel;

namespace NppDB.Core
{
    partial class FrmBehaviorSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel layoutRoot;
        private System.Windows.Forms.GroupBox grpBehavior;
        private System.Windows.Forms.TableLayoutPanel layoutOptions;
        private System.Windows.Forms.CheckBox destructiveSelectIntoCheckbox;
        private System.Windows.Forms.CheckBox newTabCheckbox;

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
            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.grpBehavior = new System.Windows.Forms.GroupBox();
            this.layoutOptions = new System.Windows.Forms.TableLayoutPanel();
            this.destructiveSelectIntoCheckbox = new System.Windows.Forms.CheckBox();
            this.newTabCheckbox = new System.Windows.Forms.CheckBox();
            this.layoutRoot.SuspendLayout();
            this.grpBehavior.SuspendLayout();
            this.layoutOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutRoot
            // 
            this.layoutRoot.ColumnCount = 1;
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Controls.Add(this.grpBehavior, 0, 0);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Location = new System.Drawing.Point(10, 10);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.RowCount = 1;
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Size = new System.Drawing.Size(382, 141);
            this.layoutRoot.TabIndex = 0;
            // 
            // grpBehavior
            // 
            this.grpBehavior.Controls.Add(this.layoutOptions);
            this.grpBehavior.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBehavior.Location = new System.Drawing.Point(3, 3);
            this.grpBehavior.Name = "grpBehavior";
            this.grpBehavior.Padding = new System.Windows.Forms.Padding(10);
            this.grpBehavior.Size = new System.Drawing.Size(376, 135);
            this.grpBehavior.TabIndex = 0;
            this.grpBehavior.TabStop = false;
            this.grpBehavior.Text = "Behavior";
            // 
            // layoutOptions
            // 
            this.layoutOptions.ColumnCount = 1;
            this.layoutOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutOptions.Controls.Add(this.destructiveSelectIntoCheckbox, 0, 0);
            this.layoutOptions.Controls.Add(this.newTabCheckbox, 0, 1);
            this.layoutOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutOptions.Location = new System.Drawing.Point(10, 23);
            this.layoutOptions.Name = "layoutOptions";
            this.layoutOptions.RowCount = 2;
            this.layoutOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.layoutOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.layoutOptions.Size = new System.Drawing.Size(356, 102);
            this.layoutOptions.TabIndex = 0;
            // 
            // destructiveSelectIntoCheckbox
            // 
            this.destructiveSelectIntoCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                                          | System.Windows.Forms.AnchorStyles.Right)));
            this.destructiveSelectIntoCheckbox.AutoEllipsis = true;
            this.destructiveSelectIntoCheckbox.Location = new System.Drawing.Point(3, 3);
            this.destructiveSelectIntoCheckbox.MinimumSize = new System.Drawing.Size(0, 28);
            this.destructiveSelectIntoCheckbox.Name = "destructiveSelectIntoCheckbox";
            this.destructiveSelectIntoCheckbox.Size = new System.Drawing.Size(350, 34);
            this.destructiveSelectIntoCheckbox.TabIndex = 0;
            this.destructiveSelectIntoCheckbox.Text = "Enable destructive SELECT INTO (MSAccess)";
            this.destructiveSelectIntoCheckbox.UseVisualStyleBackColor = true;
            this.destructiveSelectIntoCheckbox.CheckedChanged += new System.EventHandler(this.destructiveSelectIntoCheckbox_CheckedChanged);
            // 
            // newTabCheckbox
            // 
            this.newTabCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                              | System.Windows.Forms.AnchorStyles.Right)));
            this.newTabCheckbox.AutoEllipsis = true;
            this.newTabCheckbox.Location = new System.Drawing.Point(3, 43);
            this.newTabCheckbox.MinimumSize = new System.Drawing.Size(0, 28);
            this.newTabCheckbox.Name = "newTabCheckbox";
            this.newTabCheckbox.Size = new System.Drawing.Size(350, 34);
            this.newTabCheckbox.TabIndex = 1;
            this.newTabCheckbox.Text = "Create new tab every time for queries";
            this.newTabCheckbox.UseVisualStyleBackColor = true;
            this.newTabCheckbox.CheckedChanged += new System.EventHandler(this.newTabCheckbox_CheckedChanged);
            // 
            // FrmBehaviorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(402, 161);
            this.Controls.Add(this.layoutRoot);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 190);
            this.Name = "FrmBehaviorSettings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Behavior Settings";
            this.layoutRoot.ResumeLayout(false);
            this.grpBehavior.ResumeLayout(false);
            this.layoutOptions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
