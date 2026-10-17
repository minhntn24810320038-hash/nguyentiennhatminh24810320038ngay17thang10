using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaitapWinForms.Models;

namespace BaitapWinForms.Forms
{
    public class FormBai3 : Form
    {
        private BindingList<RepairOrder> _listOrders = new BindingList<RepairOrder>();
        private BindingSource _bindingSource = new BindingSource();
        private ErrorProvider _errorProvider = new ErrorProvider();

        private TextBox txtMaDon, txtTenKH, txtSdt, txtMoTa, txtChiPhi, txtSearch;
        private ComboBox cboLoaiThietBi, cboTrangThai, cboFilterStatus;
        private DateTimePicker dtpNgayNhan;
        private PictureBox picDevice;
        private Button btnBrowseImg, btnThem, btnSua, btnXoa;
        private DataGridView dgvList;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblCount, lblTotalCost;
        private string _selectedImagePath = "";

        public FormBai3()
        {
            InitializeComponentLayout();
            InitDataBinding();
        }

        private void InitializeComponentLayout()
        {
            this.Text = "Repair Service Tracker";
            this.Size = new System.Drawing.Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            menuStrip = new MenuStrip();
            var menuFile = new ToolStripMenuItem("Hệ thống");
            var menuExport = new ToolStripMenuItem("Xuất CSV", null, ExportCsv_Click);
            var menuExit = new ToolStripMenuItem("Thoát", null, (s, e) => this.Close());
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuExport, new ToolStripSeparator(), menuExit });
            menuStrip.Items.Add(menuFile);
            this.MainMenuStrip = menuStrip;

