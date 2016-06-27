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
    public partial class UserTreeview : UserControl
    {
        public UserTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group.name"));
            this.DataContext = this;
        }
        /// <summary>
        /// Evènement du UserTreeview qui renvoit le User selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du UserTreeview qui renvoit le User sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;
        private CollectionViewSource cvs = new CollectionViewSource();
        public ObservableCollection<Domain.User> liste = new ObservableCollection<Domain.User>();
        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }

        /// <summary>
        /// Remplir le treeview avec une liste de User
        /// </summary>
        /// <param name="listeTarget">la liste de User</param>
        public void fillTree(ObservableCollection<Domain.User> listeRecos)
        {
            this.liste = listeRecos;
            this.cvs.Source = this.liste;
        }

        /// <summary>
        /// Rajoute un User
        /// </summary>
        /// <param name="user">Le User à modifier</param>
        public void AddUser(Domain.User user)
        {
            this.liste.Add(user);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire un user de la liste
        /// </summary>
        /// <param name="user">Le User à modifier</param>
        public void RemoveUser(Domain.User user)
        {
            this.liste.Remove(user);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retourne un user à partir de son nom
        /// </summary>
        /// <param name="userName">Le nom du user</param>
        /// <returns>Le user renvoyé</returns>
        public Domain.User getUserByName(string userName)
        {
            foreach (Domain.User user in this.liste)
            {
                if (user.name.ToUpper() == userName.ToUpper()) return user;
            }
            return null;
        }

        /// <summary>
        /// Met à jour un User à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom du User</param>
        /// <param name="oldTableName">L'ancien nom du User</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom du User</param>
        public void updateUser(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (Domain.User user in this.liste)
            {
                if (user.name == oldTableName)
                {
                    user.name = !updateGroup ? newName : user.name;
                    if (user.profil != null)  user.profil.name = (updateGroup || user.profil == null)? newName : user.profil.name;
                    this.liste[index] = user;
                    this.cvs.DeferRefresh();
                    return;
                }
                index++;
            }
        }

        public void updateUser(Domain.User user1, Domain.User user)
        {
            int index = 0;
            int pos = 0;
            int pos1 = 0;
            bool found = false;
            bool found1 = false;
            string newName = user1.name;
            string oldTableName = user.name;

            Domain.User input = null;
            foreach (Domain.User inputtable in this.liste)
            {
                if (!found)
                {
                    if (inputtable.name == user1.name)
                    {
                        inputtable.name = user.name;
                        found = true;
                        input = inputtable;
                        pos = index;
                    }
                }
                if (!found1)
                {
                    if (inputtable.name == user.name)
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
        public void DisplayUsers(List<Domain.User> users)
        {
            this.UserTree.ItemsSource = users;
        }

        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un user  est transmis au Treeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (UserTree.SelectedItem != null && UserTree.SelectedItem is Domain.User && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(UserTree.SelectedItem as Domain.User);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(UserTree.SelectedItem as Domain.User);
                e.Handled = true;
            }
        }

    
    }
}
