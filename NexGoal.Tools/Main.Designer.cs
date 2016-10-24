namespace NexGoal.Tools
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageResetScore = new System.Windows.Forms.TabPage();
            this.tabPageChangeCertificate = new System.Windows.Forms.TabPage();
            this.tabPageEncryptor = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageResetScore);
            this.tabControl.Controls.Add(this.tabPageChangeCertificate);
            this.tabControl.Controls.Add(this.tabPageEncryptor);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(494, 532);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageResetScore
            // 
            this.tabPageResetScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPageResetScore.Location = new System.Drawing.Point(4, 24);
            this.tabPageResetScore.Name = "tabPageResetScore";
            this.tabPageResetScore.Padding = new System.Windows.Forms.Padding(5);
            this.tabPageResetScore.Size = new System.Drawing.Size(486, 504);
            this.tabPageResetScore.TabIndex = 0;
            this.tabPageResetScore.Text = "Reset Score";
            this.tabPageResetScore.UseVisualStyleBackColor = true;
            // 
            // tabPageChangeCertificate
            // 
            this.tabPageChangeCertificate.Location = new System.Drawing.Point(4, 24);
            this.tabPageChangeCertificate.Name = "tabPageChangeCertificate";
            this.tabPageChangeCertificate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageChangeCertificate.Size = new System.Drawing.Size(486, 504);
            this.tabPageChangeCertificate.TabIndex = 1;
            this.tabPageChangeCertificate.Text = "Change Certificate";
            this.tabPageChangeCertificate.UseVisualStyleBackColor = true;
            // 
            // tabPageEncryptor
            // 
            this.tabPageEncryptor.Location = new System.Drawing.Point(4, 24);
            this.tabPageEncryptor.Name = "tabPageEncryptor";
            this.tabPageEncryptor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEncryptor.Size = new System.Drawing.Size(486, 504);
            this.tabPageEncryptor.TabIndex = 2;
            this.tabPageEncryptor.Text = "Encryptor";
            this.tabPageEncryptor.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 532);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NexGoal.Tools";
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageResetScore;
        private System.Windows.Forms.TabPage tabPageChangeCertificate;
        private System.Windows.Forms.TabPage tabPageEncryptor;
    }
}
