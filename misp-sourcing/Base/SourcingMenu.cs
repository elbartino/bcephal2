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
        
        public ApplicationMenu LinkedAttributeGridMenu { get; private set; }
        
        public ApplicationMenu EnrichmentTableMenu { get; private set; }
        public ApplicationMenu NewEnrichmentTableMenu { get; private set; }
        public ApplicationMenu NewAutomaticEnrichmentTableMenu { get; private set; }
        public ApplicationMenu ListAutomaticEnrichmentTableMenu { get; private set; }
        public ApplicationMenu ListEnrichmentTableMenu { get; private set; }

        public ApplicationMenu AccessoriesMenu { get; private set; }
        public ApplicationMenu UploadMultipleFileSourcingMenu { get; private set; }
        

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0); 
            menus.Add(InputTableMenu);
            menus.Add(GridMenu);
            menus.Add(LinkedAttributeGridMenu);
            menus.Add(EnrichmentTableMenu);
            menus.Add(TargetMenu);
            menus.Add(DesignMenu);
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
            
            InputTableMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.INPUT_TABLE_LABEL, FunctionalitiesCode.INPUT_TABLE);
            NewInputTableMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.NEW_INPUT_TABLE_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_EDIT), Kernel.Domain.RightType.CREATE);
            ListInputTableMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.LIST_INPUT_TABLE_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_LIST), Kernel.Domain.RightType.VIEW);

            AutomaticSourcingMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.NEW_AUTOMATIC_SOURCING_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_SOURCING_EDIT), Kernel.Domain.RightType.CREATE);
            ListAutomaticSourcingMenu = BuildMenu(FunctionalitiesCode.INPUT_TABLE, FunctionalitiesLabel.LIST_AUTOMATIC_SOURCING_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_SOURCING_LIST), Kernel.Domain.RightType.VIEW);

            InputTableMenu.Items.Add(NewInputTableMenu);
            InputTableMenu.Items.Add(ListInputTableMenu);
            InputTableMenu.Items.Add(new Separator());
            InputTableMenu.Items.Add(AutomaticSourcingMenu);
            InputTableMenu.Items.Add(ListAutomaticSourcingMenu);

            GridMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.GRID_LABEL, FunctionalitiesCode.INPUT_TABLE_GRID);

            NewInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_INPUT_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_GRID_EDIT), Kernel.Domain.RightType.CREATE);
            ListInputGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_INPUT_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.INPUT_TABLE_GRID_LIST), Kernel.Domain.RightType.VIEW);

            NewAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_GRID_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT), Kernel.Domain.RightType.CREATE);
            ListAutomaticGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_LIST), Kernel.Domain.RightType.VIEW);

            GridMenu.Items.Add(NewInputGridMenu);
            GridMenu.Items.Add(ListInputGridMenu);
            GridMenu.Items.Add(new Separator());
            GridMenu.Items.Add(NewAutomaticGridMenu);
            GridMenu.Items.Add(ListAutomaticGridMenu);

            LinkedAttributeGridMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LINKED_ATTRIBUTE_GRID_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.LINKED_ATTRIBUTE_GRID_LIST), Kernel.Domain.RightType.VIEW);



            EnrichmentTableMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.ENRICHMENT_TABLE_LABEL, FunctionalitiesCode.ENRICHMENT_TABLE);

            NewEnrichmentTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_ENRICHMENT_TABLE_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.ENRICHMENT_TABLE_EDIT), Kernel.Domain.RightType.CREATE);
            ListEnrichmentTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_ENRICHMENT_TABLE_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.ENRICHMENT_TABLE_LIST), Kernel.Domain.RightType.VIEW);

            NewAutomaticEnrichmentTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.NEW_AUTOMATIC_ENRICHMENT_TABLE_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_EDIT), Kernel.Domain.RightType.CREATE);
            ListAutomaticEnrichmentTableMenu = BuildMenu(ApplicationMenu.SOURCING_MENU_CODE, FunctionalitiesLabel.LIST_AUTOMATIC_ENRICHMENT_TABLE_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_LIST), Kernel.Domain.RightType.VIEW);

            //EnrichmentTableMenu.Items.Add(NewEnrichmentTableMenu);
            //EnrichmentTableMenu.Items.Add(ListEnrichmentTableMenu);
            //EnrichmentTableMenu.Items.Add(new Separator());
            EnrichmentTableMenu.Items.Add(NewAutomaticEnrichmentTableMenu);
            EnrichmentTableMenu.Items.Add(ListAutomaticEnrichmentTableMenu);

            TargetMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.TARGET_LABEL, FunctionalitiesCode.TARGET);
            NewTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.NEW_TARGET_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.TARGET_EDIT), Kernel.Domain.RightType.CREATE);
            ListTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.LIST_TARGET_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.TARGET_LIST), Kernel.Domain.RightType.VIEW);

            AutomaticTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.NEW_AUTOMATIC_TARGET_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.AUTOMATIC_TARGET_EDIT), Kernel.Domain.RightType.CREATE);
            ListAutomaticTargetMenu = BuildMenu(FunctionalitiesCode.TARGET, FunctionalitiesLabel.LIST_AUTOMATIC_TARGET_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.AUTOMATIC_TARGET_LIST), Kernel.Domain.RightType.VIEW);

            TargetMenu.Items.Add(NewTargetMenu);
            TargetMenu.Items.Add(ListTargetMenu);
            TargetMenu.Items.Add(new Separator());
            TargetMenu.Items.Add(AutomaticTargetMenu);
            TargetMenu.Items.Add(ListAutomaticTargetMenu);

            DesignMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.DESIGN_LABEL, FunctionalitiesCode.DESIGN);
            NewDesignMenu = BuildMenu(FunctionalitiesCode.DESIGN, FunctionalitiesLabel.NEW_DESIGN_LABEL, NavigationToken.GetCreateViewToken(SourcingFunctionalitiesCode.DESIGN_EDIT), Kernel.Domain.RightType.CREATE);
            ListDesignMenu = BuildMenu(FunctionalitiesCode.DESIGN, FunctionalitiesLabel.LIST_DESIGN_LABEL, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.DESIGN_LIST), Kernel.Domain.RightType.VIEW);

            DesignMenu.Items.Add(NewDesignMenu);
            DesignMenu.Items.Add(ListDesignMenu);


            AccessoriesMenu = BuildMenu(FunctionalitiesCode.SOURCING, FunctionalitiesLabel.ACCESSORIES_LABEL, FunctionalitiesCode.ACCESSORIES);
            UploadMultipleFileSourcingMenu = BuildMenu(FunctionalitiesCode.ACCESSORIES, FunctionalitiesLabel.UPLOAD_MULTIPLE_FILES, NavigationToken.GetSearchViewToken(SourcingFunctionalitiesCode.MULTIPLE_FILES_UPLOAD));

            AccessoriesMenu.Items.Add(UploadMultipleFileSourcingMenu);
        }
        

    }
}
