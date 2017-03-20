using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Model
{
    public class AgeingBalanceData
    {

        public string replenishmentInstruction { get; set; }
        public string sentDate { get; set; }
        public string valueDate { get; set; }
        public string memberBankName { get; set; }
        public string pml { get; set; }
        public decimal toReceive { get; set; }
        public decimal paid { get; set; }
        public decimal pendingBalance { get; set; }
        public decimal nrftx { get; set; }
        
        public decimal notReached { get; set; }
        public decimal today { get; set; }
        public decimal oneDay { get; set; }
        public decimal twoDays { get; set; }
        public decimal threeDays { get; set; }
        public decimal fourDays { get; set; }
        public decimal moreThanFourDays { get; set; }

        public bool reconciliated { get; set; }


        public decimal lateCollectionAmount { 
            get { return oneDay + twoDays + threeDays + fourDays + moreThanFourDays; }
            set { }
        }

        public String amountType
        {
            get { return reconciliated ? "Paid" : "To receive"; }
            //set { }
        }

    }
}
