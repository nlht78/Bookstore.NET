using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Websitebansach.Models
{
    public class SachThamGiaViewModel
    {
        public int MaTacGia { get; set; } // Thêm thuộc tính này
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public decimal GiaBan { get; set; }
        public string MoTa { get; set; }
        public string AnhBia { get; set; }
        public string VaiTro { get; set; }
        public string ViTri { get; set; }
    }
}