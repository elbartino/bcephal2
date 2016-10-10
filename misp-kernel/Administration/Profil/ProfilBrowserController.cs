using Misp.Kernel.Administration.Base;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilBrowserController : BrowserController<Misp.Kernel.Domain.Profil, ProfilBrowserData>
    {
        public ProfilBrowserController() 
        {
            ModuleName = "Administration"; //PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() 
        { 
            return AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_EDIT; 
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() 
        {
            return new ProfilBrowser(); 
        }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.PROFIL;
        }
        
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            if (this.SideBar != null && this.Service != null)
            {
            }
        }

        public ProfilService getProfilService()
        {
            return (ProfilService)Service;
        }

        /// <summary>
        /// Edit property
        /// </summary>
        /// <param name="item"></param>
        /// <param name="header"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override OperationState EditProperty(ProfilBrowserData item, String header, Object value)
        {
            if (item == null || String.IsNullOrWhiteSpace(header) || value == null) return OperationState.STOP;
            if (header.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                return RenameItem(item, (String)value);
            }
            if (header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase))
            {
                ProfilBrowserData data = new ProfilBrowserData(item);
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
