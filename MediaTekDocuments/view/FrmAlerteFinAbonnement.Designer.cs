
namespace MediaTekDocuments.view
{
    partial class FrmAlerteFinAbonnement
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
            this.dgvAbonnementsAEcheance = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonnementsAEcheance)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAbonnementsAEcheance
            // 
            this.dgvAbonnementsAEcheance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAbonnementsAEcheance.Location = new System.Drawing.Point(39, 104);
            this.dgvAbonnementsAEcheance.Name = "dgvAbonnementsAEcheance";
            this.dgvAbonnementsAEcheance.RowHeadersWidth = 62;
            this.dgvAbonnementsAEcheance.RowTemplate.Height = 28;
            this.dgvAbonnementsAEcheance.Size = new System.Drawing.Size(700, 298);
            this.dgvAbonnementsAEcheance.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(406, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "La liste des abonnements qui se terminent d\'ici 30 jours!!";
            // 
            // FrmAlerteFinAbonnement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvAbonnementsAEcheance);
            this.Name = "FrmAlerteFinAbonnement";
            this.Text = "FrmAlerteFinAbonnement";
            this.Load += new System.EventHandler(this.FrmAlerteFinAbonnement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonnementsAEcheance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAbonnementsAEcheance;
        private System.Windows.Forms.Label label1;
    }
}