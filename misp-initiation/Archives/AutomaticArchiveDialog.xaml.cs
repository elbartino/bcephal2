using Misp.Kernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Misp.Initiation.Archives
{
    /// <summary>
    /// Interaction logic for AutomaticArchiveDialog.xaml
    /// </summary>
    public partial class AutomaticArchiveDialog : Window
    {
        public FileService fileService { get; set; }
        string archiveRepository { get; set; }

        public AutomaticArchiveDialog()
        {
            InitializeComponent();
            this.AtCloseCombox.Visibility = System.Windows.Visibility.Hidden;
            this.AtStartCombox.Visibility = System.Windows.Visibility.Hidden;
            this.minutesTextbox.Visibility = System.Windows.Visibility.Hidden;
            this.whenCombox.ItemsSource = new String[] {
                Kernel.Domain.ArchiveConfiguration.NO_AUTOMATIC_ARCHIVE,
                /*Kernel.Domain.ArchiveConfiguration.AT_START,*/
                Kernel.Domain.ArchiveConfiguration.MINUTES_INTERVAL,
                Kernel.Domain.ArchiveConfiguration.AT_CLOSING}; 
            this.whenCombox.SelectionChanged += onChoosePeriodicity;
            this.buttonCancel.Click += onCancelConfig;
            this.buttonOk.Click += onValidateConfig;
            this.browser.Click += OnChooseArchiveRepository;
        }

        private void OnChooseArchiveRepository(object sender, RoutedEventArgs e)
        {
            setArchiveRepository();
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

        private void onChoosePeriodicity(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = whenCombox.SelectedItem != null && !String.IsNullOrWhiteSpace(whenCombox.SelectedItem.ToString()) ?
                whenCombox.SelectedItem.ToString() : null;
            if (selectedItem == null) return;
            if (selectedItem == Kernel.Domain.ArchiveConfiguration.MINUTES_INTERVAL)
            {
                this.AtStartCombox.Visibility = System.Windows.Visibility.Visible;
                this.minutesTextbox.Visibility = System.Windows.Visibility.Visible;
                this.maxArchiveLabel.Visibility = System.Windows.Visibility.Visible;
                this.maxArchiveTextBox.Visibility = System.Windows.Visibility.Visible;
                this.archiveRepo.IsEnabled = true;
                this.browser.IsEnabled = true;
            }
            else if (selectedItem == Kernel.Domain.ArchiveConfiguration.NO_AUTOMATIC_ARCHIVE)
            {
                this.AtStartCombox.Visibility = System.Windows.Visibility.Hidden;
                this.minutesTextbox.Visibility = System.Windows.Visibility.Hidden;
                this.maxArchiveLabel.Visibility = System.Windows.Visibility.Hidden;
                this.maxArchiveTextBox.Visibility = System.Windows.Visibility.Hidden;
                this.archiveRepo.IsEnabled = false;
                this.browser.IsEnabled = false;
            }
            else
            {
                this.AtStartCombox.Visibility = System.Windows.Visibility.Hidden;
                this.minutesTextbox.Visibility = System.Windows.Visibility.Hidden;
                this.archiveRepo.IsEnabled = true;
                this.browser.IsEnabled = true;
                this.maxArchiveLabel.Visibility = System.Windows.Visibility.Visible;
                this.maxArchiveTextBox.Visibility = System.Windows.Visibility.Visible;
            }

            
        }

        private void onCancelConfig(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void onValidateConfig(object sender, RoutedEventArgs e)
        {
            if (this.fileService == null) return;

            String path = this.archiveRepo.Text + System.IO.Path.DirectorySeparatorChar + this.archiveRepo.Text;
            if (System.IO.File.Exists(path))
            {
                MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Simple Backup", "There is another file with the same name \n" +
                "Do you want to erase it ?");
                if (result != MessageBoxResult.Yes) return;
            }

            Kernel.Domain.ArchiveConfiguration archiveConfig = new Kernel.Domain.ArchiveConfiguration();
            archiveConfig.atClose = this.AtCloseCombox.IsChecked.Value;
            archiveConfig.atStart = this.AtStartCombox.IsChecked.Value;
            archiveConfig.type = this.whenCombox.SelectedItem.ToString();
            archiveConfig.repository = this.archiveRepo.Text;
            
            try
            {
                if (archiveConfig.isMinutesIntervalArchive())
                {
                    string minutes = this.minutesTextbox.Text.Trim().Replace(".", ",");

                    if (double.TryParse(minutes, out archiveConfig.frequency) && archiveConfig.frequency > 0) { }
                    else
                    {
                        Kernel.Util.MessageDisplayer.DisplayWarning("Automatic Archive ", "The Minut Interval is not valid !");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Kernel.Util.MessageDisplayer.DisplayWarning("Automatic Archive ", "The Minut Interval is not valid !");
                return;
            }
  
            try
            {
               
                if (int.TryParse(this.maxArchiveTextBox.Text, out archiveConfig.maxArchiveCount)) { }
                else
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Automatic Archive ", "The max archive number is not valid !");
                    return;
                }
            }
            catch (Exception ex)
            {
                Kernel.Util.MessageDisplayer.DisplayWarning("Automatic Archive ", "The max archive number is not valid !");
                return;
            }

            if (this.fileService.saveArchiveConfiguration(archiveConfig))
                this.Close();
        }


        public void Display(Kernel.Domain.ArchiveConfiguration archiveCongiguration)
        {
            if (archiveCongiguration == null) return;
            whenCombox.SelectedItem = archiveCongiguration.type;
            archiveRepo.Text = archiveCongiguration.repository;
            minutesTextbox.Text = archiveCongiguration.frequency.ToString();
            AtStartCombox.IsChecked = archiveCongiguration.atStart;
            maxArchiveTextBox.Text = archiveCongiguration.maxArchiveCount.ToString();
        }
    }
}
