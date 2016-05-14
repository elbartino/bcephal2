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
    /// Interaction logic for CustomizedTargetTreeview.xaml
    /// </summary>
    public partial class CombinedTransformationTreeTreeview : UserControl
    {
        public ObservableCollection<Kernel.Domain.Browser.BrowserData> liste = new ObservableCollection<Kernel.Domain.Browser.BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();
        
        public Target inputable { get; set; }
        /// <summary>
        /// Evènement du TargetTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du TargetTreeview qui renvoit l'inputTable sur lequel on
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
        public CombinedTransformationTreeTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("groupName"));
            this.DataContext = this;
        }

        /// <summary>
        /// Remplir le treeview avec une liste de Target
        /// </summary>
        /// <param name="listeTarget">la liste de Target</param>
        public void fillTree(ObservableCollection<Kernel.Domain.Browser.BrowserData> listeCombinedTransformationTrees)
        {
            this.liste = listeCombinedTransformationTrees;
            this.cvs.Source = this.liste;
        }

      
        /// <summary>
        /// Met à jour un inputTable à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'inputTable</param>
        /// <param name="oldTableName">L'ancien nom de l'inputTable</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updateTarget(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (Kernel.Domain.Browser.BrowserData inputtable in this.liste)
            {
                if (inputtable.name == oldTableName)
                {
                    inputtable.name = !updateGroup ? newName : inputtable.name;
                    if(inputtable.group != null) inputtable.group = updateGroup ? newName : inputtable.group;
                    this.liste[index] = inputtable;
                    this.cvs.DeferRefresh();
                    return;
                }
                index++;
            } 
        }

        public void updateTarget(Kernel.Domain.CombinedTransformationTree inputTable, Kernel.Domain.CombinedTransformationTree inpuTTable)
        {
            int index = 0;
            int pos=0;
            int pos1=0;
            bool found=false;
            bool found1=false;
            string newName = inputTable.name;
            string oldTableName = inpuTTable.name;

            Kernel.Domain.Browser.BrowserData input = null;
            foreach (Kernel.Domain.Browser.BrowserData inputtable in this.liste)
            {
               
                if (!found)
                {
                        
                    if (inputtable.name == inputTable.name)
                    {
                       inputtable.name = inpuTTable.name;
                       found = true;
                       input = inputtable;
                       pos = index;
                    }
                }
                if (!found1)
                {
                    if (inputtable.name == inpuTTable.name)
                    {
                        pos1 = index;
                        found1 = true;
                    }
                }
                index++;
            }
          
            this.liste[pos] = input;
            this.cvs.DeferRefresh();

        }

        public void AddTarget(CombinedTransformationTree inputTable)
        {
            Kernel.Domain.Browser.BrowserData data = new Kernel.Domain.Browser.BrowserData();
            if (inputTable.oid.HasValue) data.oid = inputTable.oid.Value;
            data.name = inputTable.name;
            if (inputTable.group != null)
                data.group = inputTable.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire un inputTable de la liste
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void RemoveTarget(Kernel.Domain.Browser.BrowserData inputTable)
        {
            this.liste.Remove(inputTable);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retourne un inputTable à partir de son nom
        /// </summary>
        /// <param name="inputTableName">Le nom de l'inputTable</param>
        /// <returns>L'inputTable renvoyé</returns>
        public CombinedTransformationTree getTargetByName(string inputTableName)
        {
            if (inputTableName == null) return null;
            CombinedTransformationTree table = new CombinedTransformationTree();
            foreach (object obj in this.liste)
            {
                if (obj is CombinedTransformationTree)
                {
                    table.name = ((CombinedTransformationTree)obj).name;
                    table.oid = ((CombinedTransformationTree)obj).oid;
                }
                else if (obj is Kernel.Domain.Browser.BrowserData)
                {
                    table.name = ((Kernel.Domain.Browser.BrowserData)obj).name;
                    table.oid = ((Kernel.Domain.Browser.BrowserData)obj).oid;
                }
                if (table.name.ToUpper() == inputTableName.ToUpper()) return table;
            }
            return null;
        }
        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un inputTable est transmis au TargetTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (CombinedTransformationTreeTree.SelectedItem != null && (CombinedTransformationTreeTree.SelectedItem is Kernel.Domain.CombinedTransformationTree || CombinedTransformationTreeTree.SelectedItem is Kernel.Domain.Browser.BrowserData) && SelectionChanged != null)
            {
                CombinedTransformationTree ctree = new CombinedTransformationTree();
                ctree.name = ((Kernel.Domain.Browser.BrowserData)CombinedTransformationTreeTree.SelectedItem).name;
                ctree.oid = ((Kernel.Domain.Browser.BrowserData)CombinedTransformationTreeTree.SelectedItem).oid;
                
                if (e.ClickCount == 1) SelectionChanged(ctree);
                else if (e.ClickCount == 2) SelectionChanged(ctree);
                e.Handled = true;
            }
        }
   }
}
