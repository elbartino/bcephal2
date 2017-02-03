using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.UserProfile
{
    public class UserProfileForm : UserControl, IEditableView<Kernel.Domain.User>
    {
        #region Property

        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        public ConnectedUserProfile ConnectedUserProfile { get; set; }

        //public UserPropertyPanel userPropertyPanel { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public Kernel.Domain.User EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.UserService UserService { get; set; }

        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public UserProfileForm()
        {
            InitializeComponents();
        }


        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;
            this.ConnectedUserProfile = new ConnectedUserProfile();
            //this.userPropertyPanel = new UserPropertyPanel();
            this.Content = ConnectedUserProfile;
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

        public void setUserService(UserService service)
        {
            //userMainPanel.setUserService(service);
        }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Kernel.Domain.User getNewObject() { return new Domain.User(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = this.ConnectedUserProfile.ValidateEdition();
            return valid;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.ConnectedUserProfile.Fill(this.EditedObject);
        }
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.ConnectedUserProfile.Display(this.EditedObject);
            //this.userMainPanel.Display(this.EditedObject);
            //if (UserService == null) return;
            //Domain.Role rootRole = UserService.RoleService.getRootRole();
            //this.userMainPanel.RelationPanel.FillRoles(rootRole.childrenListChangeHandler.Items.ToList());
            //this.userMainPanel.RelationPanel.FillUsers(UserService.getAll());
        }


        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(this.ConnectedUserProfile.getEditableControls());
            return controls;
        }

        #endregion

    }
}
