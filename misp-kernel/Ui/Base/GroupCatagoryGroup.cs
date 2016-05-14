using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;
using System.Windows.Controls;
using Misp.Kernel.Domain;

namespace Misp.Kernel.Ui.Base
{
    public class GroupCatagoryGroup : SideBarExpander
    {

        #region Properties

        public ListBox GroupCatagoryListBox { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public GroupCatagoryGroup() : base() { }

        public GroupCatagoryGroup(string header) : base(header) { }

        public GroupCatagoryGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.GroupCatagoryListBox = new ListBox();
            this.GroupCatagoryListBox.SelectionMode = SelectionMode.Single;
            this.ContentPanel.Children.Add(this.GroupCatagoryListBox);
        }

        #endregion

    }
}
