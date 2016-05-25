using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class RoleService : Service<Misp.Kernel.Domain.Role, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public PeriodNameService periodNameService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public MeasureService measureService { get; set; }

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public PostingService postingService { get; set; }

        /// <summary>
        /// L'InputTableService
        /// </summary>
        public Service.Service<InputTable, Kernel.Domain.Browser.InputTableBrowserData> InputTableService { get; set; }


        /// <summary>
        /// Le CalculatedMeasureService
        /// </summary>
        public CalculatedMeasureService CalculatedMeasureService { get; set; }


        /// <summary>
        ///  Le TargetService
        /// </summary>
        public TargetService TargetService { get; set; }
        #endregion

        #region Profil

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
