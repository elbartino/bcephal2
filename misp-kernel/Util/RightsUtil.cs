using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class RightsUtil
    {
        public static bool HasRight(Domain.RightType rightType, List<Domain.Right> rights) 
        {
            if (rights == null) return true;
            foreach (Domain.Right right in rights) 
            {
                if (right.rightType.Equals(rightType.ToString())) return true;
            }
            return false;
        }


        public static bool HasRight(Domain.RightType[] rightsType, List<Domain.Right> rights)
        {
            if (rights == null) return true;
            bool result = true;
            foreach (Domain.RightType rightstype in rightsType) 
            {
                if (HasRight(rightstype, rights) && result) result = true;
                else result = false;
            }
            return result;
        }
    }
}
