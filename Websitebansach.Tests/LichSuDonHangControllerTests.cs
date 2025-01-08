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
    public class LichSuDonHangTests
    {
        private Mock<QuanLyBanSachEntities> _mockDb;
        private KhachHangController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Khởi tạo DbContext giả lập
            _mockDb = new Mock<QuanLyBanSachEntities>();

            // Dữ liệu giả lập
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

        //Kiểm tra khi người dùng chưa đăng nhập, kết quả trả về phải chuyển hướng đến trang đăng nhập.
        [TestMethod]
        public void LichSuDonHang_UserNotLoggedIn_ShouldRedirectToLogin()
        {
            // Arrange
            _controller.Session["KhachHang"] = null;

            // Act
            var result = _controller.LichSuDonHang() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("DangNhap", result.RouteValues["action"]);
            Assert.AreEqual("KhachHang", result.RouteValues["controller"]);
        }

        //Kiểm tra khi người dùng đăng nhập và có đơn hàng, danh sách đơn hàng phải trả về và được sắp xếp theo ngày đặt (gần nhất trước).
        [TestMethod]
        public void LichSuDonHang_UserHasOrders_ShouldReturnViewWithOrders()
        {
            // Arrange
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 1, TenKH = "Nguyen Van A" };

            // Act
            var result = _controller.LichSuDonHang() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<DonHang>));

            var model = result.Model as List<DonHang>;
            Assert.AreEqual(2, model.Count);
            Assert.IsTrue(model[0].NgayDat > model[1].NgayDat); // Kiểm tra sắp xếp theo ngày đặt
        }

        //Kiểm tra khi người dùng đăng nhập nhưng không có đơn hàng, danh sách đơn hàng phải trống.
        [TestMethod]
        public void LichSuDonHang_UserHasNoOrders_ShouldReturnEmptyList()
        {
            // Arrange
            _controller.Session["KhachHang"] = new KhachHang { MaKH = 2, TenKH = "Nguyen Van B" };

            // Act
            var result = _controller.LichSuDonHang() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<DonHang>));

            var model = result.Model as List<DonHang>;
            Assert.AreEqual(0, model.Count);
        }
    }
}
