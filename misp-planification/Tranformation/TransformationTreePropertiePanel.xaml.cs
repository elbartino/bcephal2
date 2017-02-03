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

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for TransformationTreePropertiePanel.xaml
    /// </summary>
    public partial class TransformationTreePropertiePanel : ScrollViewer
    {
        public TransformationTreePropertiePanel()
        {
            InitializeComponent();
            this.periodGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.nameTextBox.IsReadOnly = readOnly;
            visibleInShortcutCheckBox.IsEnabled = !readOnly;
            //this.groupField.SetReadOnly(readOnly);
        }

        public void fillTransformationTree(TransformationTree transformationTree)
        {
            if (transformationTree == null) return;
            transformationTree.name = nameTextBox.Text;
            if (groupField.Group != null)
            {
                groupField.Group.subjectType = Kernel.Domain.SubjectType.TRANSFORMATION_TREE.label;
                transformationTree.group = groupField.Group;
            }
            transformationTree.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
        }


        public void displayTransformationTree(TransformationTree transformationTree)
        {
            if (transformationTree == null) return;
            nameTextBox.Text = transformationTree.name;
            groupField.Group = transformationTree.group;
            visibleInShortcutCheckBox.IsChecked = transformationTree.visibleInShortcut;
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
            controls.Add(this.periodComboBox);
            return controls;
        }

    }
}
