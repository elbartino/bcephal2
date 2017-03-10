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
    public class ReviewService : Misp.Kernel.Service.Service<Object, BrowserData>
    {
        
        public BfcItemService MemberBankService { get; set; }
        public BfcItemService SchemeService { get; set; }


        public PrefundingAccountData getPrefundingAccountData(ReviewFilter filter)
        {
            if (filter == null) return null;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/prefunding-account", Method.POST);

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

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


        public List<SettlementEvolutionData> getSettlementEvolutionDatas(ReviewFilter filter)
        {
            if (filter == null) return null;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/settlement-evolution", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
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


        public List<SettlementEvolutionChartData> getSettlementEvolutionChartDatas(ReviewFilter filter)
        {
            if (filter == null) return null;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/settlement-evolution-chart", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                List<SettlementEvolutionChartData> datas = Serializer.Deserialize<List<SettlementEvolutionChartData>>(queryResult.Content);
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
