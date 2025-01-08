using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Models;


namespace Websitebansach.Controllers
{
	
    public class SachController : Controller
    {
		QuanLyBanSachEntities db = new QuanLyBanSachEntities();
		// GET: Sach
		public PartialViewResult SachmenuPartial()
		{
			var listsach = db.Saches.Where(s => s.Moi == 1).Take(3).ToList();
			return PartialView(listsach);
		}
        public ViewResult XemChiTiet(int MaSach = 0)
        {
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == MaSach);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.TenChuDe = db.ChuDes.SingleOrDefault(n => n.MaChuDe == sach.MaChuDe)?.TenChuDe;
            ViewBag.NhaXuatBan = db.NhaXuatBans.SingleOrDefault(n => n.MaNXB == sach.MaNXB)?.TenNXB;

            // Lấy danh sách tác giả liên quan đến sách
            var tacGiaList = db.ThamGias
                               .Where(tg => tg.MaSach == MaSach)
                               .Select(tg => new SachThamGiaViewModel
                               {
                                   MaTacGia = tg.MaTacGia,
                                   MaSach = tg.MaSach,
                                   TenSach = tg.Sach.TenSach,
                                   GiaBan = tg.Sach.GiaBan ?? 0,
                                   MoTa = tg.Sach.MoTa,
                                   AnhBia = tg.Sach.AnhBia,
                                   VaiTro = tg.VaiTro,
                                   ViTri = tg.ViTri
                               })
                               .ToList();

            ViewBag.TacGia = tacGiaList;

            return View(sach);
        }


    }
}