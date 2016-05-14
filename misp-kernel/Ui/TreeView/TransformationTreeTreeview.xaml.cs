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
    /// Interaction logic for TransformationTreeTreeview.xaml
    /// </summary>
    public partial class TransformationTreeTreeview : UserControl
    {
        public TransformationTreeTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group"));
            this.DataContext = this;
        }
         public ObservableCollection<BrowserData> liste = new ObservableCollection<BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();

        public Misp.Kernel.Domain.TransformationTree transformationTree { get; set; }

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


        /// <summary>
        /// Remplir le treeview avec une liste de TreeLoop
        /// </summary>
        /// <param name="listeInputTable">la liste de InputTable</param>
        public void fillTree(ObservableCollection<BrowserData> listeTransformationTree)
        {
            this.liste = listeTransformationTree;
            this.cvs.Source = this.liste;
        }

        /// <summary>
        /// Met à jour un inputTable à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'inputTable</param>
        /// <param name="oldTableName">L'ancien nom de l'inputTable</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updateTransformationTree(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (BrowserData data in this.liste)
            {
                if (data.name == oldTableName)
                {
                    data.name = !updateGroup ? newName : data.name;
                    if (data.group != null) data.group = updateGroup ? newName : data.group;
                    this.liste[index] = data;
                    this.cvs.DeferRefresh();
                    return;
                }
                index++;
            }
        }

        /// <summary>
        /// Rajoute une inputTable
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void AddTransformationTree(TransformationTree transformationTree)
        {
            BrowserData data = new BrowserData();
            if (transformationTree.oid.HasValue) data.oid = transformationTree.oid.Value;
            data.name = transformationTree.name;
            if (transformationTree.group != null)
                data.group = transformationTree.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        public void AddTransformationTreeIfNatExist(TransformationTree tree)
        {
            if (getTransformationTreeByName(tree.name) != null) return;
            AddTransformationTree(tree);
        }

        /// <summary>
        /// Retire un Design de la liste
        /// </summary>
        /// <param name="inputTable">L'Design à modifier</param>
        public void RemoveTransformationTree(TransformationTree transformationTree)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == transformationTree.name)
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
        public TransformationTree getTreeLoopByName(string treeLoopName)
        {
            TransformationTree transformationTree = new TransformationTree();
            foreach (BrowserData obj in this.liste)
            {
                transformationTree.name = ((BrowserData)obj).name;
                transformationTree.oid = ((BrowserData)obj).oid;
                if (transformationTree.name.ToUpper() == treeLoopName.ToUpper()) return transformationTree;
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
            if (TransformationTreeTreeView.SelectedItem != null && TransformationTreeTreeView.SelectedItem is BrowserData && SelectionChanged != null)
            {
                TransformationTree treeLoop = new TransformationTree();
                treeLoop.name = ((BrowserData)TransformationTreeTreeView.SelectedItem).name;
                treeLoop.oid = ((BrowserData)TransformationTreeTreeView.SelectedItem).oid;
                SelectionChanged(treeLoop);
                e.Handled = true;
            }
        }


        /// <summary>
        /// Retourne un inputTable à partir de son nom
        /// </summary>
        /// <param name="inputTableName">Le nom de l'inputTable</param>
        /// <returns>L'inputTable renvoyé</returns>
        public TransformationTree getTransformationTreeByName(string inputTableName)
        {
            TransformationTree table = new TransformationTree();
            foreach (object obj in this.liste)
            {
                if (obj is TransformationTree)
                {
                    table.name = ((TransformationTree)obj).name;
                    table.oid = ((TransformationTree)obj).oid;
                }
                else if (obj is BrowserData)
                {
                    table.name = ((BrowserData)obj).name;
                    table.oid = ((BrowserData)obj).oid;
                }
                if (table.name.ToUpper() == inputTableName.ToUpper()) return table;
            }
            return null;
        }

    }
}
