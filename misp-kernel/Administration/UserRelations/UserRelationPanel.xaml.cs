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
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Service;

namespace Misp.Kernel.Administration.UserRelations
{
    /// <summary>
    /// Interaction logic for CalculatedMeasurePanel.xaml
    /// </summary>
    public partial class UserRelationPanel : ScrollViewer, IChangeable
    {


        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des CalculatedMeasureItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        
        public event SelectedItemChangedEventHandler ItemCloseParOrEqualSelected;

        #endregion


        #region Properties

        public Domain.User User { get; set; }

        public UserRelationItemPanel ActiveItemPanel { get; set; }
        public Label Label { get; set; }

        #endregion


        #region Constructors

        public UserRelationPanel()
        {
            InitializeComponent();
           
        }

        
        #endregion

        #region Operations

        /// <summary>
        /// affiche le calculatedMeasure en edition
        /// </summary>
        /// <param name="table"></param>
        public void DisplayUserRelations(Domain.User user)
        {
            this.User = user;
            this.panel.Children.Clear();
            int index = 1;
            if (user == null)
            {
                this.ActiveItemPanel = new UserRelationItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                return;
            }
            foreach (Relation item in user.relationsListChangeHandler.Items)
            {
                UserRelationItemPanel itemPanel = new UserRelationItemPanel();                
                itemPanel.Index = index;
                AddItemPanel(itemPanel);
                itemPanel.Display(item);
                index++;
            }

            this.ActiveItemPanel = new UserRelationItemPanel(index);
            AddItemPanel(this.ActiveItemPanel);
        }

     

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public bool SetCalculatedMeasureItemValue(Relation value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (UserRelationItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            return true;
        }


        protected void AddItemPanel(UserRelationItemPanel itemPanel)
        {
            itemPanel.Added+= OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.FillUsers(this.Users);
            itemPanel.FillRoles(this.Roles);
            this.panel.Children.Add(itemPanel);
        }

       
        #endregion

        #region Handlers

        private void OnCloseParOrEqualSelected(object newSelection)
        {
            if (ItemCloseParOrEqualSelected != null)
                ItemCloseParOrEqualSelected(newSelection);
            
        }


        private void OnActivated(object item)
        {
            UserRelationItemPanel panel = (UserRelationItemPanel)item;
            if (this.ActiveItemPanel != panel)
            {
                this.ActiveItemPanel = panel;
                if (ItemChanged != null && panel.RelationItem != null) ItemChanged(panel.RelationItem);
            }
        }


        private void OnAdded(object item)
        {
            UserRelationItemPanel panel = (UserRelationItemPanel)item;
            if (this.User == null) this.User = new Domain.User();
            if (panel == null) panel = new UserRelationItemPanel(1);
            if (panel.RelationItem == null)
            {
                panel.RelationItem = new Relation();
            }
            panel.RelationItem.user = this.User;
            this.User.relationsListChangeHandler.AddNew(panel.RelationItem);
            updated = false;
            OnChanged(panel.RelationItem);
        }


        bool updated = false;
        private void OnUpdated(object item)
        {
            UserRelationItemPanel panel = (UserRelationItemPanel)item;
            if (this.User == null) this.User = new Domain.User();
            this.User.relationsListChangeHandler.AddUpdated(panel.RelationItem);
            updated = true;
            OnChanged(panel.RelationItem);
        }

        private void OnDeleted(object item)
        {
            UserRelationItemPanel panel = (UserRelationItemPanel)item;
            this.panel.Children.Remove(panel);
            if (this.panel.Children.Count == 0)
            {
                OnAdded(null);
                return;
            }
            if (panel.RelationItem != null)
            {
                if (this.User.relationsListChangeHandler.Items.Count > 1)
                {
                    if (this.User == null)
                    {
                        this.User = new Domain.User();
                    }

                    if (ItemDeleted != null && panel.RelationItem != null) ItemDeleted(panel.RelationItem);

                    
                    if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                        this.ActiveItemPanel = (UserRelationItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                    int index = 1;
                    int j = 0;
                    for (int i = this.panel.Children.Count - 1; i >= 0; i--)
                    {
                        UserRelationItemPanel pan = this.panel.Children[j] as UserRelationItemPanel;
                        pan.Index = index++;
                        j++;
                    }
                    if (Changed != null) Changed();
                }
               }
        }

        private void OnChanged(object item)
        {
            if (this.User == null) this.User = new Domain.User();
            int count = this.panel.Children.Count;
            if (!updated)
            {
                this.ActiveItemPanel = new UserRelationItemPanel(count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }


        
        #endregion


        public List<string> Roles;
        public void FillRoles(List<string> roles)
        {
            Roles = new List<string>();
            this.Roles.AddRange(roles);
            this.ActiveItemPanel.FillRoles(roles);
        }

        public List<string> Users;
        public void FillUsers(List<string> users)
        {
            Users = new List<string>();
            this.Users.AddRange(users);
            this.ActiveItemPanel.FillUsers(users);
        }


        //public List<Domain.Role> Roles;
        //public void FillRoles(List<Domain.Role> roles)
        //{
        //    Roles = new List<Domain.Role>();
        //    this.Roles.AddRange(roles);
        //    this.ActiveItemPanel.FillRoles(roles);
        //}

        //public List<Domain.User> Users;
        //public void FillUsers(List<Domain.User> users)
        //{
        //    Users = new List<Domain.User>();
        //    this.Users.AddRange(users);
        //    this.ActiveItemPanel.FillUsers(users);
        //}
    }
}
