using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Moriset_Main_final.View
{
    public class Block : Tile
    {

        public Block(String name, String title)
        {
            Tile list_advisements = new Tile();
            this.Content = title;
            this.Name = name;
            this.Height = 60;
            this.Width = 142;
            this.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x9b, 0xbb));
            this.IsEnabled = false;
        }

    }
}
