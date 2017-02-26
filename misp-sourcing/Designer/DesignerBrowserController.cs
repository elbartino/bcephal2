using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Designer
{
    public class DesignerBrowserController : BrowserController<Misp.Kernel.Domain.Design, BrowserData>
    {

        public DesignerBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.DESIGN;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Sourcing.Base.SourcingFunctionalitiesCode.DESIGN_EDIT; }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new DesignerBrowser(this.SubjectType, this.FunctionalityCode); }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.DESIGN;
        }

        protected override OperationState EditProperty(BrowserData item, String header, Object value)
        {
            if (item == null || String.IsNullOrWhiteSpace(header) || value == null) return OperationState.STOP;
            if (header.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                return RenameItem(item, (String)value);
            }
            if (header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase))
            {
                BrowserData data = new BrowserData(item);
                data.visibleInShortcut = (bool)value;
                try
                {
                    item = Service.SaveBrowserData(data);
                    if (item == null) return OperationState.STOP;
                    item.visibleInShortcut = (bool)value;
                }
                catch (Misp.Kernel.Domain.BcephalException)
                {
                    DisplayError("Unable edit item", "Unable edit : " + item.name);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }


    }
}
