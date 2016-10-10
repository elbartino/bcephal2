using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Functionality
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Functionality Parent { get; set; }
        public bool ShowInDashboard { get; set; }
        public List<Functionality> Children { get; set; }

        public Functionality()
        {
            this.Children = new List<Functionality>(0);          
        }

        public Functionality(string code, string name) : this()
        {
            this.Code = code;
            this.Name = name;
            }

        public Functionality(string code, string name, bool showInDashboard)
            : this(code, name)
        {
            this.ShowInDashboard = showInDashboard;
        }

        public Functionality(Functionality parent, string code, string name, bool showInDashboard)
            : this(code, name, showInDashboard)
        {
            this.Parent = parent;
        }


        public Functionality get(String code)
        {
            if (this.equals(code)) return this;
            else return getDescendent(code);
        }

        public Functionality getDescendent(String code)
        {
            if (String.IsNullOrWhiteSpace(code)) return null;
            foreach (Functionality child in this.Children)
            {
                if (child.equals(code)) return child;
                Functionality f = child.getDescendent(code);
                if (f != null) return f;
            }
            return null;
        }

        public bool equals(String code)
        {
            if (String.IsNullOrWhiteSpace(code)) return false;
            if (String.IsNullOrWhiteSpace(this.Code)) return false;
            return this.Code.Equals(code);
        }

    }
}
