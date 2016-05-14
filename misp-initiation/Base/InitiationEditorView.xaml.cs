using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Initiation.Base
{
    /// <summary>
    /// Interaction logic for InitiationEditorView.xaml
    /// </summary>
    public partial class InitiationEditorView : LayoutDocumentPane, IView
    {
        public InitiationEditorView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

    }
}
