using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
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
using System.Windows.Threading;

namespace Misp.Kernel.Ui.Sidebar.Tree
{
    /// <summary>
    /// Interaction logic for PeriodTree.xaml
    /// </summary>
    public partial class PeriodTree : UserControl
    {
        
              
        #region Events

        /// <summary>
        /// Event to handle when single click on item.
        /// </summary>
        public event SelectedItemChangedEventHandler Click;

        /// <summary>
        /// Event to handle when single double click on item.
        /// </summary>
        public event SelectedItemChangedEventHandler DoubleClick;

        /// <summary>
        /// Event to handle when single double click on item.
        /// </summary>
        public event SelectedItemChangedEventHandler Expanded;

        #endregion


        #region Properties

        public PeriodNameService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetPeriodNameService(); } }

        protected DispatcherTimer Timer { get; set; }

        protected static String SHOW_MORE_LABEL = "Show more...";

        protected static int PAGE_SIZE = 2;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PeriodTree() 
        {
            InitializeComponent();
            UserInitializations();
        }

        #endregion


        #region Operations

        /// <summary>
        /// Initialize data
        /// </summary>
        public virtual void InitializeData()
        {
            Display(this.Service.getRootPeriodNameForSidebar());
        }

        /// <summary>
        /// Display models
        /// </summary>
        /// <param name="models"> Models to display </param>
        public void Display(Domain.PeriodName root)
        {
            if (root == null) this.treeView.ItemsSource = null;
            else {
                foreach (Domain.PeriodName period in root.childrenListChangeHandler.Items)
                {
                    period.parent = root;
                    RefreshParent(period);
                }
                this.treeView.ItemsSource = root.childrenListChangeHandler.Items;
            }            
        }

        /// <summary>
        /// Load no complete object.
        /// </summary>
        /// <param name="parent"></param>
        protected virtual void Load(Persistent parent)
        {
            if (parent == null || parent.IsDefault) return;
            if (!parent.isCompleted && parent.oid.HasValue)
            {
                if (parent.DataFilter == null)
                {
                    parent.DataFilter = new BrowserDataFilter();
                    parent.DataFilter.groupOid = parent.oid.Value;
                    parent.DataFilter.page = 0;
                    parent.DataFilter.pageSize = PAGE_SIZE;
                }

                if (parent is PeriodName)
                {
                    PeriodName period = (PeriodName)parent;
                    ForgetDefaultItems(period);
                    period.DataFilter.page++;
                    BrowserDataPage<PeriodInterval> page = this.Service.getRootIntervalsByPeriodName(period.DataFilter);
                    period.intervalListChangeHandler.Items.Clear();
                    foreach (PeriodInterval interval in page.rows)
                    {
                        interval.periodName = period;
                        period.intervalListChangeHandler.Items.Add(interval);
                    }
                    period.isCompleted = true;
                    period.DataFilter.page = page.currentPage;
                    period.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(period);
                }
                else if (parent is PeriodInterval)
                {
                    PeriodInterval interval = (PeriodInterval)parent;
                    ForgetDefaultItems(interval);
                    interval.DataFilter.page++;
                    BrowserDataPage<PeriodInterval> page = this.Service.getPeriodIntervalChildren(interval.DataFilter);
                    interval.childrenListChangeHandler.Items.Clear();
                    foreach (PeriodInterval child in page.rows)
                    {
                        child.parent = interval;
                        child.periodName = interval.periodName;
                        interval.childrenListChangeHandler.Items.Add(child);
                    }
                    interval.isCompleted = true;
                    interval.DataFilter.page = page.currentPage;
                    interval.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(interval);
                }
                parent.isCompleted = true;
            }

        }

        /// <summary>
        /// Perform default action
        /// </summary>
        /// <param name="action"></param>
        protected virtual void PerformDefaultAction(Persistent action)
        {
            if (action == null || !action.IsShowMoreItem) return;
            if (action is PeriodInterval)
            {
                PeriodInterval selection = (PeriodInterval)action;
                PeriodInterval parent = selection.parent;
                PeriodName period = selection.periodName;

                BrowserDataPage<PeriodInterval> page = null;
                if (parent != null)
                {
                    ForgetDefaultItems(parent);
                    parent.DataFilter.page++;
                    page = this.Service.getPeriodIntervalChildren(parent.DataFilter);
                    foreach (PeriodInterval interval in page.rows)
                    {
                        interval.parent = parent;
                        interval.periodName = parent.periodName;
                        parent.childrenListChangeHandler.Items.Add(interval);
                    }
                    parent.isCompleted = true;
                    parent.DataFilter.page = page.currentPage;
                    parent.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(parent);
                }
                else if (period != null)
                {
                    ForgetDefaultItems(period);
                    period.DataFilter.page++;
                    page = this.Service.getRootIntervalsByPeriodName(period.DataFilter);
                    foreach (PeriodInterval interval in page.rows)
                    {
                        interval.periodName = period;
                        period.intervalListChangeHandler.Items.Add(interval);
                    }
                    period.isCompleted = true;
                    period.DataFilter.page = page.currentPage;
                    period.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(period);
                }
            }
        }

        /// <summary>
        /// Add default nodes
        /// </summary>
        /// <param name="parent"></param>
        protected virtual void AddDefaultItems(Persistent parent)
        {
            if (parent == null || parent.IsDefault) return;
            if (parent.isCompleted && parent.HasMoreElements())
            {
                PeriodInterval showMore = new PeriodInterval();
                showMore.IsShowMoreItem = true;
                showMore.name = SHOW_MORE_LABEL;

                if (parent is PeriodName)
                {
                    PeriodName period = (PeriodName)parent;
                    showMore.periodName = period;
                    period.intervalListChangeHandler.Items.Add(showMore);
                }
                else if (parent is PeriodInterval)
                {
                    PeriodInterval interval = (PeriodInterval)parent;
                    showMore.parent = interval;
                    interval.childrenListChangeHandler.Items.Add(showMore);
                }
            }
        }

        /// <summary>
        /// Remove default nodes from root
        /// </summary>
        protected virtual void ForgetDefaultItems(Persistent parent)
        {
            if (parent != null)
            {
                if (parent is PeriodName)
                {
                    PeriodName period = (PeriodName)parent;
                    foreach (PeriodInterval interval in period.intervalListChangeHandler.Items.ToArray())
                    {
                        if (interval.IsDefault) period.intervalListChangeHandler.Items.Remove(interval);
                    }
                }
                else if (parent is PeriodInterval)
                {
                    PeriodInterval interval = (PeriodInterval)parent;
                    foreach (PeriodInterval child in interval.childrenListChangeHandler.Items.ToArray())
                    {
                        if (child.IsDefault) interval.childrenListChangeHandler.Items.Remove(child);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        public virtual void RefreshParent(Persistent parent)
        {
            if (parent is PeriodName)
            {
                PeriodName period = (PeriodName)parent;
                foreach (PeriodInterval interval in period.intervalListChangeHandler.Items)
                {
                    interval.periodName = period;
                    RefreshParent(interval);
                }
            }
            else if (parent is PeriodInterval)
            {
                PeriodInterval interval = (PeriodInterval)parent;
                foreach (PeriodInterval child in interval.childrenListChangeHandler.Items)
                {
                    child.parent = interval;
                    child.periodName = interval.periodName;
                    RefreshParent(child);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected item</returns>
        public virtual Domain.Persistent GetSelectedItem()
        {
            return this.treeView.SelectedItem != null ? this.treeView.SelectedItem as Domain.Persistent : null;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="item">The item to select</param>
        public virtual void SetSelectedItem(Domain.Persistent item)
        {
            if (item != null)
            {
                item.IsSelected = true;
            }
            else
            {
                Domain.Persistent selection = GetSelectedItem();
                if (selection != null) selection.IsSelected = false;
            }
        }


        #endregion


        #region Initializations
        
        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected virtual void UserInitializations() 
        {
            this.Timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 350), DispatcherPriority.Background, OnTimeTick, Dispatcher.CurrentDispatcher);
            this.Timer.Stop();
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize handlers
        /// </summary>
        protected virtual void InitializeHandlers() { }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Timer.Stop();
            Persistent value = GetSelectedItem();
            if (value != null && sender != null && sender is TreeViewItem)
            {
                TreeViewItem item = (TreeViewItem)sender;
                if (!item.IsSelected) { e.Handled = true; return; }
                else if (value.IsDefault) PerformDefaultAction(value);
                else if (DoubleClick != null) DoubleClick(value);
            }
            e.Handled = true;
        }

        private void OnClick(object sender, MouseEventArgs e)
        {
            this.Timer.Start();
        }

        private void OnTimeTick(object sender, EventArgs e)
        {
            this.Timer.Stop();
            //Handle single click
            Domain.Persistent value = GetSelectedItem();
            if (value != null)
            {
                if (value.IsDefault) PerformDefaultAction(value);
                else if (Click != null) Click(value);
            }
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            this.Timer.Stop();
            object obj = ((TreeViewItem)e.OriginalSource).Header;
            if (obj != null && obj is Persistent)
            {
                Persistent value = (Persistent)obj;
                if (value != null && !value.IsDefault)
                {
                    if (!value.isCompleted) Load(value);
                    if (Expanded != null) Expanded(value);
                }
            }
        }

        private void OnCollapsed(object sender, RoutedEventArgs e)
        {
            this.Timer.Stop();
        }
        
        #endregion


        #region Utils

        // searches for the corresponding TreeViewItem
        static TreeViewItem FindTreeItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);
            return source as TreeViewItem;
        }

        #endregion
        

    }
}
