using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class DesignService : Service.Service<Design, Domain.Browser.BrowserData>
    {

        /// <summary>
        ///  le PeriodNameService
        /// </summary>
        public PeriodNameService PeriodNameService { get; set; }

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
        /// Le CalculatedMeasureService
        /// </summary>
        public CalculatedMeasureService CalculatedMeasureService { get; set; }


        /// <summary>
        /// Le TargetService.
        /// </summary>
        public TargetService TargetService { get; set; }

        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override Misp.Kernel.Domain.Design Save(Misp.Kernel.Domain.Design item)
        {
            if (item == null) return item;
            if (item.oid == null || !item.oid.HasValue) return base.Save(item);

            if (!SaveBasicInfo(item)) throw new BcephalException("Unable to save Item.");
            SaveDimensions(item);
            try
            {
                if (FileService != null) FileService.SaveCurrentFile();
            }
            catch (Exception) { }
            return item;
        }


        public bool SaveBasicInfo(Misp.Kernel.Domain.Design item)
        {
            DesignDimension central = item.central;
            DesignDimension row = item.rows;
            DesignDimension column = item.columns;
            item.central = null;
            item.rows = null;
            item.columns = null;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save_basic", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;                
                string json = serializer.Serialize(item);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                Boolean result = Serializer.Deserialize<Boolean>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to save Table.", e);
            }
            finally{
                item.central = central;
                item.rows = row;
                item.columns = column;
            }
        }

        public void SaveDimensions(Design item)
        {
            item.columns = SaveDimension(item.columns);
            item.rows = SaveDimension(item.rows);
            item.central = SaveDimension(item.central);
        }


        public DesignDimension SaveDimension(DesignDimension dimension)
        {
            if (dimension == null) return dimension;

            if (dimension.lineListChangeHandler.newItems.Count == 0
                && dimension.lineListChangeHandler.updatedItems.Count == 0
                && dimension.lineListChangeHandler.deletedItems.Count == 0) return dimension;

            PersistentListChangeHandler<DesignDimensionLine> lines = new PersistentListChangeHandler<DesignDimensionLine>();
            lines.newItems = dimension.lineListChangeHandler.newItems;
            lines.updatedItems = dimension.lineListChangeHandler.updatedItems;
            lines.deletedItems = dimension.lineListChangeHandler.deletedItems;

            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save_lines/" + dimension.oid.Value, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(lines);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                dimension = Serializer.Deserialize<DesignDimension>(queryResult.Content);
                return dimension;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to save Dimension.", e);
            }
        }


    }
}
