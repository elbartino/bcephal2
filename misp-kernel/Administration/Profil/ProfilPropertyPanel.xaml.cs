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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for ProfilPropertyPanel.xaml
    /// </summary>
    public partial class ProfilPropertyPanel : ScrollViewer
    {
        public ProfilPropertyPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// display Profil property
        /// </summary>
        /// <param name="profil"></param>
        public void displayProfil(Domain.Profil profil)
        {
            if (profil == null) return;
            nameTextBox.Text = profil.name;
            groupField.Group = profil.group;
            visibleInShortcutCheckBox.IsChecked = profil.visibleInShortcut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profil"></param>
        public void fillProfil(Domain.Profil profil)
        {
            if (profil == null) return;
            profil.name = nameTextBox.Text;
            if (groupField.Group != null)
            {
                groupField.Group.subjectType = Kernel.Domain.SubjectType.PROFIL.label;
                profil.group = groupField.Group;
            }
            profil.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
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

        public bool validateEdition() 
        {
            return true;
        }
    }
}
