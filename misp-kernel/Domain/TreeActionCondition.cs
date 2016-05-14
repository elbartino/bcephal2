using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// This class represent the Transformation Tree action set of conditions.
    /// </summary>
    public class TreeActionCondition : Persistent
    {
        string expression = "";

        public int position { get; set; }

        public StatementActionItem parent { get; set; }

        public PersistentListChangeHandler<TreeActionConditionItem> conditionItems { get; set; }

        public TreeActionCondition() 
        {
            conditionItems = new PersistentListChangeHandler<TreeActionConditionItem>();
            TreeActionConditionItem IfConditionItem = new TreeActionConditionItem();
            IfConditionItem.IfStatement = true;
            IfConditionItem.position = 0;

            TreeActionConditionItem thenConditionItem = new TreeActionConditionItem();
            thenConditionItem.ThenStatement = true;
            IfConditionItem.position = 1;

            TreeActionConditionItem ElseConditionItem = new TreeActionConditionItem();
            ElseConditionItem.ElseStatement = true;
            IfConditionItem.position = 2;

            this.AddValue(IfConditionItem);
            this.AddValue(thenConditionItem);
            this.AddValue(ElseConditionItem);

        }


        public void refresh() 
        {
            foreach(TreeActionConditionItem conditionItem in this.conditionItems.getItems()) 
            {
                if (conditionItem.parent == null) conditionItem.parent = this;
                else return;
                conditionItem.refresh();
            }
        }

        /// <summary>
        /// Rajoute un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void AddValue(TreeActionConditionItem item)
        {
            item.position = conditionItems.Items.Count;
            item.parent = this;
            conditionItems.AddNew(item,true);
        }

        public void RemoveValue(TreeActionConditionItem item)
        {
            item.position = -1;
            conditionItems.AddDeleted(item,true);
        }

        public void UpdateValue(TreeActionConditionItem item)
        {
            conditionItems.AddUpdated(item,true);
        }

       

        public string getExpression()
        {
            return this.expression; 
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is TreeActionCondition)) return 1;
            if (this == obj) return 0;
            return this.position.CompareTo(((TreeActionCondition)obj).position);
        }

        public void UpdateParent()
        {
            if (this.parent != null) 
            {
                this.parent.UpdateParent();
                this.parent.UpdateTreeActionCondition(this);            
            }
        }
    }
}
