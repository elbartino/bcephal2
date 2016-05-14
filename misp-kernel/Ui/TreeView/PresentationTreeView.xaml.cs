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
    /// Interaction logic for PresentationTreeView.xaml
    /// </summary>
    public partial class PresentationTreeView : UserControl
    {
       public ObservableCollection<BrowserData> liste = new ObservableCollection<BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();
        
        public Presentation presentation { get; set; }
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
        public PresentationTreeView()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group"));
            this.DataContext = this;
        }
        /// <summary>
        /// Remplir le treeview avec une liste de InputTable
        /// </summary>
        /// <param name="listeInputTable">la liste de InputTable</param>
        public void fillTree(ObservableCollection<BrowserData> listePresentation)
        {
            this.liste = listePresentation;
            this.cvs.Source = this.liste;
        }
 
      

        /// <summary>
        /// Met à jour un inputTable à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'inputTable</param>
        /// <param name="oldTableName">L'ancien nom de l'inputTable</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updatePresentation(string newName, string oldTableName, bool updateGroup)
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
        public void AddPresentation(Presentation presentation) 
        {
            BrowserData data = new BrowserData();
            if (presentation.oid.HasValue) data.oid = presentation.oid.Value;
            data.name = presentation.name;
            if (presentation.group != null)
            data.group = presentation.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        public void AddPresentationIfNatExist(Presentation presentation) 
        {
            if (getPresentationByName(presentation.name) != null) return;
            AddPresentation(presentation);
        }
        

        /// <summary>
        /// Retire un inputTable de la liste
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void RemovePresentation(Presentation presentation)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == presentation.name)
                {
                    this.liste.Remove(data);
                    this.cvs.DeferRefresh();
                    return;
                }
            }            
        }

        /// <summary>
        /// Retourne un inputTable à partir de son nom
        /// </summary>
        /// <param name="inputTableName">Le nom de l'inputTable</param>
        /// <returns>L'inputTable renvoyé</returns>
        public Presentation getPresentationByName(string inputTableName)
        {
            Presentation table = new Presentation();
            foreach (object obj in this.liste)
            {
                if (obj is Presentation)
                {
                    table.name = ((Presentation)obj).name;
                    table.oid = ((Presentation)obj).oid;
                }
                else if (obj is BrowserData)
                {
                    table.name = ((BrowserData)obj).name;
                    table.oid = ((BrowserData)obj).oid;
                }
                if (table.name == null) continue;
                if (table.name.ToUpper() == inputTableName.ToUpper()) return table; 
            }
            return null;
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
            if (presentationTree.SelectedItem != null && presentationTree.SelectedItem is BrowserData && SelectionChanged != null)
            {
                Presentation presentation = new Presentation();
                presentation.name = ((BrowserData)presentationTree.SelectedItem).name;
                presentation.oid = ((BrowserData)presentationTree.SelectedItem).oid;
                SelectionChanged(presentation);
                e.Handled = true;
            }
        }

        public void removePresentation(Presentation presentation)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == presentation.name)
                {
                    this.liste.Remove(data);
                    this.cvs.DeferRefresh();
                    return;
                }
            }            
         }
    }
}
