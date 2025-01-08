using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Models;
using PagedList.Mvc;
using PagedList;
using System.Security.Cryptography;
namespace Websitebansach.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: TiemKiem
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        [HttpPost]
        public ActionResult KetQuaTimKiem(FormCollection f, int? page)
        {
            String sTuKhoa = f["txtTimKiem"].ToString();
            ViewBag.TuKhoa = sTuKhoa;
            List<Sach> lstKQTK = db.Saches.Where(n => n.TenSach.Contains(sTuKhoa)).ToList();
            // Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            
            if (lstKQTK.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào ";
                return View(db.Saches.OrderBy(n => n.TenSach).ToPagedList(pageNumber,pageSize));
            }
            ViewBag.ThongBao = "Đã tìm thấy" + lstKQTK.Count + " kết quả!";
                return View(lstKQTK.OrderBy(n=>n.TenSach).ToPagedList(pageNumber,pageSize));
            
        }
        [HttpGet]
        public ActionResult KetQuaTimKiem(String sTuKhoa, int? page)
        {
            ViewBag.TuKhoa = sTuKhoa;
            List<Sach> lstKQTK = db.Saches.Where(n => n.TenSach.Contains(sTuKhoa)).ToList();
            // Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            
            if (lstKQTK.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào ";
                return View(db.Saches.OrderBy(n => n.TenSach).ToPagedList(pageNumber, pageSize));
            }
            ViewBag.ThongBao = "Đã tìm thấy" + lstKQTK.Count + " kết quả!";
            return View(lstKQTK.OrderBy(n => n.TenSach).ToPagedList(pageNumber, pageSize));

        }
    }
}