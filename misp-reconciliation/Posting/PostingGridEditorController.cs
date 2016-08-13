﻿using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.RecoGrid;
using Misp.Reporting.ReportGrid;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reconciliation.Posting
{
    public class PostingGridEditorController : ReportGridEditorController
    {

        public PostingGridEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        public override void Search(int currentPage = 0)
        {
            try
            {
                PostingGridEditorItem page = (PostingGridEditorItem)getEditor().getActivePage();
                page.Search(currentPage);
            }
            catch (ServiceExecption e) { }
        }
                
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            InputGridToolBar toolBar = new InputGridToolBar();
            toolBar.SaveButton.Visibility = Visibility.Visible;
            toolBar.LoadButton.Visibility = Visibility.Visible;
            toolBar.ClearButton.Visibility = Visibility.Visible;
            return toolBar;
        }

        protected override Grille GetNewGrid()
        {
            Grille grid = GetPostingGridService().getNewReconciliationGrid("Posting Grid");
            grid.report = false;
            grid.reconciliation = true;
            //grid.group = GetInputGridService().GroupService.getDefaultGroup();
            grid.visibleInShortcut = true;     
            return grid;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            PostingGridEditor editor = new PostingGridEditor();
            editor.Service = GetPostingGridService();
            return editor;
        }
        
        /// <summary>
        /// Service pour acceder aux opérations liés aux InputGrids.
        /// </summary>
        /// <returns>InputGridService</returns>
        public PostingGridService GetPostingGridService()
        {
            return (PostingGridService)base.Service;
        }

        

    }
}