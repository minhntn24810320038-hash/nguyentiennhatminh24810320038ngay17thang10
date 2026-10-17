using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BaitapWinForms.Models;

namespace BaitapWinForms.Forms
{
    public class FormBai1 : Form
    {
        private BindingList<Appointment> _listAppointments = new BindingList<Appointment>();
        private BindingSource _bindingSource = new BindingSource();
        private ErrorProvider _errorProvider = new ErrorProvider();

        private TableLayoutPanel tlpInput;
        private DataGridView dgvList;
        private TextBox txtMa, txtHoTen, txtSdt, txtLyDo, txtSearch;
        private DateTimePicker dtpNgayGio, dtpFilterDate;
        private ComboBox cboBacSi, cboTrangThai, cboFilterStatus;
        private Button btnThem, btnSua, btnXoa;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatusCount;
        private CheckBox chkEnableDateFilter;

        public FormBai1()
        {
            InitializeComponentLayout();
            InitDataBinding();
        }

        private void InitializeComponentLayout()
        {
            this.Text = "Clinic Appointment Manager";
            this.Size = new System.Drawing.Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            menuStrip = new MenuStrip();
            var menuFile = new ToolStripMenuItem("Tệp tin");
            var menuExport = new ToolStripMenuItem("Xuất CSV", null, ExportToCsv_Click);
            menuFile.DropDownItems.Add(menuExport);
            menuStrip.Items.Add(menuFile);
            this.MainMenuStrip = menuStrip;

            statusStrip = new StatusStrip();
            lblStatusCount = new ToolStripStatusLabel("Số lịch hẹn: 0");
            statusStrip.Items.Add(lblStatusCount);

            Panel pnlFilter = new Panel { Dock = DockStyle.Top, Height = 45 };
            Label lblSearch = new Label { Text = "Tìm kiếm:", Location = new System.Drawing.Point(10, 12), AutoSize = true };
            txtSearch = new TextBox { Location = new System.Drawing.Point(75, 10), Width = 150 };
            txtSearch.TextChanged += FilterData;

            chkEnableDateFilter = new CheckBox { Text = "Lọc ngày:", Location = new System.Drawing.Point(240, 12), AutoSize = true };
            chkEnableDateFilter.CheckedChanged += FilterData;
            dtpFilterDate = new DateTimePicker { Location = new System.Drawing.Point(320, 10), Format = DateTimePickerFormat.Short, Width = 110 };
            dtpFilterDate.ValueChanged += FilterData;

            Label lblFStatus = new Label { Text = "Trạng thái:", Location = new System.Drawing.Point(445, 12), AutoSize = true };
            cboFilterStatus = new ComboBox { Location = new System.Drawing.Point(515, 10), DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            cboFilterStatus.Items.AddRange(new object[] { "Tất cả", "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
            cboFilterStatus.SelectedIndex = 0;
            cboFilterStatus.SelectedIndexChanged += FilterData;

            pnlFilter.Controls.AddRange(new Control[] { lblSearch, txtSearch, chkEnableDateFilter, dtpFilterDate, lblFStatus, cboFilterStatus });

            tlpInput = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 180,
                ColumnCount = 4,
                RowCount = 4,
                Padding = new Padding(10)
            };
            tlpInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            txtMa = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            txtHoTen = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            txtSdt = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            dtpNgayGio = new DateTimePicker { CustomFormat = "dd/MM/yyyy HH:mm", Format = DateTimePickerFormat.Custom, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            
            cboBacSi = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            cboBacSi.Items.AddRange(new object[] { "BS. Nguyễn Văn A", "BS. Trần Thị B", "BS. Lê Văn C" });
            cboBacSi.SelectedIndex = 0;

            cboTrangThai = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            cboTrangThai.Items.AddRange(new object[] { "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
            cboTrangThai.SelectedIndex = 0;

            txtLyDo = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Multiline = true, Height = 40 };

            tlpInput.Controls.Add(new Label { Text = "Mã lịch hẹn:", Anchor = AnchorStyles.Left }, 0, 0);
            tlpInput.Controls.Add(txtMa, 1, 0);
            tlpInput.Controls.Add(new Label { Text = "Bác sĩ:", Anchor = AnchorStyles.Left }, 2, 0);
            tlpInput.Controls.Add(cboBacSi, 3, 0);

            tlpInput.Controls.Add(new Label { Text = "Họ tên BN:", Anchor = AnchorStyles.Left }, 0, 1);
            tlpInput.Controls.Add(txtHoTen, 1, 1);
            tlpInput.Controls.Add(new Label { Text = "Ngày giờ hẹn:", Anchor = AnchorStyles.Left }, 2, 1);
            tlpInput.Controls.Add(dtpNgayGio, 3, 1);

            tlpInput.Controls.Add(new Label { Text = "Số điện thoại:", Anchor = AnchorStyles.Left }, 0, 2);
            tlpInput.Controls.Add(txtSdt, 1, 2);
            tlpInput.Controls.Add(new Label { Text = "Trạng thái:", Anchor = AnchorStyles.Left }, 2, 2);
            tlpInput.Controls.Add(cboTrangThai, 3, 2);

            tlpInput.Controls.Add(new Label { Text = "Lý do khám:", Anchor = AnchorStyles.Left }, 0, 3);
            tlpInput.Controls.Add(txtLyDo, 1, 3);

            Panel pnlButtons = new Panel { Dock = DockStyle.Top, Height = 45 };
            btnThem = new Button { Text = "Thêm", Location = new System.Drawing.Point(15, 8), Width = 80 };
            btnSua = new Button { Text = "Sửa", Location = new System.Drawing.Point(105, 8), Width = 80 };
            btnXoa = new Button { Text = "Xóa", Location = new System.Drawing.Point(195, 8), Width = 80 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa });

            dgvList = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false
            };

            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaLichHen", HeaderText = "Mã Lịch Hẹn", Width = 100 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ Tên", Width = 150 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoDienThoai", HeaderText = "Số Điện Thoại", Width = 110 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayGioHen", HeaderText = "Ngày Giờ Hẹn", Width = 130 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BacSi", HeaderText = "Bác Sĩ", Width = 140 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LyDoKham", HeaderText = "Lý Do Khám", Width = 150 });
            dgvList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng Thái", Width = 100 });

            dgvList.SelectionChanged += DgvList_SelectionChanged;

            this.Controls.Add(dgvList);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(tlpInput);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);
        }

