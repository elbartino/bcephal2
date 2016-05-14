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
    /// Interaction logic for AutomaticSourcingTreeview.xaml
    /// </summary>
    public partial class AutomaticSourcingTreeview : UserControl
    {
        public ObservableCollection<Kernel.Domain.Browser.BrowserData> liste = new ObservableCollection<Kernel.Domain.Browser.BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();

        public AutomaticSourcing Design { get; set; }

        /// <summary>
        /// Evènement du InputTableTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }

        public AutomaticSourcingTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group"));
            this.DataContext = this;
        }

        /// <summary>
        /// Met à jour un inputTable à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'inputTable</param>
        /// <param name="oldTableName">L'ancien nom de l'inputTable</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updateAutomaticSourcing(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (Kernel.Domain.Browser.BrowserData data in this.liste)
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
        /// Remplir le treeview avec une liste de AutomaticSourcing
        /// </summary>
        /// <param name="listeInputTable">la liste de AutomaticSourcing</param>
        public void fillTree(ObservableCollection<Kernel.Domain.Browser.BrowserData> listeAutomaticSourcing)
        {
            this.liste = listeAutomaticSourcing;
            this.cvs.Source = this.liste;
        }

        /// <summary>
        /// Rajoute une automaticSourcing
        /// </summary>
        /// <param name="inputTable">L'automaticSourcing à modifier</param>
        public void AddAutomaticSourcing(AutomaticSourcing automaticSourcing)
        {
            Kernel.Domain.Browser.BrowserData data = new Kernel.Domain.Browser.BrowserData();
            if (automaticSourcing.oid.HasValue) data.oid = automaticSourcing.oid.Value;
            data.name = automaticSourcing.name;
            data.group = automaticSourcing.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }


        /// <summary>
        /// Retire un AutomaticSourcing de la liste
        /// </summary>
        /// <param name="inputTable">L'AutomaticSourcing à modifier</param>
        public void RemoveAutomaticSourcing(AutomaticSourcing automaticSourcing)
        {
            foreach (Kernel.Domain.Browser.BrowserData data in this.liste)
            {
                if (data.name == automaticSourcing.name)
                {
                    this.liste.Remove(data);
                    this.cvs.DeferRefresh();
                    return;
                }
            }
        }

        /// <summary>
        /// Retourne un AutomaticSourcing à partir de son nom
        /// </summary>
        /// <param name="inputTableName">Le nom de l'AutomaticSourcing</param>
        /// <returns>L'AutomaticSourcing renvoyé</returns>
        public AutomaticSourcing getAutomaticSourcingByName(string inputTableName)
        {
            AutomaticSourcing table = new AutomaticSourcing();
            foreach (object obj in this.liste)
            {
                if (obj is AutomaticSourcing)
                {
                    table.name = ((AutomaticSourcing)obj).name;
                    table.oid = ((AutomaticSourcing)obj).oid;
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
        /// et cet élément un design est transmis au InputTableTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (AutomaticSourcingTree.SelectedItem != null && (AutomaticSourcingTree.SelectedItem is AutomaticSourcing 
                || AutomaticSourcingTree.SelectedItem is Kernel.Domain.Browser.BrowserData) && SelectionChanged != null)
            {
                if (AutomaticSourcingTree.SelectedItem != null)
                {
                    if (AutomaticSourcingTree.SelectedItem is AutomaticSourcing)
                    {
                        SelectionChanged(AutomaticSourcingTree.SelectedItem as AutomaticSourcing);
                    }
                    else if (AutomaticSourcingTree.SelectedItem is Kernel.Domain.Browser.BrowserData)
                    {
                        SelectionChanged(AutomaticSourcingTree.SelectedItem as Kernel.Domain.Browser.BrowserData);
                    }

                    e.Handled = true;
                }
            }
        }
    }
}
