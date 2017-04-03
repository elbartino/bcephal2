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
        public BfcItemService PmlService { get; set; }
        public BfcItemService SchemeService { get; set; }
        public BfcItemService PlatformService { get; set; }


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
            Kernel.Util.FileUtil.buildTimeMeasurementFile();
            try
            {
                Console.Out.WriteLine(" ---------------- Evolution Data Measures of " + DateTime.Now + " -------------");
                Console.Out.WriteLine();
                DateTime begin = DateTime.Now;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/settlement-evolution", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                
                DateTime requetTime = DateTime.Now;
                
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                
                Console.Out.WriteLine("               Request duration " + (DateTime.Now - requetTime));
                Console.Out.WriteLine();
                bool valid = ValidateResponse(queryResult);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;

                DateTime serialization = DateTime.Now;
                List<SettlementEvolutionData> datas = Serializer.Deserialize<List<SettlementEvolutionData>>(queryResult.Content);
                Console.Out.WriteLine("                 Serialization duration "+(DateTime.Now  - serialization));
                Console.Out.WriteLine();
                Console.Out.WriteLine();
                Console.Out.WriteLine("                 Total duration " + (DateTime.Now - begin));
                Console.Out.WriteLine(" ---------------- End Evolution Data Measures  -------------");
                Console.Out.WriteLine();
                Console.Out.WriteLine();
                Kernel.Util.FileUtil.closeTimeMeasurementFile();
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
            Kernel.Util.FileUtil.buildTimeMeasurementFile();
            try
            {
                Console.Out.WriteLine(" ---------------- Evolution Chart Measures of " + DateTime.Now + " -------------");
                Console.Out.WriteLine();
                DateTime begin = DateTime.Now;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/settlement-evolution-chart", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                DateTime chartrequest = DateTime.Now;

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                Console.Out.WriteLine("                 Chart data request " + (DateTime.Now - chartrequest));
                Console.Out.WriteLine();
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;

                DateTime chartSerialisation = DateTime.Now;
                List<SettlementEvolutionChartData> datas = Serializer.Deserialize<List<SettlementEvolutionChartData>>(queryResult.Content);
                
                Console.Out.WriteLine("             Chart data serialization " + (DateTime.Now - chartSerialisation));
                
                Console.Out.WriteLine();
                Console.Out.WriteLine();

                Console.Out.WriteLine("             Total Chart duration " + (DateTime.Now - begin));
                Console.Out.WriteLine();
                Console.Out.WriteLine(" --------------- End Evolution Chart -------------");
                Console.Out.WriteLine();
                Console.Out.WriteLine();
                Kernel.Util.FileUtil.closeTimeMeasurementFile();

                return datas;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve Settlement Evolution Datas from server.", e);
                return null;
            }
        }


        public List<AgeingBalanceData> getAgeingBalanceDatas(ReviewFilter filter)
        {
            if (filter == null) return null;

            Kernel.Util.FileUtil.buildTimeMeasurementFile();
            
            Console.Out.WriteLine(" ---------------- Ageing "+( filter.details ?"Details" : "")+" Measures of "+DateTime.Now +" -------------");
            DateTime begin = DateTime.Now;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/ageing-balance", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);

                DateTime requesttime = DateTime.Now;
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                Console.Out.WriteLine("             Ageing "+( filter.details ?"Details" : "")+" server quering duration " + (DateTime.Now - requesttime));
                Console.Out.WriteLine();
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;

                DateTime serialization = DateTime.Now;
                List<AgeingBalanceData> datas = Serializer.Deserialize<List<AgeingBalanceData>>(queryResult.Content);
                Console.Out.WriteLine("             Ageing "+( filter.details ?"Details" : "")+" serialisation duration " + (DateTime.Now - serialization));
                Console.Out.WriteLine();
                Console.Out.WriteLine("             Total Duration Ageing "+( filter.details ?"Details " : " ")+ (DateTime.Now - begin));
                Console.Out.WriteLine();
                Console.Out.WriteLine();


                Console.Out.WriteLine(" ------------End Ageing "+( filter.details ?"Details" : "")+" Measure -------------");
                Console.Out.WriteLine();
                Console.Out.WriteLine();
                Kernel.Util.FileUtil.closeTimeMeasurementFile();
                return datas;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve Ageing Balance Datas from server.", e);
                return null;
            }
        }


    }
}
