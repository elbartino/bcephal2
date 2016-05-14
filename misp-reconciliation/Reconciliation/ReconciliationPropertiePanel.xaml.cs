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

namespace Misp.Reconciliation.Reconciliation
{
    /// <summary>
    /// Interaction logic for ReconciliationPropertiePanel.xaml
    /// </summary>
    public partial class ReconciliationPropertiePanel : ScrollViewer
    {
        public ReconciliationPropertiePanel()
        {
            InitializeComponent();
        }

        

        /// <summary>
        /// display reconciliation property
        /// </summary>
        /// <param name="bReco"></param>
        public void displayReconciliation(ReconciliationTemplate bReco)
        {
            if (bReco == null) return;
            nameTextBox.Text = bReco.name;
            groupField.Group = bReco.group;
            visibleInShortcutCheckBox.IsChecked = bReco.visibleInShortcut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bReco"></param>
        public void fillReconciliation(ReconciliationTemplate bReco)
        {
            if (bReco == null) return;
            bReco.name = nameTextBox.Text;
            if (groupField.Group != null)
            {
                groupField.Group.subjectType = Kernel.Domain.SubjectType.RECONCILIATION.label;
                bReco.group = groupField.Group;
            }
            bReco.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
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
            return controls;
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }
    }
}
