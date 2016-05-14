using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;

namespace Misp.Kernel.Ui.Office
{
    public interface ISpreadsheet
    {
        event ChangeEventHandler Changed;
        event EditEventHandler Edited;
        event SelectionChangedEventHandler SelectionChanged;
        event SheetActivateEventHandler SheetActivated;
        event SheetAddedEventHandler SheetAdded;
        event SheetDeletedEventHandler SheetDeleted;

        /// <summary>
        /// Crée un menuitems dans le menu contextuel d'Excel
        /// </summary>
        /// <param name="Header">Le libelle du menuItem</param>
        /// <returns>l'objet crée</returns>
        object AddExcelMenu(string Header);

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
        String CreateNewExcelFile();
        
        /// <summary>
        /// Ouvre le fichier dont l'url est passé en paramètre.
        /// </summary>
        /// <param name="filePath">L'url du fichier à ouvrir</param>
        /// <param name="progID">Le type de fichier à ouvrir</param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        OperationState Open(String filePath,String progID);

        
        
        /// <summary>
        /// Retire un menuitem du menu contextuel d'Excel
        /// </summary>
        /// <param name="Header">le menuItem à retirer</param>
        /// <returns>l'objet retiré</returns>
        object RemoveExcelMenu(string Header);
        
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
        OperationState SaveAs(String filePath,bool overwrite);

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
        /// Les cells selectionnées dans le sheet actif
        /// </summary>
        Ui.Office.Range GetSelectedRange();

        /// <summary>
        /// Affecte une valeur dans une cellule de la feuille excel courante
        /// </summary>
        /// <param name="row">la ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        /// <param name="value">la value à mettre  dans la cellule</param>
        void SetValueAt(int row, int colunm, string sheetName, object value);

        /// <summary>
        /// Retourne la valeur d'une cellule
        /// </summary>
        /// <param name="row">la ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        /// <returns>La valeur de la cellule sélectionnée</returns>
        object getValueAt(int row, int colunm, string sheetName);

        /// <summary>
        /// Vide une cellule
        /// </summary>
        /// <param name="row">ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        void ClearValueAt(int row, int colunm);
        
        /// <summary>
        /// Retourne la cellule active
        /// </summary>
        /// <returns>La cellule active</returns>
        Cell getActiveCell();

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
