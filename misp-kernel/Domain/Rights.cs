using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Domain
{
    public class Rights : Persistent
    {
        public String functionnality { get; set; }

        public Boolean viewRight { get; set; }

        public Boolean editRight { get; set; }


    	public Profil profil { get; set;}

    	public User user { get; set;}	    

        public UserAction action { get; set; }


        public Rights(String function, Boolean view, Boolean edit)
        {
            this.functionnality = function;
            this.viewRight = view;
            this.editRight = edit;
        }

        public static List<Rights> generateDefaultFunction()
        {
            List<Rights> functionRights = new List<Rights>(0);
            functionRights.Add(new Rights("Initialisation", true, true));
            //functionRights.Add(new Rights("InputTable", true, true));
            //functionRights.Add(new Rights("Report", true, true));
            //functionRights.Add(new Rights("TransformationTree", true, true));
            //functionRights.Add(new Rights("Target", true, true));
            //functionRights.Add(new Rights("StructuredReport", true, true));
            //functionRights.Add(new Rights("AutomaticSourcing", true, true));
            //functionRights.Add(new Rights("MultipleUplaod", true, true));
            //functionRights.Add(new Rights("Design", true, true));
            //functionRights.Add(new Rights("AutomaticTarget", true, true));
            //functionRights.Add(new Rights("CombinedTranformationTree", true, true));
            //functionRights.Add(new Rights("Load", true, true));
            //functionRights.Add(new Rights("Clear", true, true));
            //functionRights.Add(new Rights("Posting", true, true));
            //functionRights.Add(new Rights("ReconciliationFilter", true, true));
            //functionRights.Add(new Rights("Clear Launch", true, true));
            //functionRights.Add(new Rights("Laod Launch", true, true));

            return functionRights;
        }

        public override string ToString()
        {
		    return functionnality + " - " + action;
        }
    }
}
