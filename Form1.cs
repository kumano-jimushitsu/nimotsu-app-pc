using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Configuration;

namespace RegisterParcelsFromPC
{
    public partial class Form1 : Form
    {
        //public string connStr = ConfigurationManager.AppSettings["connStr"];

        public const string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=parcels;UID=sa;PWD=kumano";
        MakeSQLCommand sqlstr = new MakeSQLCommand();
        public Operation ope = new Operation(connStr);


        int ryoseiTable_block = 1;
        string staff_uid = "0000000000";
        int night_duty_mode = 0;
        string staff_ryosei_name = "", staff_ryosei_room = "";


        Color color_register = Color.FromArgb(218, 255, 245);
        Color color_release = Color.FromArgb(217, 255, 218);
        Color color_delete = Color.FromArgb(255, 216, 216);
        Color color_deletable = Color.FromArgb(255, 240, 240);

        Color color_night_duty_mode = Color.Lavender;



        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowTemplate.Height = 60;
            dataGridView2.RowTemplate.Height = 60;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowCellInformation(sender, e, "left_side");
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowCellInformation(sender, e, "right_top_side");
        }

        void ShowCellInformation(object sender, DataGridViewCellEventArgs args, string boxTitle)
        {

            DataGridView g = sender as DataGridView;
            try
            {

                if (g != null)
                {
                    int col = args.ColumnIndex;
                    int row = args.RowIndex;

                    //
                    // クリックがヘッダー部分などの場合はインデックスが-1となります。
                    //ryosei table col 0:部屋番号、1:氏名、2:荷物数、3:登録、4:受取、5:slack_id, 6:ryosei_uid
                    //ryosei night col 0:部屋 1:氏名、2 
                    //event table  col 0:イベント種類、1:uid、2:部屋番号, 3:氏名、4:時刻、5:note、6:parcel_uid,7:ryosei_uid,8:is_finished("True"もしくは"False"で渡される）
                    if (boxTitle == "left_side")
                    {
                        string room_name = g[0, row].Value.ToString();
                        string ryosei_name = g[1, row].Value.ToString();
                        //int current_parcel_count = int.Parse(g[2, row].Value.ToString());
                        string slack_id = g[5, row].Value.ToString();
                        string uid = g[6, row].Value.ToString();

                        if (row >= 0 && col == 1)//事務当の登録
                        {
                            change_staff(uid, room_name, ryosei_name);
                        }
                        if (row >= 0 && col == 3)
                        {
                            if (night_duty_mode == 1)
                            {
                                if (g[7, row].Value.ToString() == "False")
                                {
                                    night_check_exist(uid);

                                }
                                else
                                {
                                    MessageBox.Show("紛失していることになっています。発見した場合は、チェックボックスからチェックを外してください", "", MessageBoxButtons.OK);
                                }


                            }
                            else//荷物の登録
                            {
                                register(uid, staff_uid, ryosei_name, room_name, slack_id);
                            }


                        }
                        if (row >= 0 && col == 4)//受取
                        {
                            if (night_duty_mode == 1)
                            {
                                night_check_nameplate(row, col, g);
                            }
                            else//荷物
                            {
                                if (int.Parse(g[2, row].Value.ToString()) > 0) release(uid, staff_uid, ryosei_name, room_name);

                            }
                            //result = MessageBox.Show(string.Format("行：{0}, 列：{1}, 値：{2}", row, col, g[col, row].Value), boxTitle, MessageBoxButtons.OKCancel);
                        }
                        if (row >= 0 && col == 7)//紛失チェックボックス
                        {
                            if (night_duty_mode == 1)
                            {
                                if (g[7, row].Value.ToString() == "False")
                                {
                                    night_check_lost(uid, 1);

                                }
                                else
                                {
                                    night_check_lost(uid, 0);
                                }
                            }
                        }
                    }


                    if (row >= 0 && col >= 0 && boxTitle == "right_top_side")
                    {//イベント削除のイベント。分岐がややこしいので、関数の中で分岐するように変更したい。

                        string event_type = g[0, row].Value.ToString();
                        string event_uid = g[1, row].Value.ToString();
                        string room_name = g[2, row].Value.ToString();
                        string ryosei_name = g[3, row].Value.ToString();
                        string parcel_uid = g[6, row].Value.ToString();
                        string ryosei_uid = g[7, row].Value.ToString();
                        string is_finished = g[8, row].Value.ToString();//TrueまたはFalse


                        delete(event_type, event_uid, ryosei_uid, parcel_uid, room_name, ryosei_name, is_finished);


                    }


                }
            }
            catch
            {

            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            show_parcels_eventTable();
        }

        public void show_ryoseiTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                if (night_duty_mode == 0)
                {
                    cmd.CommandText = sqlstr.forShow_ryosei_table(ryoseiTable_block);
                }
                else
                {
                    cmd.CommandText = sqlstr.forShow_ryosei_table_night_duty_mode();
                }

                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView1.DataSource = dt;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.RowTemplate.Height = 60;
            if (night_duty_mode == 0) this.dataGridView1.Columns["slack_id"].Visible = false;
            if (night_duty_mode == 1) dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.Columns["uid"].Visible = false;

            //各行に色を付ける処理
            //dataGridView2.Rows.Countとやると、常に51になる？？
            //よくわからんがバグの温床っぽい
            dataGridView1.Columns[3].DefaultCellStyle.BackColor = color_register;
            dataGridView1.Columns[4].DefaultCellStyle.BackColor = color_release;
            this.dataGridView1.CurrentCell = null;



        }

