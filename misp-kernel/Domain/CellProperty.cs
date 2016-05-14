using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class CellProperty : Persistent
    {
        
        /// <summary>
        /// Construit une npouvelle instance de CellProperty.
        /// </summary>
        public CellProperty() 
        {
            forAllocation = true;
        }

        /// <summary>
        /// Construit une npouvelle instance de CellProperty.
        /// </summary>
        /// <param name="nameRow">Le nom de la ligne de la cellule</param>
        /// <param name="nameColumn">Le nom de la colonne de la cellule</param>
        public CellProperty(string name, int row, int column) : this()
        {
            this.name = name;
            this.row = row;
            this.column = column;
        }

        /// <summary>
        /// Construit une npouvelle instance de CellProperty.
        /// </summary>
        /// <param name="nameRow">Le nom de la ligne de la cellule</param>
        /// <param name="nameColumn">Le nom de la colonne de la cellule</param>
        public CellProperty(string name, int row, int column, string sheetName)
            : this(name, row, column)
        {
            this.nameSheet = sheetName;
        }

        /// <summary>
        /// Construit une npouvelle instance de CellProperty.
        /// </summary>
        /// <param name="nameRow">Le nom de la ligne de la cellule</param>
        /// <param name="nameColumn">Le nom de la colonne de la cellule</param>
        public CellProperty(string name, int row, int column, int sheetIndex)
            : this(name, row, column)
        {
            this.indexSheet = sheetIndex;
        }


        public bool isValueChanged { get; set; }

        public CellPropertyAllocationData cellAllocationData { get; set; }

        public string name { get; set; }

        public int column { get; set; }

        public int row { get; set; }

        public string nameSheet { get; set; }
        
        public int indexSheet { get; set; }

        public bool forAllocation { get; set; }

        public Target cellScope { get; set; }
        
        public Period period { get; set; }

        public decimal decimalMeasure { get; set; }

        public string decimalMeasureMessage { get; set; }

        public CellMeasure cellMeasure { get; set; }
        
                

        [ScriptIgnore]
        public bool IsForAllocation
        { 
            get { return forAllocation; }
            set { forAllocation = value; }
        }
                

        public void SetDecimalMeasure(object value)
        {
            if (value == null) this.decimalMeasure = decimal.Zero;
            else
            {
                try
                {
                    this.decimalMeasure = decimal.Parse(value.ToString());
                }
                catch(Exception)
                {
                    this.decimalMeasure = decimal.Zero;
                }
            }
        }


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is CellProperty)) return 1;
            return this.name.CompareTo(((CellProperty)obj).name);
        }


        public CellProperty GetCopy()
        {
            CellProperty cellProperty = new CellProperty();
            cellProperty.column = this.column;
            cellProperty.row = this.row;
            cellProperty.name = this.name;
            cellProperty.nameSheet = this.nameSheet;
            cellProperty.decimalMeasure = this.decimalMeasure;
            cellProperty.forAllocation = this.forAllocation;
            cellProperty.cellMeasure = this.cellMeasure != null ? this.cellMeasure.GetCopy() : null;
            cellProperty.cellAllocationData = this.cellAllocationData != null ? this.cellAllocationData.GetCopy() : null;
            cellProperty.cellScope = this.cellScope != null ? this.cellScope.GetCopy() : null;
            cellProperty.indexSheet = this.indexSheet;
            return cellProperty;
        }

    }
}
