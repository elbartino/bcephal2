using Misp.Kernel.Domain;
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

namespace Misp.Sourcing.GridViews
{
    /// <summary>
    /// Interaction logic for GrilleBrowserForm.xaml
    /// </summary>
    public partial class GrilleBrowserForm : Grid, IEditableView<Grille>
    {
        
        protected bool throwHandler = true;

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// Design en édition
        /// </summary>
        public Grille EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        public Kernel.Service.DesignService DesignService { get; set; }

        public Misp.Sourcing.GridViews.GridBrowser.EditCellEventHandler EditEventHandler { get; set; }

        public GrilleFilterForm filterForm { get; private set; }


        public GrilleBrowserForm()
        {
            InitializeComponent();
            filterForm = new GrilleFilterForm();
            filterForm.periodFilter.DisplayPeriod(null);
            gridBrowser.SortEventHandler += OnSort;
            gridBrowser.EditEventHandler += OnEdit;
        }

        private void OnSort(object col)
        {
            GrilleColumn column = (GrilleColumn)col;
            this.filterForm.GrilleFilter.orderByColumn = column;
            this.filterForm.GrilleFilter.page = 1;
            //this.filterForm.GrilleFilter.orderDes = ;
            this.filterForm.OnChange();
        }

        private bool OnEdit(GrilleEditedElement element)
        {
            if (this.EditEventHandler != null) return EditEventHandler(element);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Grille getNewObject()
        {
            Grille grille = new Grille();
            return grille;
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {

            if (this.EditedObject != null)
            {
                GrilleFilter filter = this.filterForm.Fill();
                this.EditedObject.GrilleFilter = filter;
            }
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            throwHandler = false;
            if (this.EditedObject == null)
            {
                this.filterForm.Display(null);
                throwHandler = true;
                return;
            }

            GrilleFilter filter = this.filterForm.GrilleFilter;
            if (filter != null)
            {
                filter.grid = this.EditedObject;
                this.EditedObject.GrilleFilter = filter;
            }

            this.filterForm.Display(this.EditedObject.GrilleFilter);
            if (this.gridBrowser.RebuildGrid)
            {
                this.gridBrowser.buildColumns(this.EditedObject);
            }
            throwHandler = true;
        }

        public void displayPage(GrillePage page)
        {
            this.gridBrowser.displayPage(page);
            this.toolBar.displayPage(page);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            return controls;
        }


    }
}
