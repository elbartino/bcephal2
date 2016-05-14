using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
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

namespace Misp.Kernel.Ui.PeriodicityTree
{
    /// <summary>
    /// Interaction logic for PeriodIntervalTreeView.xaml
    /// </summary>
    public partial class PeriodIntervalTreeView : ScrollViewer
    {
        /// <summary>
        /// Evènement du PeriodicityTreeview qui renvoit la periodicity selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        public Base.ChangeEventHandler Changed;

        public Base.ChangeItemEventHandler ItemChanged;


        /// <summary>
        /// Evènement du PeriodicityTreeview qui renvoit la periodicity sur laquelle on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;
        /// <summary>
        /// Contruit une nouvelle instance de MeasureTreeview
        /// </summary>
        /// 
        public PeriodIntervalTreeView()
        {
            InitializeComponent();
            renameMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            InitializeContextMenu();
            this.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        }

        protected virtual void InitializeContextMenu()
        {
           // this.viewFromToDateMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.viewFromToDateMenuItem.Click += new RoutedEventHandler(OnViewFromToDate);

            //this.renameMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Copy.png", UriKind.Relative)) };
            this.renameMenuItem.Click += new RoutedEventHandler(OnRename);

            this.deleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.deleteMenuItem.Click += new RoutedEventHandler(OnDeleteNode);
        }

        private void OnViewFromToDate(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodInterval selectedItem = GetSelectedPeriodInterval();
            EO.Wpf.TreeViewItem treeviewItem = VisualUpwardSearch(VisualUpwardSearch(e.OriginalSource as DependencyObject));
            if (selectedItem == null) return;
            viewFromToDate(selectedItem);
        }

        private void viewFromToDate(Kernel.Domain.PeriodInterval interval) 
        {
            NamePanel namePanel = new NamePanel();
            namePanel.gridDefault.Visibility = System.Windows.Visibility.Collapsed;
            namePanel.gridViewDate.Visibility = System.Windows.Visibility.Visible;
            namePanel.FromDate.Text = interval.periodFromDateTime.ToShortDateString();
            namePanel.toDate.Text = interval.periodToDateTime.ToShortDateString();
            namePanel.Height = 70;
            Dialog dialog = new Dialog(interval.name, namePanel);
            dialog.cancelButton.Visibility = System.Windows.Visibility.Collapsed;
            dialog.Height = 200;
            dialog.Width = 245;
            Kernel.Domain.PeriodInterval intervalPeriod = GetSelectedPeriodInterval();

            DateTime from = intervalPeriod.periodFromDateTime;
            DateTime to = intervalPeriod.periodToDateTime;
            dialog.ShowCenteredToMouse();
            intervalPeriod.periodTo = namePanel.toDate.ToString();
            intervalPeriod.periodFrom = namePanel.FromDate.ToString();

            if (intervalPeriod.periodToDateTime < intervalPeriod.periodFromDateTime) 
            {
                intervalPeriod.periodTo = to.ToShortDateString();
                intervalPeriod.periodFrom = from.ToShortDateString();
                namePanel.FromDate.Text = to.ToShortDateString();
                namePanel.toDate.Text = from.ToShortDateString();
                MessageDisplayer.DisplayInfo("Unable to change Period interval : "+intervalPeriod.name, "The Date To must be after the Date From");
                return;
            }

            if (from != interval.periodFromDateTime || to != interval.periodToDateTime)
            {
                intervalPeriod.periodFrom = interval.periodFrom;
                intervalPeriod.periodTo = interval.periodTo;
                interval.GetParent().UpdateChild(intervalPeriod);
                if (Changed != null) Changed();
            }
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            else
            {
                PeriodInterval period = GetSelectedPeriodInterval();
                //if (period != null) period.IsSelected = false;
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

        static EO.Wpf.TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is EO.Wpf.TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as EO.Wpf.TreeViewItem;
        }

        private void OnRename(object sender, RoutedEventArgs e)
        {
           Kernel.Domain.PeriodInterval selectedItem = GetSelectedPeriodInterval();
          
            //rename(selectedItem);
        }

      
        private void OnDeleteNode(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.PeriodInterval selectedItem = GetSelectedPeriodInterval();
            DeleteNode(selectedItem);
        }

        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
                if (e.Item is Kernel.Domain.PeriodInterval)
                {
                    Kernel.Domain.PeriodInterval periodInterval = (Kernel.Domain.PeriodInterval)e.Item;
                    e.Text = periodInterval.name;
                }
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            try
            {
                if (e.Item is Kernel.Domain.PeriodInterval)
                {
                    Kernel.Domain.PeriodInterval editedInterval = (Kernel.Domain.PeriodInterval)e.Item;
                    string name = e.Text.Trim();

                    Kernel.Domain.PeriodInterval ValidInterval  = ValidateName(editedInterval, name);
                    if (ValidInterval == null)
                    {
                        e.Canceled = true;
                        return;
                    }
                    editedInterval.name = ValidInterval.name;
                    editedInterval.GetParent().UpdateChild(editedInterval);
                    //if (ItemChanged != null) ItemChanged(editedInterval);
                    if (Changed != null) Changed();
                }
                

                //The event must be canceled, otherwise the TreeView will
                //set the TreeViewItem's Header to the new text
               // e.Canceled = true;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        /// <summary>
        /// Cette méthode permet de vérifier si un PeriodInterval de l'arbre possède un nom identique à celui donné. 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="name"></param>
        /// <returns>La attribute à copier</returns>
        private Kernel.Domain.PeriodInterval ValidateName(Kernel.Domain.PeriodInterval periodInterval, string name)
        {
            bool result = true;
            periodInterval.name = name;
            Kernel.Domain.PeriodInterval currentPeriodInterval = periodInterval.CloneObject() as Kernel.Domain.PeriodInterval;
            if (string.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Interval name", "Interval name can't be empty! ");
                result = false;
            }
            Kernel.Domain.PeriodInterval m = (Kernel.Domain.PeriodInterval)Root.GetNotEditedChildByName(periodInterval, name);
            if (m == null) return periodInterval;

            if ((m != null && !m.Equals(periodInterval)))
            {
                currentPeriodInterval = currentPeriodInterval.GetCopy() as Kernel.Domain.PeriodInterval;
                currentPeriodInterval = ValidateName(currentPeriodInterval, currentPeriodInterval.name);
            }
            if (result)
                return currentPeriodInterval;
            return null;
        }

        public void DeleteNode(Kernel.Domain.PeriodInterval item)
        {
            if (item != null && item.parent != null)
            {
                int index = item.GetPosition();
                item.GetParent().RemoveChild(item);
                //if (ItemChanged != null) ItemChanged(item);
                if (Changed != null) Changed();
            }
        }


        public void UpdateNode(Kernel.Domain.PeriodInterval item) 
        {
            if (item != null && item.parent != null) 
            {
                int index = item.GetPosition();
                if (Changed != null) Changed();
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

        public void DisplayPeriodInterval(Domain.PeriodInterval root)
        {
            this.Root = root;
            if (this.Root == null) this.periodTreeview.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                this.periodTreeview.ItemsSource = null;
                this.periodTreeview.ItemsSource = this.Root.GetItems();
            }
        }

        Kernel.Domain.PeriodInterval Root;

        private void RefreshParent(PeriodInterval item)
        {
            if (item != null)
            {
                foreach (PeriodInterval child in item.GetItems())
                {
                    child.SetParent(item);
                    child.IsExpanded = true;
                    RefreshParent(child);
                }
            }
        }
    }
}
