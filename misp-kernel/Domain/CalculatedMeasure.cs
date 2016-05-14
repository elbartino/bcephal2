using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class CalculatedMeasure : Measure
    {

        public BGroup group { get; set; }

        public decimal amount { get; set; }

        public PersistentListChangeHandler<CalculatedMeasureItem> calculatedMeasureItemListChangeHandler { get; set; }

        public bool visibleInShortcut { get; set; }

        [ScriptIgnore]
        public string expression { get; set; }

        public CalculatedMeasure():base()
        {
            this.calculatedMeasureItemListChangeHandler = new PersistentListChangeHandler<CalculatedMeasureItem>();
        }

        /// <summary>
        /// retourne ses items
        /// </summary>
        public System.Collections.IList GetCalculatedMeasureItems() { return calculatedMeasureItemListChangeHandler.Items; }//return new ObservableCollection<Measure>(from measure in calculatedMeasureItemListChangeHandler.Items orderby measure.position select measure); }


        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(CalculatedMeasureItem item, bool sort = true)
        {
            item.SetPosition(calculatedMeasureItemListChangeHandler.Items.Count);
            item.SetParent(this);
            calculatedMeasureItemListChangeHandler.AddNew(item, sort);
            UpdateParents();
            OnPropertyChanged("calculatedMeasureItemListChangeHandler.Items");
        }

       

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="item"></param>
        public void UpdateItem(CalculatedMeasureItem updatedItem, bool sort = true )
        {
            calculatedMeasureItemListChangeHandler.AddUpdated(updatedItem, sort);
            UpdateParents();
            OnPropertyChanged("calculatedMeasureItemListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(CalculatedMeasureItem itemToRemove)
        {
            if (calculatedMeasureItemListChangeHandler.Items.Contains(itemToRemove))
            {
                foreach (CalculatedMeasureItem item in calculatedMeasureItemListChangeHandler.Items)
                {
                    if (item.GetPosition() > itemToRemove.GetPosition()) item.SetPosition(item.GetPosition() - 1);
                    if (!item.Equals(itemToRemove))
                    {
                        calculatedMeasureItemListChangeHandler.AddUpdated(item);
                    }
                }
                calculatedMeasureItemListChangeHandler.AddDeleted(itemToRemove);
                itemToRemove.SetPosition(-1);
                UpdateParents();
            }
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="item"></param>
        public void ForgetItem(CalculatedMeasureItem itemToForget)
        {
            foreach (CalculatedMeasureItem item in calculatedMeasureItemListChangeHandler.Items)
            {
                if (item.GetPosition() > itemToForget.GetPosition()) item.SetPosition(item.GetPosition() - 1);
            }
            itemToForget.SetPosition(-1);
            calculatedMeasureItemListChangeHandler.forget(itemToForget);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CalculatedMeasureItem GetItemByPosition(int position)
        {
            foreach (CalculatedMeasureItem item in calculatedMeasureItemListChangeHandler.Items)
            {
                if (item.GetPosition() == position) return item;
            }
            return null;
        }

        /// <summary>
        /// définit l'expression linéaire de l'operation de la mesure calculé
        /// </summary>
        /// <returns>expression</returns>
        public string computeExpression()
        {
            if(this.calculatedMeasureItemListChangeHandler!=null)
            {
               string function = "";
               foreach (CalculatedMeasureItem item in calculatedMeasureItemListChangeHandler.Items)
               {
                   function = function + item.getItemExpression();
               }
               this.expression = function;
            }
            return this.expression;
        }

    }
}
