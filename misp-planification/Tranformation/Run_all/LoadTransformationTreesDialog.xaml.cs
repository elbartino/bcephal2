using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
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

namespace Misp.Planification.Tranformation.Run_all
{
    /// <summary>
    /// Interaction logic for LoadTransformationTreesDialog.xaml
    /// </summary>
    public partial class LoadTransformationTreesDialog : Window
    {
       

         /// <summary>
        /// Assigne ou retourne la liste d'objets dans le navigateur.
        /// </summary>
        public ObservableCollection<TransformationTreeBrowserData> Datas { get; set; }

        public BrowserGrid grid;



        public RunAllTransformationTreesController RunAllTransformationTreesController;


       // public string header = " table - Select Tables";

        public string runClearLabel = "";


        public Service<Kernel.Domain.TransformationTree,BrowserData> Service { get; set; }

        public LoadTransformationTreesDialog()
        {
            InitializeComponent();
            initializeGrid();
            this.Owner = ApplicationManager.Instance.MainWindow;
        }

        public void setDialogTitle(String title) {
            this.Title = title;
        }

        public void SetRunClearLabel(String operationValue) {
           // this.Title = operationValue + header;
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
                Kernel.Util.MessageDisplayer.DisplayWarning(runClearLabel+" table", "You have to select at least one tree before click "+runClearLabel.ToLower()+"!");
                return;
            }
           // this.Close();
            RunAllTransformationTreesController.Run(items);
                   
        }

        /// <summary>
        /// Affiche une collection d'objets dans le navigateur.
        /// </summary>
        /// <param name="datas">La collection d'objet à afficher dans le navigateur</param>
        public void DisplayDatas(List<TransformationTreeBrowserData> datas)
        {
            this.grid.ItemsSource = null;
            Datas = new System.Collections.ObjectModel.ObservableCollection<TransformationTreeBrowserData>(datas);                     
            this.grid.ItemsSource = Datas;
            grid.SelectAll();
            this.Show();
        }

        public void DisplayDatas(Service<Kernel.Domain.TransformationTree, BrowserData> service, int groupOid =-1 ) 
        {
            List<TransformationTreeBrowserData> datas = new List<TransformationTreeBrowserData>(0);

                this.Service = (TransformationTreeService)Service;
                if (groupOid > 0) datas = ((TransformationTreeService)Service).getTransformationTreeBrowserDatas(groupOid);
                else datas = ((TransformationTreeService)Service).getTransformationTreeBrowserDatas();

                Datas = new System.Collections.ObjectModel.ObservableCollection<TransformationTreeBrowserData>(datas);

           
            this.grid.ItemsSource = null;
            this.grid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<TransformationTreeBrowserData>(datas);
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

        public void initializeGroup(Service<Misp.Kernel.Domain.TransformationTree, BrowserData> Service)
        {
            this.Service = Service;
            Kernel.Domain.BGroup rootGroup = this.Service.GroupService.getRootGroup(Service.SubjectTypeFound());
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
            return 2;
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
                case 2: return new DataGridCheckBoxColumn();
                case 3: return new DataGridCheckBoxColumn();
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
                case 2: return "Active";
                case 3: return "Template";
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
                case 2: return "active";
                case 3: return "template";
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
