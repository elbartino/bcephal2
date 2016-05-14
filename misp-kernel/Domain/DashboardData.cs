using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class DashboardData : Browser.BrowserData
    {
        private List<Browser.InputTableBrowserData> _tables;
        private List<Browser.InputTableBrowserData> _reports;
        private List<Browser.BrowserData> _models;
        private List<Browser.BrowserData> _transformationTrees;


        public DashboardData()
        {
            
        }

        public List<Browser.InputTableBrowserData> tables
        {
            set
            {
                _tables = value;
            }
            get
            {
                if (_tables == null) _tables = new List<Browser.InputTableBrowserData>(0);
                return _tables;
            }
        }

        public List<Browser.InputTableBrowserData> reports
        {
            set
            {
                _reports = value;
            }
            get
            {
                if (_reports == null) _reports = new List<Browser.InputTableBrowserData>(0);
                return _reports;
            }
        }

        public List<Browser.BrowserData> models
        {
            set
            {
                _models = value;
            }
            get
            {
                if (_models == null) _models = new List<Browser.BrowserData>(0);
                return _models;
            }
        }

        public List<Browser.BrowserData> transformationTrees
        {
            set
            {
                _transformationTrees = value;
            }
            get
            {
                if (_transformationTrees == null) _transformationTrees = new List<Browser.BrowserData>(0);
                return _transformationTrees;
            }
        }

    }
}
