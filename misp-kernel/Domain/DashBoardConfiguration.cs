using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class DashBoardConfiguration : Persistent
    {
        
        public static String ASC = "ASC";
	    public static String DESC = "DESC";
	
	    public static String NAME = "name";
	    public static String CREATION_DATE = "creationDate";
	    public static String MODIFICATION_DATE = "modificationDate";

        public static int MAX_ITEMS = 10;
        public static int DEFAULT_POSITION = 1000;

        public DashBoardConfiguration()
        {
            this.orderBy = DashBoardConfiguration.MODIFICATION_DATE;
            this.orderByDirection = DashBoardConfiguration.DESC;
            this.position = DashBoardConfiguration.DEFAULT_POSITION;
            this.maxItems = DashBoardConfiguration.MAX_ITEMS;
        }

        public DashBoardConfiguration(String name) : this()
        {
            this.name = name;
        }

        public DashBoardConfiguration(String name, int position)
            : this(name)
        {
            this.position = position;
        }

	    public String name { get; set; }
	
	    public int position { get; set; }

        public int maxItems { get; set; }
	
	    public String orderBy { get; set; }
               
        public String orderByDirection { get; set; }

    }
}
