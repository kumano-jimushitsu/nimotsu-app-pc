using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form4 : Form
    {
        string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=parcels;UID=sa;PWD=kumano";

        void register_new_ryosei()
        {
            try
            {


                //validating
                room_name.Text = Strings.StrConv(room_name.Text, VbStrConv.Narrow);

                //インスタンスを作り、登録したい情報を登録
                MakeSQLCommand sqlstr = new MakeSQLCommand();
                string m_room_name = room_name.Text;
                string ryosei_name = ryosei_family_name_kanji.Text + " " + ryosei_first_name_kanji.Text;
                string ryosei_name_alphabet = ryosei_alphabet_name.Text;
                string ryosei_name_kana = ryosei_family_name_kana.Text + " " + ryosei_first_name_kana.Text;

                int block_id = block_id_cal();
                string m_slack_id = slack_id.Text;

                //空欄がある場合判定
                if (m_room_name == "")
                {
                    MessageBox.Show("部屋番号が空欄です", "登録失敗", MessageBoxButtons.OK);
                    return;
                }
                if (ryosei_name == " ")
                {
                    MessageBox.Show("氏名名が空欄です", "登録失敗", MessageBoxButtons.OK);
                    return;
                }
                if (ryosei_name_kana == " ")
                {
                    MessageBox.Show("読みがなが空欄です", "登録失敗", MessageBoxButtons.OK);
                    return;
                }
                string aSqlstr = sqlstr.Register_new_ryosei_table(m_room_name, ryosei_name, ryosei_name_kana, ryosei_name_alphabet, block_id, m_slack_id);
                Operation ope = new Operation(connStr);
                ope.execute_sql(aSqlstr);
                MessageBox.Show("登録されました。");
                ryosei_family_name_kanji.Text = "";
                ryosei_first_name_kanji.Text = "";
                ryosei_alphabet_name.Text = "";
                slack_id.Text = "";
                ryosei_family_name_kana.Text = "";
                ryosei_first_name_kana.Text = "";
            }
            catch (Exception e)
            {

                NLogService.PrintInfoLog("例外_Form4");

                NLogService.PrintInfoLog(e.ToString());
            }
        }

        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            register_new_ryosei();
        }

        private void label3_Click(object sender, EventArgs e)
        {



        }

        int block_id_cal()
        {
            int block_id = 10;
            if (room_name.Text.Substring(0, 2) == "A1") block_id = 1;
            if (room_name.Text.Substring(0, 2) == "A2") block_id = 2;
            if (room_name.Text.Substring(0, 2) == "A3") block_id = 3;
            if (room_name.Text.Substring(0, 2) == "A4") block_id = 4;
            if (room_name.Text.Substring(0, 2) == "B1") block_id = 5;
            if (room_name.Text.Substring(0, 2) == "B2") block_id = 5;
            if (room_name.Text.Substring(0, 2) == "B3") block_id = 6;
            if (room_name.Text.Substring(0, 2) == "B4") block_id = 7;
            if (room_name.Text.Substring(0, 2) == "C1") block_id = 8;
            if (room_name.Text.Substring(0, 2) == "C2") block_id = 8;
            if (room_name.Text.Substring(0, 2) == "C3") block_id = 9;
            if (room_name.Text.Substring(0, 2) == "C4") block_id = 9;
            return block_id;
        }

        private void room_name_TextChanged(object sender, EventArgs e)
        {
            room_name.Text = room_name.Text.ToUpper();
            this.room_name.Select(this.room_name.Text.Length, 0);
        }

        private void label10_Click(object sender, EventArgs e)
        {


        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void slack_id_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
