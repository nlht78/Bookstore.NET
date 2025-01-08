using System;
using System.Web.Mvc;
using Websitebansach.Models;
using System.Linq;
using System.Net.Mail;
using System.Net;

namespace YourNamespace.Controllers
{
    public class KhachHangController : Controller
    {
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        public QuanLyBanSachEntities Db;

        // GET: KhachHang/DangKy
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: KhachHang/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(KhachHang kh, string XacNhanMatKhau)
        {
            if (ModelState.IsValid)
            {
                if (kh.MatKhau != XacNhanMatKhau)
                {
                    ModelState.AddModelError("XacNhanMatKhau", "Xác nhận mật khẩu không khớp.");
                    return View(kh);
                }

                kh.quyen = 0; // Mặc định quyền là 0 (người dùng)
                db.KhachHangs.Add(kh);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công";
                return RedirectToAction("DangNhap", "KhachHang");
            }

            return View(kh);
        }

        // GET: KhachHang/DangNhap
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: KhachHang/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string TaiKhoan, string MatKhau)
        {
            var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.TaiKhoan == TaiKhoan && kh.MatKhau == MatKhau);
            if (khachHang != null)
            {
                Session["KhachHang"] = khachHang;
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng.");
            return View();
        }

        // GET: KhachHang/DangXuat
        public ActionResult DangXuat()
        {
            Session["KhachHang"] = null;
            return RedirectToAction("Index", "Home");
        }

