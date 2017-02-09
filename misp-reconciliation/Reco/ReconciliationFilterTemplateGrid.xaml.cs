using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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

        #region Properties

        /// <summary>
        /// Design en édition
        /// </summary>
        public Grille EditedObject { get; set; }

        public ReconciliationFilterService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterService(); } }

        #endregion


        #region Constructors

        public ReconciliationFilterTemplateGrid()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        #endregion


        #region Operations

        public void displayObject()
        {
            this.GrilleBrowserForm.EditedObject = this.EditedObject;
            this.GrilleBrowserForm.displayObject();
        }

        public virtual void Search(int currentPage = 0)
        {
            try
            {
                GrilleFilter filter = this.GrilleBrowserForm.filterForm.Fill();
                filter.grid = new Grille();
                filter.grid.code = this.EditedObject.code;
                filter.grid.columnListChangeHandler = this.EditedObject.columnListChangeHandler;
                filter.grid.report = this.EditedObject.report;
                filter.grid.reconciliation = this.EditedObject.reconciliation;
                filter.grid.oid = this.EditedObject.oid;
                filter.grid.name = this.EditedObject.name;
                filter.page = currentPage;
                filter.pageSize = (int)this.GrilleBrowserForm.toolBar.pageSizeComboBox.SelectedItem;
                filter.showAll = this.GrilleBrowserForm.toolBar.showAllCheckBox.IsChecked.Value;
                GrillePage rows = this.Service.getGridRows(filter);
                this.GrilleBrowserForm.displayPage(rows);                
            }
            catch (ServiceExecption)
            {
                GrillePage rows = new GrillePage();
                rows.rows = new List<object[]>(0);
                this.GrilleBrowserForm.displayPage(rows);
            }
        }

        #endregion


        #region Handlers

        protected void InitializeHandlers()
        {
            this.GrilleBrowserForm.filterForm.ChangeHandler += OnFilterChange;
            this.GrilleBrowserForm.toolBar.ChangeHandler += OnPageChange;

        }

        private void OnFilterChange()
        {
            Search();
        }

        private void OnPageChange(object item)
        {
            Search((int)item);
        }

        #endregion


        #region Utils

        public void HideHeaderPanel()
        {
            this.HeaderPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
        
        #endregion
        
    }
}
