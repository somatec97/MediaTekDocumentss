
namespace MediaTekDocuments.view
{
    partial class FrmAuthentification
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txbLogin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txbPassword = new System.Windows.Forms.TextBox();
            this.btnConnexion = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnConnexion);
            this.panel1.Controls.Add(this.txbPassword);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txbLogin);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(57, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(679, 372);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Utilisateur : ";
            // 
            // txbLogin
            // 
            this.txbLogin.Location = new System.Drawing.Point(277, 97);
            this.txbLogin.Name = "txbLogin";
            this.txbLogin.Size = new System.Drawing.Size(335, 26);
            this.txbLogin.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mot de passe :";
            // 
            // txbPassword
            // 
            this.txbPassword.Location = new System.Drawing.Point(277, 195);
            this.txbPassword.Name = "txbPassword";
            this.txbPassword.Size = new System.Drawing.Size(335, 26);
            this.txbPassword.TabIndex = 3;
            // 
            // btnConnexion
            // 
            this.btnConnexion.Location = new System.Drawing.Point(57, 285);
            this.btnConnexion.Name = "btnConnexion";
            this.btnConnexion.Size = new System.Drawing.Size(555, 51);
            this.btnConnexion.TabIndex = 4;
            this.btnConnexion.Text = "Connexion";
            this.btnConnexion.UseVisualStyleBackColor = true;
            this.btnConnexion.Click += new System.EventHandler(this.btnConnexion_Click);
            // 
            // FrmAuthentification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "FrmAuthentification";
            this.Text = "FrmAuthentification";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnConnexion;
        private System.Windows.Forms.TextBox txbPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbLogin;
        private System.Windows.Forms.Label label1;
    }
}