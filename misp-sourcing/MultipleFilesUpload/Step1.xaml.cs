using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for Step1.xaml
    /// </summary>
    public partial class Step1 : Grid
    {
        /// <summary>
        /// ExcelFilesGrid
        /// </summary>
        public BrowserGrid ExcelFilesGrid { get; set; }

        public Step1()
        {
            InitializeComponent();
            InitializeExcelFilesGrid();
            InitializeHandlers();
            setSelectionTypeVisibility();
            loadDefaultRepository( UserPreferencesUtil.GetMultipleFileUploadRepository());
        }

        private void loadDefaultRepository(string repository)
        {
            if (!string.IsNullOrWhiteSpace(repository))
            {
                DirectoryInfo directory = new DirectoryInfo(repository);
                if (directory.Exists)
                {
                    this.FolderTextBox.Text = repository;
                    FileInfo[] filePaths = directory.GetFiles("*.xls*", SearchOption.AllDirectories);
                    ExcelFilesGrid.ItemsSource = new ObservableCollection<FileInfo>(filePaths);
                    ExcelFilesGrid.SelectAll();
                    this.GridPanel.Content = ExcelFilesGrid;
                    setSelectionTypeVisibility();
                }
            }

        }

        private void setSelectionTypeVisibility()
        {
            if (ExcelFilesGrid.Items != null && ExcelFilesGrid.Items.Count > 0)
            {
                selectionType.Visibility = System.Windows.Visibility.Visible;
                selectionTypeLabel.Visibility = System.Windows.Visibility.Visible;
                selectionNumberLabel.Visibility = System.Windows.Visibility.Visible;
                updateNumberLabel();
            }
            else
            {
                selectionType.Visibility = System.Windows.Visibility.Collapsed;
                selectionTypeLabel.Visibility = System.Windows.Visibility.Collapsed;
                selectionNumberLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void updateNumberLabel()
        {
            int total = ExcelFilesGrid.Items != null && ExcelFilesGrid.Items.Count > 0 ? ExcelFilesGrid.Items.Count : 0;
            int selected = ExcelFilesGrid.SelectedItems!=null && ExcelFilesGrid.SelectedItems.Count > 0 ? ExcelFilesGrid.SelectedItems.Count : 0;
            selectionNumberLabel.Content = selected + " / " + total;
        }

        public bool Validate()
        {
            if (ExcelFilesGrid.SelectedItem == null)
            {
                MessageDisplayer.DisplayWarning("Select files to upload", "You have to select at least one file to upload!");
                return false;
            }
            return true;
        }

        private void InitializeHandlers()
        {
            BrowserButton.Click += OnBrowserButtonClicked;
            selectionType.Click += OnSelectionTypeChecked;
        }

        private void OnSelectionTypeChecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)selectionType.IsChecked)
                ExcelFilesGrid.UnselectAll();
            else
                ExcelFilesGrid.SelectAll();
            updateNumberLabel();
        }

        private void OnBrowserButtonClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;

            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            var selectedPath = folderDialog.SelectedPath;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.FolderTextBox.Text = selectedPath;
                loadDefaultRepository(selectedPath);
            }
        }

      

        private void InitializeExcelFilesGrid()
        {
            ExcelFilesGrid = new BrowserGrid();
            ExcelFilesGrid.hideContextMenu();
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            ExcelFilesGrid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            ExcelFilesGrid.AlternatingRowBackground = bruch;
            ExcelFilesGrid.AlternatingRowBackground.Opacity = 0.3;

            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = "Name";
            column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            column.Binding = new System.Windows.Data.Binding("Name");
            ExcelFilesGrid.Columns.Add(column);
            ExcelFilesGrid.SelectionChanged += onExcelFilesGridSelectionChanged;

            
        }

       
        private void onExcelFilesGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateNumberLabel();
        }

       

    }
}
