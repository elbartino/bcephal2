using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using DataGridFilterLibrary;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Kernel.Ui.Base
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Le type d'objet manager par ce navigateur</typeparam>
    public abstract class Browser<B> : LayoutDocumentPane, IView
    {

        #region Attributes

        /// <summary>
        /// La grille du navigateur
        /// </summary>
        protected BrowserGrid grid;

        #endregion


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de Browser
        /// </summary>
        public Browser() 
        {
            Datas = new ObservableCollection<B>();
            initializeGrid();
        }

        #endregion


        #region Properties
        
        /// <summary>
        /// Assigne ou retourne le ChangeEventHandler qui spécifie 
        /// la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        
        /// <summary>
        /// Assigne ou retourne la valeur indiquant
        /// qu'une modification est survenue dans le navigateur.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// Retourne la grille du navigateur
        /// </summary>
        public BrowserGrid Grid { get { return grid; } }

        /// <summary>
        /// Assigne ou retourne la liste d'objets dans le navigateur.
        /// </summary>
        public ObservableCollection<B> Datas { get; set; }

        #endregion


        #region Operation

        /// <summary>
        /// Affiche une collection d'objets dans le navigateur.
        /// </summary>
        /// <param name="datas">La collection d'objet à afficher dans le navigateur</param>
        public void DisplayDatas(ObservableCollection<B> datas)
        {
            Datas = datas;
            this.grid.ItemsSource = Datas;
        }

        #endregion


        #region Initializations
        
        /// <summary>
        /// Initialise la grille.
        /// </summary>
        protected virtual void initializeGrid()
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
                column.IsReadOnly = isReadOnly(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                grid.Columns.Add(column);
            }

            LayoutDocument page = new LayoutDocument();
            page.CanClose = false;
            page.CanFloat = false;
            page.Title = getTitle();
            page.Content = grid;
            this.Children.Add(page);
            
        }

        /// <summary>
        /// Retourne le titre du navigateur
        /// </summary>
        /// <returns>Le titre du navigateur</returns>
        protected abstract string getTitle();

        /// <summary>
        /// Retourne le nombre de colonnes à créer dans la grille
        /// </summary>
        /// <returns>Le nombre de colonnes dans la grille</returns>
        protected abstract int getColumnCount();

        /// <summary>
        /// Construit et retourne la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne à contruire</param>
        /// <returns>La colonne</returns>
        protected abstract DataGridColumn getColumnAt(int index);

        /// <summary>
        /// Retourne l'entête de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>L'entête de la colonne</returns>
        protected abstract string getColumnHeaderAt(int index);

        /// <summary>
        /// Retourne la largeur de la colonne à la position indiquée.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>La largeur de colonne</returns>
        protected abstract DataGridLength getColumnWidthAt(int index);

        /// <summary>
        /// Retourne le nom de la propiété à rattacher à la colonne d'index spécifié.
        /// </summary>
        /// <param name="index">La position de la colonne</param>
        /// <returns>Le nom de la propiété à rattacher à la colonne</returns>
        protected abstract string getBindingNameAt(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAt(int index)
        {
            return new System.Windows.Data.Binding(getBindingNameAt(index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual bool isReadOnly(int index)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        #endregion
                
    }
}
