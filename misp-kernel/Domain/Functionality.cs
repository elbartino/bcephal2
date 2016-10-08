using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Kernel.Domain
{
    public class Functionality
    {

        public string code { get; set; }
        public string name { get; set; }
        List<Functionality> children { get; set; }

        public Functionality(string code, string name)
        {
            this.code = code;
            this.name = name;
        }
    }
}
