using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.Role
{
    public class RoleForm : UserControl, IEditableView<Domain.Role>
    {
        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public RoleForm(Domain.SubjectType type) : base()
        {
            this.SubjectType = type;
            InitializeComponents();
        }


        protected virtual void InitializeComponents()
        {
            this.Background = System.Windows.Media.Brushes.White;
            this.BorderBrush = null;
            
            //this.CalculatedMeasurePropertiesPanel = new CalculatedMeasurePropertiesPanel();
            this.RolePanel = new RolePanel();
            this.AddChild(this.RolePanel);
        }
        
        #endregion


        #region Properties

        public Domain.SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }

        // public CalculatedMeasurePropertiesPanel CalculatedMeasurePropertiesPanel { get; private set; }
        public RolePanel RolePanel { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// calculated Measure en édition
        /// </summary>
        public Domain.Role EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        #endregion


        #region Methods

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
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
        public Domain.Role getNewObject() { return new Domain.Role(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = true;//this.CalculatedMeasurePropertiesPanel.validateEdition();
            return valid;
            
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.RolePanel.fillObject(this.EditedObject);
        }
         /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
          //this.CalculatedMeasurePropertiesPanel.displayCalculatedMeasureProperties(this.EditedObject);
          this.RolePanel.DisplayRole(this.EditedObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            //controls.AddRange(CalculatedMeasurePropertiesPanel.getEditableControls());
            return controls;
        }

        #endregion
    }
}