        public void show_parcels_eventTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr.forShow_event_table();
                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView2.DataSource = dt;
            this.dataGridView2.Columns["操作時刻"].DefaultCellStyle.Format = "MM/dd HH:mm:ss";
            //this.dataGridView2.Columns["uid"].Visible = false;
            this.dataGridView2.Columns["#"].Visible = false;

            this.dataGridView2.Columns["parcel_uid"].Visible = false;
            this.dataGridView2.Columns["ryosei_uid"].Visible = false;
            this.dataGridView2.Columns["is_finished"].Visible = false;

            this.dataGridView2.CurrentCell = null;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView2.RowTemplate.Height = 60;


            //各行に色を付ける処理
            //Rows.Count-1が大事
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {
                string event_type = dataGridView2.Rows[row].Cells[0].Value.ToString();
                string is_finished = dataGridView2.Rows[row].Cells[8].Value.ToString();
                if (event_type == "登録")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_register;
                }
                if (event_type == "受取")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_release;
                }
                if (is_finished == "False" && (event_type == "登録" || event_type == "受取"))
                {
                    dataGridView2.Rows[row].Cells[1].Style.BackColor = color_deletable;

                }
                if (event_type == "削除")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_delete;
                }
                if (event_type == "モード解除" || event_type == "モード開始")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_night_duty_mode;
                }
            }

        }


        void register(string owner_uid, string staff_uid, string ryosei_name, string room_name, string slack_id)
        {
            if (staff_uid == "0000000000")
            {
                MessageBox.Show("事務当を登録してください", "登録", MessageBoxButtons.OK);
                return;
            }
            DialogResult result;
            result = MessageBox.Show(room_name + " " + ryosei_name + "さんの荷物を登録します。", "登録", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                //タイムスタンプ
                string time = DateTime.Now.ToString();

                //uuidの作成→今のところ不要
                Guid g = Guid.NewGuid();
                string a = g.ToString();


                //SQL文の作成
                //---------------parcels {owner_room_name}','{owner_ryosei_name}','{register_datetime}','{register_staff_room_name}','{register_staff_ryosei_name}',{placement}
                int placement = 0;
                //----------------event created_at,event_type,room_name,ryosei_name,parcel_uid
                //parcel_uid => SQL文で自動取得
                //---------------ryoseiテーブルに必要なデータはすべて上で網羅されている
                string aSqlStr = "";
                aSqlStr += sqlstr.toRegister_parcels_table(owner_uid, time, staff_uid, placement);
                aSqlStr += sqlstr.toRegister_parcelevent_table(owner_uid, time, 1);//parcelsテーブルの更新よりも後に行う（parcel_uidをSQL文で取得しているため）
                aSqlStr += sqlstr.toRegister_ryosei_table(owner_uid, time);


                //実際に書き換え
                //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql
                ope.execute_sql(aSqlStr);
                show_parcels_eventTable();
                show_ryoseiTable();

                //slackでの通知→periodic_checkでやっているので、OK
                /*
                if (slack_id != "")//登録していない人はDBではNULLとなっており、ここには""(空のstring)の形で来る
                {
                    Httppost httppost = new Httppost();
                    string message_str = $"{sqlstr.register_datetime} に荷物が登録されました。";
                    httppost.posting_DM(slack_id,message_str);
                }
                */

            }


        }
        void release(string owner_uid, string staff_uid, string ryosei_name, string room_name)
        {
            if (staff_uid == "0000000000")
            {
                MessageBox.Show("事務当を登録してください", "boxTitle", MessageBoxButtons.OK);
                return;
            }


            DateTime dt = DateTime.Now;//total_wait_timeの計算にも使用している。
            //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql

            string sqlstr_get_all_current_parcel = sqlstr.toRelease_get_all_parcels(owner_uid);
            List<string> CurrentParcels = ope.get_all_uid(sqlstr_get_all_current_parcel);
            //現状はその人名義の荷物をすべて取得している
            //ここを書き換えれば、荷物を選択とかできると思うけど、今のままにしておいてすべて受け取らせて必要があればイベント削除、とかの運用のほうが良いと思う。

            //sqlstr.release_datetime = dt.ToString();
            //sqlstr.release_staff_uid = staff_uid;


            DialogResult result;
            string msgbox_str = $@"{room_name} {ryosei_name} さんの荷物は、現在{CurrentParcels.Count.ToString()}個登録されています。
荷物をすべて受け取りますか？
";
            result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                release_kakunin kknn = new release_kakunin(owner_uid, staff_uid);

                // イベントにメソッドを登録する。
                kknn.FormClosed += this.formA_Closed;

                kknn.Show();
                /*
                show_parcels_eventTable();
                show_ryoseiTable();
                */
            }

        }

        private void formA_Closed(object sender, FormClosedEventArgs e)
        {
            show_parcels_eventTable();
            show_ryoseiTable();
        }



        void confirm(string event_type, string event_uid, string ryosei_uid, string parcel_uid, string room_name, string ryosei_name)
        {
            DialogResult result;
            string msgbox_str = $@"{room_name} {ryosei_name} の操作を取り消しますか？";
            result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                DialogResult result2;
                string msgbox_str2 = $@"一度取り消した操作は元に戻すことは出来ません。
{room_name} {ryosei_name} の操作を取り消します。
よろしいですか？";
                result2 = MessageBox.Show(msgbox_str2, "boxTitle", MessageBoxButtons.OKCancel);

                if (result2 == DialogResult.OK)
                {
                    /*
                    int event_uid, ryosei_uid, parcel_uid;
                    int.TryParse(event_uid_str, out event_uid);
                    int.TryParse(ryosei_uid_str, out ryosei_uid);
                    int.TryParse(parcel_uid_str, out parcel_uid);
                    */
                    //{created_at}',{event_type},{parcel_uid},'{owner_room_name}','{owner_ryosei_name}
                    string time = DateTime.Now.ToString();
                    int m_event_type = 0;
                    if (event_type == "登録") m_event_type = 1;
                    if (event_type == "受取") m_event_type = 2;

                    string aSqlstr = "";
                    aSqlstr += sqlstr.toDeleteLogically_event_table(event_uid, time, parcel_uid, ryosei_uid);
                    aSqlstr += sqlstr.toDeleteLogically_ryosei_table(ryosei_uid, m_event_type);
                    aSqlstr += sqlstr.toDeleteLogically_parcels_table(parcel_uid, m_event_type);
                    Operation ope = new Operation(connStr);
                    ope.execute_sql(aSqlstr);

                    show_parcels_eventTable();
                    show_ryoseiTable();
                }
            }
        }

        void delete(string event_type, string event_uid, string ryosei_uid, string parcel_uid, string room_name, string ryosei_name, string is_finished)
        {
            //when 1 then '登録' when 2 then '受取' when 3 then '削除' when 10 then '当番交代' when 11 then 'モード開始' when 12 then 'モード解除'  else 'その他'
            //MakaSQLCommandのforShow_event_table()部分を参照する
            //enumとかを使ってエレガントにしたさもある

            //登録、削除のイベントを押すと登録されている情報が見られる。
            //見られる情報は以下の通り
            //部屋番号・氏名・荷物の種類(placement,fragile)・登録された時間・受取された時間・紛失情報・特記事項

            //is_finishedフラグについて
            //目的は、削除ができるかできないかを管理するフラグ。
            //デフォルトはFalse:0で、以下の条件を満たしたらTrue:1になる
            //・そのイベントが発生してから5分が経過したとき
            //・登録イベントが受取されたとき。

            if (is_finished == "True") return;

            if (event_type == "当番交代") return;
            if (event_type == "モード開始") return;
            if (event_type == "モード終了") return;
            if (event_type == "削除") return;


            //ここで5分以内かどうかを判定

            string placement, fragile, register_datetime, release_datetime, is_lost, note;
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr.forShow_confirm_msgbox(parcel_uid);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    placement = dr["placement"].ToString();
                    fragile = dr["fragile"].ToString();
                    register_datetime = dr["register_datetime"].ToString();
                    release_datetime = dr["release_datetime"].ToString();
                    is_lost = dr["is_lost"].ToString();
                    note = dr["note"].ToString();

                }

            }
            //fragileとかplacementあたりは変換しなければならない

            //ダイアログに表示される文章を作成
            string delete_kakunin = "";
            if (is_finished == "False")
            {
                delete_kakunin = $"\r\n\r\nこの操作を取り消しますか？";
            }
            string msgbox_str = $@"
