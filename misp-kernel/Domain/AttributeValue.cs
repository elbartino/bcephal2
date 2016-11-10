using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.ComponentModel;


namespace Misp.Kernel.Domain
{
    [Serializable]
    public class AttributeValue : Target, IHierarchyObject
    {
        
        public AttributeValue()
        {
            this.childrenListChangeHandler = new PersistentListChangeHandler<AttributeValue>();
            IsDefault = false;
        }

        [ScriptIgnore]
        public List<AttributeValue> Leafs
        {
            get
            {
                List<AttributeValue> values = new List<AttributeValue>(0);
                foreach (AttributeValue value in childrenListChangeHandler.Items)
                {
                    if (value.IsLeaf) values.Add(value);
                    else values.AddRange(value.Leafs);
                }
                return values;
            }
        }

        [ScriptIgnore]
        public bool IsLeaf
        {
            get
            {
                return childrenListChangeHandler.Items.Count == 0;
            }
        }


        [ScriptIgnore]
        public Misp.Kernel.Domain.Attribute attribut {get; set;}

        [ScriptIgnore]
        public AttributeValue parent { get; set; }

        public bool usedToGenerateUniverse { get; set; }

        protected bool isDefault;
        [ScriptIgnore]
        public bool IsDefault
        {
            set { this.isDefault = value; }
            get { return IsShowMoreItem || IsAddNewItem; }
        }

        [ScriptIgnore]
        public bool IsShowMoreItem
        {
            set;
            get;
        }

        [ScriptIgnore]
        public bool IsAddNewItem
        {
            set;
            get;
        }
        
        public PersistentListChangeHandler<AttributeValue> childrenListChangeHandler { get; set; }

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child) 
        {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((AttributeValue)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddUpdated((AttributeValue)child);
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
                    childrenListChangeHandler.AddUpdated((AttributeValue)item);
                }
            }
            child.SetPosition(-1);
            childrenListChangeHandler.AddDeleted((AttributeValue)child);
            UpdateParents();
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
            childrenListChangeHandler.forget((AttributeValue)child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(AttributeValue measure1, AttributeValue measure2)
        {
            int position = measure1.position;
            measure1.SetPosition(measure2.position);
            measure2.SetPosition(position);
            childrenListChangeHandler.AddUpdated(measure1);
            childrenListChangeHandler.AddUpdated(measure2);
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
        public void SetParent(IHierarchyObject parent) { this.parent = (AttributeValue)parent; }

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
            foreach (AttributeValue value in childrenListChangeHandler.Items)
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
        public IHierarchyObject GetChildByName(string name,Kernel.Domain.Attribute Attribute)
        {
            foreach (AttributeValue value in Attribute.valueListChangeHandler.Items)
            {
                if (value.name.ToUpper().Equals(name.ToUpper())) return value;
            
            }
            return null;
        }

        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(AttributeValue editedValue, string name)
        {
            if (editedValue.name.ToUpper().Equals(name.ToUpper()))
            {

                foreach (AttributeValue value in childrenListChangeHandler.Items)
                {
                    if (value.name.ToUpper().Equals(name.ToUpper()) && !value.Equals(editedValue)) return value;
                    IHierarchyObject ob = value.GetNotEditedChildByName(editedValue, name);
                    if (ob != null) return ob;
                }

            }
            return null;
        }

        public IHierarchyObject CloneObject()
        {
            AttributeValue attributeValue = new AttributeValue();
            attributeValue.name = this.name;
            attributeValue.IsDefault = false;
            attributeValue.position = this.position;
            attributeValue.parent = this.parent;
            attributeValue.childrenListChangeHandler = this.childrenListChangeHandler;
            return attributeValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            AttributeValue value = new AttributeValue();
            value.name = "Copy Of " + this.name;
            value.position = -1;
            value.parent = null;
            value.IsDefault = this.IsDefault;
            foreach (AttributeValue child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                value.AddChild(copy);
            }
            return value;
        }

    }
}
