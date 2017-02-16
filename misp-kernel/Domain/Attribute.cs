using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EO.Wpf;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class Attribute : Target, IHierarchyObject
    {

        #region Fields


        #endregion


        #region Properties

        [ScriptIgnore]
        public List<AttributeValue> FilterAttributeValues { get; set; }

        [ScriptIgnore]
        public Attribute parent { get; set; }

        [ScriptIgnore]
        public String parentId { get { return parent != null ? parent.name : null; } set { } }

        [ScriptIgnore]
        public Entity entity { get; set; }

        [ScriptIgnore]
        public bool LoadValues { get; set; }

        public PersistentListChangeHandler<Attribute> childrenListChangeHandler { get; set; }

        public PersistentListChangeHandler<AttributeValue> valueListChangeHandler { get; set; }

        #endregion


        #region Constructors

        public Attribute()
        {
            this.childrenListChangeHandler = new PersistentListChangeHandler<Attribute>();
            this.valueListChangeHandler = new PersistentListChangeHandler<AttributeValue>();
            this.FilterAttributeValues = new List<AttributeValue>(0);
            LoadValues = false;
            setAttributeToAttributeValue();
            IsDefault = false;
            IsAddNewItem = false;
            IsShowMoreItem = false;
            this.Items = new ObservableCollection<Target>();
        }

        #endregion

        

        

        [ScriptIgnore]
        public List<AttributeValue> LeafAttributeValues
        {
            get
            {
                List<AttributeValue> values = new List<AttributeValue>(0);
                foreach (AttributeValue value in valueListChangeHandler.Items)
                {
                    if (value.IsLeaf) values.Add(value);
                    else values.AddRange(value.Leafs);
                }
                return values;
            }
        }

        

        

        /// <summary>
        /// Méthode qui assigne une valeur à la property attribut de attributeValue
        /// </summary>
        public void setAttributeToAttributeValue() 
        {
            if (!LoadValues) return;
            foreach (AttributeValue attributeValue in valueListChangeHandler.Items)
            {
                attributeValue.attribut = this;  
            }
        }

        private AttributeValue root;
        public AttributeValue GetRootValue()
        {
            if (this.valueListChangeHandler.Items.Count == 0)
                this.root = null;

            if (this.root == null)
            {
                root = new AttributeValue();
                root.name = "Root Value";                           
            }
            root.childrenListChangeHandler = valueListChangeHandler;     
            return root;
        }


        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child) 
        {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((Attribute)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddUpdated((Attribute)child);
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
                    childrenListChangeHandler.AddUpdated((Attribute)item);
                }
            }
            child.SetPosition(-1);
            Attribute Child = (Attribute)child;

            if (Child.valueListChangeHandler.Items.Count > 0)
            {
                AttributeValue root = Child.valueListChangeHandler.Items[0].parent;
                for(int i = root.GetItems().Count -1 ; i>=0;i--)
                {
                    AttributeValue value = root.childrenListChangeHandler.Items[i];
                    root.RemoveChild(value);
                }
                Child.removeDefaultValue();
            }
            childrenListChangeHandler.AddDeleted(Child);
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
            childrenListChangeHandler.forget((Attribute)child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(Attribute measure1, Attribute measure2)
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
        public void SetParent(IHierarchyObject parent) { this.parent = (Attribute)parent; }

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
            if (this.entity != null)
            {
                int pos = 0;
                foreach (Attribute attribute in this.entity.attributeListChangeHandler.Items)
                {
                    attribute.SetPosition(pos);
                    if (attribute.name.ToUpper().Equals(name.ToUpper())) return attribute;
                    pos++;
                }  
            }
            foreach (Attribute attribute in childrenListChangeHandler.Items)
            {
                if(attribute.name.ToUpper().Equals(name.ToUpper())) return attribute;
                IHierarchyObject ob = attribute.GetChildByName(name);
                if (ob != null) return ob;
            }
            return null;
        }


        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(Attribute editedAttribute, string name)
        {
            if (editedAttribute!=null && editedAttribute.name.ToUpper().Equals(name.ToUpper()))
            {

                foreach (Attribute attribute in childrenListChangeHandler.Items)
                {
                    if (attribute.name.ToUpper().Equals(name.ToUpper()) && !attribute.Equals(editedAttribute)) return attribute;
                    IHierarchyObject ob = attribute.GetNotEditedChildByName(editedAttribute, name);
                    if (ob != null) return ob;
                }

            }
            return null;
        }

        public IHierarchyObject CloneObject()
        {
            Attribute attribute = new Attribute();
            attribute.name = this.name;
            attribute.IsDefault = false;
            attribute.position = -1;
            attribute.parent = null;
            attribute.valueListChangeHandler = this.valueListChangeHandler;
            return attribute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            Attribute attribute = new Attribute();
            attribute.name = "Copy Of " + this.name;
            attribute.entity = this.entity;
            attribute.position = -1;
            attribute.parent = null;
            attribute.IsDefault = this.IsDefault;
          /*  foreach (Attribute child in this.childrenListChangeHandler.Items)
            {
                Attribute copy = (Attribute)child.GetCopy();
                if (!copy.IsDefault)
                {
                    attribute.AddChild(copy);
                }
            }*/
            foreach (AttributeValue value in this.valueListChangeHandler.Items)
            {
                AttributeValue copy = (AttributeValue)value.GetCopy();
                if (!copy.IsDefault)
                {
                    copy.position = value.position;
                    attribute.valueListChangeHandler.AddNew(copy);
                }
            }
            return attribute;
        }


        public IHierarchyObject removeDefaultValue()
        {
            AttributeValue defaultValue = null;
            foreach (AttributeValue value in this.valueListChangeHandler.Items)
            {
                if (value.IsDefault)
                {
                    defaultValue = value;
                    break;
                }
            }
            if (defaultValue != null)
            {
                this.valueListChangeHandler.newItems.Remove(defaultValue); 
                this.valueListChangeHandler.Items.Remove(defaultValue);
            }
            return this;
        }


        [ScriptIgnore]
        public ObservableCollection<Target> Items { get; set; }

        public void LoadItems()
        {
            foreach (Attribute attribute in this.childrenListChangeHandler.Items)
            {
                this.Items.Add(attribute);
            }
            foreach (AttributeValue value in this.valueListChangeHandler.Items)
            {
                this.Items.Add(value);
            }
        }

        public void ClearValuesInItems()
        {
            foreach (AttributeValue value in this.valueListChangeHandler.Items)
            {
                this.Items.Remove(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;

            if (obj is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute objm = (Kernel.Domain.Attribute)obj;
                if (objm.oid.HasValue && this.oid.HasValue)
                {
                    if (objm.oid == this.oid) return true;
                }
                if (objm.name != null && objm.name.Equals(this.name)) return true;
            }
            return false;
        }
    }
}
