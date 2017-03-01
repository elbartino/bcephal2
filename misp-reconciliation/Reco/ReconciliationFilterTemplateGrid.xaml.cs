using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
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

        public ReconciliationFilterTemplate Template { get; set; }

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        protected bool throwHandler = true;

        public ChangeEventHandler Changed { get; set; }

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
            throwHandler = false;
            if (this.EditedObject != null) this.EditedObject.loadGrilleFilter();
            this.GrilleBrowserForm.EditedObject = this.EditedObject;
            this.GrilleBrowserForm.displayObject();
            this.CreditCheckBox.IsChecked = this.EditedObject != null ? this.EditedObject.creditChecked : false;
            this.DebitCheckBox.IsChecked = this.EditedObject != null ? this.EditedObject.debitChecked : false;
            this.RecoCheckBox.IsChecked = this.EditedObject != null ? this.EditedObject.includeRecoChecked : false;
            if (ApplicationManager.Instance.User != null && !ApplicationManager.Instance.User.IsAdmin())
            {
                this.NameTextBox.Text = this.EditedObject != null ? this.EditedObject.name : "";
                this.CommentTextBlock.Text = this.EditedObject != null ? this.EditedObject.comment : "";
            }
            CustomizeDC();
            Search(this.EditedObject.GrilleFilter != null ? this.EditedObject.GrilleFilter.page : 1);
            throwHandler = true;
        }

        public virtual void Search(int currentPage = 0)
        {            
            try
            {
                if (this.EditedObject.columnListChangeHandler.Items.Count > 0)
                {
                    GrilleFilter filter = this.GrilleBrowserForm.filterForm.Fill();
                    filter.creditChecked = this.CreditCheckBox.IsChecked.Value;
                    filter.debitChecked = this.DebitCheckBox.IsChecked.Value;
                    filter.includeRecoChecked = this.RecoCheckBox.IsChecked.Value;
                    if (this.Template != null && this.Template.reconciliationType != null)
                    {
                        filter.recoType = this.Template.reconciliationType;
                    }
                    else filter.recoType = null;
                    filter.grid = new Grille();
                    filter.grid.creditChecked = this.Template.useDebitCredit != null && this.Template.useDebitCredit.Value && this.CreditCheckBox.IsChecked.Value;
                    filter.grid.debitChecked = this.Template.useDebitCredit != null && this.Template.useDebitCredit.Value && this.DebitCheckBox.IsChecked.Value;
                    filter.grid.includeRecoChecked = this.RecoCheckBox.IsChecked.Value;
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
                else
                {
                    GrillePage rows = new GrillePage();
                    rows.rows = new List<object[]>(0);
                    this.GrilleBrowserForm.displayPage(rows);
                }               
            }
            catch (ServiceExecption)
            {
                GrillePage rows = new GrillePage();
                rows.rows = new List<object[]>(0);
                this.GrilleBrowserForm.displayPage(rows);
            }
        }

        public void CustomizeDC()
        {
            bool visible = this.Template != null && this.Template.useDebitCredit.HasValue && this.Template.useDebitCredit.Value;
            this.CreditCheckBox.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            this.DebitCheckBox.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
        
        #endregion


        #region Handlers

        protected void InitializeHandlers()
        {
            this.GrilleBrowserForm.filterForm.ChangeHandler += OnFilterChange;
            this.GrilleBrowserForm.toolBar.ChangeHandler += OnPageChange;
            this.RecoCheckBox.Checked += OnChecked;
            this.RecoCheckBox.Unchecked += OnChecked;
            if (ApplicationManager.Instance.User != null && ApplicationManager.Instance.User.IsAdmin())
            {
                this.CreditCheckBox.Checked += OnChecked;
                this.CreditCheckBox.Unchecked += OnChecked;
                this.DebitCheckBox.Checked += OnChecked;
                this.DebitCheckBox.Unchecked += OnChecked;
            }
            else
            {
                this.CommentButton.Checked += OnComment;
            }
        }

        private void OnComment(object sender, RoutedEventArgs e)
        {
            this.CommentPopup.IsOpen = true;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (throwHandler && this.EditedObject != null)
            {
                this.EditedObject.creditChecked = this.CreditCheckBox.IsChecked;
                this.EditedObject.debitChecked = this.DebitCheckBox.IsChecked;
                this.EditedObject.includeRecoChecked = this.RecoCheckBox.IsChecked;
                OnChange();
            }
            OnFilterChange();
        }

        private void OnFilterChange()
        {
            if(throwHandler) Search();
            OnChange();
        }

        private void OnPageChange(object item)
        {
            if (throwHandler) Search((int)item);
        }

        public void OnChange()
        {
            if (throwHandler && Changed != null) Changed();
        }


        #endregion


        #region Utils
                
        #endregion
        
    }
}
