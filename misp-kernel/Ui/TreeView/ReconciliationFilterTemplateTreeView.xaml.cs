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
    /// Interaction logic for ReconciliationFilterTemplateTreeView.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateTreeView : UserControl
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

        public CollectionViewSource CVS { get { return this.cvs; } }

        public ReconciliationFilterTemplateTreeView()
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
        public void updateTemplate(string newName, string oldTemplateName, bool updateGroup)
        {
            int index = 0;    
            foreach (BrowserData data in this.liste)
            {
                if (data.name == oldTemplateName)
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
        public void AddTemplate(ReconciliationFilterTemplate template) 
        {
            BrowserData data = new BrowserData();
            if (template.oid.HasValue) data.oid = template.oid.Value;
            data.name = template.name;
            if (template.group != null)
                data.group = template.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        public void AddTemplateIfNatExist(ReconciliationFilterTemplate template) 
        {
            if (getTemplateByName(template.name) != null) return;
            AddTemplate(template);
        }
        

        /// <summary>
        /// Retire un inputTable de la liste
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void RemoveTemplate(ReconciliationFilterTemplate template)
        {
            foreach (BrowserData data in this.liste)
            {
                if (data.name == template.name)
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
        public ReconciliationFilterTemplate getTemplateByName(string templateName)
        {
            if (templateName == null) return null;
            ReconciliationFilterTemplate grille = new ReconciliationFilterTemplate();
            foreach (object obj in this.liste)
            {
                if (obj is ReconciliationFilterTemplate)
                {
                    grille.name = ((ReconciliationFilterTemplate)obj).name;
                    grille.oid = ((ReconciliationFilterTemplate)obj).oid;
                }
                else if (obj is BrowserData)
                {
                    grille.name = ((BrowserData)obj).name;
                    grille.oid = ((BrowserData)obj).oid;
                }
                if (grille.name.ToUpper() == templateName.ToUpper()) return grille; 
            }
            return null;
        }

        public ReconciliationFilterTemplate getGrilleByName(string templateName, ObservableCollection<BrowserData> listeCurrent)
        {
            if(listeCurrent != null && listeCurrent.Count > 0) listeCurrent.ToList().AddRange(this.liste);
            else listeCurrent = this.liste;

            ReconciliationFilterTemplate grille = new ReconciliationFilterTemplate();
            foreach (object obj in listeCurrent)
            {
                if (obj is ReconciliationFilterTemplate)
                {
                    grille.name = ((ReconciliationFilterTemplate)obj).name;
                    grille.oid = ((ReconciliationFilterTemplate)obj).oid;
                }
                else if (obj is BrowserData)
                {
                    grille.name = ((BrowserData)obj).name;
                    grille.oid = ((BrowserData)obj).oid;
                }
                if (grille.name.ToUpper() == templateName.ToUpper()) return grille;
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
            if (TemplateTree.SelectedItem != null && TemplateTree.SelectedItem is BrowserData && SelectionChanged != null)
            {
                ReconciliationFilterTemplate template = new ReconciliationFilterTemplate();
                template.name = ((BrowserData)TemplateTree.SelectedItem).name;
                template.oid = ((BrowserData)TemplateTree.SelectedItem).oid;
                SelectionChanged(template);
                e.Handled = true;
            }
        }
   }
}