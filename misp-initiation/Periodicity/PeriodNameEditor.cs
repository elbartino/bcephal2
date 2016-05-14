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

namespace Misp.Initiation.Periodicity
{
    public class PeriodNameEditor : LayoutDocumentPane, IView
    {

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        public PeriodNameEditorItem periodNameEditorItem;

        public PeriodNameEditor()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.periodNameEditorItem = new PeriodNameEditorItem();
            this.Children.Add(this.periodNameEditorItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
            this.periodNameEditorItem.SetChangeEventHandler(ChangeEventHandler);

        }

    }
}
