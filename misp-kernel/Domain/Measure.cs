using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.Windows.Forms;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class Measure : Persistent, IHierarchyObject
    {

        private string  _name;
        private int     _position;
        private bool    _isSelected;
        private bool    _isExpanded;
        private bool    _isInEditMode;
        private bool _isFocused;

        [NonSerialized]
        private System.Windows.Media.Brush foreground;

        public Measure()
        {
            this.childrenListChangeHandler = new PersistentListChangeHandler<Measure>();
            IsDefault = false;
            this.visibleInShortcut = true;
            this.defaultData = false;
        }

        [ScriptIgnore]
        public List<Measure> Leafs
        {
            get
            {
                List<Measure> measures = new List<Measure>(0);
                foreach (Measure measure in childrenListChangeHandler.Items)
                {
                    if (measure.IsLeaf) measures.Add(measure);
                    else measures.AddRange(measure.Leafs);
                }
                return measures;
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

        [ScriptIgnore]
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set { _isInEditMode = value; }
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
                
        public int position
        {
            get { return _position; }

            set
            {
                _position = value;
                this.OnPropertyChanged("position");
            }
        }

        public bool defaultData { get; set; }

        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnPropertyChanged("name");
            }
        }

        public bool visibleInShortcut { get; set; }

        [ScriptIgnore]
        public Measure parent { get; set; }

        public PersistentListChangeHandler<Measure> childrenListChangeHandler { get; set; }

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
            get {
                 
                return _isSelected;
            }
             
            set
            {
                _isSelected = value;
                
                this.OnPropertyChanged("IsSelected");
            }
        }
        

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child) {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((Measure)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }


        public void AddChild(List<Kernel.Domain.CalculatedMeasure> listeChild , bool sort=true) 
        {
            childrenListChangeHandler.AddNew(listeChild.Cast<Measure>().ToList(),sort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(Measure measure1, Measure measure2)
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
            
            childrenListChangeHandler.AddUpdated((Measure)child);
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
                   childrenListChangeHandler.AddUpdated((Measure)item); 

                }
            }
            child.SetPosition(-1);
            childrenListChangeHandler.AddDeleted((Measure)child);
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
            childrenListChangeHandler.forget((Measure)child);
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
        public void SetParent(IHierarchyObject parent) { this.parent = (Measure) parent; }

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
        public System.Collections.IList GetItems() { return childrenListChangeHandler.Items; }//return new ObservableCollection<Measure>(from measure in childrenListChangeHandler.Items orderby measure.position select measure); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetChildByName(string name)
        {
           
            foreach (Measure measure in childrenListChangeHandler.Items)
            {
                if (measure.name.ToUpper().Equals(name.ToUpper())) return measure;
                IHierarchyObject ob = measure.GetChildByName(name);
                if (ob != null) return ob;
            }
            return null;
        }

        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(Measure editedMeasure, string name)
        {
            if(editedMeasure.name.ToUpper().Equals(name.ToUpper()))
            {

            foreach (Measure measure in childrenListChangeHandler.Items)
            {
                if (measure.name.ToUpper().Equals(name.ToUpper()) && !measure.Equals(editedMeasure)) return measure;
                IHierarchyObject ob = measure.GetNotEditedChildByName(editedMeasure, name);
                if (ob != null) return ob;
            }
            
            }
            return null;
        }


       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            Measure measure = new Measure();
         
            measure.name = "Copy Of " + this.name;
            measure.IsDefault = false;
            measure.position = -1;
            measure.parent = null;
            foreach (Measure child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                measure.AddChild(copy);
            }
            return measure;
        }
        public IHierarchyObject CloneObject() 
        {
            Measure measure = new Measure();
            measure.name =  this.name;
            measure.IsDefault = false;
            measure.position = this.position;
            measure.parent = this.parent;
            measure.childrenListChangeHandler = this.childrenListChangeHandler;
            return measure;          
        }
        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Measure)) return 1;
            return this.position.CompareTo(((Measure)obj).position);
        }
    }
}
