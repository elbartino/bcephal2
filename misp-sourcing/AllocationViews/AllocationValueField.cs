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

namespace Misp.Sourcing.AllocationViews
{
    public class AllocationValueField : Border
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

        public ChangeEventHandler ChangeEventHandler;

        #endregion


        #region Properties

        public PersistentListChangeHandler<TransformationTreeLoopValue> ValueListChangeHandler { get; set; }

        private bool throwEvent = true;

        public int Index { get; set; }

        public static Periodicity Periodicity { get; set; }

        public TransformationTreeItem Line { get; set; }

        public AllocationValueItemField ActiveItemField { get; set; }

        private WrapPanel Panel;

        #endregion


        #region Contructors
        
        /// <summary>
        /// 
        /// </summary>
        public AllocationValueField()
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

            ValueListChangeHandler = new PersistentListChangeHandler<TransformationTreeLoopValue>();
        }



        public AllocationValueField(int index) : this()
        {
            this.Index = index;            
        }

        /// <summary>
        /// Construit une nouvelle instance de LineItemField
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">LineItem à afficher</param>
        public AllocationValueField(TransformationTreeItem line)
            : this(line.position)
        {
            Display(line); 
        }

        #endregion


        #region Operations

        public virtual void SetReadOnly(bool readOnly)
        {
            foreach (UIElement child in Panel.Children)
            {
                if (child is AllocationValueItemField) ((AllocationValueItemField)child).SetReadOnly(readOnly);
            }
        }


        public void Display(TransformationTreeItem item)
        {
            throwEvent = false;
            this.Clear();
            this.Line = item;
            ValueListChangeHandler = new PersistentListChangeHandler<TransformationTreeLoopValue>();
            if (item == null) return;

            IList<TransformationTreeLoopValue> listeItem = item.valueListChangeHandler.getItems().Count > item.valueListChangeHandler.Items.Count ? item.valueListChangeHandler.getItems() : item.valueListChangeHandler.Items;
            ValueListChangeHandler.originalList = new List<TransformationTreeLoopValue>(listeItem);
                     
            int index = 1;
            foreach (TransformationTreeLoopValue loopValue in ValueListChangeHandler.Items)
            {
                //loopValue.updatePeriod(Periodicity);
                AllocationValueItemField itemField = new AllocationValueItemField(loopValue);
                Add(itemField);
                index++;
            }            
            throwEvent = true;
        }

        public void Fill()
        {
            foreach (TransformationTreeLoopValue loopValue in ValueListChangeHandler.deletedItems)
            {
                this.Line.RemoveValue(loopValue);
            }
            foreach (TransformationTreeLoopValue loopValue in ValueListChangeHandler.newItems)
            {
                this.Line.AddValue(loopValue);
            }
        }



        public void SetValue(object value)
        {
            if (this.Line == null) 
            { 
                if (throwEvent && Added != null) Added(this);
            }
            if (value is IList) 
            {
                var liste = value as IList;
                throwEvent = false;
                for (int i = 0; i < liste.Count; i++)
                {
                    TransformationTreeLoopValue item = new TransformationTreeLoopValue(ValueListChangeHandler.Items.Count + 1, liste[i]);
                    AllocationValueItemField field = this.Add(item);
                    if (field != null) this.ValueListChangeHandler.AddNew(field.LoopValue);
                }
                throwEvent = true;                
                if (liste.Count > 0 && throwEvent && Added != null) Updated(this);
                if (liste.Count > 0) onChange();
            }
            else
            {
                TransformationTreeLoopValue item = new TransformationTreeLoopValue(ValueListChangeHandler.Items.Count + 1, value);
                this.Add(item);
            }
        
        }

        public void SetValue(IList value) 
        {
            if (this.Line == null)
            {
                if (throwEvent && Added != null) Added(this);
            }

            
        }
        public AllocationValueItemField Add(AllocationValueItemField field)
        {
            field.Deleted += OnDeleted;
            field.Activated += OnActivated;
            Panel.Children.Add(field);
            if (throwEvent) this.ValueListChangeHandler.AddNew(field.LoopValue);
            if (throwEvent && Updated != null) Updated(this);
            onChange();
            return field;
        }

        public AllocationValueItemField Add(TransformationTreeLoopValue item)
        {
            AllocationValueItemField field = new AllocationValueItemField(item);
            return this.Add(field); ;
        }

        public void RemoveAll()
        {
            foreach (object pan in Panel.Children)
            {
                this.ValueListChangeHandler.AddDeleted(((AllocationValueItemField)pan).LoopValue);
            }
            Panel.Children.Clear();
            if (throwEvent && Updated != null) Updated(this);
            onChange();
        }

        public void Remove(AllocationValueItemField field)
        {
            Panel.Children.Remove(field);
            this.ValueListChangeHandler.AddDeleted(field.LoopValue);
            int index = 1;
            foreach (object pan in Panel.Children)
            {
                ((AllocationValueItemField)pan).Index = index++;
            }
            
            if (throwEvent && Updated != null) Updated(this);
            onChange();
        }

        public void Clear()
        {
            Panel.Children.Clear();
            if (throwEvent && Updated != null) Updated(this);
        }

        public bool IsEmpty()
        {
            return Panel.Children.Count == 0;
        }

        

        #endregion


        private void OnDeleted(object item)
        {
            AllocationValueItemField field = (AllocationValueItemField)item;
            this.Remove(field);
        }


        private void OnActivated(object item)
        {
            AllocationValueItemField field = (AllocationValueItemField)item;
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

        public void onChange()
        {
            if (throwEvent && ChangeEventHandler != null) ChangeEventHandler();
        }

    }
}
