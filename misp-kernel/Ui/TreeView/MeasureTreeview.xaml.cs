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
using System.Windows.Threading;

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for MeasureTreeview.xaml
    /// </summary>
    public partial class MeasureTreeview : UserControl
    {
        private static String DefaultCalculatedMeasureName = "CALCULATED MEASURES";
        public ObservableCollection<Object> liste = new ObservableCollection<Object>();
        private CollectionViewSource cvs = new CollectionViewSource();
        private DispatcherTimer myClickWaitTimer;
        private bool isDoubleClick = false;
        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }    
        /// <summary>
        /// Evènement du MeasureTreeview qui renvoit la measure selectionnée
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du MeasureTreeview qui renvoit la measure sur laquelle on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        /// <summary>
        /// Contruit une nouvelle instance de MeasureTreeview
        /// </summary>
        public MeasureTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.DataContext = this;
            myClickWaitTimer = new DispatcherTimer(new TimeSpan(0, 0, 0,0 ,1), DispatcherPriority.Background, mouseWaitTimer_Tick, Dispatcher.CurrentDispatcher);
            myClickWaitTimer.Stop();
        }

        public void setDisplacherInterval(TimeSpan timeSpan)
        {
            myClickWaitTimer.Interval = timeSpan;
        }

        private void mouseWaitTimer_Tick(object sender, EventArgs e)
        {
            myClickWaitTimer.Stop();
            if (!isDoubleClick)
            {
                clickMethod();
            }
            isDoubleClick = false;
        }

        private void clickMethod()
        {
            if (measureTreeview.SelectedItem != null && measureTreeview.SelectedItem is Kernel.Domain.Measure && SelectionChanged != null)
            {
                SelectionChanged(measureTreeview.SelectedItem as Kernel.Domain.Measure);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(Kernel.Domain.Measure root)
        {
            if (root == null) this.measureTreeview.ItemsSource = null;
            else
            {
                //RefreshParent(root);
                this.measureTreeview.ItemsSource = root.GetItems();
            }
        }

        public void DisplayRoot(Kernel.Domain.Measure root, List<CalculatedMeasure> CalculatedMeasures)
        {
            if (root == null) this.measureTreeview.ItemsSource = null;
            else if(CalculatedMeasures != null && CalculatedMeasures.Count > 0)
        {
                Kernel.Domain.Measure calMeasure = new Domain.Measure() { name = DefaultCalculatedMeasureName };
                foreach(Kernel.Domain.Measure measure in CalculatedMeasures){
                    calMeasure.childrenListChangeHandler.Items.Add(measure);
        }
                root.childrenListChangeHandler.Items.Add(calMeasure);
            }
            DisplayRoot(root);
        }

        public void updateCalculatedMeasure(string newName, string oldTableName, bool updateGroup)
        {
            for (int i = this.liste.Count - 1; i >= 0; i--)
            {
                if (this.liste[i] is Kernel.Domain.CalculatedMeasure)
                {
                    var calculatedMeasure = this.liste[i] as Kernel.Domain.CalculatedMeasure;
                    if (calculatedMeasure.name == newName)
                    {
                        calculatedMeasure.name = !updateGroup ? newName : calculatedMeasure.name;
                        if (calculatedMeasure.group != null) calculatedMeasure.group.name = updateGroup ? newName : calculatedMeasure.group.name;
                        this.liste[i] = calculatedMeasure;
                        this.cvs.DeferRefresh();
                    }
                }
            }
         }

        public void AddOrUpdateCalculateMeasure(Kernel.Domain.CalculatedMeasure calculateMeasure) 
        {
            for (int i = 0; i <= this.liste.Count-1 ; i++) 
            {
                if (this.liste[i] is Kernel.Domain.CalculatedMeasure)
                {
                    var calculateM = this.liste[i] as CalculatedMeasure;
                    if (calculateM.name == calculateMeasure.name)
                    {
                        this.liste[i] = calculateM;
                        this.cvs.DeferRefresh();
                        return;
                    }
                }
            }
            this.liste.Add(calculateMeasure);
            this.cvs.DeferRefresh();
        }

        private void RefreshParent(IHierarchyObject item)
        {
            if (item != null)
            {
                foreach (IHierarchyObject child in item.GetItems())
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
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
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
                myClickWaitTimer.Start();
            }
        }

        private void OnTreeNodeDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
                // Stop the timer from ticking.
                myClickWaitTimer.Stop();

                isDoubleClick = true;
                e.Handled = true;
                if (measureTreeview.SelectedItem != null && measureTreeview.SelectedItem is Kernel.Domain.Measure && SelectionDoubleClick != null)
                {
                    SelectionDoubleClick(measureTreeview.SelectedItem as Kernel.Domain.Measure);
                    e.Handled = true;
                }
            }
        }



     }
}
