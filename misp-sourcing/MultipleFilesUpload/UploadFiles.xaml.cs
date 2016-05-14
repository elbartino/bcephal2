using Misp.Kernel.Domain;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Sourcing.MultipleFilesUpload
{
    /// <summary>
    /// Interaction logic for UploadFiles.xaml
    /// </summary>
    public partial class UploadFiles : UserControl
    {
        public BrowserGrid grid;

        // <summary>
        /// Assigne ou retourne la liste d'objets dans le navigateur.
        /// </summary>
        public ObservableCollection<InputTable> Datas { get; set; }


        public UploadFiles()
        {
            InitializeComponent();
            initializeGrid();
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

            this.GridPanel.Content = grid;
        }

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Affiche une collection d'objets dans le navigateur.
        /// </summary>
        /// <param name="datas">La collection d'objet à afficher dans le navigateur</param>
        public void DisplayDatas(List<InputTable> datas)
        {
            Datas = new System.Collections.ObjectModel.ObservableCollection<InputTable>(datas);
            this.grid.ItemsSource = Datas;
            grid.SelectAll();
            
        }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected int getColumnCount()
        {
            return 1;
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
                default: return "";
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


        public void setFolderName(string path)
        {
            this.folderField.Text = path;
        }


        private void BrowserButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;

            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            var selectedPath = folderDialog.SelectedPath;
            if (result == System.Windows.Forms.DialogResult.OK)
                setFolderName(selectedPath);
        }
    }
}
