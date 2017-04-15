using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Administration.ObjectAdmin;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.GridViews;
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

namespace Misp.Sourcing.LinkedAttribute
{
    /// <summary>
    /// Interaction logic for LinkedAttributeGridForm.xaml
    /// </summary>
    public partial class LinkedAttributeGridForm : Grid, IEditableView<LinkedAttributeGrid>
    {

        #region Properties

        public bool IsReadOnly { get; set; }

        public SubjectType SubjectType { get; set; }
        
        public AdministrationBar AdministrationBar { get; set; }

        public bool IsModify { get; set; }

        public LinkedAttributeGrid EditedObject { get; set; }
        public GrilleFilter Filter { get; set; }

        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        public ChangeEventHandler FilterChangeHandler;
        public Misp.Sourcing.GridViews.GridBrowser.EditCellEventHandler EditEventHandler { get; set; }
        

        protected bool RebuildGrid = true;

        #endregion


        #region Constructors

        public LinkedAttributeGridForm()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public LinkedAttributeGridForm(SubjectType SubjectType)
            : this()
        {
            this.SubjectType = SubjectType; 
        }

        #endregion


        #region Operations

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        /// <summary>
        /// Customize for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public virtual void Customize(List<Kernel.Domain.Right> rights, bool readOnly = false)
        {
            //bool edit = RightsUtil.HasRight(Kernel.Domain.RightType.EDIT, rights);
            //bool editLine = RightsUtil.HasRight(Kernel.Domain.RightType.EDIT_CELL, rights);
            //if (GridForm != null)
            //{
            //    GridForm.SetReadOnly(readOnly || !editLine);
            //    GridForm.filterForm.SetReadOnly(readOnly || !edit);
            //}
            //if (InputGridSheetForm != null) InputGridSheetForm.SetReadOnly(readOnly || !edit);
        }

        public LinkedAttributeGrid getNewObject()
        {
            return new LinkedAttributeGrid();
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
        public virtual void fillObject()
        {
            
        }

        public GrilleFilter FillFilter()
        {
            if (this.Filter == null) this.Filter = new GrilleFilter();
            return this.Filter;
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            buildColumns();
            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
            }
        }

        public void DisplayPage(GrillePage page)
        {
            if (page != null)
            {
                this.Grid.displayPage(page);
                this.Toolbar.displayPage(page);
            }
        }


        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            return controls;
        }

        public void buildColumns()
        {
            if (this.EditedObject != null && RebuildGrid) 
            {
                this.Grid.RebuildGrid = true;
                this.Grid.buildColumns(this.EditedObject);
                RebuildGrid = false;
            }
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.Grid.SortEventHandler += OnSort;
            this.Grid.EditEventHandler += OnEdit;
            this.Grid.FilterEventHandler += OnFilter;
        }

        private void OnFilter()
        {
            if (FilterChangeHandler != null) FilterChangeHandler();
        }

        private void OnSort(object col)
        {            
            if (FilterChangeHandler != null) FilterChangeHandler();            
        }

        private Object[] OnEdit(GrilleEditedElement element)
        {
            if (this.EditEventHandler != null) return EditEventHandler(element);
            return null;
        }

        #endregion

    }
}
