using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CalculatedMeasureOperationsGrid.xaml
    /// </summary>
    public partial class CalculatedMeasureOperationsGrid : UserControl
    {
       
        //event
        public event ChangeEventHandler Changed;

        public event DeleteEventHandler PanelItemDeleted;
        
        public CalculatedMeasureOperationsGrid()
        {
            InitializeComponent();
            InitializeHandler();
            
        }

        private void InitializeHandler()
        {
            this.OperationsPanel.Changed += OperationPanelChanged;
            this.OperationsPanel.ItemDeleted += OperationPanelItemDeleted;
        }

        private void OperationPanelItemDeleted(object item)
        {
            PanelItemDeleted(item);

        }

        private void OperationPanelChanged()
        {
            Changed();
        }

  
       
        public void DisplayCalculatedMeasure(Kernel.Domain.CalculatedMeasure calculatedMeasure)
        {
            this.OperationsPanel.DisplayCalculatedMeasure(calculatedMeasure);
        }

        public bool SetCalculatedMeasureItemValue(Kernel.Domain.CalculatedMeasureItem item)
        {
            return this.OperationsPanel.SetCalculatedMeasureItemValue(item);
        }

        
    }
}
