using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class FileTransferService : Service<Domain.Persistent, BrowserData>
    {

        public bool uploadFile()
        {
            try
            {
                string name = "cards";
                string ext = ".xlsx";
                string filePath = "D:\\" + name + ext;
                var request = new RestRequest(ResourcePath + "/upload/" + name + "/" + ext, Method.POST);
                request.AddHeader("Content-Type", "application/octet-stream");
                request.RequestFormat = RestSharp.DataFormat.Json;
                byte[] dataToSend = File.ReadAllBytes(filePath);
                request.AddParameter("application/octet-stream", dataToSend, ParameterType.RequestBody);
                IRestResponse response = RestClient.Execute(request);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                bool ok = Serializer.Deserialize<bool>(response.Content);
                return ok;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
            }
            return false;
        }



        public bool downloadFile()
        {
            try
            {
                string name = "cards";
                string ext = ".xlsx";
                var request = new RestRequest(ResourcePath + "/download/" + name + "/" + ext, Method.GET);
                request.AddHeader("Content-Type", "application/octet-stream");
                request.RequestFormat = RestSharp.DataFormat.Json;                
                byte[] data = RestClient.DownloadData(request);
                string filePath = "D:\\Wtest2\\" + name + ext;
                File.WriteAllBytes(filePath, data);
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
            }
            return false;
        }


        

    }
}
