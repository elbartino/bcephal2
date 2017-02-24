using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.ObjectAdmin
{
    public class AdministrationBar : ScrollViewer
    {

        #region Properties

        public ChangeEventHandler Changed;

        public StackPanel MainPanel { get; protected set; }

        public int? ObjectOid { get; set; }

        public String ObjectType { get; set; }

        public Persistent EditedObject { get; set; }

        #endregion


        #region Constructors

        
        /// <summary>
        /// 
        /// </summary>
        public AdministrationBar(String ObjectType) 
        {
            this.ObjectType = ObjectType;
            InitComponents();
        }

        public AdministrationBar(SubjectType SubjectType)
            : this(SubjectType.ToString())
        {
            
        }

        #endregion


        #region Operations

        public void Display()
        {
            this.MainPanel.Children.Clear();
            if (this.EditedObject != null)
            {
                if (this.EditedObject.rightsListChangeHandler == null)
                {
                    this.EditedObject.rightsListChangeHandler = new PersistentListChangeHandler<Right>();
                }

                List<Object> items = GetProfilsAndUsers();             
                Dictionary<String, RightsGroup> map = new Dictionary<String, RightsGroup>(0);
                foreach (Right right in this.EditedObject.rightsListChangeHandler.Items)
                {
                    Persistent profilOrUser =  right.profil;
                    if (profilOrUser == null) profilOrUser = right.user;
                    string name = right.profil != null ? "P-" + right.profil.name : right.user != null ? "U-" + right.user.name : null;
                    if (!map.ContainsKey(name)) 
                    {
                        RightsGroup group1 = new RightsGroup(right.objectType);
                        group1.ProfilComboBox.ItemsSource = items;
                        group1.ProfilComboBox.SelectedItem = profilOrUser;
                        map.Add(name, group1);
                        this.AddGroup(group1);
                    }
                    RightsGroup group = null;
                    if (map.TryGetValue(name, out group))
                    {
                        group.Select(right);
                    } 
                }
                AddDefaultGroup(items);
            }            
        }

        private void AddDefaultGroup(List<Object> items)
        {
            RightsGroup group1 = new RightsGroup(this.ObjectType);
            group1.ProfilComboBox.ItemsSource = items;
            group1.IsExpanded = true;
            this.AddGroup(group1);
        }
        
        public virtual void SetReadOnly(bool readOnly)
        {
            
        }

        public void AddGroup(RightsGroup group)
        {
            group.Changed += OnRightChange;
            group.Deleted += OnGroupDelete;
            group.ProfilChanged += OnProfilChange;
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Add(group);
        }
   
        public void RemoveGroup(RightsGroup group)
        {
            group.Changed -= OnRightChange;
            group.Deleted -= OnGroupDelete;
            group.ProfilChanged -= OnProfilChange;
            this.MainPanel.Children.Remove(group);
        }

        public void Clear()
        {
            this.MainPanel.Children.Clear();
        }

        public List<Object> GetProfilsAndUsers()
        {
            ProfilService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetProfilService();
            List<Domain.Profil> profils = service.getAll();
            UserService userservice = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetUserService();
            List<Domain.User> users = userservice.getAll();
            List<Object> items = new List<object>(profils);
            items.AddRange(users);
            return items;    
        }

        public void TryToAddDefaultGroup()
        {
            int count = this.MainPanel.Children.Count;
            if (count > 0)
            {
                UIElement elt = this.MainPanel.Children[count - 1];
                if (elt is RightsGroup)
                {
                    RightsGroup group = (RightsGroup)elt;
                    if (group.ProfilComboBox.SelectedItem != null) AddDefaultGroup(GetProfilsAndUsers());
                }
            }
        }

        public bool IsDuplicateProfil(RightsGroup group)
        {
            foreach (UIElement elt in this.MainPanel.Children)
            {
                if (elt is RightsGroup && elt != group)
                {
                    Object item = ((RightsGroup)elt).ProfilComboBox.SelectedItem;
                    if (item != null && item.Equals(group.ProfilComboBox.SelectedItem)) return true;
                }
            }    
            return false;
        }

        #endregion
        

        #region Initializations

        /// <summary>
        /// Initialize MainPanel and groups.
        /// </summary>
        private void InitComponents()
        {
            this.MainPanel = new StackPanel();
            this.MainPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.Content = this.MainPanel;
        }
                
        #endregion

        
        #region Handlers
        
        private void OnGroupDelete(object item)
        {
            if (item != null && item is RightsGroup)
            {
                RightsGroup group = (RightsGroup)item;
                if (this.EditedObject != null)
                {
                    foreach (Right right in group.GetCheckRights())
                    {
                        if (right.oid.HasValue) this.EditedObject.rightsListChangeHandler.AddDeleted(right);
                        else this.EditedObject.rightsListChangeHandler.forget(right);
                    }
                }
                RemoveGroup(group);
            }
            OnChange();
            TryToAddDefaultGroup();
        }

        private bool OnProfilChange(object item)
        {
            if (item != null && item is RightsGroup)
            {
                RightsGroup group = (RightsGroup)item;
                bool duplicate = IsDuplicateProfil(group);
                if (!duplicate)
                {
                    if (this.EditedObject != null)
                    {
                        Object elt = group.ProfilComboBox.SelectedItem;
                        bool isProfil = elt != null && elt is Domain.Profil;
                        foreach (Right right in group.GetCheckRights())
                        {
                            if (isProfil) right.profil = (Domain.Profil)elt;
                            else right.user = (Domain.User)elt;
                            this.EditedObject.rightsListChangeHandler.AddUpdated(right);
                        }
                    }
                    OnChange();
                    TryToAddDefaultGroup();
                }
                else
                {
                    MessageDisplayer.DisplayWarning("Duplicate Profil or User", 
                        "Another group whith Profil or User '" + group.ProfilComboBox.SelectedItem + "' already exits!");
                }
                return !duplicate;
            }
            return true;
        }

        private void OnRightChange(Right right, bool selected)
        {
            if (right != null && this.EditedObject != null)
            {
                if (selected) this.EditedObject.rightsListChangeHandler.AddNew(right);
                else if (right.oid.HasValue) this.EditedObject.rightsListChangeHandler.AddDeleted(right);
                else this.EditedObject.rightsListChangeHandler.forget(right);
            }
            OnChange();
        }

        protected void OnChange()
        {
            if (Changed != null) Changed();
        }

        #endregion

    }
}
