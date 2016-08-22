using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Profil : Persistent
    {
        public string name { get; set; }

        public bool active { get; set; }

        public bool visibleInShortcut { get; set; }

        [ScriptIgnore]
        public  List<Rights> defaultListRights;

        public PersistentListChangeHandler<Rights> rightsListChangeHandler { get; set; }
        

        public Profil()
        {
            this.rightsListChangeHandler = new PersistentListChangeHandler<Rights>();            
        }

        #region Build Rights
        /// <summary>
        /// build all right;
        /// </summary>
        public void buildRight()
        {
            defaultListRights = new List<Rights>();
            buildInitiationRight();
            buildSourcingRight();
            buildTranformationTreeRight();
            buildLoadTableRight();
            buildReportRight();
            buildReconciliationRight();
            buildAdministrationRight();
            buildSettingRight();
            sortList(defaultListRights);
        }


        /// <summary>
        /// build inititiation Right
        /// </summary>
        /// <returns></returns>
        private void buildInitiationRight()
        {
            defaultListRights.Add(new Rights(FunctionalityRights.INITIATION_EDIT_MODEL));
            defaultListRights.Add(new Rights(FunctionalityRights.INITIATION_EDIT_PERIOD));
        }

        /// <summary>
        /// build Sourcing Right
        /// </summary>
        /// <returns></returns>
        private void buildSourcingRight()
        {
            // Posting Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.POSTING));
            //defaultListRights.Add(new Rights(FunctionalityRights.POSTING_GRID));
            //defaultListRights.Add(new Rights(FunctionalityRights.POSTING_AUTO_SOURCING));

            // Input Table Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.INPUT_TABLE));
            //defaultListRights.Add(new Rights(FunctionalityRights.INPUT_TABLE_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.INPUT_TABLE_LIST));

            // Grid Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.GRID));
            //defaultListRights.Add(new Rights(FunctionalityRights.GRID_INPUT));
            //defaultListRights.Add(new Rights(FunctionalityRights.GRID_AUTO_SOURCING));

            // Target Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.TARGET));
            //defaultListRights.Add(new Rights(FunctionalityRights.TARGET_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.TARGET_LIST));

            // DESIGN Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.DESIGN));
            //defaultListRights.Add(new Rights(FunctionalityRights.DESIGN_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.DESIGN_LIST));

            // UPLOAD FILE Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.MULTIPLE_FILE_UPLOAD));
        }


        /// <summary>
        /// build Tranformation Tree Right
        /// </summary>
        /// <returns></returns>
        private void buildTranformationTreeRight()
        {
            // Transformation Tree Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.TRANSFORMATION_TREE));
            //defaultListRights.Add(new Rights(FunctionalityRights.TRANSFORMATION_TREE_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.TRANSFORMATION_TREE_LIST));

            // Combined Transformation Tree Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.COMBINED_TRANSFORMATION_TREE));
            //defaultListRights.Add(new Rights(FunctionalityRights.COMBINED_TRANSFORMATION_TREE_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.COMBINED_TRANSFORMATION_TREE_LIST));
        }

        /// <summary>
        /// build load table Right
        /// </summary>
        /// <returns></returns>
        private void buildLoadTableRight()
        {
            // Load Table Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.LOAD_TABLE));
            defaultListRights.Add(new Rights(FunctionalityRights.CLEAR_TABLE));
            defaultListRights.Add(new Rights(FunctionalityRights.LOAD_LOG));
        }

        /// <summary>
        /// build Report Right
        /// </summary>
        /// <returns></returns>
        private void buildReportRight()
        {
            // Report Grid Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.REPORT_GRID));
            //defaultListRights.Add(new Rights(FunctionalityRights.REPORT_GRID_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.REPORT_GRID_LIST));

            // Report Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.REPORT));
            //defaultListRights.Add(new Rights(FunctionalityRights.REPORT_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.REPORT_LIST));

            // Calculated Measure Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.CALCULATED_MEASURE));
            //defaultListRights.Add(new Rights(FunctionalityRights.CALCULATED_MEASURE_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.CALCULATED_MEASURE_LIST));

            // Pivot Table Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.PIVOT_TABLE));
            //defaultListRights.Add(new Rights(FunctionalityRights.PIVOT_TABLE_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.PIVOT_TABLE_LIST));
        }


        /// <summary>
        /// build Reconciliation Right
        /// </summary>
        /// <returns></returns>
        private void buildReconciliationRight()
        {
            // Reconciliation Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.RECONCILIATION));
            //defaultListRights.Add(new Rights(FunctionalityRights.POSTING_RECONCILIATION));
            //defaultListRights.Add(new Rights(FunctionalityRights.RECONCILIATION_CONFIG));
        }

        /// <summary>
        /// build Administration Right
        /// </summary>
        /// <returns></returns>
        private void buildAdministrationRight()
        {
            // User Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.USER));
            //defaultListRights.Add(new Rights(FunctionalityRights.USER_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.USER_LIST));

            // Profil Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.PROFIL));
            //defaultListRights.Add(new Rights(FunctionalityRights.PROFIL_NEW));
            //defaultListRights.Add(new Rights(FunctionalityRights.PROFIL_LIST));

            // Role Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.MANAGE_ROLE));
        }

        /// <summary>
        /// build Setting Right
        /// </summary>
        /// <returns></returns>
        private void buildSettingRight()
        {
            // Reconciliation Functionality
            defaultListRights.Add(new Rights(FunctionalityRights.GROUP));
        }
        #endregion

        /// <summary>
        /// sort default right list
        /// </summary>
        /// <returns></returns>
        private void sortList(List<Rights> l)
        {
            Misp.Kernel.Util.ListUtil.BubbleSort(l);
        }

        /// <summary>
        /// toString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        /// <summary>
        /// compare
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Profil)) return 1;
            return this.name.CompareTo(((Profil)obj).name);
        }
    }
}
