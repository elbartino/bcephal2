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

        private List<string> labelList = new List<string>();

        private bool throwHandler;

        string createLabel = "Create ";
        string editLabel = "Edit ";
        string viewLabel = "View ";
        string saveLabel = "Save ";
        string deleteLabel = "Delete ";
        string saveAsLabel = "Save As ";
        string clearLabel = "Clear ";
        string runLabel = "Run ";

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
                    setLabelText(SubjectType.INPUT_TABLE);
                }
                else if (ObjectType.Equals(SubjectType.REPORT.label))
                {
                    setLabelText(SubjectType.REPORT);
                }
                else if (ObjectType.Equals(SubjectType.INPUT_GRID.label))
                {
                    setLabelText(SubjectType.INPUT_GRID);
                }
                else if (ObjectType.Equals(SubjectType.TARGET.label))
                {
                    setLabelText(SubjectType.TARGET);
                }
                else if (ObjectType.Equals(SubjectType.DESIGN.label))
                {
                    setLabelText(SubjectType.DESIGN);
                }
            }
        }
        
        private void setLabelText(SubjectType subjectType,string name="") 
        {
            buildLabelList(subjectType.label);
            int j = 0;
            for (int i = this.labelList.Count - 1;i >=0 ; i--)
            {
                string label = labelList[j];
                string funct =   label + name;
                AddCheckBox(new RightCheckBox(funct, getTypeByLabel(label.Trim())));
                j++;
            }
        }

        private RightType getTypeByLabel(string text) 
        {
           if(text.Equals(createLabel.Trim())) return RightType.CREATE;
           if (text.Equals(editLabel.Trim())) return RightType.EDIT;
           if (text.Equals(viewLabel.Trim())) return RightType.VIEW;
           if (text.Equals(deleteLabel.Trim())) return RightType.DELETE;
           if (text.Equals(runLabel.Trim())) return RightType.LOAD;
           if (text.Equals(saveAsLabel.Trim())) return RightType.SAVE_AS;
           if (text.Equals(saveLabel.Trim())) return RightType.SAVE;
           if (text.Equals(clearLabel.Trim())) return RightType.CLEAR;
           return RightType.VIEW;
        }

        private void buildLabelList(String subjectType) 
        {
            if (subjectType.Equals(SubjectType.INPUT_GRID.label) || subjectType.Equals(SubjectType.INPUT_TABLE.label)
               || (subjectType.Equals(SubjectType.DESIGN.label) || (subjectType.Equals(SubjectType.TARGET.label) )
            {
                labelList.Add(editLabel);
                labelList.Add(viewLabel);
                labelList.Add(saveAsLabel);
                
            }                
            if (subjectType.Equals(SubjectType.INPUT_GRID.label) || subjectType.Equals(SubjectType.INPUT_TABLE.label)
                || subjectType.Equals(SubjectType.REPORT.label))
            {
                labelList.Add(runLabel);
                labelList.Add(clearLabel);
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
