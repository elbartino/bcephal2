using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web.Script.Serialization;
using Misp.Kernel.Util;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class PersistentListChangeHandler<P> where P : Persistent, INotifyPropertyChanged
    {
        [ScriptIgnore]
        List<P> _originalList;

        /** List to manage */
        public List<P> originalList 
        {
            get { return this._originalList; }
            set { 
                this._originalList = value;
                Items.Clear();
                reset();
                if (this._originalList != null)
                {
                    foreach (P item in this._originalList)
                    {
                        Items.Add(item);
                    }
                }
                Items.BubbleSort();
            }
        }

        /** List of new items */
        public List<P> newItems { get; set; }

        /** List of deleted items */
        public List<P> deletedItems { get; set; }

        /** List of updated items */
        public List<P> updatedItems { get; set; }

        /**
         * Initialize with an original list
         *
         * @param list original list
         */
        public PersistentListChangeHandler(List<P> list)
        {
            reset();
            Items = new ObservableCollection<P>();
            originalList = list;            
        }

        /**
         * Initialize with no original list
         *
         */
        public PersistentListChangeHandler()
        {
            reset();
            Items = new ObservableCollection<P>();
            originalList = new List<P>();
        }
        
        [ScriptIgnore]
        public ObservableCollection<P> Items { get; set; }

        /**
         * List of items that are currenlty in the list it is equal to:
         * original - deleted + newItems
         */

        public ObservableCollection<P> getItems()
        {
            ObservableCollection<P> result = new ObservableCollection<P>();        
            if (originalList != null){
                foreach (P item in originalList) {
                    bool found = false;
                    foreach (P deleted in deletedItems) {
                        if (sameItems(item, deleted)){
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        result.Add(item);
                    }
                    //else
                    //    Items.Remove(item);
                }
            }
            //result.AddRange(newItems);
            foreach (P item in newItems)
            {
                result.Add(item);
            }
            return result;
        }


        /**
         * Add an item to the list of deleted items
         *
         * @param deleted Deleted item
         *
         * @pre deleted must be in getItems
         */
        public void AddDeleted(P deleted, bool sort = true){
            // if item is new we just remove it from the list of new and there is no
            // need to keep it in the list of deleted
            bool found = false;
            foreach (P item in newItems)
            {
                if (sameItems(item, deleted)){
                    found = true;
                    break;
                }
            }


            if (!found)
            {
                deletedItems.Add(deleted);
                updatedItems.Remove(deleted);
            }
            else
            {
                deletedItems.Add(deleted);
                newItems.Remove(deleted);
            }

            Items.Remove(deleted);
            if(sort) Items.BubbleSort();
            Notify("Items");
            //deleted.setPersistentState(PersistentState.DELETED);
        }

        /**
         * Add an item to the list of new items
         *
         * @param newItem New item
         *
         * @pre newItem must be in getItems
         */
        public void AddNew(P newItem, bool sort = true)
        {
            newItems.Add(newItem);
            Items.Add(newItem);
            if(sort) Items.BubbleSort();
            Notify("Items");
            //newItem.setPersistentState(PersistentState.NEW);            
        }

        public void AddNew(IEnumerable<P> items, bool sort = true)
        {
            newItems.AddRange(items);
            foreach (var item in items) Items.Add(item);
            if(sort) Items.BubbleSort();
            Notify("Items");
        }

        /**
         * Add an item to the list of updated items
         *
         * @param updated Updated item
         *
         * @pre updated must be in getItems
         */
        public void AddUpdated(P updated, bool sort = true){
            // if item is new we just do nothing as it will any how be persisted
            // with the list of new items
            bool found = false;
            foreach (P item in newItems) {
                if (sameItems(item, updated)){
                    found = true;
                    break;
                }
            }
            // if item is was alway updated we just do nothing
            foreach (P item in updatedItems)
            {
                if (sameItems(item, updated)){
                    found = true;
                    break;
                }
            }
            if (!found){
                updatedItems.Add(updated);
                //updated.setPersistentState(PersistentState.UPDATED);
            }
         //   if (!Items.Contains(updated)) Items.Add(updated);
            if(sort) Items.BubbleSort();
            Notify("Items");
        }

        /**
         * Remove item for list of new, deleted, or updated
         * @param item item to forget
         */
        public void forget(P item, bool sort = true)
        {
            bool found = newItems.Remove(item);
            if (!found)
            {
                found = deletedItems.Remove(item);
            }
            if (!found)
            {
                found = updatedItems.Remove(item);
            }
        /*    if (!found)
            {
                found = originalList.Remove(item);
            }*/
            Items.Remove(item);
            if(sort) Items.BubbleSort();
            Notify("Items");
        }

        /**
         * Do item and other have the same id?
         * or do other and item refer to the same object?
         * @param item
         * @param other
         * @return True if both ids are non null and are equal
         */
        
        private bool sameItems(P item, P other)
        {
            bool result = false;
            if (item != null)
            {
                if (other != null)
                {
                    if (item.Equals(other))
                    {
                        result = true;
                    }
                    else
                    {
                        if (item.oid > 0 && item.oid.Equals(other.oid))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        /**
         * Set all internal lists to empty lists
         */
        private void reset()
        {
            newItems = new List<P>();
            deletedItems = new List<P>();
            updatedItems = new List<P>();
        }

        /**
         * Set all internal lists to empty lists
         */
        public void resetOriginalList()
        {
            reset();
            originalList = new List<P>(Items);
        }



        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
