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
            this.Children.Add(new Functionality(this, FunctionalitiesCode.LIST_GROUP_FUNCTIONALITY, "Group edition", true));

            Functionality backup = new Functionality(this, FunctionalitiesCode.BACKUP_FUNCTIONALITY, "Backup", true);
            backup.Children.Add(new Functionality(backup, FunctionalitiesCode.BACKUP_SIMPLE_FUNCTIONALITY, "Simple backup", true));
            backup.Children.Add(new Functionality(backup, FunctionalitiesCode.BACKUP_AUTOMATIC_FUNCTIONALITY, "Automatic backup", true));
            this.Children.Add(backup); 
        }


    }
}
