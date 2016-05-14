using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Misp.Kernel.Controller;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangeEventHandlerBuilder
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public ChangeEventHandlerBuilder(Controllable controller)
        {
            this.Controller = controller;
        }

        /// <summary>
        /// Controller
        /// </summary>
        public Controllable Controller { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur un control de type .
        /// </summary>
        public ChangeEventHandler ChangeEventHandler { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur un control de type ToggleButton.
        /// </summary>
        public RoutedEventHandler RoutedEventHandler { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur un control de type TextBoxBase.
        /// </summary>
        public TextChangedEventHandler TextChangedEventHandler { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur un control de type Selector (ComboBox).
        /// </summary>
        public SelectionChangedEventHandler SelectionChangedEventHandler { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur un control de type DatePicker.
        /// </summary>
        public EventHandler<SelectionChangedEventArgs> EventHandler { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public TreeView.EditableTree.ChangeHandler EditableTreeChangeHandler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DiagramDesigner.ChangeEventHandler DiagramChange { get; set; }


        public virtual void BuildHandlers()
        {
            this.ChangeEventHandler = new ChangeEventHandler(change);
            this.RoutedEventHandler = new RoutedEventHandler(onChange);
            this.TextChangedEventHandler = new TextChangedEventHandler(onChange);
            this.SelectionChangedEventHandler = new SelectionChangedEventHandler(onChange);
            this.EventHandler = new EventHandler<SelectionChangedEventArgs>(onChange);
            this.DiagramChange = new DiagramDesigner.ChangeEventHandler(onDiagramChange);
            this.EditableTreeChangeHandler = new TreeView.EditableTree.ChangeHandler(change);
        }

        protected virtual void onChange(object sender, RoutedEventArgs args)
        {
            change();
        }

        protected virtual void onDiagramChange()
        {
            change();
        }

        public virtual void change()
        {
            this.Controller.OnChange();
        }

    }
}
