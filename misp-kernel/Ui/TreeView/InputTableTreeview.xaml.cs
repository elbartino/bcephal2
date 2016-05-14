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
    /// Interaction logic for InputTableTreeview.xaml
    /// </summary>
    public partial class InputTableTreeview : UserControl
    {
        public ObservableCollection<InputTableBrowserData> liste = new ObservableCollection<InputTableBrowserData>();
        private CollectionViewSource cvs = new CollectionViewSource();
        
        public InputTable inputable { get; set; }
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
        public InputTableTreeview()
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
        public void fillTree(ObservableCollection<InputTableBrowserData> listeInputTable)
        {
            this.liste = listeInputTable;
            this.cvs.Source = this.liste;
        }
 
      
        /// <summary>
        /// Met à jour un inputTable à partir de son nom
        /// </summary>
        /// <param name="newName">Le nouveau nom de l'inputTable</param>
        /// <param name="oldTableName">L'ancien nom de l'inputTable</param>
        /// <param name="updateGroup">True=>Modification du nom du groupe, false=>Modification du nom de l'inputTable</param>
        public void updateInputTable(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;    
            foreach (InputTableBrowserData data in this.liste)
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
        public void AddInputTable(InputTable inputTable) 
        {
            InputTableBrowserData data = new InputTableBrowserData();
            if (inputTable.oid.HasValue) data.oid = inputTable.oid.Value;
            data.name = inputTable.name;
            if(inputTable.group != null)
            data.group = inputTable.group.name;
            this.liste.Add(data);
            this.cvs.DeferRefresh();
        }

        public void AddInputTableIfNatExist(InputTable inputTable) 
        {
            if (getInputTableByName(inputTable.name) != null) return;
            AddInputTable(inputTable);
        }
        

        /// <summary>
        /// Retire un inputTable de la liste
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void RemoveInputTable(InputTable inputTable)
        {
            foreach (InputTableBrowserData data in this.liste)
            {
                if (data.name == inputTable.name)
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
        public InputTable getInputTableByName(string inputTableName)
        {
            if (inputTableName == null) return null;
            InputTable table = new InputTable();
            foreach (object obj in this.liste)
            {
                if (obj is InputTable)
                {
                    table.name = ((InputTable)obj).name;
                    table.oid = ((InputTable)obj).oid;
                }
                else if (obj is InputTableBrowserData)
                {
                    table.name = ((InputTableBrowserData)obj).name;
                    table.oid = ((InputTableBrowserData)obj).oid;
                }
                if (table.name.ToUpper() == inputTableName.ToUpper()) return table; 
            }
            return null;
        }

        public InputTable getInputTableByName(string inputTableName,ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData> listeCurrent)
        {
            if(listeCurrent != null && listeCurrent.Count > 0) listeCurrent.ToList().AddRange(this.liste);
            else listeCurrent = this.liste;

            InputTable table = new InputTable();
            foreach (object obj in listeCurrent)
            {
                if (obj is InputTable)
                {
                    table.name = ((InputTable)obj).name;
                    table.oid = ((InputTable)obj).oid;
                }
                else if (obj is InputTableBrowserData)
                {
                    table.name = ((InputTableBrowserData)obj).name;
                    table.oid = ((InputTableBrowserData)obj).oid;
                }
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
            if (InputTableTree.SelectedItem != null && InputTableTree.SelectedItem is InputTableBrowserData && SelectionChanged != null)
            {
                InputTable table = new InputTable();
                table.name = ((InputTableBrowserData)InputTableTree.SelectedItem).name;
                table.oid = ((InputTableBrowserData)InputTableTree.SelectedItem).oid;
                SelectionChanged(table);
                e.Handled = true;
            }
        }
   }
}
