using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Sourcing.Designer
{
    public class DimensionField : ScrollViewer 
    {
        
        
        #region Events
        
        /// <summary>
        /// 
        /// </summary>
        public event AddEventHandler Added;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;

        /// <summary>
        /// Evenement déclenché lorsqu'on clique sur le boutton pour supprimer le TargetItem.
        /// </summary>
        public event DeleteEventHandler Deleted;

        /// <summary>
        /// 
        /// </summary>
        public event ActivateEventHandler Activated;

        #endregion


        #region Properties

        private bool throwEvent = true;

        private MenuItem addLineMenuItem;
        private MenuItem removeLineMenuItem;
        
        public DesignDimension Dimension { get; set; }

        public static Periodicity Periodicity { set{ LineField.Periodicity = value;} }

        LineField activeLineField;

        public LineField ActiveLineField
        {
            get { return activeLineField; }
            set { activeLineField = value; activateField(activeLineField); }
        }
        
        private StackPanel Panel;

        bool central;
        public bool IsCentral { 
            get { return central; } 
            set 
            { 
                central = value;
                if (central) this.ContextMenu = null;
            }
        }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public DimensionField()
        {
            IsCentral = false;
            Panel = new StackPanel();
            Panel.Orientation = System.Windows.Controls.Orientation.Vertical;

            Panel.GotFocus += OnGotFocus;
            this.MouseDown += OnMouseDown;
            Panel.MouseDown += OnMouseDown;

            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.Content = Panel;
            //this.GotFocus += OnGotFocus;

            this.ContextMenu = new ContextMenu();
            addLineMenuItem = new MenuItem();
            addLineMenuItem.Header = "Add Line...";

            removeLineMenuItem = new MenuItem();
            removeLineMenuItem.Header = "Remove Line...";

            this.ContextMenu.Items.Add(addLineMenuItem);
            this.ContextMenu.Items.Add(removeLineMenuItem);
            addLineMenuItem.Click += OnAddNewLineMenuClick;
            removeLineMenuItem.Click += OnremoveLineMenuClick;

            this.ContextMenuOpening += OnContextMenuOpening;
        }

     
                
        /// <summary>
        /// 
        /// </summary>
        public DimensionField(bool isCentral) : this()
        {
            IsCentral = isCentral;
        }
        
        /// <summary>
        /// Construit une nouvelle instance de LineItemField
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">LineItem à afficher</param>
        public DimensionField(DesignDimension dimension, bool isCentral)
            : this(isCentral)
        {
            Display(dimension); 
        }

        #endregion


        #region Operations

        public void Display(DesignDimension dimension)
        {
            throwEvent = false;

            this.Clear();
            this.Dimension = dimension;                       
            int index = 1;
            
            if (Dimension == null)
            {
                this.Dimension = GetNewDimension();
                this.ActiveLineField = new LineField(index);
                Add(this.ActiveLineField);                
                return;
            }
            foreach (DesignDimensionLine line in Dimension.lineListChangeHandler.Items)
            {
                LineField filed = new LineField(line);
                Add(filed);
                this.ActiveLineField = filed;
                index++;
            }
            
            if (Dimension.lineListChangeHandler.Items.Count == 0)
            {
                LineField field = AddNewLine();
                this.Dimension.AddLine(field.Line);
            }
                        
            throwEvent = true;
        }

        public void SetValue(object value)
        {
            if (this.ActiveLineField != null)
            {
                this.ActiveLineField.SetValue(value);
                //Display(Dimension);
            }
        }

        public bool ContainsMeasure()
        {
            if (this.Dimension == null) this.Dimension = GetNewDimension();
            return this.Dimension.ContainsMeasure();
        }

        public bool ContainsPeriod()
        {
            if (this.Dimension == null) this.Dimension = GetNewDimension();
            return this.Dimension.ContainsPeriod();
        }


        public bool ActiveLineFieldContainsMeasure()
        {
            if (this.ActiveLineField == null) return false;
            if (this.ActiveLineField.Line == null) return false;
            return this.ActiveLineField.Line.ContainsMeasure();
        }

        public bool ActiveLineFieldContainsPeriod()
        {
            if (this.ActiveLineField == null) return false;
            if (this.ActiveLineField.Line == null) return false;
            return this.ActiveLineField.Line.ContainsPeriod();
        }


        public void Add(LineField field)
        {
            field.Added += OnAdded;
            field.Updated += OnUpdated;
            field.Activated += OnActivated;
            Panel.Children.Add(field);
            if (this.Dimension == null) this.Dimension = GetNewDimension();
            if (throwEvent) this.Dimension.AddLine(field.Line);
            if (throwEvent && Added != null) Updated(this);
        }
                
        public void Add(DesignDimensionLine line)
        {
            LineField field = new LineField(line);
            this.Add(field);
        }

        public void Remove(LineField field)
        {
            Panel.Children.Remove(field);
            if (this.Dimension == null) this.Dimension = GetNewDimension();
            this.Dimension.RemoveLine(field.Line);
            int index = 1;
            foreach (object pan in Panel.Children)
            {
                ((LineField)pan).Index = index++;
            }
            if (throwEvent && Updated != null) Updated(this);
        }

        public void Clear()
        {
            Panel.Children.Clear();
            if (throwEvent && Updated != null) Updated(this);
        }

        #endregion

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (IsCentral) return;
            int count = Dimension.lineListChangeHandler.Items.Count;
            addLineMenuItem.IsEnabled = count > 0
                && Dimension.lineListChangeHandler.Items[count-1].itemListChangeHandler.Items.Count > 0;
            removeLineMenuItem.IsEnabled = count > 1|| ( count > 0
                && Dimension.lineListChangeHandler.Items[count-1].itemListChangeHandler.Items.Count > 0 );
        }

        private void OnAddNewLineMenuClick(object sender, RoutedEventArgs e)
        {
            if (IsCentral) return;
            AddNewLine();
        }

        private void OnremoveLineMenuClick(object sender, RoutedEventArgs e)
        {
            if (IsCentral) return;
            RemoveLine();
        }

        private LineField AddNewLine()
        {
            DesignDimensionLine line = new DesignDimensionLine();
            int position = Dimension.lineListChangeHandler.Items.Count + 1;
            line.position = position;
            this.ActiveLineField = new LineField(line);
            Add(this.ActiveLineField);
            return this.ActiveLineField;
        }

        private LineField RemoveLine()
        {
            DesignDimensionLine line = new DesignDimensionLine();
            int count = Dimension.lineListChangeHandler.Items.Count;
            Remove(this.ActiveLineField);
            if (count == 1)
            {
                AddNewLine();
            }
            return this.ActiveLineField;
        }

        private void OnAdded(object item)
        {
            LineField field = (LineField)item;
            if (this.Dimension == null) this.Dimension = GetNewDimension();
            this.Dimension.AddLine(field.Line);            
        }

        private void OnUpdated(object item)
        {
            LineField field = (LineField)item;
            if (this.Dimension == null) this.Dimension = GetNewDimension();
            this.Dimension.UpdateLine(field.Line);   
            if (field.Line.itemListChangeHandler.Items.Count == 0 && !IsCentral && Dimension.lineListChangeHandler.Items.Count > 1)
            {
                Remove(field);
            }
            else if (throwEvent && Updated != null) Updated(this);
        }
        
        private void OnActivated(object item)
        {
            LineField field = (LineField)item;
            this.ActiveLineField = field;
            if (throwEvent && Activated != null) Activated(this);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (throwEvent && Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (throwEvent && Activated != null) Activated(this);
        }

        private DesignDimension GetNewDimension()
        {
            DesignDimension dimension = new DesignDimension();
            return dimension;
        }


        private void activateField(LineField activeLineField)
        {
            foreach (object pan in Panel.Children)
            {
                ((LineField)pan).Background = Brushes.White;
                ((LineField)pan).BorderBrush = Brushes.LightBlue;
                ((LineField)pan).BorderThickness = new Thickness(1);
            }

            if (activeLineField != null)
            {
                activeLineField.Background = Brushes.White;
                activeLineField.BorderBrush = Brushes.Gray;
                activeLineField.BorderThickness = new Thickness(2);
            } 
        }

    }
}
