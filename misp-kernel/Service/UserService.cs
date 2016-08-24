using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class UserService : Service<Misp.Kernel.Domain.User, Misp.Kernel.Domain.Browser.UserBrowserData>
    {
        #region Properties
        
        public ProfilService ProfilService { get; set; }

        public RoleService RoleService { get; set; }

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
                var request = new RestRequest(ResourcePath + "/save", Method.POST);
                string json = Serialize(user);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                User usr = RestSharp.SimpleJson.DeserializeObject<User>(queryResult.Content);
                return usr;
            }
            catch (Exception)
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
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// get all user in data base 
        /// different to @user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> getUsersRelation(Domain.User user)
        {
            if (user.oid == null)
            {
                return  (from o in getAll() select o.ToString()).ToList();
            }
            else
            {
                var request = new RestRequest(ResourcePath + "/user_relation/" + user.oid, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                List<string> us = Serializer.Deserialize<List<string>>(queryResult.Content);
                return us;
            }
            
        }

        /// <summary>
        /// get Role by oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public User getUserByLogin(string login)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/login/" + login, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                User u = RestSharp.SimpleJson.DeserializeObject<User>(queryResult.Content);
                return u;
            }
            catch (Exception)
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
