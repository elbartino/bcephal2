using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.ObjectAdmin
{
    public class AdministrationBar : ScrollViewer
    {

        #region Properties

        public StackPanel MainPanel { get; protected set; }

        public int? ObjectOid { get; set; }

        public String ObjectType { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public AdministrationBar()
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
            
        }
        
        #endregion

    }
}
