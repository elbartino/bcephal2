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
    public class BGroup : Persistent, IHierarchyObject
    {

        public static String DEFAULT_GROUP_NAME = "DEFAULT";

        private string _name;
        private string subjectTyp;
        private int _position;
        private bool _isSelected;
        private bool _isExpanded;

        public BGroup()
        {
            this.childrenListChangeHandler = new PersistentListChangeHandler<BGroup>();
        }

        public BGroup(string name) : this()
        {
            this.name = name;
        }

        public string name
        {
            get { return _name; }

            set
            {
                _name = value;
                this.OnPropertyChanged("name");
            }
        }

        //[ScriptIgnore]
        public String subjectType
        {
            get { return !string.IsNullOrEmpty(subjectTyp) ? SubjectType.getByLabel(subjectTyp).label : SubjectType.DEFAULT.label; }
            set { this.subjectTyp = value != null ? SubjectType.getByLabel(value).label : SubjectType.DEFAULT.label; }
        }

        [ScriptIgnore]
        public int position
        {
            get { return _position; }

            set
            {
                _position = value;
                this.OnPropertyChanged("position");
            }
        }

        [ScriptIgnore]
        public BGroup parent { get; set; }

        public PersistentListChangeHandler<BGroup> childrenListChangeHandler { get; set; }

        [ScriptIgnore]
        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                _isExpanded = value;
                this.OnPropertyChanged("IsExpanded");
            }
        }

        [ScriptIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        [ScriptIgnore]
        public bool IsDefaultGroup
        {
            get { return string.IsNullOrEmpty(this.name) ? false : this.subjectTyp == SubjectType.DEFAULT.label; }
        }

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child)
        {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((BGroup)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(BGroup measure1, BGroup measure2)
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
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddUpdated((BGroup)child);
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
                if (item.GetPosition() > child.GetPosition()) item.SetPosition(item.GetPosition() - 1);
            }
            child.SetPosition(-1);
            childrenListChangeHandler.AddDeleted((BGroup)child);
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
            childrenListChangeHandler.forget((BGroup)child);
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
        public void SetParent(IHierarchyObject parent) { this.parent = (BGroup)parent; }

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
            foreach (BGroup group in childrenListChangeHandler.Items)
            {
                if (group.name.ToUpper().Equals(name.ToUpper())) return group;
                IHierarchyObject ob = group.GetChildByName(name);
                if (ob != null) return ob;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            BGroup group = new BGroup();
            group.name = "Copy Of " + this.name;
            group.subjectTyp = this.subjectTyp;
            group.position = -1;
            group.parent = null;
            foreach (BGroup child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                group.AddChild(copy);
            }
            return group;
        }
        
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is BGroup)) return 1;
            return this.name.CompareTo(((BGroup)obj).name);
        }

        public override string ToString()
        {
            return name;
        }

    }
}
