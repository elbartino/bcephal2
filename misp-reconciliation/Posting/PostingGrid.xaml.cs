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

namespace Misp.Reconciliation.Posting
{
    /// <summary>
    /// Interaction logic for PostingGrid.xaml
    /// </summary>
    public partial class PostingGrid : DataGrid
    {

        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;

        public PostingGrid()
        {
            InitializeComponent();
            InitializePostingGrid();
            this.SelectionChanged += onSelectionchange;
        }

        private void onSelectionchange(object sender, SelectionChangedEventArgs e)
        {
            if (ChangeHandler != null) ChangeHandler();
        }
        
        private void InitializePostingGrid()
        {            
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            this.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            this.AlternatingRowBackground = bruch;
            this.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < getColumnCount(); i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeaderAt(i);
                column.Width = getColumnWidthAt(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                this.Columns.Add(column);
            }

            this.SelectionChanged += onPostingGridSelectionChanged;
        }


        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnCount()
        {
            return 8;
        }

        /// <summary>
        /// Construit et retourne la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne à contruire</param>
        /// <returns>La colonne</returns>
        protected DataGridColumn getColumnAt(int index)
        {
            return new DataGridTextColumn();
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
                case 0: return "Posting N°";
                case 1: return "Account";
                case 2: return "Account Name";
                case 3: return "Scheme";
                case 4: return "Date";
                case 5: return "D/C";
                case 6: return "Amount";
                case 7: return "Reco N°";
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
                case 0: return 120;
                case 1: return 120;
                case 2: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 3: return 120;
                case 4: return 120;
                case 5: return 50;
                case 6: return 120;
                case 7: return 100;
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
                case 0: return "postingNumber";
                case 1: return "account";
                case 2: return "accountName";
                case 3: return "Scheme";
                case 4: return "date";
                case 5: return "dc";
                case 6: return "amount";
                case 7: return "reconciliationNumber";
                default: return "oid";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAt(int index)
        {
            return new System.Windows.Data.Binding(getBindingNameAt(index));
        }

        private void onPostingGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

    }
}
