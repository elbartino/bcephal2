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
            Functionality table = new Functionality(this, FunctionalitiesCode.INPUT_TABLE, "Input Table", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            Functionality autoTable = new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING, "Automatic Sourcing for Table", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            /*
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_LIST, "Input Table List", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_VIEW, "Input Table View", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_EDIT, "Input Table Edit", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING, "Automatic Sourcing for Table List", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING_VIEW, "Automatic Sourcing for Table View", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_SOURCING_EDIT, "Automatic Sourcing for Table Edit", true));
            */
            this.Children.Add(table);
            this.Children.Add(autoTable);

            Functionality grid = new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID, "Input Table Grid", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            Functionality autoGrid = new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID, "Automatic Sourcing for Table Grid", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            /*
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID_LIST, "Input Table Grid List", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID_VIEW, "Input Table Grid View", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_GRID_EDIT, "Input Table Grid Edit", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_LIST, "Automatic Sourcing for Table Grid List", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_VIEW, "Automatic Sourcing for Table Grid View", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT, "Automatic Sourcing for Table Grid Edit", true));
            */
            this.Children.Add(grid);
            this.Children.Add(autoGrid);

            Functionality linkedAttrGrid = new Functionality(this, FunctionalitiesCode.LINKED_ATTRIBUTE_GRID, "Linked Attribute Grid", true, RightType.VIEW, RightType.EDIT);
            this.Children.Add(linkedAttrGrid);     
       
            Functionality target = new Functionality(this, FunctionalitiesCode.TARGET, "Target", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            Functionality autoTargetTable = new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET, "Automatic Sourcing for Target", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            /*
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_LIST, "Target List", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_VIEW, "Target View", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_EDIT, "Target Edit", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET_LIST, "Automatic Sourcing for Target List", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET_VIEW, "Automatic Sourcing for Target View", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_TARGET_EDIT, "Automatic Sourcing for Target Edit", true));
            */
            this.Children.Add(target);
            this.Children.Add(autoTargetTable);

            Functionality design = new Functionality(this, FunctionalitiesCode.DESIGN, "Design", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            /*
            design.Children.Add(new Functionality(this, FunctionalitiesCode.DESIGN_LIST, "Design List", true));
            design.Children.Add(new Functionality(this, FunctionalitiesCode.DESIGN_VIEW, "Design View", true));
            design.Children.Add(new Functionality(this, FunctionalitiesCode.DESIGN_EDIT, "Design Edit", true));
            */
            this.Children.Add(design);

            Functionality enrichmentTable = new Functionality(this, FunctionalitiesCode.ENRICHMENT_TABLE, "Enrichment Table", true);
            Functionality autoEnrichmentTable = new Functionality(this, FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE, "Automatic Sourcing for Enrichment Table", true, RightType.VIEW, RightType.EDIT, RightType.CREATE);
            /*
            enrichmentTable.Children.Add(new Functionality(this, FunctionalitiesCode.ENRICHMENT_TABLE_LIST, "Enrichment Table List", true));
            enrichmentTable.Children.Add(new Functionality(this, FunctionalitiesCode.ENRICHMENT_TABLE_VIEW, "Enrichment Table View", true));
            enrichmentTable.Children.Add(new Functionality(this, FunctionalitiesCode.ENRICHMENT_TABLE_EDIT, "Enrichment Table Edit", true));
            enrichmentTable.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_LIST, "Automatic Sourcing for Enrichment Table List", true));
            enrichmentTable.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_VIEW, "Automatic Sourcing for Enrichment Table View", true));
            enrichmentTable.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_EDIT, "Automatic Sourcing for Enrichment Table Edit", true));
            */
            this.Children.Add(enrichmentTable);
            this.Children.Add(autoEnrichmentTable);

            Functionality accessories = new Functionality(this, FunctionalitiesCode.ACCESSORIES, "Accessories", true);
            /*
            accessories.Children.Add(new Functionality(this, FunctionalitiesCode.MULTIPLE_FILES_UPLOAD, "Multiple files uplod", true));
            */
            this.Children.Add(accessories);

        }


    }
}
