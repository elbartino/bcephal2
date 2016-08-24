using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Threading;
using System.Timers;
using System.Diagnostics;
using System.Collections;

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for EntityTreeview.xaml
    /// </summary>
    public partial class EntityTreeview : UserControl
    {

        /// <summary>
        /// Evènement du EntityTreeview qui renvoit l'entity selectionné
        /// </summary>
        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        /// <summary>
        /// Evènement du EntityTreeview qui renvoit l'entity sur lequel on
        ///  a double cliqué.
        /// </summary>
        public event Base.SelectedItemDoubleClickEventHandler SelectionDoubleClick;

        public event Base.SelectedItemChangedEventHandler OnRightClick;

        public event Base.SelectedItemChangedEventHandler ExpandAttribute;

        public ObservableCollection<Entity> liste = new ObservableCollection<Entity>();
        public ObservableCollection<Misp.Kernel.Domain.Attribute> listeAttrib = new ObservableCollection<Domain.Attribute>();
        public ObservableCollection<Misp.Kernel.Domain.AttributeValue> listeAttribValue = new ObservableCollection<Domain.AttributeValue>();
        private DispatcherTimer myClickWaitTimer;
        private bool isDoubleClick = false;
        private CollectionViewSource cvs = new CollectionViewSource();
        public CollectionViewSource CVS
        {
            get
            {
                return this.cvs;
            }
        }
        public EntityTreeview()
        {
            InitializeComponent();
            myClickWaitTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Background, mouseWaitTimer_Tick, Dispatcher.CurrentDispatcher);
            myClickWaitTimer.Stop();
        }

        public void setDisplacherInterval(TimeSpan timeSpan) 
        {
            myClickWaitTimer.Interval = timeSpan;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayModels(List<Model> models)
        {
            
            foreach (Model model in models)
            {
                model.entityListChangeHandler.originalList = model.GetAllEntities();
                RefreshModel(model);
            }
            
            this.entityTreeview.ItemsSource = models;
        }

        private void RefreshModel(Model model)
        {
            foreach (Entity entity in model.entityListChangeHandler.Items)
            {
                entity.model = model;
                RefreshEntity(entity);
            }
        }

        private void RefreshEntity(Entity entity)
        {
            foreach (Kernel.Domain.Attribute attribute in entity.attributeListChangeHandler.Items)
            {
                attribute.entity = entity;
                RefreshAttribute(attribute);
            }
        }

        private void RefreshAttribute(Kernel.Domain.Attribute attribute)
        {
            foreach (AttributeValue attributeValue in attribute.valueListChangeHandler.Items)
            {
                attributeValue.attribut = attribute;
                RefreshAttributeValue(attributeValue);
                this.listeAttribValue.Add(attributeValue);
            }
        }

        private void RefreshAttributeValue(AttributeValue value)
        {
            foreach (AttributeValue attributeValue in value.childrenListChangeHandler.Items)
            {
                attributeValue.parent = value;
                RefreshAttributeValue(attributeValue);
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

            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
                List<object> listeToSende = new List<object>(0);
                listeToSende.Add(entityTreeview);
                listeToSende.Add(SelectionChanged);
                myClickWaitTimer.Start();
            }
        }

        private void OnTreeNodeDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
                // Stop the timer from ticking.
                myClickWaitTimer.Stop();
                
                isDoubleClick = true;
                e.Handled = true;
                if (entityTreeview.SelectedItem != null && entityTreeview.SelectedItem is Target && SelectionDoubleClick != null)
                {
                    int count = e.ClickCount;
                    SelectionDoubleClick(entityTreeview.SelectedItem as Target);
                    e.Handled = true;
                }
            }            
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeviewItem = (TreeViewItem)sender;
            if (treeviewItem.Header != null && treeviewItem.Header is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)treeviewItem.Header;
                if (ExpandAttribute != null && !attribute.LoadValues) ExpandAttribute(attribute);
            }
            e.Handled = true;
        }
        
        private  void mouseWaitTimer_Tick(object sender, EventArgs e)
        {
            myClickWaitTimer.Stop();
            if (!isDoubleClick)
            {
                clickMethod();
            }
            isDoubleClick = false;
        }

        private  void clickMethod() 
        {

            if (entityTreeview.SelectedItem != null && entityTreeview.SelectedItem is Target && SelectionChanged != null)
                {
                    SelectionChanged(entityTreeview.SelectedItem as Target);
                    //e.Handled = true;
                }
        }
       

        private void OnMouseDownClickCount(object sender, MouseButtonEventArgs e)
        {
            // Checks the number of clicks. 
            if (e.ClickCount == 1)
            {
                // Single Click occurred.
                 //string Content = "Single Click";
            }
            if (e.ClickCount == 2)
            {
                // Double Click occurred.
                //string Content = "Double Click";
            }
            if (e.ClickCount >= 3)
            {
                // Triple Click occurred.
                //string Content = "Triple Click";
            }
        }

        private void entityTreeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Target SelectedItem = entityTreeview.SelectedItem as Target;
            
        }

        private void OnTreeNodeRightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                if (((TreeViewItem)sender).Header is Kernel.Domain.Target)
                {
                    popup.Tag = ((TreeViewItem)sender).Header as Target;
                    if (OnRightClick != null) OnRightClick(popup);
                    e.Handled = true;
                }
            }
        }

        
    } 
}
