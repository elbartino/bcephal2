using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Util;
using Misp.Reconciliation.Posting;
using Misp.Reporting.Base;
using Misp.Reporting.Report;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Reconciliation.Reconciliation
{
    /// <summary>
    /// Interaction logic for ReconciliationMainPanel.xaml
    /// </summary>
    public partial class ReconciliationMainPanel : ScrollViewer
    {

        #region Events
        
        #endregion


        #region Properties
        public PostingBrowserForm activeFilterGrid;
        #endregion


        #region Constructor

        public ReconciliationMainPanel()
        {
            InitializeComponent();
            postingBrowserForm.HideFilter();
            postingBrowserForm.HideToolBarDeleteButton();
            leftFilterGrid.HideToolBarButtons();
            rigthFilterGrid.HideToolBarButtons();
            activeFilterGrid = leftFilterGrid;
            IntializeHandlers();
        }


        public void setPostingService(PostingService service)
        {
            leftFilterGrid.PostingService = service;
            rigthFilterGrid.PostingService = service;
            postingBrowserForm.PostingService = service;

            leftFilterGrid.Search();
            rigthFilterGrid.Search();
        }

        public void Display(ReconciliationTemplate template)
        {
            if (postingBrowserForm.PostingService != null && template != null)
            {
                setDefaultPeriod();
                leftFilterGrid.Display(template.leftPostingFilter);
                rigthFilterGrid.Display(template.rigthPostingFilter);
            }
        }

        public void setDefaultPeriod()
        {
            PeriodName rootPeriodName = postingBrowserForm.PostingService.periodNameService.getRootPeriodName();
            PeriodName defaultPeriodName = rootPeriodName.getDefaultPeriodName();
            rigthFilterGrid.filterForm.filterPTForm.periodFilter.DefaultPeriodName = defaultPeriodName;
            leftFilterGrid.filterForm.filterPTForm.periodFilter.DefaultPeriodName = defaultPeriodName;
        }

        public ReconciliationTemplate Fill(ReconciliationTemplate template)
        {
            if (template == null) template = new ReconciliationTemplate();
            template.leftPostingFilter = leftFilterGrid.Fill();
            template.rigthPostingFilter = rigthFilterGrid.Fill();
            return template;
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(this.leftFilterGrid.getEditableControls());
            controls.AddRange(this.rigthFilterGrid.getEditableControls());
            return controls;
        }

        #endregion


        #region Handlers

        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {
            leftFilterGrid.Activated += FilterPanel_Activated;
            rigthFilterGrid.Activated += FilterPanel_Activated;

            leftFilterGrid.grid.ChangeHandler += OnGridSelectionchange;
            rigthFilterGrid.grid.ChangeHandler += OnGridSelectionchange;
            postingBrowserForm.ReconciliationEndedHandler += OnReconciliationEnded;
        }

        private void OnReconciliationEnded()
        {
            leftFilterGrid.Search();
            rigthFilterGrid.Search();
        }
        
        private void OnGridSelectionchange()
        {
            List<PostingBrowserData> items = new List<PostingBrowserData>(0);
            List<long> ids = new List<long>(0);
            foreach (object item in leftFilterGrid.grid.SelectedItems)
            {
                if (item is PostingBrowserData)
                {
                    items.Add((PostingBrowserData)item);
                    ids.Add(((PostingBrowserData)item).id);
                }
            }
            foreach (object item in rigthFilterGrid.grid.SelectedItems)
            {
                if (item is PostingBrowserData && !ids.Contains(((PostingBrowserData)item).id)) items.Add((PostingBrowserData)item);
            }
            postingBrowserForm.grid.ItemsSource = items;
            postingBrowserForm.grid.SelectAll();
        }

        #endregion

        #region Actions

        private void FilterPanel_Activated(object item)
        {
            if (item == null || !(item is PostingBrowserForm)) return;
            activeFilterGrid = (PostingBrowserForm)item;
        }

        #endregion
    }
}
