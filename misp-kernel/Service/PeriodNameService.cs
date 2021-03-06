﻿using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class PeriodNameService : Service<Misp.Kernel.Domain.PeriodName, Misp.Kernel.Domain.Browser.BrowserData>
    {
        public bool canPeriodicityModify(Domain.PeriodName periodName)
         {
            return true;
        }
        public List<Kernel.Domain.PeriodItem> getAllTableAndCellsPeriodItems() 
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/list/perioditems", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                Console.WriteLine("List = " + queryResult.StatusCode);

                List<Kernel.Domain.PeriodItem> objects = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.PeriodItem>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of objects.", e);
                throw new ServiceExecption("Unable to retrieve list of objects.", e);
            }            
        }

        public PeriodName getDefaultPeriodName()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/default-period-name", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Kernel.Domain.PeriodName root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.PeriodName>(queryResult.Content);
                return root;
            }
            catch (Exception e)
            {
                logger.Error("Unable to Return default PeriodName.", e);
                throw new ServiceExecption("Unable to Return default PeriodName.", e);
            }
        }

        public Kernel.Domain.PeriodName getRootPeriodName()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Kernel.Domain.PeriodName root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.PeriodName>(queryResult.Content);
                return root;
            }
            catch (Exception e)
            {
               logger.Error("Unable to Return PeriodName.", e);
               throw new ServiceExecption("Unable to Return PeriodName.", e);
            }
        }

        public Kernel.Domain.PeriodName getRootPeriodNameForSidebar()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root-for-sidebar", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Kernel.Domain.PeriodName root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.PeriodName>(queryResult.Content);
                return root;
            }
            catch (Exception e)
            {
                logger.Error("Unable to Return PeriodName.", e);
                throw new ServiceExecption("Unable to Return PeriodName.", e);
            }
        }

        public BrowserDataPage<Kernel.Domain.PeriodInterval> getRootIntervalsByPeriodName(BrowserDataFilter filter)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root-period-intervals", Method.POST);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<Kernel.Domain.PeriodInterval> values = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<Kernel.Domain.PeriodInterval>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return PeriodInterval.", e);
            }
        }

        public BrowserDataPage<Kernel.Domain.PeriodInterval> getPeriodIntervalChildren(BrowserDataFilter filter)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/period-interval-children", Method.POST);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<Kernel.Domain.PeriodInterval> values = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<Kernel.Domain.PeriodInterval>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return PeriodInterval.", e);
            }
        }



        
    }
}
