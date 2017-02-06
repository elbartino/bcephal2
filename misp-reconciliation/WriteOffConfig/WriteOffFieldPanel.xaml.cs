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

namespace Misp.Reconciliation.WriteOffConfig
{
    /// <summary>
    /// Interaction logic for WriteOffFieldPanel.xaml
    /// </summary>
    public partial class WriteOffFieldPanel : Grid
    {
        public Kernel.Domain.WriteOffConfiguration writeOffConfig { get; set; }

        public WriteOffConfigPanel parent { get; set; }

        public WriteOffFieldPanel()
        {
            InitializeComponent();

            //this.FieldValuePanel.Children.Clear();
        }
        bool showLabel;
        public void showRowLabel(bool show = true)
        {
            showLabel = show;
            this.fieldsValue.showRowLabel(show);
            this.MandatoryValue.showRowLabel(show);
            this.FieldValuePanel.showRowLabel(show);
        }

        public void display()
        {
            this.FieldValuePanel.showLabel = showLabel;
            //this.FieldValuePanel.fieldListChangeHandler = writeOffConfig.fieldListChangeHandler;
            if (parent.nbreLigne == 0)
                this.FieldValuePanel.showLabel = true;
            else this.FieldValuePanel.showLabel = false;
            this.FieldValuePanel.display();
        }
    }
}
