using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for ExcludedColumnItemPanel.xaml
    /// </summary>
    public partial class ExcludedColumnItemPanel : ScrollViewer, IChangeable
    {

        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des ScopeItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        #endregion


        #region Properties

        public AutomaticSourcingColumn Column { get; set; }

        public ColumItemPanel ActiveItemPanel { get; set; }

        #endregion


        #region Constructors

        public ExcludedColumnItemPanel()
        {
            InitializeComponent();
        }

        #endregion
        

        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void Display(AutomaticSourcingColumn column)
        {
            this.Column = column;
            this.panel.Children.Clear();
            int position = 1;
            if (column == null)
            {
                this.ActiveItemPanel = new ColumItemPanel(position);
                AddItemPanel(this.ActiveItemPanel);
                return;
            }
            foreach (AutomaticSourcingColumnItem item in column.excludedItemListChangeHandler.Items)
            {
                ColumItemPanel itemPanel = new ColumItemPanel(item);
                AddItemPanel(itemPanel);
                position++;
            }
            this.ActiveItemPanel = new ColumItemPanel(position);
            AddItemPanel(this.ActiveItemPanel);
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetValue(Object value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (ColumItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetValue(value);
        }

        protected void AddItemPanel(ColumItemPanel itemPanel)
        {
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            this.panel.Children.Add(itemPanel);
        }

        #endregion


        #region Handlers

        private void OnActivated(object item)
        {
            ColumItemPanel panel = (ColumItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            ColumItemPanel panel = (ColumItemPanel)item;
            if (this.Column == null) this.Column = new AutomaticSourcingColumn();
            this.Column.AddExcludedColumnItem(panel.Item);
            OnChanged(panel.Item);
        }

        private void OnUpdated(object item)
        {
            ColumItemPanel panel = (ColumItemPanel)item;
            if (this.Column == null) this.Column = new AutomaticSourcingColumn();
            this.Column.UpdateExcludedColumnItem(panel.Item);
            OnChanged(panel.Item);
        }

        private void OnDeleted(object item)
        {
            ColumItemPanel panel = (ColumItemPanel)item;
            if (panel.Item != null)
            {
                if (this.Column == null) this.Column = new AutomaticSourcingColumn();
                panel.Item.column = this.Column;
                this.Column.RemoveExcludedColumnItem(panel.Item);
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (ColumItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach (object pan in this.panel.Children)
                {
                    ((ColumItemPanel)pan).Position = index++;
                }
                tryToAddEmptyPanel();
                if (Changed != null) Changed();
                if (ItemDeleted != null && panel.Item != null) ItemDeleted(panel.Item);
            }
        }

        private void OnChanged(object item)
        {
            if (this.Column == null) this.Column = new AutomaticSourcingColumn();
            tryToAddEmptyPanel();
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }

        private void tryToAddEmptyPanel()
        {
            if (this.panel.Children.Count <= this.Column.excludedItemListChangeHandler.Items.Count)
            {
                int countItems = this.Column.excludedItemListChangeHandler.Items.Count + 1;
                this.ActiveItemPanel = new ColumItemPanel(countItems);
                AddItemPanel(this.ActiveItemPanel);
            }
        }
        
        #endregion


    }
}
