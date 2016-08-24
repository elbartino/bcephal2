using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class RoleService : Service<Misp.Kernel.Domain.Role, Misp.Kernel.Domain.Browser.RoleBrowserData>
    {
        #region Properties
        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }


        #endregion

        #region Role

        /// <summary>
        /// save role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public Role saveRole(Role role)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/save", Method.POST);
                string json = Serialize(role);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Role rule = RestSharp.SimpleJson.DeserializeObject<Role>(queryResult.Content);
                return rule;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Retoune la liste de roles du fichier ouvert.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>La liste de roles du fichier ouvert</returns>
        public Kernel.Domain.Role getRootRole()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.Role root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Role>(queryResult.Content);
                    return root;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return roles.", e);
            }
        }


        /// <summary>
        /// get all Role in data base 
        /// different to @user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> getRolesRelation()
        {
            Role root = getRootRole();
            List<string> ostring = (from o in root.childrenListChangeHandler.Items.ToList() select o.ToString()).ToList();
            return ostring;
        }

        /// <summary>
        /// get Role by oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Role getRole(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/role/" + oid, Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Role ru = RestSharp.SimpleJson.DeserializeObject<Role>(queryResult.Content);
                return ru;
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
