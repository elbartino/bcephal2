using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Base
{
    public class PlanificationFunctionality : Functionality
    {

        public PlanificationFunctionality()
        {
            this.Code = FunctionalitiesCode.TRANSFORMATION_DATA;
            this.Name = "Transformation Data";
            buildChildren();
        }

        private void buildChildren()
        {
            Functionality tree = new Functionality(this, FunctionalitiesCode.TRANSFORMATION_TREE, "Transformation Tree", true);
            /*
            tree.Children.Add(new Functionality(this, FunctionalitiesCode.TRANSFORMATION_TREE_LIST, "Transformation Tree List", true));
            tree.Children.Add(new Functionality(this, FunctionalitiesCode.TRANSFORMATION_TREE_VIEW, "Transformation Tree View", true));
            tree.Children.Add(new Functionality(this, FunctionalitiesCode.TRANSFORMATION_TREE_EDIT, "Transformation Tree Edit", true));
            */
            this.Children.Add(tree);

            Functionality combinedTree = new Functionality(this, FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES, "Combined Transformation Trees", true);
            /*
            combinedTree.Children.Add(new Functionality(this, FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_LIST, "Combined Transformation Trees List", true));
            combinedTree.Children.Add(new Functionality(this, FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_VIEW, "Combined Transformation Trees View", true));
            combinedTree.Children.Add(new Functionality(this, FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_EDIT, "Combined Transformation Trees Edit", true));
            */
            this.Children.Add(combinedTree);

        }
    }
}
