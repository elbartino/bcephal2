using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationContext : Persistent
    {

        public Attribute postingNbreAttribute { get; set; }

        public Attribute accountNbreAttribute { get; set; }

        public Attribute accountNameAttribute { get; set; }

        public Attribute recoNbreAttribute { get; set; }

        public Attribute dcNbreAttribute { get; set; }

        public AttributeValue debitAttributeValue { get; set; }

        public AttributeValue creditAttributeValue { get; set; }

        public Measure amountMeasure { get; set; }

        public AttributeValue writeOffAccount { get; set; }

        public AttributeValue chargeBackAccount { get; set; }

        public int lastPostingNumber { get; set; }

        public int lastRecoNumber { get; set; }

    }
}
