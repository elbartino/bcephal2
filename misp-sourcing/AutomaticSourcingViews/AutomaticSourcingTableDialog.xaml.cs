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
            if(textbox.Text == "")
               return false;
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
