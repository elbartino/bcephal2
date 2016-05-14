using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;
using Misp.Initiation.Model;
using Misp.Initiation.Measure;
using Misp.Initiation.Periodicity;
using Misp.Kernel.Application;
using System.Windows;

namespace Misp.Initiation.Base
{
    public class InitiationToolBarHandlerBuilder : ToolBarHandlerBuilder
    {

        public InitiationController InitiationController { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controller"></param>
        public InitiationToolBarHandlerBuilder(InitiationController InitiationController, Controllable controller)
            : base(controller)
        {
            this.InitiationController = InitiationController;
        }


        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Save de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onSaveButtonClic(object sender, RoutedEventArgs e) 
        {
            OperationState state = this.InitiationController.Save();
            if (state == OperationState.STOP) return;
            this.InitiationController.AfterSave();
        }

        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton SaveAll de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onSaveAllButtonClic(object sender, RoutedEventArgs e) { this.InitiationController.SaveAll(); }
        
        /// <summary>
        /// Methode appelée lorsqu'on clique sur le bouton Close de la toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void onCloseButtonClic(object sender, RoutedEventArgs e) { HistoryHandler.Instance.closePage(this.InitiationController); }
        

    }
}
