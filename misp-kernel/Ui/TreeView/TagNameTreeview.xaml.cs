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

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for TagNameTreeview.xaml
    /// </summary>
    public partial class TagNameTreeview : UserControl
    {
        /// <summary>
        /// Evènement du TagNameTreeview qui renvoit le Tag selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;


        /// <summary>
        /// Evènement du TagNameTreeview qui renvoit le tag sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        /// <summary>
        /// Contruit une nouvelle instance de TagTreeview
        /// </summary>
        public TagNameTreeview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayTags(List<Kernel.Domain.TagName> tags)
        {
            if (tags == null) this.tagNameTreeview.ItemsSource = null;
            else
            {
                this.tagNameTreeview.ItemsSource = tags;
            }
        }


        /// <summary>
        /// Methode de selection du treeview qui renvoit l'élément selectionné
        /// et cet élément un inputTable est transmis au InputTableTreeview 
        /// par l'évènement OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeClick(object sender, MouseButtonEventArgs e)
        {
            if (tagNameTreeview.SelectedItem != null && tagNameTreeview.SelectedItem is Kernel.Domain.TagName && SelectionChanged != null)
            {
                if (e.ClickCount == 1)
                    SelectionChanged(tagNameTreeview.SelectedItem as Kernel.Domain.TagName);
                else if (e.ClickCount == 2)
                    SelectionDoubleClick(tagNameTreeview.SelectedItem as Kernel.Domain.TagName);
                e.Handled = true;
            }
        }
    }
}
