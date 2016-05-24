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
        
        #region User
        #endregion

        #region Rights
        #endregion

    }
}
