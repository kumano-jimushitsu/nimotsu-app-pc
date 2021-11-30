using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form3 : Form
    {
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
            string shomubucho_slackid = sql.toSelect_slackid_of_ShomuBucho();

            Httppost httppost = new Httppost();
            httppost.posting_DM_image(shomubucho_slackid,ryosei_data,"ryosei tableデータ");
            httppost.posting_DM_image(shomubucho_slackid, parcels_data, "parcels tableデータ");
            httppost.posting_DM_image(shomubucho_slackid, event_data, "parcel_event tableデータ");

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }
    }
}
