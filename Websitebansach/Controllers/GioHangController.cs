using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using Websitebansach.Models;
using System.Net;
using System.Net.Mail;
namespace Websitebansach.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        #region Giỏ hàng
        //Lấy giỏ hàng
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì khởi tạo list giỏ hàng (sessionGioHang)
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;

            }
            return lstGioHang;
        }
        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int iMaSach, string strURL)
        {
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == iMaSach);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy ra session giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sách này đã tồn tại trong session[giohang] chưa
            GioHang sanpham = lstGioHang.Find(n => n.iMaSach == iMaSach);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMaSach);
                //Add sản phẩm mới thêm vào List
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Cập nhật giỏ hàng
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            //Kiểm tra masp
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == iMaSP);
            //Nếu get sai masp thì sẽ trả vè trang lỗi 404
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp có tồn tai trong session["GioHang"]
            GioHang sanpham = lstGioHang.Find(n => n.iMaSach == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");

        }
        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Kiểm tra masp
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == iMaSP);
            //Nếu get sai masp thì sẽ trả vè trang lỗi 404
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.Find(n => n.iMaSach == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
        //Tính tổng số lượng và tổng tiền
        //Tính tổng số lượng

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["Giohang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }
        //Tạo partial giỏ hàng
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        //Xây dựng 1 view cho người dùng chỉnh sửa gió hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
        #endregion
        #region Đặt hàng

        private void SendEmail(string toEmail, string subject, string body)
        {
            using (var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("websach24@gmail.com", "blew lcft lkre tlmj"); // Thay bằng email và mật khẩu của bạn

                var mail = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress("websach24@gmail.com", "Website Bán Sách"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);
                smtp.Send(mail);
            }
        }

        [HttpPost]
        public ActionResult DatHang(string PaymentMethod)
        {
            // Kiểm tra đăng nhập
            if (Session["KhachHang"] == null || Session["KhachHang"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Thêm đơn hàng
            DonHang ddh = new DonHang();
            KhachHang khachHang = (KhachHang)Session["KhachHang"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = khachHang.MaKH;
            ddh.NgayDat = DateTime.Now;

            // Xử lý phương thức thanh toán
            if (PaymentMethod == "COD")
            {
                ddh.TinhTrangGiaoHang = 0; // Chưa giao
                ddh.DaThanhToan = 0; // Thanh toán khi nhận hàng
            }
            else if (PaymentMethod == "Online")
            {
                ddh.TinhTrangGiaoHang = 0; // Chưa giao
                ddh.DaThanhToan = 1; // Thanh toán online
            }

            db.DonHangs.Add(ddh);
            db.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                ChiTietDonHang ctDH = new ChiTietDonHang();
                ctDH.MaDonHang = ddh.MaDonHang;
                ctDH.MaSach = item.iMaSach;
                ctDH.SoLuong = item.iSoLuong;
                ctDH.DonGia = (decimal)item.dDonGia;
                db.ChiTietDonHangs.Add(ctDH);
            }
            db.SaveChanges();

            // Gửi email thông báo
            string toEmail = khachHang.Email; // Lấy email từ tài khoản đăng nhập
            string subject = "Xác nhận đơn hàng";
            string body = "Thông tin đặt hàng không hợp lệ."; // Giá trị mặc định


            if (PaymentMethod == "COD")
            {
                body = $@"
            <h2>Cảm ơn bạn đã đặt hàng tại Website Bán Sách!</h2>
            <p>Mã đơn hàng: {ddh.MaDonHang}</p>
            <p>Ngày đặt: {ddh.NgayDat?.ToString("dd/MM/yyyy HH:mm")}</p>
            <p>Tổng tiền: {gh.Sum(g => g.ThanhTien):0,0} VNĐ</p>
            <p>Phương thức thanh toán: Thanh toán khi nhận hàng</p>
            <p>Đơn hàng sẽ được giao đến địa chỉ của bạn trong thời gian sớm nhất.</p>
        ";
            }
            else if (PaymentMethod == "Online")
            {
                body = $@"
            <h2>Cảm ơn bạn đã đặt hàng tại Website Bán Sách!</h2>
            <p>Mã đơn hàng: {ddh.MaDonHang}</p>
            <p>Ngày đặt: {ddh.NgayDat?.ToString("dd/MM/yyyy HH:mm")}</p>
            <p>Tổng tiền: {gh.Sum(g => g.ThanhTien):0,0} VNĐ</p>
            <p>Phương thức thanh toán: Thanh toán online</p>
            <p>Vui lòng hoàn tất thanh toán qua thông tin sau:</p>
            <ul>
                <li>Tên tài khoản: Nhom 10</li>
                <li>Số tài khoản: 1021096794</li>
                <li>Ngân hàng: Vietcombank</li>
                <li>Nội dung chuyển khoản: MuaSach {ddh.MaDonHang}</li>
            </ul>
            <p>Đơn hàng sẽ được xử lý ngay sau khi chúng tôi nhận được thanh toán.</p>
        ";
            }

            SendEmail(toEmail, subject, body);

            // Điều hướng sau khi đặt hàng
            if (PaymentMethod == "Online")
            {
                return RedirectToAction("ThanhToan", new { maDonHang = ddh.MaDonHang });
            }
            else
            {
                return RedirectToAction("XacNhanDonHang", new { maDonHang = ddh.MaDonHang });
            }
        }


        // Trang xác nhận đơn hàng
        public ActionResult XacNhanDonHang(int maDonHang)
        {
            var donHang = db.DonHangs.SingleOrDefault(dh => dh.MaDonHang == maDonHang);
            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Đảm bảo `NgayDat` được khởi tạo hợp lệ (nếu cần)
            if (donHang.NgayDat == null)
            {
                donHang.NgayDat = DateTime.Now; // Giá trị mặc định
            }
            Session["GioHang"] = null;
            return View(donHang);

        }




        // Trang thanh toán
        public ActionResult ThanhToan(int maDonHang)
        {
            // Lấy thông tin đơn hàng
            DonHang donHang = db.DonHangs.SingleOrDefault(n => n.MaDonHang == maDonHang);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            decimal tongTien = donHang.ChiTietDonHangs.Sum(ct => (ct.SoLuong ?? 0) * (ct.DonGia ?? 0));

            // Truyền thông tin sang View
            ViewBag.MaDonHang = donHang.MaDonHang;
            ViewBag.TongTien = tongTien;
            ViewBag.TenTaiKhoan = "Nhom 10"; // Thông tin tài khoản mẫu
            ViewBag.SoTaiKhoan = "1021096794";
            ViewBag.NganHang = "Vietcombank";
            ViewBag.NoiDungChuyenKhoan = $"MuaSach {donHang.MaDonHang}";
            Session["GioHang"] = null;
            return View();
        }
        




        #endregion
    }
}
