using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class StructuredReport : Persistent
    {

        public StructuredReport()
        {
            this.columnListChangeHandler = new PersistentListChangeHandler<StructuredReportColumn>();
            this.visibleInShortcut = true;
        }

        public BGroup group { get; set; }

        public String name { get; set; }

        public bool visibleInShortcut { get; set; }
        
        public PersistentListChangeHandler<StructuredReportColumn> columnListChangeHandler { get; set; }


        /// <summary>
        /// Rajoute un Column
        /// </summary>
        /// <param name="cell"></param>
        public void AddColumn(StructuredReportColumn column, bool sort = true)
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
        public void UpdateColumn(StructuredReportColumn column, bool sort = true)
        {
            column.isModified = true;
            columnListChangeHandler.AddUpdated(column, sort);
        }
        
        /// <summary>
        /// Retire un Column
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveColumn(StructuredReportColumn column, bool sort = true)
        {
            column.isModified = true;
            columnListChangeHandler.AddDeleted(column, sort);
            foreach (StructuredReportColumn child in columnListChangeHandler.Items)
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
        public void ForgetColumn(StructuredReportColumn column, bool sort = true)
        {
            columnListChangeHandler.forget(column, sort);
        }


        public StructuredReportColumn GetColumn(int col)
        {
            foreach (StructuredReportColumn column in columnListChangeHandler.Items)
            {
                if (column.position == col) return column;
            }
            return null;
        }


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is StructuredReport)) return 1;
            return this.name.CompareTo(((StructuredReport)obj).name);
        }

    }
}
