using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.LinkedAttribute
{
    public class LinkedAttributeGridBrowserController : BrowserController<LinkedAttributeGrid, BrowserData>
    {

        public LinkedAttributeGridBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.LINKED_ATTRIBUTE_GRID;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Sourcing.Base.SourcingFunctionalitiesCode.LINKED_ATTRIBUTE_GRID_EDIT ; }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new LinkedAttributeGridBrowser(this.SubjectType, this.FunctionalityCode); }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.LINKED_ATTRIBUTE_GRID;
        }

        protected override SideBar getNewSideBar() { return new SideBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Misp.Kernel.Ui.Base.ToolBar getNewToolBar() 
        {
            Misp.Kernel.Ui.Base.ToolBar toolbar = base.getNewToolBar();
            toolbar.Children.Remove(toolbar.NewButton);
            return toolbar; 
        }

        protected override void customizeContextMenu()
        {
            base.customizeContextMenu();
            this.GetBrowser().Form.Grid.View.RowCellMenuCustomizations.Remove(this.GetBrowser().Form.Grid.NewMenuItem);
            this.GetBrowser().Form.Grid.View.RowCellMenuCustomizations.Remove(this.GetBrowser().Form.Grid.DeleteMenuItem);
            this.GetBrowser().Form.Grid.View.RowCellMenuCustomizations.Remove(this.GetBrowser().Form.Grid.SaveAsMenuItem);
        }

    }
}
