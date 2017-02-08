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

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for ReconciliationFilterTemplateGrid.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateGrid : Grid
    {
        /// <summary>
        /// Design en édition
        /// </summary>
        public Grille EditedObject { get; set; }

        public ReconciliationFilterTemplateGrid()
        {
            InitializeComponent();
        }

        public void HideHeaderPanel()
        {
            this.HeaderPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void HideRecoToolBar()
        {
            this.RecoToolBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void displayObject()
        {
            this.GrilleBrowserForm.EditedObject = this.EditedObject;
            this.GrilleBrowserForm.displayObject();
        }

    }
}
