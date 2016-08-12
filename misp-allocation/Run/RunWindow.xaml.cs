using Misp.Allocation.Base;
using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
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
using Misp.Allocation.Clear;
using Misp.Kernel.Service;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;

namespace Misp.Allocation.Run
{
    /// <summary>
    /// Interaction logic for RunWindow.xaml
    /// </summary>
    public partial class RunWindow : Window
    {

        /// <summary>
        /// Assigne ou retourne la liste d'objets dans le navigateur.
        /// </summary>
        public ObservableCollection<InputTableBrowserData> Datas { get; set; }

        public BrowserGrid grid;

        public RunAllAllocationsController Controller1;

        public ClearAllocationController ClearController;

        public Controllable Controller;


        //public string header = " table - Select Tables";
        public string header 
        {
            get 
            {
                return isRun ? " table - Select Tables" : " grid/table - Select Grids/Tables";
            }
        }


        public string runClearLabel = "";

        public ClearAllocationController ClearAllocation;

        public Service<Kernel.Domain.Persistent,BrowserData> Service { get; set; }

        public RunWindow()
        {
            InitializeComponent();
            
        }

        private bool isRun { get; set; }

        public RunWindow(bool isRunAction) :this()
        {
            isRun = isRunAction;
            initializeGrid();
        }

        public void SetRunClearLabel(String operationValue) {
            this.Title = operationValue + header;
            this.RunButton.Content = operationValue;
            this.runClearLabel = operationValue;
        }

        private void selectAllButtonClick(object sender, RoutedEventArgs e)
        {
            grid.SelectAll();
            SelectionLabel.Content = "" + Datas.Count + " / " + Datas.Count;
        }

        private void unselectAllButtonClick(object sender, RoutedEventArgs e)
        {
            grid.UnselectAll();
            SelectionLabel.Content = "0 / " + Datas.Count;
        }

        private void runButtonClick(object sender, RoutedEventArgs e)
        {            
            this.Run();
        }

        
        private void closeButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = grid.SelectedItems.Count;
            SelectionLabel.Content = "" + count + " / " + Datas.Count;
        }

        private void Run()
        {
            System.Collections.IList items = grid.SelectedItems;
            if (items == null || items.Count == 0)
            {
                Kernel.Util.MessageDisplayer.DisplayWarning(runClearLabel+"table", "You have to select at least on table before click "+runClearLabel.ToLower()+"!");
                return;
            }
           // this.Close();
            if (Controller is RunAllAllocationsController) ((RunAllAllocationsController)Controller).Run(items);
            if (Controller is ClearAllocationController)
            {

                if ((Datas.Count > items.Count)
                   ||(GroupTreeview.GroupTreeview.GroupTree.SelectedItem != null && !(GroupTreeview.GroupTreeview.GroupTree.SelectedItem as Kernel.Domain.BGroup).name.Equals("All Groups") && Datas.Count == items.Count)) 
                {
                    TableActionData data = new TableActionData();
                    foreach (InputTableBrowserData table in items)
                    {
                        data.oids.Add(table.oid);
                    }
                    ((ClearAllocationController)Controller).Clear(data);
                }
                else if ((GroupTreeview.GroupTreeview.GroupTree.SelectedItem == null ||
                  (GroupTreeview.GroupTreeview.GroupTree.SelectedItem as Kernel.Domain.BGroup).name.Equals("All Groups"))
                   && items.Count == Datas.Count)
                {
                    ((ClearAllocationController)Controller).Clear();
                }
            }
            //Controller.Run(items);            
        }

        /// <summary>
        /// Affiche une collection d'objets dans le navigateur.
        /// </summary>
        /// <param name="datas">La collection d'objet à afficher dans le navigateur</param>
        public void DisplayDatas(List<InputTableBrowserData> datas)
        {
            Datas = new System.Collections.ObjectModel.ObservableCollection<InputTableBrowserData>();
            this.grid.ItemsSource = null;
            foreach(InputTableBrowserData data in datas)
            {
                if (data.active || data.isGrid)
                Datas.Add(data);
            }
            //Datas = new System.Collections.ObjectModel.ObservableCollection<InputTableBrowserData>(datas);                     
            this.grid.ItemsSource = Datas;
            grid.SelectAll();
            this.Show();
        }

