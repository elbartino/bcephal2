using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain.Browser;
using System.Windows.Controls;
using Misp.Kernel.Application;

namespace Misp.Sourcing.Table
{
    public class InputTableBrowser : Browser<InputTableBrowserData>
    {

        public InputTableBrowser(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        protected override void initializeGrid()
        {
            base.initializeGrid();
            //this.Grid.IsReadOnly = false; 
        }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 7;
        }

        protected override string getTitle() { return "Input Tables"; }

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
                case 3: return new DataGridTextColumn();
                case 4: return new DataGridCheckBoxColumn();
                case 5: return new DataGridCheckBoxColumn();
                case 6: return new DataGridCheckBoxColumn();
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
                case 1: return "Group";
                case 2: return "Creation Date";
                case 3: return "Modification Date";
                case 4: return "Active";
                case 5: return "Template";
                case 6: return "Visible in shortcut";
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
                case 3: return 120;
                case 4: return 70;
                case 5: return 70;
                case 6: return 100;
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
                case 3: return "modificationDateTime";
                case 4: return "active";
                case 5: return "template";
                case 6: return "visibleInShortcut";
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
                case 3: return "{0:dd/MM/yyyy HH:mm:ss}";
                default: return null;
            }
        }

        protected override bool isReadOnly(int index)
        {
            switch (index)
            {
                case 0: return false;
                case 1: return true;
                case 2: return true;
                case 3: return true;
                case 4: return false;
                case 5: return false;
                case 6: return false;
                default: return true;
            }
        }

        
    }
}
