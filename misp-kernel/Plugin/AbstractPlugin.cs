using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Application;

namespace Misp.Kernel.Plugin
{
    public abstract class AbstractPlugin : IPlugin
    {

        protected IPluginHost host;

        /// <summary>
        /// Construit une nouvelle instance de AbstractPlugin
        /// </summary>
        public AbstractPlugin()
		{
            this.Name = GetPluinName();
            this.Priority = GetPluinPriority();
            this.Menus = GetPluinMenus();
            this.ControllerFactory = GetPluinControllerFactory();
            this.RadioButton = new RadioButton();
            this.RadioButton.Content = this.Name;
		}

        /// <summary>
        /// Le nom du plugin
        /// </summary>
        /// <returns></returns>
        protected abstract string GetPluinName();

        /// <summary>
        /// La priorité du pluin
        /// </summary>
        /// <returns></returns>
        protected abstract int GetPluinPriority();

        /// <summary>
        /// Les menus du plugin
        /// </summary>
        /// <returns></returns>
        protected abstract List<ApplicationMenu> GetPluinMenus();

        /// <summary>
        /// Le ControllerFactory du plugin
        /// </summary>
        /// <returns></returns>
        protected abstract ControllerFactory GetPluinControllerFactory();
		
		public string Name { get; set; }
        public int Priority { get; set; }
        public ControllerFactory ControllerFactory { get; set; }
        public List<ApplicationMenu> Menus { get; set; }
		public IPluginHost Host
		{
			get{ return host; }
			set
			{
				host=value;
				host.Register(this);
			}
		}

        public RadioButton RadioButton { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is IPlugin)) return 1;
            return this.Priority.CompareTo(((IPlugin)obj).Priority);
        }
    }
}