            statusStrip = new StatusStrip();
            lblCount = new ToolStripStatusLabel("Số đơn: 0");
            lblTotalCost = new ToolStripStatusLabel("Tổng chi phí dự kiến (Chưa xong): 0 VNĐ") { ForeColor = Color.Blue };
            statusStrip.Items.AddRange(new ToolStripItem[] { lblCount, new ToolStripStatusLabel(" | "), lblTotalCost });

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 250, Padding = new Padding(10) };

            Label lblMa = new Label { Text = "Mã Đơn:", Location = new Point(15, 15), AutoSize = true };
            txtMaDon = new TextBox { Location = new Point(100, 12), Width = 150 };

            Label lblTen = new Label { Text = "Tên Khách:", Location = new Point(15, 50), AutoSize = true };
            txtTenKH = new TextBox { Location = new Point(100, 47), Width = 150 };

            Label lblSdt = new Label { Text = "Số Điện Thoại:", Location = new Point(15, 85), AutoSize = true };
            txtSdt = new TextBox { Location = new Point(100, 82), Width = 150 };

            Label lblLoai = new Label { Text = "Loại Thiết Bị:", Location = new Point(270, 15), AutoSize = true };
            cboLoaiThietBi = new ComboBox { Location = new Point(360, 12), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoaiThietBi.Items.AddRange(new object[] { "Điện thoại", "Laptop", "Máy tính bảng", "Khác" });
            cboLoaiThietBi.SelectedIndex = 0;

            Label lblNgay = new Label { Text = "Ngày Nhận:", Location = new Point(270, 50), AutoSize = true };
            dtpNgayNhan = new DateTimePicker { Location = new Point(360, 47), Format = DateTimePickerFormat.Short, Width = 150 };

            Label lblChiPhi = new Label { Text = "Chi Phí DK:", Location = new Point(270, 85), AutoSize = true };
            txtChiPhi = new TextBox { Location = new Point(360, 82), Width = 150 };

            Label lblTrangThai = new Label { Text = "Trạng Thái:", Location = new Point(270, 120), AutoSize = true };
            cboTrangThai = new ComboBox { Location = new Point(360, 117), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThai.Items.AddRange(new object[] { "Mới tiếp nhận", "Đang sửa", "Chờ linh kiện", "Hoàn tất", "Đã hủy" });
            cboTrangThai.SelectedIndex = 0;

            Label lblMoTa = new Label { Text = "Mô Tả Lỗi:", Location = new Point(15, 120), AutoSize = true };
            txtMoTa = new TextBox { Location = new Point(100, 117), Width = 150, Multiline = true, Height = 45 };

            picDevice = new PictureBox
            {
                Location = new Point(540, 12),
                Size = new Size(160, 130),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnBrowseImg = new Button { Text = "Chọn Ảnh", Location = new Point(540, 148), Width = 160 };
            btnBrowseImg.Click += BtnBrowseImg_Click;

            btnThem = new Button { Text = "Thêm Đơn", Location = new Point(15, 180), Width = 90 };
            btnSua = new Button { Text = "Cập Nhật", Location = new Point(115, 180), Width = 90 };
            btnXoa = new Button { Text = "Xóa Đơn", Location = new Point(215, 180), Width = 90 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;

            Label lblSearch = new Label { Text = "Tìm kiếm (Mã/Tên/SĐT):", Location = new Point(320, 184), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(450, 181), Width = 150 };
            txtSearch.TextChanged += FilterData;

            Label lblFStatus = new Label { Text = "Lọc:", Location = new Point(610, 184), AutoSize = true };
            cboFilterStatus = new ComboBox { Location = new Point(645, 181), Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterStatus.Items.AddRange(new object[] { "Tất cả", "Mới tiếp nhận", "Đang sửa", "Chờ linh kiện", "Hoàn tất", "Đã hủy" });
            cboFilterStatus.SelectedIndex = 0;
            cboFilterStatus.SelectedIndexChanged += FilterData;

            pnlTop.Controls.AddRange(new Control[] {
                lblMa, txtMaDon, lblTen, txtTenKH, lblSdt, txtSdt,
                lblLoai, cboLoaiThietBi, lblNgay, dtpNgayNhan, lblChiPhi, txtChiPhi,
                lblTrangThai, cboTrangThai, lblMoTa, txtMoTa,
                picDevice, btnBrowseImg, btnThem, btnSua, btnXoa,
                lblSearch, txtSearch, lblFStatus, cboFilterStatus
            });

            dgvList = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false
            };

            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaDon", HeaderText = "Mã Đơn", Width = 90 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenKhachHang", HeaderText = "Khách Hàng", Width = 130 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoDienThoai", HeaderText = "SĐT", Width = 100 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LoaiThietBi", HeaderText = "Thiết Bị", Width = 100 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MoTaLoi", HeaderText = "Mô Tả Lỗi", Width = 150 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayTiepNhan", HeaderText = "Ngày Nhận", Width = 100 });

            var colCost = new DataGridViewTextBoxColumn { DataPropertyName = "ChiPhiDuKien", HeaderText = "Chi Phí DK", Width = 110 };
            colCost.DefaultCellStyle.Format = "N0";
            dgvList.Columns.Add(colCost);

            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng Thái", Width = 110 });

            dgvList.SelectionChanged += DgvList_SelectionChanged;

            this.Controls.Add(dgvList);
            this.Controls.Add(pnlTop);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);
        }

        private void InitDataBinding()
        {
            _bindingSource.DataSource = _listOrders;
            dgvList.DataSource = _bindingSource;

            _listOrders.Add(new RepairOrder { MaDon = "DON001", TenKhachHang = "Lê Văn E", SoDienThoai = "0912345678", LoaiThietBi = "Laptop", MoTaLoi = "Hỏng bàn phím", NgayTiepNhan = DateTime.Today, ChiPhiDuKien = 500000, TrangThai = "Mới tiếp nhận" });
            _listOrders.Add(new RepairOrder { MaDon = "DON002", TenKhachHang = "Phạm Thị F", SoDienThoai = "0988888888", LoaiThietBi = "Điện thoại", MoTaLoi = "Vỡ màn hình", NgayTiepNhan = DateTime.Today.AddDays(-1), ChiPhiDuKien = 1200000, TrangThai = "Đang sửa" });

            FilterData(null, null);
        }

        private void BtnBrowseImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedImagePath = ofd.FileName;
                    picDevice.Image = Image.FromFile(_selectedImagePath);
                }
            }
        }

        private bool ValidateData(bool isNew)
        {
            _errorProvider.Clear();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtMaDon.Text))
            {
                _errorProvider.SetError(txtMaDon, "Mã đơn không được trống!");
                valid = false;
            }
            else if (isNew && _listOrders.Any(x => x.MaDon.Equals(txtMaDon.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _errorProvider.SetError(txtMaDon, "Mã đơn bị trùng!");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                _errorProvider.SetError(txtTenKH, "Tên khách hàng bắt buộc nhập!");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtSdt.Text))
            {
                _errorProvider.SetError(txtSdt, "SĐT bắt buộc nhập!");
                valid = false;
            }

            if (!decimal.TryParse(txtChiPhi.Text, out decimal cost) || cost < 0)
            {
                _errorProvider.SetError(txtChiPhi, "Chi phí phải là số không âm!");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtMoTa.Text))
            {
                _errorProvider.SetError(txtMoTa, "Mô tả lỗi bắt buộc nhập!");
                valid = false;
            }

            return valid;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateData(true)) return;

            var order = new RepairOrder
            {
                MaDon = txtMaDon.Text.Trim(),
                TenKhachHang = txtTenKH.Text.Trim(),
                SoDienThoai = txtSdt.Text.Trim(),
                LoaiThietBi = cboLoaiThietBi.SelectedItem.ToString(),
                MoTaLoi = txtMoTa.Text.Trim(),
                NgayTiepNhan = dtpNgayNhan.Value,
                ChiPhiDuKien = decimal.Parse(txtChiPhi.Text),
                TrangThai = cboTrangThai.SelectedItem.ToString(),
                HinhAnhPath = _selectedImagePath
            };

            _listOrders.Add(order);
            FilterData(null, null);
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            var current = (RepairOrder)dgvList.CurrentRow.DataBoundItem;

            if (!ValidateData(false)) return;

            if (MessageBox.Show("Xác nhận cập nhật đơn sửa chữa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                current.TenKhachHang = txtTenKH.Text.Trim();
                current.SoDienThoai = txtSdt.Text.Trim();
                current.LoaiThietBi = cboLoaiThietBi.SelectedItem.ToString();
                current.MoTaLoi = txtMoTa.Text.Trim();
                current.NgayTiepNhan = dtpNgayNhan.Value;
                current.ChiPhiDuKien = decimal.Parse(txtChiPhi.Text);
                current.TrangThai = cboTrangThai.SelectedItem.ToString();
                current.HinhAnhPath = _selectedImagePath;

                _bindingSource.ResetBindings(false);
                FilterData(null, null);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            var current = (RepairOrder)dgvList.CurrentRow.DataBoundItem;

            if (MessageBox.Show("Xác nhận xóa đơn sửa chữa này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _listOrders.Remove(current);
                FilterData(null, null);
            }
        }

        private void DgvList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow != null && dgvList.CurrentRow.DataBoundItem is RepairOrder item)
            {
                txtMaDon.Text = item.MaDon;
                txtTenKH.Text = item.TenKhachHang;
                txtSdt.Text = item.SoDienThoai;
                cboLoaiThietBi.SelectedItem = item.LoaiThietBi;
                txtMoTa.Text = item.MoTaLoi;
                dtpNgayNhan.Value = item.NgayTiepNhan;
                txtChiPhi.Text = item.ChiPhiDuKien.ToString("0");
                cboTrangThai.SelectedItem = item.TrangThai;
                _selectedImagePath = item.HinhAnhPath;

                if (!string.IsNullOrEmpty(_selectedImagePath) && File.Exists(_selectedImagePath))
                {
                    picDevice.Image = Image.FromFile(_selectedImagePath);
                }
                else
                {
                    picDevice.Image = null;
                }
            }
        }

        private void FilterData(object sender, EventArgs e)
        {
            var query = _listOrders.AsEnumerable();

            string search = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MaDon.ToLower().Contains(search) ||
                                         x.TenKhachHang.ToLower().Contains(search) ||
                                         x.SoDienThoai.Contains(search));
            }

            if (cboFilterStatus.SelectedIndex > 0)
            {
                string status = cboFilterStatus.SelectedItem.ToString();
                query = query.Where(x => x.TrangThai == status);
            }

            var list = query.ToList();
            _bindingSource.DataSource = list;

            lblCount.Text = $"Số đơn: {list.Count}";
            decimal pendingCost = _listOrders.Where(x => x.TrangThai != "Hoàn tất" && x.TrangThai != "Đã hủy").Sum(x => x.ChiPhiDuKien);
            lblTotalCost.Text = $"Tổng chi phí dự kiến (Chưa xong): {pendingCost:N0} VNĐ";
        }

        private void ExportCsv_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV File (*.csv)|*.csv", FileName = "DanhSachSuaChua.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Mã Đơn,Khách Hàng,Số Điện Thoại,Loại Thiết Bị,Mô Tả Lỗi,Ngày Nhận,Chi Phí DK,Trạng Thái");

                    foreach (var item in (System.Collections.IList)dgvList.DataSource)
                    {
                        var o = (RepairOrder)item;
                        sb.AppendLine($"\"{o.MaDon}\",\"{o.TenKhachHang}\",\"{o.SoDienThoai}\",\"{o.LoaiThietBi}\",\"{o.MoTaLoi}\",\"{o.NgayTiepNhan:dd/MM/yyyy}\",\"{o.ChiPhiDuKien:N0}\",\"{o.TrangThai}\"");
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo");
                }
            }
        }
    }
}