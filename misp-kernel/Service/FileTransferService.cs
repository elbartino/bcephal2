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


        public String downloadTable(String name)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/download-table/" + name, Method.GET);
                request.AddHeader("Content-Type", "application/octet-stream");
                request.RequestFormat = RestSharp.DataFormat.Json;
                byte[] data = RestClient.DownloadData(request);
                string tempPath = System.IO.Path.GetTempPath() + "bcephal\\tables\\";
                Directory.CreateDirectory(tempPath);
                string filePath = tempPath + name;
                File.WriteAllBytes(filePath, data);
                return tempPath;
            }
            catch (Exception e)
            {
                logger.Error("Unable to download file.", e);
            }
            return null;
        }

        public bool uploadTable(String name)
        {
            try
            {
                string tempPath = System.IO.Path.GetTempPath() + "bcephal\\tables\\";
                Directory.CreateDirectory(tempPath);
                string filePath = tempPath + name;
                string ext = Path.GetExtension(name);
                string namewithNoext = Path.GetFileNameWithoutExtension(name);
                string nameCopy = namewithNoext + "-copy";
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                File.Copy(filePath, (tempPath + nameCopy + ext));
                filePath = tempPath + nameCopy + ext;
                byte[] dataToSend = File.ReadAllBytes(filePath);
                File.Delete(filePath);
                var request = new RestRequest(ResourcePath + "/upload/" + namewithNoext + "/" + ext, Method.POST);
                request.AddHeader("Content-Type", "application/octet-stream");
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddParameter("application/octet-stream", dataToSend, ParameterType.RequestBody);
                IRestResponse response = RestClient.Execute(request);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                String messageRes = Serializer.Deserialize<String>(response.Content);
                return true;
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
