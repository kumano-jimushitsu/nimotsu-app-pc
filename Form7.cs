using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            textBox1.Text= ConfigurationManager.AppSettings["slack_testuser"];
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string nowtime= DateTime.Now.ToString("HH-mm-ss");
            string temp_path = $@"C:\temp\{nowtime}.gif";
            QrCode qr = new QrCode();
            qr.QRcodeCreate(textBox2.Text, temp_path);
            Httppost httppost = new Httppost();
            httppost.posting_DM_image(textBox1.Text,temp_path, "image test from 庶務部用管理ツール");

            File.Delete(temp_path);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //Periodic_check peri = new Periodic_check();
            //peri.send_slack("FB751035-46C3-4999-A6C5-EFBF24BFF650","フォームのボタンからのテスト送信",1);
            Httppost httppost = new Httppost();
            httppost.posting_DM(textBox1.Text, "test from 庶務部用管理ツール");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }
    }
}
