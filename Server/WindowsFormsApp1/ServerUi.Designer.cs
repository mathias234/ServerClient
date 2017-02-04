namespace ServerUI {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.StartServer = new System.Windows.Forms.Button();
            this.PlayersOnline = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartServer
            // 
            this.StartServer.Location = new System.Drawing.Point(731, 429);
            this.StartServer.Name = "StartServer";
            this.StartServer.Size = new System.Drawing.Size(226, 74);
            this.StartServer.TabIndex = 0;
            this.StartServer.Text = "Start Server";
            this.StartServer.UseVisualStyleBackColor = true;
            this.StartServer.Click += new System.EventHandler(this.StartServer_Click);
            // 
            // PlayersOnline
            // 
            this.PlayersOnline.FormattingEnabled = true;
            this.PlayersOnline.Location = new System.Drawing.Point(1, 26);
            this.PlayersOnline.Name = "PlayersOnline";
            this.PlayersOnline.Size = new System.Drawing.Size(120, 459);
            this.PlayersOnline.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Players Online";
            // 
            // Timer1
            // 
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1, 488);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Lookup";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 515);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PlayersOnline);
            this.Controls.Add(this.StartServer);
            this.Name = "Form1";
            this.Text = "Server UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartServer;
        private System.Windows.Forms.ListBox PlayersOnline;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.Button button1;
    }
}

