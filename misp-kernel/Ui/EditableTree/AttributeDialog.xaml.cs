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

namespace Misp.Kernel.Ui.EditableTree
{
    /// <summary>
    /// Interaction logic for AttributeDialog.xaml
    /// </summary>
    public partial class AttributeDialog : Window
    {

        Domain.Attribute attribute;
        public AttributeDialog()
        {
            InitializeComponent();
            this.CancelButton.Click += OnCancel;
        }


        public void Display(Domain.Attribute attribute)
        {
            this.attribute = attribute;
            this.NameTextBox.Text = attribute.name;
        }


        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.CancelButton.Click -= OnCancel;
            this.Close();
        }

    }
}
