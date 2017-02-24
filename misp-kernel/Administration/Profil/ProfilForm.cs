using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilForm : UserControl, IEditableView<Kernel.Domain.Profil>
    {
        #region Property

        public Domain.SubjectType SubjectType { get; set; }
        
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        public ProfileMainPanel profileMainPanel { get; set; }

        //public ProfilPropertyPanel profilPropertyPanel { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public Kernel.Domain.Profil EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public ProfilForm(Domain.SubjectType type) : base()
        {
            this.SubjectType = type;
            InitializeComponents();
        }

        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;
            this.profileMainPanel = new ProfileMainPanel();
            //this.profilPropertyPanel = new ProfilPropertyPanel();
            this.Content = profileMainPanel;
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
        public Kernel.Domain.Profil getNewObject() { return new Domain.Profil(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = this.profileMainPanel.ValidateEdition();
            return valid;
        }

       
        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            //this.profilPropertyPanel.fillProfil(this.EditedObject);
            this.profileMainPanel.Fill(this.EditedObject);
         }
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.profileMainPanel.Display(this.EditedObject);
            //this.profilPropertyPanel.displayProfil(this.EditedObject);
        }


        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(this.profileMainPanel.getEditableControls());
            //controls.AddRange(this.profilPropertyPanel.getEditableControls());
            return controls;
        }

        #endregion

    }
}
