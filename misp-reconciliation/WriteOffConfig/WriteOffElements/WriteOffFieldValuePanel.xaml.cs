using Misp.Kernel.Domain;
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

namespace Misp.Reconciliation.WriteOffConfig.WriteOffElements
{
    /// <summary>
    /// Interaction logic for WriteOffFieldValuePanel.xaml
    /// </summary>
    public partial class WriteOffFieldValuePanel : Grid
    {
        public PersistentListChangeHandler<WriteOffField> fieldListChangeHandler { get; set; }

        public WriteOffValueItem ActiveItem { get; set; }

        public WriteOffFieldValuePanel()
        {
            InitializeComponent();
            FieldValuePanel.Children.Clear();
           // item2.showRowLabel(false);
        }


        public void showRowLabel(bool show = false)
        {
            this.showLabel = show;
        }
        public bool showLabel = true;
        public void display()
        {
            for (int i = 0; i < 10; i++) 
            {
                WriteOffValueItem item = new WriteOffValueItem();
                if (!showLabel) item.showRowLabel(false);
                else
                {
                    if (i > 0) item.showRowLabel(false);
                }
                this.FieldValuePanel.Children.Add(item);
            }
        }

        public void display(bool isLabelPresent = true)
        {
            for (int i = 0; i < 10; i++)
            {
                WriteOffValueItem item = new WriteOffValueItem();
                if (!isLabelPresent) item.showRowLabel(false);
                else
                {
                    if (i > 0) item.showRowLabel(false);
                }
                this.FieldValuePanel.Children.Add(item);
            }
        }
    }
}
