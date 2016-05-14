using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Period : Persistent
    {

        public Period()
        {
            itemListChangeHandler = new PersistentListChangeHandler<PeriodItem>();
        }


        public string name { get; set; }

        public PersistentListChangeHandler<PeriodItem> itemListChangeHandler { get; set; }

        protected static String SEPARATOR = "-;-";

        [ScriptIgnore]
        public bool HasDefault
        {
            get 
            {
                foreach (PeriodItem item in itemListChangeHandler.getItems()) 
                {
                    if (item.name == PeriodName.DEFAULT_DATE_NAME) return true;

                }
                return false;
            }

        }

        [ScriptIgnore]
        public PeriodItem DefaultPeriodItem
        {
            get
            {
                foreach (PeriodItem item in itemListChangeHandler.getItems())
                {
                    if (item.name == PeriodName.DEFAULT_DATE_NAME) return item;

                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddPeriodItem(PeriodItem item)
        {
            item.isModified = true;
            item.position = itemListChangeHandler.Items.Count;
            item.period = this;
            itemListChangeHandler.AddNew(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePeriodItem(PeriodItem item, bool sort = true)
        {
            item.isModified = true;
            itemListChangeHandler.AddUpdated(item, sort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void RemovePeriodItem(PeriodItem item)
        {
            item.isModified = true;
            itemListChangeHandler.AddDeleted(item);
            foreach (PeriodItem child in itemListChangeHandler.Items)
            {
                if (child.position > item.position) child.position = child.position - 1;
            }
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetPeriodItem(PeriodItem item)
        {
            itemListChangeHandler.forget(item);
            foreach (PeriodItem child in itemListChangeHandler.Items)
            {
                if (child.position > item.position) child.position = child.position - 1;
            }
            item.position = -1;
        }

        /// <summary>
        /// Retourne l'item à la position spécifiée.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PeriodItem GetPeriodItem(int position)
        {
            foreach (PeriodItem item in itemListChangeHandler.Items)
            {
                if (item.position == position) return item;
            }
            return null;
        }

        public PeriodItem GetPeriodItemByName(string name)
        {
            foreach (PeriodItem item in itemListChangeHandler.Items)
            {
                if (item.name == name) return item;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public PeriodItem SynchronizePeriodItems(PeriodItem periodItem)
        {
            PeriodItem foundItem = this.GetPeriodItem(periodItem.position);
            if (foundItem == null)
            {
                foundItem = new PeriodItem();
                foundItem.name = periodItem.name;
                foundItem.value = periodItem.value;
                foundItem.loop = periodItem.loop;
                foundItem.formula = periodItem.formula;
                foundItem.comparator = periodItem.comparator;
                foundItem.closeBracket = periodItem.closeBracket;
                foundItem.openBracket = periodItem.openBracket;
                foundItem.operationGranularity = periodItem.operationGranularity;
                foundItem.operationNumber = periodItem.operationNumber;
                foundItem.operationDate = periodItem.operationDate;
                foundItem.operation = periodItem.operation;
                AddPeriodItem(foundItem);
            }
            else
            {
                foundItem.name = periodItem.name;
                foundItem.value = periodItem.value;
                foundItem.loop = periodItem.loop;
                foundItem.formula = periodItem.formula;
                foundItem.comparator = periodItem.comparator;
                foundItem.closeBracket = periodItem.closeBracket;
                foundItem.openBracket = periodItem.openBracket;
                foundItem.operationGranularity = periodItem.operationGranularity;
                foundItem.operationNumber = periodItem.operationNumber;
                foundItem.operationDate = periodItem.operationDate;
                foundItem.operation = periodItem.operation;
                UpdatePeriodItem(foundItem);
            }
            this.isModified = true;
            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public void SynchronizeDeletePeriodItem(PeriodItem item)
        {
            PeriodItem foundItem = this.GetPeriodItem(item.position);
            if (foundItem == null) return;
            RemovePeriodItem(foundItem);
            this.isModified = true;
        }


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Period)) return 1;
            return this.name.CompareTo(((Period)obj).name);
        }


        [ScriptIgnore]
        public System.Windows.Data.CollectionViewSource ItemCollectionViewSource
        {
            get
            {
                System.Windows.Data.CollectionViewSource source = new System.Windows.Data.CollectionViewSource();
                source.Source = itemListChangeHandler.Items;
                source.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("name"));
                return source;
            }
        }

        public Period GetCopy()
        {
            Period period = new Period();
            period.name = this.name;
            foreach (PeriodItem item in this.itemListChangeHandler.Items)
            {
                period.AddPeriodItem(item.GetCopy());
            }
            return period;
        }

        [ScriptIgnore]
        public String description
        {
            get
            {
                String value = "";
                String separator = "";
                foreach (PeriodItem item in this.itemListChangeHandler.Items)
                {
                    value += separator + item.description;
                    separator = Environment.NewLine;
                }
                return value;
            }
        }

        public String asString(){
    	    String value = "";
    	    value += (name != null ? name : " ");
            foreach (PeriodItem ite in this.itemListChangeHandler.Items)
            {
    		    value += SEPARATOR + ite.asString();
    	    }
    	    return value;
        }

        public void BuildName()
        {
            string text = "";
            foreach (PeriodItem item in this.itemListChangeHandler.Items)
            {
                text += string.IsNullOrEmpty(text.Trim()) ? item.name : " "+TargetItem.Operator.AND.ToString()+" " + item.name;
            }
            this.name = text;
        }

        public void BuildValue()
        {
            string text = "";
            foreach (PeriodItem item in this.itemListChangeHandler.Items)
            {
                text += string.IsNullOrEmpty(text.Trim()) ? item.value :  " "+TargetItem.Operator.AND.ToString()+" " + item.value;
            }
            this.name = text;
        }

        public Dictionary<String, List<PeriodItem>> AsDictionary()
        {
            Dictionary<String, List<PeriodItem>> dictionary = new Dictionary<string, List<PeriodItem>>(0);
            foreach (PeriodItem item in this.itemListChangeHandler.Items)
            {
                List<PeriodItem> items = new List<PeriodItem>(0);
                if (!dictionary.TryGetValue(item.name, out items))
                {
                    items = new List<PeriodItem>(0);
                    dictionary.Add(item.name, items);
                }
                items.Add(item);                
            }
            return dictionary;
        }


        public bool Contains(PeriodItem periodItem)
        {
            foreach (PeriodItem perioditem in this.itemListChangeHandler.getItems())
            {
                if (periodItem.name == periodItem.name) return true;
            }
            return false;
        }
    }
}
