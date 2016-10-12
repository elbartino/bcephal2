using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Allocation.Base
{
    public class AllocationFunctionality : Functionality
    {

        public AllocationFunctionality()
        {
            this.Code = FunctionalitiesCode.LOAD;
            this.Name = "Load";
            buildChildren();
        }

        private void buildChildren()
        {
            this.Children.Add(new Functionality(this, FunctionalitiesCode.LOAD_TABLES_AND_GRIDS, "Load Tables and Grids", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.LOAD_CLEAR_TABLES_AND_GRIDS, "Clear Tables and Grids", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.LOAD_LOG, "Load log", true));

        }
    }
}
