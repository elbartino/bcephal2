using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;
using Misp.Kernel.Domain.Browser;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class ModelService : Service<Misp.Kernel.Domain.Model, Domain.Browser.BrowserData>
    {
        /// <summary>
        /// Retoune la liste de Models du fichier ouvert.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>La liste de Models du fichier ouvert</returns>
        public List<Kernel.Domain.Model> getModels()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/get", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.Model> models = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Model>>(queryResult.Content);
                    return models;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return models.", e);
            }
        }

        public List<Kernel.Domain.Model> getModelsForSideBar() 
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/models-for-sidebar", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.Model> models = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Model>>(queryResult.Content);
                    return models;
                }
                catch (Exception)
                {
                    return new List<Model>(0);
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return models.", e);
            }
        }


        public List<Kernel.Domain.Attribute> getRootAttributesByEntity(int entityOid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root-attributes/" + entityOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<Kernel.Domain.Attribute> attributes = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Attribute>>(queryResult.Content);
                return attributes;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributes.", e);
            }
        }

        public List<Kernel.Domain.Attribute> getAttributeChildren(int parentOid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/attribute-children/" + parentOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<Kernel.Domain.Attribute> attributes = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Attribute>>(queryResult.Content);
                return attributes;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributes.", e);
            }
        }

        public BrowserDataPage<Kernel.Domain.AttributeValue> getAllAttributeValuesByAttribute(BrowserDataFilter filter)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/all-attribute-values", Method.POST);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<Kernel.Domain.AttributeValue> values = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<Kernel.Domain.AttributeValue>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributes.", e);
            }
        }

        public BrowserDataPage<Kernel.Domain.AttributeValue> getRootAttributeValuesByAttribute(BrowserDataFilter filter)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root-attribute-values", Method.POST);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<Kernel.Domain.AttributeValue> values = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<Kernel.Domain.AttributeValue>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributes.", e);
            }
        }

        public BrowserDataPage<Kernel.Domain.AttributeValue> getAttributeValueChildren(BrowserDataFilter filter)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/attribute-value-children", Method.POST);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<Kernel.Domain.AttributeValue> values = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<Kernel.Domain.AttributeValue>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributes.", e);
            }
        }


        public List<Kernel.Domain.AttributeValue> getAttributeValuesByAttribute(int attributeOid) 
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/valuesbyattribute/"+attributeOid, Method.POST);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<Kernel.Domain.AttributeValue> models = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.AttributeValue>>(queryResult.Content);
                return models;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributeValues.", e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Kernel.Domain.AttributeValue> getRootAttributeValues()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/rootvalues", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.AttributeValue> values = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.AttributeValue>>(queryResult.Content);
                    return values;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return values.", e);
            }
        }

        public Target getTargetAll()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/targetall", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Target targetAll = RestSharp.SimpleJson.DeserializeObject<Target> (queryResult.Content);
                    return targetAll;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

        public AttributeValue getOrCreateAttributeValue(int oid, string value) 
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/getorcreate/"+oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", value, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    AttributeValue AttributeValue = RestSharp.SimpleJson.DeserializeObject<AttributeValue>(queryResult.Content);
                    return AttributeValue;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

        public AttributeValue getAttributeValue(int oid, string value)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/getAttributeValue/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", value, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    AttributeValue AttributeValue = RestSharp.SimpleJson.DeserializeObject<AttributeValue>(queryResult.Content);
                    return AttributeValue;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

        

    public AttributeValue getAttributeValueByOid(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/getAttributeValueByOid/" , Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", oid, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    AttributeValue AttributeValue = RestSharp.SimpleJson.DeserializeObject<AttributeValue>(queryResult.Content);
                    return AttributeValue;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

    public Misp.Kernel.Domain.Attribute getAttributeByOid(int oid)
    {
        try
        {
            var request = new RestRequest(ResourcePath + "/getAttributeByOid/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", oid, ParameterType.RequestBody);
            RestResponse queryResult = (RestResponse)RestClient.Execute(request);
            try
            {
                Kernel.Domain.Attribute Attribute = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Attribute>(queryResult.Content);
                return Attribute;
            }
            catch (Exception)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            throw new BcephalException("Unable to Return targetAll.", e);
        }
    }

    public Misp.Kernel.Domain.Attribute getAttributeByValue(int oid)
    {
        try
        {
            var request = new RestRequest(ResourcePath + "/getAttributeByValue/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", oid, ParameterType.RequestBody);
            RestResponse queryResult = (RestResponse)RestClient.Execute(request);
            try
            {
                Kernel.Domain.Attribute Attribute = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Attribute>(queryResult.Content);
                return Attribute;
            }
            catch (Exception)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            throw new BcephalException("Unable to Return targetAll.", e);
        }
    }

        public List<AttributeValue> getAttributeValue(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/attribute/values/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<AttributeValue> ListValue = RestSharp.SimpleJson.DeserializeObject<List<AttributeValue>>(queryResult.Content);
                    return ListValue;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

        public List<AttributeValue> getAllValues()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/allvalues", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<AttributeValue> listAttributeValues = RestSharp.SimpleJson.DeserializeObject<List<AttributeValue>>(queryResult.Content);
                    return listAttributeValues;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

        public long getTargetCardinality(Target target)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/targetcardinality", Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(target);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);

                try
                {
                    long cardinality = RestSharp.SimpleJson.DeserializeObject<long>(queryResult.Content);
                    return cardinality;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }
        public String getAttributeName(String valueName) {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/attribute/name/"+valueName, Method.POST);
                
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);

                try
                {
                    String attribut = RestSharp.SimpleJson.DeserializeObject<String>(queryResult.Content);
                    if (attribut == null) return "";
                    return attribut;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return Attribute name.", e);
            }            
        }

        public Misp.Kernel.Domain.Entity getEntityByOid(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/getEntityByOid/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", oid, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.Entity Entity = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Entity>(queryResult.Content);
                    return Entity;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return targetAll.", e);
            }
        }

        public List<Kernel.Domain.AttributeValue> getAttributeValuesByPage(int attributeOid, int currentPage, int attributePageSize)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/valuesbyattributeandpage/" + attributeOid + "/" + currentPage + "/" + attributePageSize, Method.POST);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<Kernel.Domain.AttributeValue> models = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.AttributeValue>>(queryResult.Content);
                return models;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributeValues.", e);
            }
        }

        public Kernel.Domain.Attribute getAttributeWithPaginateValues(int attributeOid, int currentPage, int attributePageSize)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/attribute-page-values/" + attributeOid + "/" + currentPage + "/" + attributePageSize, Method.POST);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Kernel.Domain.Attribute model = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Attribute>(queryResult.Content);
                return model;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributeValues.", e);
            }
        }


        public List<BrowserData> getLeafAttributeValues(int attributeOid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/leaf-attribute-values/" + attributeOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<BrowserData> values = RestSharp.SimpleJson.DeserializeObject<List<BrowserData>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return attributeValues.", e);
            }
        }


    }
}
