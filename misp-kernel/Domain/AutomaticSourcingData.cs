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
        public bool overrideExisting { get; set; }
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
                String name = System.IO.Path.GetFileNameWithoutExtension(this.excelFilePath) + "11111111111";//DateTime.Now.ToLongTimeString();
                String ext = System.IO.Path.GetExtension(this.excelFilePath);
                String path = System.IO.Path.GetDirectoryName(this.excelFilePath)
                    + System.IO.Path.DirectorySeparatorChar + name + ext;

                System.IO.File.Copy(this.excelFilePath, path);
                fileBytes = System.IO.File.ReadAllBytes(path);
                System.IO.File.Delete(path);                
            }
        }


   }
}
