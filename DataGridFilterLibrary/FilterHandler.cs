using DataGridFilterLibrary.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridFilterLibrary
{
    public class FilterHandler
    {

        public delegate void FilterEventHandler(FilterData data);
        public event FilterEventHandler Handler;        

        public virtual void DoFilter(FilterData data)
        {
            if (Handler != null) Handler(data);
        }

    }
}