部屋番号/氏名 : {room_name}  {ryosei_name}
登録された時間 : {register_datetime}
受取された時間 : {release_datetime}
荷物の種類     : {fragile}&{placement} 
紛失情報       : {is_lost}
特記事項       : {note}
{delete_kakunin}
";
            //ダイアログを出す（一段階目）
            DialogResult result;
            if (is_finished == "False")
            {
                result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.YesNo);

            }
            else
            {
                result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OK);
            }

            if (result == DialogResult.Yes)
            {
                //ここで、登録イベントであり過去に受取がされていれば処理を終了する


                //ダイアログを出す（二度目）
                DialogResult result2;
                string msgbox_str2 = $@"一度取り消した操作は元に戻すことは出来ません。
#{event_uid} の操作を取り消します。
よろしいですか？";
                result2 = MessageBox.Show(msgbox_str2, "boxTitle", MessageBoxButtons.OKCancel);

                if (result2 == DialogResult.OK)
                {

                    int m_event_type = 0;
                    if (event_type == "登録") m_event_type = 1;
                    if (event_type == "受取") m_event_type = 2;

                    //{created_at}',{event_type},{parcel_uid},'{owner_room_name}','{owner_ryosei_name}
                    string time = DateTime.Now.ToString();

                    string aSqlstr = "";
                    aSqlstr += sqlstr.toDeleteLogically_event_table(event_uid, time, parcel_uid, ryosei_uid);
                    aSqlstr += sqlstr.toDeleteLogically_ryosei_table(ryosei_uid, m_event_type);
                    aSqlstr += sqlstr.toDeleteLogically_parcels_table(parcel_uid, m_event_type);
                    ope.execute_sql(aSqlstr);

                    show_parcels_eventTable();
                    show_ryoseiTable();
                }
            }
        }
        void change_staff(string ryosei_uid, string room_name, string ryosei_name)
        {
            DialogResult result;
            result = MessageBox.Show("事務当を交代します\r\n次の事務当は" + room_name + " " + ryosei_name + "さんです。", "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                textBox1.Text = room_name;
                textBox2.Text = ryosei_name;
                staff_uid = ryosei_uid;
                staff_ryosei_room = room_name;
                staff_ryosei_name = ryosei_name;

                string time = DateTime.Now.ToString();
                string aSqlstr = sqlstr.toChangeStaff_event_table(time, ryosei_uid);
                ope.execute_sql(aSqlstr);

                show_parcels_eventTable();
            }

        }




        void Change_Mode(int event_type)
        {
            string time = DateTime.Now.ToString();
            string aSqlstr = sqlstr.toChangeMode(time, event_type);
            ope.execute_sql(aSqlstr);
            show_parcels_eventTable();

        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }





        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Periodic_check periodic_ = new Periodic_check();
            periodic_.periodical_check();
            //show_parcels_eventTable();

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

        private void button1_Click_1(object sender, EventArgs e)
        {
            QrCode qr = new QrCode();
            qr.QRcodeCreate("https://www.youtube.com/watch?v=_1xaOu83Sxk", @"C:\temp\temp.gif");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Periodic_check peri = new Periodic_check();
            peri.send_slack("FB751035-46C3-4999-A6C5-EFBF24BFF650", "フォームのボタンからのテスト送信", 1);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (night_duty_mode == 0)
            {
                night_duty_mode = 1;
                MessageBox.Show("泊事務当確認モード");
                Change_Mode(11);
            }
            else
            {
                night_duty_mode = 0;
                MessageBox.Show("モード解除\n掛札の確認も忘れずに行ってください");
                Change_Mode(12);
            }
            show_ryoseiTable();
        }
        void night_check_exist(string parcel_uid)
        {
            string sql = sqlstr.toCheck_whenNightDutyMode(parcel_uid);
            ope.execute_sql(sql);
            show_ryoseiTable();
        }
        void night_check_nameplate(int row, int col, DataGridView g)
        {
            g[col, row].Style.BackColor = Color.Blue;
        }
        void night_check_lost(string parcel_uid, int a)
        {//aは1or0(bool)
            DialogResult result2;
            string msgbox_str2 = $@"この荷物は現物があるため、紛失中のチェックを外します。
よろしいですか？";
            if (a == 1)
            {
                msgbox_str2 = $@"この荷物は現物がないため、紛失中のチェックを入れます。
よろしいですか？";
            }
            result2 = MessageBox.Show(msgbox_str2, "boxTitle", MessageBoxButtons.OKCancel);

            if (result2 == DialogResult.OK)
            {

                string sql = sqlstr.toCheck_lost_whenNightDutyMode(parcel_uid, a);

                ope.execute_sql(sql);
            }
            show_ryoseiTable();
            show_parcels_eventTable();
        }
    }
}
