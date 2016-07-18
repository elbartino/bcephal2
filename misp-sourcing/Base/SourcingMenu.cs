using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;

namespace Misp.Sourcing.Base
{
    public class SourcingMenu : ApplicationMenu
    {

        public ApplicationMenu NewInputTableMenu { get; private set; }
        public ApplicationMenu ListInputTableMenu { get; private set; }

        public ApplicationMenu NewTargetMenu { get; private set; }
        public ApplicationMenu ListTargetMenu { get; private set; }

        public ApplicationMenu AutomaticTargetMenu { get; private set; }
        public ApplicationMenu ListAutomaticTargetMenu { get; private set; }

        public ApplicationMenu NewDesignMenu { get; private set; }
        public ApplicationMenu ListDesignMenu { get; private set; }

        public ApplicationMenu AutomaticSourcingMenu { get; private set; }
        public ApplicationMenu ListAutomaticSourcingMenu { get; private set; }

        public ApplicationMenu GridMenu { get; private set; }
        public ApplicationMenu NewInputGridMenu { get; private set; }
        public ApplicationMenu NewAutomaticGridMenu { get; private set; }
        public ApplicationMenu ListAutomaticGridMenu { get; private set; }
        public ApplicationMenu ListInputGridMenu { get; private set; }

        public ApplicationMenu UploadMultipleFileSourcingMenu { get; private set; }
     


        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(NewInputTableMenu);
            menus.Add(ListInputTableMenu);
            menus.Add(new Separator());
            GridMenu.Items.Add(NewInputGridMenu);
            GridMenu.Items.Add(ListInputGridMenu);
            GridMenu.Items.Add(new Separator());
            GridMenu.Items.Add(NewAutomaticGridMenu);
            GridMenu.Items.Add(ListAutomaticGridMenu);
            menus.Add(GridMenu);
            menus.Add(new Separator());
            menus.Add(NewTargetMenu);
            menus.Add(ListTargetMenu);
            menus.Add(new Separator());
            menus.Add(NewDesignMenu);
            menus.Add(ListDesignMenu);
            menus.Add(new Separator());
            menus.Add(AutomaticSourcingMenu);
            menus.Add(ListAutomaticSourcingMenu);
            menus.Add(new Separator());
            menus.Add(AutomaticTargetMenu);
            menus.Add(ListAutomaticTargetMenu);
            menus.Add(new Separator());
            menus.Add(UploadMultipleFileSourcingMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.SOURCING_MENU_CODE;
            this.Header = "Sourcing";
            NewInputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Input Table", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_INPUT_TABLE_FUNCTIONALITY));
            ListInputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Input Tables", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_INPUT_TABLE_FUNCTIONALITY));

            GridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Grid", null);

            NewInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Input Grid", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_INPUT_GRID_FUNCTIONALITY));
            ListInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Input Grid", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_INPUT_GRID_FUNCTIONALITY));
            
            NewAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Automatic Grid", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_GRID_FUNCTIONALITY));
            ListAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Automatic Grid", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_GRID_FUNCTIONALITY));
            
            NewTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Target", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_TARGET_FUNCTIONALITY));
            ListTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Targets", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_TARGET_FUNCTIONALITY));

            AutomaticTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Automatic Target", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_TARGET_FUNCTIONALITY));
            ListAutomaticTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Automatic Target", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_TARGET));
           

            NewDesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Design", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_DESIGN_FUNCTIONALITY));
            ListDesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Designs", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_DESIGN_FUNCTIONALITY));

            AutomaticSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Automatic Sourcing", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.UPLOAD_STRUCTURED_FILE_FUNCTIONALITY));
            ListAutomaticSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Automatic Sourcing", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_SOURCING));
            
            UploadMultipleFileSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Upload Multiple Files", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.UPLOAD_MULTIPLE_FILES));



        }


    }
}
