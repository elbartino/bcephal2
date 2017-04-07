using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Moriset_Main_final.View
{
    public class Block : Tile
    {
        Tile block_layout = new Tile();

        public Block(String name, String title)
        {
            Tile block_layout = new Tile();
            this.Content = title;
            this.Name = name;
            this.Height = 60;
            this.Width = 142;
            this.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x9b, 0xbb));
        }
        //public void Redirect(bool direct, string path)
        //{
        //    if (direct)
        //    {
        //        Type type = Type.GetType(path);
        //        type.Show();
        //    }
        //    else if (direct == false) 
        //    {
        //        Type type = Type.GetType(path);
        //    }
 
        //}

    }
}
