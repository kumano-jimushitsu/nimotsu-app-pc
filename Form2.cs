using System;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            string password = textBox2.Text;


            if (id == "A3" && password == "kumano2021")
            {
                Form3 form3 = new Form3();
                form3.Show();
            }
            else
            {
                MessageBox.Show("パスワードまたはIDが間違っています。");
            }
            this.Close();

        }
    }
}
