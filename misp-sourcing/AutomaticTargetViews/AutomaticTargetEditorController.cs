using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetEditorController : AutomaticSourcingEditorController
    {
        #region Editor and Service

        public override bool isAutomaticTarget()
        {
            return true;
        }
      

        /// <summary>
        /// Service pour acceder aux opérations liés à l'automaticSourcing.
        /// </summary>
        /// <returns>DesignService</returns>
        public AutomaticTargetService GetAutomaticTargetService()
        {
            return (AutomaticTargetService)base.Service;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public AutomaticTargetEditor getAutomaticTargetEditor()
        {
            return (AutomaticTargetEditor)this.View;
        }

        protected override Kernel.Domain.AutomaticSourcing GetObjectByName(string name)
        {
            return ((AutomaticTargetSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.getAutomaticSourcingByName(name);
        }

        protected void onGroupFieldChange()
        {
            AutomaticTargetEditorItem page = (AutomaticTargetEditorItem)getAutomaticTargetEditor().getActivePage();
            string name = page.getAutomaticSourcingForm().AutomaticSourcingPanel.groupGroupField.textBox.Text;
            page.EditedObject.group = GetAutomaticSourcingService().GroupService.getGroupByName(name);
            ((AutomaticTargetSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.updateAutomaticSourcing(name, page.Title, true);
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            AutomaticSourcingToolBar toolBar = new AutomaticSourcingToolBar();
            toolBar.RunButton.ToolTip = "Run Automatic Target";
            toolBar.SaveButton.ToolTip = "Save Automatic Target";
            toolBar.CloseButton.ToolTip = "Exit Automatic Target Editor";
            return toolBar;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new AutomaticTargetEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new AutomaticTargetSideBar(); }


        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_TARGET;
        }

        #endregion


         protected override Kernel.Domain.AutomaticSourcing GetNewAutomaticSourcing()
        {
            Kernel.Domain.AutomaticSourcing automaticTarget = new Kernel.Domain.AutomaticSourcing();
            automaticTarget.name = getNewPageName("Automatic Target");
            automaticTarget.isTarget = true;
            automaticTarget.group = GetAutomaticTargetService().GroupService.getDefaultGroup();
            return automaticTarget;
        }


         protected override void UpdateAutomaticSourcingSidebarName(string newName, string tableName, bool updateGroup)
         {
             ((AutomaticTargetSideBar)SideBar).AutomaticTargetGroup.AutomaticSourcingTreeview.updateAutomaticSourcing(newName, tableName, updateGroup);
         }

       
    }
}
