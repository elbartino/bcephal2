using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public interface IEditor : IView
    {

        /// <summary>
        /// Selection (active) une page sur base de son titre.
        /// </summary>
        /// <param name="title">Le titre de la page à selectionner</param>
        void selectePage(string title);

    }
}
