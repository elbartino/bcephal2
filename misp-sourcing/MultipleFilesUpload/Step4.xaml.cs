using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
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

namespace Misp.Sourcing.MultipleFilesUpload
{
    /// <summary>
    /// Interaction logic for Step4.xaml
    /// </summary>
    public partial class Step4 : ScrollViewer
    {
        /// <summary>
        /// Grid
        /// </summary>
        public BrowserGrid Grid { get; set; }

        public BrowserGridContextMenu contextMenu { get; set; }

        public Step4()
        {
            InitializeComponent();
            InitializeGrid();
        }

        public void UpdateGrid(SaveInfo info)
        {
            if (info != null)
            {
                Grid.ItemsSource = info.infos;
            }
        }

        protected void InitializeGrid()
        {
            Grid = new BrowserGrid();
            contextMenu = new BrowserGridContextMenu();
            contextMenu.NewMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            contextMenu.DeleteMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            contextMenu.SaveAsMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            contextMenu.RenameMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            contextMenu.MenuSeparator.Visibility = System.Windows.Visibility.Collapsed;
            Grid.ContextMenu = contextMenu;
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            Grid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            Grid.AlternatingRowBackground = bruch;
            Grid.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < getColumnCount(); i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeaderAt(i);
                column.Width = getColumnWidthAt(i);
                column.CanUserResize = true;
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                Grid.Columns.Add(column);
            }
            this.Content = Grid;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAt(int index)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(getBindingNameAt(index));
            return binding;
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
                case 0: return "Table";
                case 1: return "File";
                case 2: return "Error";
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
                case 0: return new DataGridLength(0.2, DataGridLengthUnitType.Star); ;
                case 1: return new DataGridLength(0.4, DataGridLengthUnitType.Star); ;
                case 2: return new DataGridLength(0.4, DataGridLengthUnitType.Star);
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
                case 0: return "item";
                case 1: return "message";
                case 2: return "errorMessage";
                default: return "";
            }
        }



    }
}
