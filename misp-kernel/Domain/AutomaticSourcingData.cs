using System;

namespace Misp.Kernel.Domain
{
   public class AutomaticSourcingData
   {
        public int automaticSourcingOid{get;set;}
        public int tableOid { get; set; }
		public String excelFilePath{get;set;}
        public String tableName { get; set; }
        public String tableGroup { get; set; }
        public String excelExtension { get; set; }
        public bool createTable { get; set; }
        public bool runTable { get; set; }
        public bool isLast { get; set; }
        public AutomaticGridAction action { get; set; }

        public byte[] fileBytes { get; set; }

        public AutomaticSourcingData() { }

        public AutomaticSourcingData( int oid, String tablename, String excelfilepath) 
        {
            this.tableName = tablename;
            this.automaticSourcingOid = oid;
            setExcelFilePath(excelfilepath);
        }
               
        public AutomaticSourcingData(int oid, String tablename, String excelfilepath,string tablegroup): this(oid,tablename,excelfilepath)
        {
            this.tableGroup = tablegroup;
        }

        public void setExcelFilePath(String excelfilepath)
        {
            this.excelFilePath = excelfilepath;
            fileBytes = null;
            if (!string.IsNullOrWhiteSpace(this.excelFilePath) && System.IO.File.Exists(this.excelFilePath))
            {
                fileBytes = System.IO.File.ReadAllBytes(this.excelFilePath);
            }
        }


   }
}
