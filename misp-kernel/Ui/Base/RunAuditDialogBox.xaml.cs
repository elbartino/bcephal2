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
using System.Windows.Shapes;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Task;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for RunAuditDialogBox.xaml
    /// </summary>
    public partial class RunAuditDialogBox : Window
    {
        public BrowserGrid cellsGrid;
        protected BrowserGrid itemsGrid;
        protected BrowserGrid reportOriginalCellsGrid;
        private AuditInfo auditInfo;
        private AuditInfo itemAuditInfo;
        public AuditInfo AuditInfo { get { return auditInfo; } set { auditInfo = value; UpdateCellsGrid();} }
        public AuditInfo ItemAuditInfo { get { return itemAuditInfo; } set { itemAuditInfo = value; UpdateItemsGrid(); } }
        public AuditService AuditService { get; set; }


        /// <summary>
        /// constructor
        /// </summary>
        public RunAuditDialogBox()
        {
            InitializeComponent();
            initializeCellsGrid();
            initializeItemsGrid();
            initializeHandler();
        }
        /// <summary>
        /// constructor2
        /// </summary>
        bool Fromreport;
        public RunAuditDialogBox(bool fromReport) : this()
        {
            this.Fromreport = fromReport;
            if (fromReport)
            {
                initializeCellsGrid();
                initializeReportItemsGrid();
            }           
        }
     
       
        /// <summary>
        /// initialize handlers
        /// </summary>
        private void initializeHandler()
        {
           cellsGrid.SelectionChanged += OnSelectionChange;
           NavigationbarCellsGrid.Changed += NavigationbarCellsGridChanged;
           NavigationbarItemsGrid.Changed += NavigationbarItemsGridChanged;
        }

        private String getSelectedCellName()
        {
            if (cellsGrid.SelectedItem != null)
            {
                return (cellsGrid.SelectedItem as AuditInfoItem).cellName;
            }
            if (auditInfo.items.Count > 0)
            {
                return auditInfo.items[0].cellName;
            }
            return "";
        }

        private String getSelectedTableName()
        {
            if (cellsGrid.SelectedItem != null)
            {
                return (cellsGrid.SelectedItem as AuditInfoItem).cellTableName;
            }
            if (auditInfo.items.Count > 0)
            {
                return auditInfo.items[0].cellTableName;
            }
            return "";
        }

        void NavigationbarItemsGridChanged()
        {
            Worker worker = new Worker("Loading...");
            worker.OnWork += UpdateItemAuditInfo;
            worker.StartWork(null);
        }

        void UpdateItemAuditInfo()
        {
            if(ItemAuditInfo == null)
                ItemAuditInfo = AuditService.auditLevel2(getSelectedTableName(), getSelectedCellName(), (int)NavigationbarItemsGrid.CurrentPage);
        }

        void NavigationbarCellsGridChanged()
        {
            Worker worker = new Worker("Loading...");
            worker.OnWork += UpdateAuditInfo;
            worker.StartWork(null);
        }

        void UpdateAuditInfo()
        {
            if (AuditInfo == null)
            AuditInfo = AuditService.getPage((int)NavigationbarCellsGrid.CurrentPage);
        }

        private void OnSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            /*Worker worker = new Worker("Loading...");
            worker.OnWork += SelectionChange;
            worker.StartWork(null);*/
            SelectionChange();
        }

        private void SelectionChange()
        {
            if (cellsGrid.SelectedItem == null) return;
            Kernel.Domain.AuditInfoItem selection = (cellsGrid.SelectedItem) as Kernel.Domain.AuditInfoItem;
            ItemAuditInfo = AuditService.auditLevel2(selection.cellTableName, selection.cellName);
        }

        /// <summary>
        /// Met à jour le contenu de la grille level1
        /// </summary>
       public void UpdateCellsGrid()
        {
            if (AuditInfo != null)
            {
                cellsGrid.ItemsSource = auditInfo.items;
                NavigationbarCellsGrid.UpdateBar(AuditInfo.currentPage, AuditInfo.pageCount);
                long first = (auditInfo.pageSize * (auditInfo.currentPage - 1)) + 1;
                long last = first + auditInfo.items.Count - 1;
                NavigationbarCellsGrid.SetComment(first, last, auditInfo.totalItemCount);
                if (cellsGrid.SelectedItem == null)
                {
                    if (auditInfo.items.Count > 0) { 
                         cellsGrid.SelectedItem = auditInfo.items[0];
                        //ItemAuditInfo = AuditService.auditLevel2(auditInfo.items[0].cellID);
                    }
                    else ItemAuditInfo = null;
                }
            }
            else
            {
                NavigationbarCellsGrid.UpdateBar(0, 0);
            }
        }

       /// <summary>
       /// Met à jour le contenu de la grille level2
       /// </summary>
       public void UpdateItemsGrid()
       {
           if (ItemAuditInfo != null)
           {
               
               itemsGrid.ItemsSource = ItemAuditInfo.items;
               if(this.Fromreport)
                   reportOriginalCellsGrid.ItemsSource = ItemAuditInfo.items;
               NavigationbarItemsGrid.UpdateBar(ItemAuditInfo.currentPage, ItemAuditInfo.pageCount);
               long first = (ItemAuditInfo.pageSize * (ItemAuditInfo.currentPage - 1)) + 1;
               long last = first + ItemAuditInfo.items.Count - 1;
               NavigationbarItemsGrid.SetComment(first, last, ItemAuditInfo.totalItemCount);
           }

           else
           {
               NavigationbarItemsGrid.UpdateBar(0, 0);
           }
       }
        /// <summary>
        /// Initialise la grille level1.
        /// </summary>
        protected void initializeCellsGrid()
        {
            cellsGrid = new BrowserGrid();
            cellsGrid.hideContextMenu();
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            cellsGrid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            cellsGrid.AlternatingRowBackground = bruch;
            cellsGrid.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < getColumnCount(); i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeaderAt(i);
                column.Width = getColumnWidthAt(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                cellsGrid.Columns.Add(column);
            }

            this.GridScrollPanel1.Content = cellsGrid;

            cellsGrid.SelectionChanged += OnSelectionChange;

        }

        
        

        /// <summary>
        /// Initialise la grille2.
        /// </summary>
        protected void initializeItemsGrid()
        {
            itemsGrid = new BrowserGrid();
            itemsGrid.hideContextMenu();
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            itemsGrid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            itemsGrid.AlternatingRowBackground = bruch;
            itemsGrid.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < 4; i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeader2At(i);
                column.Width = getColumnWidthAt(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBinding2At(i);
                }
                itemsGrid.Columns.Add(column);
            }

            this.GridScrollPanel2.Content = itemsGrid;

        }

        /// <summary>
        /// initialisation grille report level2
        /// </summary>
        private void initializeReportItemsGrid()
        {
            reportOriginalCellsGrid = new BrowserGrid();
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            reportOriginalCellsGrid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            reportOriginalCellsGrid.AlternatingRowBackground = bruch;
            reportOriginalCellsGrid.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < 11; i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeader3At(i);
                column.Width = getColumnWidthAt(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBinding3At(i);
                }
                reportOriginalCellsGrid.Columns.Add(column);
            }
           
            this.GridScrollPanel2.Content = reportOriginalCellsGrid ;
        }

        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnCount()
        {
            return Fromreport ? 10 : 11;
        }

        /// <summary>
        /// Construit et retourne la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne à contruire</param>
        /// <returns>La colonne</returns>
        protected DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                case 2: return new DataGridTextColumn();
                case 3: return new DataGridTextColumn();
                case 4: return new DataGridTextColumn();
                case 5: return new DataGridTextColumn();
                case 6: return new DataGridTextColumn();
                case 7: return new DataGridTextColumn();
                case 8: return new DataGridTextColumn();
                case 9: return new DataGridTextColumn();
                case 10: return new DataGridTextColumn();
                case 11: return new DataGridTextColumn();
                case 12: return new DataGridTextColumn();
                
                default: return new DataGridTextColumn();
            }
        }

        /// <summary>
        /// Retourne l'entête de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>L'entête de la colonne</returns>
        protected string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Table";
                case 1: return "Sheet";
                case 2: return "Filter";
                case 3: return "Period";
                case 4: return "Cell";
                case 5: return "Scope";
                case 6: return "Measure";
                case 7: return "Period";               
                case 8: return Fromreport ? "Allocated Amount" : "Initial Amount";
                case 9: return Fromreport ? "Remaining Amount" : "Allocated Amount";
                case 10: return "Remaining Amount";               
                default: return "";
            }            
        }

        protected string getColumnHeader2At(int index)
        {
            if(Fromreport){
                switch (index)
                {
                    case 0: return "Table";
                    case 1: return "Sheet";
                    case 2: return "Cell";
                    case 3: return "Amount";
                    default: return "";
                }
            }
            switch (index)
            {
                case 0: return "Cell";
                case 1: return "Measure";
                case 2: return "Period";
                case 3: return "Amount";                
                default: return "";
            }
        }

        /// <summary>
        /// Retourne l'entête de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>L'entête de la colonne</returns>
        protected string getColumnHeader3At(int index)
        {
            switch (index)
            {
                case 0: return "Table";
                case 1: return "Sheet";
                case 2: return "Cell";
                case 3: return "Table Filter";
                case 4: return "Table Period";
                case 5: return "Cell Filter";
                case 6: return "Cell Period";
                case 7: return "Allocation Type";
                case 8: return "Initial Amount";
                case 9: return "Allocated Amount";
                case 10: return "Report Amount";
                default: return "";
            }
        }

        /// <summary>
        /// Retourne la largeur de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>La largeur de colonne</returns>
        protected DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star); 
                case 1: return new DataGridLength(1, DataGridLengthUnitType.Star); 
                case 2: return new DataGridLength(1, DataGridLengthUnitType.Star); 
                case 3: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 4: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 5: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 6: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 7: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 8: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 9: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 10: return new DataGridLength(1, DataGridLengthUnitType.Star);
                default: return 100;
           }
        }

        /// <summary>
        /// Retourne le nom de la propiété à rattacher à la colonne d'index spécifié.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>Le nom de la propiété à rattacher à la colonne</returns>
        protected string getBindingNameAt(int index)
        {            
            switch (index)
            {
                case 0: return "cellTableName";
                case 1: return "cellSheetName";
                case 2: return "cellTableFilterName";
                case 3: return "cellTablePeriod";
                case 4: return "cellName";
                case 5: return "cellTargetName";
                case 6: return "cellMeasureName";
                case 7: return "cellPeriod";                
                case 8: return Fromreport ? "cellAllocatedAmount" : "cellInitialAmount";
                case 9: return Fromreport ? "cellRemainingAmount" : "cellAllocatedAmount";
                case 10: return "cellRemainingAmount";               
                default: return "";
            }           
        }

        protected string getBindingName2At(int index)
        {
            if (Fromreport)
            {
                switch (index)
                {
                    case 0: return "cellTableName";
                    case 1: return "cellSheetName";
                    case 2: return "cellName";
                    case 3: return "universeItemAmount";
                    default: return "";
                }
            }
            switch (index)
            {
                case 0: return "cellName";
                case 1: return "cellMeasureName";
                case 2: return "cellPeriod";
                case 3: return "universeItemAmount";
                default: return "";
            }
        }

        protected string getBindingName3At(int index)
        {
            switch (index)
            {
                case 0: return "cellTableName";
                case 1: return "cellSheetName";
                case 2: return "cellName";
                case 3: return "cellTableFilterName";
                case 4: return "cellTablePeriod";
                case 5: return "cellTargetName";
                case 6: return "cellPeriod";
                case 7: return "allocationType";
                case 8: return "cellInitialAmount";
                case 9: return "cellAllocatedAmount";
                case 10: return "cellAmountInReport";
                default: return "";
            }

        }

        protected string getStringFormatAt(int index)
        {
            switch (index)
            {
                case 0: return null;
                case 1: return null;
                case 2: return null;
                case 3: return null;
                case 4: return null;
                case 5: return null;
                case 6: return null;
                case 7: return null;
                case 8: return "{0:n2}";
                case 9: return "{0:n2}";
                case 10: return "{0:n2}";
                default: return null;
            }
        }

        protected string getStringFormat2At(int index)
        {
            switch (index)
            {
                case 0: return null;
                case 1: return null;
                case 2: return null;
                case 3: return null;
                case 4: return "{0:n2}";
                case 5: return "{0:n2}";
                default: return null;
            }
        }

        /// <summary>
        /// get appropriate field name binding at specify column index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAt(int index)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(getBindingNameAt(index));
            string stringFormat = getStringFormatAt(index);
            if (!string.IsNullOrEmpty(stringFormat)) binding.StringFormat = stringFormat;
            return binding;
        }

        /// <summary>
        /// get appropriate field name binding at specify column index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBinding2At(int index)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(getBindingName2At(index));
            string stringFormat = getStringFormatAt(index);
            if (!string.IsNullOrEmpty(stringFormat)) binding.StringFormat = stringFormat;
            return binding;
        }

        /// <summary>
        /// get appropriate field name binding at specify column index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBinding3At(int index)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(getBindingName3At(index));
            string stringFormat = getStringFormatAt(index);
            if (!string.IsNullOrEmpty(stringFormat)) binding.StringFormat = stringFormat;
            return binding;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// display de dialog 
        /// </summary>
        public void display()
        {
            this.ShowDialog();
        }

        private void first_Click(object sender, RoutedEventArgs e)
        {

        }

        private void precedent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void next_Click(object sender, RoutedEventArgs e)
        {

        }

        private void last_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
