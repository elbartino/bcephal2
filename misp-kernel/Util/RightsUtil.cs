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


        public static bool HasRight(Domain.RightType[] rightsType, List<Domain.Right> listRights)
        {
            bool result = true;
            foreach (Domain.RightType rightstype in rightsType) 
            {
                if(HasRight(rightstype, listRights) && result) result = true;
                else result = false;
            }
            return result;
        }
    }
}
