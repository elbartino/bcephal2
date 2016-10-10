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
            Functionality table = new Functionality(this, FunctionalitiesCode.INPUT_TABLE, "Input Table", true);
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_LIST, "Input Table List", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_VIEW, "Input Table View", true));
            table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_EDIT, "Input Table Edit", true));

            //Functionality table = new Functionality(this, FunctionalitiesCode.INPUT_TABLE, "Input Table", true);
            //table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_LIST, "Input Table List", true));
            //table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_VIEW, "Input Table View", true));
            //table.Children.Add(new Functionality(this, FunctionalitiesCode.INPUT_TABLE_EDIT, "Input Table Edit", true));

            this.Children.Add(table);
            this.Children.Add(new Functionality(this, FunctionalitiesCode.INITIATION_MEASURE, "Model", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.INITIATION_PERIOD, "Model", true));
        }


    }
}
