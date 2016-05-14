using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Reporting.Calculated_Measure
{
    /// <summary>
    /// Interaction logic for CalculatedMeasurePanel.xaml
    /// </summary>
    public partial class CalculatedMeasurePanel : ScrollViewer, IChangeable
    {


        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des CalculatedMeasureItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        
        public event SelectedItemChangedEventHandler ItemCloseParOrEqualSelected;

        #endregion


        #region Properties

        public CalculatedMeasure CalculatedMeasure { get; set; }

        public CalculatedMeasureItemPanel ActiveItemPanel { get; set; }
        public Label Label { get; set; }

        #endregion


        #region Constructors

        public CalculatedMeasurePanel()
        {
            InitializeComponent();
           
        }

        
        #endregion

        #region Operations

        /// <summary>
        /// affiche le calculatedMeasure en edition
        /// </summary>
        /// <param name="table"></param>
        public void DisplayCalculatedMeasure(CalculatedMeasure calculatedMeasure)
        {
            CalculatedMeasureItem last = calculatedMeasure.GetCalculatedMeasureItems().Count > 0 ? calculatedMeasure.GetItemByPosition(calculatedMeasure.GetCalculatedMeasureItems().Count - 1):null;
            if (last != null && last.sign != null && last.sign.Equals("="))
            {
                calculatedMeasure.RemoveItem(last);          
            }
            
            this.CalculatedMeasure = calculatedMeasure;
            this.panel.Children.Clear();
            int index = 1;
            if (calculatedMeasure == null)
            {
                this.ActiveItemPanel = new CalculatedMeasureItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                return;
            }
            foreach (CalculatedMeasureItem item in calculatedMeasure.GetCalculatedMeasureItems())
            {
                CalculatedMeasureItemPanel itemPanel = new CalculatedMeasureItemPanel(item);
                AddItemPanel(itemPanel);
                index++;
            }
           
                this.ActiveItemPanel = new CalculatedMeasureItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
           
           
        }

     

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public bool SetCalculatedMeasureItemValue(CalculatedMeasureItem value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (CalculatedMeasureItemPanel)this.panel.Children[this.panel.Children.Count - 1];
           
            this.ActiveItemPanel.SetValue(value);
            return true;
        }

      
        protected void AddItemPanel(CalculatedMeasureItemPanel itemPanel)
        {
            itemPanel.Added+= OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.CloseParOrEqualSelected += OnCloseParOrEqualSelected;
            this.panel.Children.Add(itemPanel);
        }

       
        #endregion

        #region Handlers

        private void OnCloseParOrEqualSelected(object newSelection)
        {
            if (ItemCloseParOrEqualSelected != null)
                ItemCloseParOrEqualSelected(newSelection);
            
        }


        private void OnActivated(object item)
        {
            CalculatedMeasureItemPanel panel = (CalculatedMeasureItemPanel)item;
            if (this.ActiveItemPanel != panel)
            {
                this.ActiveItemPanel = panel;
                if (ItemChanged != null && panel.CalculatedMeasureItem != null) ItemChanged(panel.CalculatedMeasureItem);
            }

        }


        private void OnAdded(object item)
        {
            CalculatedMeasureItemPanel panel = (CalculatedMeasureItemPanel)item;
            if (this.CalculatedMeasure == null) this.CalculatedMeasure = new CalculatedMeasure();
            CalculatedMeasureItem last = this.CalculatedMeasure.GetCalculatedMeasureItems().Count > 0 ? this.CalculatedMeasure.GetItemByPosition(this.CalculatedMeasure.GetCalculatedMeasureItems().Count - 1) : null;
            if (last != null && last.sign != null && last.sign.Equals("="))
            {
                this.CalculatedMeasure.RemoveItem(last);
                //string message = "Cannot add measure item after equals operator ";
                //Kernel.Util.MessageDisplayer.DisplayWarning("Add CalculatedMeasureItem", message);
                //return;
            }
            this.CalculatedMeasure.AddItem(panel.CalculatedMeasureItem);
            updated = false;
            OnChanged(panel.CalculatedMeasureItem);
        }


        bool updated = false;
        private void OnUpdated(object item)
        {
            CalculatedMeasureItemPanel panel = (CalculatedMeasureItemPanel)item;
            if (this.CalculatedMeasure == null) this.CalculatedMeasure = new CalculatedMeasure() ;
            this.CalculatedMeasure.UpdateItem(panel.CalculatedMeasureItem);
            updated = true;
            OnChanged(panel.CalculatedMeasureItem);
        }

        private void OnDeleted(object item)
        {
            CalculatedMeasureItemPanel panel = (CalculatedMeasureItemPanel)item;
            if (panel.CalculatedMeasureItem != null)
            {
                if (this.CalculatedMeasure.calculatedMeasureItemListChangeHandler.Items.Count > 1)
                {

                    if (this.CalculatedMeasure == null)
                    {
                        this.CalculatedMeasure = new CalculatedMeasure();
                        panel.CalculatedMeasureItem.calculatedMeasure = this.CalculatedMeasure;
                    }
                    
                    if (ItemDeleted != null && panel.CalculatedMeasureItem != null) ItemDeleted(panel.CalculatedMeasureItem);

                    this.panel.Children.Remove(panel);
                    if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                        this.ActiveItemPanel = (CalculatedMeasureItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                    int index = 1;
                    int j = 0;
                    for (int i = this.panel.Children.Count - 1; i >= 0; i--)
                    {
                        CalculatedMeasureItemPanel pan = this.panel.Children[j] as CalculatedMeasureItemPanel;
                        
                        /* CalculatedMeasureItem it = pan.CalculatedMeasureItem;
                        pan.Display(it);*/
                        pan.Index = index++;
                        j++;

                    }

                    if (Changed != null) Changed();
                    //if (ItemDeleted != null && panel.CalculatedMeasureItem != null) ItemDeleted(panel.CalculatedMeasureItem);

                }
                else
                {
                    string message = "Item can't be empty on Calculated Measure";
                    Kernel.Util.MessageDisplayer.DisplayError("Syntax Error", message);
                }
            }
        }

        private void OnChanged(object item)
        {
            if (this.CalculatedMeasure == null) this.CalculatedMeasure = new CalculatedMeasure();
            if ((this.panel.Children.Count <= this.CalculatedMeasure.GetCalculatedMeasureItems().Count) && !updated)
            {
                this.ActiveItemPanel = new CalculatedMeasureItemPanel(this.CalculatedMeasure.GetCalculatedMeasureItems().Count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }


        
        #endregion


    }
}
