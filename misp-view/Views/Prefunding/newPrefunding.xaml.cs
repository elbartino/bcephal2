using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace misp_view.Views.Prefunding
{
    /// <summary>
    /// Logique d'interaction pour newPrefunding.xaml
    /// </summary>
    public partial class newPrefunding : UserControl
    {
        public newPrefunding()
        {
            InitializeComponent();
        }


        private void tbPrefRequest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnClick(object sender, RoutedEventArgs e)
        {
           

            // A path to export a report.
            string reportPath = @"c:\\Temp\Test.pdf";

            using (XtraReport1 report = new XtraReport1())
            {
                // Specify PDF-specific export options.
                PdfExportOptions pdfOptions = report.ExportOptions.Pdf;

                // Specify the pages to be exported.
                pdfOptions.PageRange = "1, 3-5";

                // Specify the quality of exported images.
                pdfOptions.ConvertImagesToJpeg = false;
                pdfOptions.ImageQuality = PdfJpegImageQuality.Medium;

                // Specify the PDF/A-compatibility.
                pdfOptions.PdfACompatibility = PdfACompatibility.PdfA3b;

                // The following options are not compatible with PDF/A.
                // The use of these options will result in errors on PDF validation.
                //pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
                //pdfOptions.ShowPrintDialogOnOpen = true;

                // If required, you can specify the security and signature options. 
                //pdfOptions.PasswordSecurityOptions
                //pdfOptions.SignatureOptions

                // If required, specify necessary metadata and attachments
                // (e.g., to produce a ZUGFeRD-compatible PDF).
                //pdfOptions.AdditionalMetadata
                //pdfOptions.Attachments

                // Specify the document options.
                pdfOptions.DocumentOptions.Application = "Test Application";
                pdfOptions.DocumentOptions.Author = "DX Documentation Team";
                pdfOptions.DocumentOptions.Keywords = "DevExpress, Reporting, PDF";
                pdfOptions.DocumentOptions.Producer = Environment.UserName.ToString();
                pdfOptions.DocumentOptions.Subject = "Document Subject";
                pdfOptions.DocumentOptions.Title = "Document Title";


                report.ExportToPdf(reportPath, pdfOptions);

                
            }
        }
    }
}

