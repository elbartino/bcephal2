using Misp.Kernel.Util;
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
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.General
{
    /// <summary>
    /// Interaction logic for PropertiesDialog.xaml
    /// </summary>
    public partial class PropertiesDialog : Window
    {
        public PropertiesDialog()
        {
            InitializeComponent();
            setFolderName(UserPreferencesUtil.GetMultipleFileUploadRepository());
            
        }

        public void display()
        {
            this.OkButton.IsEnabled = false;
            this.Show();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string current = UserPreferencesUtil.GetMultipleFileUploadRepository();
            if (!UserPreferencesUtil.SetMultipleFileUploadRepository(this.folderField.Text))
            {
                MessageDisplayer.DisplayWarning("Upload Multiple File Repository", "Attempt to save an empty repository!");
                setFolderName(UserPreferencesUtil.GetMultipleFileUploadRepository());
            }
            else 
            {
                    this.Close();
            }
        }

       

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BrowserButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            var selectedPath = folderDialog.SelectedPath;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                setFolderName(selectedPath);
                this.OkButton.IsEnabled = true;
            }
        }

        public void setFolderName(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                this.folderField.Text = path;
        }

    }
}
