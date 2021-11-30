using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form6 : Form
    {
        public string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=parcels;UID=sa;PWD=kumano";
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filepath;
            OpenFileDialog ofDialog = new OpenFileDialog();

            // デフォルトのフォルダを指定する
            ofDialog.InitialDirectory = @"C:";

            //ダイアログのタイトルを指定する
            ofDialog.Title = "ダイアログのタイトル";
            ofDialog.Filter = "csv文書|*.csv";
            //ダイアログを表示する
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(ofDialog.FileName);
                filepath = ofDialog.FileName;

                            List<string[]> new_ryosei_list= new List<string[]>();
                bool flagOK = true;
                try
                {
                    //読み込むtxtファイルのパスを指定して開く
                    using (StreamReader sr = new StreamReader(filepath,Encoding.GetEncoding("UTF-8")))
                    {
                        //ブロック,部屋番号（部屋名）,名前（姓・漢字）,名前（名・漢字）,名前（姓・かな）,名前（名・かな）,名前（氏名・アルファベット）
                        //3,A303,前田,敏貴,まえだ,としき,Toshiki Maeda
                        //末尾まで繰り返す
                        int i = 0;//ヘッダは読み飛ばす
                        while (!sr.EndOfStream)
                        {
                            //1行づつ読み取る。
                            string line = sr.ReadLine();
                            if (i == 0)
                            {
                                i = 1;

                                continue;
                            }

                            string[] temp = line.Split(',');
                            temp[2] += " " + temp[3];
                            temp[4] += " " + temp[5];
                            if(temp[4]==" "||temp[1]=="" || temp[0] == "")
                            {
                                //かなの名前がない場合、ブロック番号、部屋名が無い場合
                                flagOK = false;
                            }
                            if(temp[2]==" " && temp[6] == "")
                            {
                                flagOK = false;
                            }
                            new_ryosei_list.Add(temp); 
                        }
                    }
                }
                //System.IO.FileNotFoundExceptionを省略して
                catch (FileNotFoundException ea)
                {
                    Console.WriteLine(ea.Message);
                }
                //System.Exceptionを省略して
                catch (Exception ea)
                {
                    //textBox1.Text += "読み込みたいファイルが開かれています";
                    Console.WriteLine(ea.Message);
                }
                if (flagOK)
                {

                MakeSQLCommand make = new MakeSQLCommand();
                string insert_str=make.Register_new_ryosei_atonetime(new_ryosei_list);
                Operation ope = new Operation(connStr);
                ope.execute_sql(insert_str);
                }
                else
                {
                    MessageBox.Show("csvファイルのデータに欠損があります", "boxTitle", MessageBoxButtons.OK);

                }

            }
            else
            {
                MessageBox.Show("キャンセルされました", "boxTitle", MessageBoxButtons.OK);

            }

            // オブジェクトを破棄する
            ofDialog.Dispose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MakeSQLCommand make = new MakeSQLCommand();
            string insert_str = make.toSelect_distinct_ryosei();
            Operation ope = new Operation(connStr);
            int a = int.Parse(ope.select_one_xx(insert_str,"diff"));//ryosei_name_kanaでdistinct
            if (a > 0)
            {
                MessageBox.Show("寮生の氏名(かな)が重複しています（計"+a.ToString()+"件\r\n修正してください）", "boxTitle", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("寮生の氏名は重複していません", "boxTitle", MessageBoxButtons.OK);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }
    }
}
