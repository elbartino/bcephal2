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
using Misp.Kernel.Domain;
using System.Windows.Forms.Integration;
using Misp.Sourcing.Table;
using Misp.Kernel.Administration.ObjectAdmin;
using Misp.Kernel.Application;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeForm : UserControl, IEditableView<Kernel.Domain.CombinedTransformationTree>
    {
        
        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public CombinedTransformationTreeForm(SubjectType subjectType)
        {
            this.SubjectType = subjectType;
            InitializeComponents();      
        }


        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;
            this.CombinedTransformationTreePropertiesPanel = new CombinedTransformationTreePropertiesPanel();
            this.CombinedTransformationTreePanel = new CombinedTransformationTreePanel();
            this.Content = CombinedTransformationTreePanel;

            if (ApplicationManager.Instance.User.IsAdmin())
            {
                this.AdministrationBar = new AdministrationBar(this.SubjectType);
            }
        }
        
        #endregion


        #region Properties

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }
        
        public CombinedTransformationTreePropertiesPanel CombinedTransformationTreePropertiesPanel { get; private set; }

        public CombinedTransformationTreePanel CombinedTransformationTreePanel { get; private set; }

        public AdministrationBar AdministrationBar { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public Kernel.Domain.CombinedTransformationTree EditedObject { get; set; }

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
        /// Customize for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public virtual void Customize(List<Kernel.Domain.Right> rights, bool readOnly = false)
        {

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
        public Kernel.Domain.CombinedTransformationTree getNewObject() { return new Kernel.Domain.CombinedTransformationTree(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = this.CombinedTransformationTreePropertiesPanel.validateEdition();
            return valid;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.CombinedTransformationTreePropertiesPanel.fillCombinedTransformationTree(this.EditedObject);
        }
         /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            correctGroup(this.EditedObject);
            this.CombinedTransformationTreePropertiesPanel.displayCombinedTransformationTree(this.EditedObject);
            this.CombinedTransformationTreePanel.DisplayTransformationTrees(this.EditedObject);
            
            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
            }
        }

        public void correctGroup(Kernel.Domain.CombinedTransformationTree combined)
        {
            if (GroupService != null)
            {
                int oidG = (int)combined.group.oid;
                BGroup gr = (BGroup)GroupService.getByOid(oidG);
                combined.group = gr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(CombinedTransformationTreePropertiesPanel.getEditableControls());
            return controls;
        }

        #endregion


    }
}
