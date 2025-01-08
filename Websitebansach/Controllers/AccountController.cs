using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Models;
namespace Websitebansach.Controllers
{
    public class AccountController : Controller
    {
        private QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string TaiKhoan, string MatKhau)
        {
            var user = db.KhachHangs.FirstOrDefault(k => k.TaiKhoan == TaiKhoan && k.MatKhau == MatKhau);

            if (user != null)
            {
                Session["UserId"] = user.MaKH;
                Session["UserRole"] = user.quyen; // Lưu quyền để kiểm tra admin

                if (user.quyen == 1) // Admin
                {
                    return RedirectToAction("Index", "QuanliDonHang");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }



    }
}