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

namespace Misp.Kernel.Administration.Role
{
    /// <summary>
    /// Interaction logic for RolePanel.xaml
    /// </summary>
    public partial class RolePanel : ScrollViewer, IChangeable
    {


        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des RoleItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        
        public event SelectedItemChangedEventHandler ItemAddedSelected;

        #endregion


        #region Properties

        public Domain.Role RootRole { get; set; }

        public RoleItemPanel ActiveItemPanel { get; set; }
        public Label Label { get; set; }

        #endregion


        #region Constructors

        public RolePanel()
        {
            InitializeComponent();
           
        }

        
        #endregion

        #region Operations

        /// <summary>
        /// affiche le calculatedMeasure en edition
        /// </summary>
        /// <param name="table"></param>
        public void DisplayRole(Domain.Role root)
        {
            this.panel.Children.Clear();
            int index = 1;
            this.ActiveItemPanel = new RoleItemPanel(index);
            if (root == null)
            {
                this.ActiveItemPanel = new RoleItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                return;
            }
            foreach (Domain.Role item in root.childrenListChangeHandler.Items)
            {

                RoleItemPanel itemPanel = new RoleItemPanel(item);
                itemPanel.Index = index;
                itemPanel.newButton.Visibility = System.Windows.Visibility.Hidden;
                AddItemPanel(itemPanel);
                index++;
            }

            this.ActiveItemPanel = new RoleItemPanel(index);
            AddItemPanel(this.ActiveItemPanel);
        }

     

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public bool SetRoleItemValue(Domain.Role value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (RoleItemPanel)this.panel.Children[this.panel.Children.Count - 1];
           
            this.ActiveItemPanel.SetValue(value);
            return true;
        }


        protected void AddItemPanel(RoleItemPanel itemPanel)
        {
            itemPanel.Added+= OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            this.panel.Children.Add(itemPanel);
        }

       
        #endregion

        #region Handlers

        private void OnNewItemSelected(object newSelection)
        {
            if (ItemAddedSelected != null)
                ItemAddedSelected(newSelection);
        }


        private void OnActivated(object item)
        {
            RoleItemPanel panel = (RoleItemPanel)item;
            if (this.ActiveItemPanel.Role != null && this.ActiveItemPanel.Role != panel.Role)
            {
                this.ActiveItemPanel = panel;
                if (ItemChanged != null && panel.Role != null) ItemChanged(panel.Role);
            }
        }


        private void OnAdded(object item)
        {
            RoleItemPanel panel = (RoleItemPanel)item;
            if (this.RootRole == null) this.RootRole = new Domain.Role();
            updated = false;
            OnChanged(panel);
            OnActivated(item);
        }


        bool updated = false;
        private void OnUpdated(object item)
        {
            RoleItemPanel panel = (RoleItemPanel)item;
            updated = true;
            OnChanged(panel.Role);
        }

        public void fillObject(Domain.Role role)
        {
            foreach (UIElement el in this.panel.Children)
            {
                RoleItemPanel item = (RoleItemPanel)el;
                String roleName = item.TextBox.Text;
                if (String.IsNullOrEmpty(roleName)) continue;
                Domain.Role roleItem = new Domain.Role()
                {
                        name = roleName
                };
                if (item.Role != null)
                {
                    item.Role.name = roleName;
                    role.UpdateChild(item.Role);
                }
                else{
                    role.AddChild(roleItem);
                }
            }
        }

        public void Display() 
        {
            int j = 0;
            RoleItemPanel item;
            for(int i = this.panel.Children.Count - 1; i >=0; i--)
            {
                item = (RoleItemPanel)this.panel.Children[j];
                item.newButton.Visibility = System.Windows.Visibility.Hidden;
                j++;
                item.Index = j;
            }
            item = (RoleItemPanel)this.panel.Children[j-1];
            item.newButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnDeleted(object item)
        {
            RoleItemPanel panel = (RoleItemPanel)item;
            if (item is UIElement)
            {
                this.panel.Children.Remove((UIElement)item);
                if (this.panel.Children.Count == 0) OnAdded(null);
            }
            if(panel != null) ItemDeleted(panel.Role);
            Display();
        }

        private void OnChanged(object item)
        {
            if (this.RootRole == null) this.RootRole = new Domain.Role();
            int count = this.panel.Children.Count;
            if (!updated)
            {
                this.ActiveItemPanel = new RoleItemPanel(count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
          
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
            Display();
        }


        
        #endregion


    }
}