        // GET: KhachHang/ChinhSua
        [HttpGet]
        public ActionResult ChinhSua()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            KhachHang kh = (KhachHang)Session["KhachHang"];
            return View(kh);
        }

        // POST: KhachHang/ChinhSua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSua(KhachHang updatedKhachHang)
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            if (ModelState.IsValid)
            {
                KhachHang currentKhachHang = (KhachHang)Session["KhachHang"];
                KhachHang khFromDb = db.KhachHangs.Find(currentKhachHang.MaKH);

                if (khFromDb != null)
                {
                    // Cập nhật thông tin
                    khFromDb.HoTen = updatedKhachHang.HoTen;
                    khFromDb.DiaChi = updatedKhachHang.DiaChi;
                    khFromDb.Email = updatedKhachHang.Email;
                    khFromDb.DienThoai = updatedKhachHang.DienThoai;

                    db.SaveChanges();

                    // Cập nhật lại thông tin trong session
                    Session["KhachHang"] = khFromDb;

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                }
            }

            return View(updatedKhachHang);
        }

        // GET: KhachHang/DoiMatKhau
        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            KhachHang kh = (KhachHang)Session["KhachHang"];
            return View(kh);
        }

        // POST: KhachHang/DoiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhanMatKhauMoi)
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            if (ModelState.IsValid)
            {
                KhachHang currentKhachHang = (KhachHang)Session["KhachHang"];
                KhachHang kh = db.KhachHangs.Find(currentKhachHang.MaKH);

                if (kh.MatKhau != matKhauCu) // Nên hash mật khẩu nếu cần
                {
                    ViewBag.Error = "Mật khẩu cũ không đúng!";
                    return View();
                }

                // Kiểm tra xác nhận mật khẩu mới
                if (matKhauMoi != xacNhanMatKhauMoi)
                {
                    ViewBag.Error = "Mật khẩu mới và xác nhận mật khẩu không khớp!";
                    return View();
                }

                // Cập nhật mật khẩu
                kh.MatKhau = matKhauMoi;
                db.SaveChanges();
                ViewBag.Message = "Mật khẩu đã được thay đổi thành công!";
            }
            return View();
        }
        // GET: KhachHang/LichSuDonHang
        [HttpGet]
        public ActionResult LichSuDonHang()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }
            var kh = (KhachHang)Session["KhachHang"];
            // Lấy danh sách đơn hàng của khách hàng
            var donHangs = db.DonHangs
                        .Where(dh => dh.MaKH == kh.MaKH)
                        .OrderByDescending(dh => dh.NgayDat)
                        .ToList();
            return View(donHangs);
        }
        // GET: KhachHang/ChiTietLichSuDonHang
        [HttpGet]
        public ActionResult ChiTietLichSuDonHang(int? maDonHang)
        {
            // Lấy chi tiết đơn hàng từ CSDL
            var chiTietDonHangs = db.ChiTietDonHangs
                .Where(ct => ct.MaDonHang == maDonHang)
                .ToList();

            // Lấy thông tin đơn hàng để hiển thị thêm
            var donHang = db.DonHangs.FirstOrDefault(dh => dh.MaDonHang == maDonHang);
            ViewBag.DonHang = donHang;

            return View(chiTietDonHangs);
        }

        // GET: KhachHang/HuyDonHang
        [HttpPost]
        public ActionResult HuyDonHang(int maDonHang)
        {
            if (maDonHang <= 0)
            {
                TempData["ErrorMessage"] = "Mã đơn hàng không hợp lệ.";
                return RedirectToAction("LichSuDonHang");
            }

            try
            {
                // Xóa chi tiết đơn hàng
                var chiTietDonHangs = db.ChiTietDonHangs.Where(ct => ct.MaDonHang == maDonHang).ToList();
                db.ChiTietDonHangs.RemoveRange(chiTietDonHangs);

                // Xóa đơn hàng
                var donHang = db.DonHangs.SingleOrDefault(dh => dh.MaDonHang == maDonHang);
                if (donHang != null)
                {
                    db.DonHangs.Remove(donHang);
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi hủy đơn hàng: " + ex.Message;
            }

            return RedirectToAction("LichSuDonHang");
        }

        // Trang Quên Mật Khẩu
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuenMatKhau(string email)
        {
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == email);
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống!";
                return View();
            }

            // Tạo OTP ngẫu nhiên
            var otp = new Random().Next(100000, 999999).ToString();

            // Lưu OTP vào Session để xác thực
            Session["OTP"] = otp;
            Session["EmailReset"] = email;

            // Gửi OTP qua email
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("websach24@gmail.com", "blew lcft lkre tlmj"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("websach24@gmail.com"),
                    Subject = "Mã OTP Đổi Mật Khẩu",
                    Body = $"Mã OTP của bạn là: {otp}",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
                TempData["SuccessMessage"] = "Mã OTP đã được gửi đến email của bạn.";
                return RedirectToAction("XacNhanOTP");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể gửi email. Vui lòng thử lại.";
                return View();
            }
        }

        // Trang Xác Nhận OTP
        public ActionResult XacNhanOTP()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XacNhanOTP(string otp, string matKhauMoi, string xacNhanMatKhauMoi)
        {
            // Kiểm tra OTP
            if (Session["OTP"] == null || otp != Session["OTP"].ToString())
            {
                TempData["ErrorMessage"] = "Mã OTP không hợp lệ!";
                return View();
            }

            // Kiểm tra mật khẩu mới và xác nhận
            if (matKhauMoi != xacNhanMatKhauMoi)
            {
                TempData["ErrorMessage"] = "Xác nhận mật khẩu không khớp!";
                return View();
            }

            // Cập nhật mật khẩu mới
            var email = Session["EmailReset"].ToString();
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == email);

            if (khachHang != null)
            {
                khachHang.MatKhau = matKhauMoi; // Lưu mật khẩu mới (nên mã hóa trước khi lưu)
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap");
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra, vui lòng thử lại.";
            return View();
        }
        // GET: KhachHang/ThongBao
        public ActionResult ThongBao()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }
            var kh = (KhachHang)Session["KhachHang"];
            // Lấy danh sách đơn hàng của khách hàng
            var donHangs = db.DonHangs
                        .Where(dh => dh.MaKH == kh.MaKH)
                        .OrderByDescending(dh => dh.NgayDat)
                        .ToList();
            return View(donHangs);
        }
    }
}