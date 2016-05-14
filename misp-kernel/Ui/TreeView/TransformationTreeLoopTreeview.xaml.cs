using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
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
    /// Interaction logic for TransformationTreeLoopTreeview.xaml
    /// </summary>
    public partial class TransformationTreeLoopTreeview : UserControl
    {
        public ObservableCollection<TransformationTreeItem> liste = new ObservableCollection<TransformationTreeItem>();
        private CollectionViewSource cvs = new CollectionViewSource();

        //public Misp.Kernel.Domain.TransformationTreeLoop treeLoop { get; set; }

        /// <summary>
        /// Evènement du InputTableTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du InputTableTreeview qui renvoit l'inputTable sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }

        public TransformationTreeLoopTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.DataContext = this;
        }

        /// <summary>
        /// Remplir le treeview avec une liste de TreeLoop
        /// </summary>
        /// <param name="listeInputTable">la liste de InputTable</param>
        public void fillTree(ObservableCollection<TransformationTreeItem> listeTreeLoops)
        {
            this.LoopTree.ItemsSource = listeTreeLoops;
        }

        /// <summary>
        /// Rajoute une inputTable
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void AddTreeLoop(TransformationTreeItem treeLoop)
        {
            BrowserData data = new BrowserData();
            if (treeLoop.oid.HasValue) data.oid = treeLoop.oid.Value;
            data.name = treeLoop.name;
            this.liste.Add(treeLoop);
            this.cvs.DeferRefresh();
        }


        /// <summary>
        /// Retire un Design de la liste
        /// </summary>
        /// <param name="inputTable">L'Design à modifier</param>
        public void RemoveTreeLoop(TransformationTreeItem treeLoop)
        {
            foreach (TransformationTreeItem data in this.liste)
            {
                if (data.name == treeLoop.name)
                {
                    this.liste.Remove(data);
                    this.cvs.DeferRefresh();
                    return;
                }
            }
        }

        /// <summary>
        /// Retourne un Design à partir de son nom
        /// </summary>
        /// <param name="designName">Le nom de l'Design</param>
        /// <returns>L'Design renvoyé</returns>
        public TransformationTreeItem getTreeLoopByName(string treeLoopName)
        {            
            foreach (TransformationTreeItem obj in this.liste)
            {
                if (obj.name.ToUpper() == treeLoopName.ToUpper())
                {
                    TransformationTreeItem treeLoop = new TransformationTreeItem();
                    treeLoop.name = obj.name;
                    treeLoop.oid = obj.oid;
                    return treeLoop;
                }
            }
            return null;
        }

        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un design est transmis au InputTableTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (LoopTree.SelectedItem != null && LoopTree.SelectedItem is TransformationTreeItem && SelectionChanged != null)
            {
                SelectionChanged((TransformationTreeItem)LoopTree.SelectedItem);
                e.Handled = true;
            }
        }
    }
}
