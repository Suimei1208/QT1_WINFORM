using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QT1
{
    public partial class addOrder : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Model1 context;
        private BindingList<OrderDetail> orderDetails = new BindingList<OrderDetail>();
        int temp;
        int order_id;
        public addOrder()
        {
            InitializeComponent();
            context = new Model1();
            var custID = context.Customers.Select(c=> c.CustID).ToList();
            customer_id.DataSource = custID;

            var itemId = context.Items.Select(i =>i.ItemID).ToList();
            item_id.DataSource = itemId;
        }

        private void addOrder_Load(object sender, EventArgs e)
        {
            customer_name.Enabled = false;
            name_item.Enabled = false;
        }

        private void customer_id_SelectedIndexChanged(object sender, EventArgs e)
        {
            context = new Model1();
            customer_name.Text = context.Customers.Where(c => c.CustID == customer_id.Text)
                                                    .Select(c => c.CustName).FirstOrDefault();
        }

        private void item_id_SelectedIndexChanged(object sender, EventArgs e)
        {
            context = new Model1();
            int itemID;
            if (int.TryParse(item_id.Text, out itemID))
            {
                name_item.Text = context.Items
                    .Where(c => c.ItemID == itemID)
                    .Select(c => c.ItemName)
                    .FirstOrDefault();
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (item_id.Text == "" || Quantity.Text == "" || UnitAmount.Text == "")
            {
                MessageBox.Show("Please fill all the box!");
            }else if (groupBox1.Enabled == true) MessageBox.Show("Please choose customer!");
            else
            {
                context = new Model1();
                int rowCount = gridView.RowCount;
                if (rowCount > 0)
                {
                    string tenCotValue = gridView.GetRowCellValue(rowCount - 1, "ID").ToString();
                    temp = int.Parse(tenCotValue) + 1;
                }
                else
                {
                    temp = context.OrderDetails.OrderByDescending(od => od.ID).Select(od => od.ID).FirstOrDefault() + 1;
                }

                OrderDetail orderDetail = new OrderDetail()
                {
                    ID = temp,
                    OrderID = order_id,
                    ItemID = int.Parse(item_id.Text),
                    Quantity = int.Parse(Quantity.Text),
                    UnitAmount = int.Parse(UnitAmount.Text),
                };

                orderDetails.Add(orderDetail);
                gridControl.DataSource = orderDetails;

                Quantity.Clear();
                UnitAmount.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;

            using (var context = new Model1()) 
            {
                var customer = context.Customers.FirstOrDefault(c => c.CustID == customer_id.Text);
                order_id = GenerateUniqueOrderID();
                var order = new Order
                {
                    OrderID = order_id,
                    OrderDate = DateTime.Now,
                    CustID = customer_id.Text
                };

                context.Orders.Add(order);
                context.SaveChanges();

                MessageBox.Show("Order added successfully.");
            }
        }
        private int GenerateUniqueOrderID()
        {
            using (var context = new Model1())
            {
                var maxOrderID = context.Orders.Max(o => o.OrderID);
                return maxOrderID + 1;
            }
        }

        private void bbiSaveAndClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            var detailOrderBindingList = gridControl.DataSource as BindingList<OrderDetail>;

            if (detailOrderBindingList != null)
            {
                foreach (var orderDetail in detailOrderBindingList)
                {
                    context.OrderDetails.Add(orderDetail);
                }

                context.SaveChanges();                
            }
            context.Dispose();
            XtraForm1 xtraForm1 = new XtraForm1();
            xtraForm1.Show();
            this.Hide();
        }
    }
}
