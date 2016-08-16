using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Service;
using RestSharp;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using WebSocketSharp;

namespace Misp.Reporting.StructuredReport
{
    public class StructuredReportService : Service<Misp.Kernel.Domain.StructuredReport, Misp.Kernel.Domain.Browser.BrowserData>
    {

        /// <summary>
        /// Le MeasureService.
        /// </summary>
        public MeasureService MeasureService { get; set; }

        /// <summary>
        /// Le Calculated measure service.
        /// </summary>
        public CalculatedMeasureService CalculatedMeasureService { get; set; }

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public PeriodicityService PeriodicityService { get; set; }
        
        /// <summary>
        /// Le TargetService.
        /// </summary>
        public TargetService TargetService { get; set; }


        /// <summary>
        /// Le PeriodNameService.
        /// </summary>
        public PeriodNameService PeriodNameService { get; set; }

        public event RunInfoEventHandler RunHandler;

        /// <summary>
        /// Demande au serveur d'exécuter le rapport
        /// </summary>
        /// <param name="oid"> L'identifiant de la table</param>
        /// <returns>La table</returns>
        /// 

        
        public virtual void Run(StructuredReportRunData data)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = buildSocket(SocketResourcePath + "/structured/run/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (RunHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError += (sender, e) => { logger.Debug("Socket error  : " + e.Message); };
            socket.OnClose += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.STRUCTURED_REPORT;

        }

        public AllocationRunInfo deserializeRunInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AllocationRunInfo runInfo = Serializer.Deserialize<AllocationRunInfo>(json);
                //if (runInfo == null || runInfo.runedCellCount == 0) return null;
                return runInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize runInfo!", e);
            }
            return null;
        }




    }
}
