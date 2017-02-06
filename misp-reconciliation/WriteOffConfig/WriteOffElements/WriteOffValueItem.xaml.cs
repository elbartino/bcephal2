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
    /// Interaction logic for WriteOffValueItem.xaml
    /// </summary>
    public partial class WriteOffValueItem : Grid
    {
        public Kernel.Domain.WriteOffFieldValue writeOffValueField { get; set; }
        public WriteOffValueItem()
        {
            InitializeComponent();
        }

        public void showRowLabel(bool show = true) 
        {
            this.PossibleValues.showRowLabel(show);
            this.DefaultValues.showRowLabel(show);
        }

        public void display()
        {
            this.PossibleValues.display();
        }
    }
}
