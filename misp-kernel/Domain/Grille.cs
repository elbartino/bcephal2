﻿using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            this.debitChecked = false;
            this.creditChecked = false;
            this.includeRecoChecked = false;
            this.showAllRows = false;
        }

        public Target filterScope { get; set; }

        public Period filterPeriod { get; set; }

        public String code {get;set;}
	
	    public String name {get;set;}

        public bool report { get; set; }

        public bool reconciliation { get; set; }

        public bool allocation { get; set; }

        public bool loaded { get; set; }

        public bool? visibleInShortcut { get; set; }

        public bool? showAllRows { get; set; }
	
        public BGroup group { get; set; }

        public int? tableOid { get; set; }

        public int? cellOid { get; set; }

        public bool? creditChecked { get; set; }
        
        public bool? debitChecked { get; set; }
        
        public bool? includeRecoChecked { get; set; }

        public String comment { get; set; }

        public GrilleRelationship relationship { get; set; }

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
            if (column.type != ParameterType.SCOPE.ToString()) return;
            PrimaryColumnsDataSource.Add(column);
            RelatedColumnsDataSource.Add(column);
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
            PrimaryColumnsDataSource.Remove(column);
            RelatedColumnsDataSource.Remove(column);
        }

        /// <summary>
        /// Oublier un Column
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetColumn(GrilleColumn column, bool sort = true)
        {
            columnListChangeHandler.forget(column, sort);
            PrimaryColumnsDataSource.Remove(column);
            RelatedColumnsDataSource.Remove(column);
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

        public void ClearColumnFilter() 
        {
            if (this.GrilleFilter != null) this.GrilleFilter.filter = null;
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
            return null;// GetColumn(ParameterType.SCOPE.ToString(), context.recoNbreAttribute.oid.Value);
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
            return this.report || this.reconciliation;
        }


        public List<int> getPeriodColumnPositions() {
		    List<int> positions = new List<int>(0);
		    foreach (GrilleColumn col in this.columnListChangeHandler.Items) {
                if (col.type == ParameterType.PERIOD.ToString())
				    positions.Add(col.position);
		    }
		    return positions;
	    }




        ObservableCollection<GrilleColumn> primaryColumnsDataSource;
        ObservableCollection<GrilleColumn> relatedColumnsDataSource;

        [ScriptIgnore]
        public ObservableCollection<GrilleColumn> PrimaryColumnsDataSource 
        { 
            get
            {
                if (primaryColumnsDataSource == null) buildPrimaryAndRelatedColumnsDataSource();
                return primaryColumnsDataSource;
            } 
        }

        [ScriptIgnore]
        public ObservableCollection<GrilleColumn> RelatedColumnsDataSource
        {
            get
            {
                if (relatedColumnsDataSource == null) buildPrimaryAndRelatedColumnsDataSource();
                return relatedColumnsDataSource;
            }
        }

        public void buildPrimaryAndRelatedColumnsDataSource()
        {
            primaryColumnsDataSource = new ObservableCollection<GrilleColumn>();
            relatedColumnsDataSource = new ObservableCollection<GrilleColumn>();
            primaryColumnsDataSource.Clear();
            relatedColumnsDataSource.Clear();
            foreach (GrilleColumn column in columnListChangeHandler.Items)
            {
                if (column.type != ParameterType.SCOPE.ToString()) continue;
                if (this.relationship == null)
                {
                    primaryColumnsDataSource.Add(column);
                    relatedColumnsDataSource.Add(column);
                }
                else
                {
                    bool primary = this.relationship.IsPrimaryColumn(column);
                    bool related = this.relationship.IsRelatedColumn(column);
                    if (!related) primaryColumnsDataSource.Add(column);
                    if (!primary) relatedColumnsDataSource.Add(column);
                }
            }
        }

        public GrilleColumn GetColumn(GrilleColumn column)
        {
            foreach (GrilleColumn item in columnListChangeHandler.Items)
            {
                if (column != null && item.type.Equals(column.type) && item.name.Equals(column.name)) return item;
            }
            return null;
        }

        public void loadGrilleFilter()
        {
            if (GrilleFilter == null) GrilleFilter = new GrilleFilter();
            this.GrilleFilter.filterScope = filterScope;
            this.GrilleFilter.filterPeriod = filterPeriod;
        }

        public void loadFilters()
        {
            this.filterScope = null;
            this.filterPeriod = null;
            if (GrilleFilter != null)
            {
                if (GrilleFilter.filterScope != null) GrilleFilter.filterScope.targetItemListChangeHandler.resetOriginalList();
                if (GrilleFilter.filterPeriod != null) GrilleFilter.filterPeriod.itemListChangeHandler.resetOriginalList();
                this.filterScope = GrilleFilter.filterScope;
                this.filterPeriod = GrilleFilter.filterPeriod;
            }
        }
    }
}
