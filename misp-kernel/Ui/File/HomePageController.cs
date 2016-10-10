using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;

namespace Misp.Kernel.Ui.File
{
    public class HomePageController : FileController
    {

        public HomePageController()
        {
            FunctionalityCode = FunctionalitiesCode.HOME_PAGE_FUNCTIONALITY;
        }

        /// <summary>
        /// The tool bar used to manage file.
        /// </summary>
        /// <returns>New instance of FileToolBar</returns>
        protected override ToolBar getNewToolBar() 
        {
            FileToolBar toolbar = new FileToolBar();
            toolbar.Children.Remove(toolbar.SaveAllButton);
            toolbar.Children.Remove(toolbar.RenameButton);
            //toolbar.Children.Remove(toolbar.CloseButton);
            return toolbar; 
        }

    }
}
