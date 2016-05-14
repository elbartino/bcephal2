using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class DesignDimensionLine : Persistent
    {

        public int position { get; set; }

        public PersistentListChangeHandler<LineItem> itemListChangeHandler { get; set; }

        public DesignDimensionLine()
        {
            this.itemListChangeHandler = new PersistentListChangeHandler<LineItem>();
        }

        public bool ContainsMeasure()
        {
            foreach (LineItem item in itemListChangeHandler.Items)
            {
                if (item.IsMeasure()) return true;
            }
            return false;
        }

        public bool ContainsPeriod()
        {
            foreach (LineItem item in itemListChangeHandler.Items)
            {
                if (item.IsPeriod()) return true;
            }
            return false;
        }

        /// <summary>
        /// Rajoute un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void AddLineItem(LineItem item)
        {
            item.position = itemListChangeHandler.Items.Count;
            itemListChangeHandler.AddNew(item);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateLineItem(LineItem item)
        {
            itemListChangeHandler.AddUpdated(item);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveLineItem(LineItem item)       
        {
            item.position = -1;
            itemListChangeHandler.AddDeleted(item);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetLineItem(LineItem item)
        {
            item.position = -1;
            itemListChangeHandler.forget(item);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        public int GetItemCount()
        {
            return itemListChangeHandler.Items.Count;
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is DesignDimensionLine)) return 1;
            return this.position.CompareTo(((DesignDimensionLine)obj).position);
        }

    }
}
