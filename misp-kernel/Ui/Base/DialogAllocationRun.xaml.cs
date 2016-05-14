using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DialogAllocationRun.xaml
    /// </summary>
    public partial class DialogAllocationRun : Window
    {
        //grid of first tab
        protected BrowserGrid grid;
        //grids of second tab
        protected DataGrid grid1;
        protected DataGrid grid2;
        protected DataGrid grid3;
        protected DataGrid grid4;
        
       /*TabItem listTabItem;
        TabItem metricsTabItem;*/
        protected AllocationRunInfo runInfo;
        protected MetricMeasureAllocation metricsInfo;
        public AllocationLogService Service { get; set; }
        public long AllocationRunOid { get; set; }

        public AllocationRunInfo RunInfo
        {
            get { return runInfo; }
            set
            {
                runInfo = value;
                UpdateGrid();
                UpdatePagination();
            }
        }

        public MetricMeasureAllocation MetricsInfo
        {

            get { return metricsInfo; }
            set
            {
               metricsInfo = value;
               UpdateMerticsGrid();
               
            }
        }

        /// <summary>
        /// mettre a jour la grid des metrics
        /// </summary>
        protected void UpdateMerticsGrid()
        {
            if (MetricsInfo != null)
            {
                this.nbreCellForAllocation.Content = MetricsInfo.nbrCellAllocated;
                this.allocationTime.Content = MetricsInfo.timeAllocation;
                this.numberCellWithError.Content = MetricsInfo.nbrCellWithError;
                this.nullScope.Content = MetricsInfo.nbrErrorNullScope;
                this.noMeasure.Content = MetricsInfo.nbrErrorNoMeasure;
                this.nullPeriod.Content = MetricsInfo.nbrErrorNullPeriod;
                this.notNumericValue.Content = MetricsInfo.nbrErrorNotNumericValue;
                this.grid1.ItemsSource = MetricsInfo.metricNumberOfCellPerMeasure;
                this.grid2.ItemsSource = MetricsInfo.metricAmountToAllocatePerMeasure;
                this.grid3.ItemsSource = MetricsInfo.metricAllocatedAmountPerMeasure;
                this.grid4.ItemsSource = MetricsInfo.metricRemainingAmountPerMeasure;
            }
        }
        /// <summary>
        ///  
        /// </summary>
        public DialogAllocationRun()
        {   
            InitializeComponent();
            initializeMetricsGrid();
            initializeGrid();
            initializeHandler();
            this.Owner = ApplicationManager.Instance.MainWindow;
 
        }


        /// <summary>
        /// initialize handlers
        /// </summary>
        private void initializeHandler()
        {
            allocationTabControl.SelectionChanged += onSelectTabChanged;

            this.PaginationPanel.GotoFirstPageButton.Click += OnGotoFirstPage;
            this.PaginationPanel.GotoPreviousPageButton.Click += OnGotoPreviousPage;
            this.PaginationPanel.GotoNextPageButton.Click += OnGotoNextPage;
            this.PaginationPanel.GotoLastPageButton.Click += OnGotoLastPage;
        }

        private void OnGotoLastPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.RunInfo = Service.GetAllocationRunInfo(AllocationRunOid, this.RunInfo.pageCount);
        }

        private void OnGotoNextPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.RunInfo = Service.GetAllocationRunInfo(AllocationRunOid, this.RunInfo.currentPage + 1);
        }

        private void OnGotoPreviousPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.RunInfo = Service.GetAllocationRunInfo(AllocationRunOid, this.RunInfo.currentPage - 1);
        }

        private void OnGotoFirstPage(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            this.RunInfo = Service.GetAllocationRunInfo(AllocationRunOid, 1);
        }
                
        /// <summary>
        /// executed action when selected tab changed
        /// </summary>
        ///<param name="sender"></param>
        /// <param name="e"></param>
        private void onSelectTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.metricsTabItem.IsSelected)
            {
                UpdateMerticsGrid();
            }
        }

        public DialogAllocationRun(string title) : this()
        {
            Title = title;
        }

        private void UpdatePagination()
        {
            this.PaginationPanel.Visibility = this.RunInfo == null || this.RunInfo.pageCount <= 0 || this.RunInfo.currentPage <= 0 ? Visibility.Collapsed : Visibility.Visible;
            if (this.RunInfo == null) return;
            this.PaginationPanel.UpdatePagination(this.RunInfo.currentPage, this.RunInfo.pageCount, "Total : " + this.RunInfo.totalCellCount);
        }


        /// <summary>
        /// Met à jour le contenu de la grille
        /// </summary>
        protected void UpdateGrid()
        {
            if (RunInfo != null)
            {
                if(RunInfo.infos != null && RunInfo.infos.Count > 0) grid.ItemsSource = RunInfo.infos;
                CloseButton.IsEnabled = RunInfo.runEnded;
            }
        }

        public void UpdateGrid(AllocationRunInfo info)
        {
            if (RunInfo == null)
            {
                RunInfo = info;
                return;
            }
            if (info == null) return;
            //if (info.runEnded) return;
            RunInfo.errorMessage = info.errorMessage;
            RunInfo.isError = info.isError;
            RunInfo.runedCellCount = info.runedCellCount;
            RunInfo.runEnded = info.runEnded;
            RunInfo.totalCellCount = info.totalCellCount;
            //List<CellAllocationRunInfoBrowserData> infos = new List<CellAllocationRunInfoBrowserData>(RunInfo.infos);
            //infos.AddRange(info.infos);
            //RunInfo.infos = infos;
            UpdateGrid();
            UpdatePagination();
        }


        /// <summary>
        /// Initialise la grille.
        /// </summary>
        protected void initializeGrid()
        {
            grid = new BrowserGrid();
            grid.hideContextMenu();

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
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

            this.GridPanel.Content = grid;

        }



        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnCount()
        {
            return 13;
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
        protected  string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Table";
                case 1: return "Sheet";
                case 2: return "Cell";
                case 3: return "Table Scope";
                case 4: return "Cell Scope";
                case 5: return "Measure";
                case 6: return "Cell amount";
                case 7: return "Loaded Amount";
                case 8: return "Date Tag";
                case 9: return "Attribute Tag";
                case 10: return "Cell Tag";
                case 11: return "Allocation";
                case 12: return "Error";
                default: return "";
            }
        }

       

      

        /// <summary>
        /// Retourne la largeur de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>La largeur de colonne</returns>
        protected  DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return 100;
                case 1: return 70;
                case 2: return 50;
                case 3: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 4: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 5: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 6: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 7: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 8: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 9: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 10: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 11: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 12: return new DataGridLength(1, DataGridLengthUnitType.Star);
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
                case 0: return "table";
                case 1: return "sheet";
                case 2: return "name";
                case 3: return "tableScope";
                case 4: return "cellScope";
                case 5: return "measure";
                case 6: return "cellAmount";
                case 7: return "loadedAmount";
                case 8: return "dateTag";
                case 9: return "attributeTag";
                case 10: return "cellTag";
                case 11: return "allocation";
                case 12: return "errorMessage";
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
                case 7: return "{0:n2}";
                case 8: return "{0:n2}";
                case 9: return "{0:n2}";
                case 10: return "{0:n2}";
                case 11: return null;
                case 12: return null;
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
            runInfo = null;
            this.Close();            
        }

        private void initializeMetricsGrid()
        {
            grid1 = new DataGrid(); grid2 = new DataGrid(); grid3 = new DataGrid(); grid4 = new DataGrid();

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
         
            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            grid1.AlternatingRowBackground = bruch; grid2.AlternatingRowBackground = bruch; grid3.AlternatingRowBackground = bruch; grid4.AlternatingRowBackground = bruch;
            grid1.AlternatingRowBackground.Opacity = 0.3; grid2.AlternatingRowBackground.Opacity = 0.3; grid3.AlternatingRowBackground.Opacity = 0.3; grid4.AlternatingRowBackground.Opacity = 0.3;

          
            bindColumnGrid1();
            bindColumnGrid2();
            bindColumnGrid3();
            bindColumnGrid4();

        }
        /// <summary>
        /// initialize column binding of any grid for tab2
        /// </summary>
        private void bindColumnGrid1()
        {
           

          DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "Measure Name";
            column1.Width = 430;
            column1.Binding = new System.Windows.Data.Binding("measureName");
            grid1.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Value";
            column2.Width = 450;
            column2.Binding = new System.Windows.Data.Binding("value");
            grid1.Columns.Add(column2);

            this.ScrollGrid1.Content = grid1;
        }
        private void bindColumnGrid2()
        {
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "Measure Name";
            column1.Width = 430;
            column1.Binding = new System.Windows.Data.Binding("measureName");
            grid2.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Value";
            column2.Width = 450;
            column2.Binding = new System.Windows.Data.Binding("value");
            grid2.Columns.Add(column2);

            this.ScrollGrid2.Content = grid2;
        }
        private void bindColumnGrid3()
        {
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "Measure Name";
            column1.Width = 430;
            column1.Binding = new System.Windows.Data.Binding("measureName");
            grid3.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Value";
            column2.Width = 450;
            column2.Binding = new System.Windows.Data.Binding("value");
            grid3.Columns.Add(column2);

            this.ScrollGrid3.Content = grid3;
        }
        private void bindColumnGrid4()
        {
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "Measure Name";
            column1.Width = 430;
            column1.Binding = new System.Windows.Data.Binding("measureName");
            grid4.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Value";
            column2.Width = 450;
            column2.Binding = new System.Windows.Data.Binding("value");
            grid4.Columns.Add(column2);

            this.ScrollGrid4.Content = grid4;
        }

        
    }
}
