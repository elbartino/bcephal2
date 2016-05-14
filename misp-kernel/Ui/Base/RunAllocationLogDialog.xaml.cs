using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
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

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for AuditAllocation.xaml
    /// </summary>
    public partial class RunAllocationLogDialog : Window
    {
        public BrowserGrid grid { get; set; }
        private AuditInfo auditInfo;
        public AuditInfo AuditInfo { get { return auditInfo; } set { auditInfo = value; UpdateCellsGrid(); } }
        
        private AllocationRunBrowserDataPage dataPage;
        public AllocationRunBrowserDataPage DataPage { 
            get { return dataPage; } 
            set 
            {
                this.dataPage = value;
                AuditInfo info = new AuditInfo();
                info.allocationRunLogInfos = new List<AllocationRunBrowserData>(0);
                if (dataPage != null && dataPage.datas != null) info.allocationRunLogInfos.AddRange(dataPage.datas);
                this.AuditInfo = info;
                UpdatePagination();
            }
        }

        private void UpdatePagination()
        {
            this.PaginationPanel.Visibility = DataPage == null || DataPage.pageCount <= 0 || DataPage.page <= 0 ? Visibility.Collapsed : Visibility.Visible;
            if (DataPage == null) return;
            this.PaginationPanel.UpdatePagination(DataPage.page, DataPage.pageCount, "Total : " + DataPage.totalCount);
        }

        public AllocationLogService Service { get; set; }

        public RunAllocationLogDialog()
        {
            InitializeComponent();
            initializeGrid();
        }

        /// <summary>
        /// Initialise la grille.
        /// </summary>
        protected void initializeGrid()
        {
            grid = new BrowserGrid();
            grid.hideContextMenu();

            grid.Sorting += OnSort;

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            grid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            grid.AlternatingRowBackground = bruch;
            grid.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < getColumnCount(); i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeaderAt(i);
                column.Width = getColumnWidthAt(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                grid.Columns.Add(column);
            }

            this.GridScrollPanel.Content = grid;

            this.grid.PreviewMouseLeftButtonDown += OnSelectionChange;

            this.PaginationPanel.GotoFirstPageButton.Click += OnGotoFirstPage;
            this.PaginationPanel.GotoPreviousPageButton.Click += OnGotoPreviousPage;
            this.PaginationPanel.GotoNextPageButton.Click += OnGotoNextPage;
            this.PaginationPanel.GotoLastPageButton.Click += OnGotoLastPage;
            
        }

        private void OnGotoLastPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.DataPage = Service.GetAllocationRunBrowserDataPage(this.DataPage.pageCount, this.DataPage.pageSize); 
        }

        private void OnGotoNextPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.DataPage = Service.GetAllocationRunBrowserDataPage(this.DataPage.page + 1, this.DataPage.pageSize); 
        }

        private void OnGotoPreviousPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.DataPage = Service.GetAllocationRunBrowserDataPage(this.DataPage.page - 1, this.DataPage.pageSize);             
        }

        private void OnGotoFirstPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.DataPage = Service.GetAllocationRunBrowserDataPage(1, this.DataPage.pageSize);            
        }

        private void OnSort(object sender, DataGridSortingEventArgs e)
        {
            var col = e.Column;
        }

        /// <summary>
        /// action a faire lorsquon selectionne une ligne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChange(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == null) return;
            var row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row != null)
            {
                var data = (AllocationRunBrowserData)grid.Items[row.GetIndex()];
                long runId = data.oid;
                DialogAllocationRun dialog = new DialogAllocationRun();
                dialog.Title = "Log Load " + data.allocationId;
                dialog.RunInfo = Service.GetAllocationRunInfo(runId, 1);
                dialog.Service = Service;
                dialog.AllocationRunOid = runId;
                dialog.MetricsInfo = Service.MetricMeasureAllocation(runId);
                dialog.CloseButton.IsEnabled = true;
                dialog.Owner = this;
                dialog.ShowDialog();
            }
        }


        



        /// <summary>
        /// Met à jour le contenu de la grille level1
        /// </summary>
        public void UpdateCellsGrid()
        {
            if (AuditInfo != null)
            {
                grid.ItemsSource = auditInfo.allocationRunLogInfos;
            }
        }
      

         /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnCount()
        {
            return 3;
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
                case 0: return "Table load N°";
                case 1: return "Date";
                case 2: return "Type";
               
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
                case 0: return "allocationId";
                case 1: return "creationDate";
                case 2: return "allocationRunType";
               
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
                
                default: return null;
            }
        }

        /// <summary>
        /// 
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

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


       
    }
}
