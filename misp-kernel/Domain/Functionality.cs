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

        public List<RightType> RightTypes { get; set; }
        

        public Functionality()
        {
            this.Children = new List<Functionality>(0);
            this.RightTypes = new List<RightType>(0);  
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

        public Functionality(Functionality parent, string code, string name, bool showInDashboard, params RightType[] types)
            : this(code, name, showInDashboard)
        {
            this.Parent = parent;
            this.RightTypes.AddRange(types);
        }


        public Functionality get(String code, RightType? type = null)
        {
            if (this.equals(code, type)) return this;
            else return getDescendent(code, type);
        }

        public Functionality getDescendent(String code, RightType? type = null)
        {
            if (String.IsNullOrWhiteSpace(code)) return null;
            foreach (Functionality child in this.Children)
            {
                if (child.equals(code, type)) return child;
                Functionality f = child.getDescendent(code, type);
                if (f != null) return f;
            }
            return null;
        }

        public bool HasType(RightType type)
        {
            return this.RightTypes.Contains(type);
        }

        public bool equals(String code, RightType? type = null)
        {
            if (String.IsNullOrWhiteSpace(code)) return false;
            if (String.IsNullOrWhiteSpace(this.Code)) return false;
            return this.Code.Equals(code) && (!type.HasValue || HasType(type.Value));
        }

    }
}
