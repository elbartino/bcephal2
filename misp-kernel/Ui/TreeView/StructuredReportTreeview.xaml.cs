using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using System.Collections.ObjectModel;
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
    /// Interaction logic for StructuredReportTreeview.xaml
    /// </summary>
    public partial class StructuredReportTreeview : UserControl
    {
        public ObservableCollection<BrowserData> liste = new ObservableCollection<BrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();

        public StructuredReport StructuredReport { get; set; }

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
        public StructuredReportTreeview()
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
        public void updateStructuredReport(string newName, string oldTableName, bool updateGroup)
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
        /// Rajoute un retport
        /// </summary>
        /// <param name="inputTable">L'design à modifier</param>
        public void AddStructuredReport(StructuredReport retport) 
        {
            BrowserData data = new BrowserData();
            if (retport.oid.HasValue) data.oid = retport.oid.Value;
            data.name = retport.name;
            data.group = retport.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire un retport de la liste
        /// </summary>
        /// <param name="inputTable">L'retport à modifier</param>
        public void RemoveStructuredReport(StructuredReport retport)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == retport.name)
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
        public StructuredReport getStructuredReportByName(string designName)
        {
            StructuredReport design = new StructuredReport();
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
            if (StructuredReportTree.SelectedItem != null && StructuredReportTree.SelectedItem is BrowserData && SelectionChanged != null)
            {
                StructuredReport design = new StructuredReport();
                design.name = ((BrowserData)StructuredReportTree.SelectedItem).name;
                design.oid = ((BrowserData)StructuredReportTree.SelectedItem).oid;
                SelectionChanged(design);
                e.Handled = true;
            }
        }
    }
}
