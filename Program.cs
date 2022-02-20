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
            登録 = 1,
            受取 = 2,
            削除 = 3,
            事務当交代 = 10,
            泊まり事務当用モード開始 = 11,
            泊まり事務当用モード終了 = 12,
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
            NLogService.PrintInfoLog("アプリ起動");
            Application.Run(new Form1());
            //Console.ReadKey();
        }
        
        

    }
}
