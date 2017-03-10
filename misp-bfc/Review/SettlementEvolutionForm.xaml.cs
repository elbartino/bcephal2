using Misp.Bfc.Model;
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

namespace Misp.Bfc.Review
{
    /// <summary>
    /// Interaction logic for SettlementEvolutionForm.xaml
    /// </summary>
    public partial class SettlementEvolutionForm : ScrollViewer
    {

        #region Properties

        public ChangeEventHandler SchemeChanged { get; set; }

        public BfcItem Scheme { get; private set; }

        bool throwHandlers;

        #endregion


        #region Constructors

        public SettlementEvolutionForm()
        {
            InitializeComponent();
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(List<SettlementEvolutionData> datas)
        {
            throwHandlers = false;
            this.Grid.ItemsSource = datas;
            
            throwHandlers = true;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.SchemeComboBox.SelectionChanged += OnselectScheme;
        }

        private void OnselectScheme(object sender, SelectionChangedEventArgs e)
        {
            Object obj = this.SchemeComboBox.SelectedItem;
            if (obj != null && obj is BfcItem)
            {
                BfcItem item = (BfcItem)obj;
                SchemeTextBox.Text = item.id;
                this.Scheme = item;
            }
            else
            {
                this.Scheme = null;
                SchemeTextBox.Text = "";
            }
            if (throwHandlers && SchemeChanged != null) SchemeChanged();
        }

        #endregion

    }
}
