using Misp.Kernel.Application;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Kernel.Administration.ObjectAdmin
{
    /// <summary>
    /// Interaction logic for RightsGroup.xaml
    /// </summary>
    public partial class RightsGroup : Expander
    {

        
        #region Properties

        public RightEventHandler Changed;

        public DeleteEventHandler Deleted;

        public ActionEventHandler ProfilChanged;
        
        public String ObjectType { get; set; }

        public RightsGroupHeader RightsGroupHeader { get; set; }
        
        private bool throwHandler;
        
        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RightsGroup()
        {
            throwHandler = true;
            InitializeComponent();
            this.RightsGroupHeader = new RightsGroupHeader();
            this.Header = this.RightsGroupHeader;
            Brush color = (Brush)new BrushConverter().ConvertFrom("#FFFAC090");
            this.BorderBrush = color;
            this.Background = color;
            this.Margin =new Thickness(0 ,5 ,0 ,5);
        }
        
        public RightsGroup(String ObjectType) : this()
        {
            this.ObjectType = ObjectType;
            Customize();
        }

        public RightsGroup(Object profilOrUser, String ObjectType)
            : this(ObjectType)
        {            
            
            this.ProfilComboBox.SelectedItem = profilOrUser;
        }
        
        #endregion


        #region Operations

        public void Select(Right right)
        {
            throwHandler = false;
            foreach (UIElement check in this.RightsPanel.Children)
            {
                if (check is RightCheckBox)
                {
                    RightCheckBox box = (RightCheckBox)check;
                    if (box.RightType.ToString().Equals(right.rightType))
                    {
                        box.Right = right;
                        box.IsChecked = true;
                        break;
                    }
                }
            }
            throwHandler = true;
        }

        public List<Right> GetCheckRights()
        {
            List<Right> rights = new List<Right>(0);
            foreach (UIElement check in this.RightsPanel.Children)
            {
                if (check is RightCheckBox)
                {
                    RightCheckBox box = (RightCheckBox)check;
                    if (box.Right != null && box.IsChecked.Value)
                    {
                        rights.Add(box.Right);
                    }
                }
            }
            return rights;
        }

        #endregion


        #region Initializations

        private void Customize()
        {            
            this.ProfilComboBox.SelectionChanged += OnSelectProfil;
            this.RightsGroupHeader.DeleteButton.Click += OnDelete;

            if (!string.IsNullOrWhiteSpace(ObjectType))
            {
                if (ObjectType.Equals(SubjectType.INPUT_TABLE.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit Table", RightType.EDIT));
                    AddCheckBox(new RightCheckBox("Edit Cell", RightType.EDIT_CELL));
                    AddCheckBox(new RightCheckBox("Edit Allocation", RightType.EDIT_ALLOCATION));
                    AddCheckBox(new RightCheckBox("Edit Excel only", RightType.EDIT_EXCEL));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                    AddCheckBox(new RightCheckBox("Load", RightType.LOAD));
                    AddCheckBox(new RightCheckBox("Clear", RightType.CLEAR));
                    AddCheckBox(new RightCheckBox("Save as", RightType.SAVE_AS));
                }
                else if (ObjectType.Equals(SubjectType.REPORT.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit Report", RightType.EDIT));
                    AddCheckBox(new RightCheckBox("Edit Cell", RightType.EDIT_CELL));
                    AddCheckBox(new RightCheckBox("Edit Excel only", RightType.EDIT_EXCEL));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                    AddCheckBox(new RightCheckBox("Run", RightType.LOAD));
                    AddCheckBox(new RightCheckBox("Save as", RightType.SAVE_AS));
                }
                else if (ObjectType.Equals(SubjectType.INPUT_GRID.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit Grid", RightType.EDIT));
                    AddCheckBox(new RightCheckBox("Edit Line", RightType.EDIT_CELL));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                    AddCheckBox(new RightCheckBox("Save as", RightType.SAVE_AS));
                }
                else if (ObjectType.Equals(SubjectType.TARGET.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit", RightType.EDIT));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                    AddCheckBox(new RightCheckBox("Save as", RightType.SAVE_AS));
                }
                else if (ObjectType.Equals(SubjectType.DESIGN.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit", RightType.EDIT));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                    AddCheckBox(new RightCheckBox("Save as", RightType.SAVE_AS));
                }
                else if (ObjectType.Equals(SubjectType.RECONCILIATION_FILTER.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit write off", RightType.EDIT_WRITE_OFF));
                    AddCheckBox(new RightCheckBox("Reset reconciliation", RightType.RESET_RECONCILIATION));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                }
                else
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                    AddCheckBox(new RightCheckBox("Edit", RightType.EDIT));
                    AddCheckBox(new RightCheckBox("Delete", RightType.DELETE));
                    AddCheckBox(new RightCheckBox("Save as", RightType.SAVE_AS));
                }
            }
        }
        
        protected void AddCheckBox(RightCheckBox checkBox)
        {
            checkBox.Checked += OnHandlingCheckbox;
            checkBox.Unchecked += OnHandlingCheckbox;
            RightsPanel.Children.Add(checkBox);
        }

        private void OnHandlingCheckbox(object sender, RoutedEventArgs e)
        {
            if (throwHandler && sender is RightCheckBox) OnChange((RightCheckBox)sender);
        }


        private void setTableAdminPanel() { }

        private void setGridAdminPanel() 
        {
            
        }

        #endregion


        #region Handlers


        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (throwHandler && Deleted != null) Deleted(this);
        }

        private void OnSelectProfil(object sender, SelectionChangedEventArgs e)
        {
            bool isOk = true;
            if (throwHandler && ProfilChanged != null) isOk = ProfilChanged(this);
            if (!isOk)
            {
                throwHandler = false;
                this.ProfilComboBox.SelectedItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] : null;
                throwHandler = true;                
            }
            String name = this.ProfilComboBox.SelectedItem != null ? this.ProfilComboBox.SelectedItem.ToString() : "";
            this.RightsGroupHeader.Label.Content = name;
            this.RightsScrollViewer.Visibility = this.ProfilComboBox.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
            this.RightsGroupHeader.DeleteButton.Visibility = this.ProfilComboBox.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
            this.RightsGroupHeader.DeleteButton.ToolTip = "Remove " + name + " group";            
        }

        protected void OnChange(RightCheckBox box)
        {
            if (throwHandler && Changed != null)
            {
                if (box.IsChecked.Value && box.Right == null)
                {
                    box.Right = new Right();
                    box.Right.objectType = this.ObjectType;
                    box.Right.rightType = box.RightType.ToString();
                    box.Right.projectReference = ApplicationManager.Instance.File.code;
                    Object item = this.ProfilComboBox.SelectedItem;
                    if (item != null && item is Domain.Profil) box.Right.profil = (Domain.Profil)item;
                    else if (item != null && item is Domain.User) box.Right.user = (Domain.User)item;
                }
                Changed(box.Right, box.IsChecked.Value);
                if (!box.IsChecked.Value) box.Right = null;
            }
        }

        #endregion

    }
}
