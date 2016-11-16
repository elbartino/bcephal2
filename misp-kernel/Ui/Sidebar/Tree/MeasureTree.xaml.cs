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
    /// Interaction logic for MeasureTree.xaml
    /// </summary>
    public partial class MeasureTree : UserControl
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

        public MeasureService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetMeasureService(); } }

        protected DispatcherTimer Timer { get; set; }

        protected static String SHOW_MORE_LABEL = "Show more...";

        protected static int PAGE_SIZE = 2;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeasureTree() 
        {
            InitializeComponent();
            UserInitializations();
        }

        #endregion


        #region Operations

        /// <summary>
        /// Initialize data
        /// </summary>
        public virtual void InitializeData(bool showPostingMeasure = true)
        {
            Display(this.Service.getRootMeasureForSideBar(showPostingMeasure));
        }

        /// <summary>
        /// Display models
        /// </summary>
        /// <param name="models"> Models to display </param>
        public void Display(Domain.Measure root)
        {
            if (root == null) this.treeView.ItemsSource = null;
            else {
                foreach (Domain.Measure measure in root.childrenListChangeHandler.Items)
                {
                    RefreshParent(measure);
                }
                this.treeView.ItemsSource = root.childrenListChangeHandler.Items;
            }            
        }



        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        public virtual void RefreshParent(Persistent parent)
        {
            if (parent is Model)
            {
                Model model = (Model)parent;
                foreach (Entity entity in model.entityListChangeHandler.Items)
                {
                    entity.model = model;
                    RefreshParent(entity);
                }
            }
            else if (parent is Entity)
            {
                Entity entity = (Entity)parent;
                foreach (Domain.Attribute attribute in entity.attributeListChangeHandler.Items)
                {
                    attribute.entity = entity;
                    RefreshParent(attribute);
                }
            }
            else if (parent is Domain.Attribute)
            {
                Domain.Attribute attribute = (Domain.Attribute)parent;
                attribute.LoadItems();
                foreach (Domain.Attribute child in attribute.childrenListChangeHandler.Items)
                {
                    child.parent = attribute;
                    RefreshParent(child);
                }
                foreach (AttributeValue value in attribute.valueListChangeHandler.Items)
                {
                    value.attribut = attribute;
                    RefreshParent(value);
                }
            }
            else if (parent is AttributeValue)
            {
                AttributeValue value = (AttributeValue)parent;
                foreach (AttributeValue child in value.childrenListChangeHandler.Items)
                {
                    child.parent = value;
                    RefreshParent(child);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected item</returns>
        public virtual Domain.Measure GetSelectedItem()
        {
            return this.treeView.SelectedItem != null ? this.treeView.SelectedItem as Domain.Measure : null;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="item">The item to select</param>
        public virtual void SetSelectedItem(Domain.Measure item)
        {
            if (item != null)
            {
                item.IsSelected = true;
            }
            else
            {
                Domain.Measure selection = GetSelectedItem();
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
            Domain.Measure value = GetSelectedItem();
            if (value != null && sender != null && sender is TreeViewItem)
            {
                TreeViewItem item = (TreeViewItem)sender;
                if (!item.IsSelected) { e.Handled = true; return; }
                else if (!value.IsDefault && DoubleClick != null) DoubleClick(value);
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
            Domain.Measure value = GetSelectedItem();
            if (value != null)
            {
                if (value.IsDefault) { }
                else if (Click != null) Click(value);
            }
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            this.Timer.Stop();
            object obj = ((TreeViewItem)e.OriginalSource).Header;
            if (obj != null && obj is Domain.Measure)
            {
                Domain.Measure value = (Domain.Measure)obj;
                if (value != null && !value.IsDefault)
                {
                    if (!value.isCompleted) { }
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
