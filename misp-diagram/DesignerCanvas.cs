using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;
using System.Xml.Linq;

namespace DiagramDesigner
{
    public partial class DesignerCanvas : Canvas
    {
        public event AddBlockEventHandler       AddBlock;
        public event DeleteBlockEventHandler    DeleteBlock;
        public event ModifyBlockEventHandler    ModifyBlock;
        public event AddLinkEventHandler        AddLink;
        public event DeleteLinkEventHandler     DeleteLink;

        public event MoveLinkSourceEventHandler MoveLinkSource;
        public event MoveLinkTargetEventHandler MoveLinkTarget;

        //public event SelectionChangeEventHandler SelectionChange;
        public event ZoomEventHandler           Zoomed;
        public event ChangeEventHandler         Changed;
        

        private Point? rubberbandSelectionStartPoint = null;

        private SelectionService selectionService;
        public SelectionService SelectionService
        {
            get
            {
                if (selectionService == null)
                {
                    selectionService = new SelectionService(this);
                }
                return selectionService;
            }
        }

        public string AsString()
        {
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            XElement designerItemsXML = SerializeDesignerItems(designerItems);
            XElement connectionsXML = SerializeConnections(connections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);
            
            return root.ToString();
        }

        public void Display(string xml)
        {
            XElement root = XElement.Load(new StringReader(xml));
            if (root == null)
                return;

            this.Children.Clear();
            this.SelectionService.ClearSelection();

            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
                DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);
                item.Edition += new DesignerItem.RoutedEventHandler(onEdit);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
            }

            this.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                this.Children.Add(connection);
            }
        }

        public double Scale { get; set; }
        
        public void Zoom(double scale)
        {
            this.LayoutTransform = new System.Windows.Media.ScaleTransform(scale, scale);
            if (Zoomed != null) Zoomed();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                // in case that this click is the start of a 
                // drag operation we cache the start point
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));
                Focus();
                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                SelectionService.ClearSelection();
           
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
        }

        protected DesignerItem DisplayBlock(UIElement block, object tag)
        {
            DesignerItem newItem = GetNewDesignerItem(Guid.NewGuid());
            newItem.SetContent(block);
            newItem.Tag = tag;
            newItem.Renderer.Text = tag != null ? tag.ToString() : newItem.Renderer.Text + newItem.ID;

            newItem.Edition += new DesignerItem.RoutedEventHandler(onEdit);

            Point p = Mouse.GetPosition(this);

            var n = Scale + 1;
            newItem.Width = DesignerItem.BlockWidth * n;
            newItem.Height = DesignerItem.BlockHeight * n;

            if (point != null && point.HasValue)
            {
                DesignerCanvas.SetLeft(newItem, Math.Max(0, p.X));
                DesignerCanvas.SetTop(newItem, Math.Max(0, p.Y));
            }
            else
            {
                DesignerCanvas.SetLeft(newItem, Math.Max(0, p.X - newItem.Width / 2));
                DesignerCanvas.SetTop(newItem, Math.Max(0, p.Y - newItem.Height / 2));
            }

            Canvas.SetZIndex(newItem, this.Children.Count);
            this.Children.Add(newItem);
            SetConnectorDecoratorTemplate(newItem);

            this.SelectionService.SelectItem(newItem);
            newItem.Focus();
            return newItem;
        }

        protected void AddNewBlock(UIElement block, object tag)
        {
            DesignerItem newItem = DisplayBlock(block, tag);
            if (AddBlock != null) AddBlock(newItem);
            if (Changed != null) Changed();
        }

        protected virtual void onEdit(DesignerItem item, string name)
        {
            
        }

        public virtual void TryToAddNewConnection(Connector sourceConnector, Connector sinkConnector)
        {
            if (this.CanAddLinkBetween(sourceConnector.ParentDesignerItem, sinkConnector.ParentDesignerItem))
            {
                Connection newConnection = new Connection(sourceConnector, sinkConnector);
                Canvas.SetZIndex(newConnection, this.Children.Count);
                this.Children.Add(newConnection);
                if (AddLink != null) AddLink(sourceConnector.ParentDesignerItem, sinkConnector.ParentDesignerItem);
                if (Changed != null) Changed();
            }
        }

        protected virtual bool CanAddLinkBetween(DesignerItem source, DesignerItem cible)
        {
            return true;
        }



        public virtual void OnMoveLinkSource(Connector sourceConnector, Connector sinkConnector, Connector newSourceConnector)
        {
            if (MoveLinkSource != null) MoveLinkSource(sourceConnector.ParentDesignerItem, sinkConnector.ParentDesignerItem, newSourceConnector.ParentDesignerItem);
            if (Changed != null) Changed();
        }

        public virtual void OnMoveLinkTarget(Connector sourceConnector, Connector sinkConnector, Connector newTargetConnector)
        {
            if (MoveLinkSource != null) MoveLinkTarget(sourceConnector.ParentDesignerItem, sinkConnector.ParentDesignerItem, newTargetConnector.ParentDesignerItem);
            if (Changed != null) Changed();
        }

        public virtual bool CanMoveLinkSource(DesignerItem source, DesignerItem target, DesignerItem newSource)
        {
            return true;
        }

        public virtual bool CanMoveLinkTarget(DesignerItem source, DesignerItem target, DesignerItem newTarget)
        {
            return true;
        }

        public virtual void AddNewObject() { }

        public virtual void AddNewValueChain() { }

        public virtual DiagramDesigner.DesignerItem GetBlockByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            foreach (UIElement item in this.Children)
            {
                if (item is DiagramDesigner.DesignerItem)
                {
                    object tag = ((DiagramDesigner.DesignerItem)item).Tag;
                    if (tag != null && tag.ToString().ToUpper().Equals(name.ToUpper())) return (DiagramDesigner.DesignerItem)item;
                }
            }
            return null;
        }

        protected virtual bool IsBlockNameValid(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            foreach (UIElement item in this.Children)
            {
                if (item is DiagramDesigner.DesignerItem)
                {
                    object tag = ((DiagramDesigner.DesignerItem)item).Tag;
                    if (tag != null && tag.ToString().ToUpper().Equals(name.ToUpper())) return false;
                }
            }
            return true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
            {
                DesignerItem newItem = null;
                Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

                if (content != null)
                {
                    newItem = GetNewDesignerItem(Guid.NewGuid());
                    newItem.SetContent(content);

                    Point position = e.GetPosition(this);

                    if (dragObject.DesiredSize.HasValue)
                    {
                        Size desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                    }
                    else
                    {
                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    }

                    Canvas.SetZIndex(newItem, this.Children.Count);
                    this.Children.Add(newItem);                    
                    SetConnectorDecoratorTemplate(newItem);

                    //update selection
                    this.SelectionService.SelectItem(newItem);
                    newItem.Focus();
                }

                e.Handled = true;
            }
        }


        protected virtual DesignerItem GetNewDesignerItem(Guid id)
        {
            DesignerItem item = new DesignerItem(id);
            return item;
        }


        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;
                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        protected void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.ApplyTemplate() && item.Content is UIElement)
            {
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
                Control decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if (decorator != null && template != null)
                    decorator.Template = template;
            }
        }

        public void notifyAddBlock(DesignerItem item)
        {
            if (AddBlock != null) AddBlock(item);
            if (Changed != null) Changed();
        }

        public void notifyModifyBlock(DesignerItem item)
        {
            if (ModifyBlock != null) ModifyBlock(item);
            if (Changed != null) Changed();
        }

        public void OnChange()
        {
            if (Changed != null) Changed();
        }

    }
}
