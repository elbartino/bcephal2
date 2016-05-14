namespace Misp.Kernel.Ui.Office.EDraw
{
    partial class EdrawOffice
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EdrawOffice));
            this.axEDOffice1 = new AxEDOfficeLib.AxEDOffice();
            ((System.ComponentModel.ISupportInitialize)(this.axEDOffice1)).BeginInit();
            this.SuspendLayout();
            // 
            // axEDOffice1
            // 
            this.axEDOffice1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axEDOffice1.Enabled = true;
            this.axEDOffice1.Location = new System.Drawing.Point(-3, 0);
            this.axEDOffice1.Name = "axEDOffice1";
            this.axEDOffice1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axEDOffice1.OcxState")));
            this.axEDOffice1.Size = new System.Drawing.Size(150, 144);
            this.axEDOffice1.TabIndex = 0;
            // 
            // EdrawOffice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axEDOffice1);
            this.Name = "EdrawOffice";
            ((System.ComponentModel.ISupportInitialize)(this.axEDOffice1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxEDOfficeLib.AxEDOffice axEDOffice1;
        

    }
}
