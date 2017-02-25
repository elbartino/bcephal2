using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Misp.Kernel.Application;

namespace Misp.Kernel.Controller
{
    public class ToolBarHandlerBuilder : IToolBarHandler
    {

        /// <summary>
        /// Controller
        /// </summary>
        public Controllable Controller { get; set; }

        /// <summary>
        /// Toolbar
        /// </summary>
        public Misp.Kernel.Ui.Base.ToolBar ToolBar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public ToolBarHandlerBuilder(Controllable controller)
        {
            this.Controller = controller;
            ToolBar = Controller.ToolBar;
        }

        /// <summary>
        /// Build handlers
        /// </summary>
        public virtual void buildHandlers() 
        {
            if (ToolBar == null) return;                       
            ToolBar.NewButton.Click += new RoutedEventHandler(onNewButtonClic);
            ToolBar.SaveButton.Click += new RoutedEventHandler(onSaveButtonClic);
            ToolBar.CloseButton.Click += new RoutedEventHandler(onCloseButtonClic);
        }
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Import de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onImportButtonClic(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Export de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onExportButtonClic(object sender, RoutedEventArgs e) { }
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton New de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onNewButtonClic(object sender, RoutedEventArgs e) { Controller.Create(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Open de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onOpenButtonClic(object sender, RoutedEventArgs e) { Controller.Open(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Save de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onSaveButtonClic(object sender, RoutedEventArgs e) 
        {
            OperationState state = Controller.Save();
            if (state == OperationState.STOP) return;
            this.Controller.AfterSave();
        }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton SaveAll de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onSaveAllButtonClic(object sender, RoutedEventArgs e) { Controller.SaveAll(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton SaveAs de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onSaveAsButtonClic(object sender, RoutedEventArgs e) { Controller.SaveAs(); }
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Rename de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onRenameButtonClic(object sender, RoutedEventArgs e) { Controller.Rename(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Delete de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onDeleteButtonClic(object sender, RoutedEventArgs e) { Controller.Delete(); }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Close de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void onCloseButtonClic(object sender, RoutedEventArgs e) { HistoryHandler.Instance.closePage(Controller); }

    }
}
