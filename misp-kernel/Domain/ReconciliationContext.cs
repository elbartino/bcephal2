using Misp.Kernel.Application;
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


        public bool isDefaultColumn(GrilleColumn column)
        {
            return column != null && column.valueOid.HasValue && (
                (column.type.Equals(ParameterType.MEASURE.ToString()) 
                    && amountMeasure != null && column.valueOid.Value == amountMeasure.oid.Value) ||
                
                (column.type.Equals(ParameterType.SCOPE.ToString()) 
                    && postingNbreAttribute != null && column.valueOid.Value == postingNbreAttribute.oid.Value) ||
                
                (column.type.Equals(ParameterType.SCOPE.ToString()) 
                    && accountNbreAttribute != null && column.valueOid.Value == accountNbreAttribute.oid.Value) ||
                    
                (column.type.Equals(ParameterType.SCOPE.ToString()) 
                    && accountNameAttribute != null && column.valueOid.Value == accountNameAttribute.oid.Value) ||                    
                    
                (column.type.Equals(ParameterType.SCOPE.ToString()) 
                    && recoNbreAttribute != null && column.valueOid.Value == recoNbreAttribute.oid.Value) ||
                    
                (column.type.Equals(ParameterType.SCOPE.ToString()) 
                    && dcNbreAttribute != null && column.valueOid.Value == dcNbreAttribute.oid.Value)
                );
        }

    }
}
