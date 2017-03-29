using DevExpress.Xpf.Grid;
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
            this.Form.Grid.Columns.Clear();
            for (int i = 0; i < getColumnCount(); i++)
            {
                GridColumn column = getColumn(i);
                this.Form.Grid.Columns.Add(column);
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

        protected bool isReplenishment()
        {
            return this.AdvisementType == AdvisementType.REPLENISHMENT;
        }

        protected bool isSettlement()
        {
            return this.AdvisementType == AdvisementType.SETTLEMENT;
        }
                
        protected override string getTitle()
        {
            if (isMember()) return "Member Advisements";
            else if (isReplenishment()) return "Replenishment Instructions";
            else if (isSettlement()) return "Settlement Advisements";
            return "Prefunding Advisements"; 
        }

        protected override int getColumnCount()
        {
            if (isSettlement()) return 7;
            return 9;
        }

        protected override string getColumnHeaderAt(int index)
        {
            if (isSettlement())
            {
                switch (index)
                {
                    case 0: return "SA n°";
                    case 1: return "Date";
                    case 2: return "Scheme";
                    case 3: return "Amount";
                    case 4: return "D/C";
                    case 5: return "Value date";
                    case 6: return "Creator";
                    default: return "";
                }
            }
            switch (index)
            {

                case 0: return isMember() ? "MA n°" : isReplenishment() ? "RI n°" : "PF n°";
                case 1: return "Date";
                case 2: return "Member Bank";
                case 3: return "PML";
                case 4: return "Scheme";
                case 5: return "Amount";
                case 6: return "D/C";
                case 7: return "Value date";
                case 8: return "Creator";
                default: return "";
            }
        }

        protected override GridColumnWidth getColumnWidthAt(int index)
        {
            if (isSettlement())
            {
                switch (index)
                {
                    case 0: return new GridColumnWidth(1, GridColumnUnitType.Star);
                    case 1: return 100;
                    case 2: return 150;
                    case 3: return 150;
                    case 4: return 50;
                    case 5: return 100;
                    case 6: return 100;
                    default: return 100;
                }
            }
            switch (index)
            {
                case 0: return new GridColumnWidth(1, GridColumnUnitType.Star);
                case 1: return 100;
                case 2: return 150;
                case 3: return 150;
                case 4: return 100;
                case 5: return 100;
                case 6: return 50;
                case 7: return 100;
                case 8: return 100;
                default: return 100;
            }
        }

        protected override string getFieldNameAt(int index)
        {
            if (isSettlement())
            {
                switch (index)
                {
                    case 0: return "code";
                    case 1: return "creationDateTime";
                    case 2: return "scheme";
                    case 3: return "amount";
                    case 4: return "dc";
                    case 5: return "valueDate";
                    case 6: return "creator";
                    default: return "oid";
                }
            }
            switch (index)
            {
                case 0: return "code";
                case 1: return "creationDateTime";
                case 2: return "memberBank";
                case 3: return "pml";
                case 4: return "scheme";
                case 5: return "amount";
                case 6: return "dc";
                case 7: return "valueDate";
                case 8: return "creator";
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
            if (isSettlement())
            {
                switch (index)
                {
                    case 1: return "{0:dd/MM/yyyy}";
                    case 5: return "{0:dd/MM/yyyy}";
                    default: return null;
                }
            }
            switch (index)
            {
                case 1: return "{0:dd/MM/yyyy}";
                case 7: return "{0:dd/MM/yyyy}";
                default: return null;
            }
        }
    }
}
