using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class FileUtil
    {
        public static bool copy(String sourcePath, string destPath)
        {
            try
            {
                if (isDirectory(sourcePath)) DirectoryCopy(sourcePath, destPath, true);
                else CopyFile(sourcePath, destPath, true);
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static bool isDirectory(String path) {
            return !isFile(path);
        }

        public static bool isFile(String path) {
            FileAttributes attr = File.GetAttributes(path);
            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) return false;
            else return true;
        }

        public static void CopyFile(String sourceFilePath, string destFilePath, bool overwrite) 
        {
            System.IO.File.Copy(sourceFilePath, destFilePath, overwrite);
        }

        public static bool isValidFileName(string fileName)
        {
            string strRegex = "[" + new string(Path.GetInvalidFileNameChars()) + "]+";
            Regex re = new Regex(strRegex, RegexOptions.IgnoreCase);
            if (!re.IsMatch(fileName))
                return (true);
            else
                return (false);
        }
        static System.IO.FileStream fileStream = null;
        public static void buildTimeMeasurementFile(string filePathAndName = null) 
        {
            closeTimeMeasurementFile();
            if (filePathAndName == null) filePathAndName = "D:\\BcephalDuration.txt";
            try
            {
                Path.GetFullPath(filePathAndName);                
            }
            catch (Exception e) 
            {
                filePathAndName = "D:\\BcephalDuration.txt";
            }
            
            fileStream = new System.IO.FileStream(filePathAndName,System.IO.FileMode.Append);
            var streamwriter = new System.IO.StreamWriter(fileStream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            //Console.SetError(streamwriter);
            
        }

        public static void closeTimeMeasurementFile()
        {
            try
            {
                fileStream.Close();
            }catch(Exception){
            }
        }
    }
}
