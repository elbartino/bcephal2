using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moriset_Main_final.View
{
    public static class ListScreens
    {
        public static BindingList<string> listS;


        static ListScreens()
        {
            listS = new BindingList<string>();
        }

        public static void Record(string win)
        {
            listS.Add(win);
        }
        public static void Remove(string win)
        {
            listS.Remove(win);
        }

        public static void RemoveAll()
        {
            for (int i = listS.Count - 1; i >=0 ; i--)
            {
                listS.Remove(listS[i]);
            }
        }
        public static void Display()
        {
            foreach (var win in listS)
            {
                //Console.WriteLine(value);
                System.Windows.Forms.MessageBox.Show(win);
            }
        }
    }
}
