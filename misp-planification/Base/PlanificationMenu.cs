using Misp.Kernel.Ui.Base.Menu;
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
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = FunctionalitiesCode.TRANSFORMATION_DATA;
            this.Header = FunctionalitiesLabel.TRANSFORMATION_LABEL;
            NewTransformationTreeMenu = BuildMenu(FunctionalitiesCode.TRANSFORMATION_DATA, FunctionalitiesLabel.NEW_TRANSFORMATION_TREE_LABEL, NavigationToken.GetCreateViewToken(PlanificationFunctionalitiesCode.TRANSFORMATION_TREE_EDIT));
            ListTransformationTreeMenu = BuildMenu(FunctionalitiesCode.TRANSFORMATION_DATA, FunctionalitiesLabel.LIST_TRANSFORMATION_TREE_LABEL, NavigationToken.GetSearchViewToken(PlanificationFunctionalitiesCode.TRANSFORMATION_TREE_LIST));
            CreateCombinedTransformationTreesMenu = BuildMenu(FunctionalitiesCode.TRANSFORMATION_DATA, FunctionalitiesLabel.NEW_COMBINED_TRANSFORMATION_TREES_LABEL, NavigationToken.GetCreateViewToken(PlanificationFunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_EDIT));
            ListCombinedTransformationTreesMenu = BuildMenu(FunctionalitiesCode.TRANSFORMATION_DATA, FunctionalitiesLabel.LIST_COMBINED_TRANSFORMATION_TREES_LABEL, NavigationToken.GetSearchViewToken(PlanificationFunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_LIST));
        }

    }
}
