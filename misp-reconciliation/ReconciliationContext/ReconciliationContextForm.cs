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
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Sourcing.Table;
using Misp.Kernel.Service;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContexForm : UserControl, IEditableView<Kernel.Domain.ReconciliationContext>
    {

        #region Properties

        public ReconciliationContextPanel ReconciliationContextPanel { get; private set; }        

        //public ReconciliationPropertiePanel ReconciliationPropertiePanel { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public Kernel.Domain.ReconciliationContext EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public event Misp.Kernel.Ui.Base.ActivateEventHandler ActivatedItem;

        public Kernel.Service.GroupService GroupService { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public ReconciliationContexForm()
        {
            InitializeComponents();
        }

        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;

            this.ReconciliationContextPanel = new ReconciliationContextPanel();
            this.ReconciliationContextPanel.ActivatedItem += OnActivateFormItem; 
            //this.ReconciliationPropertiePanel = new ReconciliationPropertiePanel();
            this.Content = ReconciliationContextPanel;
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.ReconciliationContextPanel.ActiveItem.setAttribute(attribute);
        }

        public void setValue(Kernel.Domain.AttributeValue value) { }

        private void OnActivateFormItem(object item)
        {
            if(item is ReconciliationContextItem){
                this.ReconciliationContextPanel.ActiveItem = (ReconciliationContextItem)item;
                //if (((ReconciliationContextItem)item) == this.ReconciliationContextPanel.postingAttribute)
                //{

                //}

                //if (((TextBox)item) == this.ReconciliationContextPanel.reconcilTextbox)
                //{

                //}

                //if (((TextBox)item) == this.ReconciliationContextPanel.accoutTextbox)
                //{

                //}

                //if (((TextBox)item) == this.ReconciliationContextPanel.dcTextbox)
                //{

                //}

                //if (((TextBox)item) == this.ReconciliationContextPanel.lastPosTextbox)
                //{

                //}

                //if (((TextBox)item) == this.ReconciliationContextPanel.lastRecoTextbox)
                //{

                //}
            }
        }

       
        
        #endregion


        #region Methods

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
        public Kernel.Domain.ReconciliationContext getNewObject() { return new Kernel.Domain.ReconciliationContext(); }

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
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            // this.ReconciliationContextPanel.fillReconciliation(this.EditedObject);
           
        }
         /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            //this.ReconciliationPropertiePanel.displayReconciliation(this.EditedObject);
        }


        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            // controls.AddRange(this.ReconciliationContextPanel.getEditableControls());
            return controls;
        }

        #endregion

      
   
    }
}
