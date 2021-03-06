﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Right : Persistent
    {
        public String functionnality { get; set; }

        public String rightType { get; set; } 

        public int objectOid { get; set; }

        public String objectType { get; set; }

        public String projectReference { get; set; }

        //[ScriptIgnore]
        public Profil profil { get; set;}

        //[ScriptIgnore]
        public User user { get; set;}

        public Right()
        {
           
        }

        public Right(String function)
        {
            this.functionnality = function;
        }
        
        public override string ToString()
        {
            return functionnality ;
        }

        /// <summary>
        /// compare
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Right)) return 1;
            if (!String.IsNullOrWhiteSpace(this.functionnality))
                return this.functionnality.CompareTo(((Right)obj).functionnality);
            return 1;
        }
    }
}
