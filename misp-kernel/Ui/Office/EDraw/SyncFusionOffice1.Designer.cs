using Syncfusion.XlsIO;
namespace Misp.Kernel.Ui.Office.EDraw
{
    partial class SyncFusionOffice1
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
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcelEngine));
            this.excelEngine1 = new ExcelEngine();
            this.SuspendLayout();
            this.Name = "SyncFusionExcel";
            //IApplication   newExcelApp = new this.excelEngine1.Excel.Application();
            //this.cont
            ////this.Controls.Add(this.excelEngine1);
            //components.Add(this.excelEngine1.Excel.Workbooks[0]);
        }

        private ExcelEngine excelEngine1;
        #endregion
    }
}
