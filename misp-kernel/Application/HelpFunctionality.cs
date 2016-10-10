using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Application
{
    public class HelpFunctionality : Functionality{

        public HelpFunctionality()
        {
            this.Code = FunctionalitiesCode.HELP;
            this.Name = "Help";
            buildChildren();
        }

        private void buildChildren()
        {
            //this.Children.Add(new Functionality(this, FunctionalitiesCode.a, "Project creation and edition", true));            
        }

    }
}
