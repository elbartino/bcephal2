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

        protected bool isMember()
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
            if (isMember()) return "Member Advisement";
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
            if (isMember())
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
            if (isExceptional())
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

            if (isSettlement())
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
                   
                    case 0: return "Pre-funding n°";
                    case 1: return "Date";
                    case 2: return "Member Bank";
                    case 3: return "Scheme";
                    case 4: return "Amount";
                    case 5: return "Value date";
                    case 6: return "Creator";
                    default: return "";
                }
            }
            if (isExceptional())
            {
                switch (index)
                {
                    case 0: return "Replenishment Instruction n°";
                    case 1: return "Date";
                    case 2: return "Member Bank";
                    case 3: return "Scheme";
                    case 4: return "Amount";
                    case 5: return "Value Date";
                    case 6: return "Creator";
                    default: return "";
                }
            }

            if (isMember())
            {
                switch (index)
                {
                    case 0: return "Member Advisement n°";
                    case 1: return "Date";
                    case 2: return "Member Bank";
                    case 3: return "Scheme";
                    case 4: return "Amount";
                    case 5: return "Value Date";
                    case 6: return "Creator";
                    default: return "";
                }
            }

            if (isSettlement())
            {
                switch (index)
                {
                    case 0: return "Settlement Advisement n°";
                    case 1: return "Date";
                    case 2: return "Scheme";
                    case 3: return "Amount";
                    case 4: return "Value Date";
                    case 5: return "Creator";
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
                    case 1: return "creationDateTime";
                    case 2: return "memberBank";
                    case 3: return "scheme";
                    case 4: return "amount";
                    case 5: return "valueDate";
                    case 6: return "creator";
                    default: return "oid";
                }
            }

            if (isMember())
            {
                switch (index)
                {
                    case 0: return "name";
                    case 1: return "creationDateTime";
                    case 2: return "memberBank";
                    case 3: return "amount";
                    case 4: return "valueDate";
                    case 5: return "creator";
                    default: return "oid";
                }
            }

            if (isExceptional())
            {
                switch (index)
                {
                    case 0: return "name";
                    case 1: return "creationDateTime";
                    case 2: return "memberBank";
                    case 3: return "scheme";
                    case 4: return "amount";
                    case 5: return "valueDate";
                    case 6: return "creator";
                    default: return "oid";
                }
            }

            if (isSettlement()) 
            {
                switch (index)
                {
                    case 0: return "name";
                    case 1: return "creationDateTime";
                    case 2: return "scheme";
                    case 3: return "amount";
                    case 4: return "valueDate";
                    case 5: return "creator";
                    default: return "oid";
                }
            }

            return "";
        }
    }
}
