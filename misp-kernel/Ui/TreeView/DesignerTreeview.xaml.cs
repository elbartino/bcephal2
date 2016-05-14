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
    /// Interaction logic for DesignerTreeview.xaml
    /// </summary>
    public partial class DesignerTreeview : UserControl
    {
        public ObservableCollection<BrowserData> liste = new ObservableCollection<BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();

        public Design Design { get; set; }

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
        public DesignerTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group"));
            this.DataContext = this;
        }
        /// <summary>
        /// Remplir le treeview avec une liste de Design
        /// </summary>
        /// <param name="Designs">la liste de Design</param>
        public void fillTree(ObservableCollection<BrowserData> designs)
        {
            this.liste = designs;
            this.cvs.Source = this.liste;
        }
 
      
        /// <summary>
        /// Met à jour un inputTable à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'inputTable</param>
        /// <param name="oldTableName">L'ancien nom de l'inputTable</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updateDesign(string newName, string oldTableName, bool updateGroup)
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
        /// Rajoute une design
        /// </summary>
        /// <param name="inputTable">L'design à modifier</param>
        public void AddDesign(Design design) 
        {
            BrowserData data = new BrowserData();
            if (design.oid.HasValue) data.oid = design.oid.Value;
            data.name = design.name;
            data.group = design.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire un Design de la liste
        /// </summary>
        /// <param name="inputTable">L'Design à modifier</param>
        public void RemoveDesign(Design design)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == design.name)
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
        public Design getDesignByName(string designName)
        {
            Design design = new Design();
            foreach (BrowserData obj in this.liste)
            {
                design.name = ((BrowserData)obj).name;
                design.oid = ((BrowserData)obj).oid;
                if (design.name.ToUpper() == designName.ToUpper()) return design;
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
            if (DesignTree.SelectedItem != null && DesignTree.SelectedItem is BrowserData && SelectionChanged != null)
            {
                Design design = new Design();
                design.name = ((BrowserData)DesignTree.SelectedItem).name;
                design.oid = ((BrowserData)DesignTree.SelectedItem).oid;
                SelectionChanged(design);
                e.Handled = true;
            }
        }
    }
}
