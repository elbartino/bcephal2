using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Initiation.Base
{
    public class InitiationFunctionality : Functionality
    {

        public InitiationFunctionality()
        {
            this.Code = FunctionalitiesCode.INITIATION;
            this.Name = "Initiation";
            buildChildren();
        }

        private void buildChildren()
        {
            this.Children.Add(new Functionality(this, FunctionalitiesCode.INITIATION_MODEL, "Model", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.INITIATION_MEASURE, "Model", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.INITIATION_PERIOD, "Model", true));
        }

    }
}
