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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            textBox1.Text= ConfigurationManager.AppSettings["slack_testuser"];
        }

        private void button1_Click(object sender, EventArgs e)
        {

            QrCode qr = new QrCode();
            qr.QRcodeCreate("FA8239E3-320F-45BA-9EC0-F3678774FBEC", @"C:\temp\temp.gif");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //Periodic_check peri = new Periodic_check();
            //peri.send_slack("FB751035-46C3-4999-A6C5-EFBF24BFF650","フォームのボタンからのテスト送信",1);
            Httppost httppost = new Httppost();
            httppost.posting_DM(textBox1.Text, "test");
        }
    }
}
