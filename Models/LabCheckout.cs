using System;

namespace BaitapWinForms.Models
{
    public class LabCheckout
    {
        public string MaPhieu { get; set; }
        public string MaSinhVien { get; set; }
        public string HoTenSinhVien { get; set; }
        public string ThietBi { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        
        public string TrangThai
        {
            get
            {
                if (NgayTraThucTe.HasValue) return "Đã trả";
                if (HanTra.Date < DateTime.Today) return "Quá hạn";
                return "Đang mượn";
            }
        }
    }
}