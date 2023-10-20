using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QT1
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (username.Text == "" || password.Text == "")
            {
                MessageBox.Show("Please enter all");
            }
            else
            {
                using (var context = new Model1())
                {
                    var user = context.Customers.SingleOrDefault(u => u.Username == username.Text && u.password == password.Text);
                    if (user != null)
                    {
                        XtraForm1 newform = new XtraForm1();
                        newform.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Error password or username!!!!!");
                    }
                }
            }            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            register newform = new register();
            newform.Show();
            this.Hide();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
