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
            if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
            {
                menus.Add(PostingMenu);
                menus.Add(new Separator());
            }            
            menus.Add(InputTableMenu);
            menus.Add(new Separator());
            menus.Add(GridMenu);
            menus.Add(new Separator());
            menus.Add(TargetMenu);
            menus.Add(new Separator());
            menus.Add(DesignMenu);
            menus.Add(new Separator());
            menus.Add(AccessoriesMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = FunctionalitiesCode.SOURCING;
            this.Header = FunctionalitiesLabel.SOURCING_LABEL;

            PostingMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.POSTING_LABEL, FunctionalitiesCode.POSTING_GRID);
            NewPostingGridMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.NEW_POSTING_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.POSTING_GRID_EDIT));
            ListPostingGridMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.LIST_POSTING_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.POSTING_GRID_LIST));
            NewAutomaticPostingGridMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.NEW_AUTOMATIC_POSTING_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT));
            ListAutomaticPostingGridMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.LIST_AUTOMATIC_POSTING_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_POSTING_GRID_LIST));

            PostingMenu.Items.Add(NewPostingGridMenu);
            PostingMenu.Items.Add(ListPostingGridMenu);
            PostingMenu.Items.Add(new Separator());
            PostingMenu.Items.Add(NewAutomaticPostingGridMenu);
            PostingMenu.Items.Add(ListAutomaticPostingGridMenu);

            InputTableMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.INPUT_TABLE_LABEL, FunctionalitiesCode.INPUT_TABLE);
            NewInputTableMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.NEW_INPUT_TABLE_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_EDIT));
            ListInputTableMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.LIST_INPUT_TABLE_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_LIST));

            AutomaticSourcingMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.NEW_AUTOMATIC_SOURCING_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.UPLOAD_STRUCTURED_FILE_FUNCTIONALITY));
            ListAutomaticSourcingMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.LIST_AUTOMATIC_SOURCING_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_SOURCING_LIST));

            InputTableMenu.Items.Add(NewInputTableMenu);
            InputTableMenu.Items.Add(ListInputTableMenu);
            InputTableMenu.Items.Add(new Separator());
            InputTableMenu.Items.Add(AutomaticSourcingMenu);
            InputTableMenu.Items.Add(ListAutomaticSourcingMenu);

            GridMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.GRID_LABEL, FunctionalitiesCode.INPUT_TABLE_GRID);

            NewInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_INPUT_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_GRID_EDIT));
            ListInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_INPUT_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_GRID_LIST));
            
            NewAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT));
            ListAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_LIST));

            GridMenu.Items.Add(NewInputGridMenu);
            GridMenu.Items.Add(ListInputGridMenu);
            GridMenu.Items.Add(new Separator());
            GridMenu.Items.Add(NewAutomaticGridMenu);
            GridMenu.Items.Add(ListAutomaticGridMenu);

            TargetMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.TARGET_LABEL, FunctionalitiesCode.TARGET);
            NewTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.NEW_TARGET_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.TARGET_EDIT));
            ListTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.LIST_TARGET_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.TARGET_LIST));

            AutomaticTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.NEW_AUTOMATIC_TARGET_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_TARGET_EDIT));
            ListAutomaticTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.LIST_AUTOMATIC_TARGET_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_TARGET_LIST));

            TargetMenu.Items.Add(NewTargetMenu);
            TargetMenu.Items.Add(ListTargetMenu);
            TargetMenu.Items.Add(new Separator());
            TargetMenu.Items.Add(AutomaticTargetMenu);
            TargetMenu.Items.Add(ListAutomaticTargetMenu);

            DesignMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.DESIGN_LABEL, FunctionalitiesCode.DESIGN);
            NewDesignMenu = BuildMenu(FunctionalitiesCode.DESIGN, FunctionalitiesLabel.NEW_DESIGN_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.DESIGN_EDIT));
            ListDesignMenu = BuildMenu(FunctionalitiesCode.DESIGN, FunctionalitiesLabel.LIST_DESIGN_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.DESIGN_LIST));

            DesignMenu.Items.Add(NewDesignMenu);
            DesignMenu.Items.Add(ListDesignMenu);


            AccessoriesMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.ACCESSORIES_LABEL, FunctionalitiesCode.ACCESSORIES);
            UploadMultipleFileSourcingMenu = BuildMenu(FunctionalitiesCode.ACCESSORIES, FunctionalitiesLabel.UPLOAD_MULTIPLE_FILES, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.MULTIPLE_FILES_UPLOAD));

            AccessoriesMenu.Items.Add(UploadMultipleFileSourcingMenu);
        }
        

    }
}
