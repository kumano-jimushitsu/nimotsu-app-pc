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
            //�����ǉ� NLOG
            //InitializeLogger();
            //�����ǉ�


            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }

        /*
        NLOG�p�̂��ꁨ���s��
        private static void InitializeLogger()
        {
            var conf = new LoggingConfiguration();
            //�t�@�C���o�͒�`
            var file = new FileTarget("file");
            file.Encoding = System.Text.Encoding.GetEncoding("shift-jis");
            file.Layout = "${longdate} [${threadid:padding=2}] [${uppercase:${level:padding=-5}}] ${callsite}() - ${message}${exception:format=ToString}";
            file.FileName = "${basedir}/logs/sample_${date:format=yyyyMMdd}.log";
            file.ArchiveNumbering = ArchiveNumberingMode.Date;           file.ArchiveFileName = "${basedir}/logs/sample.log.{#}";
            file.ArchiveEvery = FileArchivePeriod.None;
            file.MaxArchiveFiles = 10;
            conf.AddTarget(file);
            conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, file));

            //�C�x���g���O�o�͒�` ������������͊Ǘ��҂Ƃ��Ď��s���Ȃ���Source�̓o�^���ł��Ȃ�
            EventLogTarget eventlog = new EventLogTarget("eventlog");
            eventlog.Layout = "${message}${newline}${exception:format=ToString}";
            eventlog.Source = "NLogNoConfigSample";

            eventlog.Log = "Application";
            eventlog.EventId = "1001";
            conf.AddTarget(eventlog);
            conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, eventlog));

            // �ݒ�𔽉f����
            LogManager.Configuration = conf;
        }
        //�����ǉ�
        */
    }
}
