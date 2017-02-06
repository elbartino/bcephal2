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
    /// Interaction logic for PossibleValues.xaml
    /// </summary>
    public partial class PossibleValues : Grid
    {
        public Kernel.Domain.WriteOffFieldValue writeOffValueField { get; set; }
        public PossibleValues()
        {
            InitializeComponent();
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
            string name = writeOffValueField.attribute != null ? writeOffValueField.attribute.name : 
                writeOffValueField.attributeValue != null ? writeOffValueField.attributeValue.name :
                writeOffValueField.period != null ? writeOffValueField.period.name : "";
            this.ValueTypeTextBox.Text = name;
        }
    }
}
