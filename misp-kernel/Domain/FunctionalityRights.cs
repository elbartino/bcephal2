using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class FunctionalityRights 
    {
        /* File functionality*/
        public static string CREATE_FILE= "Create File";
        public static string BACKUP_FILE = "Backup File";
        
        /* Initiation functionality*/
        public static string INITIATION_FUNCTION = "Initiation";
        public static string INITIATION_EDIT_MODEL = "Edit Model";
        public static string INITIATION_EDIT_PERIOD = "Edit Period";

        #region Functionality Sourcing
        /*SOURCING*/

        // Posting Functionality
        public static string POSTING = "Posting";
        public static string POSTING_GRID = "Posting Grid";
        public static string POSTING_AUTO_SOURCING = "Automatic Sourcing for Posting";

        // Input Table Functionality
        public static string INPUT_TABLE = "Input Table";
        public static string INPUT_TABLE_NEW = "New Input Table";
        public static string INPUT_TABLE_LIST = "List Input Table";

        // Grid Functionality
        public static string GRID = "Grid";
        public static string GRID_INPUT = "Input Grig";
        public static string GRID_AUTO_SOURCING = "Automatic Sourcing for Grid";

        // Target Functionality
        public static string TARGET = "Target";
        public static string TARGET_NEW = "New Target";
        public static string TARGET_LIST = "List Target";

        // DESIGN Functionality
        public static string DESIGN = "Target";
        public static string DESIGN_NEW = "New Design";
        public static string DESIGN_LIST = "List Design";

        // UPLOAD FILE Functionality
        public static string MULTIPLE_FILE_UPLOAD = "Upload Mutiple File";

        #endregion

        #region Functionality Tansformation Tree

        // Transformation Tree Functionality
        public static string TRANSFORMATION_TREE = "Transformation Tree";
        public static string TRANSFORMATION_TREE_NEW = "New Transformation Tree";
        public static string TRANSFORMATION_TREE_LIST = "List Transformation Tree";

        // Combined Transformation Tree Functionality
        public static string COMBINED_TRANSFORMATION_TREE = "Combined Transformation Tree";
        public static string COMBINED_TRANSFORMATION_TREE_NEW = "New Combined Transformation Tree";
        public static string COMBINED_TRANSFORMATION_TREE_LIST = "List Combined Transformation Tree";

        #endregion

        #region Functionality Load Table

        // Transformation Tree Functionality
        public static string LOAD_TABLE = "Load Table";
        public static string CLEAR_TABLE = "Clear Table";
        public static string LOAD_LOG = "Load Log";

        #endregion

        #region Functionality Report


        // Report Grid Functionality
        public static string REPORT_GRID = "Report Grid";
        public static string REPORT_GRID_NEW = "New Report Grid";
        public static string REPORT_GRID_LIST = "List Report Grid";

        // Report Functionality
        public static string REPORT = "Report";
        public static string REPORT_NEW = "New Report";
        public static string REPORT_LIST = "List Report";

        // Calculated Measure Functionality
        public static string CALCULATED_MEASURE = "Calculated Measure";
        public static string CALCULATED_MEASURE_NEW = "New Calculated Measure";
        public static string CALCULATED_MEASURE_LIST = "List Calculated Measure";

        // Pivot Table Functionality
        public static string PIVOT_TABLE = "Pivot Table";
        public static string PIVOT_TABLE_NEW = "New Pivot Table";
        public static string PIVOT_TABLE_LIST = "List Pivot Table";

        #endregion

        #region Functionality Reconciliation

        // Reconciliation Functionality
        public static string RECONCILIATION = "Reconciliation";
        public static string POSTING_RECONCILIATION = "Posting";
        public static string RECONCILIATION_CONFIG = "Reconciliation Config";

        #endregion

        #region Functionality Administration


        // User Functionality
        public static string USER = "User";
        public static string USER_NEW = "New User";
        public static string USER_LIST = "List User";

        // Profil Functionality
        public static string PROFIL = "Profil";
        public static string PROFIL_NEW = "New Profil";
        public static string PROFIL_LIST = "List Profil";

        // Role Functionality
        public static string MANAGE_ROLE = "Manage Role";
        #endregion

        #region Functionality Setting

        // Group Functionality
        public static string GROUP = "Group";
        #endregion
    }
}
