﻿using System;
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

        public Kernel.Domain.CellProperty CellProperty { get; set; }

        public Kernel.Domain.CellPropertyAllocationData CellAllocationData { get; set; }

        private TransformationTree allocationTree;
        public TransformationTree AllocationTree
        {
            get { return this.allocationTree; }
            set
            {
                this.allocationTree = value;

            }
        }

        public AllocationPropertiesPanel()
        {
            InitializeComponent();
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
            this.AllocationForm.Dispose
            thrawChange = true;
        }

        public Kernel.Domain.CellPropertyAllocationData FillAllocationData() 
        {
            this.CellAllocationData = cel
            return null;
        }

        private void IntializeHandlers()
        {
            this.ForAllocationCheckBox.Checked += OnForAllocationChange;
            this.ForAllocationCheckBox.Unchecked += OnForAllocationChange;
        }

        private void OnForAllocationChange(object sender, RoutedEventArgs e)
        {
            if (ForAllocationChange != null && thrawChange)
            {
                ForAllocationChange();
            }
        }        
    }
}
