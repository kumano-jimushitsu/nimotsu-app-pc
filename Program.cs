using System;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace RegisterParcelsFromPC
{
    static class Program
    {
        enum event_type
        {
            �o�^ = 1,
            ��� = 2,
            �폜 = 3,
            ��������� = 10,
            ���܂莖�����p���[�h�J�n = 11,
            ���܂莖�����p���[�h�I�� = 12,
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
            NLogService.PrintInfoLog("�A�v���N��");
            Application.Run(new Form1());
            //Console.ReadKey();
        }
        
        

    }
}
