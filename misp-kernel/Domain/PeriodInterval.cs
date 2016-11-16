using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// Une période est un sous élément de la périodicité.
    /// Une période est définie par un nom, une date de début et une date de fin.
    /// 
    /// Le nom d'une période dépend du type de la périodicité dont elle est issue:
    /// 
    /// - YEAR => YYYY (les 4 chiffres de l’année). Exemple: 2014
    /// - SEMESTER => S1/S2 + les 4 chiffres de l’année. Exemple: S1/2014
    /// - QUARTER => Q1/Q2/Q3/Q4  + les 4 chiffres de l’année. Exemple: Q3/2014
    /// - MONTH => MM/YYYY. Exemple: 08/2014
    /// - DAY => DD/MM/YYYY. Exemple: 15/08/2014
    /// 
    /// </summary>
    public class PeriodInterval : Persistent,IHierarchyObject
    {
        
        [ScriptIgnore]
        public bool isStandardPeriod { 
            get 
            {
                if (this.periodName == null) return false;
                foreach (PeriodInterval interval in this.periodName.intervalListChangeHandler.Items)
                {
                    if (interval.name.Equals(this.name, StringComparison.OrdinalIgnoreCase)) return true;
                }
                return false;   
            }
        }
        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public PeriodInterval() 
        {
            childrens = new ObservableCollection<PeriodInterval>();
            childrenListChangeHandler = new PersistentListChangeHandler<PeriodInterval>();
        }

        /// <summary>
        /// Contruit une nouvelle instance de Period sur base 
        /// du nom, de la date de début et de la date de fin.
        /// </summary>
        /// <param name="name">Le nom de la période</param>
        /// <param name="from">La date de début de période</param>
        /// <param name="to">La date de fin de période</param>
        public PeriodInterval(int position, string name, DateTime from, DateTime to)
            : this()
        {
            this.position = position;
            this.name = name;
            this.periodFrom = from.ToShortDateString();
            this.periodTo = to.ToShortDateString();
            this.gridPosition = position;
        }

        [ScriptIgnore]
        public int gridPosition { get; set; }

        public ObservableCollection<PeriodInterval> childrens { get; set; }

        public PersistentListChangeHandler<PeriodInterval> childrenListChangeHandler { get; set; }

        [ScriptIgnore]
        public PeriodName periodName { get; set; }

        /// <summary>
        /// Le rang de la période
        /// </summary>
        public int position { get; set; }

        /// <summary>
        /// Le nom de la période
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// La date de début de période
        /// </summary>
        /// 
        public string periodFrom { get; set; }

        /// <summary>
        /// La date de fin de période
        /// </summary>
        /// 
        public string periodTo { get; set; }

        [ScriptIgnore]
        public DateTime periodFromDateTime
        {
            get
            {
                if (periodFrom != null) return DateTime.Parse(periodFrom);
                else return new DateTime();
            }
            set { periodFrom = value.ToShortDateString(); }
        }

        /// <summary>
        /// La date de fin de période
        /// </summary>
        /// 
        [ScriptIgnore]
        public DateTime periodToDateTime
        {
            get
            {
                if (periodTo != null) return DateTime.Parse(periodTo);
                else return new DateTime();
            }
            set { periodTo = value.ToShortDateString(); }
        }

       
        [ScriptIgnore]
        public PeriodInterval parent { get; set; }

        /// <summary>
        /// La date de début de période
        /// </summary>
        /// 
        [ScriptIgnore]
        public String fromAsString { get { return periodFromDateTime.ToShortDateString(); } }

        /// <summary>
        /// La date de fin de période
        /// </summary>
        /// 
        [ScriptIgnore]
        public String toAsString { get { return periodToDateTime.ToShortDateString(); } }


        public PeriodInterval GetChild(string from, string to)
        {
            foreach (PeriodInterval period in childrens)
            {
                if (period.fromAsString == from && period.toAsString == to) return period;
                return period.GetChild(from, to);
            }
            return null;
        }

        public PeriodItem getFromPeriodItem(string sign)
        {
            PeriodItem item = new PeriodItem();
            item.name = this.GetRoot().periodName.name;
            item.value = this.periodFromDateTime.ToShortDateString();
            if(!String.IsNullOrWhiteSpace(sign)) item.operatorSign = sign;
            return item;
        }

        public PeriodItem getToPeriodItem(string sign)
        {
            PeriodItem item = new PeriodItem();
            item.name = this.periodName.name;
            item.value = this.periodToDateTime.ToShortDateString();
            if (!String.IsNullOrWhiteSpace(sign)) item.operatorSign = sign;
            return item;
        }

        public override string ToString()
        {
            return this.name;
        }


        [ScriptIgnore]
        public List<PeriodInterval> Leafs
        {
            get
            {
                List<PeriodInterval> values = new List<PeriodInterval>(0);
                foreach (PeriodInterval value in childrenListChangeHandler.Items)
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

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child)
        {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((PeriodInterval)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }
        
     
        public void AddChild(List<PeriodInterval> children)
        {
            foreach (PeriodInterval child in children) 
            {
                AddChild(child);
            }
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddUpdated((PeriodInterval)child);
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
                    childrenListChangeHandler.AddUpdated((PeriodInterval)item);
                }
            }
            
            child.SetPosition(-1);
            childrenListChangeHandler.AddDeleted((PeriodInterval)child);
            UpdateParents();
        }


        public Kernel.Domain.PeriodInterval GetRoot() 
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
            childrenListChangeHandler.forget((PeriodInterval)child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(PeriodInterval periodInterval1, PeriodInterval periodInterval2)
        {
            int position = periodInterval1.position;
            periodInterval1.SetPosition(periodInterval2.position);
            periodInterval2.SetPosition(position);
            childrenListChangeHandler.AddUpdated(periodInterval1);
            childrenListChangeHandler.AddUpdated(periodInterval2);
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
        public void SetParent(IHierarchyObject parent) { this.parent = (PeriodInterval)parent; }

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
            foreach (PeriodInterval value in childrenListChangeHandler.Items)
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
        public IHierarchyObject GetChildByName(string name, Kernel.Domain.PeriodName periodName)
        {
            foreach (PeriodInterval periodInterval in periodName.intervalListChangeHandler.Items)
            {
                if (periodInterval.name.ToUpper().Equals(name.ToUpper())) return periodInterval;

            }
            return null;
        }

        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(PeriodInterval editedValue, string name)
        {
            if (editedValue.name.ToUpper().Equals(name.ToUpper()))
            {

                foreach (PeriodInterval periodInterval in childrenListChangeHandler.Items)
                {
                    if (periodInterval.name.ToUpper().Equals(name.ToUpper()) && !periodInterval.Equals(editedValue)) return periodInterval;
                    IHierarchyObject ob = periodInterval.GetNotEditedChildByName(editedValue, name);
                    if (ob != null) return ob;
                }

            }
            return null;
        }

        public IHierarchyObject CloneObject()
        {
            PeriodInterval periodInterval = new PeriodInterval();
            periodInterval.name = this.name;
            periodInterval.position = this.position;
            periodInterval.childrenListChangeHandler = new PersistentListChangeHandler<PeriodInterval>();
            foreach (PeriodInterval interval in this.childrenListChangeHandler.Items)
            {
                PeriodInterval cloneInterval = (PeriodInterval)interval.CloneObject();
                periodInterval.childrenListChangeHandler.AddNew(cloneInterval);
            }
            return periodInterval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            PeriodInterval periodInterval = new PeriodInterval();
            periodInterval.name = "Copy Of " + this.name;
            periodInterval.position = -1;
            periodInterval.parent = null;

            foreach (PeriodInterval child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                periodInterval.AddChild(copy);
            }
            return periodInterval;
        }

        public void refreshPeriodInterval(PeriodName periodName) 
        {
            foreach (PeriodInterval child in childrenListChangeHandler.Items) 
            {
                child.periodName = periodName;
                child.refreshPeriodInterval(periodName);
            }
        }

        public virtual int CompareTo(object obj)
        {
            if (obj == null || !(obj is PeriodInterval)) return 1;
            if (this == obj) return 0;
            return this.position.CompareTo(((PeriodInterval)obj).position);
        }

        public bool Containts(DateTime date)
        {
            if (date == null) return false;
            if (string.IsNullOrWhiteSpace(periodFrom)) return false;
            return periodFromDateTime <= date;
        }
    }
}
