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

        public ReconciliationMainPanel reconciliationMainPanel { get; private set; }        

        public ReconciliationPropertiePanel ReconciliationPropertiePanel { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public ReconciliationTemplate EditedObject { get; set; }

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
        public ReconciliationForm()
        {
            InitializeComponents();
        }

        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;
            this.reconciliationMainPanel = new ReconciliationMainPanel();
            this.ReconciliationPropertiePanel = new ReconciliationPropertiePanel();
            this.Content = reconciliationMainPanel;
        }

        public void setPostingService(PostingService service)
        {
            reconciliationMainPanel.setPostingService(service);
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
        public ReconciliationTemplate getNewObject() { return new ReconciliationTemplate(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = this.ReconciliationPropertiePanel.validateEdition();
            return valid;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.ReconciliationPropertiePanel.fillReconciliation(this.EditedObject);
            this.reconciliationMainPanel.Fill(this.EditedObject);
        }
         /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.ReconciliationPropertiePanel.displayReconciliation(this.EditedObject);
            this.reconciliationMainPanel.Display(this.EditedObject);
        }


        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(this.ReconciliationPropertiePanel.getEditableControls());
            controls.AddRange(this.reconciliationMainPanel.getEditableControls());
            return controls;
        }

        #endregion

      
    public Misp.Kernel.Domain.ReconciliationContext IEditableView<Kernel.Domain.ReconciliationContext>.EditedObject
    {
	      get;set;
    }

    public Kernel.Domain.ReconciliationContext IEditableView<Kernel.Domain.ReconciliationContext>.getNewObject()
    {
 	    return new Kernel.Domain.ReconciliationContext();
    }
    }
}
