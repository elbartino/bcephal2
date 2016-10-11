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

namespace Misp.Kernel.Administration.FunctionnalityViews
{
    /// <summary>
    /// Interaction logic for FunctionnalityField.xaml
    /// </summary>
    public partial class FunctionnalityField : StackPanel
    {
        #region Events

            public event OnSelecteMainFunctionality SelectMainFunctionality;
            public delegate void OnSelecteMainFunctionality(Functionality data, bool isRemove, bool disableSubFunc);

            public event OnSelecteSubMainFunctionality SelectSubMainFunctionality;
            public delegate void OnSelecteSubMainFunctionality(Functionality data, bool isRemove);

        #endregion

        private Domain.Functionality data;
        
        public int oid { get; set; }
        public FunctionnalityGroupField GroupField { get; set; }

        public FunctionnalityField()
        {
            InitializeComponent();
        }


        public FunctionnalityField(Domain.Functionality data) : this()
        {
            this.data = data;
            CustomizeView(data);
        }

        protected void CustomizeView(Domain.Functionality data)
        {
            Run run1 = new Run(data.Name);
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + data.Name),
                //DataContext = token
            };
            //this.TextBlock.Inlines.Add(hyperLink);
            //this.TextBlock.ToolTip = data.name;
           // hyperLink.RequestNavigate += OnRequestNavigate;
            this.CheckBox.Content = data.Name;
            this.CheckBox.ToolTip = data.Name;
            this.CheckBox.Tag = data;
            this.CheckBox.Checked += OnSelectFunctionnality;
            this.CheckBox.Unchecked += OnDeselectFunctionnality;
            
        }

        private void OnSelectFunctionnality(object sender, RoutedEventArgs e)
        {
            Functionality data = this.CheckBox.Tag as Functionality;
            if (SelectMainFunctionality != null)
            {
                SelectMainFunctionality(data, false,false);
                e.Handled = true;
            }
            if (SelectSubMainFunctionality != null)
            {
                SelectSubMainFunctionality(data, false);
                e.Handled = true;
            }
        }

        private void OnDeselectFunctionnality(object sender, RoutedEventArgs e)
        {
            Functionality data = this.CheckBox.Tag as Functionality;
            if (SelectMainFunctionality != null)
            {
                SelectMainFunctionality(data, true,true);
                e.Handled = true;
            }
            if (SelectSubMainFunctionality != null)
            {
                SelectSubMainFunctionality(data, true);
                e.Handled = true;
            }
        }
    }
}
