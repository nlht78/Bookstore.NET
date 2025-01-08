using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using Websitebansach.Controllers;
using Websitebansach.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using YourNamespace.Controllers;

namespace Websitebansach.Tests.Controllers
{
    [TestClass] // Đánh dấu class là một lớp test.
    public class KhachHangControllerTests
    {
        private Mock<QuanLyBanSachEntities> _mockDb; // Mock đối tượng DbContext.
        private KhachHangController _controller; // Controller cần test.

        [TestInitialize] // Phương thức khởi tạo trước khi chạy mỗi test case.
        public void Setup()
        {
            // Tạo đối tượng mock DbContext
            _mockDb = new Mock<QuanLyBanSachEntities>();

            // Dữ liệu giả lập (Danh sách khách hàng)
            var khachHangs = new List<KhachHang>
            {
                new KhachHang { MaKH = 1, TaiKhoan = "user1", MatKhau = "123456" }, // Mật khẩu plaintext, không khuyến nghị.
                new KhachHang { MaKH = 2, TaiKhoan = "user2", MatKhau = "abcdef" }
            }.AsQueryable(); // Chuyển thành IQueryable để sử dụng với LINQ.

            // Mock DbSet<KhachHang>
            var mockKhachHangs = new Mock<DbSet<KhachHang>>();
            mockKhachHangs.As<IQueryable<KhachHang>>().Setup(m => m.Provider).Returns(khachHangs.Provider);
            mockKhachHangs.As<IQueryable<KhachHang>>().Setup(m => m.Expression).Returns(khachHangs.Expression);
            mockKhachHangs.As<IQueryable<KhachHang>>().Setup(m => m.ElementType).Returns(khachHangs.ElementType);
            mockKhachHangs.As<IQueryable<KhachHang>>().Setup(m => m.GetEnumerator()).Returns(khachHangs.GetEnumerator());

            // Cấu hình DbContext để trả về DbSet giả lập.
            _mockDb.Setup(m => m.KhachHangs).Returns(mockKhachHangs.Object);

            // Tạo controller và inject DbContext giả lập.
            _controller = new KhachHangController
            {
                Db = _mockDb.Object
            };
        }

        //Kiểm tra hành vi khi người dùng chưa đăng nhập và thực hiện hành động đổi mật khẩu.
        [TestMethod]
        public void DoiMatKhau_UserNotLoggedIn_ShouldRedirectToLogin()
        {
            // Arrange - Chuẩn bị dữ liệu
            _controller.Session["KhachHang"] = null; // Không có người dùng đăng nhập.

            // Act - Thực hiện hành động
            var result = _controller.DoiMatKhau("123456", "newpass", "newpass") as RedirectToRouteResult;

            // Assert - Kiểm tra kết quả
            Assert.IsNotNull(result); // Đảm bảo kết quả không null.
            Assert.AreEqual("DangNhap", result.RouteValues["action"]); // Kiểm tra action chuyển hướng.
            Assert.AreEqual("KhachHang", result.RouteValues["controller"]); // Kiểm tra controller chuyển hướng.
        }

        //Kiểm tra hành vi khi mật khẩu cũ được cung cấp không chính xác.
        [TestMethod]
        public void DoiMatKhau_IncorrectOldPassword_ShouldReturnErrorMessage()
        {
            // Arrange
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 1, TaiKhoan = "user1" }; // Người dùng đã đăng nhập.

            // Act
            var result = _controller.DoiMatKhau("wrongpassword", "newpass", "newpass") as ViewResult;

            // Assert
            Assert.IsNotNull(result); // Đảm bảo kết quả không null.
            Assert.AreEqual("Mật khẩu cũ không đúng!", _controller.ViewBag.Error); // Kiểm tra thông báo lỗi.
        }

        //Kiểm tra hành vi khi mật khẩu mới và mật khẩu xác nhận không khớp.
        [TestMethod]
        public void DoiMatKhau_PasswordMismatch_ShouldReturnErrorMessage()
        {
            // Arrange
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 1, TaiKhoan = "user1" };

            // Act
            var result = _controller.DoiMatKhau("123456", "newpass", "differentpass") as ViewResult;

            // Assert
            Assert.IsNotNull(result); // Đảm bảo kết quả không null.
            Assert.AreEqual("Mật khẩu mới và xác nhận mật khẩu không khớp!", _controller.ViewBag.Error); // Kiểm tra thông báo lỗi.
        }

        //Kiểm tra hành vi khi mật khẩu được đổi thành công và xác nhận rằng dữ liệu được lưu lại.
        [TestMethod]
        public void DoiMatKhau_Success_ShouldUpdatePassword()
        {
            // Arrange
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 1, TaiKhoan = "user1" };

            // Act
            var result = _controller.DoiMatKhau("123456", "newpass", "newpass") as ViewResult;

            // Assert
            Assert.IsNotNull(result); // Đảm bảo kết quả không null.
            Assert.AreEqual("Mật khẩu đã được thay đổi thành công!", _controller.ViewBag.Message); // Kiểm tra thông báo thành công.
            _mockDb.Verify(m => m.SaveChanges(), Times.Once); // Đảm bảo phương thức SaveChanges() được gọi đúng một lần.
        }
    }
}
