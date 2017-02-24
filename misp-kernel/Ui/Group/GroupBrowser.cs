using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Misp.Kernel.Ui.Group
{
    public class GroupBrowser : Browser<BrowserData>
    {

        public GroupBrowser(Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 3;
        }

        protected override string getTitle() { return "Groups"; }

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
                case 2: return new DataGridTextColumn();
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
                case 1: return "Category";
                case 2: return "Creation Date";
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
                case 2: return 120;
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
                case 1: return "group";
                case 2: return "creationDateTime";
                default: return "oid";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getBindingStringFormatAt(int index)
        {
            switch (index)
            {
                case 2: return "{0:dd/MM/yyyy HH:mm:ss}";
                default: return null;
            }
        }

        protected override bool isReadOnly(int index)
        {
            switch (index)
            {
                case 0: return false;
                default: return true;
            }
        }

    }
}
