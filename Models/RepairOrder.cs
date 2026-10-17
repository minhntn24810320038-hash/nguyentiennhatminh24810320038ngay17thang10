using System;

namespace BaitapWinForms.Models
{
    public class RepairOrder
    {
        public string MaDon { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string LoaiThietBi { get; set; }
        public string MoTaLoi { get; set; }
        public DateTime NgayTiepNhan { get; set; }
        public decimal ChiPhiDuKien { get; set; }
        public string HinhAnhPath { get; set; }
        public string TrangThai { get; set; }
    }
}