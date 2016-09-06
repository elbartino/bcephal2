using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationFilter : Grille
    {

        public bool basic { get; set; }

        public PostingFilter leftPostingFilter { get; set; }

        public PostingFilter rigthPostingFilter { get; set; }


        public ReconciliationFilter()
        {
            basic = false;
        }

        public override bool IsReadOnly()
        {
            return true;
        }

    }
}
