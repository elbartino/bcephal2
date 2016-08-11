using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class FileTransferService : Service<Persistent, BrowserData>
    {


        /// <summary>
        /// get Role by oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public void download()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/file", Method.GET);
                byte[] bytes = RestClient.DownloadData(request);
                string filePath = "D:\\cards-1.xlsx";
                ByteArrayToFile(filePath, bytes);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }


        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        //test envoi de fichier. code a supprimer !!!
        public void sendExcelFile()
        {
            //Kernel.Service.FileDirs fileDirs = this.FileService.GetFileDirs();
            //string filePath = fileDirs.InputTableDir + "testTransfert.xlsx";
            //try
            //{
            //    JavaScriptSerializer serializer = new JavaScriptSerializer();
            //    var request = new RestRequest(ResourcePath + "/sends", Method.POST);
            //    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            //    /*Re-Ecriture du fichier pour se rassurer que le byte est correct*/
            //    string filePath2 = fileDirs.InputTableDir + "Copy_testTransfert.xlsx";
            //    System.IO.File.WriteAllBytes(filePath2, fileBytes);

            //    /*Envoie du fichier au serveur*/
            //    request.RequestFormat = DataFormat.Json;
            //    serializer.MaxJsonLength = int.MaxValue;
            //    string json = serializer.Serialize(fileBytes);
            //    request.AddParameter("application/json", json, ParameterType.RequestBody);
            //    var response = RestClient.ExecuteTaskAsync(request);
            //    RestResponse queryResult = (RestResponse)response.Result;
            //    bool resp = serializer.Deserialize<bool>(queryResult.Content);
            //    return resp;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }

    }
}
