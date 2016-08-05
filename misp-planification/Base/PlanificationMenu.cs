﻿using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Planification.Base
{
    public class PlanificationMenu : ApplicationMenu
    {

        public ApplicationMenu NewTransformationTreeMenu { get; private set; }
        public ApplicationMenu ListTransformationTreeMenu { get; private set; }
        public ApplicationMenu LoadTransformationTreesMenu { get; private set; }
        public ApplicationMenu ClearTransformationTreesMenu { get; private set; }
        public ApplicationMenu CreateCombinedTransformationTreesMenu { get; private set; }
        public ApplicationMenu ListCombinedTransformationTreesMenu { get; private set; }
        
        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(NewTransformationTreeMenu);
            menus.Add(ListTransformationTreeMenu);
            menus.Add(new Separator());
            menus.Add(CreateCombinedTransformationTreesMenu);
            menus.Add(ListCombinedTransformationTreesMenu);
            //menus.Add(new Separator());
           // menus.Add(LoadTransformationTreesMenu);
           // menus.Add(ClearTransformationTreesMenu);

            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.SOURCING_MENU_CODE;
            this.Header = FunctionalitiesLabel.TRANSFORMATION_LABEL;
            NewTransformationTreeMenu = BuildMenu(ApplicationMenu.PLANIFICATION_MENU_CODE,FunctionalitiesLabel.NEW_TRANSFORMATION_TREE_LABEL, NavigationToken.GetCreateViewToken(PlanificationFunctionalitiesCode.NEW_TRANSFORMATION_TREE_FUNCTIONALITY));
            ListTransformationTreeMenu = BuildMenu(ApplicationMenu.PLANIFICATION_MENU_CODE, FunctionalitiesLabel.NEW_TRANSFORMATION_TREE_LABEL, NavigationToken.GetSearchViewToken(PlanificationFunctionalitiesCode.LIST_TRANSFORMATION_TREE_FUNCTIONALITY));
            LoadTransformationTreesMenu = BuildMenu(ApplicationMenu.PLANIFICATION_MENU_CODE, FunctionalitiesLabel.LOAD_TRANSFORMATION_TREES_LABEL, NavigationToken.GetCreateViewToken(PlanificationFunctionalitiesCode.LOAD_TRANSFORMATION_TREES_FUNCTIONALITY));
            ClearTransformationTreesMenu = BuildMenu(ApplicationMenu.PLANIFICATION_MENU_CODE, FunctionalitiesLabel.CLEAR_TRANSFORMATION_TREES_LABEL, NavigationToken.GetCreateViewToken(PlanificationFunctionalitiesCode.CLEAR_TRANSFORMATION_TREES_FUNCTIONALITY));
            CreateCombinedTransformationTreesMenu = BuildMenu(ApplicationMenu.PLANIFICATION_MENU_CODE,FunctionalitiesLabel.NEW_COMBINED_TRANSFORMATION_TREES_LABEL, NavigationToken.GetCreateViewToken(PlanificationFunctionalitiesCode.NEW_COMBINED_TRANSFORMATION_TREES_FUNCTIONALITY));
            ListCombinedTransformationTreesMenu = BuildMenu(ApplicationMenu.PLANIFICATION_MENU_CODE, FunctionalitiesLabel.LIST_COMBINED_TRANSFORMATION_TREES_LABEL, NavigationToken.GetSearchViewToken(PlanificationFunctionalitiesCode.LIST_COMBINED_TRANSFORMATION_TREES_FUNCTIONALITY));
        }

    }
}
