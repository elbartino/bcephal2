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

namespace Misp.Kernel.Administration.User
{
    /// <summary>
    /// Interaction logic for UserPropertyPanel.xaml
    /// </summary>
    public partial class UserPropertyPanel : ScrollViewer
    {
        public UserPropertyPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// display User property
        /// </summary>
        /// <param name="bReco"></param>
        public void displayUser(Domain.User user)
        {
            if (user == null) return;
            nameTextBox.Text = user.name;
            //groupField.Group = user.group;
            //visibleInShortcutCheckBox.IsChecked = user.visibleInShortcut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bReco"></param>
        public void fillUser(Domain.User user)
        {
            if (user == null) return;
            user.name = nameTextBox.Text;
            if (groupField.Group != null)
            {
                groupField.Group.subjectType = Kernel.Domain.SubjectType.USER.label;
                //user.group = groupField.Group;
            }
            //user.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
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
