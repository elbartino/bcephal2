using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Base
{
    public class SourcingFunctionality : Functionality
    {

        public SourcingFunctionality()
        {
            this.Code = FunctionalitiesCode.SOURCING;
            this.Name = "Sourcing";
            buildChildren();
        }

        private void buildChildren()
        {
            Functionality posting = new Functionality(this, FunctionalitiesCode.POSTING_GRID, "Posting", true);
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.POSTING_GRID_LIST, "Posting Grid List", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.POSTING_GRID_VIEW, "Posting Grid View", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.POSTING_GRID_EDIT, "Posting Grid Edit", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_LIST, "Automatic Sourcing for Posting Grid List", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_VIEW, "Automatic Sourcing for Posting Grid View", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT, "Automatic Sourcing for Posting Grid Edit", true));
            this.Children.Add(posting);

            Functionality table = new Functionality(this, FunctionalitiesCode.INPUT_TABLE, "Input Table", true);
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_LIST, "Input Table List", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_VIEW, "Input Table View", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_EDIT, "Input Table Edit", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING, "Automatic Sourcing for Table List", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING_VIEW, "Automatic Sourcing for Table View", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING_EDIT, "Automatic Sourcing for Table Edit", true));
            this.Children.Add(table);

            Functionality grid = new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID, "Input Table Grid", true);
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID_LIST, "Input Table Grid List", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID_VIEW, "Input Table Grid View", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID_EDIT, "Input Table Grid Edit", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_LIST, "Automatic Sourcing for Table Grid List", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_VIEW, "Automatic Sourcing for Table Grid View", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT, "Automatic Sourcing for Table Grid Edit", true));
            this.Children.Add(grid);

            Functionality target = new Functionality(this, FunctionalitiesCode.TARGET, "Target", true);
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_LIST, "Target List", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_VIEW, "Target View", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_EDIT, "Target Edit", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET_LIST, "Automatic Sourcing for Target List", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET_VIEW, "Automatic Sourcing for Target View", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET_EDIT, "Automatic Sourcing for Target Edit", true));
            this.Children.Add(target);

            Functionality design = new Functionality(this, FunctionalitiesCode.DESIGN, "Design", true);
            design.Children.Add(new Functionality(this, FunctionalitiesCode.DESIGN_LIST, "Design List", true));
            design.Children.Add(new Functionality(this, FunctionalitiesCode.DESIGN_VIEW, "Design View", true));
            design.Children.Add(new Functionality(this, FunctionalitiesCode.DESIGN_EDIT, "Design Edit", true));
            this.Children.Add(design);

            Functionality accessories = new Functionality(this, FunctionalitiesCode.ACCESSORIES, "Accessories", true);
            accessories.Children.Add(new Functionality(this, FunctionalitiesCode.MULTIPLE_FILES_UPLOAD, "Multiple files uplod", true));
            this.Children.Add(accessories);

        }


    }
}
