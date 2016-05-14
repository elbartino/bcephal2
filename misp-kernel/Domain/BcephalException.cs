using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class BcephalException : Exception
    {

        public BcephalException() 
            : base() { }
    
        public BcephalException(string message) 
            : base(message) { }    
    
        public BcephalException(string message, Exception innerException)
            : base(message, innerException) { }    

    }
}
