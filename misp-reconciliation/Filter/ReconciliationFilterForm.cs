using Misp.Kernel.Domain;
using Misp.Reconciliation.Posting;
using Misp.Sourcing.GridViews;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterForm : InputGridForm
    {
        public GrilleBrowserForm ActiveBrowserForm;
        public GrilleBrowserForm leftGrilleBrowserForm;
        public GrilleBrowserForm rigthGrilleBrowserForm;
        
        public PostingToolBar ActiveToolBar;
        public PostingToolBar leftPostingToolBar;
        public PostingToolBar rigthPostingToolBar;
        public PostingToolBar recoPostingToolBar;


        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.GridForm.filterForm.RecoPanel.Visibility = Visibility.Visible;
            //GridForm.Children.Remove(GridForm.splitter);
            this.GridForm.toolBar.Visibility = System.Windows.Visibility.Visible;

            leftPostingToolBar = new PostingToolBar();
            leftPostingToolBar.hideButtons();
            
            rigthPostingToolBar = new PostingToolBar();
            rigthPostingToolBar.hideButtons();

            recoPostingToolBar = new PostingToolBar();

            this.AuditTabItem.Content = null;            

            leftGrilleBrowserForm = new GrilleBrowserForm();
            leftGrilleBrowserForm.filterForm.Margin = new Thickness(2.0);
            //Grid.SetRow(leftGrilleBrowserForm.filterForm, 0);
            leftGrilleBrowserForm.Children.Add(leftGrilleBrowserForm.filterForm);
            leftGrilleBrowserForm.filterForm.RecoPanel.Visibility = System.Windows.Visibility.Visible;
            leftGrilleBrowserForm.toolBar.Visibility = System.Windows.Visibility.Visible;
            leftGrilleBrowserForm.otherToolBarPanel.Visibility = System.Windows.Visibility.Visible;
            leftGrilleBrowserForm.otherToolBarPanel.Children.Add(leftPostingToolBar);

            rigthGrilleBrowserForm = new GrilleBrowserForm();
            rigthGrilleBrowserForm.filterForm.Margin = new Thickness(2.0);
            //Grid.SetRow(rigthGrilleBrowserForm.filterForm, 0);
            rigthGrilleBrowserForm.Children.Add(rigthGrilleBrowserForm.filterForm);
            rigthGrilleBrowserForm.filterForm.RecoPanel.Visibility = System.Windows.Visibility.Visible;
            rigthGrilleBrowserForm.toolBar.Visibility = System.Windows.Visibility.Visible;
            rigthGrilleBrowserForm.otherToolBarPanel.Visibility = System.Windows.Visibility.Visible;
            rigthGrilleBrowserForm.otherToolBarPanel.Children.Add(rigthPostingToolBar);
            
            GridSplitter splitter = new GridSplitter();
            splitter.ResizeDirection = GridResizeDirection.Columns;
            splitter.Width = 5.0;
            splitter.Background = Brushes.Gray;
            splitter.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            splitter.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;


            GridForm.filterForm.Margin = new Thickness(12.0);
            Grid.SetRow(GridForm.filterForm, 1);
            GridForm.otherToolBarPanel.Visibility = System.Windows.Visibility.Visible;
            GridForm.otherToolBarPanel.Children.Add(recoPostingToolBar);
            GridForm.Children.Remove(GridForm.splitter);

            GridSplitter splitter1 = new GridSplitter();
            splitter1.ResizeDirection = GridResizeDirection.Rows;
            splitter1.Height = 5.0;
            splitter1.Background = Brushes.Gray;
            splitter1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            splitter1.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;


            Grid filterPnel = new Grid();
            filterPnel.ColumnDefinitions.Add(new ColumnDefinition());
            filterPnel.ColumnDefinitions.Add(new ColumnDefinition());
            filterPnel.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            filterPnel.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            Grid.SetColumn(rigthGrilleBrowserForm, 0);
            filterPnel.Children.Add(leftGrilleBrowserForm);
            Grid.SetColumn(splitter, 0);
            filterPnel.Children.Add(splitter);
            Grid.SetColumn(rigthGrilleBrowserForm, 1);            
            filterPnel.Children.Add(rigthGrilleBrowserForm);

            Grid panel = new Grid();
            panel.RowDefinitions.Add(new RowDefinition());
            panel.RowDefinitions.Add(new RowDefinition());
            panel.RowDefinitions[0].Height = new GridLength(300);
            panel.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            Grid.SetRow(filterPnel, 0);
            panel.Children.Add(filterPnel);
            Grid.SetRow(splitter1, 0);
            panel.Children.Add(splitter1);
            Grid.SetRow(GridForm, 1);
            panel.Children.Add(GridForm);

            ScrollViewer viewer = new ScrollViewer();
            viewer.Content = panel;
            this.AuditTabItem.Content = viewer;

            leftGrilleBrowserForm.GotFocus += OnLeftFocus;
            rigthGrilleBrowserForm.GotFocus += OnRigthFocus;

            this.ActiveBrowserForm = leftGrilleBrowserForm;

        }
        
        private void OnLeftFocus(object sender, RoutedEventArgs e)
        {
            this.ActiveBrowserForm = leftGrilleBrowserForm;
            this.ActiveToolBar = leftPostingToolBar;
        }

        private void OnRigthFocus(object sender, RoutedEventArgs e)
        {
            this.ActiveBrowserForm = rigthGrilleBrowserForm;
            this.ActiveToolBar = rigthPostingToolBar;
        }

        public override void SetTarget(Target target)
        {
            this.ActiveBrowserForm.filterForm.targetFilter.SetTargetValue(target);
        }

        public override void SetPeriodInterval(PeriodInterval interval)
        {
            this.ActiveBrowserForm.filterForm.periodFilter.SetPeriodInterval(interval);
        }

        public override void SetPeriodItemName(string name)
        {
            this.ActiveBrowserForm.filterForm.periodFilter.SetPeriodItemName(name);
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public override void fillObject()
        {
            base.fillObject();
            ((ReconciliationFilter)this.EditedObject).leftPostingFilter.FromGrilleFilter(this.leftGrilleBrowserForm.filterForm.GrilleFilter);
            ((ReconciliationFilter)this.EditedObject).rigthPostingFilter.FromGrilleFilter(this.rigthGrilleBrowserForm.filterForm.GrilleFilter);
            //this.leftGrilleBrowserForm.fillObject();
            //this.rigthGrilleBrowserForm.fillObject();
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public override void displayObject()
        {
            base.displayObject();
            
            this.leftGrilleBrowserForm.EditedObject = this.EditedObject;
            this.leftGrilleBrowserForm.filterForm.GrilleFilter = ((ReconciliationFilter)this.EditedObject).leftPostingFilter.ToGrilleFilter();
            this.leftGrilleBrowserForm.displayObject();

            this.rigthGrilleBrowserForm.EditedObject = this.EditedObject;
            this.rigthGrilleBrowserForm.filterForm.GrilleFilter = ((ReconciliationFilter)this.EditedObject).rigthPostingFilter.ToGrilleFilter();
            this.rigthGrilleBrowserForm.displayObject();
        }

    }
}
