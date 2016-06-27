using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class UserService : Service<Misp.Kernel.Domain.User, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        
        public ProfilService ProfilService { get; set; }

        #endregion

        #region USER

        /// <summary>
        /// save role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public override User Save(User user)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/save/user", Method.POST);
                string json = Serialize(user);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                User usr = RestSharp.SimpleJson.DeserializeObject<User>(queryResult.Content);
                return usr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        

        /// <summary>
        /// get Role by oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public User getUser(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/user/" + oid, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                User u = RestSharp.SimpleJson.DeserializeObject<User>(queryResult.Content);
                return u;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion
        
        #region User
        #endregion

        #region Rights
        #endregion

    }
}
