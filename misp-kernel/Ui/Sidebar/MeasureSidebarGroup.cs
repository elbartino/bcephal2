using Misp.Kernel.Application;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Sidebar.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Sidebar
{
    public class MeasureSidebarGroup : SidebarGroup
    {

        #region Properties

        public MeasureTree Tree { get; set; }

        #endregion


        #region Constructors
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public MeasureSidebarGroup() : this("Measures") { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        public MeasureSidebarGroup(string header) : this(header, true) { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        /// <param name="expanded"></param>
        public MeasureSidebarGroup(string header, bool expanded) : base(header, expanded) { }

        #endregion


        #region Operations

        /// <summary>
        /// Initialize data
        /// </summary>
        public virtual void InitializeMeasure(bool showPostingMeasure = true)
        {
            this.Tree.InitializeData(showPostingMeasure);
        }

        #endregion


        #region Initializations

        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected override void UserInitializations()
        {
            this.Background = System.Windows.Media.Brushes.LightBlue;
            this.Background = System.Windows.Media.Brushes.LightBlue;
            this.Tree = new MeasureTree();            
            this.ContentPanel.Children.Add(this.Tree);
        }

        #endregion


        #region Utils

        
        #endregion

    }
}
