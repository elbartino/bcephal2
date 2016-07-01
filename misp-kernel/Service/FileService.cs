using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using RestSharp;
using System.Net;
using System.Web.Script.Serialization;
using Misp.Kernel.Domain.Browser;

namespace Misp.Kernel.Service
{

    /// <summary>
    /// Cette classe implémente toutes les méthodes necessaires pour interragir avec le serveur au sujet d'un fichier.
    /// On s'adresse à cette classe pour:
    ///     - créer un nouveau fichier, 
    ///     - ouvrir un fichier existant
    ///     - renommer un fichier
    ///     - faire une copie d'un fichier.
    ///     
    /// La communication avec le serveur se fait via des requêtes HTTP.
    /// </summary>
    public class FileService : Service<File, BrowserData>
    {

        public DashBoardService DashBoardService { get; set; }

        /// <summary>
        /// Is server alive?
        /// </summary>
        /// <returns></returns>
        public bool IsServerAlive()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/alive", Method.GET);
                RestClient.Authenticator = new HttpBasicAuthenticator("joseph", "secret");  //new SimpleAuthenticator("username", "joseph", "password", "secret");
                var queryResult = RestClient.ExecuteTaskAsync(request);
                if (String.IsNullOrEmpty(queryResult.Result.Content)) return true;
                bool value = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Result.Content);
                return value;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ApplcationConfiguration GetApplcationConfiguration()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/application-config", Method.GET);
                var queryResult = RestClient.Execute(request);
                ApplcationConfiguration config = RestSharp.SimpleJson.DeserializeObject<ApplcationConfiguration>(queryResult.Content);
                return config;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of files</returns>
        public List<String> getProjects()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/projects", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<String> objects = RestSharp.SimpleJson.DeserializeObject<List<String>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of projects.", e);
                throw new ServiceExecption("Unable to retrieve list of projects.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of files</returns>
        public List<String> getRecentOpenedProjects()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/recents-projects", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<String> objects = RestSharp.SimpleJson.DeserializeObject<List<String>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of projects.", e);
                throw new ServiceExecption("Unable to retrieve list of projects.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of files</returns>
        public String getDefaultNewProjectName()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/new-project-name", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                String name = RestSharp.SimpleJson.DeserializeObject<String> (queryResult.Content);
                return name;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve default new project name.", e);
                throw new ServiceExecption("Unable to retrieve default new project name.", e);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of files</returns>
        public bool isProjectExist(String name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/is-project-exist/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                bool value = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                return true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public File CreateFile(string fileDir, string fileName)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/create", Method.POST);
                request.AddParameter("fileDir", fileDir);
                request.AddParameter("fileName", fileName);

                var queryResult = RestClient.ExecuteTaskAsync(request);
                checkError(queryResult.Result);
                File value = RestSharp.SimpleJson.DeserializeObject<File>(queryResult.Result.Content);
                return value;
            }
            catch (BcephalException e)
            {
                logger.Error("Unable to create file", e);
                throw new BcephalException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error("Unable to create file", e);
                throw new BcephalException("Unable to create new File.", e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public File OpenFile(string fileDir, string fileName)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/open", Method.POST);
                request.AddParameter("fileDir", fileDir);
                request.AddParameter("fileName", fileName);

                var queryResult = RestClient.ExecuteTaskAsync(request);
                checkError(queryResult.Result);
                File value = RestSharp.SimpleJson.DeserializeObject<File>(queryResult.Result.Content);
                return value;
            }
            catch (BcephalException e)
            {
                logger.Error("Unable to open file", e);
                throw new BcephalException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error("Unable to open file", e);
                throw new BcephalException("Unable to open File.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public File SaveAs(string fileDir, string fileName)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/saveas", Method.POST);
                request.AddParameter("fileDir", fileDir);
                request.AddParameter("fileName", fileName);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                File value = RestSharp.SimpleJson.DeserializeObject<File>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to save File.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public File Rename(string fileDir, string fileName)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/rename", Method.POST);
                request.AddParameter("fileDir", fileDir);
                request.AddParameter("fileName", fileName);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                File value = RestSharp.SimpleJson.DeserializeObject<File>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to rename File.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public bool CloseFile()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/close", Method.POST);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                checkError(queryResult);
                bool value = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return true;
            }
            catch (Exception e)
            {
                throw new BcephalException(e.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public bool ShutdownApplication()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/shutdown", Method.POST);                
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                checkError(queryResult);
                if (String.IsNullOrEmpty(queryResult.Content)) return true;
                bool value = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                throw new BcephalException(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        public void ForceShutdownApplication()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/force_shutdown", Method.POST);
                var queryResult = RestClient.ExecuteTaskAsync(request);
            }
            catch (Exception){ }
        }

        

        protected void checkError(IRestResponse response)
        {
            if (response == null) throw new BcephalException("Error");
            if (response.StatusCode == HttpStatusCode.OK) return;
            if (response.StatusCode == 0) return;
            throw new BcephalException(response.Content);
        }

        public FileDirs GetFileDirs()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/filedirs", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                FileDirs value = RestSharp.SimpleJson.DeserializeObject<FileDirs>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to get FileDirs.", e);
            }
        }


        public bool SaveCurrentFile()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/savecurrent", Method.GET);
                var response = RestClient.ExecuteTaskAsync(request);

                bool result = RestSharp.SimpleJson.DeserializeObject<bool>(response.Result.Content);
                return result;
            }
            catch (Exception e)
            {
                return false;
            }            
        }


        public long GetAllocationCount()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/allocation_count", Method.GET);
                var response = RestClient.ExecuteTaskAsync(request);
                long result = RestSharp.SimpleJson.DeserializeObject<int>(response.Result.Content);
                return result;
            }
            catch (Exception e)
            {
                return 0;
            }      
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>ArchiveConfiguration</returns>
        public ArchiveConfiguration getArchiveConfiguration()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/archive/configuration", Method.GET);
                var queryResult = RestClient.ExecuteTaskAsync(request);
                checkError(queryResult.Result);
                ArchiveConfiguration config = RestSharp.SimpleJson.DeserializeObject<ArchiveConfiguration>(queryResult.Result.Content);
                return config;
            }
            catch (BcephalException e)
            {
                logger.Error("Unable to retrieve archive configuration", e);
                throw new BcephalException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve archive configuration", e);
                throw new BcephalException("Unable to retrieve archive configuration.", e);
            }
        }

        public bool saveArchiveConfiguration(ArchiveConfiguration config)
        {
            if (config == null) return true;
            try
            { 
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/archive/configuration/save", Method.POST);
                
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(config);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                return Serializer.Deserialize<bool>(response.Result.Content);  
            }
            catch (Exception e)
            {
                logger.Error("Unable to save ArchiveConfiguration.", e);
                throw new BcephalException("Unable to save ArchiveConfiguration.", e);
            }
            return false;
        }

        public bool saveSimpleArchive(SimpleArchive archive)
        {
            if (archive == null) return true;
            try
            { 
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/archive/simple/save", Method.POST);
                
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(archive);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                return Serializer.Deserialize<bool>(response.Result.Content);  
            }
            catch (Exception e)
            {
                logger.Error("Unable to save SimpleArchive.", e);
                throw new BcephalException("Unable to save SimpleArchive.", e);
            }
            return false;
        }
        

    }
}
