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
    /// Interaction logic for WriteOffBlock.xaml
    /// </summary>
    public partial class WriteOffBlock : StackPanel
    {
        public Kernel.Domain.WriteOffConfiguration WriteOffConfiguration { get; set; }

        public WriteOffBlock()
        {
            InitializeComponent();
        }

        public void display() 
        {
            this.Children.Clear();
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

        public List<WriteOffField> Fill()
        {
            List<WriteOffField> fields = new List<WriteOffField>(0);
            foreach (UIElement elt in this.Children)
            {
                if (elt is WriteOffLine){
                    WriteOffField field = ((WriteOffLine)elt).Fill();
                    if (field != null) fields.Add(field);
                }
            }
            return fields;
        }

        public bool Validate()
        {
            foreach (UIElement elt in this.Children)
            {
                if (elt is WriteOffLine && !((WriteOffLine)elt).Validate()) return false;
            }
            return true;
        }

    }
}
