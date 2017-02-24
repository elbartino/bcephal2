using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Application;

namespace Misp.Sourcing.Table
{
    public class InputTableBrowserController : BrowserController<Misp.Kernel.Domain.InputTable, InputTableBrowserData>
    {

        public InputTableBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.INPUT_TABLE;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Sourcing.Base.SourcingFunctionalitiesCode.INPUT_TABLE_EDIT; }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new InputTableBrowser(this.SubjectType); }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
        }
        
        protected override OperationState EditProperty(InputTableBrowserData item, String header, Object value)
        {
            if (item == null || String.IsNullOrWhiteSpace(header) || value == null) return OperationState.STOP;
            if (header.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Kernel.Util.FileUtil.isValidFileName((String)value))
                    return RenameItem(item, (String)value);
                else
                {
                    DisplayError("Unable edit item", "Invalid File name.");
                    return OperationState.STOP;
                }
            }
            if (header.Equals("Active", StringComparison.InvariantCultureIgnoreCase)
                || header.Equals("Template", StringComparison.InvariantCultureIgnoreCase)
                || header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase))
            {
                InputTableBrowserData data = new InputTableBrowserData(item);
                if (header.Equals("Active", StringComparison.InvariantCultureIgnoreCase)) data.active = (bool)value;
                if (header.Equals("Template", StringComparison.InvariantCultureIgnoreCase)) data.template = (bool)value;
                if (header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase)) data.visibleInShortcut = (bool)value;
                try
                {
                    item = Service.SaveBrowserData(data);
                    if (item == null) return OperationState.STOP;
                    if (header.Equals("Active", StringComparison.InvariantCultureIgnoreCase)) item.active = (bool)value;
                    if (header.Equals("Template", StringComparison.InvariantCultureIgnoreCase)) item.template = (bool)value;
                    if (header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase)) item.visibleInShortcut = (bool)value;                    
                }
                catch (Misp.Kernel.Domain.BcephalException)
                {
                    DisplayError("Unable edit item", "Unable edit : " + item.name);
                    return OperationState.STOP;
                }
            }

            return OperationState.CONTINUE;
        }

        public override OperationState SaveAs(string name)
        {
            if (Kernel.Util.FileUtil.isValidFileName(name))
                return base.SaveAs(name);
            else
            {
                DisplayError("Unable edit item", "Invalid File name.");
                return OperationState.STOP;
            }
        }


    }
}
