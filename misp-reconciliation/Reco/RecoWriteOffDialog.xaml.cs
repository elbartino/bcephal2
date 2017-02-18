using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Sourcing.GridViews;
using System;
using System.Collections;
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

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for RecoWriteOffDialog.xaml
    /// </summary>
    public partial class RecoWriteOffDialog : Window
    {

        #region Properties

        public ReconciliationFilterTemplate EditedObject { get; set; }

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        #endregion


        #region Constructors

        public RecoWriteOffDialog()
        {
            InitializeComponent();
            ReconciliationGrid.RecoToolBar.Visibility = Visibility.Collapsed;
            this.confirmationMessageLabel.Content = "You are about to create a reconciliation for the selected items.\nDo you confirm the operation?";
            InitHandlers();
        }

        #endregion


        #region Operations

        public void displayObject(IList items)
        {
            this.ReconciliationGrid.GridBrowser.RebuildGrid = true;
            this.ReconciliationGrid.EditedObject = this.EditedObject != null ? this.EditedObject.bottomGrid : null;
            this.ReconciliationGrid.displayObject();
            ((GridTableView)this.ReconciliationGrid.GridBrowser.gridControl.View).ShowCheckBoxSelectorColumn = false;

            this.ReconciliationGrid.GridBrowser.gridControl.ItemsSource = items;
            this.ReconciliationGrid.GridBrowser.gridControl.SelectAll();
        }

        public void displayConfig(WriteOffConfiguration writeOffConfiguration)
        {
            this.WriteOffBlock.WriteOffConfiguration = writeOffConfiguration;
            this.WriteOffBlock.display();
        }

        #endregion


        #region Handlers

        private void InitHandlers()
        {
            //this.ReconciliationGrid.GridBrowser.ChangeHandler += OnGridSelectionChange;
            
        }

        #endregion


        
    }
}
