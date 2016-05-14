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

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for PeriodicityTreeview.xaml
    /// </summary>
    public partial class PeriodicityTreeview : UserControl
    {
        
        /// <summary>
        /// Evènement du PeriodicityTreeview qui renvoit la periodicity selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        public Base.ChangeEventHandler Changed;


        /// <summary>
        /// Evènement du PeriodicityTreeview qui renvoit la periodicity sur laquelle on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;
        /// <summary>
        /// Contruit une nouvelle instance de MeasureTreeview
        /// </summary>
        public PeriodicityTreeview()
        {
            InitializeComponent();
          //  InitializeDataTemplate();
            InitializeContextMenu();
        }

        protected virtual void InitializeContextMenu()
        {
           // this.viewFromToDateMenuItem..Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.viewFromToDateMenuItem.Click += new RoutedEventHandler(OnviewFromToDate);
            this.renameMenuItem.Click += new RoutedEventHandler(OnRenameItem);
            this.deleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.deleteMenuItem.Click += new RoutedEventHandler(OnDeleteItem);
        }

        private void OnDeleteItem(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodInterval selectedItem = GetSelectedPeriodInterval();
            DeleteNode(selectedItem);
        }

        private void OnRenameItem(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodInterval selectedItem = GetSelectedPeriodInterval();
            if (selectedItem == null) return;
            NamePanel namePanel = new NamePanel();
            namePanel.NameLabel.Visibility = System.Windows.Visibility.Visible;
            namePanel.NameTextBox.Visibility = System.Windows.Visibility.Visible;

            namePanel.fromDateLabel.Visibility = System.Windows.Visibility.Collapsed;
            namePanel.toDate.Visibility = System.Windows.Visibility.Collapsed;

            namePanel.toDateLabel.Visibility = System.Windows.Visibility.Collapsed;
            namePanel.toDate.Visibility = System.Windows.Visibility.Collapsed;

            namePanel.Height = 40;
            Dialog dialog = new Dialog(selectedItem.name, namePanel);
            dialog.Height = 150;
            dialog.Width = 300;
            bool result =(bool) dialog.ShowCenteredToMouse();
            if (result == true)
            {
               if (!String.IsNullOrWhiteSpace(namePanel.EditedName))
               {
                   PeriodInterval parent = selectedItem.parent;
                   selectedItem.name = namePanel.EditedName;
                   parent.UpdateChild(selectedItem);
                   
                   
               }
           }
        }

       
       

        private void OnviewFromToDate(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodInterval selectedItem = GetSelectedPeriodInterval();
            if (selectedItem == null) return;

            NamePanel namePanel = new NamePanel();
            namePanel.NameLabel.Visibility = System.Windows.Visibility.Collapsed;
            namePanel.NameTextBox.Visibility = System.Windows.Visibility.Collapsed;
           
            namePanel.fromDateLabel.Visibility = System.Windows.Visibility.Visible;
            namePanel.FromDate.Visibility = System.Windows.Visibility.Visible;

            namePanel.toDate.Visibility = System.Windows.Visibility.Visible;
            namePanel.toDateLabel.Visibility = System.Windows.Visibility.Visible;

            namePanel.FromDate.Text = selectedItem.periodFromDateTime.ToShortDateString();
            namePanel.toDate.Text = selectedItem.periodToDateTime.ToShortDateString();

            namePanel.Height = 80;
            Dialog dialog = new Dialog(selectedItem.name, namePanel);
            dialog.cancelButton.Visibility = System.Windows.Visibility.Collapsed;
            dialog.Height = 150;
            dialog.Width = 300;
            dialog.ShowCenteredToMouse();
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            else
            {
                Kernel.Domain.PeriodInterval periodInterval = GetSelectedPeriodInterval();
                periodTreeview.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée</returns>
        public Kernel.Domain.PeriodInterval GetSelectedPeriodInterval()
        {
            return this.periodTreeview.SelectedItem != null ? this.periodTreeview.SelectedItem as Kernel.Domain.PeriodInterval : null;
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        protected virtual void InitializeDataTemplate()
        {
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(Misp.Kernel.Domain.PeriodInterval));
            dataTemplate.ItemsSource = new Binding("childrenListChangeHandler.Items");

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding("name"));

            dataTemplate.VisualTree = factory;
            //this.tree.ItemTemplate = dataTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayPeriodicity(Kernel.Domain.Periodicity periodicity)
        {
            if (periodicity == null) this.periodTreeview.ItemsSource = null;
            else
            {
                this.periodTreeview.ItemsSource = periodicity.getHierarchicalPeriod();
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
            if (periodTreeview.SelectedItem != null && periodTreeview.SelectedItem is Kernel.Domain.PeriodInterval && SelectionChanged != null)
            {
                SelectionChanged(periodTreeview.SelectedItem as Kernel.Domain.PeriodInterval);
                e.Handled = true;
            }
        }

        public void DeleteNode(Kernel.Domain.PeriodInterval item)
        {
            if (item != null && item.parent != null)
            {
                int index = item.GetPosition();
                item.GetParent().RemoveChild(item);
                if (Changed != null) Changed();
            }
        }

        public void DisplayPeriodInterval(Domain.PeriodInterval root)
        {
            this.Root = root;
            if (this.Root == null) this.periodTreeview.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                this.periodTreeview.ItemsSource = this.Root.GetItems();
            }
        }

        Kernel.Domain.PeriodInterval Root;
      
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

    }

}
