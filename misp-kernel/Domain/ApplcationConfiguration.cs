using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ApplcationConfiguration
    {

        public enum Editon { MONOUSER, MULTIUSER }
        public enum Domain { DEFAULT, RECONCILIATION }

        public String editon { get; set; }
        public String domain { get; set; }

        public String projectsDir { get; set; }

        public ApplcationConfiguration() { }

        public ApplcationConfiguration(String editon, String domain)
        {
            this.editon = editon;
            this.domain = domain;
        }

        public ApplcationConfiguration(Editon editon, Domain domain)
        {
            this.editon = editon.ToString();
            this.domain = domain.ToString();
        }

        public bool IsReconciliationDomain()
        {
            return this.domain.Equals(Domain.RECONCILIATION.ToString());
        }

    }
}
