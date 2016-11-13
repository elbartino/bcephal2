using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Misp.Kernel.Ui.Sidebar.Tree
{
    public class SidebarTree : System.Windows.Controls.TreeView
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

        protected DispatcherTimer Timer { get; set; }

        protected static String SHOW_MORE_LABEL = "Show more...";

        protected static int PAGE_SIZE = 10;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SidebarTree() 
        {
            InitializeComponents();
        }

        #endregion


        #region Operations

        /// <summary>
        /// Initialize data
        /// </summary>
        public virtual void InitializeData() { }

        /// <summary>
        /// Display children of root node
        /// </summary>
        /// <param name="root"> Object representing the root node </param>
        public virtual void DisplayRoot(List<Persistent> roots)
        {            
            this.ItemsSource = roots;
        }

        /// <summary>
        /// Load no complete object.
        /// </summary>
        /// <param name="parent"></param>
        protected virtual void Load(Persistent parent)
        {
            if (parent == null || parent.IsDefault) return;
        }

        /// <summary>
        /// Perform default action
        /// </summary>
        /// <param name="action"></param>
        protected virtual void PerformDefaultAction(Persistent action) 
        {
            if (action == null || !action.IsDefault) return;
        }

        protected virtual void AddDefaultItems(Persistent parent)
        {
            
        }

        /// <summary>
        /// Remove default nodes from root
        /// </summary>
        protected virtual void ForgetDefaultItems(Persistent parent)
        {
            
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        public virtual void RefreshParent(Persistent parent)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected item</returns>
        public virtual Persistent GetSelectedItem()
        {
            return this.SelectedItem != null ? this.SelectedItem as Persistent : null;
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
        /// Initialize tree and treeItem resources,
        /// Initialize Timer used to handle clik and double clik.
        /// </summary>
        private void InitializeComponents()
        {
            this.Timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 350), DispatcherPriority.Background, OnTimeTick, Dispatcher.CurrentDispatcher);
            this.Timer.Stop();
            InitializeResources();
            UserInitializations();
        }

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
        protected virtual void UserInitializations() { }

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
            if (value != null)
            {
                if (value.IsDefault) PerformDefaultAction(value);
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
            Persistent value = (Persistent)((TreeViewItem)e.OriginalSource).Header;
            if (value != null && !value.IsDefault)
            {
                if (!value.isCompleted) Load(value);
                if (Expanded != null) Expanded(value);
            }
            
            
            //Domain.AttributeValue value = (Domain.AttributeValue)((TreeViewItem)e.OriginalSource).Header;
            //if (value != this.Root && Expanded != null)
            //{
            //    ForgetDefaultAttributeValues(value);
            //    Expanded(value);
            //    AddDefaultAttributeValues(value);
            //}
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
