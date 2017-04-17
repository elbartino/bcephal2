using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Dashboard
{
    public class NavBlockContextMenu : ContextMenu
    {
                
        #region Properties

        public MenuItem EditMenuItem { get; protected set; }
        public MenuItem HideMenuItem { get; protected set; }

        #endregion


        #region Constructors

        public NavBlockContextMenu()
        {
            InitMenuItem();
        }
        
        #endregion


        #region Operations
        
        protected virtual void InitMenuItem()
        {
            this.EditMenuItem = new MenuItem();
            this.EditMenuItem.Header = "Edit";
            this.Items.Add(this.EditMenuItem);

            this.HideMenuItem = new MenuItem();
            this.HideMenuItem.Header = "Hide";
            this.Items.Add(this.HideMenuItem);
        }

        #endregion


        #region Handlers


        #endregion             
        

    }
}
