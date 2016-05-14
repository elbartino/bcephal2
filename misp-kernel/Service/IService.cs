using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Misp.Kernel.Service
{
    /// <summary>
    /// Cette classe encapsule les propriétés communues des services.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IService
    {

        /// <summary>
        /// RestClient
        /// </summary>
        RestClient RestClient { get; set; }

        /// <summary>
        /// RestClient
        /// </summary>
        string ResourcePath { get; set; }


    }
}
