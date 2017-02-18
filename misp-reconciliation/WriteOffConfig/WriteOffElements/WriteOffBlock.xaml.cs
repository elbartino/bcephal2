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
    /// Interaction logic for WriteOffBlock.xaml
    /// </summary>
    public partial class WriteOffBlock : StackPanel
    {
        public Kernel.Domain.WriteOffConfiguration WriteOffConfiguration { get; set; }

        public WriteOffBlock()
        {
            InitializeComponent();
            //WriteOffConfiguration = new Kernel.Domain.WriteOffConfiguration();
            //Kernel.Domain.WriteOffField field = null;
            //for (int i = 0; i <= 7; i++) 
            //{
            //    if (i % 2 == 0)
            //    {
            //        field = new Kernel.Domain.WriteOffField();
            //        field.setPeriodName(new Kernel.Domain.PeriodName("Date" + (i+1)));
            //        field.mandatory = true;
            //    }
            //    else
            //    {
            //        field = new Kernel.Domain.WriteOffField();
            //        Kernel.Domain.Attribute attrib = new Kernel.Domain.Attribute();
            //        attrib.name = "Attrib" + (i+1);
            //        field.setAttribute(attrib);
            //    }
            //    WriteOffConfiguration.AddFieldValue(field);
            //}
        }

        public void display() 
        {
            this.Children.Clear();
            this.WriteOffConfiguration = null;
            bool displayLines = this.WriteOffConfiguration != null &&
                this.WriteOffConfiguration.fieldListChangeHandler != null && 
                this.WriteOffConfiguration.fieldListChangeHandler.Items.Count > 0;

            if (!displayLines) 
            {
                WriteOffLine line = new WriteOffLine();
                line.display();
                this.Children.Add(line);
                return;
            }

            foreach (Kernel.Domain.WriteOffField fieldValue in this.WriteOffConfiguration.fieldListChangeHandler.Items)
            {
                WriteOffLine line = new WriteOffLine();
                line.writeOffField = fieldValue;
                line.display();
                this.Children.Add(line);
            }
        }

    }
}
