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

namespace Misp.Sourcing.CustomizedTarget
{
    /// <summary>
    /// Interaction logic for TargetPropertiesPanel.xaml
    /// </summary>
    public partial class TargetPropertiesPanel : ScrollViewer
    {
        public TargetPropertiesPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void displayTarget(Target target)
        {
            if (target == null) return;
            nameTextBox.Text = target.name;
            groupField.Group = target.group;
            visibleInShortcutCheckBox.IsChecked = target.visibleInShortcut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void fillTarget(Target target)
        {
            if (target == null) return;
            target.name = nameTextBox.Text;
            groupField.Group.subjectType = Kernel.Domain.SubjectType.TARGET.label;
            target.group = groupField.Group;
            target.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            controls.Add(this.nameTextBox);
            controls.Add(this.groupField);
            controls.Add(this.visibleInShortcutCheckBox);
            return controls;
        }
        
    }
}
