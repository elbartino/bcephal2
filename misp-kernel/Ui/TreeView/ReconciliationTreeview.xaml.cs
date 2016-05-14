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
    /// Interaction logic for ReconciliationTreeview.xaml
    /// </summary>
    public partial class ReconciliationTreeview : UserControl
    {

        /// <summary>
        /// Evènement du ReconciliationTreeview qui renvoit la reconciliation bancaire selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du ReconciliationTreeview qui renvoit la reconciliation bancaire sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;
        private CollectionViewSource cvs = new CollectionViewSource();
        public ObservableCollection<ReconciliationTemplate> liste = new ObservableCollection<ReconciliationTemplate>();
        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }

        public ReconciliationTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group.name"));
            this.DataContext = this;
        }

        /// <summary>
        /// Remplir le treeview avec une liste de reconciliation
        /// </summary>
        /// <param name="listeTarget">la liste de reconciliation</param>
        public void fillTree(ObservableCollection<ReconciliationTemplate> listeRecos)
        {
            this.liste = listeRecos;
            this.cvs.Source = this.liste;
        }

        /// <summary>
        /// Rajoute une reco
        /// </summary>
        /// <param name="inputTable">L'Reconciliation à modifier</param>
        public void AddReconciliation(ReconciliationTemplate reco)
        {
            this.liste.Add(reco);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire une reco de la liste
        /// </summary>
        /// <param name="reco">La reco à modifier</param>
        public void RemoveReconciliation(ReconciliationTemplate reco)
        {
            this.liste.Remove(reco);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retourne un reco à partir de son nom
        /// </summary>
        /// <param name="inputTableName">Le nom de la reco</param>
        /// <returns>La reco renvoyé</returns>
        public ReconciliationTemplate getReconciliationByName(string recoName)
        {
            foreach (ReconciliationTemplate reco in this.liste)
            {
                if (reco.name.ToUpper() == recoName.ToUpper()) return reco;
            }
            return null;
        }

        /// <summary>
        /// Met à jour un Reconciliation à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'Reconciliation</param>
        /// <param name="oldTableName">L'ancien nom de l'Reconciliation</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'Reconciliation</param>
        public void updateReconciliation(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (ReconciliationTemplate reCo in this.liste)
            {
                if (reCo.name == oldTableName)
                {
                    reCo.name = !updateGroup ? newName : reCo.name;
                    if (reCo.group != null) reCo.group.name = updateGroup ? newName : reCo.group.name;
                    this.liste[index] = reCo;
                    this.cvs.DeferRefresh();
                    return;
                }
                index++;
            }
        }

        public void updateReconciliation(ReconciliationTemplate reco1, ReconciliationTemplate reCo)
        {
            int index = 0;
            int pos = 0;
            int pos1 = 0;
            bool found = false;
            bool found1 = false;
            string newName = reco1.name;
            string oldTableName = reCo.name;

            ReconciliationTemplate input = null;
            foreach (ReconciliationTemplate inputtable in this.liste)
            {

                if (!found)
                {

                    if (inputtable.name == reco1.name)
                    {
                        inputtable.name = reCo.name;
                        found = true;
                        input = inputtable;
                        pos = index;
                    }
                }
                if (!found1)
                {
                    if (inputtable.name == reCo.name)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targets"></param>
        public void DisplayReconciliations(List<ReconciliationTemplate> recos)
        {
            this.ReconciliationTree.ItemsSource = recos;
        }

        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un reco  est transmis au Treeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (ReconciliationTree.SelectedItem != null && ReconciliationTree.SelectedItem is ReconciliationTemplate && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(ReconciliationTree.SelectedItem as ReconciliationTemplate);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(ReconciliationTree.SelectedItem as ReconciliationTemplate);
                e.Handled = true;
            }
        }

    }
}
