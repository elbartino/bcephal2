using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;

namespace Misp.Sourcing.Table
{
    public class InputTableToolBarHandlerBuilder : ToolBarHandlerBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public InputTableToolBarHandlerBuilder(InputTableEditorController controller)
            : base(controller) { }

        /// <summary>
        /// Controller
        /// </summary>
        public InputTableEditorController GetInputTableEditorController()
        {
            return (InputTableEditorController)Controller;
        }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Import de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onImportButtonClic(object sender, RoutedEventArgs e) { GetInputTableEditorController().ImportExcel(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Export de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onExportButtonClic(object sender, RoutedEventArgs e) { GetInputTableEditorController().ExportExcel(); }

    }
}
