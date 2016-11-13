using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Sidebar
{
    public class GroupSidebarGroup : SidebarGroup
    {

        #region Properties


        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupSidebarGroup() : this("Groups") { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        public GroupSidebarGroup(string header) : this(header, true) { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        /// <param name="expanded"></param>
        public GroupSidebarGroup(string header, bool expanded) : base(header, expanded) { }

        #endregion


        #region Operations

        

        #endregion


        #region Initializations

        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected override void UserInitializations()
        {
            
        }

        #endregion

    }
}
