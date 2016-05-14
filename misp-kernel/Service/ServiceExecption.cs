using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;

namespace Misp.Kernel.Service
{
    public class ServiceExecption : BcephalException
    {

        public ServiceExecption() 
            : base() { }
    
        public ServiceExecption(string message) 
            : base(message) { }

        public ServiceExecption(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
