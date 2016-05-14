using Misp.Kernel.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Misp.Planification.PresentationView
{
    public class PresentationToolBarHandlerBuilder : ToolBarHandlerBuilder
    {
         /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public PresentationToolBarHandlerBuilder(PresentationEditorController controller)
            : base(controller) { }

        /// <summary>
        /// Controller
        /// </summary>
        public PresentationEditorController GetPresentationEditorController()
        {
            return (PresentationEditorController)Controller;
        }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Import de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onImportButtonClic(object sender, RoutedEventArgs e) { GetPresentationEditorController().ImportSlide(); }

      

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Export de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onExportButtonClic(object sender, RoutedEventArgs e) { GetPresentationEditorController().ExportSlide(); }

    }
}
