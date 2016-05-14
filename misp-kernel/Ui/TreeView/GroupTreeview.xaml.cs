using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for GroupTreeview.xaml
    /// </summary>
    public partial class GroupTreeview : UserControl
    {

        /// <summary>
        /// Evènement du GroupTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du GroupTreeview qui renvoit le groupe sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        public GroupTreeview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(Kernel.Domain.BGroup root)
        {
            if (root == null) this.GroupTree.ItemsSource = null;
            else
            {
                RefreshParent(root);
                root.name = "All Groups";
                List<BGroup> groups = new List<BGroup>(0);
                groups.Add(root);
                this.GroupTree.ItemsSource = groups;
                root.IsExpanded = true;
            }
        }

        private void RefreshParent(IHierarchyObject item)
        {
            if (item != null)
            {
                foreach (IHierarchyObject child in item.GetItems())
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }

        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un inputTable est transmis au InputTableTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (GroupTree.SelectedItem != null && GroupTree.SelectedItem is Kernel.Domain.BGroup && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(GroupTree.SelectedItem as Kernel.Domain.BGroup);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(GroupTree.SelectedItem as Kernel.Domain.BGroup);
                e.Handled = true;
            }
        }


    }
}
