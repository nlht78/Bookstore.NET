using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Models;

namespace Websitebansach.Controllers
{
    public class ChudeController : Controller
    {
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        // GET: Chude
        public PartialViewResult ChudePartial()
        {
            return PartialView(db.ChuDes.Take(9).ToList());
        }
        public ViewResult SachTheoChuDe(int MaChuDe = 0)
        {
            ChuDe cd = db.ChuDes.SingleOrDefault(n => n.MaChuDe == MaChuDe);
            if (cd == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<Sach> lsach = db.Saches.Where(n => n.MaChuDe == MaChuDe).OrderBy(n => n.GiaBan).ToList();
            if (lsach.Count == 0)
            {
                ViewBag.Sach = "Không có sách cho chủ đề này";
            }
            return View(lsach);
        }
    }
}