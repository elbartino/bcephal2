﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Relation : Persistent
    {
        private string _owner;
        private string _role;



        [ScriptIgnore]
        public User user { get; set; }

       public string owner
        {
            get { return _owner; }

            set
            {
                _owner = value;
                this.OnPropertyChanged("owner");
            }
        }

        public string role
        {
            get { return _role; }

            set
            {
                _role = value;
                this.OnPropertyChanged("role");
            }
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Relation)) return 1;
            return 1;
        }
    }
}
