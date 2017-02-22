using Misp.Kernel.Domain;
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

        public String ObjectType { get; set; }

        private List<string> labelList = new List<string>();

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
            InitializeComponent();
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

        public RightsGroup(String profile,String ObjectType)
            : this(ObjectType)
        {
            this.Header = profile;
        }

        public RightsGroup(String profile, String ObjectType,String righttype)
            : this(profile,ObjectType)
        {
            if (string.IsNullOrEmpty(righttype)) return;
            setRight(righttype);
        }

        #endregion


        #region Operations


        #endregion


        #region Initializations

        private void Customize()
        {
            if (!string.IsNullOrWhiteSpace(ObjectType))
            {
               
                if (ObjectType.Equals(SubjectType.INPUT_TABLE.label))
                {
                    setLabelText(SubjectType.INPUT_TABLE.label);
                }
                else if (ObjectType.Equals(SubjectType.REPORT.label))
                {
                    setLabelText(SubjectType.REPORT.label);
                }
                else if (ObjectType.Equals(SubjectType.INPUT_GRID.label))
                {
                    setLabelText(SubjectType.INPUT_GRID.label);
                }
            }
        }

        private void setLabelText(String subjectType) 
        {
            buildLabelList(subjectType);
            int j = 0;
            for (int i = this.labelList.Count - 1;i >=0 ; i--)
            {
                string label = labelList[j];
                string funct =   label + subjectType;
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
            labelList.Add(createLabel);
            labelList.Add(editLabel);
            labelList.Add(viewLabel);
            labelList.Add(saveLabel);
            labelList.Add(saveAsLabel);
                
            if (subjectType.Equals(SubjectType.INPUT_GRID.label) || subjectType.Equals(SubjectType.INPUT_TABLE.label)
                || subjectType.Equals(SubjectType.REPORT.label))
            {
                labelList.Add(runLabel);
            }
            if (subjectType.Equals(SubjectType.INPUT_GRID.label) || subjectType.Equals(SubjectType.INPUT_TABLE.label))
            {
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

        }

        private void setRight(String right)
        {
            foreach (UIElement check in this.RightsPanel.Children) 
            {
                if (!(check is RightCheckBox)) continue;
                RightCheckBox righchec = (RightCheckBox)check;
                if (righchec.RightType.ToString().Equals(right))
                {
                    righchec.IsChecked = true;
                    break;
                }
            }
        }

        private void setTableAdminPanel() { }

        private void setGridAdminPanel() 
        {
            
        }

        #endregion


        #region Handlers


        #endregion

    }
}
