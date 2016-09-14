using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain.Browser
{
    public class BrowserData: IComparable
    {

        public int oid { get; set; }

        public string name { get; set; }

        public string group { get; set; }

        public bool visibleInShortcut { get; set; }

        [ScriptIgnore]
        public bool selected { get; set; }

        public string creationDate { get; set; }

        public string modificationDate { get; set; }

        [ScriptIgnore]
        public string groupName { get { return group; } }



        [ScriptIgnore]
        public DateTime creationDateTime
        {
            get
            {
                return DateTime.ParseExact(creationDate, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        [ScriptIgnore]
        public DateTime modificationDateTime
        {
            get
            {
                return DateTime.ParseExact(modificationDate, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            }
        }


        public BrowserData() { }

        public BrowserData(BrowserData data)
        {
            this.oid = data.oid;
            this.name = data.name;
            this.group = data.group;
            this.visibleInShortcut = data.visibleInShortcut;
            this.creationDate = data.creationDate;
            this.modificationDate = data.modificationDate;
            this.selected = data.selected;
        }


        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.name) ? this.name : base.ToString();
        }
        
        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is BrowserData)) return 1;
            return this.name.CompareTo(((BrowserData)obj).name);
        }
    }
}
