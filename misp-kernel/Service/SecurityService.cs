using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class SecurityService : Service<Misp.Kernel.Domain.User, Misp.Kernel.Domain.Browser.BrowserData>
    {
        
        #region User

        public User authentificate(String login, String password)
        {
            try
            {
                RestClient.Authenticator = new HttpBasicAuthenticator(login, password);
                var request = new RestRequest(ResourcePath + "/authentificate", Method.GET);                
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                User user = RestSharp.SimpleJson.DeserializeObject<User>(queryResult.Content);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


        #region Profil

        public UserAction getUserAction(int oid, String functionnality)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/action/" + oid + "/" + functionnality, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                UserAction userAction = RestSharp.SimpleJson.DeserializeObject<UserAction>(queryResult.Content);
                return userAction;
            }
            catch (Exception e) 
            {
                return UserAction.NULL;
            }
        }

        public List<Rights> getUserAction(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/rights/" + oid, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<Rights> userRights = RestSharp.SimpleJson.DeserializeObject<List<Rights>>(queryResult.Content);
                return userRights;
            }
            catch (Exception e) 
            {
                return new List<Rights>(0);
            }
        }

        public Profil getUserProfil(int oid) 
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/profil/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Profil profil = RestSharp.SimpleJson.DeserializeObject<Profil>(queryResult.Content);
                return profil;
            }
            catch (Exception e) 
            {
                return null;
            }
        }

        #endregion
        
        

        #region Rights
        #endregion

    }
}
