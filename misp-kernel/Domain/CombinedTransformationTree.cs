
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Util;

namespace Misp.Kernel.Domain
{
    public class CombinedTransformationTree : Persistent
    {
        public string name { get; set; }

        public PersistentListChangeHandler<CombinedTransformationTreeItem> treeItemListChangeHandler;

        public BGroup group { get; set; }

        public bool visibleInShortcut { get; set; }

        public CombinedTransformationTree()
        {
            this.treeItemListChangeHandler = new PersistentListChangeHandler<CombinedTransformationTreeItem>();
            this.visibleInShortcut = true;
        }

        public void AddTreeItem(CombinedTransformationTreeItem item)
        {
            item.position = treeItemListChangeHandler.getItems().Count;
            treeItemListChangeHandler.AddNew(item);
            item.parent = this;
        }

        public void DeleteTreeItem(CombinedTransformationTreeItem item)
        {
            for (int i = this.treeItemListChangeHandler.Items.Count - 1; i >= 0; i--)
            {
                CombinedTransformationTreeItem titem = this.treeItemListChangeHandler.Items[i];
                if (item.position < titem.position)
                {
                    int position = titem.position;
                    titem.position = position - 1;
                    UpdateTreeItem(titem);
                }
            }
            item.position = -1;
            this.treeItemListChangeHandler.AddDeleted(item);
        }

        public void UpdateTreeItem(CombinedTransformationTreeItem item)
        {
            this.treeItemListChangeHandler.AddUpdated(item);
        }

        public void ForgetTreeItem(CombinedTransformationTreeItem item)
        {
            this.treeItemListChangeHandler.forget(item);
        }

        public List<CombinedTransformationTreeItem> GetAllItems()
        {
            List<CombinedTransformationTreeItem> trees = new List<CombinedTransformationTreeItem>(0);
            foreach (CombinedTransformationTreeItem tree in treeItemListChangeHandler.Items)
            {
                trees.Add(tree);
            }
            return trees;
        }


        public List<int> getTransformationTreesOids()
        {
            List<int> listOid = new List<int>(0);
            this.treeItemListChangeHandler.Items.BubbleSort();

            foreach (CombinedTransformationTreeItem item in this.treeItemListChangeHandler.Items)
            {
                if (item.tree != null && item.tree.oid.HasValue) listOid.Add(item.tree.oid.Value);
            }

            return listOid;
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is CombinedTransformationTree)) return 1;
            return this.name.CompareTo(((CombinedTransformationTree)obj).name);
        }

        public void RefreshItems()
        {
            //this.treeItemListChangeHandler.originalList = this.treeItemListChangeHandler.originalList;
            //foreach (CombinedTransformationTreeItem item in this.treeItemListChangeHandler.Items)
            //{
            //    item.RefreshItems();
            //}
        }
    }
}
