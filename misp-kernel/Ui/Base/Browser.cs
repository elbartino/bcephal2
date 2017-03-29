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
using Misp.Kernel.Domain.Browser;
using DataGridFilterLibrary.Support;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base.BrowserUI;
using DevExpress.Xpf.Grid;

namespace Misp.Kernel.Ui.Base
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Le type d'objet manager par ce navigateur</typeparam>
    public abstract class Browser<B> : LayoutDocumentPane, IView
    {

        #region Attributes
        
        protected BrowserForm form;

        #endregion


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de Browser
        /// </summary>
        public Browser(SubjectType subjectType, String functionality) 
        {
            this.FunctionalityCode = functionality;
            this.SubjectType = subjectType;
            Datas = new ObservableCollection<B>();
            initializeGrid();
        }
        
        #endregion


        #region Properties

        public String FunctionalityCode { get; set; }

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }
        
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
        public BrowserForm Form { get { return form; } }

        public PaginationBar NavigationBar { get { return Form.PaginationBar; } }

        /// <summary>
        /// Assigne ou retourne la liste d'objets dans le navigateur.
        /// </summary>
        public ObservableCollection<B> Datas { get; set; }

        #endregion


        #region Operation

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            if (this.Form != null) this.Form.SetReadOnly(readOnly);
        }

        /// <summary>
        /// Customize for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public virtual void Customize(List<Domain.Right> rights, bool readOnly = false)
        {

        }

        public BrowserDataFilter BuildFilter(int page = 0)
        {
            BrowserDataFilter filter = new BrowserDataFilter();
            filter.page = page;            
            filter.pageSize = (int)NavigationBar.pageSizeComboBox.SelectedItem;
            if (filter.pageSize <= 0) filter.pageSize = BrowserDataFilter.DEFAULT_PAGE_SIZE;
            //foreach (FilterData data in Grid.FilterData.Datas)
            //{
            //    if (data.IsEmpty()) continue;
            //    filter.items.Add(new BrowserDataFilterItem(data.ValuePropertyBindingPath, data.QueryString, data.Operator));                
            //}
            return filter;
        }

        /// <summary>
        /// Affiche une collection d'objets dans le navigateur.
        /// </summary>
        /// <param name="datas">La collection d'objet à afficher dans le navigateur</param>
        public void DisplayPage(BrowserDataPage<B> page)
        {
            this.Form.Grid.ItemsSource = new ObservableCollection<B>();
            if (page != null)
            {
                this.Form.Grid.ItemsSource = page.rows;
                this.NavigationBar.displayPage(page.pageSize, page.pageFirstItem, page.pageLastItem, page.totalItemCount, page.pageCount, page.currentPage);
            }
            else
            {
                this.Form.Grid.ItemsSource = new ObservableCollection<B>();
                this.NavigationBar.displayPage(10, 0, 0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Affiche une collection d'objets dans le navigateur.
        /// </summary>
        /// <param name="datas">La collection d'objet à afficher dans le navigateur</param>
        public void DisplayDatas(ObservableCollection<B> datas)
        {
            Datas = datas;
            this.Form.Grid.ItemsSource = Datas;
        }

        #endregion


        #region Initializations
        
        /// <summary>
        /// Initialise la grille.
        /// </summary>
        protected virtual void initializeGrid()
        {
            form = new BrowserForm();
            for (int i = 0; i < getColumnCount(); i++)
            {
                GridColumn column = getColumn(i);
                form.Grid.Columns.Add(column);
            }

            LayoutDocument page = new LayoutDocument();
            page.CanClose = false;
            page.CanFloat = false;
            page.Title = getTitle();
            page.Content = form;
            this.Children.Add(page);            
        }


        protected virtual GridColumn getColumn(int index)
        {
            GridColumn column = new GridColumn();
            column.Header = getColumnHeaderAt(index);
            column.FieldName = getFieldNameAt(index);
            column.Width = getColumnWidthAt(index);
            column.IsSmart = true;
            column.ReadOnly = isReadOnly(index);
            column.ColumnFilterMode = ColumnFilterMode.DisplayText;
            column.Style = this.Form.Grid.FindResource("GridColumn") as Style;
            return column;
        }


        protected abstract string getFieldNameAt(int index);




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

        ///// <summary>
        ///// Construit et retourne la colonne à la position indiquée.
        ///// </summary>
        ///// <param name="index">La position de la colonne à contruire</param>
        ///// <returns>La colonne</returns>
        //protected abstract DataGridColumn getColumnAt(int index);

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
        protected abstract GridColumnWidth getColumnWidthAt(int index);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual String getBindingStringFormatAt(int index)
        {
            return null;
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
