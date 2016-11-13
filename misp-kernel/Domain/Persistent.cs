using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.ComponentModel;
using Misp.Kernel.Domain.Browser;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// Cette classe encapsule les propriétés communes des objets persistants.
    /// </summary>
    [Serializable]
    public class Persistent : NotifyPropertyChanged, INotifyPropertyChanged, IComparable
    {

        public Persistent()
        {
            typeName = GetType().Name;
            isCompleted = true;
            isModified = true;
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
        public bool IsExpanded
        {
            set;
            get;
        }


        [ScriptIgnore]
        public bool IsSelected
        {
            set;
            get;
        }

        public string typeName { get; set; }

        public int? oid { get; set; }

        public bool isCompleted { get; set; }

        public bool isModified { get; set; }

        [ScriptIgnore]
        public bool selected { get; set; }

        [ScriptIgnore]
        public string creationDate { get; set; }

        [ScriptIgnore]
        public string modificationDate { get; set; }

        [ScriptIgnore]
        public DateTime creationDateTime
        { 
            get
            {
                return DateTime.ParseExact(creationDate, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        [ScriptIgnore]
        public DateTime modificationDateTime
        {
            get {
                return DateTime.ParseExact(modificationDate, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual int CompareTo(object obj)
        {
            if (obj == null || !(obj is Persistent)) return 1;
            if (this == obj) return 0;
            return this.oid.Value.CompareTo(((Persistent)obj).oid.Value);
        }

        [ScriptIgnore]
        public BrowserDataFilter Filter { get; set; }

        public virtual bool HasMoreElements()
        {
            return !this.isCompleted || (this.Filter != null && this.Filter.totalPages > 0 && this.Filter.totalPages > this.Filter.page); 
        }

    }
}
