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

namespace Misp.Planification.CombinedTransformationTree
{
    /// <summary>
    /// Interaction logic for TargetPropertiesPanel.xaml
    /// </summary>
    public partial class CombinedTransformationTreePropertiesPanel : ScrollViewer
    {
        public CombinedTransformationTreePropertiesPanel()
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
        public void displayCombinedTransformationTree(Kernel.Domain.CombinedTransformationTree combinedTransformationTree)
        {
            if (combinedTransformationTree == null) return;
            nameTextBox.Text = combinedTransformationTree.name;
            groupField.Group = combinedTransformationTree.group;
            visibleInShortcutCheckBox.IsChecked = combinedTransformationTree.visibleInShortcut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void fillCombinedTransformationTree(Kernel.Domain.CombinedTransformationTree combinedTransformationTree)
        {
            if (combinedTransformationTree == null) return;
            combinedTransformationTree.name = nameTextBox.Text;
            groupField.Group.subjectType = Kernel.Domain.SubjectType.COMBINED_TRANSFORMATION_TREE.label;
            combinedTransformationTree.group = groupField.Group;
            combinedTransformationTree.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
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
