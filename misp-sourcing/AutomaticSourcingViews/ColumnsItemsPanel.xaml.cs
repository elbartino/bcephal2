using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Base;
using System;
using System.Collections;
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

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for ColumnsItemsPanel.xaml
    /// </summary>
    public partial class ColumnsItemsPanel : ScrollViewer
    {
        #region Properties

        private int index;

        public ColumnTargetItem ColumnTargetItem { get; set; }

        public ColumnsItemsPanel ActiveColumnsItemsPanel { get; set; }

        public ColumnsItems ActiveColumnsItems { get; set; }
        private AutomaticSourcingColumn AutomaticSourcingColumn { get; set; }

        private List<object> listInUsedObject { get; set; }

        public bool isAutomaticTarget { get; set; }
        #endregion
    
        #region Events
        
        public event OnNewColumnEventHandler OnNewColumn;
        public delegate void OnNewColumnEventHandler();

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(object item);

        public event OnRemoveColumnItemEventHandler OnRemoveColumnItem;
        public delegate void OnRemoveColumnItemEventHandler(object item);

        public event OnSelectionChangeEventHandler OnSelectColumnItem;
        public delegate void OnSelectionChangeEventHandler(object item);


        public event OnSelectionTargetChangeEventHandler OnSelectTarget;
        public delegate void OnSelectionTargetChangeEventHandler(object item, object operatorValue);
        #endregion

        #region Constructor
        public ColumnsItemsPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Handlers
        private void ActiveColumnsItems_OnRemoveColumn(object item)
        {
            if (OnRemoveColumn != null)
                OnRemoveColumn(item);
        }
       
        /// <summary>
        /// Event called when selecting a ColumnTargetItem.
        /// </summary>
        /// <param name="item"></param>
        private void itemPanel_OnSelectColumnItem(object item)
        {
            if (OnSelectColumnItem != null)
            {
                OnSelectColumnItem(((ComboBox)item).SelectedItem);
                AddColumnTargetItem(((ComboBox)item).SelectedItem);
            }
        }

        /// <summary>
        /// Event called when removing a ColumnTargetItem.
        /// </summary>
        /// <param name="item"></param>
        private void itemPanel_OnRemoveColumn(object item)
        {

            ColumnsItems panel = (ColumnsItems)item;
            if(panel.comboBoxColunm.SelectedItem != null)
            this.listInUsedObject.Add(panel.comboBoxColunm.SelectedItem);

            if (this.panel.Children.Count > 1)
            {

                this.panel.Children.Remove(panel);

                if (this.ActiveColumnsItems != null && this.ActiveColumnsItems == panel)
                {
                    this.ActiveColumnsItems = (ColumnsItems)this.panel.Children[this.panel.Children.Count - 1];
                    this.listInUsedObject.Add(this.ActiveColumnsItems.comboBoxColunm.SelectedItem);
                    FillComboBox(this.ActiveColumnsItems.comboBoxColunm, this.listInUsedObject);
                    this.ActiveColumnsItems.IsEnabled = true;
                }
                int index = 1;
                int lastPos = this.panel.Children.Count;
                foreach (object pan in this.panel.Children)
                {
                    ((ColumnsItems)pan).Index = index++;
                    if (((ColumnsItems)pan).Index == lastPos)
                    {
                        this.listInUsedObject.Add(((ColumnsItems)pan).comboBoxColunm.SelectedItem);

                        ((ColumnsItems)pan).IsEnabled = true;
                        
                    }
                }
                if (OnRemoveColumnItem != null)
                {
                    OnRemoveColumnItem(panel.comboBoxColunm.SelectedItem);
        
                }
            }
        }
        #endregion

        #region Methods

        public void customizeForAutomaticTarget()
        {
            this.isAutomaticTarget = true;
        }

        public void DisplayAutomaticSourcingColumn(List<object> listeAutomaticSourcingColumn, AutomaticSourcingColumn automaticSourcingColumn)
        {
            this.panel.Children.Clear();
            int index = 1;
            if (listeAutomaticSourcingColumn != null  && listeAutomaticSourcingColumn.Count>0)
            {
                this.ActiveColumnsItems = new ColumnsItems(index,isAutomaticTarget);
                AddItemPanel(this.ActiveColumnsItems,listeAutomaticSourcingColumn);
                return;
            }
        }
   
        /// <summary>
        /// Adding a liste of Ui ColumnsItems and fill each with an object's list.
        /// The number of Ui ColumnsItems to add depends on the list of ColumnTargetItem given.
        /// The number of Ui ColumsItems is listecolumntargetItem.count+1.
        /// </summary>
        /// <param name="listecolumntargetItem"></param>
        /// <param name="liste"></param>
        public void AddItemPanel(PersistentListChangeHandler<ColumnTargetItem> listecolumntargetItem, List<object> liste)
        {
            this.panel.Children.Clear();

            List<object> element = listecolumntargetItem.Items.Cast<object>().ToList();

            int topIndex = listecolumntargetItem.Items.Count + 1;
            for (int i = 0; i < topIndex; i++)
            {
                ColumnsItems columnItems = new ColumnsItems(i + 1,isAutomaticTarget);

                if (i < listecolumntargetItem.Items.Count)
                {
                    for (int p = liste.Count - 1; p >= 0; p--)
                    {

                        if (((AutomaticSourcingColumn)liste[p]).columnIndex == listecolumntargetItem.Items[i].columnIndex)
                        {
                            liste.RemoveAt(p);
                            break;
                        }
                    }
                }

                if (i == listecolumntargetItem.Items.Count)
                {
                    element = liste;
                    this.AddItemPanel(columnItems, liste);
                    return;
                }
                else
                {
                    List<object> currentListe = new List<object>(0);
                    currentListe.Add(listecolumntargetItem.Items[i]);
                    this.AddItemPanel(columnItems, currentListe);
                    columnItems.comboBoxColunm.SelectedItem = currentListe[0];
                   // columnItems.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Adding a new Ui ColumnsItems and fill it with liste of object.
        /// </summary>
        /// <param name="itemPanel"></param>
        /// <param name="liste"></param>
        protected void AddItemPanel(ColumnsItems itemPanel,List<object> liste)
        {
            itemPanel.OnRemoveColumnItem += itemPanel_OnRemoveColumn;
            itemPanel.OnSelectColumnItem += itemPanel_OnSelectColumnItem;
            if (itemPanel.isAutomaticTarget) itemPanel.OnSelectTarget += itemPanel_OnSelectTarget;
            if(liste.Count >= 1)
            {
                this.panel.Children.Add(itemPanel);
                listInUsedObject = liste;
                FillComboBox(itemPanel.comboBoxColunm, listInUsedObject);
            }
        }

        public void AddItemPanel(ColumnTargetItem item,int index,String colName) 
        {
           ColumnsItems col = new ColumnsItems(index);
           col.OnRemoveColumnItem += itemPanel_OnRemoveColumn;
           col.OnSelectColumnItem += itemPanel_OnSelectColumnItem;
           if (isAutomaticTarget) col.OnSelectTarget += itemPanel_OnSelectTarget;
           this.panel.Children.Add(col);
           col.comboBoxColunm.SelectedItem = colName;
        }

        private void itemPanel_OnSelectTarget(object item, object operatorValue)
        {
            if (OnSelectTarget != null)
            {
                OnSelectTarget(item, operatorValue);
            }
            AddColumnTargetItem(item);
        }
     
        /// <summary>
        /// Add a new Ui element while adding a new ColumnTargetItem.
        /// </summary>
        /// <param name="colonnetargetitem"></param>
        private void AddColumnTargetItem(object colonnetargetitem) 
        {
            index = this.panel.Children.Count;
            this.ActiveColumnsItems = new ColumnsItems(index + 1,isAutomaticTarget);
            this.listInUsedObject=this.listInUsedObject.Distinct().ToList();
            for (int i = this.listInUsedObject.Count - 1; i >= 0; i--)
            {
                
                if (this.listInUsedObject[i] is AutomaticSourcingColumn)
                {
                    var columnTarget = this.listInUsedObject[i] as AutomaticSourcingColumn;
                    if (columnTarget.columnIndex == ((AutomaticSourcingColumn)colonnetargetitem).columnIndex)
                    {
                        this.listInUsedObject.RemoveAt(i);
                        break;
                    }
                }
                else if (this.listInUsedObject[i] is ColumnTargetItem)
                {
                    var columnTarget = this.listInUsedObject[i] as ColumnTargetItem;
                    if (columnTarget.columnIndex == ((ColumnTargetItem)colonnetargetitem).columnIndex)
                    {
                        this.listInUsedObject.RemoveAt(i);
                        break;
                    }
                }
            }
            AddItemPanel(this.ActiveColumnsItems, this.listInUsedObject);
            for (int i = 0; i < panel.Children.Count - 1; i++)
            {
                ColumnsItems colItems = panel.Children[i] as ColumnsItems;
                colItems.IsEnabled = false;
            }
        
        }

        /// <summary>
        /// Filling a combobox with a list of object.
        /// </summary>
        /// <param name="combobox"></param>
        /// <param name="list"></param>
        private void FillComboBox(ComboBox combobox, List<object> list) 
        {
                combobox.ItemsSource = list;
        }

        private void OnChanged(object item)
        {

        }
        #endregion


    }
}