        private void InitDataBinding()
        {
            _bindingSource.DataSource = _listAppointments;
            dgvList.DataSource = _bindingSource;

            _listAppointments.Add(new Appointment { MaLichHen = "LH001", HoTen = "Nguyễn Văn A", SoDienThoai = "0901234567", NgayGioHen = DateTime.Now.AddHours(2), BacSi = "BS. Nguyễn Văn A", LyDoKham = "Đau đầu", TrangThai = "Chờ khám" });
            _listAppointments.Add(new Appointment { MaLichHen = "LH002", HoTen = "Trần Thị B", SoDienThoai = "0987654321", NgayGioHen = DateTime.Now.AddDays(1), BacSi = "BS. Trần Thị B", LyDoKham = "Tái khám", TrangThai = "Chờ khám" });
            FilterData(null, null);
        }

        private bool ValidateInput(bool isNew)
        {
            _errorProvider.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtMa.Text))
            {
                _errorProvider.SetError(txtMa, "Mã lịch hẹn không được để trống!");
                isValid = false;
            }
            else if (isNew && _listAppointments.Any(x => x.MaLichHen.Equals(txtMa.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _errorProvider.SetError(txtMa, "Mã lịch hẹn đã tồn tại!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                _errorProvider.SetError(txtHoTen, "Tên bệnh nhân không được để trống!");
                isValid = false;
            }

            if (!Regex.IsMatch(txtSdt.Text.Trim(), @"^0\d{9}$"))
            {
                _errorProvider.SetError(txtSdt, "Số điện thoại phải có 10 chữ số và bắt đầu bằng số 0!");
                isValid = false;
            }

            if (dtpNgayGio.Value < DateTime.Now.AddMinutes(-5) && isNew)
            {
                _errorProvider.SetError(dtpNgayGio, "Ngày hẹn không được nằm trong quá khứ!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLyDo.Text))
            {
                _errorProvider.SetError(txtLyDo, "Lý do khám không được để trống!");
                isValid = false;
            }

            return isValid;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(true)) return;

            var item = new Appointment
            {
                MaLichHen = txtMa.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                SoDienThoai = txtSdt.Text.Trim(),
                NgayGioHen = dtpNgayGio.Value,
                BacSi = cboBacSi.SelectedItem.ToString(),
                LyDoKham = txtLyDo.Text.Trim(),
                TrangThai = cboTrangThai.SelectedItem.ToString()
            };

            _listAppointments.Add(item);
            FilterData(null, null);
            MessageBox.Show("Thêm lịch hẹn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            var currentObj = (Appointment)dgvList.CurrentRow.DataBoundItem;

            if (!ValidateInput(false)) return;

            if (MessageBox.Show("Xác nhận cập nhật lịch hẹn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                currentObj.HoTen = txtHoTen.Text.Trim();
                currentObj.SoDienThoai = txtSdt.Text.Trim();
                currentObj.NgayGioHen = dtpNgayGio.Value;
                currentObj.BacSi = cboBacSi.SelectedItem.ToString();
                currentObj.LyDoKham = txtLyDo.Text.Trim();
                currentObj.TrangThai = cboTrangThai.SelectedItem.ToString();

                _bindingSource.ResetBindings(false);
                FilterData(null, null);
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            var currentObj = (Appointment)dgvList.CurrentRow.DataBoundItem;

            if (MessageBox.Show($"Xác nhận xóa lịch hẹn {currentObj.MaLichHen}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _listAppointments.Remove(currentObj);
                FilterData(null, null);
            }
        }

        private void DgvList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow != null && dgvList.CurrentRow.DataBoundItem is Appointment item)
            {
                txtMa.Text = item.MaLichHen;
                txtHoTen.Text = item.HoTen;
                txtSdt.Text = item.SoDienThoai;
                dtpNgayGio.Value = item.NgayGioHen;
                cboBacSi.SelectedItem = item.BacSi;
                txtLyDo.Text = item.LyDoKham;
                cboTrangThai.SelectedItem = item.TrangThai;
            }
        }

        private void FilterData(object sender, EventArgs e)
        {
            var query = _listAppointments.AsEnumerable();

            string keyword = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.HoTen.ToLower().Contains(keyword) || x.SoDienThoai.Contains(keyword));
            }

            if (chkEnableDateFilter.Checked)
            {
                query = query.Where(x => x.NgayGioHen.Date == dtpFilterDate.Value.Date);
            }

            if (cboFilterStatus.SelectedIndex > 0)
            {
                string status = cboFilterStatus.SelectedItem.ToString();
                query = query.Where(x => x.TrangThai == status);
            }

            var filteredList = query.ToList();
            _bindingSource.DataSource = filteredList;
            lblStatusCount.Text = $"Số lịch hẹn: {filteredList.Count}";
        }

        private void ExportToCsv_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV File (*.csv)|*.csv", FileName = "DanhSachLichHen.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Mã Lịch Hẹn,Họ Tên,Số Điện Thoại,Ngày Giờ Hẹn,Bác Sĩ,Lý Do Khám,Trạng Thái");

                    foreach (var item in (System.Collections.IList)dgvList.DataSource)
                    {
                        var app = (Appointment)item;
                        sb.AppendLine($"\"{app.MaLichHen}\",\"{app.HoTen}\",\"{app.SoDienThoai}\",\"{app.NgayGioHen:dd/MM/yyyy HH:mm}\",\"{app.BacSi}\",\"{app.LyDoKham}\",\"{app.TrangThai}\"");
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}