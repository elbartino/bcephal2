using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class Role : Persistent, IHierarchyObject
    {
        private bool _isDefault;
        private string _name;
        private bool    _isSelected;

        [ScriptIgnore]
        public Role parent { get; set; }

        public PersistentListChangeHandler<Role> childrenListChangeHandler { get; set; }

        [NonSerialized]
        private System.Windows.Media.Brush foreground;


        public Role()
        {
            this.childrenListChangeHandler = new PersistentListChangeHandler<Role>();
            IsDefault = false;
        }

        [ScriptIgnore]
        public bool IsDefault 
        { 
            set
            {
                _isDefault = value;
                Foreground = value ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Black;
            }
            get
            {
                return _isDefault;
            }
        }

        [ScriptIgnore]
        public System.Windows.Media.Brush Foreground 
        {
            set
            {
                foreground = value;
            }
            get
            {
                return foreground;
            } 
        }
    
        [ScriptIgnore]
        public double FontSize { get; set; }

        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnPropertyChanged("name");
            }
        }

        [ScriptIgnore]
        public bool IsSelected
        {
            get 
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetChildByName(string name)
        {

            foreach (Role role in childrenListChangeHandler.Items)
            {
                if (role.name.ToUpper().Equals(name.ToUpper())) return role;
                IHierarchyObject ob = role.GetChildByName(name);
                if (ob != null) return ob;
            }
            return null;
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
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child)
        {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((Role)child);
            bubbleSortDesc(childrenListChangeHandler.Items);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {

            childrenListChangeHandler.AddUpdated((Role)child);
            bubbleSortDesc(childrenListChangeHandler.Items);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddDeleted((Role)child);
            bubbleSortDesc(childrenListChangeHandler.Items);
            UpdateParents();
        }

        /// <summary>
        ///
        /// </summary>
        public void UpdateParents()
        {
            if (this.parent != null)
            {
                this.parent.childrenListChangeHandler.AddUpdated(this);
                bubbleSortDesc(childrenListChangeHandler.Items);
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
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            Role role = new Role();

            role.name = "Copy Of " + this.name;
            role.IsDefault = false;
            role.parent = null;
            foreach (Role child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                role.AddChild(copy);
            }
            return role;
        }

        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(Role editedRole, string name)
        {
            if (editedRole.name.ToUpper().Equals(name.ToUpper()))
            {

                foreach (Role r in childrenListChangeHandler.Items)
                {
                    if (r.name.ToUpper().Equals(name.ToUpper()) && !r.Equals(editedRole)) return r;
                    IHierarchyObject ob = r.GetNotEditedChildByName(editedRole, name);
                    if (ob != null) return ob;
                }

            }
            return null;
        }

        public IHierarchyObject CloneObject()
        {
            Role role = new Role();
            role.name = this.name;
            role.IsDefault = false;
            role.parent = this.parent;
            role.childrenListChangeHandler = this.childrenListChangeHandler;
            return role;
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }



        /// <summary>
        /// Définit le parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(IHierarchyObject parent) { this.parent = (Role)parent; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetParent() { return this.parent; }

        /// <summary>
        /// Définit la position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(int position) {  }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPosition() { return 1; }

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
            childrenListChangeHandler.forget((Role)child);
        }

        public void bubbleSortDesc(IList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    object o1 = list[j - 1];
                    object o2 = list[j];
                    if (((IComparable)o1).CompareTo(o2) < 0)
                    {
                        list.Remove(o1);
                        list.Insert(j, o1);
                    }
                }
            }
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Role)) return 1;
            return this.name.CompareTo(((Role)obj).name);
        }
    }
    
}
