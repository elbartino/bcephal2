using Misp.Kernel.Application;
using Misp.Kernel.Ui.Sidebar.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Sidebar
{
    public class SideBar : ScrollViewer
    {
        #region Properties

        public StackPanel MainPanel { get; protected set; }
        public StatusSidebarGroup StatusGroup { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SideBar()
        {
            InitComponents();
        }

        #endregion


        #region Operations

        public virtual void SetReadOnly(bool readOnly)
        {
            
        }

        public void AddGroup(Expander group, int position)
        {
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Insert(position, group);
        }

        public void RemoveGroup(int position)
        {
            this.MainPanel.Children.RemoveAt(position);
        }

        public void AddGroup(Expander group)
        {
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Add(group);
        }

        public void RemoveGroup(Expander group)
        {
            this.MainPanel.Children.Remove(group);
        }

        public void Clear()
        {
            this.MainPanel.Children.Clear();
        }

        public void SelectStatus(String name)
        {
            if (this.StatusGroup != null) this.StatusGroup.SelectStatus(name);
        }

        #endregion
        

        #region Initializations

        /// <summary>
        /// Initialize MainPanel and groups.
        /// </summary>
        private void InitComponents()
        {
            this.MainPanel = new StackPanel();
            this.MainPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.Content = this.MainPanel;
            InitializeGroups();
        }

        /// <summary>
        /// Initialize groups
        /// </summary>
        public virtual void InitializeGroups()
        {
            InitializeDefaultGroups();
        }

        /// <summary>
        /// Initialize StatusGroup
        /// </summary>
        protected void InitializeDefaultGroups()
        {
            StatusGroup = new StatusSidebarGroup();
            this.AddGroup(StatusGroup);
        }

        #endregion

    }
}
