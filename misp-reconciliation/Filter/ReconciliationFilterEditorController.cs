using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Posting;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterEditorController : PostingGridEditorController
    {

        public override OperationState Create()
        {
            OperationState state = base.Create();
            ReconciliationFilterEditorItem page = (ReconciliationFilterEditorItem)getEditor().getActivePage();                        
            page.SearchAll();
            return state;
        }

        public override OperationState Open(Grille grid)
        {
            OperationState state = base.Open(grid);
            ReconciliationFilterEditorItem page = (ReconciliationFilterEditorItem)getEditor().getActivePage();
            page.SearchAll();
            return state;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputGrids.
        /// </summary>
        /// <returns>InputGridService</returns>
        public ReconciliationFilterService GetReconciliationFilterService()
        {
            return (ReconciliationFilterService)base.Service;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            ReconciliationFilterEditor editor = new ReconciliationFilterEditor();
            editor.Service = GetReconciliationFilterService();
            return editor;
        }

        protected override Grille GetNewGrid()
        {
            Grille grid = GetReconciliationFilterService().getNewReconciliationGrid("Filter");
            grid.columnListChangeHandler.Items = new ObservableCollection<GrilleColumn>(grid.columnListChangeHandler.newItems); 
            return grid;
        }

        protected override void initializeGridFormHandlers(InputGridForm inputGridForm)
        {
            ReconciliationFilterForm recoForm = (ReconciliationFilterForm)inputGridForm;
            
            recoForm.rigthGrilleBrowserForm.filterForm.searchButton.Click += OnSearchClick;
            recoForm.rigthGrilleBrowserForm.filterForm.resetButton.Click += OnResetClick;
            recoForm.rigthGrilleBrowserForm.filterForm.ChangeHandler += OnFilterChange;
            recoForm.rigthGrilleBrowserForm.toolBar.ChangeHandler += OnPageChange;

            recoForm.leftGrilleBrowserForm.filterForm.searchButton.Click += OnSearchClick;
            recoForm.leftGrilleBrowserForm.filterForm.resetButton.Click += OnResetClick;
            recoForm.leftGrilleBrowserForm.filterForm.ChangeHandler += OnFilterChange;
            recoForm.leftGrilleBrowserForm.toolBar.ChangeHandler += OnPageChange;
        }

        protected override void UpdateGridForm()
        {
            ReconciliationFilterEditorItem page = (ReconciliationFilterEditorItem)getInputGridEditor().getActivePage();
            page.getReconciliationFilterForm().EditedObject = page.EditedObject;
            page.getReconciliationFilterForm().displayObjectInGridForm();
            page.SearchAll();
        }

        protected override void OnSelectedTabChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender == null) return;
            if (e == null) return;
            if (!(e.Source is ReconciliationFilterForm)) return;
            ReconciliationFilterEditorItem page = (ReconciliationFilterEditorItem)getEditor().getActivePage();
            if (page.getReconciliationFilterForm().SelectedIndex == 1)
            {
                ((InputGridPropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel;
                ApplicationManager.MainWindow.displayPropertyBar(this.PropertyBar);
            }
            else
            {
                ApplicationManager.MainWindow.displayPropertyBar(null);
                if (page.getReconciliationFilterForm().GridForm.gridBrowser.RebuildGrid) UpdateGridForm();
            }
            e.Handled = true;
        }
        
        private void OnFilterChange()
        {
            Search();
            OnChange();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            ((ReconciliationFilterEditorItem)getEditor().getActivePage()).getReconciliationFilterForm().ActiveBrowserForm.filterForm.reset();
            Search();
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void OnPageChange(object item)
        {
            Search((int)item);
        }



    }
}
