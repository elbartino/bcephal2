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
        public override Profil Save(Profil profil)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string ressourceP = ResourcePath + "/save";
                var request = new RestRequest(ressourceP, Method.POST);
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
        /// save profil
        /// </summary>
        /// <param name="profil"></param>
        /// <returns></returns>
        public int Save(PersistentListChangeHandler<Domain.Profil> profilListChangeHandler, int? objectOid)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                if (objectOid == null) return 0;
                var request = new RestRequest(ResourcePath + "/save/" + objectOid, Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(profilListChangeHandler);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                int pf = RestSharp.SimpleJson.DeserializeObject<int>(queryResult.Content);
                return pf;
            }
            catch (Exception)
            {
                return 0;
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

        /// <summary>
        /// get all Role in data base 
        /// different to @user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> getProfilsRelation()
        {
            List<Profil> root = getAll();
            List<string> ostring = (from o in root select o.ToString()).ToList();
            return ostring;
        }
        
        #endregion
        
        #region User
        #endregion

        #region Rights
        #endregion


    }
}
