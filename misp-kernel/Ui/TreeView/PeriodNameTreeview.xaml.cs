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
    /// Interaction logic for PeriodNameTreeview.xaml
    /// </summary>
    public partial class PeriodNameTreeview : UserControl
    {
        /// <summary>
        /// Evènement du TagNameTreeview qui renvoit le Tag selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;


        /// <summary>
        /// Evènement du TagNameTreeview qui renvoit le tag sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        private DispatcherTimer myClickWaitTimer;
        private bool isDoubleClick = false;
        /// <summary>
        /// Contruit une nouvelle instance de TagTreeview
        /// </summary>
        public PeriodNameTreeview()
        {
            InitializeComponent();
            myClickWaitTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0,1), DispatcherPriority.Background, mouseWaitTimer_Tick, Dispatcher.CurrentDispatcher);
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
            if (periodNameTreeview.SelectedItem != null  && SelectionChanged != null)
            {
                if (periodNameTreeview.SelectedItem is Kernel.Domain.PeriodName)
                {
                    SelectionChanged(periodNameTreeview.SelectedItem as Kernel.Domain.PeriodName);
                }
                if (periodNameTreeview.SelectedItem is Kernel.Domain.PeriodInterval)
                {
                    SelectionChanged(periodNameTreeview.SelectedItem as Kernel.Domain.PeriodInterval);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayPeriods(List<Kernel.Domain.PeriodName> periods)
        {
            if (periods == null) this.periodNameTreeview.ItemsSource = null;
            else
            {
                foreach (Kernel.Domain.PeriodName periodName in periods)
                {
                    refreshPeriodInterval(periodName);
                }
                this.periodNameTreeview.ItemsSource = periods;
            }
        }

        public void DisplayPeriods(Kernel.Domain.PeriodName root)
        {
            if (root == null) this.periodNameTreeview.ItemsSource = null;
            else
            {
                DisplayPeriods(root.childrenListChangeHandler.getItems().ToList());
            }
        }

        public void refreshPeriodInterval(Kernel.Domain.PeriodName periodName) 
        {
            foreach (Kernel.Domain.PeriodInterval interval in periodName.intervalListChangeHandler.Items)
            {
                interval.periodName = periodName;
            }    
        }

        public void DisplayPeriodsWithIntervals(List<Kernel.Domain.PeriodName> periods, List<Kernel.Domain.PeriodInterval> listeIntervals) 
        {
            if (periods == null) this.periodNameTreeview.ItemsSource = null;
            else
            {

                this.periodNameTreeview.ItemsSource = periods;
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
                if (periodNameTreeview.SelectedItem != null && SelectionChanged != null)
                {
                   
                    myClickWaitTimer.Start();
                    e.Handled = true;
                }
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
                if (periodNameTreeview.SelectedItem != null && SelectionChanged != null)
                {
                    myClickWaitTimer.Stop();
                    isDoubleClick = true;

                    e.Handled = true;
                    if (periodNameTreeview.SelectedItem != null && SelectionDoubleClick != null)
                    {
                        if (periodNameTreeview.SelectedItem is Kernel.Domain.PeriodName)
                        {
                            SelectionDoubleClick(periodNameTreeview.SelectedItem as Kernel.Domain.PeriodName);
                        }
                        if (periodNameTreeview.SelectedItem is Kernel.Domain.PeriodInterval)
                        {
                            SelectionDoubleClick(periodNameTreeview.SelectedItem as Kernel.Domain.PeriodInterval);
                        }
                    }

                }
            }
        }

    }
}
