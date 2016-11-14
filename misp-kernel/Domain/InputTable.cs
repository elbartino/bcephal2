using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class InputTable : Persistent, IComparable
    {

        public string name { get; set; }

        public bool template { get; set; }

        public bool active { get; set; }

        public bool visibleInShortcut { get; set; }
                
        public string excelFileName { get; set; }

        public String excelFileExtension { get; set; }

        [ScriptIgnore]
        public string periodFrom { get; set; }

        [ScriptIgnore]
        public string periodTo { get; set; }

        [ScriptIgnore]
        public string formulaPeriodFrom { get; set; }

        [ScriptIgnore]
        public string formulaPeriodTo { get; set; }

        [ScriptIgnore]
        public string formatPeriodFrom { get; set; }

        [ScriptIgnore]
        public string formatPeriodTo { get; set; }

        public Target filter { get; set; }

        public Period period { get; set; }

        public BGroup group { get; set; }

        public int? tranformationTreeOid { get; set; }

        [ScriptIgnore]
        public PersistentListChangeHandler<CellProperty> cellPropertyListChangeHandler  {get;set;}

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


        public InputTable()
        {
            this.cellPropertyListChangeHandler = new PersistentListChangeHandler<CellProperty>();
        }


        /// <summary>
        /// Rajoute un CellProperty
        /// </summary>
        /// <param name="cell"></param>
        public void AddCellProperty(CellProperty cell, bool sort = true)
        {
            cell.isModified = true;
            cellPropertyListChangeHandler.AddNew(cell, sort);
            OnPropertyChanged("cellPropertyListChangeHandler.Items");
        }

        /// <summary>
        /// RangeProperty
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="sort"></param>
        public void AddRangeProperty(RangeProperty cell, bool sort = true)
        {
           // rangePropertyListChangeHandler.AddNew(cell, sort);
            OnPropertyChanged("rangePropertyListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un CellProperty
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateCellProperty(CellProperty cell, bool sort = true)
        {
            cell.isModified = true;
            cellPropertyListChangeHandler.AddUpdated(cell, sort);
            OnPropertyChanged("cellPropertyListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un RangeProperty
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateRangeProperty(RangeProperty range, bool sort = true)
        {
          //  rangePropertyListChangeHandler.AddUpdated(range, sort);
            OnPropertyChanged("rangePropertyListChangeHandler.Items");
        }


        /// <summary>
        /// Retire un CellProperty
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveCellProperty(CellProperty cell, bool sort = true)        
        {
            cell.isModified = true;
            cellPropertyListChangeHandler.AddDeleted(cell, sort);
            OnPropertyChanged("cellPropertyListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un RangeProperty
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveRangeProperty(RangeProperty range, bool sort = true)
        {
            //rangePropertyListChangeHandler.AddDeleted(range, sort);
            OnPropertyChanged("rangePropertyListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un CellProperty
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetCellProperty(CellProperty cell, bool sort = true)
        {
            cellPropertyListChangeHandler.forget(cell, sort);
            OnPropertyChanged("cellPropertyListChangeHandler.Items");
        }


        /// <summary>
        /// Retourne la cellule identifiée par des coordonnées. 
        /// </summary>
        /// <param name="row">Le numéro de la ligne de la cellule</param>
        /// <param name="col">Le numéro de la colonne de la cellule</param>
        /// <returns>Retourne la cellule [row, col] si elle exite dans la table, sinon retourne NULL</returns>
        public CellProperty GetCellProperty(int row, int col, int sheetIndex, string sheetName)
        {
            string cellName = Kernel.Util.RangeUtil.GetCellName(row, col);
            return GetCellProperty(cellName, sheetIndex, sheetName);
        }


        /// <summary>
        /// Retourne la cellule identifiée par un nom.
        /// </summary>
        /// <param name="cellName">Le nom de la cellule</param>
        /// <returns>Retourne la cellule identifiée par cellName si elle exite dans la table, sinon retourne NULL</returns>
        public CellProperty GetCellProperty(string cellName, int sheetIndex, string sheetName)
        {
            foreach (CellProperty cell in cellPropertyListChangeHandler.Items)
            {
                if (cell.name == cellName)
                {
                    if (sheetName != null && cell.nameSheet == sheetName) return cell;
                    //if (sheetIndex > 0 && cell.indexSheet == sheetIndex) return cell;
                    //else if (sheetName != null && cell.nameSheet == sheetName) return cell;
                }
            }
            return null;
        }

        /// <summary>
        /// Retourne la cellule identifiée par des coordonnées. 
        /// </summary>
        /// <param name="row">Le numéro de la ligne de la cellule</param>
        /// <param name="col">Le numéro de la colonne de la cellule</param>
        /// <returns>Retourne la cellule [row, col] si elle exite dans la table, sinon retourne NULL</returns>
        public CellProperty GetCellProperty(int row, int col, string sheetName)
        {
            string cellName = Kernel.Util.RangeUtil.GetCellName(row, col);
            return GetCellProperty(cellName, sheetName);
        }

        /// <summary>
        /// Retourne la cellule identifiée par un nom.
        /// </summary>
        /// <param name="cellName">Le nom de la cellule</param>
        /// <returns>Retourne la cellule identifiée par cellName si elle exite dans la table, sinon retourne NULL</returns>
        public CellProperty GetCellProperty(string cellName, string sheetName)
        {
            foreach (CellProperty cell in cellPropertyListChangeHandler.Items)
            {
                if (cell.name == cellName && cell.nameSheet == sheetName) return cell;
            }
            return null;
        }

        public CellProperty GetCellProperty(string cellName, int sheetIndex)
        {
            foreach (CellProperty cell in cellPropertyListChangeHandler.Items)
            {
                if (cell.name == cellName && cell.indexSheet == sheetIndex) return cell;
            }
            return null;
        }
        

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is InputTable)) return 1;
            return this.name.CompareTo(((InputTable)obj).name);
        }

        public Target correctFilter()
        {
            if (filter != null)
            {
                filter.targetItemListChangeHandler.Items = getItems();
            }
            return filter;
        }

        /**
         * List of items that are currenlty in the list it is equal to:
         * original - deleted + newItems
         */
        private ObservableCollection<TargetItem> getItems()
        {
            ObservableCollection<TargetItem> result = new ObservableCollection<TargetItem>();
            if (filter.targetItemListChangeHandler.originalList != null)
            {
                foreach (TargetItem item in filter.targetItemListChangeHandler.originalList)
                {
                    bool found = false;
                    foreach (TargetItem deleted in filter.targetItemListChangeHandler.deletedItems)
                    {
                        if (sameItems(item, deleted) && item.isDeleted)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        result.Add(item);
                    }
                }
            }
            foreach (TargetItem item in filter.targetItemListChangeHandler.newItems)
            {
                result.Add(item);
            }
            return result;
        }

        private bool sameItems(TargetItem item, TargetItem other)
        {
            bool result = false;
            if (item != null && item.value != null )
            {
                if (other != null && other.value != null)
                {
                    if (item.position == other.position && (item.value.oid > 0 && item.value.oid.Equals(other.value.oid)))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            
            else if (item != null && item.attribute != null)
            {
                if (other != null && other.attribute != null)
                {
                    if (item.position == other.position && (item.attribute.oid > 0 && item.attribute.oid.Equals(other.attribute.oid)))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
