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

namespace Misp.Sourcing.AllocationViews
{
    /// <summary>
    /// Interaction logic for AllocationViewsPanel.xaml
    /// </summary>
    public partial class AllocationPropertiesPanel : Grid
    {
        public Kernel.Ui.Base.ChangeEventHandler ForAllocationChange;
        public Kernel.Ui.Base.ChangeEventHandler NoAllocationChange;
        public bool thrawChange = true;

        public Kernel.Ui.Base.ChangeEventHandler Change;

        public Kernel.Domain.CellProperty CellProperty { get; set; }

        public Kernel.Domain.CellPropertyAllocationData CellAllocationData { get; set; }
      

        public AllocationPropertiesPanel()
        {
            InitializeComponent();
            this.AllocationForm.Visibility = System.Windows.Visibility.Collapsed;
            IntializeHandlers();
        }


        public void Display(Kernel.Domain.CellProperty cellProperty)
        {
            thrawChange = false;
            bool isNoAllocation = cellProperty.cellAllocationData != null &&
            cellProperty.cellAllocationData.type == Kernel.Domain.CellPropertyAllocationData.AllocationType.NoAllocation.ToString();

            this.CellProperty = cellProperty;

            this.CellTextBox.Text = cellProperty != null ? cellProperty.name : "";
            this.ForAllocationCheckBox.IsChecked = cellProperty != null ? cellProperty.IsForAllocation : false;
            this.CellAllocationData = cellProperty != null ? cellProperty.cellAllocationData : null;
            this.AllocationPanel.DisplayAllocationData(this.CellAllocationData);
            this.AllocationForm.EditedObject = this.CellAllocationData != null ? this.CellAllocationData.allocationTree : null;
            this.AllocationForm.displayObject();
            thrawChange = true;
        }

        public Kernel.Domain.CellPropertyAllocationData FillAllocationData() 
        {
            this.AllocationPanel.FillAllocationData();
            this.CellAllocationData = this.AllocationPanel.AllocationData;
            this.CellAllocationData.active = this.ActivateAllocationCheckBox.IsChecked.Value;
            this.AllocationForm.fillObject();
            this.CellAllocationData.allocationTree = this.AllocationForm.EditedObject;
            return CellAllocationData;
        }

        private void IntializeHandlers()
        {
            this.AllocationPanel.AllocationTypeChanged += OnAllocationTypeChange;
            this.ForAllocationCheckBox.Checked += OnForAllocationChange;
            this.ForAllocationCheckBox.Unchecked += OnForAllocationChange;
            this.ActivateAllocationCheckBox.Checked += OnActivateAllocationChange;
            this.ActivateAllocationCheckBox.Unchecked += OnActivateAllocationChange;
            this.AllocationPanel.Change += OnChange;
            this.AllocationForm.Change += OnChange;
        }

        private void OnAllocationTypeChange(object item)
        {
            if (item is string) 
            {
                bool visible = false;
                if (item.ToString() == CellPropertyAllocationData.AllocationType.Linear.ToString()){ 
                    visible = true;
                    this.AllocationPanel.RefMeasureGrid.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (item.ToString() == CellPropertyAllocationData.AllocationType.Reference.ToString()) visible = true;
                else if (item.ToString() == CellPropertyAllocationData.AllocationType.NoAllocation.ToString()) visible = false;
                
                this.AllocationForm.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        private void OnActivateAllocationChange(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            //controls.Add(this.ForAllocationCheckBox);
            controls.AddRange(this.AllocationPanel.getEditableControls());
            controls.AddRange(this.AllocationForm.getEditableControls());
            return controls;
        }
               

        private void OnForAllocationChange(object sender, RoutedEventArgs e)
        {
            if (ForAllocationChange != null && thrawChange)
            {
                ForAllocationChange();
            }
        }


        private void OnChange()
        {
            if (Change != null && thrawChange)
            {
                FillAllocationData();
                Change();
            }
        }
    }
}
