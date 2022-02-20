using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form3 : Form
    {

        public string connStr = ConfigurationManager.AppSettings["connStr"];
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            form6.Show();


        }

        private void button4_Click(object sender, EventArgs e)
        {

            try
            {

                Process p = new Process();

                p.StartInfo.FileName = @"\temp\dataoutput.bat";
                p.StartInfo.Verb = "RunAs"; //管理者として実行する場合

                p.Start();

                string now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string ryosei_data = @$"C:\temp\ryosei_data_at_{now}.csv";
                string parcels_data = @$"C:\temp\parcels_data_at_{now}.csv";
                string event_data = @$"C:\temp\event_data_at_{now}.csv";

                File.Move(@"C:\temp\output_ryosei.csv", ryosei_data);
                File.Move(@"\temp\output_parcels.csv", parcels_data);
                File.Move(@"\temp\output_parcel_event.csv", event_data);

                MakeSQLCommand sql = new MakeSQLCommand();
                Operation ope = new Operation(connStr);
                string shomubucho_slackid = ope.select_one_xx(sql.toSelect_slackid_of_ShomuBucho(), "slack_id");

                string msgbox = "庶務部長として登録されているユーザーがいません。";
                if (shomubucho_slackid != null)
                {
                    msgbox = "以下のユーザーにバックアップデータを送信しました\r\n" + shomubucho_slackid;
                }
                MessageBox.Show(msgbox, "boxTitle", MessageBoxButtons.OKCancel);
                Httppost httppost = new Httppost();
                httppost.posting_DM_image(shomubucho_slackid, ryosei_data, "ryosei tableデータ");
                httppost.posting_DM_image(shomubucho_slackid, parcels_data, "parcels tableデータ");
                httppost.posting_DM_image(shomubucho_slackid, event_data, "parcel_event tableデータ");

            }
            catch (Exception ee)
            {

                NLogService.PrintInfoLog("例外_Form3");

                NLogService.PrintInfoLog(ee.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }
    }
}
