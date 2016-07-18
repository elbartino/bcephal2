using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for GrilleTreeview.xaml
    /// </summary>
    public partial class GrilleTreeview : UserControl
    {
        public ObservableCollection<BrowserData> liste = new ObservableCollection<BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();
     
        /// <summary>
        /// Evènement du GrilleTreeview qui renvoit la grille selectionnée
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du GrilleTreeview qui renvoit la grille sur lequel on
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
        public GrilleTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group"));
            this.DataContext = this;
        }
        /// <summary>
        /// Remplir le treeview avec une liste de grille
        /// </summary>
        /// <param name="listeGrille">la liste de grille</param>
        public void fillTree(ObservableCollection<BrowserData> listeGrille)
        {
            this.liste = listeGrille;
            this.cvs.Source = this.liste;
        }
 
      
        /// <summary>
        /// Met à jour une grille à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de la grille</param>
        /// <param name="oldGrilleName">L'ancien nom de la grille</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updateGrille(string newName, string oldGrilleName, bool updateGroup)
        {
            int index = 0;    
            foreach (BrowserData data in this.liste)
            {
                if (data.name == oldGrilleName)
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
        /// Rajoute une grille
        /// </summary>
        /// <param name="grille">La grille à modifier</param>
        public void AddGrille(Grille grille) 
        {
            BrowserData data = new BrowserData();
            if (grille.oid.HasValue) data.oid = grille.oid.Value;
            data.name = grille.name;
            if (grille.group != null)
                data.group = grille.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        public void AddGrilleIfNatExist(Grille grille) 
        {
            if (getGrilleByName(grille.name) != null) return;
            AddGrille(grille);
        }
        

        /// <summary>
        /// Retire un inputTable de la liste
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void RemoveGrille(Grille grille)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == grille.name)
                {
                    this.liste.Remove(data);
                    this.cvs.DeferRefresh();
                    return;
                }
            }            
        }

        /// <summary>
        /// Retourne une grille à partir de son nom
        /// </summary>
        /// <param name="grilleName">Le nom de la grille</param>
        /// <returns>La grille renvoyée</returns>
        public Grille getGrilleByName(string grilleName)
        {
            if (grilleName == null) return null;
            Grille grille = new Grille();
            foreach (object obj in this.liste)
            {
                if (obj is Grille)
                {
                    grille.name = ((Grille)obj).name;
                    grille.oid = ((Grille)obj).oid;
                }
                else if (obj is BrowserData)
                {
                    grille.name = ((BrowserData)obj).name;
                    grille.oid = ((BrowserData)obj).oid;
                }
                if (grille.name.ToUpper() == grilleName.ToUpper()) return grille; 
            }
            return null;
        }

        public Grille getGrilleByName(string grilleName, ObservableCollection<BrowserData> listeCurrent)
        {
            if(listeCurrent != null && listeCurrent.Count > 0) listeCurrent.ToList().AddRange(this.liste);
            else listeCurrent = this.liste;

            Grille grille = new Grille();
            foreach (object obj in listeCurrent)
            {
                if (obj is Grille)
                {
                    grille.name = ((Grille)obj).name;
                    grille.oid = ((Grille)obj).oid;
                }
                else if (obj is BrowserData)
                {
                    grille.name = ((BrowserData)obj).name;
                    grille.oid = ((BrowserData)obj).oid;
                }
                if (grille.name.ToUpper() == grilleName.ToUpper()) return grille;
            }
            return null;
        }
        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément une grille est transmise au GrilleTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (GrilleTree.SelectedItem != null && GrilleTree.SelectedItem is BrowserData && SelectionChanged != null)
            {
                Grille grille = new Grille();
                grille.name = ((BrowserData)GrilleTree.SelectedItem).name;
                grille.oid = ((BrowserData)GrilleTree.SelectedItem).oid;
                SelectionChanged(grille);
                e.Handled = true;
            }
        }
   }
}
