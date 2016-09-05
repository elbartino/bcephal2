using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class FileTransferResource : Service<Persistent, BrowserData>
    {
        
        public void download()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/download", Method.GET);
                byte[] fileByte = RestClient.DownloadData(request);     
                
                string path = "B:\\test-2.xlsx";
                System.IO.File.WriteAllBytes(path, fileByte);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public void upload()
        {
            var request = new RestRequest(ResourcePath + "/upload", Method.POST);
            string path = "B:\\test.xlsx";
            request.AddFile("test", path);
            IRestResponse response = RestClient.Execute(request);

            if (response.ErrorException != null)
            {
                Console.WriteLine("Error: " + response.ErrorException);
            }

            Console.WriteLine("content=" + response.Content);
        }



    }
}
