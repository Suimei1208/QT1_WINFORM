using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Export;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraBars.Ribbon;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.XtraGrid;

namespace QT1
{
    public partial class XtraForm1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private RibbonPage tab1;
        private RibbonPage tab2, tab3;
        private BindingList<ItemSummary> bestSellingItemsList;
        private Model1 context;
        public XtraForm1()
        {
            InitializeComponent();
            bestSellingItemsList = new BindingList<ItemSummary>();
        }
        private void XtraForm1_Load(object sender, EventArgs e)
        {
            context = new Model1();
            var query = from o in context.Orders
                        join od in context.OrderDetails on o.OrderID equals od.OrderID
                        select new
                        {
                            od.ID,
                            o.OrderID,
                            o.OrderDate,
                            o.CustID,
                            CustomerName = o.Customer.CustName,
                            od.Quantity,
                            od.UnitAmount,
                            ItemName = od.Item.ItemName,
                        };

            var result = query.ToList();



            gridView.Columns.Clear();

            gridControl.DataSource = null;
            gridControl.DataSource = result;

        }

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = Microsoft.VisualBasic.Interaction.InputBox("Nhập tên mặt hàng:", "Thêm mặt hàng", "");
            string size = Microsoft.VisualBasic.Interaction.InputBox("Nhập kích thước:", "Thêm mặt hàng", "");

