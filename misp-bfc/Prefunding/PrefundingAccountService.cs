using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Bfc.Prefunding
{
    public class PrefundingAccountService : Misp.Kernel.Service.Service<PrefundingAccountData, BrowserData>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberBankIDoid"></param>
        /// <returns></returns>
        public PrefundingAccountData getPrefundingAccountData(int memberBankIDoid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/" + memberBankIDoid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                PrefundingAccountData data = Serializer.Deserialize<PrefundingAccountData>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve Prefunding Account Data from server.", e);
                return null;
            }
        }

    }
}
