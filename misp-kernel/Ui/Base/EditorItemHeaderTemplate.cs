using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Misp.Kernel.Ui.Base
{
    public class EditorItemHeaderTemplate : DataTemplate
    {

        /// <summary>
        /// Construit une nouvelle instance de EditorItemHeaderTemplate
        /// </summary>
        public EditorItemHeaderTemplate()
        {
            this.DataType = typeof(EditorItem<>);
            FrameworkElementFactory textBoxFactory = new FrameworkElementFactory(typeof(TextBox));
            textBoxFactory.SetBinding(TextBox.TextProperty, new Binding("."));
            this.VisualTree = textBoxFactory;
        }
    }
}
