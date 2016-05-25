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

        public User saveAdministrator(User user)
        {
            try
            {
                RestClient.Authenticator = new HttpBasicAuthenticator(user.login, user.password);
                var request = new RestRequest(ResourcePath + "/save-administrator", Method.POST);
               
                string json = request.JsonSerializer.Serialize(user);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                User admin = RestSharp.SimpleJson.DeserializeObject<User>(queryResult.Content);
                return admin;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public long getUserCount()
        {
            try
            {
                RestClient.Authenticator = new HttpBasicAuthenticator("", "");
                var request = new RestRequest(ResourcePath + "/user-count", Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                long count = RestSharp.SimpleJson.DeserializeObject<long>(queryResult.Content);
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion


    }
}
