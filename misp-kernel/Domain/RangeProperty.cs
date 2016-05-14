using Misp.Kernel.Ui.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace Misp.Kernel.Domain
{
    public class RangeProperty:Persistent
    {
        
        
        public enum AllocationType { FOR_ALLOCATION, FOR_REFERENCE }

        public RangeProperty() 
        {
            allocationType = AllocationType.FOR_ALLOCATION.ToString();
            this.childrenListChangeHandler = new PersistentListChangeHandler<RangeProperty>();
            this.resetCellListChangeHandler = new PersistentListChangeHandler<ResetCell>();
        }

        public RangeProperty(RangeItem rangeItem) 
        {
            this.nameRow1 = rangeItem.Row1.ToString();
            this.nameColumn1 = Kernel.Util.RangeUtil.GetColumnName(rangeItem.Column1);

            this.nameRow2 =rangeItem.Row2.ToString();
            this.nameColumn2 = Kernel.Util.RangeUtil.GetColumnName(rangeItem.Column2);
            this.name = rangeItem.Name;
            this.childrenListChangeHandler = new PersistentListChangeHandler<RangeProperty>();
            this.resetCellListChangeHandler = new PersistentListChangeHandler<ResetCell>();
        }

        public string name { get; set; }
        public string nameRow1 { get; set; }
        public string nameColumn1 { get; set; }
        public string nameRow2 { get; set; }
        public string nameColumn2 { get; set; }

        public int indexSheet { get; set; }
        public string nameSheet { get; set; }
        public string periodFrom { get; set; }
        public string periodTo { get; set; }
        public CellMeasure cellMeasure { get; set; }
        public string allocationType { get; set; }
        public CellPropertyAllocationData cellAllocationData { get; set; }
        public Target cellScope { get; set; }
        public PersistentListChangeHandler<RangeProperty> childrenListChangeHandler { get; set; }
        public PersistentListChangeHandler<ResetCell> resetCellListChangeHandler { get; set; }

        /// <summary>
        /// La date de début
        /// </summary>
        [ScriptIgnore]
        public DateTime periodFromDateTime
        {
            get { return DateTime.Parse(periodFrom); }
            set { periodFrom = value.ToShortDateString(); }
        }

        /// <summary>
        /// La date de fin
        /// </summary>
        [ScriptIgnore]
        public DateTime periodToDateTime
        {
            get { return DateTime.Parse(periodTo); }
            set { periodTo = value.ToShortDateString(); }
        }

      
        [ScriptIgnore]
        public bool IsForAllocation
        {
            get
            {
                return allocationType != null ? allocationType == AllocationType.FOR_ALLOCATION.ToString() : false;
            }
            set
            {
                allocationType = value ? AllocationType.FOR_ALLOCATION.ToString() : AllocationType.FOR_REFERENCE.ToString();
            }
        }


        public RangeProperty GetCopy()
        {
            RangeProperty cellProperty = new RangeProperty();
            cellProperty.nameColumn1 = this.nameColumn1;
            cellProperty.nameRow1 = this.nameRow1;
            cellProperty.nameSheet = this.nameSheet;
            cellProperty.allocationType = this.allocationType;
            cellProperty.cellMeasure = this.cellMeasure != null ? this.cellMeasure.GetCopy() : null;
            cellProperty.cellAllocationData = this.cellAllocationData != null ? this.cellAllocationData.GetCopy() : null;
            cellProperty.cellScope = this.cellScope != null ? this.cellScope.GetCopy() : null;
            return cellProperty;
        }

        /// <summary>
        /// sets range parameters to cellProperty
        /// </summary>
        /// <param name="cellProperty"></param>
        /// <returns></returns>
        public CellProperty CompleteCellProperty(CellProperty cellProperty)
        {
            cellProperty.indexSheet = this.indexSheet;
            cellProperty.nameSheet = this.nameSheet;
            cellProperty.cellMeasure = this.cellMeasure != null ? this.cellMeasure : cellProperty.cellMeasure;
            cellProperty.cellAllocationData = this.cellAllocationData != null ? this.cellAllocationData : cellProperty.cellAllocationData;
            cellProperty.cellScope = this.cellScope != null ? this.cellScope : cellProperty.cellScope;
            return cellProperty;
        }

        /// <summary>
        /// Verify whether a cell in within a range
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsCellInRange(Kernel.Ui.Office.Cell cell) 
        {
            List<Point> bornes =  Kernel.Util.RangeUtil.getCellsCoord(this.name);
            int startLine = (int)Convert.ToInt64(this.nameRow1);
            int endLine = (int)Convert.ToInt64(this.nameRow2);

            int startCol = (int)Convert.ToInt64(Kernel.Util.RangeUtil.GetColumnIndex(this.nameColumn1));
            int endCol = (int)Convert.ToInt64(Kernel.Util.RangeUtil.GetColumnIndex(this.nameColumn2));
            
            if(startLine <= cell.Row && endLine >= cell.Row)
                if (startCol <= cell.Column && endCol >= cell.Column)
                    return true;
 
            return false;
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is RangeProperty)) return 1;
            return this.name.CompareTo(((RangeProperty)obj).name);
        }


        public override string ToString()
        {
            return this.name;
        }
    }
}
