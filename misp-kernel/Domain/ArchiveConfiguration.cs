using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class ArchiveConfiguration
    {
        [ScriptIgnore]
	    public static String NO_AUTOMATIC_ARCHIVE = "No automatic archive";
    
        [ScriptIgnore]
	    public static String AT_START = "At start";
    
        [ScriptIgnore]
	    public static String MINUTES_INTERVAL = "Minutes interval";
    
        [ScriptIgnore]
	    public static String AT_CLOSING = "At closing";


        [ScriptIgnore]
        public bool isDefaultRepository { get; set; }
	
	    public String type = NO_AUTOMATIC_ARCHIVE;
		
	    public String repository;
	
	    public double frequency = 10;

        public int maxArchiveCount = 2;
	
	    public bool atStart = false;
	
	    public bool atClose = false;
	
	    public bool isNoAutomaticArchive(){
            return type == null || type.Equals(NO_AUTOMATIC_ARCHIVE);
	    }

	    public bool isAtStartArchive(){
            return type != null && type.Equals(AT_START);
	    }

	    public bool isAtClosingArchive(){
		    return type != null && type.Equals(AT_CLOSING);
	    }
                
        
	    public bool isMinutesIntervalArchive(){
            return type != null && type.Equals(MINUTES_INTERVAL);
	    }
    }
}
