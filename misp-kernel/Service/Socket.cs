using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class Socket : WebSocket
    {
                
        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILog logger;
        
        /// <summary>
        /// RestClient
        /// </summary>
        public string ResourcePath { get; set; }


        public Socket(string path, string user, string password) : base(path)
        {
            logger = LogManager.GetLogger(this.GetType());
            this.ResourcePath = path;
            this.SetCredentials(user, password, true);
            //Initialize();
        }

        protected void Initialize()
        {
            this.OnOpen += (sender, e) =>
            {
                logger.Debug("Socket opened!");
            };

            this.OnMessage += (sender, e) =>
            {
                logger.Debug("Recieve text : " + e.Data);
            };

            this.OnError += (sender, e) =>
            {
                logger.Debug("Socket error : " + e.Message);
            };

            this.OnClose += (sender, e) =>
            {
                logger.Debug("Socket closed : " + e.Reason);
            };
        }



    }
}
