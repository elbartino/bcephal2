using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class ProfilService : Service<Misp.Kernel.Domain.Profil, Misp.Kernel.Domain.Browser.ProfilBrowserData>
    {
        #region Properties

        #endregion

        #region Profil

        /// <summary>
        /// save profil
        /// </summary>
        /// <param name="profil"></param>
        /// <returns></returns>
        public override  Profil Save(Profil profil)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(profil);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Profil pf = RestSharp.SimpleJson.DeserializeObject<Profil>(queryResult.Content);
                return pf;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// get profil by oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Profil getUserProfil(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/profil/" + oid, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Profil profil = RestSharp.SimpleJson.DeserializeObject<Profil>(queryResult.Content);
                return profil;
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
