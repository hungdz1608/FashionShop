using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FashionShop.BLL;
using FashionShop.DTO;

namespace FashionShop.GUI
{
    public class FrmProducts : Form
    {
        ProductService service = new ProductService();
        private Account current;

        DataGridView dgv;

        TextBox txtCode, txtName, txtSize, txtColor, txtSearch;
        ComboBox cboCategory, cboGender;
        NumericUpDown nudPrice, nudStock;

        Button btnAdd, btnUpd, btnDel, btnReload, btnSearch;

        public FrmProducts(Account acc)
        {
            current = acc;

            // ===== Form base =====
            Text = "Products Management";
            MinimumSize = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10f);

            // ===== Root SplitContainer =====
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.None,
                BackColor = Color.White
            };
            Controls.Add(split);




            // ================= LEFT: Input =================
            split.Panel1.Padding = new Padding(12);

            var gbInput = new GroupBox
            {
                Text = "Product Information",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 10.5f),
                Padding = new Padding(12)
            };

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                ColumnCount = 2,
                RowCount = 0,
                Padding = new Padding(6),
            };

            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); // label rộng cố định
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // input fill


            // row helper
            int _row = 0;
            void AddRow(string label, Control control)
            {
                tbl.RowCount = _row + 1; // <<< tăng số row thật sự

                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));

                var lb = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(0, 2, 0, 2);

                tbl.Controls.Add(lb, 0, _row);
                tbl.Controls.Add(control, 1, _row);

                _row++;
            }

            txtCode = new TextBox();
            txtName = new TextBox();

            cboCategory = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboGender = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboGender.Items.AddRange(new object[] { "Men", "Women", "Unisex" });
            cboGender.SelectedIndex = 0;

            txtSize = new TextBox();
            txtColor = new TextBox();

            nudPrice = new NumericUpDown
            {
                Maximum = 1000000000,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            nudStock = new NumericUpDown
            {
                Maximum = 1000000,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            AddRow("Code", txtCode);
            AddRow("Name", txtName);
            AddRow("Category", cboCategory);
            AddRow("Gender", cboGender);
            AddRow("Size", txtSize);
            AddRow("Color", txtColor);
            AddRow("Price", nudPrice);
            AddRow("Stock", nudStock);

            // Buttons panel
            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.LeftToRight,
                Height = 90,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnAdd = MakeButton("Add", Color.FromArgb(33, 150, 243));
            btnUpd = MakeButton("Update", Color.FromArgb(76, 175, 80));
            btnDel = MakeButton("Delete", Color.FromArgb(244, 67, 54));
            btnReload = MakeButton("Reload", Color.FromArgb(96, 125, 139));

            btnAdd.Click += BtnAdd_Click;
            btnUpd.Click += BtnUpd_Click;
            btnDel.Click += BtnDel_Click;
            btnReload.Click += (s, e) => LoadGrid();

            pnlButtons.Controls.AddRange(new Control[] { btnAdd, btnUpd, btnDel, btnReload });

            gbInput.Controls.Add(tbl);
            gbInput.Controls.Add(pnlButtons);

            split.Panel1.Controls.Add(gbInput);

            // ================= RIGHT: Search + Grid =================
            split.Panel2.Padding = new Padding(12);

            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46
            };

            txtSearch = new TextBox
            {
                Width = 350,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            btnSearch = MakeButton("Search", Color.FromArgb(63, 81, 181));
            btnSearch.Width = 100;
            btnSearch.Height = 34;
            btnSearch.Margin = new Padding(8, 0, 0, 0);

            btnSearch.Click += (s, e) =>
                dgv.DataSource = service.Search(txtSearch.Text.Trim());

            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnSearch);
            txtSearch.Location = new Point(0, 8);
            btnSearch.Location = new Point(txtSearch.Right + 8, 6);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
            };
            dgv.CellClick += Dgv_CellClick;

            StyleGrid(dgv);

            split.Panel2.Controls.Add(dgv);
            split.Panel2.Controls.Add(pnlSearch);

            Shown += (s, e) =>
            {
                split.Panel1MinSize = 320;
                split.Panel2MinSize = 600;

                // đảm bảo splitter distance hợp lệ theo width hiện tại
                int desiredLeft = 340;
                int maxLeft = split.Width - split.Panel2MinSize;

                split.SplitterDistance = Math.Max(split.Panel1MinSize,
                                          Math.Min(desiredLeft, maxLeft));
            };


            // ===== Load data =====
            Load += (s, e) =>
            {
                cboCategory.DataSource = service.GetCategories();
                cboCategory.DisplayMember = "category_name";
                cboCategory.ValueMember = "category_id";

                LoadGrid();
                ApplyRolePermission();
            };
        }

        public FrmProducts() : this(null) { }

        // ================= UI STYLE HELPERS =================
        Button MakeButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 38,
                Width = 95,
                Font = new Font("Segoe UI Semibold", 10f),
                Cursor = Cursors.Hand,
                Margin = new Padding(6, 0, 6, 0)
            };
        }

        void StyleGrid(DataGridView g)
        {
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10f);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            g.ColumnHeadersHeight = 38;

            g.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
        }

        // ================= PERMISSION =================
        private void ApplyRolePermission()
        {
            if (current == null) return;

            bool isStaff = current.Role != null &&
                           current.Role.Equals("Staff", StringComparison.OrdinalIgnoreCase);

            if (isStaff)
            {
                btnAdd.Enabled = false;
                btnUpd.Enabled = false;
                btnDel.Enabled = false;

                txtCode.ReadOnly = true;
                txtName.ReadOnly = true;
                txtSize.ReadOnly = true;
                txtColor.ReadOnly = true;
                nudPrice.Enabled = false;
                nudStock.Enabled = false;
                cboCategory.Enabled = false;
                cboGender.Enabled = false;

                Text += " (View only)";
            }
        }

        void LoadGrid() => dgv.DataSource = service.GetAll();

        Product ReadForm()
        {
            return new Product
            {
                Code = txtCode.Text.Trim(),
                Name = txtName.Text.Trim(),
                CategoryId = Convert.ToInt32(cboCategory.SelectedValue),
                Gender = cboGender.Text,
                Size = txtSize.Text.Trim(),
                Color = txtColor.Text.Trim(),
                Price = nudPrice.Value,
                Stock = (int)nudStock.Value
            };
        }

        // ================= ACTIONS =================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var p = ReadForm();
                if (service.Add(p, out string err))
                {
                    MessageBox.Show("Added!");
                    LoadGrid();
                }
                else MessageBox.Show(err);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid data: " + ex.Message);
            }
        }

        private void BtnUpd_Click(object sender, EventArgs e)
        {
            try
            {
                var p = ReadForm();
                if (service.Update(p, out string err))
                {
                    MessageBox.Show("Updated!");
                    LoadGrid();
                }
                else MessageBox.Show(err);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid data: " + ex.Message);
            }
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text)) return;
            if (MessageBox.Show("Delete this product?", "Confirm", MessageBoxButtons.YesNo)
                == DialogResult.Yes)
            {
                service.Delete(txtCode.Text.Trim());
                LoadGrid();
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var r = dgv.Rows[e.RowIndex];

            txtCode.Text = r.Cells["product_code"].Value?.ToString();
            txtName.Text = r.Cells["product_name"].Value?.ToString();
            txtSize.Text = r.Cells["size"].Value?.ToString();
            txtColor.Text = r.Cells["color"].Value?.ToString();
            cboGender.Text = r.Cells["gender"].Value?.ToString();

            if (decimal.TryParse(r.Cells["price"].Value?.ToString(), out var price))
                nudPrice.Value = Math.Min(nudPrice.Maximum, price);

            if (int.TryParse(r.Cells["stock"].Value?.ToString(), out var stock))
                nudStock.Value = Math.Min(nudStock.Maximum, stock);
        }
    }
}
