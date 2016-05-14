using Misp.Kernel.Domain;
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

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardConfiguration.xaml
    /// </summary>
    public partial class DashboardConfigurationForm : Grid
    {
        public DashBoardConfiguration Configuration { get; set; }

        public DashboardConfigurationForm()
        {
            InitializeComponent();
            this.OrderByComboBox.ItemsSource = new String[] { DashBoardConfiguration.MODIFICATION_DATE, DashBoardConfiguration.NAME };
            this.OrderByDirectionComboBox.ItemsSource = new String[] { DashBoardConfiguration.ASC, DashBoardConfiguration.DESC };
        }

        public void Display(DashBoardConfiguration confg)
        {
            this.Configuration = confg;
            this.BloackTextBox.Text = this.Configuration.name;
            this.MaxTextBox.Text = this.Configuration.maxItems.ToString();
            this.OrderByComboBox.SelectedItem = this.Configuration.orderBy;
            this.OrderByDirectionComboBox.SelectedItem = this.Configuration.orderByDirection;
        }

        public DashBoardConfiguration Fill()
        {
            if (this.Configuration == null) this.Configuration = new DashBoardConfiguration(this.BloackTextBox.Text);
            this.Configuration.orderBy = this.OrderByComboBox.SelectedItem.ToString();
            this.Configuration.orderByDirection = this.OrderByDirectionComboBox.SelectedItem.ToString();
            int max = DashBoardConfiguration.MAX_ITEMS;
            if (int.TryParse(this.MaxTextBox.Text.Trim(), out max)) this.Configuration.maxItems = max;
            else this.Configuration.maxItems = DashBoardConfiguration.MAX_ITEMS;
            return this.Configuration;
        }

    }
}
