﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class ReconciliationContext : Persistent
    {
        
        public  string PERFIX_WO = "WO-";
	    
	    public Attribute postingNbreAttribute { get; set; }
	
	    public Attribute accountNbreAttribute { get; set; }	
	    
	    public Attribute recoNbreAttribute { get; set; } 	
        
        public Attribute dcNbreAttribute { get; set; }    
        
        public AttributeValue debitAttributeValue { get; set; }    
        
        public AttributeValue creditAttributeValue { get; set; }    
        
	    public int lastPostingNumber { get; set; }

        public int lastRecoNumber { get; set; }

    }
}
