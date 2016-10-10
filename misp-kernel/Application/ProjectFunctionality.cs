using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Application
{
    public class ProjectFunctionality : Functionality{

        public ProjectFunctionality()
        {
            this.Code = FunctionalitiesCode.PROJECT;
            this.Name = "Project";
            buildChildren();
        }

        private void buildChildren()
        {
            this.Children.Add(new Functionality(this, FunctionalitiesCode.PROJECT_EDIT, "Project creation and edition", true));            
        }


    }
}
