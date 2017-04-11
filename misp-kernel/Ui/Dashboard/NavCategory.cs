using DevExpress.Xpf.Navigation;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Misp.Kernel.Ui.Dashboard
{
    public class NavCategory : TileNavCategory
    {
        
        #region Properties

        public ChangeItemEventHandler Selection { get; set; }
        public String FunctionalityCode { get; set; }

        public NavBlock Block { get; set; }

        #endregion


        #region Constructors

        public NavCategory(Object content = null, String functionalityCode = null)
        {
            this.Content = content;
            this.FunctionalityCode = functionalityCode;
            this.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x9b, 0xbb));
            this.Foreground = Brushes.White;
            InitHandlers();
        }

        #endregion


        #region Operations

        public void Dispose()
        {
            RemoveHandlers();
        }

        #endregion


        #region Handlers

        protected virtual void InitHandlers()
        {
            this.Click += OnClick;
        }

        protected virtual void RemoveHandlers()
        {
            this.Click -= OnClick;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (Selection != null) Selection(this);
        }

        #endregion             
        
    }
}
