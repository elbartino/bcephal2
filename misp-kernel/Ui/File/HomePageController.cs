﻿using System;
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
            FunctionalityCode = FunctionalitiesCode.HOME_PAGE;
        }

        /// <summary>
        /// The tool bar used to manage file.
        /// </summary>
        /// <returns>New instance of FileToolBar</returns>
        protected override ToolBar getNewToolBar() 
        {
            FileToolBar toolbar = new FileToolBar();
            return toolbar; 
        }

    }
}
