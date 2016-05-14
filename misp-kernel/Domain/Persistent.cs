﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// Cette classe encapsule les propriétés communes des objets persistants.
    /// </summary>
    [Serializable]
    public class Persistent : INotifyPropertyChanged, IComparable
    {

        public Persistent()
        {
            typeName = GetType().Name;
            isCompleted = true;
            isModified = true;
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

    }
}
