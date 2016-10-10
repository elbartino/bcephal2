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
        public delegate void OnCheckMainFunctionality(Functionality name);
        public event OnCheckMainFunctionality CheckMainFunctionality;

        public delegate void OnCheckSubFunctionality(Functionality name);
        public event OnCheckSubFunctionality CheckSubFunctionality;
       
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
            hyperLink.RequestNavigate += OnRequestNavigate;
            this.CheckBox.Content = data.Name;
            this.CheckBox.ToolTip = data.Name;
            this.CheckBox.Tag = data;
            this.CheckBox.Checked += OnSelectFunctionnality;
            
        }

        private void OnSelectFunctionnality(object sender, RoutedEventArgs e)
        {
            Functionality data = this.CheckBox.Tag as Functionality;
            if (CheckMainFunctionality != null) CheckMainFunctionality(data);
            if (CheckSubFunctionality != null) CheckSubFunctionality(data);
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            
        }
    }
}
