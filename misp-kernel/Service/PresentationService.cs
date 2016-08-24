using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace Misp.Kernel.Service
{
    public class PresentationService : Service<Misp.Kernel.Domain.Presentation, Misp.Kernel.Domain.Browser.BrowserData>
    {
        /// <summary>
        /// Le MeasureService.
        /// </summary>
        public MeasureService MeasureService { get; set; }

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


        /// <summary>
        /// L'InputTableService
        /// </summary>
        public Service.Service<InputTable, Kernel.Domain.Browser.InputTableBrowserData> ReportService { get; set; }


        public event SaveInfoEventHandler SavePresentationHandler;

        ///// <summary>
        ///// Retoune la table sur base de son oid.
        ///// </summary>
        ///// <param name="oid"> L'identifiant de la table à retourner</param>
        ///// <returns>La table</returns>
        //public virtual Misp.Kernel.Domain.Presentation getByName(String name)
        //{
        //    try
        //    {
        //        var request = new RestRequest(ResourcePath + "/get/" + name, Method.POST);
        //        request.DateFormat = "dd/MM/yyyy";
        //        RestResponse queryResult = (RestResponse)RestClient.Execute(request);
        //        try
        //        {
        //            JavaScriptSerializer Serializer = new JavaScriptSerializer();
        //            Serializer.MaxJsonLength = int.MaxValue;
        //            Misp.Kernel.Domain.Presentation slide = Serializer.Deserialize<Misp.Kernel.Domain.Presentation>(queryResult.Content);
        //            return slide;
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new BcephalException("Unable to Return presentation identified by: " + name, e);
        //    }
        //}

        public override Misp.Kernel.Domain.Presentation Save(Misp.Kernel.Domain.Presentation item)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = buildSocket(SocketResourcePath + "/Save/");
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SavePresentationHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SavePresentationHandler != null) SavePresentationHandler(info, null);
                    }
                        );
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                Presentation presentation = deserializePresentation(e.Data);
                if (presentation != null)
                {
                    if (SavePresentationHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SavePresentationHandler != null)
                            SavePresentationHandler(null, presentation);
                    }
                        );
                    return;
                }

                logger.Debug("Recieve text : " + e.Data);
            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened!"); };
            socket.OnError += (sender, e) =>
            {
                logger.Debug("Socket error : " + e.Message);
            };
            socket.OnClose += (sender, e) =>
            {
                logger.Debug("Socket closed : " + e.Reason);
            };

            socket.Connect();
            string text = serializer.Serialize(item);
            socket.Send(text);
            return item;
        }

        public SaveInfo deserializeSaveInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                SaveInfo saveInfo = Serializer.Deserialize<SaveInfo>(json);
                if (saveInfo == null || saveInfo.stepCount < 1) return null;
                return saveInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize saveInfo!", e);
            }
            return null;
        }

        public Presentation deserializePresentation(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                Presentation presentation = Serializer.Deserialize<Presentation>(json);
                if (presentation == null || presentation.oid == null || !presentation.oid.HasValue) return null;
                return presentation;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize InputTable!", e);
            }
            return null;
        }

        public String getUserSavingdir(int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/usersavingdir/"+oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                string path = RestSharp.SimpleJson.DeserializeObject<string>(queryResult.Content);
                return path;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }
      
    }
}
