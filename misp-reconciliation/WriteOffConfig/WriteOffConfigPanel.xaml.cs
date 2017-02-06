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
    /// Interaction logic for WriteOffConfigMainPanel.xaml
    /// </summary>
    public partial class WriteOffConfigPanel : Grid
    {
        public int nbreLigne = 0;
        public WriteOffConfigPanel()
        {
            InitializeComponent();
            this.configPanel.Children.Clear();
            
        }

        public void display(Kernel.Domain.ReconciliationFilterTemplate reconciliationFilterTemplate)
        {
            WriteOffFieldPanel wpanel = new WriteOffFieldPanel();
            wpanel.parent = this;
            wpanel.display();
            this.configPanel.Children.Add(wpanel);
            nbreLigne++;
            WriteOffFieldPanel wpanel1 = new WriteOffFieldPanel();
            wpanel1.parent = this;
            wpanel1.showRowLabel(false);
            wpanel1.display();
            this.configPanel.Children.Add(wpanel1);
            nbreLigne++;
        }

        public List<object> getEditableControls()
        {
            List<object> list = new List<object>();
            //list.Add(postingAttributePanel);
            //list.Add(reconciliationAttributePanel);
            //list.Add(accountNbrAttributePanel);
            //list.Add(accountNameAttributePanel);
            //list.Add(dcAttributePanel);
            ////list.Add(debitValuePanel);
            ////list.Add(creditValuePanel);
            //list.Add(writeOffAccountPanel);
            //list.Add(amountMeasurePanel);
            return list;
        }
    }
}
