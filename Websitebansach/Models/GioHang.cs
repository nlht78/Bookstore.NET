﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Websitebansach.Models
{
    public class GioHang
    {
       

        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        public int iMaSach {  get; set; }
        public string sTenSach {  get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double ThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        //hàm tạo cho giỏ hàng
        public GioHang(int MaSach) 
        {
            iMaSach = MaSach;
            Sach sach = db.Saches.Single(n => n.MaSach == iMaSach);
            sTenSach = sach.TenSach;
            sAnhBia = sach.AnhBia;
            dDonGia = double.Parse(sach.GiaBan.ToString());
            iSoLuong = 1;
        }

    }

}
