using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Grille : Persistent
    {

        public Grille()
        {
            this.columnListChangeHandler = new PersistentListChangeHandler<GrilleColumn>();
            this.visibleInShortcut = true;
            this.loaded = true;
        }

        public String code {get;set;}
	
	    public String name {get;set;}

        public bool report { get; set; }

        public bool reconciliation { get; set; }

        public bool allocation { get; set; }

        public bool loaded { get; set; }
		
        public bool? visibleInShortcut {get;set;}
	
        public BGroup group { get; set; }

        public int? tableOid { get; set; }

        public int? cellOid { get; set; }

        public GrilleRelationship grilleRelationship;

        public PersistentListChangeHandler<GrilleColumn> columnListChangeHandler { get; set; }

        [ScriptIgnore]
        public GrilleFilter GrilleFilter { get; set; }

        
        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Grille)) return 1;
            return this.name.CompareTo(((Grille)obj).name);
        }


        /// <summary>
        /// Rajoute un Column
        /// </summary>
        /// <param name="cell"></param>
        public void AddColumn(GrilleColumn column, bool sort = true)
        {
            column.isAdded = true;
            column.isModified = true;
            columnListChangeHandler.AddNew(column, sort);
            OnPropertyChanged("columnListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un Column
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateColumn(GrilleColumn column, bool sort = true)
        {
            column.isModified = true;
            columnListChangeHandler.AddUpdated(column, sort);
        }

        /// <summary>
        /// Retire un Column
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveColumn(GrilleColumn column, bool sort = true)
        {
            column.isModified = true;
            columnListChangeHandler.AddDeleted(column, sort);
            foreach (GrilleColumn child in columnListChangeHandler.Items)
            {
                if (child.position > column.position)
                {
                    child.position = child.position - 1;
                    child.isModified = true;
                    columnListChangeHandler.AddUpdated(child, false);
                }
            }
        }

        /// <summary>
        /// Oublier un Column
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetColumn(GrilleColumn column, bool sort = true)
        {
            columnListChangeHandler.forget(column, sort);
        }

        public GrilleColumn GetColumn(int col)
        {
            foreach (GrilleColumn column in columnListChangeHandler.Items)
            {
                if (column.position == col) return column;
            }
            return null;
        }

        public GrilleColumn GetColumn(string col)
        {
            foreach (GrilleColumn column in columnListChangeHandler.Items)
            {
                if (column.name.Equals(col)) return column;
            }
            return null;
        }

        public GrilleColumn GetColumn(string type, int oid)
        {
            foreach (GrilleColumn column in columnListChangeHandler.Items)
            {
                if (column.type.Equals(type) && column.valueOid == oid) return column;
            }
            return null;
        }



        public GrilleColumn GetRecoNbrColumn(ReconciliationContext context)
        {
            return GetColumn(ParameterType.SCOPE.ToString(), context.recoNbreAttribute.oid.Value);
        }

        public GrilleColumn GetDCColumn(ReconciliationContext context)
        {
            return GetColumn(ParameterType.SCOPE.ToString(), context.dcNbreAttribute.oid.Value);
        }

        public GrilleColumn GetAmountColumn(ReconciliationContext context)
        {
            return GetColumn(ParameterType.MEASURE.ToString(), context.amountMeasure.oid.Value);
        }

        public GrilleColumn GetAccountNbrColumn(ReconciliationContext context)
        {
            return GetColumn(ParameterType.SCOPE.ToString(), context.accountNbreAttribute.oid.Value);
        }

        public GrilleColumn GetAccountNameColumn(ReconciliationContext context)
        {
            return GetColumn(ParameterType.SCOPE.ToString(), context.accountNameAttribute.oid.Value);
        }

        public virtual bool IsReadOnly()
        {
            return this.report && !this.reconciliation;
        }

    }
}
