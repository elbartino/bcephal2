using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class StructuredReportColumn : Persistent
    {
        public enum Type {MEASURE, PERIOD, TAG, TARGET, LOOP, INCREMENTAL, FREE}
                
        public StructuredReportColumn()
        {
            this.itemListChangeHandler = new PersistentListChangeHandler<StructuredReportColumnItem>();
            this.show = true;
            this.isAdded = true;
        }

        [ScriptIgnore]
        public bool isAdded { get; set; }

        public String type { get; set; }

        public String name { get; set; }

        public int position { get; set; }

        public bool show { get; set; }

        public PeriodName periodName { get; set; }

        public PeriodInterval periodInterval { get; set; }
               
        public Target scope { get; set; }

        public Measure measure { get; set; }

        public String cellRef { get; set; }

        public TransformationTreeItem loop { get; set; }
    
        public String freeText { get; set; }

        public int? incrementalStart { get; set; }

        public string periodFormulaOperation { get; set; }

        public int? periodFormulaNumber { get; set; }

        public string periodFormulaGranularity { get; set; }

        public String periodFormula { get; set; }


        public PersistentListChangeHandler<StructuredReportColumnItem> itemListChangeHandler { get; set; }


        [ScriptIgnore]
        public String periodOperation
        {
            get { return !string.IsNullOrEmpty(periodFormulaOperation) ? Operation.getBySign(periodFormulaOperation).sign : Operation.PLUS.sign; }
            set { this.periodFormulaOperation = Operation.getBySign(value).name; }
        }

        [ScriptIgnore]
        public String periodGranularity
        {
            get { return !string.IsNullOrEmpty(periodFormulaGranularity) ? Granularity.getByName(periodFormulaGranularity).name : Granularity.YEAR.name; }
            set { this.periodFormulaGranularity = Granularity.getByName(value).name; }
        }


        /// <summary>
        /// Rajoute un item
        /// </summary>
        /// <param name="cell"></param>
        public void AddItem(StructuredReportColumnItem item, bool sort = true)
        {
            item.isModified = true;
            itemListChangeHandler.AddNew(item, sort);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un item
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateItem(StructuredReportColumnItem item, bool sort = true)
        {
            item.isModified = true;
            itemListChangeHandler.AddUpdated(item, sort);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un item
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveItem(StructuredReportColumnItem item, bool sort = true)
        {
            item.isModified = true;
            itemListChangeHandler.AddDeleted(item, sort);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un item
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetItem(StructuredReportColumnItem item, bool sort = true)
        {
            itemListChangeHandler.forget(item, sort);
            OnPropertyChanged("itemListChangeHandler.Items");
        }


        /// <summary>
        /// Rajoute un item
        /// </summary>
        /// <param name="cell"></param>
        public void Add(object obj)
        {
            StructuredReportColumnItem item = new StructuredReportColumnItem();
            item.SetValue(obj);
            AddItem(item, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveAll()
        {
            foreach (StructuredReportColumnItem item in itemListChangeHandler.Items.ToList()) RemoveItem(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        public void Remove(object obj)
        {
            RemoveItem((StructuredReportColumnItem)obj);
        }


        public void SetValue(object value)
        {
            if (value is Measure)
            {
                this.type = StructuredReportColumn.Type.MEASURE.ToString();
                this.measure = (Measure)value;
                this.periodName = null;
                this.periodInterval = null;
                this.scope = null;
                this.loop = null;
                this.freeText = null;
                this.incrementalStart = null;
                this.name = value.ToString();
            }
            else if (value is TransformationTreeItem)
            {
                this.type = StructuredReportColumn.Type.LOOP.ToString();
                this.measure = null;
                this.periodName = null;
                this.periodInterval = null;
                this.scope = null;
                this.loop = (TransformationTreeItem)value;
                this.freeText = null;
                this.incrementalStart = null;
                this.name = value.ToString();
            }
            else if (value is StructuredReportColumn.Type)
            {               
                this.measure = null;
                this.periodName = null;
                this.periodInterval = null;
                this.scope = null;
                this.loop = null;
                if (StructuredReportColumn.Type.INCREMENTAL == (StructuredReportColumn.Type)value)
                {
                    this.freeText = null;
                    if(!this.incrementalStart.HasValue) this.incrementalStart = 1;
                    this.type = StructuredReportColumn.Type.INCREMENTAL.ToString();
                    this.name = "N°";
                }
                else
                {
                    this.freeText = "";
                    this.incrementalStart = null;
                    this.type = StructuredReportColumn.Type.FREE.ToString();
                    this.name = "Text";
                }
            }
            else if (value is Target)
            {
                this.type = StructuredReportColumn.Type.TARGET.ToString();
                this.measure = null;
                this.periodName = null;
                this.periodInterval = null;
                this.scope = (Target)value;
                this.loop = null;
                this.freeText = null;
                this.incrementalStart = null;
                this.name = value.ToString();
            }
            else if (value is PeriodName)
            {
                this.type = StructuredReportColumn.Type.PERIOD.ToString();
                this.measure = null;
                this.periodName = (PeriodName)value;
                this.periodInterval = null;
                this.scope = null;
                this.loop = null;
                this.freeText = null;
                this.incrementalStart = null;
                this.name = ((PeriodName)value).name;
            }
            else if (value is PeriodInterval)
            {
                this.type = StructuredReportColumn.Type.PERIOD.ToString();
                this.measure = null;
                this.periodName = null;
                this.periodInterval = (PeriodInterval)value;
                this.scope = null;
                this.loop = null;
                this.freeText = null;
                this.incrementalStart = null;
                this.name = ((PeriodInterval)value).periodName.name;
            }
        }


        public bool ContainsIntemWithValueString(String name)
        {
            return GetIntemWithValueString(name) != null;
        }

        public StructuredReportColumnItem GetIntemWithValueString(String name)
        {
            foreach (StructuredReportColumnItem item in itemListChangeHandler.Items)
            {
                if (item.EqualToValueString(name)) return item;
            }
            return null;
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is StructuredReportColumn)) return 1;
            if (this.name!=null && this.name.Equals(((StructuredReportColumn)obj).name)) return 0;
            return this.position.CompareTo(((StructuredReportColumn)obj).position);
        }

    }
}
