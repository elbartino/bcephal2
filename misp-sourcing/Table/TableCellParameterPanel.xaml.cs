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


namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for TableCellParameterPanel.xaml
    /// </summary>
    public partial class TableCellParameterPanel : ScrollViewer
    {

        public Kernel.Ui.Base.ChangeEventHandler ForAllocationChange;
        public Kernel.Ui.Base.ChangeEventHandler NoAllocationChange;
        public bool thrawChange = true;

        public RPeriodPanel reportPeriodPanel;

        public TableCellParameterPanel()
        {
            InitializeComponent();            
            TableCellMappingPanel = new TableCellMappingPanel();
            this.filterScopePanel.DisplayScope(null);
            IntializeHandlers();
            Expand(true);
        }

        private void IntializeHandlers()
        {
            //this.ForAllocationCheckBox.Checked += OnForAllocationChange;
            //this.ForAllocationCheckBox.Unchecked += OnForAllocationChange;
        }
                
        public TableCellMappingPanel TableCellMappingPanel { get; private set; }

        public Kernel.Domain.CellProperty CellProperty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            return controls;
        }


        public void Display(Kernel.Domain.CellProperty cellProperty,bool readOnly = false)
        {
            thrawChange = false;
            bool isNoAllocation = cellProperty.cellAllocationData != null && 
            cellProperty.cellAllocationData.type == Kernel.Domain.CellPropertyAllocationData.AllocationType.NoAllocation.ToString(); 

            this.CellProperty = cellProperty;

            this.CellTextBox.Text = cellProperty != null ? cellProperty.name : "";
          //  this.ForAllocationCheckBox.IsChecked = cellProperty != null ? cellProperty.IsForAllocation : false;
            this.CellMeasurePanel.Display(cellProperty != null ? cellProperty.cellMeasure : null,readOnly);

            if (reportPeriodPanel != null) this.reportPeriodPanel.DisplayPeriod(cellProperty != null ? cellProperty.period : null,false,readOnly);
            else this.periodPanel.DisplayPeriod(cellProperty != null ? cellProperty.period : null,false,readOnly);
            this.filterScopePanel.DisplayScope((cellProperty != null ? cellProperty.cellScope : null),isNoAllocation,readOnly);
           // this.allocationPanel.DisplayAllocationData(cellProperty != null ? cellProperty.cellAllocationData : null);
            thrawChange = true;
        }


        private void OnForAllocationChange(object sender, RoutedEventArgs e)
        {
            if (ForAllocationChange != null && thrawChange)
            {
                ForAllocationChange();
            }
        }


        public void CustomizeForReport()
        {
            //ForAllocationCheckBox.Content = "For run";
            //AllocationGroupBox.Visibility = System.Windows.Visibility.Collapsed;
            if (reportPeriodPanel == null) reportPeriodPanel = new RPeriodPanel();
            periodGroupBox.Content = reportPeriodPanel;
        }

        public void Expand(bool expand)
        {
            CellMeasurePanel.Expand(expand);
            filterScopePanel.Expand(expand);
        }


        public  void SetReadOnly(bool readOnly)
        {
            this.ResetButton.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}
