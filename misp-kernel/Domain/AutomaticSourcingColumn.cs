using Misp.Kernel.Application;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public  class AutomaticSourcingColumn : Persistent,IComparable
    {
        public int columnIndex { get; set; }

        public BGroup targetGroup { get; set; }

        public Attribute attribute { get; set; }

        public ParameterType parameterType { get; set; }

        public Measure measure { get; set; }

        public TargetType targetType{ get; set; }

        public PersistentListChangeHandler<ColumnTargetItem> columnTargetItemListChangeHandler { get; set; }
        
        [ScriptIgnore]
        public AutomaticSourcingSheet parent { get; set; }
     
        public string tagName { get; set; }

        public string periodName { get; set; }
        
        public string dateFormat { get; set; }
              
        public CellPropertyAllocationData allocationData { get; set; }

        [ScriptIgnore]
        public int indexInListBox { get; set; }

        #region Managing Persitent  new , update, delete Lists
     
        public void setToUpdate()
        {
            this.toUpdate = true;
            this.toNew = false;
            this.toDelete = false;
            this.toForget = false;
        }

        public void setToNew()
        {
            this.toUpdate = false;
            this.toNew = true;
            this.toDelete = false;
            this.toForget = false;
        }

        public void setToDelete()
        {
            this.toDelete = true;
            this.toUpdate = false;
            this.toNew = false;
            this.toForget = false;
        }

        public void setToForget()
        {
            this.toForget = true;
            this.toDelete = false;
            this.toUpdate = false;
            this.toNew = false;
        }
        #endregion

        

        [ScriptIgnore]
        public bool toDelete { get; set; }

        [ScriptIgnore]
        public bool toUpdate { get; set; }

        [ScriptIgnore]
        public bool toNew { get; set; }

        [ScriptIgnore]
        public bool toForget { get; set; }

        [field:NonSerialized]
        private string name;

        [ScriptIgnore]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public AutomaticSourcingColumn()
        {
            this.columnTargetItemListChangeHandler = new PersistentListChangeHandler<ColumnTargetItem>();
        }

        public AutomaticSourcingColumn(int _position,string _name = "") 
        {
            this.Name = _name;
            this.columnIndex = _position;
            this.columnTargetItemListChangeHandler = new PersistentListChangeHandler<ColumnTargetItem>();
        }

        public void RestoreDefault() 
        {
            this.allocationData = null;
            this.attribute = null;
            this.measure = null;
            this.dateFormat = "";
            this.parameterType = ParameterType.SCOPE;
        }

 
        public void ChangeName(string newName)
        {
            this.Name = newName;
        }

        public ColumnTargetItem getColumnTargetItem(int position)  
        {
            List<ColumnTargetItem> liste = this.columnTargetItemListChangeHandler.Items.ToList();
            if (liste.Count == 0) return null;
            
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].columnIndex == position)
                {
                    found = true;
                    return liste[mil];
                }
                else
                {
                    if (liste[mil].columnIndex > position)
                    {
                        fin = mil - 1;
                    }
                    else
                    {
                        debut = mil + 1;
                    }
                }
            }
            while (!found && debut <= fin);
            return null;

        }

        public int getColumnTargetItemIndex(int position)
        {

            List<ColumnTargetItem> liste = this.columnTargetItemListChangeHandler.Items.ToList();
            if (liste.Count == 0) return -1;
            
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].columnIndex == position)
                {
                    found = true;
                    return mil;
                }
                else
                {
                    if (liste[mil].columnIndex > position)
                    {
                        fin = mil - 1;

                    }
                    else
                    {
                        debut = mil + 1;

                    }
                }
            }
            while (!found && debut <= fin);
            return -1;

        }

        /// <summary>
        /// Rajoute un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void AddColumnTargetItem(ColumnTargetItem columnTargetItem)
        {
            columnTargetItemListChangeHandler.AddNew(columnTargetItem);
            OnPropertyChanged("columnTargetItemListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateColumnTargetItem(ColumnTargetItem columnTargetItem)
        {
            columnTargetItemListChangeHandler.AddUpdated(columnTargetItem);
            OnPropertyChanged("columnTargetItemListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveColumnTargetItem(ColumnTargetItem columnTargetItem)
        {
            columnTargetItemListChangeHandler.AddDeleted(columnTargetItem);
            OnPropertyChanged("columnTargetItemListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetColumnTargetItem(ColumnTargetItem columnTargetItem)
        {
            columnTargetItemListChangeHandler.forget(columnTargetItem);
            OnPropertyChanged("columnTargetItemListChangeHandler.Items");
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is AutomaticSourcingColumn)) return 1;
            return this.columnIndex.CompareTo(((AutomaticSourcingColumn)obj).columnIndex);
        }
        
        public override string ToString()
        {
            return   string.IsNullOrEmpty(this.Name)? RangeUtil.GetColumnName(this.columnIndex):this.Name;
        }

        public void setParamType(object element)
        {
            this.parameterType = (Kernel.Application.ParameterType)element;
        }
    }
}
