namespace WebPMaker {
    partial class Mask_Progrs {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.progrs_Text = new System.Windows.Forms.TextBox();
            this.progrs_Bar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // progrs_Text
            // 
            this.progrs_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progrs_Text.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.progrs_Text.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.progrs_Text.HideSelection = false;
            this.progrs_Text.Location = new System.Drawing.Point(280, 306);
            this.progrs_Text.Name = "progrs_Text";
            this.progrs_Text.Size = new System.Drawing.Size(496, 20);
            this.progrs_Text.TabIndex = 3;
            this.progrs_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // progrs_Bar
            // 
            this.progrs_Bar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progrs_Bar.Location = new System.Drawing.Point(279, 265);
            this.progrs_Bar.Name = "progrs_Bar";
            this.progrs_Bar.Size = new System.Drawing.Size(497, 25);
            this.progrs_Bar.TabIndex = 2;
            // 
            // Mask_Progrs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Controls.Add(this.progrs_Text);
            this.Controls.Add(this.progrs_Bar);
            this.Name = "Mask_Progrs";
            this.Size = new System.Drawing.Size(1030, 598);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox progrs_Text;
        private System.Windows.Forms.ProgressBar progrs_Bar;
    }
}
