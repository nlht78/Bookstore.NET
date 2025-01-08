using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Filters;
using Websitebansach.Models; // Namespace chứa model Entity Framework 

namespace Websitebansach.Controllers
{
    [AdminAuthorize]
    public class QuanLiChuDeController : Controller
    {
        public QuanLyBanSachEntities db = new QuanLyBanSachEntities();

        // GET: QuanLiChuDe
        public ActionResult Index()
        {
            var chuDes = db.ChuDes.ToList(); // Lấy danh sách tất cả các chủ đề
            return View(chuDes);
        }

        // GET: QuanLiChuDe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuanLiChuDe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ChuDes.Add(chuDe); // Thêm mới chủ đề vào CSDL
                    db.SaveChanges(); // Lưu thay đổi
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi: Không thể thêm mới. " + ex.Message);
                }
            }
            return View(chuDe);
        }

        // GET: QuanLiChuDe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ChuDe chuDe = db.ChuDes.Find(id); // Tìm chủ đề theo ID
            if (chuDe == null)
            {
                return HttpNotFound();
            }
            return View(chuDe);
        }

        // POST: QuanLiChuDe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(chuDe).State = System.Data.Entity.EntityState.Modified; // Đánh dấu chủ đề là đã sửa
                    db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return RedirectToAction("Index"); // Quay lại trang danh sách sau khi sửa
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi: Không thể sửa. " + ex.Message); // Thêm lỗi vào model state
                }
            }
            return View(chuDe); // Nếu có lỗi, quay lại trang Edit
        }

        // GET: QuanLiChuDe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ChuDe chuDe = db.ChuDes.Find(id); // Tìm chủ đề theo ID
            if (chuDe == null)
            {
                return HttpNotFound();
            }
            return View(chuDe);
        }

        // POST: QuanLiChuDe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ChuDe chuDe = db.ChuDes.Find(id);
                if (chuDe == null)
                {
                    return HttpNotFound();
                }

                db.ChuDes.Remove(chuDe); // Xóa chủ đề
                db.SaveChanges(); // Lưu thay đổi
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: Không thể xóa. " + ex.Message);
                return RedirectToAction("Delete", new { id = id }); // Quay lại trang xóa
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Giải phóng tài nguyên khi không còn sử dụng
            }
            base.Dispose(disposing);
        }
    }
}
