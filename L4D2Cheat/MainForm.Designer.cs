namespace L4D2Cheat
{
    partial class MainForm
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
            this.cbAutoBunny = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbAutoBunny
            // 
            this.cbAutoBunny.AutoSize = true;
            this.cbAutoBunny.Location = new System.Drawing.Point(12, 12);
            this.cbAutoBunny.Name = "cbAutoBunny";
            this.cbAutoBunny.Size = new System.Drawing.Size(81, 17);
            this.cbAutoBunny.TabIndex = 0;
            this.cbAutoBunny.Text = "Auto Bunny";
            this.cbAutoBunny.UseVisualStyleBackColor = true;
            this.cbAutoBunny.CheckedChanged += new System.EventHandler(this.cbAutoBunny_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cbAutoBunny);
            this.Name = "MainForm";
            this.Text = "L4D2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbAutoBunny;
    }
}

