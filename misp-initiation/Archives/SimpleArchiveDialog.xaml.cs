using Misp.Kernel.Application;
using Misp.Kernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Misp.Initiation.Archives
{
    /// <summary>
    /// Interaction logic for AutomaticArchiveDialog.xaml
    /// </summary>
    public partial class SimpleArchiveDialog : Window
    {
        public FileService fileService { get; set; }
              
        public SimpleArchiveDialog()
        {
            InitializeComponent();
            this.archiveRepo.Visibility = Visibility.Collapsed;
            this.browser.Visibility = Visibility.Collapsed;
            this.archiveLabel.Visibility = Visibility.Collapsed;
            this.buttonOk.Click += onValidateConfig;
            this.buttonCancel.Click += onCancelConfig;
            this.browser.Click += onBrowserClick;
            
        }

        private void onBrowserClick(object sender, RoutedEventArgs e)
        {
            string defaultRepository = archiveRepo.Text;
          //  if (string.IsNullOrEmpty(defaultRepository))
                setArchiveRepository();
            //else
              //  setArchiveRepository(defaultRepository);
        }

        private void onCancelConfig(object sender, RoutedEventArgs e)
        {
            if (this.fileService == null) return;
            this.Close();            
        }


        private void setArchiveRepository()
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = true;
            folderDialog.Description = "Archives ";
            
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();

            var selectedPath = folderDialog.SelectedPath;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string valueText = selectedPath != null ? selectedPath : "";
                this.archiveRepo.Text = valueText;
                Kernel.Util.UserPreferencesUtil.SetArchiveRepository(valueText);
            }
        }

        private void setArchiveRepository(String path)
        {
            String selectedPath;
            if (ShowFBD(path, "Please Select a folder", out selectedPath))
            {
                MessageBox.Show(selectedPath);
            }
        }

        public bool ShowFBD(String rootFolder, String title, out String selectedPath)
        {
            var shellType = Type.GetTypeFromProgID("Shell.Application");
            var shell = Activator.CreateInstance(shellType);
            var result = shellType.InvokeMember("BrowseForFolder", BindingFlags.InvokeMethod, null, shell, new object[] { 0, title, 0, rootFolder });
            if (result == null)
            {
                selectedPath = "";
                return false;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while (result != null)
                {
                    var folderName = result.GetType().InvokeMember("Title", BindingFlags.GetProperty, null, result, null).ToString();
                    sb.Insert(0, String.Format("{0}\\", folderName));
                    result = result.GetType().InvokeMember("ParentFolder", BindingFlags.GetProperty, null, result, null);
                }
                selectedPath = sb.ToString();

                selectedPath = Regex.Replace(selectedPath, @"Desktop\\Computer\\.*\(\w:\)\\", rootFolder.Substring(0, 3));
                return true;
            }
        }

        private void onValidateConfig(object sender, RoutedEventArgs e)
        {
            if (this.fileService == null) return;
            
            String path =  this.archiveRepo.Text + System.IO.Path.DirectorySeparatorChar + this.archiveName.Text;
            if (System.IO.File.Exists(path))
            {
                MessageBoxResult result =  Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Simple Backup", "There is another file with the same name \n"+
                "Do you want to erase it ?");
                if (result != MessageBoxResult.Yes) return; 
            }
            
            Kernel.Domain.SimpleArchive simpleArchive = new Kernel.Domain.SimpleArchive();
            simpleArchive.name = this.archiveName.Text;
            simpleArchive.repository = null; // this.archiveRepo.Text;
            simpleArchive.comments = this.archiveComments.Text;
            Kernel.Util.UserPreferencesUtil.SetArchiveRepository(this.archiveRepo.Text);
            if (this.fileService.saveSimpleArchive(simpleArchive))
                this.Close();
        }
        
        public void Display(Kernel.Domain.SimpleArchive simpleArchive)
        {
            if (simpleArchive == null) return;
            archiveName.Text = simpleArchive.name;
            archiveRepo.Text = simpleArchive.repository;
            archiveComments.Text = string.IsNullOrEmpty(simpleArchive.comments) ? simpleArchive.comments : "";
        }

    }
}
