using System;
using System.Drawing;
using System.Windows.Forms;
using FashionShop.DTO;
using FashionShop.BLL;

namespace FashionShop.GUI
{
    public class FrmMain : Form
    {
        private Account current;
        private ProductService productService = new ProductService();
        private CustomerService customerService = new CustomerService();

        Label lblProducts, lblCustomers, lblRevenue;

        Panel sidebar, topbar, content;
        Button btnHome, btnProducts, btnCustomers, btnOrders, btnLogout;

        public FrmMain(Account acc)
        {
            current = acc;
            Text = "Fashion Shop";
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(245, 246, 250);

            BuildLayout();
            Load += (s, e) => RefreshDashboard();
            Activated += (s, e) => RefreshDashboard();
        }

        private Image ResizeImage(Image img, int w, int h)
        {
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(img, 0, 0, w, h);
            }
            return bmp;
        }


        private void BuildLayout()
        {
            // ===== OUTER SPLIT: 30% Sidebar | 70% Content =====
            var splitOuter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                IsSplitterFixed = true,            // không cho kéo
                FixedPanel = FixedPanel.None,
                BackColor = Color.FromArgb(245, 246, 250)
            };
            Controls.Add(splitOuter);

            // set tỉ lệ 30/70 ban đầu
            splitOuter.SplitterDistance = (int)(splitOuter.Width * 0.10);

            // giữ tỉ lệ khi resize
            this.Resize += (s, e) =>
            {
                splitOuter.SplitterDistance = (int)(splitOuter.Width * 0.10);
            };

