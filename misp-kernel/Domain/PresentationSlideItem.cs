using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class PresentationSlideItem : Persistent, IComparable,IHierarchyObject
    {
        public string name { get; set; }

        //Shape index
        public int index { get; set; }

        [ScriptIgnore]
        public int position { get; set; }


        [ScriptIgnore]
        public float top { get; set; }

        [ScriptIgnore]
        public float left { get; set; }

        [ScriptIgnore]
        public float width { get; set; }

        [ScriptIgnore]
        public float heigth { get; set; }

        public string value { get; set; }

        //Shape type
        public Kernel.Application.SlideItemType type { get; set; }

        public PersistentListChangeHandler<PresentationSlideItem> childrenListChangeHandler;

        [ScriptIgnore]
        public PresentationSlideItem parent { get; set; }

        public PresentationSlideItem() 
        {
            childrenListChangeHandler = new PersistentListChangeHandler<PresentationSlideItem>();
        }

        public PresentationSlideItem(int _index)
            : this()
        {
            this.index = _index;
        }


        public PresentationSlideItem(int _index, Kernel.Application.SlideItemType _type)
            : this(_index)
        {
            this.type = _type;
        }

        public PresentationSlideItem(int _index, string _value, Kernel.Application.SlideItemType _type)
            : this(_index,_type)
        {
            this.value = _value;
        }

       
        [ScriptIgnore]
        public bool IsLeaf
        {
            get
            {
                return childrenListChangeHandler.Items.Count == 0;
            }
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is PresentationSlideItem)) return 1;
            return this.index.CompareTo(((PresentationSlideItem)obj).index);
        }

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child)
        {
            child.SetPosition(childrenListChangeHandler.Items.Count+1);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((PresentationSlideItem)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }
        

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddUpdated((PresentationSlideItem)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IHierarchyObject child)
        {
            foreach (IHierarchyObject item in childrenListChangeHandler.Items)
            {
                if (item.GetPosition() > child.GetPosition())
                {
                    item.SetPosition(item.GetPosition() - 1);
                    childrenListChangeHandler.AddUpdated((PresentationSlideItem)item);
                }
            }

            child.SetPosition(-1);
            childrenListChangeHandler.AddDeleted((PresentationSlideItem)child);
            UpdateParents();
        }


        public Kernel.Domain.PresentationSlideItem GetRoot()
        {
            if (parent != null)
            {
                return parent.GetRoot();
            }
            return this;
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetChild(IHierarchyObject child)
        {
            foreach (IHierarchyObject item in childrenListChangeHandler.Items)
            {
                if (item.GetPosition() > child.GetPosition()) item.SetPosition(item.GetPosition() - 1);
            }
            child.SetPosition(-1);
            childrenListChangeHandler.forget((PresentationSlideItem)child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(PresentationSlideItem shape1, PresentationSlideItem shape2)
        {
            int position = shape1.position;
            shape1.SetPosition(shape1.position);
            shape2.SetPosition(position);
            childrenListChangeHandler.AddUpdated(shape1);
            childrenListChangeHandler.AddUpdated(shape2);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetChildByPosition(int position)
        {
            foreach (IHierarchyObject item in childrenListChangeHandler.Items)
            {
                if (item.GetPosition() == position) return item;
            }
            return null;
        }

        /// <summary>
        /// Définit le parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(IHierarchyObject parent) { this.parent = (PresentationSlideItem)parent; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetParent() { return this.parent; }

        /// <summary>
        /// Définit la position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(int position) { this.position = position; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPosition() { return this.position; }

        /// <summary>
        ///
        /// </summary>
        public void UpdateParents()
        {
            if (this.parent != null)
            {
                this.parent.childrenListChangeHandler.AddUpdated(this);
                this.parent.UpdateParents();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.IList GetItems() { return childrenListChangeHandler.Items; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetChildByName(string name)
        {
            foreach (PresentationSlideItem value in childrenListChangeHandler.Items)
            {
                if (value.name.ToUpper().Equals(name.ToUpper())) return value;
                IHierarchyObject ob = value.GetChildByName(name);
                if (ob != null) return ob;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetChildByName(string name, Kernel.Domain.PresentationSlide slide)
        {
            foreach (PresentationSlideItem shape in slide.slideItemsListChangeHandler.Items)
            {
                if (shape.name.ToUpper().Equals(name.ToUpper())) return shape;

            }
            return null;
        }

        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(PresentationSlideItem editedValue, string name)
        {
            if (editedValue.name.ToUpper().Equals(name.ToUpper()))
            {

                foreach (PresentationSlideItem slide in childrenListChangeHandler.Items)
                {
                    if (slide.name.ToUpper().Equals(name.ToUpper()) && !slide.Equals(editedValue)) return slide;
                    IHierarchyObject ob = slide.GetNotEditedChildByName(editedValue, name);
                    if (ob != null) return ob;
                }

            }
            return null;
        }

        public IHierarchyObject CloneObject()
        {
            PresentationSlideItem slide = new PresentationSlideItem();
            slide.name = this.name;
            slide.position = this.position;
            slide.parent = this.parent;
            slide.childrenListChangeHandler = this.childrenListChangeHandler;
            return slide;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            PresentationSlideItem slide = new PresentationSlideItem();
            slide.name = "Copy Of " + this.name;
            slide.position = -1;
            slide.parent = null;

            foreach (PresentationSlideItem child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                slide.AddChild(copy);
            }
            return slide;
        }
    }
}
