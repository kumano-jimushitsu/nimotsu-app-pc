using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class release_kakunin : Form
    {
        string m_owner_uid, m_staff_uid;
        public string connStr = ConfigurationManager.AppSettings["connStr"];

        public release_kakunin(string owner_uid, string staff_uid)
        {
            m_owner_uid = owner_uid;
            m_staff_uid = staff_uid;

            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {

                MakeSQLCommand sqlstr = new MakeSQLCommand();

                string sqlstr_get_all_current_parcel = sqlstr.toRelease_get_all_parcels(m_owner_uid);
                Operation ope = new Operation(connStr);
                List<string> CurrentParcels = ope.get_all_uid(sqlstr_get_all_current_parcel);
                //現状はその人名義の荷物をすべて取得している
                //ここを書き換えれば、荷物を選択とかできると思うけど、今のままにしておいてすべて受け取らせて必要があればイベント削除、とかの運用のほうが良いと思う。

                string time = DateTime.Now.ToString();
                //sqlstr.parcels_total_waittime = ope.calculate_registered_time(CurrentParcels, dt, owner_uid);
                string aSqlStr = "";
                aSqlStr += sqlstr.toRelease_parcels_table(CurrentParcels, "", time, m_staff_uid);
                aSqlStr += sqlstr.toRelease_parcelevent_table(CurrentParcels, m_owner_uid, time);
                aSqlStr += sqlstr.toRelease_ryosei_table(CurrentParcels, m_owner_uid, time);

                ope.execute_sql(aSqlStr);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dairi dr = new dairi(m_owner_uid, m_staff_uid);

            dr.FormClosed += this.dr_close;
            dr.Show();
        }
        private void dr_close(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }


        private void release_kakunin_Load(object sender, EventArgs e)
        {

        }
    }
}
