using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misp_view.Views
{
    public static class ListContacts
    {
        
       public static BindingList<string> listC ;



        static ListContacts()
        {
            listC = new BindingList<string>();
        }

        public static void Record(string value)
        {
            listC.Add(value);
        }
        public static void Remove(string value)
        {
            listC.Remove(value);
        }

        public static void Display()
        {
            foreach (var value in listC)
            {
                //Console.WriteLine(value);
                System.Windows.Forms.MessageBox.Show(value);
            }
        }
    }
}
