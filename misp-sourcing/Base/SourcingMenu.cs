using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Domain;

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


        public ApplicationMenu PostingMenu { get; private set; }
        public ApplicationMenu NewPostingGridMenu { get; private set; }
        public ApplicationMenu ListPostingGridMenu { get; private set; }
        public ApplicationMenu NewAutomaticPostingGridMenu { get; private set; }
        public ApplicationMenu ListAutomaticPostingGridMenu { get; private set; }


        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);

            PostingMenu.Items.Add(NewPostingGridMenu);
            PostingMenu.Items.Add(ListPostingGridMenu);
            PostingMenu.Items.Add(new Separator());
            PostingMenu.Items.Add(NewAutomaticPostingGridMenu);
            PostingMenu.Items.Add(ListAutomaticPostingGridMenu);
            if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
            {
                menus.Add(PostingMenu);
                menus.Add(new Separator());
            }

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
            this.Header = FunctionalitiesLabel.SOURCING_LABEL;

            PostingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.POSTING_LABEL, null);
            NewPostingGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_POSTING_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_POSTING_GRID_FUNCTIONALITY));
            ListPostingGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_POSTING_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_POSTING_GRID_FUNCTIONALITY));
            NewAutomaticPostingGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_POSTING_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_POSTING_GRID_FUNCTIONALITY));
            ListAutomaticPostingGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_POSTING_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_POSTING_GRID_FUNCTIONALITY));

            InputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.INPUT_TABLE_LABEL, null);
            NewInputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_INPUT_TABLE_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_INPUT_TABLE_FUNCTIONALITY));
            ListInputTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_INPUT_TABLE_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_INPUT_TABLE_FUNCTIONALITY));

            GridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.GRID_LABEL, null);

            NewInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_INPUT_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_INPUT_GRID_FUNCTIONALITY));
            ListInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_INPUT_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_INPUT_GRID_FUNCTIONALITY));
            
            NewAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_GRID_FUNCTIONALITY));
            ListAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_GRID_FUNCTIONALITY));

            TargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.TARGET_LABEL, null);
            NewTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_TARGET_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_TARGET_FUNCTIONALITY));
            ListTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_TARGET_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_TARGET_FUNCTIONALITY));

            AutomaticTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_TARGET_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_AUTOMATIC_TARGET_FUNCTIONALITY));
            ListAutomaticTargetMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_TARGET_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_TARGET));

            DesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.DESIGN_LABEL, null);
            NewDesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_DESIGN_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.NEW_DESIGN_FUNCTIONALITY));
            ListDesignMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_DESIGN_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_DESIGN_FUNCTIONALITY));

            AutomaticSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_SOURCING_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.UPLOAD_STRUCTURED_FILE_FUNCTIONALITY));
            ListAutomaticSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_SOURCING_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LIST_AUTOMATIC_SOURCING));

            AccessoriesMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE,FunctionalitiesLabel.ACCESSORIES_LABEL, null);
            UploadMultipleFileSourcingMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.UPLOAD_MULTIPLE_FILES, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.UPLOAD_MULTIPLE_FILES));
        }

        public override ApplicationMenu customize(User user)
        {
            if (user == null || !user.active.Value) return null;
            if (user.IsAdmin()) return this;
            if (user.profil == null || !user.profil.active) return null;
            
            return this;
        }


    }
}
