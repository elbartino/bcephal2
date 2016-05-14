using Misp.Kernel.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    /// <summary>
    /// this class save and return informations about user settings 
    /// </summary>
    public class UserPreferencesUtil
    {
        public static string FILE_DIRS = "FileDirectories";
        public static int MAX_FILE = 10;
        public static string MODEL_DIRS = "ModelDirectories";
        public static List<Domain.Model> listeModels = new List<Domain.Model>(0);
        
        /// <summary>
        /// return all recent files with a max range of ten files
        /// </summary>
        /// <returns></returns>
        public static StringCollection GetRecentFiles()
        {
            return Properties.Settings.Default.FileDirectories;
        }

        /// <summary>
        /// return default multiple upload path
        /// </summary>
        /// <returns></returns>
        /// 

        public static String GetFileOpeningRepository()
        {
            return Properties.Settings.Default.FileOpeningRepository;
        }

        public static String GetMultipleFileUploadRepository()
        {
            return Properties.Settings.Default.MultipleFileUploadRepository;
        }

        public static string GetPowerPowerPointSavingRepository()
        {
            return Properties.Settings.Default.PowerPointSavingRepository;
        }

        public static string GetArchiveRepository() 
        {
            return Properties.Settings.Default.ArchiveRepository;
        }

        /// <summary>
        /// set multiple file upload path
        /// </summary>
        /// <returns></returns>
        public static bool SetFileOpeningRepository(string nPath)
        {
            bool validPath = false;
            if (!string.IsNullOrWhiteSpace(nPath))
            {
                Properties.Settings.Default.FileOpeningRepository = nPath;
                Properties.Settings.Default.Save();
                validPath = true;
            }
            return validPath;
        }
        
        public static bool SetMultipleFileUploadRepository(string newPath)
        {
            bool validPath = false;
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                Properties.Settings.Default.MultipleFileUploadRepository = newPath;
                Properties.Settings.Default.Save();
                validPath = true;
            }
            return validPath;
        }

        public static bool SetPowerPointSavingFolder(string path) 
        {
            bool validPath = false;
            if (!string.IsNullOrWhiteSpace(path))
            {
                Properties.Settings.Default.PowerPointSavingRepository = path;
                Properties.Settings.Default.Save();
                validPath = true;
            }
            return validPath;
        }

        public static bool SetArchiveRepository(string path)
        {
            bool validPath = false;
            if (!string.IsNullOrWhiteSpace(path))
            {
                Properties.Settings.Default.ArchiveRepository = path;
                Properties.Settings.Default.Save();
                validPath = true;
            }
            return validPath;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RemoveAllRecentFiles()
        {
            StringCollection files = GetRecentFiles();
            if (files != null && files.Count > 0)
            {
                files.Clear();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// store information about the  recent  opened file 
        /// </summary>
        /// <param name="fileDirector">file Directory</param>
        public static void AddRecentFile(string fileDirector)
        {
            StringCollection files = GetRecentFiles();
            if (files.Contains(fileDirector))
            {
                files.Remove(fileDirector);                
            }
            files.Insert(0, fileDirector);
            if (files.Count > MAX_FILE) files.RemoveAt(files.Count - 1);
            Properties.Settings.Default.Save();

        }

        internal static void RenameLastOpened(string filePath)
        {
            StringCollection files = GetRecentFiles();
            files.RemoveAt(0);
            files.Insert(0, filePath);
            Properties.Settings.Default.Save();
        }



    }
}
