using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Reconciliation.WriteOffConfig
{
    public class ReconciliationFilterTemplateForm : UserControl, IEditableView<Kernel.Domain.ReconciliationFilterTemplate>
    {
          #region Properties

        public bool IsReadOnly { get; set; }
        
        public WriteOffConfigPanel WriteOffConfigPanel { get; private set; }

        //public ReconciliationContextPropertyBar ReconciliationContextPropertyBar { get; set; }

        //public ModelService ModelService { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public Kernel.Domain.ReconciliationFilterTemplate EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public ReconciliationFilterTemplateForm()
        {
            InitializeComponents();
        }

        protected virtual void InitializeComponents()
        {
            this.Background = Brushes.White;
            this.BorderBrush = null;

            this.WriteOffConfigPanel = new WriteOffConfigPanel();
            //this.ReconciliationContextPropertyBar = new ReconciliationContextPropertyBar();
            this.Content = WriteOffConfigPanel;
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            //this.WriteOffConfigPanel.setAttribute(attribute);
        }

        public void setValue(Kernel.Domain.AttributeValue value)
        {
           // if (this.ModelService == null) return;
//this.ReconciliationContextPanel.ModelService = this.ModelService;
            //this.ReconciliationContextPanel.setAttributeValue(value);
        }

        public void setMeasure(Kernel.Domain.Measure measure) 
        {
            //this.ReconciliationContextPanel.setMeasure(measure);
        }

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
        public Kernel.Domain.ReconciliationFilterTemplate getNewObject() { return new Kernel.Domain.ReconciliationFilterTemplate(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            //bool valid = this.ReconciliationContextPanel.validateEdition();
            return true;
        }
        
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.WriteOffConfigPanel.display(this.EditedObject);            
        }

        public void fillObject()
        {
            
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(this.WriteOffConfigPanel.getEditableControls());
            return controls;
        }

       

        #endregion
    }
}
