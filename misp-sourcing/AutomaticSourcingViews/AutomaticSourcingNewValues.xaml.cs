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
using System.Windows.Shapes;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for AutomaticSourcingNewValues.xaml
    /// </summary>
    public partial class AutomaticSourcingNewValues : Window
    {
        public bool requestUpdateUniverse;
        public bool requestCancelUpdate;
        
        public AutomaticSourcingNewValues()
        {
            InitializeComponent();
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            InitializeGridAttribute();
        }
        protected BrowserGrid grid;
        
        public void DisplayDatas(List<Misp.Kernel.Domain.Browser.NewValuesBrowserData> datas)
        {
            this.grid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<Misp.Kernel.Domain.Browser.NewValuesBrowserData>(datas);
        }

        protected void InitializeGridAttribute()
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
            
            for (int i = 0; i < getColumnAttributesCount(); i++)
            {
                DataGridColumn column = getColumnAttributesAt(i);
                column.Header = getColumnHeaderAttributesAt(i);
                column.Width = getColumnWidthAttributeAt(i);
               
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAttributeAt(i);
                }
                grid.Columns.Add(column);
            }

            this.AttributeList.Content = grid;
            this.grid.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChange);
            
        }
        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnValuesCount()
        {
            return 4;
        }


        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnAttributesCount()
        {
            return 4;
        }

        /// <summary>
        /// Construit et retourne la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne à contruire</param>
        /// <returns>La colonne</returns>
        protected DataGridColumn getColumnValuesAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridCheckBoxColumn();
                default: return new DataGridTextColumn();
            }
        }

        /// <summary>
        /// Construit et retourne la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne à contruire</param>
        /// <returns>La colonne</returns>
        protected DataGridColumn getColumnAttributesAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                case 2: return new DataGridTextColumn();
                case 3: return new DataGridTextColumn();
                default: return new DataGridTextColumn();
            }
        }

        /// <summary>
        /// Retourne l'entête de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>L'entête de la colonne</returns>
        protected string getColumnHeaderValuesAt(int index)
        {
            switch (index)
            {
                case 0: return "Values";
                case 1: return "";
                default: return "";
            }
        }

        /// <summary>
        /// Retourne l'entête de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>L'entête de la colonne</returns>
        protected string getColumnHeaderAttributesAt(int index)
        {
            switch (index)
            {
                case 0: return "Model";
                case 1: return "Entity";
                case 2: return "Attribute";
                case 3: return "Values";
                default: return "";
            }
        }

        /// <summary>
        /// Retourne la largeur de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>La largeur de colonne</returns>
        protected DataGridLength getColumnWidthValuesAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return 50;
                default: return 100;
            }
        }


        /// <summary>
        /// Retourne la largeur de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>La largeur de colonne</returns>
        protected DataGridLength getColumnWidthAttributeAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 2: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 3: return new DataGridLength(1, DataGridLengthUnitType.Star);
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingValuesAt(int index)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(getBindingNameValuesAt(index));
            string stringFormat = getStringFormatValuesAt(index);
            if (!string.IsNullOrEmpty(stringFormat)) binding.StringFormat = stringFormat;
            return binding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAttributeAt(int index)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(getBindingNameAttributeAt(index));
            string stringFormat = getStringFormatAttributeAt(index);
            if (!string.IsNullOrEmpty(stringFormat)) binding.StringFormat = stringFormat;
            return binding;
        }


        /// <summary>
        /// Retourne le nom de la propiété à rattacher à la colonne d'index spécifié.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>Le nom de la propiété à rattacher à la colonne</returns>
        protected string getBindingNameAttributeAt(int index)
        {
            switch (index)
            {
                case 0: return "nameModel";
                case 1: return "nameEntity";
                case 2: return "nameAttribute";
                case 3: return "nameValue";
                default: return "";
            }
        }

        /// <summary>
        /// Retourne le nom de la propiété à rattacher à la colonne d'index spécifié.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>Le nom de la propiété à rattacher à la colonne</returns>
        protected string getBindingNameValuesAt(int index)
        {
            switch (index)
            {
                case 0: return "name";
                case 2: return "";    
                default: return "";
            }
        }

        protected string getStringFormatValuesAt(int index)
        {
            switch (index)
            {
                case 0: return null;
                case 1: return null;
                default: return null;
            }
        }

        protected string getStringFormatAttributeAt(int index)
        {
            switch (index)
            {
                case 0: return null;
                case 1: return null;
                default: return null;
            }
        }

        protected virtual void OnSelectionChange(object sender, SelectionChangedEventArgs args)
        {
            
        }

        private void OnCancelUpdateUniverse(object sender, RoutedEventArgs e)
        {
            requestCancelUpdate = true;
            this.Close();
        }

        private void OnRequestUpdateUniverse(object sender, RoutedEventArgs e)
        {
            requestUpdateUniverse = true;
            this.Close();
        }
    }
}
