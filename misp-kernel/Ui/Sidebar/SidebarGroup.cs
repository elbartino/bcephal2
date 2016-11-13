using Misp.Kernel.Ui.Sidebar.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Kernel.Ui.Sidebar
{
    public class SidebarGroup : Expander
    {

        #region Properties

        public StackPanel ContentPanel { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SidebarGroup()
        {
            InitComponents();
            UserInitializations();
        }

        /// <summary>
        /// Create a new instance of SidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        public SidebarGroup(string header) : this()
        {
            this.Header = header;
        }

        /// <summary>
        /// Create a new instance of SidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        /// <param name="expanded"></param>
        public SidebarGroup(string header, bool expanded) : this(header)
        {
            this.IsExpanded = expanded;
        }

        #endregion


        #region Operations

        /// <summary>
        /// Initialize data
        /// </summary>
        public virtual void InitializeData() { }

        #endregion


        #region Initializations

        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected virtual void UserInitializations() { }

        /// <summary>
        /// Initialize ContentPanel components
        /// </summary>
        protected virtual void InitComponents()
        {
            this.ContentPanel = new StackPanel();
            this.ContentPanel.Background = Brushes.White;
            this.ContentPanel.Orientation = Orientation.Vertical;
            this.Content = this.ContentPanel;
            this.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            this.FontFamily = new System.Windows.Media.FontFamily("Arial");
            this.BorderBrush = Brushes.LightBlue; 
            this.Background = Brushes.LightBlue;
        }

        #endregion

    }
}
