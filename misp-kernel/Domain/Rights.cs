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
        //public UserAction action { get; set; }


        public Rights()
        {
            this.viewRight = true;
            this.editRight = true;
        }

        public Rights(String function, Boolean view, Boolean edit)
        {
            this.functionnality = function;
            this.viewRight = view;
            this.editRight = edit;
        }

        public static List<Rights> generateDefaultFunction()
        {
            List<Rights> functionRights = new List<Rights>(0);
            functionRights.Add(new Rights("Initialisation", false, false));
            functionRights.Add(new Rights("InputTable", false, false));
            functionRights.Add(new Rights("Report", false, false));
            functionRights.Add(new Rights("TransformationTree", false, false));
            functionRights.Add(new Rights("Target", false, false));
            functionRights.Add(new Rights("StructuredReport", false, false));
            functionRights.Add(new Rights("AutomaticSourcing", false, false));
            functionRights.Add(new Rights("MultipleUplaod", false, false));
            functionRights.Add(new Rights("Design", false, false));
            functionRights.Add(new Rights("AutomaticTarget", false, false));
            functionRights.Add(new Rights("CombinedTranformationTree", false, false));
            functionRights.Add(new Rights("Load", false, false));
            functionRights.Add(new Rights("Clear", false, false));
            functionRights.Add(new Rights("Posting", false, false));
            functionRights.Add(new Rights("ReconciliationFilter", false, false));
            functionRights.Add(new Rights("Clear Launch", false, false));
            functionRights.Add(new Rights("Laod Launch", false, false));

            return functionRights;
        }

        public override string ToString()
        {
            return functionnality + " - "; // + action;
        }
    }
}
