using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class ResourcePath
    {
        public static string FILE_RESOURCE_PATH     = "file";
        public static string GROUP_RESOURCE_PATH    = "group";

        public static string DASHBOARD_RESOURCE_PATH = "dashboard";

        public static string INITIATION_RESOURCE_PATH   = "initiation";
        public static string MODEL_RESOURCE_PATH        = INITIATION_RESOURCE_PATH + "/model";
        public static string MEASURE_RESOURCE_PATH      = INITIATION_RESOURCE_PATH + "/measure";
        public static string PERIODICITY_RESOURCE_PATH  = INITIATION_RESOURCE_PATH + "/periodicity";
        public static string PERIOD_NAME_RESOURCE_PATH  = "period_name";



        public static string SOURCING_RESOURCE_PATH             = "sourcing";
        public static string INPUT_TABLE_RESOURCE_PATH          = SOURCING_RESOURCE_PATH + "/table";
        public static string SOCKET_INPUT_TABLE_RESOURCE_PATH   = ApplicationManager.Instance.ServerWebSocketUri + "table";
        public static string TARGET_RESOURCE_PATH               = SOURCING_RESOURCE_PATH + "/target";
        public static string AUTOMATIC_SOURCING_RESOURCE_PATH   = "/automatic";
        public static string SOCKET_AUTOMATIC_SOURCING_RESOURCE_PATH = ApplicationManager.Instance.ServerWebSocketUri + "automatic";
        public static string AUTOMATIC_TARGET_RESOURCE_PATH     = AUTOMATIC_SOURCING_RESOURCE_PATH + "/target";
        public static string UPLOAD_MULTIPE_FILES_RESOURCE_PATH = "/upload";
        public static string CLEAR_ALL_RESOURCE_PATH            = ALLOCATION_RESOURCE_PATH + "/clear_all";
        public static string SOCKET_ALLOCATION_RESOURCE_PATH    = ApplicationManager.Instance.ServerWebSocketUri + "table";
        public static string SOCKET_CLEAR_ALL_ALLOCATION_RESOURCE_PATH = ApplicationManager.Instance.ServerWebSocketUri + "allocation/clear/all/";



        public static string DESIGN_RESOURCE_PATH   = "/design";
        public static string TAG_RESOURCE_PATH      = "tag";
        public static string TAG_NAME_RESOURCE_PATH = "tag_name";


        public static string PLANIFICATION_RESOURCE_PATH                = "planification";
        public static string TRANSFORMATION_TREE_RESOURCE_PATH          = PLANIFICATION_RESOURCE_PATH + "/transformation";
        public static string TRANSFORMATION_TABLE_RESOURCE_PATH         = PLANIFICATION_RESOURCE_PATH + "/transformation/table";
        public static string TRANSFORMATION_SLIDE_RESOURCE_PATH         = PLANIFICATION_RESOURCE_PATH + "/transformation/slide";
        public static string TRANSFORMATION_COMBINED_RESOURCE_PATH      = PLANIFICATION_RESOURCE_PATH + "/transformation/combined";
        public static string SOCKET_TRANSFORMATION_TREE_RESOURCE_PATH   = ApplicationManager.Instance.ServerWebSocketUri + "planification/transformation";
        public static string SOCKET_TRANSFORMATION_TABLE_RESOURCE_PATH  = ApplicationManager.Instance.ServerWebSocketUri + "planification/transformation/table";
        public static string SOCKET_TRANSFORMATION_SLIDE_RESOURCE_PATH  = ApplicationManager.Instance.ServerWebSocketUri + "planification/slide";
        

        public static string ALLOCATION_RESOURCE_PATH = "allocation";
        public static string ALLOCATION_LOG_RESOURCE_PATH = ALLOCATION_RESOURCE_PATH + "/log";

        public static string REPORTING_RESOURCE_PATH = "reporting";

        public static string AUDIT_RESOURCE_PATH = "audit";
        public static string AUDIT_ALLOCATION_RESOURCE_PATH = ALLOCATION_RESOURCE_PATH + "/audit";
        public static string AUDIT_REPORT_RESOURCE_PATH = REPORTING_RESOURCE_PATH + "/audit";


        
        public static string REPORT_RESOURCE_PATH               = REPORTING_RESOURCE_PATH + "/report";
        public static string STRUCTURED_REPORT_RESOURCE_PATH    = REPORTING_RESOURCE_PATH + "/structured_report";
        public static string SOCKET_REPORT_RESOURCE_PATH        = ApplicationManager.Instance.ServerWebSocketUri + "report";
        public static string CALCULATED_MEASURE_RESOURCE_PATH = INITIATION_RESOURCE_PATH + "/calculatedmeasure";

        public static string RECONCILIATION_RESOURCE_PATH = "reconciliation";
        public static string RECONCILIATON_POSTING_RESOURCE_PATH = RECONCILIATION_RESOURCE_PATH + "/posting";
        public static string RECONCILIATON_FILTERS_RESOURCE_PATH = RECONCILIATION_RESOURCE_PATH + "/filters";
        public static string TRANSACTION_FILE_TYPE_RESOURCE_PATH = RECONCILIATION_RESOURCE_PATH + "/transaction_file_type";


        public static string SECURITY_RESOURCE_PATH = "security";
        public static string SECURITY_USER_RESOURCE_PATH = SECURITY_RESOURCE_PATH + "/user";
        public static string SECURITY_PROFIL_RESOURCE_PATH = SECURITY_RESOURCE_PATH + "/profil";
        public static string SECURITY_ROLE_RESOURCE_PATH = SECURITY_RESOURCE_PATH + "/role";
   
    }
}
