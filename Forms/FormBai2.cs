using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaitapWinForms.Models;

namespace BaitapWinForms.Forms
{
    public class FormBai2 : Form
    {
        private BindingList<LabCheckout> _listCheckouts = new BindingList<LabCheckout>();
        private BindingSource _bindingSource = new BindingSource();
        private ErrorProvider _errorProvider = new ErrorProvider();

        private TextBox txtMaPhieu, txtMaSV, txtHoTenSV, txtSearch;
        private ComboBox cboThietBi, cboFilterStatus;
        private NumericUpDown nudSoLuong;
        private DateTimePicker dtpNgayMuon, dtpHanTra, dtpNgayTraThucTe;
        private CheckBox chkDaTra;
        private DataGridView dgvList;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblTotal, lblOverdue;

        public FormBai2()
        {
            InitializeComponentLayout();
            InitDataBinding();
        }

        private void InitializeComponentLayout()
        {
            this.Text = "Lab Equipment Checkout Manager";
            this.Size = new System.Drawing.Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            Panel pnlInput = new Panel { Dock = DockStyle.Top, Height = 210, Padding = new Padding(10) };
            
            Label lblMaPhieu = new Label { Text = "Mã Phiếu:", Location = new System.Drawing.Point(20, 15), AutoSize = true };
            txtMaPhieu = new TextBox { Location = new System.Drawing.Point(110, 12), Width = 150 };

            Label lblMaSV = new Label { Text = "Mã SV:", Location = new System.Drawing.Point(280, 15), AutoSize = true };
            txtMaSV = new TextBox { Location = new System.Drawing.Point(360, 12), Width = 150 };

            Label lblHoTen = new Label { Text = "Họ Tên SV:", Location = new System.Drawing.Point(530, 15), AutoSize = true };
            txtHoTenSV = new TextBox { Location = new System.Drawing.Point(620, 12), Width = 200 };

            Label lblThietBi = new Label { Text = "Thiết Bị:", Location = new System.Drawing.Point(20, 50), AutoSize = true };
            cboThietBi = new ComboBox { Location = new System.Drawing.Point(110, 47), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboThietBi.Items.AddRange(new object[] { "Kính hiển vi", "Máy dao động số", "Bộ Kit Arduino", "Cảm biến nhiệt độ" });
            cboThietBi.SelectedIndex = 0;

            Label lblSoLuong = new Label { Text = "Số Lượng:", Location = new System.Drawing.Point(280, 50), AutoSize = true };
            nudSoLuong = new NumericUpDown { Location = new System.Drawing.Point(360, 48), Width = 150, Minimum = 1, Maximum = 100, Value = 1 };

            Label lblNgayMuon = new Label { Text = "Ngày Mượn:", Location = new System.Drawing.Point(20, 85), AutoSize = true };
            dtpNgayMuon = new DateTimePicker { Location = new System.Drawing.Point(110, 82), Format = DateTimePickerFormat.Short, Width = 150 };

            Label lblHanTra = new Label { Text = "Hạn Trả:", Location = new System.Drawing.Point(280, 85), AutoSize = true };
            dtpHanTra = new DateTimePicker { Location = new System.Drawing.Point(360, 82), Format = DateTimePickerFormat.Short, Width = 150 };

            chkDaTra = new CheckBox { Text = "Đánh dấu đã trả", Location = new System.Drawing.Point(530, 84), AutoSize = true };
            chkDaTra.CheckedChanged += ChkDaTra_CheckedChanged;

            Label lblNgayTra = new Label { Text = "Ngày Trả TT:", Location = new System.Drawing.Point(670, 85), AutoSize = true };
            dtpNgayTraThucTe = new DateTimePicker { Location = new System.Drawing.Point(760, 82), Format = DateTimePickerFormat.Short, Width = 130, Enabled = false };

            Button btnThem = new Button { Text = "Thêm Phiếu", Location = new System.Drawing.Point(20, 125), Width = 100 };
            Button btnSua = new Button { Text = "Cập Nhật", Location = new System.Drawing.Point(130, 125), Width = 100 };
            Button btnXoa = new Button { Text = "Xóa Phiếu", Location = new System.Drawing.Point(240, 125), Width = 100 };
            Button btnExport = new Button { Text = "Xuất CSV", Location = new System.Drawing.Point(350, 125), Width = 100 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnExport.Click += BtnExport_Click;

            Label lblSearch = new Label { Text = "Tìm kiếm (Mã SV / Thiết bị):", Location = new System.Drawing.Point(20, 172), AutoSize = true };
            txtSearch = new TextBox { Location = new System.Drawing.Point(200, 169), Width = 200 };
            txtSearch.TextChanged += FilterData;

            Label lblFilter = new Label { Text = "Lọc trạng thái:", Location = new System.Drawing.Point(430, 172), AutoSize = true };
            cboFilterStatus = new ComboBox { Location = new System.Drawing.Point(530, 169), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterStatus.Items.AddRange(new object[] { "Tất cả", "Đang mượn", "Quá hạn", "Đã trả" });
            cboFilterStatus.SelectedIndex = 0;
            cboFilterStatus.SelectedIndexChanged += FilterData;

            pnlInput.Controls.AddRange(new Control[] {
                lblMaPhieu, txtMaPhieu, lblMaSV, txtMaSV, lblHoTen, txtHoTenSV,
                lblThietBi, cboThietBi, lblSoLuong, nudSoLuong,
                lblNgayMuon, dtpNgayMuon, lblHanTra, dtpHanTra, chkDaTra, lblNgayTra, dtpNgayTraThucTe,
                btnThem, btnSua, btnXoa, btnExport,
                lblSearch, txtSearch, lblFilter, cboFilterStatus
            });

            dgvList = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false
            };

            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaPhieu", HeaderText = "Mã Phiếu", Width = 90 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSinhVien", HeaderText = "Mã SV", Width = 90 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTenSinhVien", HeaderText = "Họ Tên SV", Width = 140 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ThietBi", HeaderText = "Thiết Bị", Width = 140 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuong", HeaderText = "SL", Width = 50 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayMuon", HeaderText = "Ngày Mượn", Width = 100 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HanTra", HeaderText = "Hạn Trả", Width = 100 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayTraThucTe", HeaderText = "Ngày Trả TT", Width = 100 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng Thái", Width = 100 });

            dgvList.SelectionChanged += DgvList_SelectionChanged;

            statusStrip = new StatusStrip();
            lblTotal = new ToolStripStatusLabel("Tổng số phiếu: 0");
            lblOverdue = new ToolStripStatusLabel("Số phiếu quá hạn: 0") { ForeColor = System.Drawing.Color.Red };
            statusStrip.Items.AddRange(new ToolStripItem[] { lblTotal, new ToolStripStatusLabel(" | "), lblOverdue });

            this.Controls.Add(dgvList);
            this.Controls.Add(pnlInput);
            this.Controls.Add(statusStrip);
        }

        private void InitDataBinding()
        {
            _bindingSource.DataSource = _listCheckouts;
            dgvList.DataSource = _bindingSource;

            _listCheckouts.Add(new LabCheckout { MaPhieu = "P001", MaSinhVien = "SV101", HoTenSinhVien = "Nguyễn Văn C", ThietBi = "Kính hiển vi", SoLuong = 1, NgayMuon = DateTime.Today.AddDays(-5), HanTra = DateTime.Today.AddDays(-1), NgayTraThucTe = null });
            _listCheckouts.Add(new LabCheckout { MaPhieu = "P002", MaSinhVien = "SV102", HoTenSinhVien = "Lê Thị D", ThietBi = "Bộ Kit Arduino", SoLuong = 2, NgayMuon = DateTime.Today, HanTra = DateTime.Today.AddDays(3), NgayTraThucTe = null });

            FilterData(null, null);
        }

        private void ChkDaTra_CheckedChanged(object sender, EventArgs e)
        {
            dtpNgayTraThucTe.Enabled = chkDaTra.Checked;
            if (chkDaTra.Checked)
            {
                dtpNgayTraThucTe.Value = DateTime.Today;
            }
        }

        private bool ValidateData(bool isNew)
        {
            _errorProvider.Clear();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtMaPhieu.Text))
            {
                _errorProvider.SetError(txtMaPhieu, "Mã phiếu không được trống");
                valid = false;
            }
            else if (isNew && _listCheckouts.Any(x => x.MaPhieu.Equals(txtMaPhieu.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _errorProvider.SetError(txtMaPhieu, "Mã phiếu trùng lặp");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                _errorProvider.SetError(txtMaSV, "Mã sinh viên không được trống");
                valid = false;
            }

            if (dtpHanTra.Value.Date < dtpNgayMuon.Value.Date)
            {
                _errorProvider.SetError(dtpHanTra, "Hạn trả không được trước ngày mượn");
                valid = false;
            }

            if (chkDaTra.Checked && dtpNgayTraThucTe.Value.Date < dtpNgayMuon.Value.Date)
            {
                _errorProvider.SetError(dtpNgayTraThucTe, "Ngày trả thực tế không trước ngày mượn");
                valid = false;
            }

            return valid;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateData(true)) return;

            var item = new LabCheckout
            {
                MaPhieu = txtMaPhieu.Text.Trim(),
                MaSinhVien = txtMaSV.Text.Trim(),
                HoTenSinhVien = txtHoTenSV.Text.Trim(),
                ThietBi = cboThietBi.SelectedItem.ToString(),
                SoLuong = (int)nudSoLuong.Value,
                NgayMuon = dtpNgayMuon.Value.Date,
                HanTra = dtpHanTra.Value.Date,
                NgayTraThucTe = chkDaTra.Checked ? dtpNgayTraThucTe.Value.Date : (DateTime?)null
            };

            _listCheckouts.Add(item);
            FilterData(null, null);
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            var current = (LabCheckout)dgvList.CurrentRow.DataBoundItem;

            if (!ValidateData(false)) return;

            if (MessageBox.Show("Xác nhận cập nhật phiếu mượn?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                current.MaSinhVien = txtMaSV.Text.Trim();
                current.HoTenSinhVien = txtHoTenSV.Text.Trim();
                current.ThietBi = cboThietBi.SelectedItem.ToString();
                current.SoLuong = (int)nudSoLuong.Value;
                current.NgayMuon = dtpNgayMuon.Value.Date;
                current.HanTra = dtpHanTra.Value.Date;
                current.NgayTraThucTe = chkDaTra.Checked ? dtpNgayTraThucTe.Value.Date : (DateTime?)null;

                _bindingSource.ResetBindings(false);
                FilterData(null, null);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            var current = (LabCheckout)dgvList.CurrentRow.DataBoundItem;

            if (MessageBox.Show("Xác nhận xóa phiếu mượn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _listCheckouts.Remove(current);
                FilterData(null, null);
            }
        }

        private void DgvList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow != null && dgvList.CurrentRow.DataBoundItem is LabCheckout item)
            {
                txtMaPhieu.Text = item.MaPhieu;
                txtMaSV.Text = item.MaSinhVien;
                txtHoTenSV.Text = item.HoTenSinhVien;
                cboThietBi.SelectedItem = item.ThietBi;
                nudSoLuong.Value = item.SoLuong;
                dtpNgayMuon.Value = item.NgayMuon;
                dtpHanTra.Value = item.HanTra;
                
                if (item.NgayTraThucTe.HasValue)
                {
                    chkDaTra.Checked = true;
                    dtpNgayTraThucTe.Value = item.NgayTraThucTe.Value;
                }
                else
                {
                    chkDaTra.Checked = false;
                }
            }
        }

        private void FilterData(object sender, EventArgs e)
        {
            var query = _listCheckouts.AsEnumerable();

            string search = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MaSinhVien.ToLower().Contains(search) || x.ThietBi.ToLower().Contains(search));
            }

            if (cboFilterStatus.SelectedIndex > 0)
            {
                string status = cboFilterStatus.SelectedItem.ToString();
                query = query.Where(x => x.TrangThai == status);
            }

            var list = query.ToList();
            _bindingSource.DataSource = list;

            lblTotal.Text = $"Tổng số phiếu: {_listCheckouts.Count}";
            lblOverdue.Text = $"Số phiếu quá hạn: {_listCheckouts.Count(x => x.TrangThai == "Quá hạn")}";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV File (*.csv)|*.csv", FileName = "PhieuMuonThietBi.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Mã Phiếu,Mã SV,Họ Tên SV,Thiết Bị,Số Lượng,Ngày Mượn,Hạn Trả,Ngày Trả TT,Trạng Thái");

                    foreach (var item in (System.Collections.IList)dgvList.DataSource)
                    {
                        var c = (LabCheckout)item;
                        sb.AppendLine($"\"{c.MaPhieu}\",\"{c.MaSinhVien}\",\"{c.HoTenSinhVien}\",\"{c.ThietBi}\",\"{c.SoLuong}\",\"{c.NgayMuon:dd/MM/yyyy}\",\"{c.HanTra:dd/MM/yyyy}\",\"{c.NgayTraThucTe:dd/MM/yyyy}\",\"{c.TrangThai}\"");
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo");
                }
            }
        }
    }
}