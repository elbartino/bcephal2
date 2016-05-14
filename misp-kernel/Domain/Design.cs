using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Design : Persistent
    {

        public Design()
        {
            this.central = new DesignDimension();
            this.columns = new DesignDimension();
            this.rows = new DesignDimension();
            this.visibleInShortcut = true;
        }
        
        public BGroup group { get; set; }
        	
	    public String name { get; set; }
	
	    public DesignDimension columns { get; set; }
	
	    public DesignDimension rows { get; set; }
	
	    public DesignDimension central { get; set; }
	
	    public bool addTotalColumnRight { get; set; }
	
	    public bool addTotalRowBelow { get; set; }
	
	    public bool concatenateRowHearder { get; set; }

        public bool concatenateColumnHearder { get; set; }

        public bool visibleInShortcut { get; set; }
        
        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Design)) return 1;
            if (this == obj) return 0;
            return this.name.CompareTo(((Design)obj).name);
        }


        public Design getCopy(String name)
        {
            Design design = new Design();
            design.name =  name;
            design.typeName = this.typeName;
            design.group = this.group;
            design.columns = this.columns;
            design.rows = this.rows;
            design.central = this.central;
            design.addTotalColumnRight = this.addTotalColumnRight;
            design.addTotalRowBelow = this.addTotalRowBelow;
            design.concatenateColumnHearder = this.concatenateColumnHearder;
            design.concatenateRowHearder = this.concatenateRowHearder;
            
            return design;
        }
    }
}
