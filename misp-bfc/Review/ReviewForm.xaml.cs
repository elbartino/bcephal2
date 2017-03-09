using DevExpress.Xpf.Core;
using Misp.Bfc.Model;
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

namespace Misp.Bfc.Review
{
    /// <summary>
    /// Interaction logic for ReviewForm.xaml
    /// </summary>
    public partial class ReviewForm : UserControl
    {
        public ReviewForm()
        {            
            InitializeComponent();
            ThemeManager.SetThemeName(this, "None");
        }

        public void Display(PrefundingAccountData data)
        {
            this.PrefundingAccountForm.Display(data);
        }
    }
}
