using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Kernel.Controller
{
    public interface IToolBarHandler
    {
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Import de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onImportButtonClic(object sender, RoutedEventArgs e);

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Export de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onExportButtonClic(object sender, RoutedEventArgs e);
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton New de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onNewButtonClic(object sender, RoutedEventArgs e);

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Open de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onOpenButtonClic(object sender, RoutedEventArgs e);

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Save de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onSaveButtonClic(object sender, RoutedEventArgs e);

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton SaveAll de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onSaveAllButtonClic(object sender, RoutedEventArgs e);
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Rename de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onRenameButtonClic(object sender, RoutedEventArgs e);

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Delete de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onDeleteButtonClic(object sender, RoutedEventArgs e);

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Close de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onCloseButtonClic(object sender, RoutedEventArgs e);

    }
}
