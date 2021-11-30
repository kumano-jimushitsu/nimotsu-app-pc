using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class release_kakunin : Form
    {
        string m_owner_uid,m_staff_uid;
        public string connStr = ConfigurationManager.AppSettings["connStr"];

        public release_kakunin(string owner_uid,string staff_uid)
        {
            m_owner_uid = owner_uid;
            m_staff_uid = staff_uid;

            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                DateTime dt = DateTime.Now;//total_wait_timeの計算にも使用している。
                                           //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql
                MakeSQLCommand sqlstr = new MakeSQLCommand();
                sqlstr.owner_uid = m_owner_uid;

                string sqlstr_get_all_current_parcel = sqlstr.toRelease_get_all_parcels();
                Operation ope = new Operation(connStr);
                List<string> CurrentParcels = ope.get_all_uid(sqlstr_get_all_current_parcel);
                //現状はその人名義の荷物をすべて取得している
                //ここを書き換えれば、荷物を選択とかできると思うけど、今のままにしておいてすべて受け取らせて必要があればイベント削除、とかの運用のほうが良いと思う。

                sqlstr.release_datetime = dt.ToString();
                sqlstr.release_staff_uid = m_staff_uid;
                //sqlstr.parcels_total_waittime = ope.calculate_registered_time(CurrentParcels, dt, owner_uid);
                string aSqlStr = "";
                aSqlStr += sqlstr.toRelease_parcels_table(CurrentParcels,"");
                aSqlStr += sqlstr.toRelease_parcelevent_table(CurrentParcels);
                aSqlStr += sqlstr.toRelease_ryosei_table(CurrentParcels);

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
            dairi dr = new dairi(m_owner_uid,m_staff_uid);

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
