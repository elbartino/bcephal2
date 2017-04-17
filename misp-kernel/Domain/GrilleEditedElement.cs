using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleEditedElement
    {
        
        public int? oid { get; set; }

        public GrilleColumn column { get; set; }

        public BrowserData value { get; set; }

        public decimal measure { get; set; }

        public string date { get; set; }

        public Attribute attribute { get; set; }

        public Grille grid;
    }
}
