using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Misp.Reporting.Calculated_Measure
{
    /// <summary>
    /// Interaction logic for IgnorePropertiesGrid.xaml
    /// </summary>
    public partial class IgnorePropertiesGrid : Grid
    {
        public event ChangeEventHandler Changed;

        public bool isDisplayChecked = true;
        /// <summary>
        /// contruit une nvelle instance de IgnorePropertiesGrid
        /// </summary>
        public IgnorePropertiesGrid()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        /// <summary>
        /// initialize handlers
        /// </summary>
        private void InitializeHandlers()
        {
            this.ignoreAll.Checked += ignoreAll_Checked;
            this.ignoreAll.Unchecked += ignoreAll_Unchecked;
            this.ignoreCellObject.Checked += ignoreCellObject_Checked;
            this.ignoreCellPeriod.Checked += ignoreCellPeriod_Checked;
            this.ignoreCellVC.Checked += ignoreCellVC_Checked;
            this.ignoreTableObject.Checked += ignoreTableObject_Checked;
            this.ignoreTablePeriod.Checked += ignoreTablePeriod_Checked;
            this.ignoreTableVC.Checked += ignoreTableVC_Checked;

        }

        void ignoreTableVC_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && isDisplayChecked) Changed();
        }

        void ignoreTablePeriod_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && isDisplayChecked) Changed();
        }

        void ignoreTableObject_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && isDisplayChecked) Changed();
        }

        void ignoreCellVC_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && isDisplayChecked) Changed();
        }

        void ignoreCellPeriod_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && isDisplayChecked) Changed();
        }

        void ignoreCellObject_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && isDisplayChecked) Changed();
        }

      public  void ignoreAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ignoreCellObject.IsChecked = false;
            ignoreTableObject.IsChecked = false;
            ignoreCellVC.IsChecked = false;
            ignoreTableVC.IsChecked = false;
            ignoreCellPeriod.IsChecked = false;
            ignoreTablePeriod.IsChecked = false;
            if (Changed != null && isDisplayChecked) Changed();
        }

      
      public  void ignoreAll_Checked(object sender, RoutedEventArgs e)
        {
            ignoreCellObject.IsChecked = true;
            ignoreTableObject.IsChecked = true;
            ignoreCellVC.IsChecked = true;
            ignoreTableVC.IsChecked = true;
            ignoreCellPeriod.IsChecked = true;
            ignoreTablePeriod.IsChecked = true;
            if (Changed != null && isDisplayChecked) Changed();
        }
        /// <summary>
        /// set the visibility of the Grid
        /// </summary>
        /// <param name="visibility"></param>
        public void setVisible(bool visibility)
        {
            if (visibility == true)
                this.Visibility = System.Windows.Visibility.Visible;
            else
                this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
