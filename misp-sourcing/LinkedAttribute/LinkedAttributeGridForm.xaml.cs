using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Administration.ObjectAdmin;
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

        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        protected bool RebuildGrid = true;

        #endregion


        #region Constructors

        public LinkedAttributeGridForm()
        {
            InitializeComponent();
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

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            buildColumns();
            //this.InputGridSheetForm.EditedObject = this.EditedObject;
            //this.InputGridSheetForm.displayObject();

            //this.GridForm.EditedObject = this.EditedObject;
            //this.GridForm.displayObject();

            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
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
                int position = 0;
                LinkedAttributeGridColumn column = new LinkedAttributeGridColumn(this.EditedObject.attribute, position++, true);
                GridColumn gridColumn = getColumn(column);
                this.Grid.Columns.Add(gridColumn);
                foreach (Kernel.Domain.Attribute attribute in this.EditedObject.attribute.childrenListChangeHandler.Items)
                {
                    column = new LinkedAttributeGridColumn(attribute, position++);
                    gridColumn = getColumn(column);
                    this.Grid.Columns.Add(gridColumn);
                }
                RebuildGrid = false;
            }
        }


        private GridColumn getColumn(LinkedAttributeGridColumn column)
        {
            DevExpress.Xpf.Grid.GridColumn gridColumn = new DevExpress.Xpf.Grid.GridColumn();
            gridColumn.Header = column.ToString();
            gridColumn.IsSmart = true;
            gridColumn.ReadOnly = this.IsReadOnly;
            gridColumn.ColumnFilterMode = ColumnFilterMode.DisplayText;
            Binding b = new Binding(getBindingName(column));
            b.Mode = BindingMode.TwoWay;
            gridColumn.Binding = b;
            gridColumn.Style = this.Grid.FindResource("GridColumn") as Style;
            gridColumn.Width = new GridColumnWidth(1, GridColumnUnitType.Star);

            if (column.attribute.related && !column.isKey)
            {
                //try
                //{
                //    column.values = Service.ModelService.getLeafAttributeValues(grilleColumn.valueOid.Value);
                //}
                //catch (Exception) { }
                //ComboBoxEditSettings combo = new ComboBoxEditSettings();
                //combo.ItemsSource = column.Items;
                //combo.IsTextEditable = true;
                //combo.ShowText = true;
                //combo.ValidateOnTextInput = true;
                //combo.AllowNullInput = true;
                //gridColumn.EditSettings = combo;
            }

            
            return gridColumn;
        }

        private String getBindingName(LinkedAttributeGridColumn column)
        {
            return "Datas[" + column.position + "]";
        }

        #endregion


    }
}
