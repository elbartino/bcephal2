using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Sourcing.GridViews;
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
            InitHandlers();
        }

        #endregion


        #region Operations

        public void displayObject(List<GridItem> items)
        {
            this.ReconciliationGrid.GridBrowser.RebuildGrid = true;
            this.ReconciliationGrid.EditedObject = this.EditedObject != null ? this.EditedObject.bottomGrid : null;
            this.ReconciliationGrid.displayObject();

            this.ReconciliationGrid.GridBrowser.gridControl.ItemsSource = items;
            this.ReconciliationGrid.GridBrowser.gridControl.SelectAll();
        }

        #endregion


        #region Handlers

        private void InitHandlers()
        {
            //this.ReconciliationGrid.GridBrowser.ChangeHandler += OnGridSelectionChange;
            this.ReconciliateButton.Click += OnReconciliate;
            this.CancelButton.Click += OnCancel;
        }

        private void OnReconciliate(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #endregion

    }
}
