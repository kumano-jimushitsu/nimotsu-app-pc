using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            show_ryoseiTable();
        }

        public string connStr = ConfigurationManager.AppSettings["connStr"];
        int ryoseiTable_block = 0;
        string ryosei_uid = "";
        string room_name = "";
        string ryosei_name = "";
        string ryosei_name_kana = "";
        string ryosei_name_alphabet = "";
        string block_id = "";
        string slack_id = "";
        string status = "";

        public void show_ryoseiTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                MakeSQLCommand sqlstr = new MakeSQLCommand();

                cmd.CommandText = sqlstr.forShow_ryosei_table_for_management(ryoseiTable_block);//ryoseiTable_blockが負の値なら、その他を取るようにsqlstr内で分岐してる
                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                
            }
            dataGridView1.DataSource = dt;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.RowTemplate.Height = 45;
            this.dataGridView1.CurrentCell = null;
            this.dataGridView1.Columns["uid"].Visible = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }


        void block_change(int block_id)
        {
            ryoseiTable_block = block_id;
            show_ryoseiTable();
        }

        private void A1_CheckedChanged(object sender, EventArgs e)
        {
            block_change(1);
        }

        private void A2_CheckedChanged(object sender, EventArgs e)
        {
            block_change(2);
        }

        private void A3_CheckedChanged(object sender, EventArgs e)
        {
            block_change(3);
        }

        private void A4_CheckedChanged(object sender, EventArgs e)
        {
            block_change(4);
        }

        private void B12_CheckedChanged(object sender, EventArgs e)
        {
            block_change(5);
        }

        private void B3_CheckedChanged(object sender, EventArgs e)
        {
            block_change(6);
        }

        private void B4_CheckedChanged(object sender, EventArgs e)
        {
            block_change(7);
        }

        private void C12_CheckedChanged(object sender, EventArgs e)
        {
            block_change(8);
        }

        private void C34_CheckedChanged(object sender, EventArgs e)
        {
            block_change(9);
        }

        private void rin_CheckedChanged(object sender, EventArgs e)
        {
            block_change(10);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView g = sender as DataGridView;
            cbx_block_id.SelectedIndex = -1;
            cbx_status.SelectedIndex = -1;

            if (g != null)
            {
                int col = e.ColumnIndex;
                int row = e.RowIndex;
                if (col < 0 || row < 0) { return; }
                ryosei_uid = g[0, row].Value.ToString();
                room_name = g[1, row].Value.ToString();
                ryosei_name = g[2, row].Value.ToString();
                ryosei_name_kana = g[3, row].Value.ToString();
                ryosei_name_alphabet = g[4, row].Value.ToString();
                block_id = g[5, row].Value.ToString();
                slack_id = g[6, row].Value.ToString();
                status = g[7, row].Value.ToString();//TrueまたはFalse

                tbx_room_name.Text = room_name;
                tbx_ryosei_name.Text = ryosei_name;
                tbx_ryosei_name_kana.Text = ryosei_name_kana;
                tbx_ryosei_name_alphabet.Text = ryosei_name_alphabet;
                tbx_slack_id.Text = slack_id;
                label8.Text = "現在：" + block_id;
                label11.Text = "現在：" + status;
                /*
                 * 
        string room_name = "";
        string ryosei_name = "";
        string ryosei_name_kana = "";
        string ryosei_name_alphabet = "";
        string block_id = "";
        string slack_id = "";
        string status = "";
                 */
            }


        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //update文を発行
            MakeSQLCommand sqlstr = new MakeSQLCommand();
            sqlstr.ryosei_uid = ryosei_uid;
            sqlstr.room_name = tbx_room_name.Text;
            sqlstr.ryosei_name = tbx_ryosei_name.Text;
            sqlstr.ryosei_name_kana = tbx_ryosei_name_kana.Text;
            sqlstr.ryosei_name_alphabet = tbx_ryosei_name_alphabet.Text;
            sqlstr.slack_id = tbx_slack_id.Text;
            int block_id_int;
            if (cbx_block_id.SelectedIndex < 0)//初期状態だと-1を返す→その時は初期値を設定
            {
                if (int.TryParse(block_id, out block_id_int))
                {
                    sqlstr.block_id = block_id_int;
                }
            }
            else
            {
                if (int.TryParse(cbx_block_id.Items[cbx_block_id.SelectedIndex].ToString(), out block_id_int))
                {
                    sqlstr.block_id = block_id_int;
                }
            }

            int status_int;
            if (cbx_status.SelectedIndex < 0)//初期状態だと-1を返す→その時は初期値を設定
            {
                if (int.TryParse(status, out status_int))
                {
                    sqlstr.status = status_int;
                }
            }
            else
            {
                if (int.TryParse(cbx_status.Items[cbx_status.SelectedIndex].ToString(), out status_int))
                {
                    sqlstr.status = status_int;
                }
            }


            //メッセージボックスの文面を作成
            string msg = room_name + " " + ryosei_name + "さんの情報を以下のように変更します\n";
            string change = "";
            if (room_name != sqlstr.room_name) change += $"部屋番号: {room_name} → {sqlstr.room_name}\n";
            if (ryosei_name != sqlstr.ryosei_name) change += $"氏名（漢字）: {ryosei_name} → {sqlstr.ryosei_name}\n";
            if (ryosei_name_kana != sqlstr.ryosei_name_kana) change += $"氏名（かな）：{ryosei_name_kana} → {sqlstr.ryosei_name_kana}\n";
            if (ryosei_name_alphabet != sqlstr.ryosei_name_alphabet) change += $"氏名（英字）： {ryosei_name_alphabet} → {sqlstr.ryosei_name_alphabet}\n";
            if (slack_id != sqlstr.slack_id) change += $"slack_id：{slack_id} → {sqlstr.slack_id}\n";
            if (block_id != sqlstr.block_id.ToString()) change += $"block_id：　{block_id} → {sqlstr.block_id}\n";
            if (status != sqlstr.status.ToString()) change += $"在籍状態：　{status} → {sqlstr.status}\n";
            msg += change;
            if (ryosei_uid == "" || change == "")
            {
                msg = "変更されません";
                MessageBox.Show(msg, "boxTitle", MessageBoxButtons.OK);
            }
            else
            {
                DialogResult result;
                result = MessageBox.Show(msg, "boxTitle", MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {

                    string edit_str = sqlstr.toEdit_ryosei_for_management();
                    Operation ope = new Operation(connStr);
                    ope.execute_sql(edit_str);
                    show_ryoseiTable();
                }
            }




        }

        private void button2_Click(object sender, EventArgs e)
        {
            Httppost httppost = new Httppost();
            string user_code = tbx_slack_id.Text;
            string message_str = $"庶務部用編集画面からのテスト送信";
            httppost.posting_DM(user_code,message_str);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            block_change(-1);
        }

        private void cbx_status_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

