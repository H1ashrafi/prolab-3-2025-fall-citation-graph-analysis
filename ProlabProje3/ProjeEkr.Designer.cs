namespace ProlabProje3
{
    partial class ProjeEkr
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlGraf = new System.Windows.Forms.Panel();
            this.txtKCore = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnHCore = new System.Windows.Forms.Button();
            this.btnKCore = new System.Windows.Forms.Button();
            this.btnGenel = new System.Windows.Forms.Button();
            this.istatstk_txt = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlGraf
            // 
            this.pnlGraf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGraf.Location = new System.Drawing.Point(0, 0);
            this.pnlGraf.Name = "pnlGraf";
            this.pnlGraf.Size = new System.Drawing.Size(1683, 742);
            this.pnlGraf.TabIndex = 0;
            this.pnlGraf.Paint += new System.Windows.Forms.PaintEventHandler(this.panelgraf_renk);
            // 
            // txtKCore
            // 
            this.txtKCore.Location = new System.Drawing.Point(36, 150);
            this.txtKCore.Name = "txtKCore";
            this.txtKCore.Size = new System.Drawing.Size(121, 22);
            this.txtKCore.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.istatstk_txt);
            this.panel1.Controls.Add(this.txtKCore);
            this.panel1.Controls.Add(this.btnHCore);
            this.panel1.Controls.Add(this.btnKCore);
            this.panel1.Controls.Add(this.btnGenel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 742);
            this.panel1.TabIndex = 2;
            // 
            // btnHCore
            // 
            this.btnHCore.Location = new System.Drawing.Point(36, 208);
            this.btnHCore.Name = "btnHCore";
            this.btnHCore.Size = new System.Drawing.Size(121, 23);
            this.btnHCore.TabIndex = 6;
            this.btnHCore.Text = "H-Core Analizi";
            this.btnHCore.UseVisualStyleBackColor = true;
            this.btnHCore.Click += new System.EventHandler(this.btnHCore_Click_1);
            // 
            // btnKCore
            // 
            this.btnKCore.Location = new System.Drawing.Point(36, 121);
            this.btnKCore.Name = "btnKCore";
            this.btnKCore.Size = new System.Drawing.Size(121, 23);
            this.btnKCore.TabIndex = 1;
            this.btnKCore.Text = "K-Core Analizi";
            this.btnKCore.UseVisualStyleBackColor = true;
            // 
            // btnGenel
            // 
            this.btnGenel.Location = new System.Drawing.Point(36, 62);
            this.btnGenel.Name = "btnGenel";
            this.btnGenel.Size = new System.Drawing.Size(121, 23);
            this.btnGenel.TabIndex = 0;
            this.btnGenel.Text = "Genel Çizim";
            this.btnGenel.UseVisualStyleBackColor = true;
            this.btnGenel.Click += new System.EventHandler(this.btnGenel_Click_1);
            // 
            // istatstk_txt
            // 
            this.istatstk_txt.BackColor = System.Drawing.SystemColors.ControlDark;
            this.istatstk_txt.Location = new System.Drawing.Point(12, 321);
            this.istatstk_txt.Name = "istatstk_txt";
            this.istatstk_txt.ReadOnly = true;
            this.istatstk_txt.Size = new System.Drawing.Size(173, 286);
            this.istatstk_txt.TabIndex = 7;
            this.istatstk_txt.Text = "Veriler Alınıyor!";
            // 
            // ProjeEkr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1683, 742);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlGraf);
            this.Name = "ProjeEkr";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ProjeEkr_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlGraf;
        private System.Windows.Forms.TextBox txtKCore;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnKCore;
        private System.Windows.Forms.Button btnGenel;
        private System.Windows.Forms.Button btnHCore;
        private System.Windows.Forms.RichTextBox istatstk_txt;
    }
}

