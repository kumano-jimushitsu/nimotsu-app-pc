using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class dairi : Form
    {
        string m_owner_uid, m_staff_uid;

        int ryoseiTable_block = 0;
        public string connStr = ConfigurationManager.AppSettings["connStr"];
        public dairi(string owner_uid, string staff_uid)
        {

            m_owner_uid = owner_uid;
            m_staff_uid = staff_uid;
            InitializeComponent();
        }

        private void dairi_Load(object sender, EventArgs e)
        {

        }
        public void show_ryosei_table_dairi()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                MakeSQLCommand sqlstr = new MakeSQLCommand();

                cmd.CommandText = sqlstr.forShow_ryosei_table_dairiRelease(ryoseiTable_block);


                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView1.DataSource = dt;
            this.dataGridView1.Columns["uid"].Visible = false;

            this.dataGridView1.CurrentCell = null;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.RowTemplate.Height = 60;
        }

        private void A1_CheckedChanged(object sender, EventArgs e)
        {
            ryoseiTable_block = 1;
            show_ryosei_table_dairi();
        }

        private void A2_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 2;
            show_ryosei_table_dairi();
        }

        private void A3_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 3;
            show_ryosei_table_dairi();
        }

        private void A4_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 4;
            show_ryosei_table_dairi();
        }

        private void B12_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 5;
            show_ryosei_table_dairi();
        }

        private void B3_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 6;
            show_ryosei_table_dairi();
        }

        private void B4_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 7;
            show_ryosei_table_dairi();
        }

        private void C12_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 8;
            show_ryosei_table_dairi();
        }

        private void C34_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 9;
            show_ryosei_table_dairi();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView g = sender as DataGridView;

            if (g != null)
            {
                int col = e.ColumnIndex;
                int row = e.RowIndex;
                string agent_uid = g[0, row].Value.ToString();
                string agent_room = g[1, row].Value.ToString();
                string agent_name = g[2, row].Value.ToString();


                DialogResult result;
                string msgbox_str = $@"代理で{agent_room} {agent_name}さんが受け取ります";
                result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    DateTime dt = DateTime.Now;//total_wait_timeの計算にも使用している。
                                               //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql
                    MakeSQLCommand sqlstr = new MakeSQLCommand();

                    string sqlstr_get_all_current_parcel = sqlstr.toRelease_get_all_parcels(m_owner_uid);
                    Operation ope = new Operation(connStr);
                    List<string> CurrentParcels = ope.get_all_parcels_uid(sqlstr_get_all_current_parcel);
                    //現状はその人名義の荷物をすべて取得している
                    //ここを書き換えれば、荷物を選択とかできると思うけど、今のままにしておいてすべて受け取らせて必要があればイベント削除、とかの運用のほうが良いと思う。

                    string time = dt.ToString();
                    //sqlstr.parcels_total_waittime = ope.calculate_registered_time(CurrentParcels, dt, owner_uid);
                    string aSqlStr = "";
                    aSqlStr += sqlstr.toRelease_parcels_table(CurrentParcels, agent_uid, time, m_staff_uid);
                    aSqlStr += sqlstr.toRelease_parcelevent_table(CurrentParcels, m_owner_uid, time);
                    aSqlStr += sqlstr.toRelease_ryosei_table(CurrentParcels, m_owner_uid, time);

                    ope.execute_sql(aSqlStr);
                    this.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rin_CheckedChanged(object sender, EventArgs e)
        {

            ryoseiTable_block = 10;
            show_ryosei_table_dairi();
        }
    }
}