            if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(size))
            {
                using (var context = new Model1())
                {
                    Item newItem = new Item
                    {
                        ItemName = itemName,
                        Size = size
                    };

                    context.Items.Add(newItem);
                    context.SaveChanges();

                    var items = context.Items.ToList();
                    gridControl.DataSource = items;
                    gridView.RefreshData();
                }
            }
        }

        private void gridControl_DoubleClick(object sender, EventArgs e)
        {
            using (var context = new Model1())
            {
                var selectedRow = gridView.GetFocusedRow() as Item;

                if (selectedRow != null)
                {
                    string updatedItemName = Microsoft.VisualBasic.Interaction.InputBox("Nhập tên mặt hàng:", "Chỉnh sửa mặt hàng", selectedRow.ItemName);
                    string updatedSize = Microsoft.VisualBasic.Interaction.InputBox("Nhập kích thước:", "Chỉnh sửa mặt hàng", selectedRow.Size);

                    if (!string.IsNullOrEmpty(updatedItemName) && !string.IsNullOrEmpty(updatedSize))
                    {
                        selectedRow.ItemName = updatedItemName;
                        selectedRow.Size = updatedSize;

                        int id = selectedRow.ItemID;
                        var itemupdate = context.Items.FirstOrDefault(i => i.ItemID == id);
                        if(itemupdate != null)
                        {
                            itemupdate.ItemName = updatedItemName;
                            itemupdate.Size = updatedSize;
                            context.SaveChanges();
                        }                      

                        gridControl.RefreshDataSource();
                    }
                }
            }
        }

        private void bbiEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var context = new Model1())
            {
                var selectedRow = gridView.GetFocusedRow() as Item;

                if (selectedRow != null)
                {
                    string updatedItemName = Microsoft.VisualBasic.Interaction.InputBox("Nhập tên mặt hàng:", "Chỉnh sửa mặt hàng", selectedRow.ItemName);
                    string updatedSize = Microsoft.VisualBasic.Interaction.InputBox("Nhập kích thước:", "Chỉnh sửa mặt hàng", selectedRow.Size);

                    if (!string.IsNullOrEmpty(updatedItemName) && !string.IsNullOrEmpty(updatedSize))
                    {
                        selectedRow.ItemName = updatedItemName;
                        selectedRow.Size = updatedSize;

                        int id = selectedRow.ItemID;
                        var itemupdate = context.Items.FirstOrDefault(i => i.ItemID == id);
                        if (itemupdate != null)
                        {
                            itemupdate.ItemName = updatedItemName;
                            itemupdate.Size = updatedSize;
                            context.SaveChanges();
                        }

                        gridControl.RefreshDataSource();
                    }
                }
            }
        }

        private void bbiDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var context = new Model1())
            {
                var selectedRow = gridView.GetFocusedRow() as Item;
                if (selectedRow != null)
                {
                    var itemToDelete = context.Items.FirstOrDefault(item => item.ItemID == selectedRow.ItemID);

                    if (itemToDelete != null)
                    {
                        context.Items.Remove(itemToDelete);

                        context.SaveChanges();

                        var items = context.Items.ToList();
                        gridControl.DataSource = items;
                    }
                }
            }
        }

        private void bbiPrintPreview_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string outputPath = saveFileDialog.FileName;
                ExportGridToPDF(outputPath, gridView);
            }
        }

        private void ExportGridToPDF(string outputPath, GridView gridView)
        {

            PrintingSystem printingSystem = new PrintingSystem();


            PrintableComponentLink printableComponentLink = new PrintableComponentLink(printingSystem);


            printableComponentLink.Component = gridControl;


            printableComponentLink.PrintingSystem.ExportOptions.Pdf.DocumentOptions.Author = "Your Name";
            printableComponentLink.PrintingSystem.ExportOptions.Pdf.DocumentOptions.Title = "GridControl Export";
            printableComponentLink.PrintingSystem.ExportOptions.Pdf.DocumentOptions.Subject = "Data Export";


            printableComponentLink.Landscape = false;


            printableComponentLink.ExportToPdf(outputPath);

            printableComponentLink.Dispose();
            printingSystem.Dispose();
        }

        private void ribbonControl_SelectedPageChanged(object sender, EventArgs e)
        {
            RibbonControl ribbon = sender as RibbonControl;

            if (ribbon != null)
            {
                if (tab1 == null)
                {
                    tab1 = ribbon.Pages["ADD ITEM"] as RibbonPage;
                }
                if (tab2 == null)
                {
                    tab2 = ribbon.Pages["ORDER"] as RibbonPage;
                }
                if (tab3 != null){
                    tab3 = ribbon.Pages["Ranking"] as RibbonPage;
                }

                if (ribbon.SelectedPage == tab1)
                {
                    if (context == null) context = new Model1();
                    var items = context.Items.ToList();

                    gridView.Columns.Clear();
                    gridControl.DataSource = null;
                    gridControl.DataSource = items;
                }
                else if (ribbon.SelectedPage == tab2)
                {
                    var query = from o in context.Orders
                                join od in context.OrderDetails on o.OrderID equals od.OrderID
                                select new
                                {
                                    od.ID,
                                    o.OrderID,
                                    o.OrderDate,
                                    o.CustID,
                                    CustomerName = o.Customer.CustName,
                                    od.Quantity,
                                    od.UnitAmount,
                                    ItemName = od.Item.ItemName,
                                };

                    var result = query.ToList();



                    gridView.Columns.Clear();

                    gridControl.DataSource = null;
                    gridControl.DataSource = result;
                }
                else if(ribbon.SelectedPage == tab3)
                {
                    gridView.Columns.Clear();
                    gridControl.DataSource = null;

                }
            }

        }

        private void NEW_ItemClick(object sender, ItemClickEventArgs e)
        {
            addOrder addOrder = new addOrder();
            addOrder.Show();
            this.Hide();
        }

        private void Delete_ItemClick(object sender, ItemClickEventArgs e)
        {
            /*using (var context = new Model1())
            {
                var selectedRow = gridView.GetFocusedRow() as Item;
                if (selectedRow != null)
                {
                    var oderDelete = context.OrderDetails.FirstOrDefault(o => o.ID == selectedRow.ID);

                    if (itemToDelete != null)
                    {
                        context.Items.Remove(itemToDelete);

                        context.SaveChanges();

                        var items = context.Items.ToList();
                        gridControl.DataSource = items;
                    }
                }
            }*/
        }

        private void print_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string outputPath = saveFileDialog.FileName;
                ExportGridToPDF(outputPath, gridView);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            register newform = new register();
            newform.Show();
            this.Hide();
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            FilterBestItems();
        }

        private void best_cus_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<Customer> bestCustomers = FindBestCustomers();

            gridView.Columns.Clear();
            gridControl.DataSource = null;
            gridControl.DataSource = bestCustomers;
        }

        public List<Customer> FindBestCustomers()
        {
            List<Customer> bestCustomers;
            bestCustomers = context.Customers
              .Include(c => c.Orders.Select(o => o.OrderDetails))
              .Select(c => new
              {
                  Customer = c,
                  TotalPurchaseAmount = c.Orders.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.UnitAmount))
              })
              .OrderByDescending(result => result.TotalPurchaseAmount)
              .Select(result => result.Customer)
              .ToList();

            return bestCustomers;
        }

        private void FilterBestItems()
        {
            var query = context.OrderDetails
        .GroupBy(od => od.ItemID)
        .Select(group => new
        {
            ItemID = group.Key ?? 0,
            TotalQuantity = group.Sum(od => od.Quantity)
        })
        .OrderByDescending(item => item.TotalQuantity)
        .Take(10)
        .ToList();

            bestSellingItemsList.Clear();

            foreach (var item in query)
            {
                var itemName = context.OrderDetails
                    .Where(od => od.ItemID == item.ItemID)
                    .Select(od => od.Item.ItemName)
                    .FirstOrDefault();

                ItemSummary bestSellingItem = new ItemSummary
                {
                    ItemID = item.ItemID,
                    ItemName = itemName,
                    TotalQuantity = item.TotalQuantity ?? 0
                };

                bestSellingItemsList.Add(bestSellingItem);
            }

            gridView.Columns.Clear();
            gridControl.DataSource = null;
            gridControl.DataSource = bestSellingItemsList;
        }
    }
}
