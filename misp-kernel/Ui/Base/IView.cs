﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interface des vues
    /// </summary>
    public interface IView 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler);

        /// <summary>
        /// 
        /// </summary>
        void SetReadOnly(bool readOnly);

        /// <summary>
        /// Customize for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        void Customize(List<Domain.Right> rights, bool readOnly);

        bool IsReadOnly { get; set; }
    }
}
