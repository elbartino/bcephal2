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

        public ApplicationMenu InputTableMenu { get; private set; }
        public ApplicationMenu NewInputTableMenu { get; private set; }
        public ApplicationMenu ListInputTableMenu { get; private set; }

        public ApplicationMenu TargetMenu { get; private set; }
        public ApplicationMenu NewTargetMenu { get; private set; }
        public ApplicationMenu ListTargetMenu { get; private set; }

        public ApplicationMenu AutomaticTargetMenu { get; private set; }
        public ApplicationMenu ListAutomaticTargetMenu { get; private set; }

        public ApplicationMenu DesignMenu { get; private set; }
        public ApplicationMenu NewDesignMenu { get; private set; }
        public ApplicationMenu ListDesignMenu { get; private set; }

        public ApplicationMenu AutomaticSourcingMenu { get; private set; }
        public ApplicationMenu ListAutomaticSourcingMenu { get; private set; }

        public ApplicationMenu GridMenu { get; private set; }
        public ApplicationMenu NewInputGridMenu { get; private set; }
        public ApplicationMenu NewAutomaticGridMenu { get; private set; }
        public ApplicationMenu ListAutomaticGridMenu { get; private set; }
        public ApplicationMenu ListInputGridMenu { get; private set; }

        public ApplicationMenu AccessoriesMenu { get; private set; }
        public ApplicationMenu UploadMultipleFileSourcingMenu { get; private set; }
     


        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            
            InputTableMenu.Items.Add(NewInputTableMenu);
            InputTableMenu.Items.Add(ListInputTableMenu);
            InputTableMenu.Items.Add(new Separator());
            InputTableMenu.Items.Add(AutomaticSourcingMenu);
            InputTableMenu.Items.Add(ListAutomaticSourcingMenu);
            menus.Add(InputTableMenu);
            menus.Add(new Separator());

            GridMenu.Items.Add(NewInputGridMenu);
            GridMenu.Items.Add(ListInputGridMenu);
            GridMenu.Items.Add(new Separator());
            GridMenu.Items.Add(NewAutomaticGridMenu);
            GridMenu.Items.Add(ListAutomaticGridMenu);
            menus.Add(GridMenu);
            menus.Add(new Separator());


            TargetMenu.Items.Add(NewTargetMenu);
            TargetMenu.Items.Add(ListTargetMenu);
            TargetMenu.Items.Add(new Separator());
            TargetMenu.Items.Add(AutomaticTargetMenu);
            TargetMenu.Items.Add(ListAutomaticTargetMenu);
            menus.Add(TargetMenu);
            menus.Add(new Separator());

            DesignMenu.Items.Add(NewDesignMenu);
            DesignMenu.Items.Add(ListDesignMenu);
            menus.Add(DesignMenu);
            menus.Add(new Separator());

            AccessoriesMenu.Items.Add(UploadMultipleFileSourcingMenu);
            menus.Add(AccessoriesMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.SOURCING_MENU_CODE;
            this.Header = "Sourcing";

            InputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "InputTable", null);
            NewInputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Input Table", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_INPUT_TABLE_FUNCTIONALITY));
            ListInputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Input Tables", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_INPUT_TABLE_FUNCTIONALITY));

            GridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Grid", null);

            NewInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Input Grid", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_INPUT_GRID_FUNCTIONALITY));
            ListInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Input Grid", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_INPUT_GRID_FUNCTIONALITY));
            
            NewAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Automatic Sourcing for Grid", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_GRID_FUNCTIONALITY));
            ListAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Automatic Sourcing for Grid", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_GRID_FUNCTIONALITY));

            TargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Target", null);
            NewTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Target", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_TARGET_FUNCTIONALITY));
            ListTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Targets", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_TARGET_FUNCTIONALITY));

            AutomaticTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Automatic Sourcing for Target", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_TARGET_FUNCTIONALITY));
            ListAutomaticTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Automatic Sourcing for Target", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_TARGET));

            DesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Design", null);
            NewDesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Design", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_DESIGN_FUNCTIONALITY));
            ListDesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Designs", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_DESIGN_FUNCTIONALITY));

            AutomaticSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "New Automatic Sourcing for table", NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.UPLOAD_STRUCTURED_FILE_FUNCTIONALITY));
            ListAutomaticSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "List Automatic Sourcing for table", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_SOURCING));

            AccessoriesMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Accessories", null);
            UploadMultipleFileSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, "Upload Multiple Files", NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.UPLOAD_MULTIPLE_FILES));
        }


    }
}
