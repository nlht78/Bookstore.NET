using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.FakeDb;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Websitebansach.Models;
using YourNamespace.Controllers;

namespace Websitebansach.Tests.Controllers
{
    [TestClass]
    public class HuyDonHangTests
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
                new DonHang { MaDonHang = 1, MaKH = 1, TinhTrangGiaoHang = 0, DaThanhToan = 0 },
                new DonHang { MaDonHang = 2, MaKH = 1, TinhTrangGiaoHang = 1, DaThanhToan = 1 }
            }.AsQueryable();

            var chiTietDonHangs = new List<ChiTietDonHang>
            {
                new ChiTietDonHang { MaDonHang = 1, MaSach = 101, SoLuong = 2, DonGia = 15000 },
                new ChiTietDonHang { MaDonHang = 1, MaSach = 102, SoLuong = 1, DonGia = 20000 }
            }.AsQueryable();

            var mockDonHangs = new Mock<DbSet<DonHang>>();
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.Provider).Returns(donHangs.Provider);
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.Expression).Returns(donHangs.Expression);
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.ElementType).Returns(donHangs.ElementType);
            mockDonHangs.As<IQueryable<DonHang>>().Setup(m => m.GetEnumerator()).Returns(donHangs.GetEnumerator());

            var mockChiTietDonHangs = new Mock<DbSet<ChiTietDonHang>>();
            mockChiTietDonHangs.As<IQueryable<ChiTietDonHang>>().Setup(m => m.Provider).Returns(chiTietDonHangs.Provider);
            mockChiTietDonHangs.As<IQueryable<ChiTietDonHang>>().Setup(m => m.Expression).Returns(chiTietDonHangs.Expression);
            mockChiTietDonHangs.As<IQueryable<ChiTietDonHang>>().Setup(m => m.ElementType).Returns(chiTietDonHangs.ElementType);
            mockChiTietDonHangs.As<IQueryable<ChiTietDonHang>>().Setup(m => m.GetEnumerator()).Returns(chiTietDonHangs.GetEnumerator());

            _mockDb.Setup(m => m.DonHangs).Returns(mockDonHangs.Object);
            _mockDb.Setup(m => m.ChiTietDonHangs).Returns(mockChiTietDonHangs.Object);

            // Khởi tạo controller
            _controller = new KhachHangController
            {
                Db = _mockDb.Object // Inject DbContext giả lập
            };
        }

        [TestMethod]
        public void HuyDonHang_InvalidId_ShouldReturnErrorMessage()
        {
            // Act
            var result = _controller.HuyDonHang(-1) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LichSuDonHang", result.RouteValues["action"]);
            Assert.AreEqual("Mã đơn hàng không hợp lệ.", _controller.TempData["ErrorMessage"]);
        }

        [TestMethod]
        public void HuyDonHang_ValidId_OrderNotShipped_ShouldRemoveOrder()
        {
            // Act
            var result = _controller.HuyDonHang(1) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LichSuDonHang", result.RouteValues["action"]);
            Assert.AreEqual("Đơn hàng đã được hủy thành công.", _controller.TempData["SuccessMessage"]);
            _mockDb.Verify(m => m.DonHangs.Remove(It.Is<DonHang>(d => d.MaDonHang == 1)), Times.Once);
            _mockDb.Verify(m => m.ChiTietDonHangs.RemoveRange(It.IsAny<IEnumerable<ChiTietDonHang>>()), Times.Once);
            _mockDb.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void HuyDonHang_ValidId_OrderShipped_ShouldReturnErrorMessage()
        {
            // Act
            var result = _controller.HuyDonHang(2) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LichSuDonHang", result.RouteValues["action"]);
            Assert.AreEqual("Đơn hàng đã được giao, không thể hủy.", _controller.TempData["ErrorMessage"]);
            _mockDb.Verify(m => m.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void HuyDonHang_Exception_ShouldReturnErrorMessage()
        {
            // Arrange
            _mockDb.Setup(m => m.SaveChanges()).Throws(new System.Exception("Database error"));

            // Act
            var result = _controller.HuyDonHang(1) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LichSuDonHang", result.RouteValues["action"]);
            Assert.AreEqual("Lỗi khi hủy đơn hàng: Database error", _controller.TempData["ErrorMessage"]);
        }
    }
}
