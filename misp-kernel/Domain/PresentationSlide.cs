using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class PresentationSlide : Persistent, IComparable
    {
        public string name { get; set; }

        //slide Position
        public int position { get; set; }


        [ScriptIgnore]
        public Presentation parent { get; set; }

        public PersistentListChangeHandler<PresentationSlideItem> slideItemsListChangeHandler;
        
        public PresentationSlide()
        {
            slideItemsListChangeHandler = new PersistentListChangeHandler<PresentationSlideItem>();
        }

        public PresentationSlide(int _position , string _name) :this()
        {
            this.name = _name;
            this.position = _position;
        }

        public PresentationSlide(int _position) : this() 
        {
            this.position = _position;
        }
        

        public PresentationSlideItem getShape(int index) 
        {
            foreach (PresentationSlideItem item in slideItemsListChangeHandler.Items)
            {
                if (item.index == index) return item;
            }
            return null;
        }

        public void AddShape(PresentationSlideItem shape)
        {
            shape.SetPosition(slideItemsListChangeHandler.Items.Count + 1);
            slideItemsListChangeHandler.AddNew(shape);
        }

        public void UpdateShape(PresentationSlideItem shape)
        {
            slideItemsListChangeHandler.AddUpdated(shape);
        }

        public void DeleteShape(PresentationSlideItem shape)
        {
            foreach (PresentationSlideItem item in slideItemsListChangeHandler.Items)
            {
                if (item.GetPosition() > shape.GetPosition())
                {
                    item.SetPosition(item.GetPosition() - 1);
                    slideItemsListChangeHandler.AddUpdated(item);
                }
            }

            shape.SetPosition(-1);
            slideItemsListChangeHandler.AddDeleted(shape);
        }


        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is PresentationSlide)) return 1;
            return this.position.CompareTo(((PresentationSlide)obj).position);
        }

        /// <summary>
        /// Définit la position
        /// </summary>
        
        public PresentationSlide CloneObject()
        {
            PresentationSlide slide = new PresentationSlide();
            slide.name = this.name;
            slide.position = this.position;
            slide.parent = this.parent;
            slide.slideItemsListChangeHandler = this.slideItemsListChangeHandler;
            return slide;
        }

        public PresentationSlideItem containsShape(int position)
        {
            if (slideItemsListChangeHandler == null)
            slideItemsListChangeHandler = new PersistentListChangeHandler<PresentationSlideItem>();
            foreach (PresentationSlideItem item in slideItemsListChangeHandler.getItems())
            {
                if (item == null) continue;
                if (position == item.index) return item;
            }
            return null;
        }
      
    }
}
