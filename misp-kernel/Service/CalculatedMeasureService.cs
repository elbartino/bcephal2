using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Service
{
    public class CalculatedMeasureService : Kernel.Service.Service<Kernel.Domain.CalculatedMeasure, Kernel.Domain.Browser.BrowserData>
    {
        /// <summary>
        /// Le MeasureService.
        /// </summary>
        public MeasureService MeasureService { get; set; }

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public PeriodicityService PeriodicityService { get; set; }
        
        /// <summary>
        /// Le DesignService.
        /// </summary>
        public DesignService DesignService { get; set; }

        /// <summary>
        /// Le AuditService.
        /// </summary>
        public AuditService AuditService { get; set; }

        /// <summary>
        /// return all calculated measure in database
        /// </summary>
        /// <returns></returns>
        public List<Kernel.Domain.CalculatedMeasure> getAllCalculatedMeasure()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/allCal", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.CalculatedMeasure> root = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.CalculatedMeasure>>(queryResult.Content);
                    return root;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return CalculatedMeasures.", e);
            }
        }

        /// <summary>
        /// return the empty calculatedMeasure
        /// </summary>
        /// <returns></returns>
        public Kernel.Domain.Measure getEmptyMeasure()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/empty", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.Measure emptyMeasure = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Measure>(queryResult.Content);
                    return emptyMeasure;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return empty Measure.", e);
            }
        }
    }


}
