using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Websitebansach.Controllers;
using Websitebansach.Models;
using YourNamespace.Controllers;

namespace Websitebansach.Tests.Controllers
{
    [TestClass]
    public class ThongBaoControllerTests
    {
        private Mock<QuanLyBanSachEntities> _mockDb;
        private KhachHangController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Khởi tạo DbContext giả lập
            _mockDb = new Mock<QuanLyBanSachEntities>();

            // Setup dữ liệu giả lập
            var donHangs = new List<DonHang>
            {
                new DonHang { MaDonHang = 1, MaKH = 1, TinhTrangGiaoHang = 0, DaThanhToan = 0, NgayDat = System.DateTime.Now.AddDays(-1) },
                new DonHang { MaDonHang = 2, MaKH = 1, TinhTrangGiaoHang = 1, DaThanhToan = 1, NgayDat = System.DateTime.Now.AddDays(-2) }
            }.AsQueryable();

            var mockDonHangs = new Mock<DbSet<DonHang>>();
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.Provider).Returns(donHangs.Provider);
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.Expression).Returns(donHangs.Expression);
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.ElementType).Returns(donHangs.ElementType);
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.GetEnumerator()).Returns(donHangs.GetEnumerator());

            _mockDb.Setup(m => m.DonHangs).Returns(mockDonHangs.Object);

            // Khởi tạo controller
            _controller = new KhachHangController
            {
                Db = _mockDb.Object // Inject DbContext giả lập
            };
        }

        [TestMethod]
        public void ThongBao_UserNotLoggedIn_ShouldRedirectToLogin()
        {
            // Arrange: Người dùng chưa đăng nhập
            _controller.Session["KhachHang"] = null;

            // Act: Gọi action ThongBao
            var result = _controller.ThongBao() as RedirectToRouteResult;

            // Assert: Kiểm tra kết quả trả về là RedirectToAction "DangNhap"
            Assert.IsNotNull(result);
            Assert.AreEqual("DangNhap", result.RouteValues["action"]);
            Assert.AreEqual("KhachHang", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ThongBao_UserHasOrders_ShouldReturnViewWithOrders()
        {
            // Arrange: Người dùng đã đăng nhập
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 1, TenKH = "Nguyen Van A" };

            // Act: Gọi action ThongBao
            var result = _controller.ThongBao() as ViewResult;

            // Assert: Kiểm tra kết quả trả về là View với danh sách đơn hàng
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<DonHang>));

            var model = result.Model as List<DonHang>;
            Assert.AreEqual(2, model.Count); // Kiểm tra có 2 đơn hàng

            // Kiểm tra đơn hàng được sắp xếp theo ngày đặt
            Assert.IsTrue(model[0].NgayDat > model[1].NgayDat);
        }

        [TestMethod]
        public void ThongBao_UserHasNoOrders_ShouldReturnNoNotificationsMessage()
        {
            // Arrange: Người dùng có tài khoản nhưng không có đơn hàng
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 2, TenKH = "Nguyen Van B" };

            // Act: Gọi action ThongBao
            var result = _controller.ThongBao() as ViewResult;

            // Assert: Kiểm tra View trả về thông báo không có đơn hàng
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<DonHang>));

            var model = result.Model as List<DonHang>;
            Assert.AreEqual(0, model.Count); // Kiểm tra danh sách đơn hàng rỗng
        }

        [TestMethod]
        public void ThongBao_ReturnsCorrectNotificationMessage()
        {
            // Arrange: Người dùng đã đăng nhập
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 1, TenKH = "Nguyen Van A" };

            // Act: Gọi action ThongBao
            var result = _controller.ThongBao() as ViewResult;

            // Assert: Kiểm tra thông báo có chứa thông tin đúng
            var model = result.Model as List<DonHang>;

            Assert.IsTrue(model.Any(d => d.MaDonHang == 1 && d.NgayDat != null));
        }

    }
}