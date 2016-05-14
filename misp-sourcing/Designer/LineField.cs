using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Sourcing.Designer
{
    public class LineField : Border
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

        public int Index { get; set; }

        public static Periodicity Periodicity { get; set; }

        public DesignDimensionLine Line { get; set; }

        public LineItemField ActiveItemField { get; set; }

        private WrapPanel Panel;

        #endregion


        #region Contructors
        
        /// <summary>
        /// 
        /// </summary>
        public LineField()
        {
            Panel = new WrapPanel();
            Panel.MinHeight = 20;
            this.Background = Brushes.White;
            this.BorderBrush = Brushes.LightBlue;
            this.BorderThickness = new Thickness(1);
            this.Child = Panel;

            this.GotFocus += OnGotFocus;
            Panel.GotFocus += OnGotFocus;
            this.MouseDown += OnMouseDown;
            Panel.MouseDown += OnMouseDown;
        }



        public LineField(int index) : this()
        {
            this.Index = index;            
        }

        /// <summary>
        /// Construit une nouvelle instance de LineItemField
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">LineItem à afficher</param>
        public LineField(DesignDimensionLine line)
            : this(line.position)
        {
            Display(line); 
        }

        #endregion


        #region Operations

        public void Display(DesignDimensionLine Line)
        {
            throwEvent = false;
            this.Clear();
            this.Line = Line;
            if (this.Line == null) this.Line = GetNewLine();            
            int index = 1;
            foreach (LineItem item in Line.itemListChangeHandler.Items)
            {                
                LineItemField itemField = new LineItemField(item);
                Add(itemField);
                index++;
            }            
            throwEvent = true;
        }

        public void SetValue(object value)
        {
            if (this.Line == null) 
            { 
                this.Line = GetNewLine();
                if (throwEvent && Added != null) Added(this);
            }
            if (value is IList) 
            {
                var liste = value as IList;
                throwEvent = false;
                for (int i = 0; i < liste.Count; i++)
                {
                    LineItem item = new LineItem(this.Line.itemListChangeHandler.Items.Count + 1, liste[i]);
                    LineItemField field = this.Add(item);
                    if (field != null) this.Line.AddLineItem(field.LineItem);
                }
                throwEvent = true;                
                if (liste.Count > 0 && throwEvent && Added != null) Updated(this);
            }
            else
            {
                LineItem item = new LineItem(this.Line.itemListChangeHandler.Items.Count + 1, value);
                this.Add(item);
            }
        
        }

        public void SetValue(IList value) 
        {
            if (this.Line == null)
            {
                this.Line = GetNewLine();
                if (throwEvent && Added != null) Added(this);
            }

            
        }
        public LineItemField Add(LineItemField field)
        {
            field.Deleted += OnDeleted;
            field.Activated += OnActivated;
            Panel.Children.Add(field);
            if (this.Line == null) this.Line = GetNewLine();
            if (throwEvent) this.Line.AddLineItem(field.LineItem);
            if (throwEvent && Updated != null) Updated(this);
            return field;
        }

        public LineItemField Add(LineItem item)
        {
            LineItemField field = new LineItemField(item);
            return this.Add(field); ;
        }

        public void Remove(LineItemField field)
        {
            
            Panel.Children.Remove(field);
            if (this.Line == null) this.Line = GetNewLine();
            this.Line.RemoveLineItem(field.LineItem);

            int index = 1;
            foreach (object pan in Panel.Children)
            {
                ((LineItemField)pan).Index = index++;
            }
            
            
            if (throwEvent && Updated != null) Updated(this);
        }

        public void Clear()
        {
            Panel.Children.Clear();
            if (throwEvent && Updated != null) Updated(this);
        }

        

        #endregion


        private void OnDeleted(object item)
        {
            LineItemField field = (LineItemField)item;
            this.Remove(field);
        }


        private void OnActivated(object item)
        {
            LineItemField field = (LineItemField)item;
            if (throwEvent && Activated != null) Activated(this);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (throwEvent && Activated != null) Activated(this);
        }

        private DesignDimensionLine GetNewLine()
        {
            DesignDimensionLine line = new DesignDimensionLine();
            line.position = Index;
            return line;
        }

    }
}
