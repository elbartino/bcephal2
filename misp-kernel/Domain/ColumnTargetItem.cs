using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    
    [Serializable]
    public class ColumnTargetItem : Persistent,IComparable
    {
        public int columnIndex { get; set; }

        public TargetItem.Operator targetOperator { get; set; }

        [field: NonSerialized]
        private string name;

        [ScriptIgnore]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public void setToUpdate()
        {
            this.toUpdate = true;
            this.toNew = false;
            this.toDelete = false;
            this.toForget = false;
        }

        public void setToNew()
        {
            this.toNew = true;
            this.toUpdate = false;
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
            this.toUpdate = false;
            this.toNew = false;
            this.toDelete = false;
            
        }

        [ScriptIgnore]
        public bool toDelete { get; set; }

        [ScriptIgnore]
        public bool toUpdate { get; set; }

        [ScriptIgnore]
        public bool toNew { get; set; }

        [ScriptIgnore]
        public bool toForget { get; set; }

        public ColumnTargetItem() { }
        public ColumnTargetItem(int index) 
        {
            this.columnIndex = index;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? RangeUtil.GetColumnName(this.columnIndex) : this.Name;
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is ColumnTargetItem)) return 1;
            return this.columnIndex.CompareTo(((ColumnTargetItem)obj).columnIndex);
        }
    }
}
