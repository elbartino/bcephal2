using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class PostingFilter : Persistent
    {
        
        public Target filterScope { get; set; }

        public Period filterPeriod { get; set; }

        public bool creditChecked { get; set; }

        public bool debitChecked { get; set; }

        public bool includeRecoChecked { get; set; } 

         [ScriptIgnore]
        public string periodFrom { get; set; }

        [ScriptIgnore]
        public string periodTo { get; set; }
        


        /// <summary>
        /// constructor
        /// </summary>
        public PostingFilter()
        {
            filterScope = new Target();
            filterPeriod = new Period();
        }

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
        /// <summary>
        /// correct filters
        /// </summary>
        /// <returns></returns>
        /// [ScriptIgnore]
        public Target correctFilter()
        {
            if (filterScope != null)
            {
                filterScope.targetItemListChangeHandler.Items = getItems();
            }
            return filterScope;
        }

        /**
         * List of items that are currenlty in the list it is equal to:
         * original - deleted + newItems
         */
        private ObservableCollection<TargetItem> getItems()
        {
            ObservableCollection<TargetItem> result = new ObservableCollection<TargetItem>();
            if (filterScope.targetItemListChangeHandler.originalList != null)
            {
                foreach (TargetItem item in filterScope.targetItemListChangeHandler.originalList)
                {
                    bool found = false;
                    foreach (TargetItem deleted in filterScope.targetItemListChangeHandler.deletedItems)
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
            foreach (TargetItem item in filterScope.targetItemListChangeHandler.newItems)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// verify sames items
        /// </summary>
        /// <param name="item"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        /// [ScriptIgnore]
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

        public GrilleFilter ToGrilleFilter()
        {
            GrilleFilter filter = new GrilleFilter();
            filter.creditChecked = this.creditChecked;
            filter.debitChecked = this.debitChecked;
            filter.filterPeriod = this.filterPeriod;
            filter.filterScope = this.filterScope;
            return filter;
        }

        public void FromGrilleFilter(GrilleFilter filter)
        {
            this.creditChecked = filter.creditChecked;
            this.debitChecked = filter.debitChecked;
            this.filterPeriod = filter.filterPeriod;
            this.filterScope = filter.filterScope;
        }

    }
}
