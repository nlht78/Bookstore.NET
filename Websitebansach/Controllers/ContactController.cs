using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Websitebansach.Controllers
{
    public class ContactController : Controller
    {

        public SmtpClient SmtpClient { get; set; } = new SmtpClient("smtp.gmail.com", 587);

        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(string HoTen, string Email, string SoDienThoai, string NoiDung)
        {
            try
            {
                // Thông tin email của quản trị viên
                string adminEmail = "adwebsach24@gmail.com";
                string subject = "Liên hệ từ người dùng";
                string body = $@"
                    <h3>Thông tin liên hệ:</h3>
                    <p><strong>Họ và tên:</strong> {HoTen}</p>
                    <p><strong>Email:</strong> {Email}</p>
                    <p><strong>Số điện thoại:</strong> {SoDienThoai}</p>
                    <p><strong>Nội dung:</strong> {NoiDung}</p>
                ";

                // Cấu hình SMTP
                using (var smtpClient = SmtpClient)
                {
                    smtpClient.Credentials = new NetworkCredential("websach24@gmail.com", "blew lcft lkre tlmj");
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(Email),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(adminEmail);

                    smtpClient.Send(mailMessage);
                }

                TempData["SuccessMessage"] = "Lời nhắn của bạn đã được gửi thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Gửi lời nhắn thất bại. Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
