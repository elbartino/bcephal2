using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
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

namespace Misp.Kernel.Ui.ListView
{
    /// <summary>
    /// Interaction logic for PeriodListBox.xaml
    /// </summary>
    public partial class PeriodListBox : ScrollViewer
    {
        public event ChangeItemEventHandler ItemChanged;
        private Kernel.Service.PeriodNameService periodNameService;
        private bool throwEvent = true;
        public ChangeEventHandler Changed;
        public ObservableCollection<Kernel.Domain.PeriodName> liste = new ObservableCollection<Kernel.Domain.PeriodName>();
        private CollectionViewSource cvs = new CollectionViewSource();
        Dictionary<Kernel.Domain.PeriodName, int> selectedMeasures = new Dictionary<Domain.PeriodName, int>();
        
        public PeriodListBox()
        {
            InitializeComponent();
            this.periodNameService = Kernel.Application.ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetPeriodNameService();
            InitializeHandlers();
            initializeContexMenu();
        }

        /// <summary>
         /// 
         /// </summary>
         private void InitializeHandlers()
         {
             this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnItemClick);
         }

         private void OnItemClick(object sender, MouseButtonEventArgs e)
         {
             Kernel.Domain.PeriodName periodName;
             ListBoxItem listBoxItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

             if (listBoxItem != null)
             {
                 //SelectionManager(treeViewItem, false);
             }
             else
             {
                 periodName = GetSelectedPeriodName();
                 this.periodNameList.Focus();
             }
         }

         static ListBoxItem VisualUpwardSearch(DependencyObject source)
         {
             while (source != null && !(source is EO.Wpf.TreeViewItem))
                 source = VisualTreeHelper.GetParent(source);

             return source as ListBoxItem;
         }

         public Kernel.Domain.PeriodName GetSelectedPeriodName()
         {
             return this.periodNameList.SelectedItem != null ? this.periodNameList.SelectedItem as Kernel.Domain.PeriodName : null;
         }
      
