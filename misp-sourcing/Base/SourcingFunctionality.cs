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

            posting.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_LIST, "Automatic Posting Grid List", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_VIEW, "Automatic Posting Grid View", true));
            posting.Children.Add(new Functionality(this, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT, "Automatic Posting Grid Edit", true));
            this.Children.Add(posting);

            Functionality table = new Functionality(this, FunctionalitiesCode.INPUT_TABLE, "Input Table", true);
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_LIST, "Input Table List", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_VIEW, "Input Table View", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_EDIT, "Input Table Edit", true));
            this.Children.Add(table);

            Functionality target = new Functionality(this, FunctionalitiesCode.TARGET, "Target", true);
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_LIST, "Target List", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_VIEW, "Target View", true));
            target.Children.Add(new Functionality(this, FunctionalitiesCode.TARGET_EDIT, "Target Edit", true));
            this.Children.Add(target);

            //Functionality table = new Functionality(this, FunctionalitiesCode.INPUT_TABLE, "Input Table", true);
            //table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_LIST, "Input Table List", true));
            //table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_VIEW, "Input Table View", true));
            //table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_EDIT, "Input Table Edit", true));

        }


    }
}
