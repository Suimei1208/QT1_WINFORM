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
    public partial class register : Form
    {
        public register()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (username.Text == "" || pass.Text == "" || name.Text == "" || address.Text == "" || confirm.Text == "")
            {
                MessageBox.Show("Please enter all");
            }
            else if(pass.Text != confirm.Text)
            {
                MessageBox.Show("Please enter password correct!");
            }
            else
            {
                using (var context = new Model1())
                {
                    var user = context.Customers.SingleOrDefault(u => u.Username == username.Text);
                    if (user != null)
                    {
                        MessageBox.Show("Username allredy have exsit!!!!!");
                    }
                    else
                    {
                        int newCustomerNumber = 1;
                        
                        var sortedData = context.Customers.ToList().OrderBy(c => int.Parse(c.CustID.Substring("CustID".Length))).ToList();
                        if (sortedData != null)
                        {
                            string lastCustID = sortedData.Last().CustID;
                            string[] parts = lastCustID.Split(new string[] { "CustID" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                            {
                                if (int.TryParse(parts[1], out int lastNumber))
                                {
                                    newCustomerNumber = lastNumber + 1;
                                }
                            }
                        }

                        string newCustID = "CustID" + newCustomerNumber.ToString();
                        Customer newCustomer = new Customer
                        {
                            CustID = newCustID,
                            CustName = name.Text,
                            Address = address.Text,
                            Username = username.Text,
                            password = pass.Text                                                   
                        };
                        context.Customers.Add(newCustomer);
                        context.SaveChanges();

                        MessageBox.Show("Register sucessfully! Please click OK return login form and login again.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Main main = new Main();
                        main.Show();
                        this.Close();

                    }
                }
            }
        }
    }
}
