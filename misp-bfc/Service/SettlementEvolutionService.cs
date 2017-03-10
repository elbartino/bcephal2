using Misp.Bfc.Model;
using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Bfc.Service
{
    public class SettlementEvolutionService : Misp.Kernel.Service.Service<SettlementEvolutionData, BrowserData>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberBankIDoid"></param>
        /// <returns></returns>
        public List<SettlementEvolutionData> getSettlementEvolutionDatas(int bankIDoid, int schemeIDoid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/" + bankIDoid + "/" + schemeIDoid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                List<SettlementEvolutionData> datas = Serializer.Deserialize<List<SettlementEvolutionData>>(queryResult.Content);
                return datas;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve Settlement Evolution Datas from server.", e);
                return null;
            }
        }

    }
}
