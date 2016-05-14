using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class TransformationTree : Persistent
    {
        public string name { get; set; }

        public BGroup group { get; set; }

        public string diagramXml { get; set; }

        public bool visibleInShortcut { get; set; }

        public PersistentListChangeHandler<TransformationTreeItem> itemListChangeHandler { get; set; }
        
        public TransformationTree()
        {
            this.itemListChangeHandler = new PersistentListChangeHandler<TransformationTreeItem>();
            this.visibleInShortcut = true;
        }
        
        public void AddItem(TransformationTreeItem item)
        {
            itemListChangeHandler.AddNew(item);
            item.tree = this;
        }

        public void DeleteItem(TransformationTreeItem item)
        {
            itemListChangeHandler.AddDeleted(item);
        }

        public void UpdateItem(TransformationTreeItem item)
        {
            itemListChangeHandler.AddUpdated(item);
        }

        public void ForgetItem(TransformationTreeItem item)
        {
            itemListChangeHandler.forget(item);
        }
        
        public List<TransformationTreeItem> GetAllItems()
        {
            List<TransformationTreeItem> entities = new List<TransformationTreeItem>(0);
            foreach (TransformationTreeItem entity in itemListChangeHandler.Items)
            {
                entities.Add(entity);
                entities.AddRange(entity.GetDescendentsTree());
            }
            return entities;
        }

        public ObservableCollection<TransformationTreeItem> GetAllLoops()
        {
            List<TransformationTreeItem> loops = new List<TransformationTreeItem>();
            foreach (TransformationTreeItem item in itemListChangeHandler.Items)
            {
                if (item.IsLoop) loops.Add(item);
                loops.AddRange(item.GetDescendentsLoopTree());
            }
            return new ObservableCollection<TransformationTreeItem>(loops);
        }


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is TransformationTree)) return 1;
            return this.name.CompareTo(((TransformationTree)obj).name);
        }

        public void RefreshItems()
        {
            this.itemListChangeHandler.originalList = this.itemListChangeHandler.originalList;
            foreach(TransformationTreeItem item in this.itemListChangeHandler.Items)
            {
                item.RefreshChildren();
            }
        }

        public void ReplaceItem(TransformationTreeItem child)
        {
            TransformationTreeItem found = null;
            foreach (TransformationTreeItem item in this.itemListChangeHandler.Items)
            {
                if (item.oid.HasValue && item.oid.Value == child.oid.Value)
                {
                    found = item;
                    break;
                }
            }
            if (found != null) ForgetItem(found);
            this.itemListChangeHandler.originalList.Add(child);
            this.itemListChangeHandler.Items.Add(child);
        }

        public bool hasUnsavedLoop()
        {
            foreach (TransformationTreeItem item in this.itemListChangeHandler.Items)
            {
                if(item.hasUnsavedLoop()) return true;
            }
            return false;
        }

        public bool hasUnsavedItem()
        {
            foreach (TransformationTreeItem item in this.itemListChangeHandler.Items)
            {
                if (item.hasUnsavedItem()) return true;
            }
            return false;
        }

    }
}
