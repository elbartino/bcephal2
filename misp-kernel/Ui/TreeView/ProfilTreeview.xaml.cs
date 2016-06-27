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
    /// Interaction logic for UserTreeview.xaml
    /// </summary>
    public partial class ProfilTreeview : UserControl
    {
        public ProfilTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group.name"));
            this.DataContext = this;
        }
        /// <summary>
        /// Evènement du ReconciliationTreeview qui renvoit le User selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du ReconciliationTreeview qui renvoit le User sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;
        private CollectionViewSource cvs = new CollectionViewSource();
        public ObservableCollection<Domain.Profil> liste = new ObservableCollection<Domain.Profil>();
        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }

        /// <summary>
        /// Remplir le treeview avec une liste de Profil
        /// </summary>
        /// <param name="listeProfils">la liste de Profil</param>
        public void fillTree(ObservableCollection<Domain.Profil> listeProfils)
        {
            this.liste = listeProfils;
            this.cvs.Source = this.liste;
        }

        /// <summary>
        /// Rajoute un profil
        /// </summary>
        /// <param name="inputTable">Le profil à modifier</param>
        public void AddProfil(Domain.Profil profil)
        {
            this.liste.Add(profil);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire un profil de la liste
        /// </summary>
        /// <param name="user">Le profil à modifier</param>
        public void RemoveProfil(Domain.Profil profil)
        {
            this.liste.Remove(profil);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retourne un profil à partir de son nom
        /// </summary>
        /// <param name="profilName">Le nom du profil</param>
        /// <returns>Le profil renvoyé</returns>
        public Domain.Profil getProfilByName(string profilName)
        {
            foreach (Domain.Profil profil in this.liste)
            {
                if (profil.name.ToUpper() == profilName.ToUpper()) return profil;
            }
            return null;
        }

        /// <summary>
        /// Met à jour un Profil à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de Profil</param>
        /// <param name="oldTableName">L'ancien nom de Profil</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de Profil</param>
        public void updateProfil(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (Domain.Profil profil in this.liste)
            {
                if (profil.name == oldTableName)
                {
                    profil.name = !updateGroup ? newName : profil.name;
                    //if (profil.group != null) profil.group.name = updateGroup ? newName : profil.group.name;
                    this.liste[index] = profil;
                    this.cvs.DeferRefresh();
                    return;
                }
                index++;
            }
        }

        public void updateProfil(Domain.Profil profil1, Domain.Profil profil)
        {
            int index = 0;
            int pos = 0;
            int pos1 = 0;
            bool found = false;
            bool found1 = false;
            string newName = profil1.name;
            string oldTableName = profil.name;

            Domain.Profil input = null;
            foreach (Domain.Profil inputtable in this.liste)
            {
                if (!found)
                {
                    if (inputtable.name == profil1.name)
                    {
                        inputtable.name = profil.name;
                        found = true;
                        input = inputtable;
                        pos = index;
                    }
                }
                if (!found1)
                {
                    if (inputtable.name == profil.name)
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
        public void DisplayProfils(List<Domain.Profil> users)
        {
            this.ProfilTree.ItemsSource = users;
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
            if (ProfilTree.SelectedItem != null && ProfilTree.SelectedItem is Domain.Profil && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(ProfilTree.SelectedItem as Domain.Profil);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(ProfilTree.SelectedItem as Domain.Profil);
                e.Handled = true;
            }
        }

    
    }
}
