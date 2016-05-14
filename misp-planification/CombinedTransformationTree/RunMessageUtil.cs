using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.CombinedTransformationTree
{
    public class RunMessageUtil
    {
        #region Message Utils
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearOption"></param>
        /// <returns></returns>
        public static string getRunButtonText(bool isClearOption = false)
        {
            return isClearOption ? "Clear" : "Run";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearOption"></param>
        /// <returns></returns>
        public static string getMaskStartText(bool isClearOption = false)
        {
            return isClearOption ? "Clear..." : "Running...";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearOption"></param>
        /// <returns></returns>
        public static string getMaskEndedText(bool isClearOption = false)
        {
            return isClearOption ? "Clear ended" : "Run ended";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearOption"></param>
        /// <returns></returns>
        public static string getDialogTitle(bool isClearOption = false)
        {
            return isClearOption ? "Clear Transformation trees - Select items" : "Run Transformation trees - Select items";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearOption"></param>
        /// <returns></returns>
        public static string getNoItemsTitle(bool isClearOption = false)
        {
            return isClearOption ? "Clear All TransformationTrees " : "Run All TransformationTrees";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearOption"></param>
        /// <returns></returns>
        public static string getNoItemsMessage(bool isClearOption = false)
        {
            return isClearOption ? "No TransformationTree has been runned" : "No TransformationTree created ";
        }

        public static string getRunCancelToolTip(bool isClearOption = false)
        {
            return isClearOption ? "Cancel clear" : "Cancel run";
        }
        #endregion
    }
}
