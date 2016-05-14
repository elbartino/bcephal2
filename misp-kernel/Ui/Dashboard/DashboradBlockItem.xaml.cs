using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
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
    /// Interaction logic for DashboradBlockItem.xaml
    /// </summary>
    public partial class DashboradBlockItem : StackPanel
    {

        public int oid { get; set; }
        public DashboardBlock Block { get; set; }

        public DashboradBlockItem()
        {
            InitializeComponent();
        }

        public DashboradBlockItem(BrowserData data, String fuctionalityCode) : this()
        {
            CustomizeView(data, fuctionalityCode);
        }

        protected void CustomizeView(BrowserData data, String fuctionalityCode)
        {
            this.oid = data.oid;
            NavigationToken token = NavigationToken.GetModifyViewToken(fuctionalityCode, data.oid);

            Run run1 = new Run(data.name);
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + data.name),
                DataContext = token
            };
            this.TextBlock.Inlines.Add(hyperLink);
            this.TextBlock.ToolTip = data.name;
            hyperLink.RequestNavigate += OnRequestNavigate;

            this.CheckBox.ToolTip = data.name;
            this.CheckBox.Tag = token;

            this.PreviewMouseRightButtonDown += BeforeContextMenu;
        }

        private void BeforeContextMenu(object sender, MouseButtonEventArgs e)
        {
            if (!this.CheckBox.IsChecked.Value)
            {
                this.Block.DeselectAll();
                this.CheckBox.IsChecked = true;
            }
        }
        
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (sender is Hyperlink)
            {
                Hyperlink link = (Hyperlink)sender;
                object context = link.DataContext;
                if (context is NavigationToken)
                {
                    NavigationToken token = (NavigationToken)context;
                    HistoryHandler.Instance.openPage(token);
                }
            }
        }

    }
}
