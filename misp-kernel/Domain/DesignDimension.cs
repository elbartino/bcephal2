using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class DesignDimension : Persistent
    {

        public DesignDimension()
        {
            lineListChangeHandler = new PersistentListChangeHandler<DesignDimensionLine>();
        }


        [ScriptIgnore]
        public bool IsModify { get; set; }

        public PersistentListChangeHandler<DesignDimensionLine> lineListChangeHandler { get; set; }

        public bool ContainsMeasure()
        {
            foreach (DesignDimensionLine line in lineListChangeHandler.Items)
            {
                if (line.ContainsMeasure()) return true;
            }
            return false;
        }

        public bool ContainsPeriod()
        {
            foreach (DesignDimensionLine line in lineListChangeHandler.Items)
            {
                if (line.ContainsPeriod()) return true;
            }
            return false;
        }

        /// <summary>
        /// Rajoute un Line
        /// </summary>
        /// <param name="cell"></param>
        public void AddLine(DesignDimensionLine line)
        {
            line.position = lineListChangeHandler.Items.Count+1;
            lineListChangeHandler.AddNew(line);
            OnPropertyChanged("lineListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un LineItem
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateLine(DesignDimensionLine line)
        {
            lineListChangeHandler.AddUpdated(line);
            OnPropertyChanged("itemListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un line
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveLine(DesignDimensionLine line)
        {
            line.position = -1;
            lineListChangeHandler.AddDeleted(line);
            OnPropertyChanged("lineListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un line
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetLine(DesignDimensionLine line)
        {
            line.position = -1;
            lineListChangeHandler.forget(line);
            OnPropertyChanged("lineListChangeHandler.Items");
        }

        public int GetLineCount()
        {
            return lineListChangeHandler.Items.Count;
        }

        /// <summary>
        /// Retourne le nombre de lineitems présents dans le DesignDimension
        /// </summary>
        /// <returns></returns>
        public int GetLineItemCount()
        {
            int nbreLineItem = 0;

            foreach (DesignDimensionLine designDimensionLine in lineListChangeHandler.Items)
            {
                foreach (LineItem lineitem in designDimensionLine.itemListChangeHandler.Items) 
                {
                    nbreLineItem++;
                }
            }
            return nbreLineItem;
        }
        
    }
}
