﻿using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
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

                ProfilService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetProfilService();
                List<Domain.Profil> profils = service.getAll();
                UserService userservice = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetUserService();
                List<Domain.User> users = userservice.getAll();
                List<Object> items = new List<object>(profils);
                items.AddRange(users);
             
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
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Add(group);
        }
        
        public void RemoveGroup(int position)
        {
            this.MainPanel.Children.RemoveAt(position);
        }
        
        public void RemoveGroup(RightsGroup group)
        {
            this.MainPanel.Children.Remove(group);
        }

        public void Clear()
        {
            this.MainPanel.Children.Clear();
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

    }
}
