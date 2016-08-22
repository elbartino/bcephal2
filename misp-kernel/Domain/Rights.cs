using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Rights : Persistent
    {
        public String functionnality { get; set; }

        public bool viewRight { get; set; }

        public bool editRight { get; set; }

        

        [ScriptIgnore]
        public Profil profil { get; set;}

        [ScriptIgnore]
        public User user { get; set;}

        //[ScriptIgnore]
        //public Rights parent { get; set; }

        //[ScriptIgnore]
        //public List<Rights> childs { get; set; }

        //[ScriptIgnore]
        //public UserAction action { get; set; }

        public Rights()
        {
            this.viewRight = false;
            this.editRight = false;
        }

        public Rights(String function)
        {
            this.functionnality = function;
            this.viewRight = false;
            this.editRight = false;
        }

        public Rights(String function, Boolean view, Boolean edit)
        {
            this.functionnality = function;
            this.viewRight = view;
            this.editRight = edit;
        }
        
        public override string ToString()
        {
            return functionnality ;//+ " - "  + action;
        }

        /// <summary>
        /// compare
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Rights)) return 1;
            return this.functionnality.CompareTo(((Rights)obj).functionnality);
        }
    }
}
