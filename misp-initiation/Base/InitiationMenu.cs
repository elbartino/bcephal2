using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;

namespace Misp.Initiation.Base
{
    public class InitiationMenu : ApplicationMenu
    {

        private ApplicationMenu model; 
        
        private ApplicationMenu period;

        private ApplicationMenu backupMenu;

        private ApplicationMenu backupSimpleMenu;

        private ApplicationMenu backupAutomaticMenu;

        public ApplicationMenu Model { get { return model; } }
                
        public ApplicationMenu Period { get { return period; } }

        public ApplicationMenu ArchiveMenu { get { return backupMenu; } }

        public ApplicationMenu BackupSimpleMenu {get {return backupSimpleMenu;}}

        public ApplicationMenu BackupAutomaticMenu {get {return backupAutomaticMenu;}}

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(Model);
            menus.Add(period);
            ArchiveMenu.Items.Add(backupSimpleMenu);
            ArchiveMenu.Items.Add(backupAutomaticMenu);
            menus.Add(ArchiveMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.INITIATION_MENU_CODE;
            this.Header = "Initiation";
            model = BuildMenu(ApplicationMenu.INITIATION_MENU_CODE, "Edit Model", NavigationToken.GetSearchViewToken(InitiationFunctionalitiesCode.INITIATION_FUNCTIONALITY));
            period = BuildMenu(ApplicationMenu.INITIATION_MENU_CODE, "Edit Period", NavigationToken.GetSearchViewToken(InitiationFunctionalitiesCode.PERIOD_FUNCTIONALITY));
            backupMenu = BuildMenu(ApplicationMenu.INITIATION_MENU_CODE, "Backup", null);
            //backupMenu.IsEnabled = false;
            backupSimpleMenu = BuildMenu(ApplicationMenu.INITIATION_MENU_CODE, "Simple Backup", NavigationToken.GetCreateViewToken(InitiationFunctionalitiesCode.BACKUP_SIMPLE_FUNCTIONALITY));
            backupAutomaticMenu = BuildMenu(ApplicationMenu.INITIATION_MENU_CODE, "Automatic Backup", NavigationToken.GetCreateViewToken(InitiationFunctionalitiesCode.BACKUP_AUTOMATIC_FUNCTIONALITY));          
        }

    }
}
