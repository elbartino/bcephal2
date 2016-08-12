using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class AutomaticPostingGridEditorController : AutomaticSourcingGridEditorController
    {

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            AutomaticSourcingToolBar toolBar = new AutomaticSourcingToolBar();
            toolBar.RunButton.ToolTip = "Run Automatic Posting Grid";
            toolBar.SaveButton.ToolTip = "Save Automatic Posting Grid";
            toolBar.CloseButton.ToolTip = "Exit Automatic Posting Grid";
            return toolBar;
        }

        protected override Kernel.Ui.Base.SideBar getNewSideBar()
        {
            AutomaticSourcingSideBar sideBar = new AutomaticSourcingSideBar();
            sideBar.AutomaticSourcingGroup.Header = "Automatic Posting Grids";
            return sideBar;
        }


        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_POSTING_GRID;
        }
        

        protected override Kernel.Domain.AutomaticSourcing GetNewAutomaticSourcing()
        {
            Kernel.Domain.AutomaticSourcing automaticGrid = new Kernel.Domain.AutomaticSourcing();
            automaticGrid.name = getNewPageName("Automatic Posting Grid");
            automaticGrid.isGrid = true;
            automaticGrid.isAutomaticGrid = true;
            automaticGrid.isPosting = true;
            automaticGrid.group = GetAutomaticSourcingGridService().GroupService.getDefaultGroup();
            return automaticGrid;
        }

        protected override void initializeSideBarData()
        {
            base.initializeSideBarData();
            ((AutomaticSourcingSideBar)SideBar).MeasureGroup.InitializeTreeViewDatas(true);
        }

    }
}
