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

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for TargetTreeview.xaml
    /// </summary>
    public partial class TargetTreeview : UserControl
    {

        /// <summary>
        /// Evènement du TargetTreeview qui renvoit le target selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du TargetTreeview qui renvoit le target sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        public TargetTreeview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targets"></param>
        public void DisplayTargets(List<Target> targets)
        {
            this.TargetTree.ItemsSource = targets;
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
            if (TargetTree.SelectedItem != null && TargetTree.SelectedItem is Target && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(TargetTree.SelectedItem as Target);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(TargetTree.SelectedItem as Target);    
                e.Handled = true;
            }
        }
        
    }
}

