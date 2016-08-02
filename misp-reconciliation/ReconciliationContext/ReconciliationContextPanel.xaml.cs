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

namespace Misp.Reconciliation.ReconciliationContext
{
    /// <summary>
    /// Interaction logic for ReconciliationContextPanel.xaml
    /// </summary>
    public partial class ReconciliationContextPanel : Grid
    {


        public Kernel.Domain.ReconciliationContext reconciliationContext { get; set; }

        public ReconciliationContextPanel()
        {
            InitializeComponent();
        }

        public void display(Kernel.Domain.ReconciliationContext reconciliationContext) 
        {
            postingTextbox.Text = reconciliationContext.postingNbreAttribute.name;
            accoutTextbox.Text = reconciliationContext.accountNbreAttribute.name;
            reconcilTextbox.Text = reconciliationContext.recoNbreAttribute.name;
            dcTextbox.Text = reconciliationContext.dcNbreAttribute.name;
            lastPosTextbox.Text = "" + reconciliationContext.lastPostingNumber;
            lastRecoTextbox.Text = "" + reconciliationContext.lastRecoNumber;
        }

        public Kernel.Domain.ReconciliationContext Fill() 
        {
            return reconciliationContext;
        }

    }
}
