using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Misp.Kernel.Ui.Office.EDraw
{
    public class EdrawSpreadsheetPanel : UserControl
    {

        #region Property

        public AxEDOfficeLib.AxEDOffice axEDOffice1 { get; set; }

        /// <summary>
        /// Renvoi l'url du document courant
        /// </summary>
        public string documentUrl { get; set; }

        #endregion


        public EdrawSpreadsheetPanel()
        {
            axEDOffice1 = new AxEDOfficeLib.AxEDOffice();
            this.AddChild(axEDOffice1);
        }
        /// <summary>
        /// Ouvre un fichier à partir de la boite de dialogue
        /// </summary>
        public void Open() 
        {
            axEDOffice1.OpenFileDialog();
            documentUrl = axEDOffice1.GetDocumentFullName();
        }
        /// <summary>
        /// Ouvre un fichier donné
        /// </summary>
        /// <param name="filePath">Le fichier à ouvrir</param>
        public void Open(string filePath)
        {
            axEDOffice1.Open(filePath);
            documentUrl = filePath;   
        }
             public void Protect_Click_OnClick()
        {
             axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableNew, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableOpen, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableOfficeButton, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableSave, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableSaveAs, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableClose, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisablePrint, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisablePrintQuick, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisablePrintPreview, false);
              axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableSaveAsMenu, false);
               axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableCopyButton, true);
               axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableCutButton, true);
               axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableUpgradeDocument, true);
               axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisablePermissionRestrictMenu, true);
               axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisablePrepareMenu, true);
               axEDOffice1.DisableFileCommand(EDOfficeLib.WdUIType.wdUIDisableServerTasksMenu, true);
            
                 axEDOffice1.Toolbars = false;
              
            axEDOffice1.OpenFileDialog();

            if (axEDOffice1.GetCurrentProgID() == "Word.Application")
            {
                axEDOffice1.Toolbars = false;
                axEDOffice1.WordDisableCopyHotKey(true);
                axEDOffice1.WordDisableSaveHotKey(true);
                axEDOffice1.WordDisablePrintHotKey(true);
                axEDOffice1.DisableViewRightClickMenu(true);
                axEDOffice1.WordDisableDragAndDrop(true);
                axEDOffice1.ProtectDoc(2);
            }
            else if (axEDOffice1.GetCurrentProgID() == "Excel.Application")
            {
                axEDOffice1.Toolbars = false;
                axEDOffice1.WordDisableCopyHotKey(true);
                axEDOffice1.WordDisableSaveHotKey(true);
                axEDOffice1.WordDisablePrintHotKey(true);
                axEDOffice1.DisableViewRightClickMenu(true);
                axEDOffice1.ProtectDoc(1);
            }
            else if (axEDOffice1.GetCurrentProgID() == "PowerPoint.Application")
            {
                axEDOffice1.Toolbars = false;
                axEDOffice1.SlideShowPlay(true);
            }
        }

        /// <summary>
        /// Save as par défaut
        /// </summary>
             public void SaveFileDialog()
             {
                 axEDOffice1.SaveFileDialog();
                
             }
        /// <summary>
        /// Save
        /// </summary>
             public void save()
             {
                 axEDOffice1.Save();
             }
    
        /// <summary>
        /// Save as d'un fichier spécifique
        /// </summary>
        /// <param name="filePath">Le fichier à sauvegarder</param>
             public void saveAs(string filePath) 
             {
                 axEDOffice1.SaveAs(filePath);
             }
             public void Print()
             {
                 axEDOffice1.PrintPreview();
             }
            /// <summary>
            /// Fermeture du fichier courant
            /// </summary>
             public void Close()
             {
                 axEDOffice1.ExitOfficeApp();
             }

    }
}
