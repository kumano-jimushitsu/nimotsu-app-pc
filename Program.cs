using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;

namespace RegisterParcelsFromPC
{
    static class Program
    {
        enum event_type
        {
            �o�^=1,
            ���=2,
            �폜=3,
            ���������=10,
            ���܂莖�����p���[�h�J�n=11,
            ���܂莖�����p���[�h�I��=12,
        }


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
