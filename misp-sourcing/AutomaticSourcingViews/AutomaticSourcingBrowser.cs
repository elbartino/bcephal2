using DevExpress.Xpf.Grid;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    public class AutomaticSourcingBrowser : Browser<BrowserData>
    {

        public AutomaticSourcingBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 5;
        }

        protected virtual bool isAutomaticGrid()
        {
            return false;
        }

        protected override string getTitle() {
            return  isAutomaticGrid() ? "Automatic Grid" : "Automatic Sourcing"; 
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
                case 4: return "Visible in shortcut";
                default: return "";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override GridColumnWidth getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new GridColumnWidth(1, GridColumnUnitType.Star);
                case 1: return 150;
                case 2: return 120;
                case 3: return 120;
                case 4: return 100;
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getFieldNameAt(int index)
        {
            switch (index)
            {
                case 0: return "name";
                case 1: return "group";
                case 2: return "creationDateTime";
                case 3: return "modificationDateTime";
                case 4: return "visibleInShortcut";
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
                case 4: return false;
                default: return true;
            }
        }

    }
}
