﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Relation : Persistent
    {
        public User owner { get; set; }
        public User user { get; set; }
        public Role role { get; set; }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Relation)) return 1;
            return 1;
        }
    }
}
