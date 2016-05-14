using Misp.Kernel.Ui.Base;
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
    /// Interaction logic for OperationTreeview.xaml
    /// </summary>
    public partial class OperationTreeview : UserControl
    {
        // liste des éléments de operationTree
        public ObservableCollection<string> operationItems = new ObservableCollection<string>();
        private CollectionViewSource cvs = new CollectionViewSource();

        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du InputTableTreeview qui renvoit l'inputTable sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

       
        public OperationTreeview()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Remplir le treeview 
        /// </summary>
        /// <param name="listeInputTable">la liste de InputTable</param>
        public void fillTree(ObservableCollection<string> items)
        {
            if (items != null)
            {
                this.operationItems = items;
                this.operationTreeview.ItemsSource = this.operationItems;
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
            if (operationTreeview.SelectedItem != null  && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged((string)operationTreeview.SelectedItem);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick((string)operationTreeview.SelectedItem);
                e.Handled = true;
            }
        }
    }
}