        public void DisplayDatas(Service<Kernel.Domain.Persistent, BrowserData> service, int groupOid =-1 ) 
        {
            List<InputTableBrowserData> datas = new List<InputTableBrowserData>(0);

            if (Service is AllocationService)
            {
                this.Service = (AllocationService)Service;
                if (groupOid > 0) datas = ((AllocationService)Service).getTableBrowserDatas(groupOid);
                else datas = ((AllocationService)Service).getTableBrowserDatas();
            }
            if (Service is ClearAllocationService)
            {
                this.Service = (ClearAllocationService)Service;
                if (groupOid > 0) datas = ((ClearAllocationService)Service).getRunedTableBrowserDatas(groupOid);
                else datas = ((ClearAllocationService)Service).getRunnedTableBrowserDatas();
            }
            Datas = new System.Collections.ObjectModel.ObservableCollection<InputTableBrowserData>();
            foreach (InputTableBrowserData data in datas)
            {
                if(data.active || data.isGrid)
                Datas.Add(data);
            }
            this.grid.ItemsSource = null;
            this.grid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<InputTableBrowserData>(datas);
            grid.SelectAll();
            if(this.IsVisible) this.Show();
           
        }

        /// <summary>
        /// display de dialog 
        /// </summary>
        public void Display()
        {
            this.Show();
        }

        public void initializeGroup(Service<Misp.Kernel.Domain.Persistent,BrowserData> Service)
        {
            if(Service is AllocationService) this.Service = (AllocationService)Service;
            if (Service is ClearAllocationService) this.Service = (ClearAllocationService)Service;
            SubjectType subjectType = SubjectType.INPUT_TABLE;

            Kernel.Domain.BGroup rootGroup = this.Service.GroupService.getRootGroup(subjectType); 
            GroupTreeview.GroupTreeview.DisplayRoot(rootGroup);
            GroupTreeview.GroupTreeview.SelectionChanged += OnGroupSelected;
        }

        private void OnGroupSelected(object newSelection)
        {
            if (newSelection == null) return;
            Kernel.Domain.BGroup group = (Kernel.Domain.BGroup)newSelection;
            if (group.oid == null || !group.oid.HasValue) DisplayDatas(this.Service);
            else DisplayDatas(Service, group.oid.Value);
        }

        private void initializeGrid()
        {
            grid = new BrowserGrid();

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

            grid.SelectionChanged += onSelectionChanged;

            this.GridScrollPanel.Content = grid;
        }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected int getColumnCount()
        {
            return isRun ? 2 : 3;
        }

        /// <summary>
        /// Build and returns the column at index position
        /// </summary>
        /// <param name="index">The position of the column</param>
        /// <returns></returns>
        protected DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                case 2: return new DataGridTextColumn();
                case 3: return new DataGridCheckBoxColumn();
                case 4: return new DataGridCheckBoxColumn();
                default: return new DataGridTextColumn();
            }
        }


        /// <summary>
        /// Column Label
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Name";
                case 1: return "Creation Date";
                case 2: return "Type";
                case 3: return "Active";
                case 4: return "Template";
                default: return "";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return 120;
                case 2: return 70;
                case 3: return 70;
                case 4: return 70;
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string getBindingNameAt(int index)
        {
            switch (index)
            {
                case 0: return "name";
                case 1: return "creationDate";
                case 2: return "type";
                case 3: return "active";
                case 4: return "template";
                default: return "oid";
            }
        }

        // <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAt(int index)
        {
            return new System.Windows.Data.Binding(getBindingNameAt(index));
        }



    }
}
