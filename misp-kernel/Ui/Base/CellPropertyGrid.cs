using Misp.Kernel.Domain;
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
    public class CellPropertyGrid : BrowserGrid
    {

        /// <summary>
        /// 
        /// </summary>
        public CellPropertyGrid()
        {
            initializeGrid();
        }

        /// <summary>
        /// Initialise la grille.
        /// </summary>
        protected void initializeGrid()
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
        }

        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected int getColumnCount()
        {
            return 6;
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
                case 3:
                    {
                        DataGridTextColumn column = new DataGridTextColumn();
                        Style style = new Style(typeof(TextBlock));
                        Trigger backgroundColorTrigger = new Trigger();
                        backgroundColorTrigger.Property = TextBlock.TextProperty;
                        backgroundColorTrigger.Value = "V31";
                        //backgroundColorTrigger.Setters.Add(
                        //    new Setter(
                        //        TextBlock.BackgroundProperty,
                        //        new SolidColorBrush(Colors.LightGreen)));
                        style.Triggers.Add(backgroundColorTrigger);

                        column.ElementStyle = style;
                        return column;
                    }
                case 4: return new DataGridTextColumn();
                case 5: return new DataGridCheckBoxColumn();
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
                case 0: return "Sheet";
                case 1: return "Cell";
                case 2: return "Scope";
                case 3: return "Period";
                case 4: return "Measure";
                case 5: return "For allocation";
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
                default: return new DataGridLength(1, DataGridLengthUnitType.Star);

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
                case 0: return "nameSheet";
                case 1: return "name";
                case 2: return "cellScope.name";
                case 3: return "period.description";
                case 4: return "cellMeasure.name";
                case 5: return "IsForAllocation";
                default: return "";
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

    }
}
