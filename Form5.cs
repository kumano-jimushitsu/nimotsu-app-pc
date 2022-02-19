using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form5 : Form
    {
        public string connStr = ConfigurationManager.AppSettings["connStr"];
        Operation ope;
        MakeSQLCommand sqlstr = new MakeSQLCommand();
        
        int ryoseiTable_block = 0;
        string ryosei_uid = "";
        string room_name = "";
        string ryosei_name = "";
        string ryosei_name_kana = "";
        string ryosei_name_alphabet = "";
        string block_id = "";
        string slack_id = "";
        string block_name = "";
        string status_name = "";
        string status = "";
        public Form5()
        {
            ope = new Operation(connStr);
            InitializeComponent();
            show_ryoseiTable();
            List<string> block_list = ope.get_all_uid(sqlstr.get_all_block_id());//as uidとしてselectすることで、荷物引き渡しの時に使っていたget_all_uidを流用してList<string>を得る
            cbx_block_id.Items.Clear();
            cbx_block_id.Items.AddRange(block_list.ToArray());
            
            List<string> status_list = ope.get_all_uid(sqlstr.get_all_ryosei_status());//as uidとしてselectすることで、荷物引き渡しの時に使っていたget_all_uidを流用してList<string>を得る
            cbx_status.Items.Clear();
            cbx_status.Items.AddRange(status_list.ToArray());
        }
        public void show_ryoseiTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();

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

                block_name = ope.select_one_xx($@"select block_id from block_id where no = {block_id}", "block_id");
                status_name = ope.select_one_xx($@"select ryosei_status from ryosei_status where no = {status}", "ryosei_status");

                tbx_room_name.Text = room_name;
                tbx_ryosei_name.Text = ryosei_name;
                tbx_ryosei_name_kana.Text = ryosei_name_kana;
                tbx_ryosei_name_alphabet.Text = ryosei_name_alphabet;
                tbx_slack_id.Text = slack_id;
                label8.Text = "現在：" + block_name;
                label11.Text = "現在：" + status_name;
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
            try
            {
                //update文を発行
                MakeSQLCommand sqlstr = new MakeSQLCommand();
                Operation ope = new Operation(connStr);
                string m_ryosei_uid = ryosei_uid;
                string m_room_name = tbx_room_name.Text;
                string m_ryosei_name = tbx_ryosei_name.Text;
                string m_ryosei_name_kana = tbx_ryosei_name_kana.Text;
                string m_ryosei_name_alphabet = tbx_ryosei_name_alphabet.Text;
                string m_slack_id = tbx_slack_id.Text;
                int m_block_id_int;
                //string m_block_id_string;
                string m_block_name="";
                if (cbx_block_id.SelectedIndex < 0)//初期状態だと-1を返す→その時は初期値を設定
                {
                    int.TryParse(block_id, out m_block_id_int);
                    //m_block_id_string = block_id;

                }
                else
                {
                    m_block_name = cbx_block_id.Items[cbx_block_id.SelectedIndex].ToString();
                    string sql =sqlstr.get_block_id_no_from_index(m_block_name);//←表示されているのを選ぶ
                    m_block_id_int = ope.select_one_xx_int(sql, "no");
                    //int.TryParse(cbx_block_id.Items[cbx_block_id.SelectedIndex].ToString(), out m_block_id_int);
                }

                int m_status_int;
                //string m_status_string;
                string m_status_name="";
                if (cbx_status.SelectedIndex < 0)//初期状態だと-1を返す→その時は初期値を設定
                {
                    int.TryParse(status, out m_status_int);
                    //m_status_string= status;

                }
                else
                {
                    m_status_name = cbx_status.Items[cbx_status.SelectedIndex].ToString();
                    string sql = sqlstr.get_ryosei_status_no_from_index(m_status_name);//←表示されているのを選ぶ
                    m_status_int = ope.select_one_xx_int(sql, "no");

                    //int.TryParse(cbx_status.Items[cbx_status.SelectedIndex].ToString(), out m_status_int);

                }


                //メッセージボックスの文面を作成
                string msg = room_name + " " + ryosei_name + "さんの情報を以下のように変更します\n";
                string change = "";
                if (room_name != m_room_name) change += $"部屋番号: {room_name} → {m_room_name}\n";
                if (ryosei_name != m_ryosei_name) change += $"氏名（漢字）: {ryosei_name} → {m_ryosei_name}\n";
                if (ryosei_name_kana != m_ryosei_name_kana) change += $"氏名（かな）：{ryosei_name_kana} → {m_ryosei_name_kana}\n";
                if (ryosei_name_alphabet != m_ryosei_name_alphabet) change += $"氏名（英字）： {ryosei_name_alphabet} → {m_ryosei_name_alphabet}\n";
                if (slack_id != m_slack_id) change += $"slack_id：{slack_id} → {m_slack_id}\n";

                
                if (block_id != m_block_id_int.ToString()) change += $"block_id：　{block_name} → {m_block_name}\n";
                //if (block_id != m_block_id_int.ToString()) change += $"block_id：　{block_id} → {m_block_id_int}\n";
                if (status != m_status_int.ToString()) change += $"在籍状態：　{status_name} → {m_status_name}\n";
                //if (status != m_status_int.ToString()) change += $"在籍状態：　{status} → {m_status_int}\n";
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

                        if (slack_id != m_slack_id)
                        {
                            //slack id が変更（追加）されたら認証用のQRコードを送る

                            string filename = $@"C:\temp\{ryosei_uid}.gif";
                            Httppost httppost = new Httppost();
                            QrCode qr = new QrCode();
                            qr.QRcodeCreate(ryosei_uid, filename);
                            httppost.posting_DM_image(slack_id,filename, "初めまして！荷物を通知する熊野あじりbotです！このQRコードを事務室で読み込んで本人確認をしてね！");

                        }
                        string edit_str = sqlstr.toEdit_ryosei_for_management(room_name, ryosei_name, ryosei_name_kana, ryosei_name_alphabet, m_slack_id, m_block_id_int, m_status_int, ryosei_uid);
                        
                        ope.execute_sql(edit_str);
                        show_ryoseiTable();
                    }
                }

            }
            catch (Exception ee)
            {

                NLogService.PrintInfoLog("例外_form5");

                NLogService.PrintInfoLog(ee.ToString());
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Httppost httppost = new Httppost();
            string user_code = tbx_slack_id.Text;
            string message_str = $"あ～てすてす。てすと送信だよ！(庶務部用編集画面)";
            httppost.posting_DM(user_code, message_str);
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

