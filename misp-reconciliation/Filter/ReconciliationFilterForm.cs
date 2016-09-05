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

        public GrilleBrowserForm leftGrilleBrowserForm;
        public GrilleBrowserForm rigthGrilleBrowserForm;


        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.GridForm.filterForm.RecoPanel.Visibility = Visibility.Visible;
            //GridForm.Children.Remove(GridForm.splitter);

            this.AuditTabItem.Content = null;
            

            leftGrilleBrowserForm = new GrilleBrowserForm();
            leftGrilleBrowserForm.filterForm.Margin = new Thickness(2.0);
            Grid.SetRow(leftGrilleBrowserForm.filterForm, 0);
            leftGrilleBrowserForm.Children.Add(leftGrilleBrowserForm.filterForm);

            rigthGrilleBrowserForm = new GrilleBrowserForm();
            rigthGrilleBrowserForm.filterForm.Margin = new Thickness(2.0);
            Grid.SetRow(rigthGrilleBrowserForm.filterForm, 0);
            rigthGrilleBrowserForm.Children.Add(rigthGrilleBrowserForm.filterForm);

            GridSplitter splitter = new GridSplitter();
            splitter.ResizeDirection = GridResizeDirection.Columns;
            splitter.Width = 5.0;
            splitter.Background = Brushes.Gray;
            splitter.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            splitter.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            //StackPanel filterPnel = new StackPanel();
            //filterPnel.Orientation = Orientation.Horizontal;
            //filterPnel.Children.Add(leftGrilleBrowserForm);
            //filterPnel.Children.Add(splitter);
            //filterPnel.Children.Add(rigthGrilleBrowserForm);

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

            StackPanel panel = new StackPanel();
            panel.Children.Add(filterPnel);
            panel.Children.Add(GridForm);

            ScrollViewer viewer = new ScrollViewer();
            viewer.Content = panel;
            this.AuditTabItem.Content = viewer;            
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
