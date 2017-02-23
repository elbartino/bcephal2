using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class RightsUtil
    {
        public static bool HasRight(Domain.RightType rightType, List<Domain.Right> listRights) 
        {
            foreach (Domain.Right right in listRights) 
            {
                if (right.rightType.Equals(rightType.ToString())) return true;
            }
            return false;
        }
    }
}
