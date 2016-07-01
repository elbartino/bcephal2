using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using log4net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Misp.Kernel.Service
{
    /// <summary>
    /// Cette classe encapsule les propriétés communues des services.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Service<T,B> : IService
    {

        protected readonly ILog logger;

        /// <summary>
        /// Le GroupService.
        /// </summary>
        public GroupService GroupService { get; set; }

        public Service()
        {
            logger = LogManager.GetLogger(this.GetType());
            MispJsonSerializerStrategy strategy = new MispJsonSerializerStrategy();
            RestSharp.SimpleJson.CurrentJsonSerializerStrategy = strategy;
        }

        /// <summary>
        /// RestClient
        /// </summary>
        public RestClient RestClient { get; set; }

        /// <summary>
        /// RestClient
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// RestClient
        /// </summary>
        public string SocketResourcePath { get; set; }

        /// <summary>
        /// Le FileService au controller.
        /// </summary>
        public FileService FileService { get; set; }

        public static bool ValidateResponse(RestResponse response) 
        {
            if(response == null) return false;
            System.Net.HttpStatusCode statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.OK) return true;
            
            switch (statusCode)
            {
                case HttpStatusCode.NotFound: break;

                case HttpStatusCode.Forbidden: break;

                case HttpStatusCode.UnsupportedMediaType: break;

                default: break;
                
            }
            throw new BcephalException(response.StatusDescription);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public virtual List<B> getBrowserDatas()
        {
            
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatas", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<B> objects = RestSharp.SimpleJson.DeserializeObject<List<B>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

        public virtual List<B> getBrowserDatas(bool isTarget)
        {
            return new List<B>(0);    
        }

        public List<B> getBrowserDatasForTree()
        {

            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatas/tree", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<B> objects = RestSharp.SimpleJson.DeserializeObject<List<B>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

        public T getRootMeasure()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                T root = RestSharp.SimpleJson.DeserializeObject<T>(queryResult.Content);
                return root;
            }
            catch (Exception e)
            {
                logger.Error("Unable to Return Measures.", e);
                throw new BcephalException("Unable to Return Measures.", e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public List<B> getBrowserDatasByGroup(int groupOid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatasbygroup/" + groupOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<B> objects = RestSharp.SimpleJson.DeserializeObject<List<B>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of objects</returns>
        public List<T> getAll() 
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/list", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                Console.WriteLine("List = " + queryResult.StatusCode);
                                
                List<T> objects = RestSharp.SimpleJson.DeserializeObject<List<T>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of objects.", e);
                throw new ServiceExecption("Unable to retrieve list of objects.", e);
            }            
        }

        public Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            if (this is MeasureService) return Misp.Kernel.Domain.SubjectType.MEASURE;
            else if (this is CalculatedMeasureService) return Misp.Kernel.Domain.SubjectType.CALCULATED_MEASURE;
            else if (this is TransformationTreeService) return Misp.Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
            else if (this is DesignService) return Misp.Kernel.Domain.SubjectType.DESIGN;
            else if (this is TargetService) return Misp.Kernel.Domain.SubjectType.TARGET;
            else if (this is AutomaticSourcingService) return Misp.Kernel.Domain.SubjectType.AUTOMATIC_SOURCING;
            else if (this is AllocationLogService) return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
            else if (this is PresentationService) return Misp.Kernel.Domain.SubjectType.PRESENTATION;
            else if (this is CombineTransformationTreeService) return Misp.Kernel.Domain.SubjectType.COMBINED_TRANSFORMATION_TREE;
            else return Misp.Kernel.Domain.SubjectType.DEFAULT;
            
        }

        public bool Delete(System.Collections.IList items)
        {
            bool resutl = true;
            foreach (object item in items)
            {
                if (item is Persistent && !Delete((Persistent)item)) resutl = false;
                if (item is BrowserData && !Delete(((BrowserData)item).oid)) resutl = false;
            }
            return resutl;
        }

        public bool Delete(Persistent item)
        {
            if(!item.oid.HasValue) return true;
            return Delete(item.oid.Value);
        }

        public bool Delete(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/delete/" + oid, Method.DELETE);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                return ValidateResponse(queryResult);
            }
            catch (Exception e)
            {
                logger.Error("Unable to delete object.", e);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public virtual T getByOid(int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                T value = Serializer.Deserialize<T>(queryResult.Content);

                //var settings = new JsonSerializerSettings();
                //settings.Converters.Add(new CustomJsonConverter());
                //settings.TypeNameHandling = TypeNameHandling.Objects;
                //T value = JsonConvert.DeserializeObject<T>(queryResult.Content, settings);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public virtual T getByName(string name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/duplicate/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                T value = Serializer.Deserialize<T>(queryResult.Content);

                //var settings = new JsonSerializerSettings();
                //settings.Converters.Add(new CustomJsonConverter());
                //settings.TypeNameHandling = TypeNameHandling.Objects;
                //T value = JsonConvert.DeserializeObject<T>(queryResult.Content, settings);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }

        public virtual bool buildCellProperty(String tableName, GroupProperty groupProperty) 
        {
            return true;
        }

        public virtual List<Misp.Kernel.Domain.CellProperty> getCellsValues(String tableName) 
        {
            return new List<CellProperty>(0);
        }

        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual T Save(T item)
        {
            if (item == null) return item;
            try
            { 
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save", Method.POST);
                
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(item);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                item = Serializer.Deserialize<T>(queryResult.Content);                
                try
                {
                    if(FileService != null) FileService.SaveCurrentFile();
                }
                catch (Exception) { }
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }


        /// <summary>
        /// Save as the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T SaveAs(int oid, string name)
        {            
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/saveas/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", name, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = 99999999;
                T item = Serializer.Deserialize<T>(queryResult.Content);                
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }

        public virtual T SaveAs(string currentName, string newName)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/saveas/name/" + currentName, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", newName, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = 99999999;
                T item = Serializer.Deserialize<T>(queryResult.Content);
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }

        public T SaveAs(int oid, string name,int treeOid)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/saveas/report/" + oid + "/" + treeOid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", name, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = 99999999;
                T item = Serializer.Deserialize<T>(queryResult.Content);
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }

        /// <summary>
        /// permet d'obtenir le module en courrant.
        /// </summary>
        /// <returns>
        /// Domain.SubjectType courrant
        /// </returns>
        


        /// <summary>
        /// Save as the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int SaveAsCopy(int oid)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/saveas/copy/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = 99999999;
                int copyOid = Serializer.Deserialize<int>(queryResult.Content);
                return copyOid;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }

        /// <summary>
        /// Rename the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Rename(int oid, string name)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/renameobject/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", name, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = 99999999;
                T item = Serializer.Deserialize<T>(queryResult.Content);
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to rename Item.", e);
                throw new BcephalException("Unable to rename Item.", e);
            }
        }

        public Boolean IsDuplicateName(int oid, string newName)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/isduplicate/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", newName, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = 99999999;
                Boolean duplicate = Serializer.Deserialize<Boolean>(queryResult.Content);
                return duplicate;
            }
            catch (Exception e)
            {
                logger.Error("Unable to rename Item.", e);
                throw new BcephalException("Unable to rename Item.", e);
            }
        }

        public B SaveBrowserData(B item)
        {
            if (item == null) return item;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/savebrowserdata", Method.POST);

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(item);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                item = Serializer.Deserialize<B>(queryResult.Content);                
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }


        public string Serialize(Object item)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            return serializer.Serialize(item);
        }

    }
}
