using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationContext : Persistent
    {
        
	public Attribute postingNbreAttribute;
	

    public Attribute accountNbreAttribute;
	

	public Attribute recoNbreAttribute; 
	

    public Attribute dcNbreAttribute;
    

    public AttributeValue debitAttributeValue;
    

    public AttributeValue creditAttributeValue;
    

	public int lastPostingNumber;
    

	public int lastRecoNumber;
    }
}
