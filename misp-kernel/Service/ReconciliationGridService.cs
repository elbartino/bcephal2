using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Domain.Browser;
using System.Threading;
using Misp.Kernel.Ui.Base;
using RestSharp;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class ReconciliationGridService : InputGridService
    {

        public PostingService PostingService { get; set; }

        public ReconciliationContextService ReconciliationContextService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public override Grille getByOid(int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                ReconciliationGrid value = Serializer.Deserialize<ReconciliationGrid>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public ReconciliationGrid getNewReconciliationGrid(String name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/new/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                ReconciliationGrid value = Serializer.Deserialize<ReconciliationGrid>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }

    }
}
