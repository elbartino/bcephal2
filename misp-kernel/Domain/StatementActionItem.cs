using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// This is a complete part of a TreeActionConditionItem
    /// </summary>
    public class StatementActionItem : Persistent
    {
        
        #region properties
        public string instruction { get; set; }
        public string transformationTableName { get; set; }
        public int? transformationTableOid { get; set; }
        public int position { get; set; }
        public PersistentListChangeHandler<TreeActionCondition> treeActionConditions { get; set; }
        public string logicalOperator { get; set; }
        public string openPar { get; set; }
        public string closePar { get; set; }

        public string firstValue { get; set; }
        public string secondValue { get; set; }
        public string mathOpValue { get; set; }
        public string loopName { get; set; }
        
        [ScriptIgnore]
        public TreeActionConditionItem parent { get; set; }

        public bool hasConditions 
        {
            get 
            {
                if (this.treeActionConditions.getItems().Count > 0) return true;
                return false;
            }
        }
        #endregion 

       public void refresh() 
        {
            foreach(TreeActionCondition treeActionConditions in this.treeActionConditions.getItems()) 
            {
                if (treeActionConditions.parent == null) treeActionConditions.parent = this;
                else return;
                treeActionConditions.refresh();
            }
        }

        public StatementActionItem(int position, object value):this()
        {
            this.position = position;
            this.SetValue(value);
        }

        public void SetValue(object value)
        {
            if (value == null) return;
            if (value != null && value is StatementActionItem)
            {
                var selectedStatementActionItem = (StatementActionItem)value;
                this.position = selectedStatementActionItem.position;
                this.firstValue = selectedStatementActionItem.firstValue;
                this.secondValue = selectedStatementActionItem.secondValue;
                this.mathOpValue = selectedStatementActionItem.mathOpValue;
                this.logicalOperator = selectedStatementActionItem.logicalOperator;
                this.instruction = selectedStatementActionItem.instruction;
                this.transformationTableName = selectedStatementActionItem.transformationTableName;
            } 
        }
    
        public StatementActionItem() 
        {
            treeActionConditions = new PersistentListChangeHandler<TreeActionCondition>();
        }

        public StatementActionItem(string instruction, string name) 
        {
            this.instruction = instruction;
            this.transformationTableName = name;
            treeActionConditions = new PersistentListChangeHandler<TreeActionCondition>();
        }

        public void AddTreeActionCondition(TreeActionCondition actionCondition) 
        {
            actionCondition.position = this.treeActionConditions.Items.Count;
            actionCondition.parent = this;
            this.treeActionConditions.AddNew(actionCondition);
        }

        public void RemoveTreeActionCondition(TreeActionCondition actionCondition)
        {
            actionCondition.position = -1;
            actionCondition.parent = this;
            this.treeActionConditions.AddDeleted(actionCondition);
        }

        public void UpdateTreeActionCondition(TreeActionCondition actionCondition)
        {
            actionCondition.parent = this;
            this.treeActionConditions.AddUpdated(actionCondition);
        }



        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is StatementActionItem)) return 1;
            if (this == obj) return 0;
            return this.position.CompareTo(((StatementActionItem)obj).position);
        }

        public StatementActionItem UpdateParent()
        {
            if (this.parent != null) 
            {
                this.parent.UpdateParent();
                return this;
            }
            return null;
        }
    }
}
