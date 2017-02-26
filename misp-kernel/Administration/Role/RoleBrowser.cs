using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Kernel.Administration.Role
{
    public class RoleBrowser : Browser<BrowserData>
    {

        public RoleBrowser(Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 2;
        }

        public RoleForm form;

        protected override void initializeGrid()
        {
            base.initializeGrid();
            this.Children.RemoveAt(this.Children.Count - 1);

            form = new RoleForm(this.SubjectType);

            LayoutDocument page = new LayoutDocument();
            page.CanClose = false;
            page.CanFloat = false;
            page.Title = getTitle();
            page.Content = form;
            this.Children.Add(page);
        }

        protected override string getTitle() { return "Roles"; }

        /// <summary>
        /// Build and returns the column at index position
        /// </summary>
        /// <param name="index">The position of the column</param>
        /// <returns></returns>
        protected override DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                default: return new DataGridTextColumn();
            }
        }


        /// <summary>
        /// Column Label
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Name";
                case 1: return "Delete";
                default: return "";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return 150;
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getBindingNameAt(int index)
        {
            switch (index)
            {
                case 0: return "name";
                case 1: return "delete";
                default: return "oid";
            }
        }

        protected override bool isReadOnly(int index)
        {
            switch (index)
            {
                case 0: return false;
                case 1: return true;
                default: return true;
            }
        }
    }
}
