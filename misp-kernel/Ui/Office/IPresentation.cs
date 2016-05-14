using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
    public interface IPresentation
    {
            event ChangeEventHandler Changed;
            event EditEventHandler Edited;
            event SelectionChangedEventHandler SelectionChanged;
            event SheetActivateEventHandler SheetActivated;
            event SheetAddedEventHandler SheetAdded;
            event SheetDeletedEventHandler SheetDeleted;
        
            /// <summary>
            /// Ouvre le dialogue permettant de choisir le document à importer.
            /// </summary>
            /// <returns>
            /// OperationState.CONTINUE si l'opération a réussi
            /// OperationState.STOP sinon
            /// </returns>
            OperationState Import();


            /// <summary>
            /// Permet de créer un fichier Excel.
            /// </summary>
            /// <returns>
            /// true si l'opération a réussi
            /// false sinon
            /// </returns>
            String CreateNewPowerPointFile();

            /// <summary>
            /// Ouvre le fichier dont l'url est passé en paramètre.
            /// </summary>
            /// <param name="filePath">L'url du fichier à ouvrir</param>
            /// <param name="progID">Le type de fichier à ouvrir</param>
            /// <returns>
            /// OperationState.CONTINUE si l'opération a réussi
            /// OperationState.STOP sinon
            /// </returns>
            OperationState Open(String filePath, String progID);
      

            /// <summary>
            /// Ferme le fichier ouvert.
            /// </summary>
            /// <returns>
            /// OperationState.CONTINUE si l'opération a réussi
            /// OperationState.STOP sinon
            /// </returns>
            OperationState Close();

            /// <summary>
            /// Sauve le fichier ouvert sous un autre nom.
            /// </summary>
            /// <param name="filePath">L'url du nouveau fichier</param>
            /// <param name="overwrite"></param>
            /// <returns>
            /// OperationState.CONTINUE si l'opération a réussi
            /// OperationState.STOP sinon
            /// </returns>
            OperationState SaveAs(String filePath, bool overwrite);

            /// <summary>
            /// Sauve le fichier ouvert sous un autre nom.
            /// </summary>
            /// <param name="filePath">L'url du nouveau fichier</param>
            /// <param name="overwrite"></param>
            /// <returns>
            /// OperationState.CONTINUE si l'opération a réussi
            /// OperationState.STOP sinon
            /// </returns>
            OperationState Export(String filePath);

            /// <summary>
            /// Retourne le nom du fichier ouvert
            /// </summary>
            /// <returns></returns>
            String GetFilePath();
                

            /// <summary>
            /// Rend visible/invisible la barre d'outils Excel
            /// </summary>
            /// <param name="value">True=>Visible; False=>Invisible</param>
            void DisableToolBar(bool value);

            /// <summary>
            /// Rend visible/invisible la barre de titre excel
            /// </summary>
            /// <param name="value">True=>Visible; False=>Invisible</param>
            void DisableTitleBar(bool value);
        
    }
}
