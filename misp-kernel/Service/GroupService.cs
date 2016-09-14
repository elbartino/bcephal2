using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using RestSharp;
using Misp.Kernel.Domain.Browser;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class GroupService : Service.Service<BGroup, BrowserData>
    {


        public BrowserDataPage<BrowserData> getBrowserDatasByCategory(BrowserDataFilter filter, String category)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/browserdatasbycategory/" + category, Method.POST);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<BrowserData> objects = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<BrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

        public BGroup getGroupByNameAndType(String name, String type)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/byname/type/" + name+"/"+type, Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    BGroup group = RestSharp.SimpleJson.DeserializeObject<BGroup>(queryResult.Content);
                    return group;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return group named: " + name, e);
            }
        }

        /// <summary>
        /// Retoune la liste de mesures du fichier ouvert.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>La liste de groupes</returns>
        public BGroup getRootGroup(SubjectType subjectType)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root/" + subjectType.label, Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    BGroup root = RestSharp.SimpleJson.DeserializeObject<BGroup>(queryResult.Content);
                    return root;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return Measures.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BGroup getDefaultGroup(String subjectType)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/default/"+subjectType, Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    BGroup group = RestSharp.SimpleJson.DeserializeObject<BGroup>(queryResult.Content);
                    return group;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return default group", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BGroup getDefaultGroup()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/default", Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    BGroup group = RestSharp.SimpleJson.DeserializeObject<BGroup>(queryResult.Content);
                    return group;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return default group", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BGroup getGroupByName(string name)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/byname/" + name, Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    BGroup group = RestSharp.SimpleJson.DeserializeObject<BGroup>(queryResult.Content);
                    return group;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return group named: " + name, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BGroup> getGroupByNameTemplate(string name)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/bynametemplate/" + name, Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<BGroup> groups = RestSharp.SimpleJson.DeserializeObject<List<BGroup>>(queryResult.Content);
                    return groups;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return group named: " + name, e);
            }
        }


        public void delete(BGroup group)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/deleteg/" + group.oid, Method.DELETE);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to delete group"+ e );
            }
        }
    }
}
