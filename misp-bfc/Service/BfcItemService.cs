using Misp.Bfc.Model;
using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Service
{
    public class BfcItemService : Misp.Kernel.Service.Service<BfcItem, BrowserData>
    {

        public List<BfcItem> getAll()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/all", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<BfcItem> items = RestSharp.SimpleJson.DeserializeObject<List<BfcItem>>(queryResult.Content);
                return items;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of objects.", e);
                return new List<BfcItem>(0);
            }
        }

    }
}
