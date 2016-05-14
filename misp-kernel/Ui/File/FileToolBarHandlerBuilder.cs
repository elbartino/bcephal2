using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;
using System.Windows;

namespace Misp.Kernel.Ui.File
{
    public class FileToolBarHandlerBuilder : ToolBarHandlerBuilder
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controller"></param>
        public FileToolBarHandlerBuilder(FileController controller) : base(controller) { }


        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Open de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onOpenButtonClic(object sender, RoutedEventArgs e) { Controller.Search(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton SaveAs de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onSaveAllButtonClic(object sender, RoutedEventArgs e) { Controller.SaveAs(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Rename de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onRenameButtonClic(object sender, RoutedEventArgs e) { Controller.Rename(); }



    }
}
