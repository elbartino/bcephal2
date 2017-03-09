using Misp.Kernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Base
{
    public class BfcResourcePath : ResourcePath
    {

        public static string BFC_RESOURCE_PATH = "bfc";
        public static string BFC_REVIEW_RESOURCE_PATH = BFC_RESOURCE_PATH + "/review";
        public static string BFC_PREFUNDING_ACCOUNT_RESOURCE_PATH = BFC_REVIEW_RESOURCE_PATH + "/prefunding-account";

        public static string BFC_MEMBER_BANK_RESOURCE_PATH = BFC_RESOURCE_PATH + "/member-bank";

    }
}
