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
    /// Interaction logic for CalculatedMeasureTreeview.xaml
    /// </summary>
    public partial class CalculatedMeasureTreeview : UserControl
    {
        public ObservableCollection<CalculatedMeasure> liste = new ObservableCollection<CalculatedMeasure>();
        private CollectionViewSource cvs = new CollectionViewSource();

        /// <summary>
        /// Evènement du CalculatedMeasureTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du CalculatedMeasureTreeview qui renvoit l'inputTable sur lequel on
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
        public CalculatedMeasureTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("group.name"));
            this.DataContext = this;
        }

        

        public void fillTree(System.Collections.ObjectModel.ObservableCollection<Domain.CalculatedMeasure> listeCalculatedMeasure)
        {
            this.liste = listeCalculatedMeasure;
            this.cvs.Source = this.liste;
        }

        public void updateCalculatedMeasure(string newName, string oldTableName, bool updateGroup)
        {
            int index = 0;
            foreach (CalculatedMeasure calculatedMeasure in this.liste)
            {
                if (calculatedMeasure.name == oldTableName)
                {
                    calculatedMeasure.name = !updateGroup ? newName : calculatedMeasure.name;
                    if (calculatedMeasure.group != null) calculatedMeasure.group.name = updateGroup ? newName : calculatedMeasure.group.name;
                    this.liste[index] = calculatedMeasure;
                    this.cvs.DeferRefresh();
                    return;
                }
                index++;
            } 
        }

        public void updateCalculatedMeasure(CalculatedMeasure inputTable, CalculatedMeasure inpuTTable)
        {
            int index = 0;
            int pos = 0;
            int pos1 = 0;
            bool found = false;
            bool found1 = false;
            string newName = inputTable.name;
            string oldTableName = inpuTTable.name;

            CalculatedMeasure input = null;
            foreach (CalculatedMeasure inputtable in this.liste)
            {

                if (!found)
                {

                    if (inputtable.name == inputTable.name)
                    {
                        inputtable.name = inpuTTable.name;
                        found = true;
                        input = inputtable;
                        pos = index;
                    }
                }
                if (!found1)
                {
                    if (inputtable.name == inpuTTable.name)
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
        /// Rajoute une inputTable
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void AddCalculatedMeasure(CalculatedMeasure inputTable)
        {
            this.liste.Add(inputTable);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retire un inputTable de la liste
        /// </summary>
        /// <param name="inputTable">L'inputTable à modifier</param>
        public void RemoveCalculatedMeasure(CalculatedMeasure inputTable)
        {
            this.liste.Remove(inputTable);
            this.cvs.DeferRefresh();
        }

        /// <summary>
        /// Retourne un inputTable à partir de son nom
        /// </summary>
        /// <param name="inputTableName">Le nom de l'inputTable</param>
        /// <returns>L'inputTable renvoyé</returns>
        public CalculatedMeasure getCalculatedMeasureByName(string inputTableName)
        {
            foreach (CalculatedMeasure inputtable in this.liste)
            {
                if (inputtable.name.ToUpper() == inputTableName.ToUpper()) return inputtable;
            }
            return null;
        }
        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un inputTable est transmis au CalculatedMeasureTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (CalculatedMeasureTree.SelectedItem != null && CalculatedMeasureTree.SelectedItem is CalculatedMeasure && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(CalculatedMeasureTree.SelectedItem as CalculatedMeasure);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(CalculatedMeasureTree.SelectedItem as CalculatedMeasure);
                e.Handled = true;
            }
        }
    }
}
