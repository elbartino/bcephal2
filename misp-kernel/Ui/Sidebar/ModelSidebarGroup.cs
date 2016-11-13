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
    public class ModelSidebarGroup : SidebarGroup
    {

        #region Properties

        public ModelTree Tree { get; set; }

        #endregion


        #region Constructors
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public ModelSidebarGroup() : this("Entities") { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        public ModelSidebarGroup(string header) : this(header, true) { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        /// <param name="expanded"></param>
        public ModelSidebarGroup(string header, bool expanded) : base(header, expanded) { }

        #endregion


        #region Operations

        /// <summary>
        /// Initialize data
        /// </summary>
        public override void InitializeData()
        {
            this.Tree.InitializeData();
        }

        #endregion


        #region Initializations

        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected override void UserInitializations()
        {
            this.Background = System.Windows.Media.Brushes.LightBlue;
            this.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.Tree = new ModelTree();            
            this.ContentPanel.Children.Add(this.Tree);
        }

        #endregion


        #region Utils

        
        #endregion

    }
}
