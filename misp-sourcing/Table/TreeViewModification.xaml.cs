using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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


namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for TreeViewModification.xaml
    /// </summary>
    public partial class TreeViewModification : UserControl
    {
        public TreeViewModification()
        {
            InitializeComponent();
            myClickWaitTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.Background, mouseWaitTimer_Tick, Dispatcher.CurrentDispatcher);
            myClickWaitTimer.Stop();
        }

          /// <summary>
        /// Evènement du EntityTreeview qui renvoit l'entity selectionné
        /// </summary>
        public event Misp.Kernel.Ui.Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du EntityTreeview qui renvoit l'entity sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Misp.Kernel.Ui.Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        public ObservableCollection<Entity> liste = new ObservableCollection<Entity>();
        private DispatcherTimer myClickWaitTimer;
        private bool isDoubleClick = false;
        private CollectionViewSource cvs = new CollectionViewSource();
        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }
   
        private void RefreshAttribute(Kernel.Domain.Attribute attribute)
        {
            foreach (AttributeValue attributeValue in attribute.valueListChangeHandler.Items)
            {
                attributeValue.attribut = attribute;
                RefreshAttributeValue(attributeValue);
            }
        }

        public void DisplayAttributeWithValues(Kernel.Domain.Attribute attribute) {
            List<Kernel.Domain.Attribute> liste = new List<Kernel.Domain.Attribute>();
            RefreshAttribute(attribute);
            liste.Add(attribute);
            this.TreeViewModifications.ItemsSource = liste;
            
        }

        private void RefreshAttributeValue(AttributeValue value)
        {
            foreach (AttributeValue attributeValue in value.childrenListChangeHandler.Items)
            {
                attributeValue.parent = value;
                RefreshAttributeValue(attributeValue);
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
                List<object> listeToSende = new List<object>(0);
                listeToSende.Add(TreeViewModifications);
                listeToSende.Add(SelectionChanged);
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
                if (TreeViewModifications.SelectedItem != null && TreeViewModifications.SelectedItem is Target && SelectionDoubleClick != null)
                {
                    int count = e.ClickCount;
                    SelectionDoubleClick(TreeViewModifications.SelectedItem as Target);
                    e.Handled = true;
                }
            }            
        }

      
        
        private  void mouseWaitTimer_Tick(object sender, EventArgs e)
        {
            myClickWaitTimer.Stop();
            if (!isDoubleClick)
            {
                clickMethod();
            }
            isDoubleClick = false;
        }

        private  void clickMethod() 
        {

            if (TreeViewModifications.SelectedItem != null && TreeViewModifications.SelectedItem is Target && SelectionChanged != null)
                {
                    SelectionChanged(TreeViewModifications.SelectedItem as Target);
                    //e.Handled = true;
                }
        }
       

        private void OnMouseDownClickCount(object sender, MouseButtonEventArgs e)
        {
            // Checks the number of clicks. 
            if (e.ClickCount == 1)
            {
                // Single Click occurred.
                 string Content = "Single Click";
            }
            if (e.ClickCount == 2)
            {
                // Double Click occurred.
                string Content = "Double Click";
            }
            if (e.ClickCount >= 3)
            {
                // Triple Click occurred.
                string Content = "Triple Click";
            }
        }

        
    }
}