        public void initializeContexMenu()
        {
            this.newMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("/bcephal;Component/Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.newMenuItem.Click += new RoutedEventHandler(OnNewPeriodName);
            this.deleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("/bcephal;Component/Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.deleteMenuItem.Click += new RoutedEventHandler(OnDeletePeriodName);

            this.moveUpMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("/bcephal;Component/Resources/Images/Icons/Moveup.png", UriKind.Relative)) };
            this.moveUpMenuItem.Click += new RoutedEventHandler(OnMoveUpPeriodName);

            this.moveDownMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("/bcephal;Component/Resources/Images/Icons/Movedown.png", UriKind.Relative)) };
            this.moveDownMenuItem.Click += new RoutedEventHandler(OnMoveDownPeriodName);
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            bool enabled = false;
            Kernel.Domain.PeriodName selecteItem = GetSelectedPeriodName();
            int size = this.periodNameList.Items.Count-1;
            enabled = selecteItem != null ;

            this.moveUpMenuItem.IsEnabled = enabled && selecteItem.position != 0;
            this.moveDownMenuItem.IsEnabled = enabled && selecteItem.position < size;
            this.deleteMenuItem.IsEnabled = enabled && !selecteItem.iDateDefault;
        }

        private void OnMoveDownPeriodName(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodName selecteItem = GetSelectedPeriodName();
            if (selecteItem == null) return;
            List<Kernel.Domain.PeriodName> listeToDelete = this.periodNameList.SelectedItems.Cast<Kernel.Domain.PeriodName>().ToList();
            listeToDelete.BubbleSort();
            int firstIndex = listeToDelete[0].position;
            MoveNode(selecteItem, false);
            setSelectedItem(selecteItem.position);
        }

        private void OnMoveUpPeriodName(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodName selecteItem = GetSelectedPeriodName();
            if (selecteItem == null) return;
            List<Kernel.Domain.PeriodName> listeToDelete = this.periodNameList.SelectedItems.Cast<Kernel.Domain.PeriodName>().ToList();
            listeToDelete.BubbleSort();
            int firstIndex = listeToDelete[0].position;
            MoveNode(selecteItem, true);
            firstIndex = firstIndex - 1 > 0 ? firstIndex - 1 : 0;
            setSelectedItem(selecteItem.position);
        }

        private void OnDeletePeriodName(object sender, RoutedEventArgs e)
        {
            List<Kernel.Domain.PeriodName> listeToDelete = this.periodNameList.SelectedItems.Cast<Kernel.Domain.PeriodName>().ToList();
            listeToDelete.BubbleSort();
            int firstIndex = listeToDelete[0].position;
            foreach (Kernel.Domain.PeriodName periodName in listeToDelete) 
            {
                DeleteNode(periodName);
            }
            firstIndex = firstIndex -1 > 0 ? firstIndex -1 : 0;
            setSelectedItem(firstIndex);
        }

        public void OnNewPeriodName(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodName periodName = AddNode();
            if (Changed != null) Changed();
            this.DisplayRoot(this.Root, periodName.position);
            setSelectedItem(periodName.position);
        }


        public virtual Kernel.Domain.PeriodName AddNode(string name = "")
        {
            Kernel.Domain.PeriodName child = GetNewPeriodName();
            
            if (name != "") child.name = name;
            this.Root.AddChild(child);
            if (Changed != null) Changed();
            return child;
        }


        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(Kernel.Domain.PeriodName item)
        {
            if (item != null && item.parent != null)
            {
                int index = item.GetPosition();
                item.GetParent().RemoveChild(item);
                if (Changed != null) Changed();
            }
        }

        public void MoveNode(Kernel.Domain.PeriodName item, bool up)
        {
            if (item.parent != null)
            {
                int position = item.position + (up ? -1 : 1);
                IHierarchyObject child = item.parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(item.position);
                    item.parent.UpdateChild(child);
                    item.SetPosition(position);
                    item.parent.UpdateChild(item);
                    if (Changed != null) Changed();
                }
            }
        }

        public void setSelectedItem(Kernel.Domain.PeriodName periodName) 
        {
            if (this.periodNameList.Items.Count > 0)
            this.periodNameList.SelectedItem = this.periodNameList.Items.GetItemAt(periodName.position);
        }

        public void setSelectedItem(int position)
        {
            if(this.periodNameList.Items.Count > 0)
            this.periodNameList.SelectedItem = this.periodNameList.Items.GetItemAt(position);
        }

        public Kernel.Domain.PeriodName BuildNewPeriodName()
        {
            Kernel.Domain.PeriodName periodName = new Kernel.Domain.PeriodName();
            periodName.name = "Period";
            List<Kernel.Domain.PeriodName> periodNames = this.periodNameList.Items.Cast<Kernel.Domain.PeriodName>().ToList();
            if (periodNames != null)
            {
                Kernel.Domain.PeriodName p = null;
                int i = 1;
                do
                {
                    periodName.name = "Period" + i++;
                    p = getPeriodByName(periodName.name);
                }
                while (p != null);
            }
            else periodName.name = "Period1";
            return periodName;
        }

        protected Kernel.Domain.PeriodName getPeriodByName(string name)
        {
            List<Kernel.Domain.PeriodName> periodNames = this.periodNameList.Items.Cast<Kernel.Domain.PeriodName>().ToList();
            foreach (Kernel.Domain.PeriodName periodName in periodNames)
            {
                if (periodName.name.ToUpper().Equals(name.ToUpper()))
                    return periodName;
            }
            return null;

        }
        public Kernel.Domain.PeriodName Root { get; set; }

        public void DisplayRoot(Domain.PeriodName root,int? selectedIndex = null)
        {
            this.Root = root;
            if (this.Root == null) this.periodNameList.ItemsSource = null;
            else
            {
               RefreshParent(this.Root);
               this.periodNameList.ItemsSource = null;
               this.periodNameList.ItemsSource = this.Root.listePeriodNames.Items;
               int countItems = this.Root.listePeriodNames.Items.Count;
               selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
               if (countItems > 0 && selectedIndex != null ) this.periodNameList.SelectedItem = this.periodNameList.Items.GetItemAt(selectedIndex.Value);
            }
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

        protected Kernel.Domain.PeriodName GetNewPeriodName(Domain.PeriodName value = null)
        {
            Kernel.Domain.PeriodName periodname = new Kernel.Domain.PeriodName();
            periodname.name = "Period1";
            if (Root != null)
            {
                Kernel.Domain.PeriodName m = null;
                int i = 1;
                do
                {
                    periodname.name = "Period" + i++;
                    m = (Kernel.Domain.PeriodName)Root.GetChildByName(periodname.name);
                }
                while (m != null);
            }
            return periodname;
        }


    }   
}
