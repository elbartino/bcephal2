using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridBrowser : AutomaticSourcingBrowser
    {

        public InputGridBrowser(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        protected override string getTitle()
        {
            return "Input Grids";
        }
          
    }
}
