using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TagTreeview.xaml
    /// </summary>
    public partial class TagTreeview : UserControl
    {
        /// <summary>
        /// Evènement du TagTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du TagTreeview qui renvoit le tag sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;


        public ObservableCollection<Kernel.Domain.TagItem> liste = new ObservableCollection<Kernel.Domain.TagItem>();
        private CollectionViewSource cvs = new CollectionViewSource();

        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }


        /// <summary>
        /// Contruit une nouvelle instance de TagTreeview
        /// </summary>
        public TagTreeview()
        {
            InitializeComponent();
            this.cvs.Source = this.liste;
            this.cvs.GroupDescriptions.Add(new PropertyGroupDescription("name"));
            this.DataContext = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayTags(List<Kernel.Domain.Tag> tags)
        {
            if (tags == null) this.tagTreeview.ItemsSource = null;
            else
            {
                this.tagTreeview.ItemsSource = tags;
            }
        }

        /// <summary>
        /// Remplir le treeview avec une liste de tag items
        /// </summary>
        /// <param name="listeInputTable">la liste de tag items</param>
        public void FillTree(ObservableCollection<Kernel.Domain.TagItem> items)
        {
            this.liste = items;
            this.cvs.Source = this.liste;
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
            if (tagTreeview.SelectedItem != null && SelectionChanged != null)
            {
                if (tagTreeview.SelectedItem is Kernel.Domain.Tag)
                {
                    SelectionChanged(tagTreeview.SelectedItem as Kernel.Domain.Tag);
                    e.Handled = true;
                }
                else if (tagTreeview.SelectedItem is Kernel.Domain.TagItem)
                {

                    SelectionChanged(tagTreeview.SelectedItem as Kernel.Domain.TagItem);
                    e.Handled = true;
                }
                else if(tagTreeview.SelectedItem is CollectionViewGroup)
                {
                    SelectionChanged(tagTreeview.SelectedItem);
                    e.Handled = true;
                }
            }
        }
        private void OnTreeNodeDoubleClick(object sender, MouseButtonEventArgs e) 
        {
            if (tagTreeview.SelectedItem != null && SelectionDoubleClick != null)
            {
                if (tagTreeview.SelectedItem is Kernel.Domain.Tag)
                {
                    SelectionDoubleClick(tagTreeview.SelectedItem as Kernel.Domain.Tag);
                    e.Handled = true;
                }
                else if (tagTreeview.SelectedItem is Kernel.Domain.TagItem)
                {
                    SelectionDoubleClick(tagTreeview.SelectedItem as Kernel.Domain.TagItem);
                    e.Handled = true;
                }
            }
        }
 
    }
}
