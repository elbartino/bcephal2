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
        public CheckBox CheckBox { get; set; }
        public String Functionality { get; set; }
        public StackPanel ChidrenPanel { get; set; }

        public event RightEventHandler RightSelected;

        protected bool throwHandler;

        public RightField()
        {
            initComponents();
            initHandlers();
        }

        public RightField(String functionalityCode) : this()
        {
            this.Functionality = functionalityCode;
        }

        public RightField(String functionalityCode, String title)
            : this(functionalityCode)
        {
            this.CheckBox.Content = title;
        }

        public RightField(Functionality functionality)
            : this(functionality.Code, functionality.Name)
        {
            SetFunctionality(functionality);
        }

        public void SetFunctionality(Functionality functionality)
        {
            this.Functionality = functionality.Code;
            this.CheckBox.Content = functionality.Name;
            foreach (Functionality child in functionality.Children)
            {
                AddChild(child);
            }
        }

        public void Select(String functionalityCode)
        {
            throwHandler = false;
            if (this.Functionality.Equals(functionalityCode))
            {
                this.CheckBox.IsChecked = true;
            }
            else
            {
                foreach (RightField child in this.ChildrenFields)
                {
                    child.Select(functionalityCode);
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
            this.CheckBox = new CheckBox();
            this.CheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
            this.ChidrenPanel = new StackPanel();
            this.ChidrenPanel.Margin = new System.Windows.Thickness(50, 0, 0, 0);
            this.Children.Add(this.CheckBox);
            this.Children.Add(this.ChidrenPanel);
        }

        private void initHandlers()
        {
            this.CheckBox.Checked += OnChecked;
            this.CheckBox.Unchecked += OnChecked;
            throwHandler = true;
        }

        private void OnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            enableChildren(!(this.CheckBox.IsChecked.HasValue && this.CheckBox.IsChecked.Value));
            if (throwHandler && RightSelected != null) RightSelected(this.Functionality, this.CheckBox.IsChecked.Value);
        }

        private void OnChildSelected(string functionality, bool selected)
        {
            if (throwHandler && RightSelected != null) RightSelected(functionality, selected);
        }

        protected void enableChildren(bool enable)
        {
            foreach (RightField child in this.ChildrenFields)
            {
                child.CheckBox.IsEnabled = enable;
            }
        }

    }
}
