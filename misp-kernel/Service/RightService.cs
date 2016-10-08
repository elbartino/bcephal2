﻿using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class RightService : Service<Misp.Kernel.Domain.Right, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        
        #endregion

        #region Right

        /// <summary>
        /// Right according to Profil
        /// </summary>
        /// <returns>return rights according to profil </returns>
        public List<Right> getRightByProfil(Profil profil)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/r_profil", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(profil);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                List<Right> objects = RestSharp.SimpleJson.DeserializeObject<List<Right>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve right.", e);
                throw new ServiceExecption("Unable to retrieve right.", e);
            }
        }

        /// <summary>
        /// Right according to user
        /// </summary>
        /// <returns>return rights according to user </returns>
        public List<Right> getRightByUser(User user)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/r_user", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(user);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                List<Right> objects = RestSharp.SimpleJson.DeserializeObject<List<Right>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve right.", e);
                throw new ServiceExecption("Unable to retrieve right.", e);
            }
        }

        /// <summary>
        /// Right according to user
        /// </summary>
        /// <returns>return all rights according to user only and profil of user </returns>
        public List<Right> getRightByUserProfil(User user)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/r_userprofil", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(user);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                List<Right> objects = RestSharp.SimpleJson.DeserializeObject<List<Right>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve right.", e);
                throw new ServiceExecption("Unable to retrieve right.", e);
            }
        }
       
        #endregion
        
        #region User
        #endregion

    }
}