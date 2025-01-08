using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Filters;
using Websitebansach.Models;

namespace Websitebansach.Controllers
{
    [AdminAuthorize]
    public class TacGiasController : Controller
    {
        private readonly QuanLyBanSachEntities db;

        public TacGiasController() : this(new QuanLyBanSachEntities()) { }

        public TacGiasController(QuanLyBanSachEntities context)
        {
            db = context;
        }

        // GET: TacGias
        public ActionResult Index()
        {
            return View(db.TacGias.ToList());
        }

        // GET: TacGias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            return View(tacGia);
        }

        // GET: TacGias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TacGias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaTacGia,TenTacGia,DiaChi,TieuSu,DienThoai")] TacGia tacGia)
        {
            // Kiểm tra ràng buộc các trường bắt buộc
            if (string.IsNullOrWhiteSpace(tacGia.TenTacGia) || string.IsNullOrWhiteSpace(tacGia.DiaChi) || string.IsNullOrWhiteSpace(tacGia.DienThoai))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin các trường.");
            }

            // Kiểm tra định dạng số điện thoại
            if (!string.IsNullOrWhiteSpace(tacGia.DienThoai) && !tacGia.DienThoai.All(char.IsDigit))
            {
                ModelState.AddModelError("DienThoai", "Số điện thoại phải là ký tự số.");
            }

            if (ModelState.IsValid)
            {
                db.TacGias.Add(tacGia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tacGia);
        }

        // GET: TacGias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            return View(tacGia);
        }

        // POST: TacGias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTacGia,TenTacGia,DiaChi,TieuSu,DienThoai")] TacGia tacGia)
        {
            // Kiểm tra ràng buộc các trường bắt buộc
            if (string.IsNullOrWhiteSpace(tacGia.TenTacGia) || string.IsNullOrWhiteSpace(tacGia.DiaChi) || string.IsNullOrWhiteSpace(tacGia.DienThoai))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin các trường.");
            }

            // Kiểm tra định dạng số điện thoại
            if (!string.IsNullOrWhiteSpace(tacGia.DienThoai) && !tacGia.DienThoai.All(char.IsDigit))
            {
                ModelState.AddModelError("DienThoai", "Số điện thoại phải là ký tự số.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(tacGia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tacGia);
        }

        // GET: TacGias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            return View(tacGia);
        }

        // POST: TacGias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TacGia tacGia = db.TacGias.Find(id);
            db.TacGias.Remove(tacGia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult SachDaThamGia(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin tác giả
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách sách mà tác giả đã tham gia
                        var sachDaThamGia = db.ThamGias
            .Where(tg => tg.MaTacGia == id)
            .Select(tg => new SachThamGiaViewModel
            {
             MaTacGia = tg.MaTacGia, // Gán giá trị từ model ThamGia
             MaSach = tg.MaSach,     // Gán giá trị từ model ThamGia
             TenSach = tg.Sach.TenSach,
             GiaBan = tg.Sach.GiaBan ?? 0,
             MoTa = tg.Sach.MoTa,
             AnhBia = tg.Sach.AnhBia,
             VaiTro = tg.VaiTro,
             ViTri = tg.ViTri
            })
            .ToList();


            ViewBag.TenTacGia = tacGia.TenTacGia;
            return View(sachDaThamGia);
        }
        // GET: TacGias/EditVaiTro
        public ActionResult EditVaiTro(int maTacGia, int maSach)
        {
            var thamGia = db.ThamGias.FirstOrDefault(tg => tg.MaTacGia == maTacGia && tg.MaSach == maSach);
            if (thamGia == null)
            {
                return HttpNotFound();
            }

            return View(thamGia);
        }
        // POST: TacGias/EditVaiTro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVaiTro(ThamGia model)
        {
            if (ModelState.IsValid)
            {
                var thamGia = db.ThamGias.FirstOrDefault(tg => tg.MaTacGia == model.MaTacGia && tg.MaSach == model.MaSach);
                if (thamGia == null)
                {
                    return HttpNotFound();
                }

                thamGia.VaiTro = model.VaiTro;
                thamGia.ViTri = model.ViTri;
                db.SaveChanges();

                return RedirectToAction("SachDaThamGia", new { id = model.MaTacGia });
            }

            return View(model);
        }

        // GET: TacGias/DeleteThamGia
        public ActionResult DeleteThamGia(int MaTacGia, int MaSach)
        {
            var thamGia = db.ThamGias.FirstOrDefault(tg => tg.MaTacGia == MaTacGia && tg.MaSach == MaSach);
            if (thamGia == null)
            {
                return HttpNotFound();
            }

            db.ThamGias.Remove(thamGia);
            db.SaveChanges();

            return RedirectToAction("SachDaThamGia", new { id = MaTacGia });
        }



        public ActionResult CreateThamGia()
        {
            // Lấy danh sách sách và tác giả từ cơ sở dữ liệu
            ViewBag.MaSach = new SelectList(db.Saches, "MaSach", "TenSach"); // Dropdown cho sách
            ViewBag.MaTacGia = new SelectList(db.TacGias, "MaTacGia", "TenTacGia"); // Dropdown cho tác giả
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateThamGia(ThamGia model)
        {
            // Kiểm tra ràng buộc các trường bắt buộc
            if (model.MaSach == 0 || model.MaTacGia == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn sách và tác giả.");
            }

            if (string.IsNullOrWhiteSpace(model.VaiTro))
            {
                ModelState.AddModelError("VaiTro", "Vai trò không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(model.ViTri))
            {
                ModelState.AddModelError("ViTri", "Vị trí không được để trống.");
            }

            if (ModelState.IsValid)
            {
                db.ThamGias.Add(model); // Thêm mới vào bảng ThamGia
                db.SaveChanges();
                return RedirectToAction("Index"); // Quay lại trang Index sau khi lưu
            }

            // Nếu có lỗi, tải lại dropdown
            ViewBag.MaSach = new SelectList(db.Saches, "MaSach", "TenSach", model.MaSach);
            ViewBag.MaTacGia = new SelectList(db.TacGias, "MaTacGia", "TenTacGia", model.MaTacGia);
            return View(model);
        }









        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
