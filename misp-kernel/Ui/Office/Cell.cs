using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
    [Serializable]
    /// <summary>
    /// Cette classe représente une cellule dans une feuille d'un tableur.
    /// Une cellule est identifiée par le numéro de sa ligne et le numéro de sa colonne
    /// </summary>
    public class Cell
    {

        #region Properties
        
        /// <summary>
        /// Assigne ou retourne le numéro de la ligne
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Assigne ou retourne le numéro de la colonne
        /// </summary>
        public int Column { get; set; }
        
        #endregion


        #region Constructors

        /// <summary>
        /// Construit une instance de la classe Cell.
        /// </summary>
        public Cell() { }

        /// <summary>
        /// Construit une instance de la classe Cell.
        /// </summary>
        /// <param name="row">Le numéro de la ligne</param>
        /// <param name="column">Le numéro de la colonne</param>
        public Cell(int row, int column) 
        {
            this.Row = row;
            this.Column = column;
        }

        #endregion


        #region Methos
                
        /// <summary>
        /// Retoune le nom de la cellule.
        /// </summary>
        public string Name 
        {
            get {
		        return Util.RangeUtil.GetCellName(Row, Column);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Le nom de la cellule</returns>
        public override string ToString() {
		    return Name;
	    }

        #endregion

    }
}
