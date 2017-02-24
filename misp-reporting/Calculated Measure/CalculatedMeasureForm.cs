using Misp.Kernel.Administration.ObjectAdmin;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Misp.Reporting.Calculated_Measure
{
    public class CalculatedMeasureForm : UserControl, IEditableView<CalculatedMeasure>
    {
           
        #region Constructor


        /// <summary>
        /// Constructeur
        /// </summary>
        public CalculatedMeasureForm(SubjectType subjectType)
        {
            this.SubjectType = subjectType;
            InitializeComponents();
            this.CalculatedMeasurePanel.Changed += CalculatedMeasureOperationsGrid_Changed;
        }

        /// <summary>
        /// methode appele lorsquil ya changement sur le calculatedmeasureoperation grid
        /// </summary>
        void CalculatedMeasureOperationsGrid_Changed()
        {
            int nbre = this.CalculatedMeasurePanel.panel.Children.Count;
            if (nbre ==1)
            {
                this.CalculatedMeasurePropertiesPanel.IgnorePropertiesGrid.setVisible(false);
            }
            
        }


        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;
            if (ApplicationManager.Instance.User.IsAdmin())
            {
                this.AdministrationBar = new AdministrationBar(this.SubjectType);
            }
            this.CalculatedMeasurePropertiesPanel = new CalculatedMeasurePropertiesPanel();
            this.CalculatedMeasurePanel = new CalculatedMeasurePanel();
            this.AddChild(this.CalculatedMeasurePanel);
        }
        
        #endregion


        #region Properties

        public AdministrationBar AdministrationBar { get; set; }

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }
        
        public CalculatedMeasurePropertiesPanel CalculatedMeasurePropertiesPanel { get; private set; }
        public CalculatedMeasurePanel CalculatedMeasurePanel { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// calculated Measure en édition
        /// </summary>
        public CalculatedMeasure EditedObject { get; set; }

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
        public CalculatedMeasure getNewObject() { return new CalculatedMeasure(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = this.CalculatedMeasurePropertiesPanel.validateEdition();
            return valid;
            
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.CalculatedMeasurePropertiesPanel.fillCalculatedMeasure(this.EditedObject);
        }
         /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
            }
            this.CalculatedMeasurePropertiesPanel.displayCalculatedMeasureProperties(this.EditedObject);
            this.CalculatedMeasurePanel.DisplayCalculatedMeasure(this.EditedObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(CalculatedMeasurePropertiesPanel.getEditableControls());
            return controls;
        }

        #endregion
    }
}
