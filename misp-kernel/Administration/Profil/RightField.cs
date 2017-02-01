using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.Profil
{
    public class RightField : StackPanel
    {

        public List<RightField> ChildrenFields { get; set; }
        public CheckBox CreationCheckBox { get; set; }
        public CheckBox EditionCheckBox { get; set; }
        public CheckBox ConsultationCheckBox { get; set; }
        public Functionality Functionality { get; set; }
        public StackPanel ChidrenPanel { get; set; }


        public event RightEventHandler RightSelected;

        protected bool throwHandler;

        public RightField()
        {
            //initComponents();
            //initHandlers();
        }

        
        protected RightField(Functionality functionality) : this()
        {
            SetFunctionality(functionality);
        }

        public void SetFunctionality(Functionality functionality)
        {
            this.Functionality = functionality;
            initComponents();          
            foreach (Functionality child in functionality.Children)
            {
                AddChild(child);
            }
        }

        public void Select(Right right)
        {
            throwHandler = false;
            bool sameCode = this.Functionality.Code.Equals(right.functionnality);
            if (sameCode)
            {
                if (String.IsNullOrWhiteSpace(right.rightType)) this.CreationCheckBox.IsChecked = true;
                else if (right.rightType.Equals(RightType.VIEW.ToString())) this.ConsultationCheckBox.IsChecked = true;
                else if (right.rightType.Equals(RightType.EDIT.ToString())) this.EditionCheckBox.IsChecked = true;
                else this.CreationCheckBox.IsChecked = true;
            }
            else
            {
                foreach (RightField child in this.ChildrenFields)
                {
                    child.Select(right);
                }
            }
            throwHandler = true;
        }

        public void AddChild(Functionality functionality)
        {
            RightField field = new RightField(functionality);
            AddChild(field);
        }

        private void AddChild(RightField field)
        {
            this.ChidrenPanel.Children.Add(field);
            this.ChildrenFields.Add(field);
            field.RightSelected += OnChildSelected;

        }

        private void initComponents()
        {
            this.ChildrenFields = new List<RightField>(0);
            StackPanel editorPanel = new StackPanel();
            editorPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            if (this.Functionality.HasType(RightType.VIEW))
            {
                this.ConsultationCheckBox = new CheckBox();
                this.ConsultationCheckBox.Content = " ";
                this.ConsultationCheckBox.ToolTip = "Consultation";
                this.ConsultationCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                this.ConsultationCheckBox.Checked += OnChecked;
                this.ConsultationCheckBox.Unchecked += OnChecked;
                editorPanel.Children.Add(this.ConsultationCheckBox);
            }

            if (this.Functionality.HasType(RightType.EDIT))
            {
                this.EditionCheckBox = new CheckBox();
                this.EditionCheckBox.Content = " ";
                this.EditionCheckBox.ToolTip = "Edition";
                this.EditionCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                this.EditionCheckBox.Checked += OnChecked;
                this.EditionCheckBox.Unchecked += OnChecked;
                editorPanel.Children.Add(this.EditionCheckBox);
            }

            this.CreationCheckBox = new CheckBox();
            this.CreationCheckBox.ToolTip = "Creation";
            this.CreationCheckBox.Checked += OnChecked;
            this.CreationCheckBox.Unchecked += OnChecked;
            this.CreationCheckBox.Content = Functionality.Name;
            this.CreationCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
            editorPanel.Children.Add(this.CreationCheckBox);
            
            this.ChidrenPanel = new StackPanel();
            this.ChidrenPanel.Margin = new System.Windows.Thickness(30, 0, 0, 0);
            this.Children.Add(editorPanel);
            this.Children.Add(this.ChidrenPanel);

            throwHandler = true;
        }

        private void OnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            enableChildren(!(checkBox.IsChecked.HasValue && checkBox.IsChecked.Value));
            if (throwHandler && RightSelected != null)
            {
                bool selected = checkBox.IsChecked.Value;
                Right right = new Right(this.Functionality.Code);
                if (ConsultationCheckBox != null && checkBox.Equals(ConsultationCheckBox)) right.rightType = RightType.VIEW.ToString();
                else if (EditionCheckBox != null && checkBox.Equals(EditionCheckBox)) right.rightType = RightType.EDIT.ToString();
                else if (this.Functionality.HasType(RightType.CREATE)) right.rightType = RightType.CREATE.ToString();                
                RightSelected(right, selected);
            }
        }

        private void OnChildSelected(Right right, bool selected)
        {
            if (throwHandler && RightSelected != null) RightSelected(right, selected);
        }

        protected void enableChildren(bool enable)
        {
            foreach (RightField child in this.ChildrenFields)
            {
                if (child.CreationCheckBox != null) child.CreationCheckBox.IsEnabled = enable;
                if (child.EditionCheckBox != null) child.EditionCheckBox.IsEnabled = enable;
                if (child.ConsultationCheckBox != null) child.ConsultationCheckBox.IsEnabled = enable;
                child.enableChildren(enable);
            }
        }

    }
}
