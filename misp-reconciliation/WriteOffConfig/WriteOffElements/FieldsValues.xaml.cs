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

namespace Misp.Reconciliation.WriteOffConfig.WriteOffElements
{
    /// <summary>
    /// Interaction logic for FieldsValues.xaml
    /// </summary>
    public partial class FieldsValues : StackPanel
    {

        public WriteOffField writeOffField { get; set; }

        public FieldsValues()
        {
            InitializeComponent();
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
             string name = writeOffField.attributeField != null ? writeOffField.attributeField.name :
                 writeOffField.periodField != null ? writeOffField.periodField.name :
                 writeOffField.measureField != null ? writeOffField.measureField.name :"" ;
             this.ValueTypeTextBox.Text = name;       
        }
    }
}
