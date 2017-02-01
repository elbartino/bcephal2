using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Application
{
    public class FunctionalityFactory
    {

        /// <summary>
        /// Gets or sets the FunctionalityFactory. 
        /// </summary>
        public static FunctionalityFactory Instance { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }

        /// <summary>
        /// Build a new instance of HistoryHandler.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public FunctionalityFactory(ApplicationManager applicationManager)
        {
            Instance = this;
            this.ApplicationManager = applicationManager;
        }

        public Functionality Get(String code, RightType? type = null)
        {
            foreach (Functionality functionality in this.Functionalities)
            {
                Functionality f = functionality.get(code, type);
                if (f != null) return f;
            }
            return null;
        }

        public List<Functionality> Functionalities
        {
            get
            {
                List<Functionality> functionalities = new List<Functionality>(0);
                functionalities.Add(new ProjectFunctionality());
                //functionalities.Add(new HelpFunctionality());

                foreach (Plugin.IPlugin plugin in this.ApplicationManager.Plugins)
                {
                    List<Functionality> funcs = plugin.Functionalities;
                    if (funcs != null) functionalities.AddRange(funcs);
                }
                return functionalities;
            }
        }

    }
}
