using System.ComponentModel;

namespace NppDB.Core
{
    partial class FrmBehaviorSettings
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
            this.destructiveSelectIntoCheckbox = new System.Windows.Forms.CheckBox();
            this.newTabCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // destructiveSelectIntoCheckbox
            // 
            this.destructiveSelectIntoCheckbox.Location = new System.Drawing.Point(12, 12);
            this.destructiveSelectIntoCheckbox.Name = "destructiveSelectIntoCheckbox";
            this.destructiveSelectIntoCheckbox.Size = new System.Drawing.Size(248, 47);
            this.destructiveSelectIntoCheckbox.TabIndex = 0;
            this.destructiveSelectIntoCheckbox.Text = "Enable destructive SELECT INTO (MSAccess)";
            this.destructiveSelectIntoCheckbox.UseVisualStyleBackColor = true;
            this.destructiveSelectIntoCheckbox.CheckedChanged += new System.EventHandler(this.destructiveSelectIntoCheckbox_CheckedChanged);
            // 
            // newTabCheckbox
            // 
            this.newTabCheckbox.Location = new System.Drawing.Point(12, 65);
            this.newTabCheckbox.Name = "newTabCheckbox";
            this.newTabCheckbox.Size = new System.Drawing.Size(248, 47);
            this.newTabCheckbox.TabIndex = 1;
            this.newTabCheckbox.Text = "Create new tab every time for queries";
            this.newTabCheckbox.UseVisualStyleBackColor = true;
            this.newTabCheckbox.CheckedChanged += new System.EventHandler(this.newTabCheckbox_CheckedChanged);
            // 
            // FrmBehaviorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 129);
            this.Controls.Add(this.newTabCheckbox);
            this.Controls.Add(this.destructiveSelectIntoCheckbox);
            this.Name = "FrmBehaviorSettings";
            this.ShowIcon = false;
            this.Text = "Behavior Settings";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox destructiveSelectIntoCheckbox;

        private System.Windows.Forms.CheckBox newTabCheckbox;

        #endregion
    }
}