using Misp.Bfc.Model;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Bfc.Service
{
    public class AdvisementService : Misp.Kernel.Service.Service<Advisement, AdvisementBrowserData>
    {
        public BfcItemService MemberBankService { get; set; }
        public BfcItemService SchemeService { get; set; }
        public BfcItemService PlatformService { get; set; }
        public BfcItemService PmlService { get; set; }
        public BfcItemService DebitCreditService { get; set; }

        public decimal getAlreadyRequestedPrefundingAmount(int memberBankIdOid, int schemeIdOid){           
            try
            {
                var request = new RestRequest(ResourcePath + "/already-requested-prefunding/" + memberBankIdOid + "/" + schemeIdOid, Method.POST);
                request.RequestFormat = DataFormat.Json;                
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                decimal amount = RestSharp.SimpleJson.DeserializeObject<decimal>(queryResult.Content);
                return amount;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve Already Requested Prefunding Amount.", e);
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public override BrowserDataPage<AdvisementBrowserData> getBrowserDatas(BrowserDataFilter filter)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/browser-data-page", Method.POST);
                request.RequestFormat = DataFormat.Json;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                AdvisementBrowserDataPage objects = RestSharp.SimpleJson.DeserializeObject<AdvisementBrowserDataPage>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

    }
}