            // ===== SIDEBAR nằm trong Panel1 =====
            sidebar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(44, 62, 80)
            };
            splitOuter.Panel1.Controls.Add(sidebar);


            // logo / shop name
            //var lblLogo = new Label
            //{
            //    Text = "FashionShop",
            //    ForeColor = Color.White,
            //    Font = new Font("Segoe UI", 16, FontStyle.Bold),
            //    AutoSize = false,
            //    Height = 60,
            //    Dock = DockStyle.Top,
            //    TextAlign = ContentAlignment.MiddleCenter
            //};
            //sidebar.Controls.Add(lblLogo);

            // ===== USER INFO TOP (tách biệt rõ) =====
            var userPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(12, 14, 12, 10),
                BackColor = Color.FromArgb(52, 73, 94)
            };
            sidebar.Controls.Add(userPanel);

            var lblUser = new Label
            {
                Text = current.EmployeeName,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 14),
                Dock = DockStyle.Top,
                Height = 28
            };
            var lblRole = new Label
            {
                Text = current.Role,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 20
            };

            // ===== MENU WRAP (nằm dưới userPanel) =====
            var menuWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(44, 62, 80)
            };
            sidebar.Controls.Add(menuWrap);
            menuWrap.BringToFront(); // đảm bảo menu nằm dưới userPanel


            // đường kẻ tách khỏi menu
            var sep = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.White
            };

            userPanel.Controls.Add(sep);
            userPanel.Controls.Add(lblRole);
            userPanel.Controls.Add(lblUser);


            // menu buttons
            btnHome = MakeSidebarButton("Home");
            btnProducts = MakeSidebarButton("Products");
            btnCustomers = MakeSidebarButton("Customers");
            btnOrders = MakeSidebarButton("Sales / Orders");

            btnHome.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnProducts.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCustomers.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnOrders.Font = new Font("Segoe UI", 12, FontStyle.Bold);


            // add vào menuWrap theo DockTop
            menuWrap.Controls.Add(btnOrders);
            menuWrap.Controls.Add(btnCustomers);
            menuWrap.Controls.Add(btnProducts);
            menuWrap.Controls.Add(btnHome);


            // ===== LOGOUT dưới cùng + icon =====
            btnLogout = MakeSidebarButton("Logout");
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 55;

            // bỏ dấu cách thừa (để icon không bị mất)
            btnLogout.Text = "  Logout";
            btnLogout.Font = new Font("Segoe UI", 12, FontStyle.Bold);


            // icon 24x24
            btnLogout.Image = ResizeImage(Properties.Resources.logout, 18, 18);
            btnLogout.ImageAlign = ContentAlignment.MiddleLeft;
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.TextImageRelation = TextImageRelation.ImageBeforeText;

            // padding nhẹ thôi
            btnLogout.Padding = new Padding(8, 0, 0, 0);

            sidebar.Controls.Add(btnLogout);


            // events
            btnProducts.Click += (s, e) =>
            {
                new FrmProducts(current).ShowDialog();
                RefreshDashboard();
            };
            btnCustomers.Click += (s, e) =>
            {
                new FrmCustomers().ShowDialog();
                RefreshDashboard();
            };
            btnOrders.Click += (s, e) =>
            {
                new FrmOrders(current).ShowDialog();
                RefreshDashboard();
            };
            btnLogout.Click += (s, e) => Close();

            // ===== TOPBAR =====
            topbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10)
            };
            splitOuter.Panel2.Controls.Add(topbar);

            var lblTitle = new Label
            {
                Text = "Fashion Shop",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true
            };

            //var lblHello = new Label
            //{
            //    Text = $"Hello, {current.EmployeeName} ({current.Role})",
            //    ForeColor = Color.Gray,
            //    Dock = DockStyle.Right,
            //    AutoSize = true,
            //};

            //topbar.Controls.Add(lblHello);
            topbar.Controls.Add(lblTitle);

            // ===== CONTENT AREA =====
            content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                BackColor = Color.FromArgb(245, 246, 250)
            };
            splitOuter.Panel2.Controls.Add(content);
            content.BringToFront(); // để content nằm dưới topbar

            // Row 1 - cards
            var cardsWrap = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = content.BackColor
            };
            content.Controls.Add(cardsWrap);

            // FlowLayoutPanel chứa card
            var cardsRow = new FlowLayoutPanel
            {
                AutoSize = true,                 // quan trọng
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            cardsWrap.Controls.Add(cardsRow);

            // tạo card
            var cardItems = MakeCard("Items", "0", Color.FromArgb(76, 133, 220), out lblProducts);
            var cardCustomers = MakeCard("Customers", "0", Color.FromArgb(244, 186, 63), out lblCustomers);
            var cardRevenue = MakeCard("Revenue", "0 đ", Color.FromArgb(95, 209, 120), out lblRevenue);
            cardsRow.Controls.AddRange(new Control[] { cardItems, cardCustomers, cardRevenue });

            // ===== hàm center cardsRow mỗi khi resize =====
            void CenterCards()
            {
                cardsRow.Left = (cardsWrap.Width - cardsRow.Width) / 2;
                cardsRow.Top = (cardsWrap.Height - cardsRow.Height) / 2; // nếu muốn giữa theo chiều dọc luôn
            }
            cardsWrap.Resize += (s, e) => CenterCards();
            CenterCards();

            // Row 2 - quick links
            //var quickWrap = new GroupBox
            //{
            //    Text = "Quick Links",
            //    Dock = DockStyle.Top,
            //    Height = 170,
            //    Padding = new Padding(12),
            //    BackColor = Color.White
            //};
            //content.Controls.Add(quickWrap);

            //var quickRow = new FlowLayoutPanel
            //{
            //    Dock = DockStyle.Fill,
            //    FlowDirection = FlowDirection.LeftToRight,
            //    WrapContents = true
            //};
            //quickWrap.Controls.Add(quickRow);

            //var qProducts = MakeQuickButton("Open Products", Color.FromArgb(52, 152, 219));
            //var qCustomers = MakeQuickButton("Open Customers", Color.FromArgb(155, 89, 182));
            //var qOrders = MakeQuickButton("Open Sales", Color.FromArgb(230, 126, 34));

            //qProducts.Click += (s, e) => btnProducts.PerformClick();
            //qCustomers.Click += (s, e) => btnCustomers.PerformClick();
            //qOrders.Click += (s, e) => btnOrders.PerformClick();

            //quickRow.Controls.AddRange(new Control[] { qProducts, qCustomers, qOrders });

            // Row 3 - chart placeholder
            // Row 3 - plain panel (không GroupBox, không viền)
            var chartWrap = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };
            content.Controls.Add(chartWrap);

            var lblChart = new Label
            {
                Text = " ",
                Dock = DockStyle.Fill,
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            chartWrap.Controls.Add(lblChart);

        }

        // ====== Refresh dashboard numbers ======
        private void RefreshDashboard()
        {
            try
            {
                int pCount = productService.GetAll().Rows.Count;
                int cCount = customerService.GetAll().Rows.Count;

                lblProducts.Text = pCount.ToString();
                lblCustomers.Text = cCount.ToString();
                lblRevenue.Text = "0 đ";
            }
            catch
            {
                lblProducts.Text = "0";
                lblCustomers.Text = "0";
                lblRevenue.Text = "0 đ";
            }
        }

        // ===== UI helpers =====

        private Button MakeSidebarButton(string text)
        {
            var b = new Button
            {
                Text = "   " + text,
                Dock = DockStyle.Top,
                Height = 48,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 73, 94);
            return b;
        }

        private Panel MakeCard(string title, string value, Color headerColor, out Label lblValue)
        {
            // Card container
            var card = new Panel
            {
                Width = 300,
                Height = 200,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 25, 0),
                Padding = new Padding(0),
            };

            // tạo viền + bóng nhẹ bằng Panel ngoài
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // viền xám rất nhẹ
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    g.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            // Header (màu)
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = headerColor
            };
            card.Controls.Add(header);

            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(30, 30, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblTitle);

            // Body trắng
            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            card.Controls.Add(body);

            // Table 1 ô để center chuẩn
            var bodyTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            bodyTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            bodyTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            body.Controls.Add(bodyTable);

            lblValue = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(30, 30, 30),
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Margin = new Padding(0, 60, 0, 0)   // ✅ lùi xuống 10px
            };


            bodyTable.Controls.Add(lblValue, 0, 0);

            return card;
        }

 
        private Button MakeQuickButton(string text, Color color)
        {
            var b = new Button
            {
                Text = text,
                Width = 180,
                Height = 55,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11),
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}
