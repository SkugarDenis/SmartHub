namespace RecivedDataContolRemoted
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
            this.remoteTVButton = new System.Windows.Forms.Button();
            this.RemoteAirButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // remoteTVButton
            // 
            this.remoteTVButton.Location = new System.Drawing.Point(22, 57);
            this.remoteTVButton.Name = "remoteTVButton";
            this.remoteTVButton.Size = new System.Drawing.Size(75, 23);
            this.remoteTVButton.TabIndex = 0;
            this.remoteTVButton.Text = "RemoteTV";
            this.remoteTVButton.UseVisualStyleBackColor = true;
            this.remoteTVButton.Click += new System.EventHandler(this.remoteTVButton_Click);
            // 
            // RemoteAirButton
            // 
            this.RemoteAirButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RemoteAirButton.Location = new System.Drawing.Point(142, 57);
            this.RemoteAirButton.Name = "RemoteAirButton";
            this.RemoteAirButton.Size = new System.Drawing.Size(75, 23);
            this.RemoteAirButton.TabIndex = 1;
            this.RemoteAirButton.Text = "RemoteAir";
            this.RemoteAirButton.UseVisualStyleBackColor = true;
            this.RemoteAirButton.Click += new System.EventHandler(this.RemoteAirButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 153);
            this.Controls.Add(this.RemoteAirButton);
            this.Controls.Add(this.remoteTVButton);
            this.MaximumSize = new System.Drawing.Size(261, 192);
            this.MinimumSize = new System.Drawing.Size(261, 192);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);

        }

        #endregion

        private Button remoteTVButton;
        private Button RemoteAirButton;
    }
}