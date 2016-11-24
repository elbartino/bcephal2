using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class GrilleRelationship : Persistent
    {

        
        public PersistentListChangeHandler<GrilleRelationshipItem> itemListChangeHandler { get; set; }

        /**
         * Constructor
         */
        public GrilleRelationship() {
            itemListChangeHandler = new PersistentListChangeHandler<GrilleRelationshipItem>();
	    }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(GrilleRelationshipItem item, bool sort = true)
        {
            item.isModified = true;
            //item.position = itemListChangeHandler.Items.Count;            
            itemListChangeHandler.AddNew(item, sort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void UpdateItem(GrilleRelationshipItem item, bool sort = true)
        {
            item.isModified = true;
            itemListChangeHandler.AddUpdated(item, sort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(GrilleRelationshipItem item, bool sort = true)
        {
            item.isModified = true;
            itemListChangeHandler.AddDeleted(item, sort);
            foreach (GrilleRelationshipItem child in itemListChangeHandler.Items)
            {
                if (child.primary == item.primary && child.position > item.position) child.position = child.position - 1;
            }
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetItem(GrilleRelationshipItem item, bool sort = true)
        {
            itemListChangeHandler.forget(item, sort);
            foreach (GrilleRelationshipItem child in itemListChangeHandler.Items)
            {
                if (child.primary == item.primary && child.position > item.position) child.position = child.position - 1;
            }
            item.position = -1;
        }


        [ScriptIgnore]
        public Grille Grid { get; set; }

        [ScriptIgnore]
        public ObservableCollection<GrilleRelationshipItem> PrimaryItems
        {
            get
            {
                ObservableCollection<GrilleRelationshipItem> items = new ObservableCollection<GrilleRelationshipItem>();
                foreach (GrilleRelationshipItem item in itemListChangeHandler.Items)
                {
                    if (item.primary) items.Add(item);
                }
                return items;
            }
        }

        [ScriptIgnore]
        public ObservableCollection<GrilleRelationshipItem> RelatedItems
        {
            get
            {
                ObservableCollection<GrilleRelationshipItem> items = new ObservableCollection<GrilleRelationshipItem>();
                foreach (GrilleRelationshipItem item in itemListChangeHandler.Items)
                {
                    if (!item.primary) items.Add(item);
                }
                return items;
            }
        }

        public bool IsPrimaryColumn(GrilleColumn column)
        {
            GrilleRelationshipItem item = GetItemByColumn(column);
            return item != null && item.primary;
        }

        public bool IsRelatedColumn(GrilleColumn column)
        {
            GrilleRelationshipItem item = GetItemByColumn(column);
            return item != null && !item.primary;
        }

        public GrilleRelationshipItem GetItemByColumn(GrilleColumn column)
        {
            foreach (GrilleRelationshipItem item in itemListChangeHandler.Items)
            {
                if (column != null && item.column != null && item.column.name.Equals(column.name)) return item;
            }
            return null;
        }
    }
}
