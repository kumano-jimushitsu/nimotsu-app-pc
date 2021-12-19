using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace RegisterParcelsFromPC
{
    static class Program
    {
        enum event_type
        {
            登録=1,
            受取=2,
            削除=3,
            事務当交代=10,
            泊まり事務当用モード開始=11,
            泊まり事務当用モード終了=12,
        }


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //↓↓追加 NLOG
            //InitializeLogger();
            //↑↑追加


            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }

        /*
        NLOG用のあれ→失敗中
        private static void InitializeLogger()
        {
            var conf = new LoggingConfiguration();
            //ファイル出力定義
            var file = new FileTarget("file");
            file.Encoding = System.Text.Encoding.GetEncoding("shift-jis");
            file.Layout = "${longdate} [${threadid:padding=2}] [${uppercase:${level:padding=-5}}] ${callsite}() - ${message}${exception:format=ToString}";
            file.FileName = "${basedir}/logs/sample_${date:format=yyyyMMdd}.log";
            file.ArchiveNumbering = ArchiveNumberingMode.Date;           file.ArchiveFileName = "${basedir}/logs/sample.log.{#}";
            file.ArchiveEvery = FileArchivePeriod.None;
            file.MaxArchiveFiles = 10;
            conf.AddTarget(file);
            conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, file));

            //イベントログ出力定義 ※ただし初回は管理者として実行しないとSourceの登録ができない
            EventLogTarget eventlog = new EventLogTarget("eventlog");
            eventlog.Layout = "${message}${newline}${exception:format=ToString}";
            eventlog.Source = "NLogNoConfigSample";

            eventlog.Log = "Application";
            eventlog.EventId = "1001";
            conf.AddTarget(eventlog);
            conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, eventlog));

            // 設定を反映する
            LogManager.Configuration = conf;
        }
        //↑↑追加
        */
    }
}
