using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Sourcing.Designer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.Designer
{
    /// <summary>
    /// Interaction logic for ApplyDesignDialog.xaml
    /// </summary>
    public partial class ApplyDesignDialog : Window
    {
        public bool requestApplyDesign;
        public bool requestViewDesign;
        public bool requestCancelDesign;

        public DesignerForm designForm;
        public Design design;
        public Periodicity periodicity;
        public bool showHeader;
        public ApplyDesignDialog()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.panelSheet.Visibility = Visibility.Collapsed;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            requestCancelDesign = true;
            this.Close();
        }

        private void OnViewDesign(object sender, RoutedEventArgs e)
        {
            requestViewDesign = true;
            if (this.panelSheet.IsVisible)
            {
                this.panelSheet.Visibility = Visibility.Collapsed;
                buttonViewDesign.Content = "View Design";
                Height = 170;
                designForm.SpreadSheet.Close();
                designForm = null;

            }
            else 
            {
                buttonViewDesign.Content = "Hide Design";
                this.panelSheet.Visibility = Visibility.Visible;
                if (designForm == null)
                {
                    designForm = new DesignerForm();
                    designForm.EditedObject = design;
                    designForm.periodicity = periodicity;
                    designForm.BuildSheetTableWithoutFill();
                }
                this.previewDesignBlock.Content = designForm;
                Height = 560;    
            }
        }

        private void OnApplyDesign(object sender, RoutedEventArgs e)
        {
            requestApplyDesign = true;
            this.Close();

        }

        private void OnShowHeader(object sender, RoutedEventArgs e)
        {
            showHeader = true;
        }
        private void OnHideHeader(object sender, RoutedEventArgs e)
        {
            showHeader = false;
        }
    }
}
