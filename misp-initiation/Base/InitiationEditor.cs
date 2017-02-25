using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Initiation.Base
{
    /// <summary>
    /// L'éditieur d'initialisation.
    /// 
    /// Cet éditeur comporte 3 pages:
    /// 1. L'éditeur de Models
    /// 2. L'éditeur de Mesures
    /// 3. L'éditeur de périodicité
    /// 
    /// </summary>
    public class InitiationEditor : LayoutDocumentPane, IView
    {

        public bool IsReadOnly { get; set; }

        private Model.ModelEditor modelEditor;
        private Measure.MeasureEditorItem measureEditorItem;

        /// <summary>
        /// 
        /// </summary>
        public InitiationEditor()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelEditor"></param>
        /// <param name="measureEditorItem"></param>
        /// <param name="periodicityEditorItem"></param>
        public InitiationEditor(Model.ModelEditor modelEditor, Measure.MeasureEditorItem measureEditorItem)
            : this()
        {
            Initializepages(modelEditor, measureEditorItem);
        }


        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        /// <summary>
        /// Customize for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public virtual void Customize(List<Kernel.Domain.Right> rights, bool readOnly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelEditor"></param>
        /// <param name="measureEditorItem"></param>
        /// <param name="periodicityEditorItem"></param>
        public void Initializepages(Model.ModelEditor modelEditor, Measure.MeasureEditorItem measureEditorItem)
        {
            this.Children.Clear();
            this.modelEditor = modelEditor;
            this.measureEditorItem = measureEditorItem;
            if (this.modelEditor != null)
            {
                this.Children.Add(this.modelEditor.getLayoutContent());
            }
            if (this.measureEditorItem != null)
            {
                this.Children.Add(this.measureEditorItem);
            }
        }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }
    }
}
