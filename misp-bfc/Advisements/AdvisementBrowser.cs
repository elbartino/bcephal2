using Misp.Bfc.Model;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Bfc.Advisements
{
    public class AdvisementBrowser : Browser<AdvisementBrowserData>
    {

        public AdvisementBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }


        public AdvisementType AdvisementType { get; set; }
        public AdvisementBrowser(Kernel.Domain.SubjectType subjectType, String functionality, AdvisementType advisementType) : base(subjectType,functionality)
        {
            this.AdvisementType = advisementType;
            customizeGrid();
        }

        private void customizeGrid()
        {
            grid.Columns.Clear();
            for (int i = 0; i < getColumnCount(); i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeaderAt(i);
                column.Width = getColumnWidthAt(i);
                column.IsReadOnly = isReadOnly(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                grid.Columns.Add(column);
            }
            ((LayoutDocument)this.Children[0]).Title = getTitle();
        }

        protected bool isPrefunding()
        {
            return this.AdvisementType == AdvisementType.PREFUNDING;
        }

        protected bool isMemeber()
        {
            return this.AdvisementType == AdvisementType.MEMBER;
        }

        protected bool isExceptional()
        {
            return this.AdvisementType == AdvisementType.EXCEPTIONAL;
        }

        protected bool isSettlement()
        {
            return this.AdvisementType == AdvisementType.SETTLEMENT;
        }

        

        protected override string getTitle()
        {
            if (isMemeber()) return "Member Advisement";
            else if (isExceptional()) return "Exception Advisement";
            else if (isSettlement()) return "Settlement Advisement";
            return "Prefunding Advisement"; 
        }

        protected override int getColumnCount()
        {
            if (isExceptional() || isSettlement()) return 6;
            return 7;
        }

        protected override System.Windows.Controls.DataGridColumn getColumnAt(int index)
        {
            if (isPrefunding())
            {
                switch (index)
                {
                    case 0: return new DataGridTextColumn();
                    case 1: return new DataGridTextColumn();
                    case 2: return new DataGridTextColumn();
                    case 3: return new DataGridTextColumn();
                    case 4: return new DataGridTextColumn();
                    case 5: return new DataGridTextColumn();
                    default: return new DataGridTextColumn();
                }
            }
            return new DataGridTextColumn();
        }

        protected override string getColumnHeaderAt(int index)
        {
            if (isPrefunding())
            {
                switch (index)
                {
                    case 0: return "Name";
                    case 1: return "Group";
                    case 2: return "Creation Date";
                    case 3: return "Modification Date";
                    case 4: return "Active";
                    case 5: return "Template";
                    default: return "";
                }
            }
            return "";
        }

        protected override System.Windows.Controls.DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return 150;
                case 2: return 120;
                case 3: return 120;
                case 4: return 70;
                case 5: return 70;
                default: return 100;
            }
        }

        protected override string getBindingNameAt(int index)
        {
            if (isPrefunding())
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
            return "";
        }
    }
}
