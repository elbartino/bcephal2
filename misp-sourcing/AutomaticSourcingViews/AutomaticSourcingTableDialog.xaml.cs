using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
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
using System.Windows.Shapes;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for AutomaticSourcingTableDialog.xaml
    /// </summary>
    public partial class AutomaticSourcingTableDialog : Window
    {
        public bool requestGenerateInputTable { get; set; }
        public bool requestRunAllocation { get; set; }
        public String inputTableName { get; set; }
        private string element ="table";
        public bool isGrid {get;set;}
        public bool isTarget { get; set; }

        public AutomaticSourcingService AutomaticSourcingService { get; set; }

        public AutomaticSourcingTableDialog()
        {
            InitializeComponent();
            inpuTableNameTextbox.IsEnabled = true;
            requestGenerateInputTable = false;
            this.ShowInTaskbar = false;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            cancelButton.Click +=cancelButton_Click;
            generateButton.Click +=generateButton_Click;
            runAllocationCheckBox.Checked += OnRequestRunAllocation;
            runAllocationCheckBox.Unchecked +=OnRequestRunAllocation;
            
        }


        public void Customize() 
        {
            if (isGrid) CustomizeForGrid();
            if (isTarget) CustomizeForTarget();
        }

        private void CustomizeForTarget() 
        {
            this.element = "Target";
            this.Title = "Generate Target";
            this.labelName.Content = "Target Name";
            this.runAllocationCheckBox.Visibility = System.Windows.Visibility.Collapsed;

        }
       
        private void CustomizeForGrid() 
        {
            this.element = "Grid";
            this.Title = "Generate Grid";
            this.labelName.Content = "Grid Name";
            this.runAllocationCheckBox.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void OnRequestRunAllocation(object sender, RoutedEventArgs e)
        {
            if (isTextBoxOk(inpuTableNameTextbox))
            {
                requestRunAllocation = runAllocationCheckBox.IsChecked.Value;
            }
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            if (isTextBoxOk(inpuTableNameTextbox))
            {
                requestGenerateInputTable = true;
                inputTableName = inpuTableNameTextbox.Text;
                this.Close();
            }
        }

        private bool isTextBoxOk(TextBox textbox) 
        {
            if(String.IsNullOrWhiteSpace(textbox.Text)) return false;
            String name = textbox.Text.Trim();
            Object table = null;
            if (isGrid)
            {
                table = AutomaticSourcingService.InputGridService.getByName(name);
            }
            else if (isTarget)
            {
                table = AutomaticSourcingService.TargetService.getByName(name);
            }
            else
            {
                table = AutomaticSourcingService.InputTableService.getByName(name);
            }
            if (table != null)
            {
                MessageDisplayer.DisplayWarning("Duplicate name", "Another "+element+" named '" + name + "' already exist");
                return false;
            }
            return true;
        }

        public void SetInputTableName(String excelFileName) 
        {
            inpuTableNameTextbox.Text = excelFileName;
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            requestGenerateInputTable = false;
            this.Close();
        }
       
    }
}
