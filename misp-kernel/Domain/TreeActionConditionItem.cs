using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// There are three main TreeActionConditionItem : 
    ///  the if TreeActionConditionItem, the then and then else which represent the 
    ///  structure of a condition
    /// </summary>
    public class TreeActionConditionItem : Persistent
    {
        #region properties
        public int position { get; set; }

        public PersistentListChangeHandler<StatementActionItem> statementActionItems { get; set; }

        public string openPar { get; set; }
        public string closePar { get; set; }

        public bool ThenStatement {get;set;}

        public bool ElseStatement {get;set;}
        
        public bool IfStatement{get;set;}

        [ScriptIgnore]
        public TreeActionCondition parent { get; set; }

#endregion
       
        public TreeActionConditionItem() 
        {
            statementActionItems = new PersistentListChangeHandler<StatementActionItem>();
        }

        public TreeActionConditionItem(int position, object value)
            : this()
        {
            this.position = position;
        }

        public void refresh() 
        {
            foreach(StatementActionItem StatementActionItem in this.statementActionItems.getItems()) 
            {
                if (StatementActionItem.parent == null) StatementActionItem.parent = this;
                else return;
                StatementActionItem.refresh();
            }
        }


        public void RemoveValue(StatementActionItem statementActionItem)
        {
            foreach (StatementActionItem item in statementActionItems.getItems())
            {
                if (item.position > statementActionItem.position)
                {
                    item.position = item.position - 1;
                }
            }
            statementActionItem.position = -1;
            this.statementActionItems.AddDeleted(statementActionItem);
        }

        public void AddValue(StatementActionItem statementActionItem)
        {
            statementActionItem.position = statementActionItems.Items.Count;
            this.statementActionItems.AddNew(statementActionItem);
        }

        public void UpdateValue(StatementActionItem statementActionItem)
        {
             this.statementActionItems.AddUpdated(statementActionItem);
        }
              

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is TreeActionConditionItem)) return 1;
            if (this == obj) return 0;
            return this.position.CompareTo(((TreeActionConditionItem)obj).position);
        }

        public void UpdateParent()
        {
            if (this.parent != null) 
            {
                this.parent.UpdateParent();
                this.parent.UpdateValue(this);
            }
        }
    }
}
