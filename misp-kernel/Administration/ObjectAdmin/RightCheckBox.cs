using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.ObjectAdmin
{
    class RightCheckBox : CheckBox
    {

        #region Properties

        public Right Right { get; set; }

        public RightType RightType { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RightCheckBox()
        {
            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RightCheckBox(String label) : this()
        {
            this.Content = label;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RightCheckBox(String label, RightType RightType)
            : this(label)
        {
            this.RightType = RightType;
        }

        #endregion



    }
}