using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Kernel.Util
{
    public class MessageDisplayer
    {

        /// <summary>
        /// Display a information message.
        /// </summary>
        /// <param name="title">Dialog's title</param>
        /// <param name="message">Information message to display</param>
        public static void DisplayInfo(string title, string message)
        {
            DisplayMessage(title, message, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Display a warning message.
        /// </summary>
        /// <param name="title">Dialog's title</param>
        /// <param name="message">Warning message to display</param>
        public static void DisplayWarning(string title, string message)
        {
            DisplayMessage(title, message, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Display an error message.
        /// </summary>
        /// <param name="title">Dialog's title</param>
        /// <param name="message">Error message to display</param>
        public static void DisplayError(string title, string message)
        {
            DisplayMessage(title, message, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message in MessageBox.
        /// The user can choise YES or NO.
        /// </summary>
        /// <param name="title">Dialog's title</param>
        /// <param name="message">Message to display</param>
        /// <returns>Yes or No</returns>
        public static MessageBoxResult DisplayYesNoQuestion(string title, string message)
        {
            return DisplayMessage(title, message, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        /// <summary>
        /// Display message in MessageBox.
        /// The user can choise YES, NO or CANCEL.
        /// </summary>
        /// <param name="title">Dialog's title</param>
        /// <param name="message">Message to display</param>
        /// <returns>Yes, No or Cancel</returns>
        public static MessageBoxResult DisplayYesNoCancelQuestion(string title, string message)
        {
            return DisplayMessage(title, message, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        /// <summary>
        /// Display a massage in MessageBox
        /// </summary>
        /// <param name="title">Dialog's title</param>
        /// <param name="message">Message to display</param>
        /// <param name="button">dialog's Buttons</param>
        /// <param name="image">Image in the dialog</param>
        /// <returns></returns>
        public static MessageBoxResult DisplayMessage(string title, string message, MessageBoxButton button, MessageBoxImage image)
        {
            return MessageBox.Show(message, title, button, image);
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.Dll")]
        static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        public static void ShowAutoClosingMessageBox(Window owner, string message, string caption,int timeout=1000)
        {
            var timer = new System.Timers.Timer(timeout) { AutoReset = false };
            timer.Elapsed += delegate
            {
                IntPtr hWnd = FindWindowByCaption(IntPtr.Zero, caption);
                if (hWnd.ToInt32() != 0) PostMessage(hWnd, WM_CLOSE, 0, 0);
            };
            timer.Enabled = true;
            MessageBox.Show(owner,message, caption);
        }
    }
}
