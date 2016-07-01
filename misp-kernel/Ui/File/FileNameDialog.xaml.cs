using Misp.Kernel.Service;
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
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.File
{
    /// <summary>
    /// Interaction logic for FileNameDialog.xaml
    /// </summary>
    public partial class FileNameDialog : Window
    {
        public delegate bool ValidateNameEventHandler(String name);
        public ValidateNameEventHandler ValidateNameAction { get; set; }
        
        public String Name { get; set; }

        public FileNameDialog()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            //this.KeyUp += OnEnter;
            NameTextBox.KeyUp += OnEnter;
            OkButton.Click += OnOkClick;
            CancelButton.Click += OnCancelClick;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Name = null;
            this.Close();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Name = NameTextBox.Text.Trim();
            if (ValidateNameAction != null && !ValidateNameAction(Name))
            {
                Name = null;
                NameTextBox.SelectAll();
                NameTextBox.Focus();
            }
            else this.Close();
        }

        private void OnEnter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) OnOkClick(null, null);
        }
    }
}
