using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesigner.Controls;

namespace DiagramDesigner
{
    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable
    {
        public static int BlockWidth = 70;
        public static int BlockHeight = 45;

        #region ID
        private Guid id;
        public Guid ID
        {
            get { return id; }
        }
        #endregion

        #region ParentID
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(DesignerItem));
        #endregion

        #region IsGroup
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion

        #region ConnectorDecoratorTemplate Property

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public Object Object { get; set; }
        public TextBox Editor { get; set; }
        public TextBlock Renderer { get; set; }

        public RoutedEventHandler Editing;
        public RoutedEventHandler Edition;
        public delegate void RoutedEventHandler(DesignerItem item, string name);

        public DesignerItem(Guid id)
        {
            this.id = id;
            this.Loaded += new System.Windows.RoutedEventHandler(DesignerItem_Loaded);            
        }

        public DesignerItem() : this(Guid.NewGuid()) { }
        
        public void SetContent(Object content)
        {
            this.Content = content;
            if (Content is Grid)
            {
                Grid grid = (Grid)Content;
                foreach (UIElement element in grid.Children)
                {
                    if (element is TextBox) Editor = (TextBox)element;
                    else if (element is TextBlock) Renderer = (TextBlock)element;
                }
            }
            InitEditorHandlers();
        }

        public virtual void OnEditing()
        {
            if (Editing != null) Editing(this, Renderer.Text);
            else Edit();
        }


        public virtual void Edit()
        {
            if (Editor != null && !Editor.IsVisible)
            {
                Editor.Text = Renderer.Text;
                Editor.Visibility = System.Windows.Visibility.Visible;
                Renderer.Visibility = System.Windows.Visibility.Hidden;
                Editor.SelectAll();
                Editor.Focus();
            }
        }

        public void ValidateEdition()
        {
            if (Editor != null)
            {
                Renderer.Text = Editor.Text;
                CancelEdition();
                if (Edition != null)
                {                    
                    Edition(this, Renderer.Text);
                }
            }
        }

        public void CancelEdition()
        {
            if (Editor != null)
            {
                Renderer.Visibility = System.Windows.Visibility.Visible;
                Editor.Visibility = System.Windows.Visibility.Hidden;                
            }
        }


        protected void InitEditorHandlers()
        {
            if (Editor == null)
            {
                Editor = new TextBox();
                Editor.Visibility = System.Windows.Visibility.Hidden;
            }
            if (Renderer == null)
            {
                Renderer = new TextBlock();
                Renderer.Text = "Renderer";
            }
            
            MouseDoubleClick += new MouseButtonEventHandler(edit);
            Renderer.MouseLeftButtonUp += new MouseButtonEventHandler(edit);
            Editor.LostFocus += new System.Windows.RoutedEventHandler(cancelEdition);
            Editor.KeyUp += new KeyEventHandler(validateEdition);
            Renderer.MouseEnter += new MouseEventHandler(mouseEnter);
        }

        protected void mouseEnter(object sender, MouseEventArgs args)
        {
            //Cursor = Cursors.SizeAll;
            Cursor = Cursors.Pen;
        }

        protected void tryToEdit(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 2) OnEditing();
        }

        protected void edit(object sender, MouseButtonEventArgs args)
        {
            OnEditing();
        }



        protected void validateEdition(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                CancelEdition();
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEdition();
            }
        }

        public void cancelEdition(object sender, RoutedEventArgs args)
        {
            CancelEdition();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            // update selection
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        designer.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                }
                //Focus();
            }

            e.Handled = false;
        }

        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null && VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    if (contentVisual != null)
                    {
                        DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                        if (thumb != null)
                        {
                            ControlTemplate template =
                                DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                            if (template != null)
                                thumb.Template = template;
                        }
                    }
                }
            }
        }
 
    }
}
