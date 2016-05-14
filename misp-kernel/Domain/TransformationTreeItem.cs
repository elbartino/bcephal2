using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class TransformationTreeItem : Persistent
    {
        public string name { get; set; }
        
        public int position { get; set; }

        public bool loop { get; set; }

        public string type { get; set; }

        public Measure ranking { get; set; }

        public bool increase { get; set; }

        public string conditions { get; set; }

        [ScriptIgnore]
        public Instruction Instruction { get; set; }
        
        [ScriptIgnore]
        public TreeActionCondition TreeActionCondition { get; set; }
        
        [ScriptIgnore]
        public  Dictionary<string, double> conditionsParams { get; set; }

        public int? reportOid { get; set; }

        public int? refreshLoopOid { get; set; }
        
        [ScriptIgnore]
        public TransformationTree tree { get; set; }

        [ScriptIgnore]
        public TransformationTreeItem parent { get; set; }

        [ScriptIgnore]
        public bool IsLoop { get { return loop; } set { loop = value; } }

        [ScriptIgnore]
        public bool IsAction { get { return !loop; } set { loop = !value; } }

        public PersistentListChangeHandler<TransformationTreeItem> childrenListChangeHandler { get; set; }

        public PersistentListChangeHandler<TransformationTreeLoopValue> valueListChangeHandler { get; set; }

        [ScriptIgnore]
        public bool IsScope { get { 
            return type != null && type.Equals(Kernel.Application.ParameterType.SCOPE.ToString());
        } }

        [ScriptIgnore]
        public bool IsPeriod
        {
            get
            {
                return type != null && type.Equals(Kernel.Application.ParameterType.PERIOD.ToString());
            }
        }

        [ScriptIgnore]
        public bool IsTag
        {
            get
            {
                return type != null && type.Equals(Kernel.Application.ParameterType.TAG.ToString());
            }
        }

      


        public TransformationTreeItem()
        {
            increase = true;
            this.valueListChangeHandler = new PersistentListChangeHandler<TransformationTreeLoopValue>();
            this.childrenListChangeHandler = new PersistentListChangeHandler<TransformationTreeItem>();            
        }

        public TransformationTreeItem(bool isLoop) : this()
        {
            IsLoop = isLoop;
        }

        /// <summary>
        /// Rajoute un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void AddValue(TransformationTreeLoopValue item)
        {
            item.position = valueListChangeHandler.Items.Count;
            valueListChangeHandler.AddNew(item);
        }

        /// <summary>
        /// Met à jour un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateValue(TransformationTreeLoopValue item)
        {
            valueListChangeHandler.AddUpdated(item);
        }

        /// <summary>
        /// Retire un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveValue(TransformationTreeLoopValue item)
        {
            item.position = -1;
            valueListChangeHandler.AddDeleted(item);
        }

        /// <summary>
        /// Oublier un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetValue(TransformationTreeLoopValue item)
        {
            item.position = -1;
            valueListChangeHandler.forget(item);
        }


        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(TransformationTreeItem child)
        {
            child.parent = this;
            childrenListChangeHandler.AddNew(child);
            UpdateParents();
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(TransformationTreeItem child)
        {
            childrenListChangeHandler.AddUpdated(child);
            UpdateParents();
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(TransformationTreeItem child)
        {
            child.parent = null;
            childrenListChangeHandler.AddDeleted(child);
            UpdateParents();
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetChild(TransformationTreeItem child)
        {
            childrenListChangeHandler.forget(child);
        }

        /// <summary>
        ///
        /// </summary>
        public void UpdateParents()
        {
            if (this.parent != null)
            {
                this.parent.childrenListChangeHandler.AddUpdated(this);
                this.parent.UpdateParents();
            }
            else if (this.tree != null)
            {
                this.tree.UpdateItem(this);
            }
        }


        public List<TransformationTreeItem> GetAscendentsTree()
        {
            List<TransformationTreeItem> entities = new List<TransformationTreeItem>(0);
            TransformationTreeItem parent = this.parent;
            while (parent != null)
            {
                entities.Add(parent);
                parent = parent.parent;
            }
            return entities;
        }

        public List<TransformationTreeItem> GetAscendentsLoopTree(bool addCurrent = false)
        {
            List<TransformationTreeItem> entities = new List<TransformationTreeItem>(0);
            if (addCurrent && this.IsLoop) entities.Add(this);
            TransformationTreeItem parent = this.parent;
            while (parent != null)
            {
                if (parent.IsLoop) entities.Insert(0, parent);
                parent = parent.parent;
            }
            return entities;
        }

        public List<TransformationTreeItem> GetDescendentsTree()
        {
            List<TransformationTreeItem> entities = new List<TransformationTreeItem>(0);
            foreach (TransformationTreeItem entity in childrenListChangeHandler.Items)
            {
                entities.Add(entity);
                entities.AddRange(entity.GetDescendentsTree());
            }
            return entities;
        }

        public List<TransformationTreeItem> GetDescendentsLoopTree()
        {
            List<TransformationTreeItem> loops = new List<TransformationTreeItem>();
            foreach (TransformationTreeItem item in childrenListChangeHandler.Items)
            {
                if (item.IsLoop) loops.Add(item);
                loops.AddRange(item.GetDescendentsLoopTree());
            }
            return loops;
        }

        
        public override String ToString()
        {
            return name;
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is TransformationTreeItem)) return 1;
            return this.position.CompareTo(((TransformationTreeItem)obj).position);
        }


        public void RefreshChildren()
        {
            this.childrenListChangeHandler.originalList = this.childrenListChangeHandler.originalList;
            foreach (TransformationTreeItem item in this.childrenListChangeHandler.Items)
            {
                item.RefreshChildren();
            }
        }

        public void ReplaceChild(TransformationTreeItem child)
        {
            TransformationTreeItem found = null;
            foreach (TransformationTreeItem item in this.childrenListChangeHandler.Items)
            {
                if (item.oid.HasValue && item.oid.Value == child.oid.Value)
                {
                    found = item;
                    break;
                }
            }
            if (found != null) ForgetChild(found);
            this.childrenListChangeHandler.originalList.Add(child);
            this.childrenListChangeHandler.Items.Add(child);
        }

        public TransformationTreeItem GetCopy(bool isCutMode = false)
        {
            TransformationTreeItem item = new TransformationTreeItem();
            item.name = isCutMode ? this.name : "Copy Of " + this.name ;
            item.position = -1;
            item.parent = null;
            item.tree = this.tree;
            item.type = this.type;
            item.ranking = this.ranking;
            item.increase = this.increase;
            item.conditions = this.conditions;
            item.reportOid = this.reportOid;
            item.loop = this.loop;
            item.valueListChangeHandler = new PersistentListChangeHandler<TransformationTreeLoopValue>();
            foreach (TransformationTreeLoopValue loopValue in this.valueListChangeHandler.Items) 
            {
                TransformationTreeLoopValue value = loopValue.GetCopy();
                value.loop = item;
                item.valueListChangeHandler.AddNew(value);
            }
            return item;
        }
   

        public TransformationTreeItem setCopyReport(TransformationTreeItem copy, Service<InputTable, InputTableBrowserData> service) 
        {
            if (!this.loop && this.reportOid.HasValue)
            {
                copy.reportOid = service.SaveAsCopy(this.reportOid.Value);
            }
            return copy; 
        }

        public bool hasUnsavedLoop()
        {
            if (this.IsLoop && !this.oid.HasValue) return true;
            foreach (TransformationTreeItem item in this.childrenListChangeHandler.Items)
            {
                if (item.hasUnsavedLoop()) return true;
            }
            return false;
        }

        public bool hasUnsavedItem()
        {
            if (!this.oid.HasValue) return true;
            foreach (TransformationTreeItem item in this.childrenListChangeHandler.Items)
            {
                if (item.hasUnsavedItem()) return true;
            }
            return false;
        }

    }
}
