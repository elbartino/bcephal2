using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class LoopUserDialogTemplate : Persistent
    {
        public string message { get; set; }

        public string help { get; set; }

        public string conditions { get; set; }

        public bool active { get; set; }

        public bool onePossibleChoice { get; set; }
                
        public PersistentListChangeHandler<LoopCondition> loopConditionsChangeHandler { get; set; }

        [ScriptIgnore]
        public Instruction Instruction { get; set; }

        public LoopUserDialogTemplate()
        {
            this.loopConditionsChangeHandler = new PersistentListChangeHandler<LoopCondition>();
        }

        public override string ToString()
        {
            return this.message != null ? this.message : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is LoopUserDialogTemplate)) return 1;
            return this.message.CompareTo(((LoopUserDialogTemplate)obj).message);
        }

        public LoopCondition SynchronizeLoopCondition(LoopCondition loopCondition)
        {
            LoopCondition foundcondition = this.GetLoopCondition(loopCondition.position);
            if (foundcondition == null)
            {
                foundcondition = new LoopCondition();
                foundcondition.cellProperty = loopCondition.cellProperty;
                foundcondition.comment = loopCondition.comment;
                foundcondition.conditions = loopCondition.conditions;
                foundcondition.openBracket = loopCondition.openBracket;
                foundcondition.closeBracket = loopCondition.closeBracket;
                if (foundcondition.position != 0) foundcondition.operatorType = loopCondition.operatorType;
                AddLoopCondition(foundcondition);
            }
            else
            {
                foundcondition.cellProperty = loopCondition.cellProperty;
                foundcondition.comment = loopCondition.comment;
                foundcondition.conditions = loopCondition.conditions;
                foundcondition.openBracket = loopCondition.openBracket;
                foundcondition.closeBracket = loopCondition.closeBracket;
                if (foundcondition.position != 0) foundcondition.operatorType = loopCondition.operatorType;
                UpdateLoopCondition(foundcondition);
            }
            this.isModified = true;
            return foundcondition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddLoopCondition(LoopCondition item, bool sort = true)
        {
            item.isModified = true;
            item.position = loopConditionsChangeHandler.Items.Count;
            loopConditionsChangeHandler.AddNew(item, sort);
            OnPropertyChanged("loopConditionsChangeHandler.Items");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void UpdateLoopCondition(LoopCondition item, bool sort = true)
        {
            item.isModified = true;
            loopConditionsChangeHandler.AddUpdated(item, sort);
            OnPropertyChanged("loopConditionsChangeHandler.Items");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void RemoveLoopCondition(LoopCondition item, bool sort = true)
        {
            item.isModified = true;
            loopConditionsChangeHandler.AddDeleted(item, sort);
            foreach (LoopCondition child in loopConditionsChangeHandler.Items)
            {
                if (child.position > item.position) child.position = child.position - 1;
            }
        }

        /// <summary>
        /// Retourne l'item à la position spécifiée.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public LoopCondition GetLoopCondition(int position)
        {
            foreach (LoopCondition item in loopConditionsChangeHandler.Items)
            {
                if (item.position == position) return item;
            }
            return null;
        }

        public void SynchronizeDeleteLoopCondition(LoopCondition loopCondition)
        {
            LoopCondition foundItem = this.GetLoopCondition(loopCondition.position);
            if (foundItem == null) return;
            RemoveLoopCondition(foundItem);
            this.isModified = true;
        }

    }
}
