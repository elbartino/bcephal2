﻿using Misp.Bfc.Model;
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
            if (isSettlement()) return 7;
            return 9;
        }

        protected override System.Windows.Controls.DataGridColumn getColumnAt(int index)
        {            
            return new DataGridTextColumn();
        }

        protected override string getColumnHeaderAt(int index)
        {
            if (isSettlement())
            {
                switch (index)
                {
                    case 0: return "Settlement Advisement n°";
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

                case 0: return "Pre-funding n°";
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

        protected override System.Windows.Controls.DataGridLength getColumnWidthAt(int index)
        {
            if (isSettlement())
            {
                switch (index)
                {
                    case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
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
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
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

        protected override string getBindingNameAt(int index)
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
