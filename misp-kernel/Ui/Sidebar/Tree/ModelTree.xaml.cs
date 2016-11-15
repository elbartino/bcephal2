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
    /// Interaction logic for ModelTree.xaml
    /// </summary>
    public partial class ModelTree : UserControl
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

        public ModelService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetModelService(); } }

        protected DispatcherTimer Timer { get; set; }

        protected static String SHOW_MORE_LABEL = "Show more...";

        protected static int PAGE_SIZE = 2;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ModelTree() 
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
            Display(this.Service.getModelsForSideBar());
        }

        /// <summary>
        /// Display models
        /// </summary>
        /// <param name="models"> Models to display </param>
        public void Display(List<Model> models)
        {
            if (models != null)
            {
                foreach (Model model in models)
                {
                    RefreshParent(model);
                }
            }
            this.treeView.ItemsSource = models;
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

                if (parent is Domain.Attribute)
                {
                    Domain.Attribute attribute = (Domain.Attribute)parent;
                    ForgetDefaultItems(attribute);
                    attribute.DataFilter.page++;
                    BrowserDataPage<AttributeValue> page = this.Service.getRootAttributeValuesByAttribute(attribute.DataFilter);
                    attribute.ClearValuesInItems();
                    foreach (AttributeValue value in page.rows)
                    {
                        value.attribut = attribute;
                        attribute.Items.Add(value);
                    }
                    attribute.isCompleted = true;
                    attribute.DataFilter.page = page.currentPage;
                    attribute.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(attribute);
                }
                else if (parent is AttributeValue)
                {
                    AttributeValue value = (AttributeValue)parent;
                    ForgetDefaultItems(value);
                    value.DataFilter.page++;
                    BrowserDataPage<AttributeValue> page = this.Service.getAttributeValueChildren(value.DataFilter);
                    value.childrenListChangeHandler.Items.Clear();
                    foreach (AttributeValue child in page.rows)
                    {
                        child.parent = value;
                        value.childrenListChangeHandler.Items.Add(child);
                    }
                    value.isCompleted = true;
                    value.DataFilter.page = page.currentPage;
                    value.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(value);
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
            if (action is AttributeValue)
            {
                AttributeValue selection = (AttributeValue)action;
                AttributeValue parent = selection.parent;
                Domain.Attribute attribute = selection.attribut;

                BrowserDataPage<AttributeValue> page = null;
                if (parent != null)
                {
                    ForgetDefaultItems(parent);
                    parent.DataFilter.page++;
                    page = this.Service.getAttributeValueChildren(parent.DataFilter);
                    foreach (AttributeValue value in page.rows)
                    {
                        value.parent = parent;
                        parent.childrenListChangeHandler.Items.Add(value);
                    }
                    parent.isCompleted = true;
                    parent.DataFilter.page = page.currentPage;
                    parent.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(parent);
                }
                else if (attribute != null)
                {
                    ForgetDefaultItems(attribute);
                    attribute.DataFilter.page++;
                    page = this.Service.getRootAttributeValuesByAttribute(attribute.DataFilter);
                    foreach (AttributeValue value in page.rows)
                    {
                        value.attribut = attribute;
                        attribute.Items.Add(value);
                    }
                    attribute.isCompleted = true;
                    attribute.DataFilter.page = page.currentPage;
                    attribute.DataFilter.totalPages = page.pageCount;
                    AddDefaultItems(attribute);
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
                AttributeValue showModeAttributes = new Domain.AttributeValue();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = SHOW_MORE_LABEL;

                if (parent is Domain.Attribute)
                {
                    Domain.Attribute attribute = (Domain.Attribute)parent;
                    showModeAttributes.attribut = attribute;
                    attribute.Items.Add(showModeAttributes);

                }
                else if (parent is AttributeValue)
                {
                    AttributeValue value = (AttributeValue)parent;
                    showModeAttributes.parent = value;
                    value.childrenListChangeHandler.Items.Add(showModeAttributes);
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
                if (parent is Domain.Attribute)
                {
                    Domain.Attribute attribute = (Domain.Attribute)parent;
                    foreach (Domain.AttributeValue value in attribute.Items.ToArray())
                    {
                        if (value.IsDefault) attribute.Items.Remove(value);
                    }
                }
                else if (parent is AttributeValue)
                {
                    AttributeValue value = (AttributeValue)parent;
                    foreach (Domain.AttributeValue child in value.childrenListChangeHandler.Items.ToArray())
                    {
                        if (child.IsDefault) value.childrenListChangeHandler.Items.Remove(child);
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
        public virtual Persistent GetSelectedItem()
        {
            return this.treeView.SelectedItem != null ? this.treeView.SelectedItem as Persistent : null;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="item">The item to select</param>
        public virtual void SetSelectedItem(Persistent item)
        {
            if (item != null)
            {
                item.IsSelected = true;
            }
            else
            {
                Persistent selection = GetSelectedItem();
                if (selection != null) selection.IsSelected = false;
            }
        }


        #endregion


        #region Initializations
        
        /// <summary>
        /// Initialize Treeview Ressources.
        /// </summary>
        protected virtual void InitializeResources()
        {
            //this.Resources = new ResourceDictionary();
            //this.ItemContainerStyle = System.Windows.Application.Current.Resources["SidebarTreeItemStyle"] as Style;
            //this.ItemContainerStyle.Resources.Add("SidebarTreeItemStyle", System.Windows.Application.Current.Resources["SidebarTreeItemStyle"]);
        }
        
        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected virtual void UserInitializations() 
        {
            this.Timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 350), DispatcherPriority.Background, OnTimeTick, Dispatcher.CurrentDispatcher);
            this.Timer.Stop();
            InitializeResources();
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
            Persistent value = GetSelectedItem();
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
