using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Websitebansach.Controllers;
using Websitebansach.Models;
using System.Data.Entity;
using Sitecore.FakeDb;

namespace Websitebansach.Tests.Controllers
{
    [TestClass]
    public class QuanLiChuDeControllerTests
    {
        private Mock<QuanLyBanSachEntities> _mockDb;
        private QuanLiChuDeController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock QuanLyBanSachEntities
            _mockDb = new Mock<QuanLyBanSachEntities>();

            // Tạo controller với mock db context
            _controller = new QuanLiChuDeController
            {
                db = _mockDb.Object
            };
        }

        // Test Create POST khi Model hợp lệ
        [TestMethod]
        public void Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange: Tạo dữ liệu chuDe hợp lệ
            var chuDe = new ChuDe { TenChuDe = "Chủ đề mới" };

            // Mock DbSet<ChuDe> trả về dữ liệu chuDe
            var mockDbSet = new Mock<DbSet<ChuDe>>();
            _mockDb.Setup(db => db.ChuDes).Returns(mockDbSet.Object);

            // Act: Gọi hành động Create (POST) với chủ đề hợp lệ
            var result = _controller.Create(chuDe) as RedirectToRouteResult;

            // Assert: Kiểm tra xem phương thức SaveChanges đã được gọi và chuyển hướng về Index
            _mockDb.Verify(db => db.SaveChanges(), Times.Once);  // Kiểm tra SaveChanges
            Assert.AreEqual("Index", result?.RouteValues["action"]); // Kiểm tra chuyển hướng
        }

        // Test Create POST khi Model không hợp lệ
        [TestMethod]
        public void Create_Post_InvalidModel_ReturnsView()
        {
            // Arrange: Tạo dữ liệu chuDe không hợp lệ
            var chuDe = new ChuDe { TenChuDe = "" }; // Mô phỏng chuDe không hợp lệ

            _controller.ModelState.AddModelError("TenChuDe", "Tên chủ đề không được để trống");

            // Act: Gọi hành động Create (POST) với chủ đề không hợp lệ
            var result = _controller.Create(chuDe) as ViewResult;

            // Assert: Kiểm tra kết quả trả về là View với model
            Assert.IsNotNull(result);
            Assert.AreEqual(chuDe, result.Model);
        }

        // Test Delete POST khi xóa chủ đề hợp lệ
        [TestMethod]
        public void DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            // Arrange: Tạo dữ liệu chuDe và mock phương thức Find để trả về chuDe hợp lệ
            var chuDe = new ChuDe { MaChuDe = 1, TenChuDe = "Chủ đề cần xóa" };
            _mockDb.Setup(db => db.ChuDes.Find(1)).Returns(chuDe);

            // Mock DbSet<ChuDe> và phương thức Remove
            var mockDbSet = new Mock<DbSet<ChuDe>>();
            _mockDb.Setup(db => db.ChuDes).Returns(mockDbSet.Object);

            // Act: Gọi hành động DeleteConfirmed với id hợp lệ
            var result = _controller.DeleteConfirmed(1) as RedirectToRouteResult;

            // Assert: Kiểm tra xem phương thức Remove và SaveChanges đã được gọi
            _mockDb.Verify(db => db.ChuDes.Remove(chuDe), Times.Once);  // Kiểm tra Remove
            _mockDb.Verify(db => db.SaveChanges(), Times.Once);         // Kiểm tra SaveChanges
            Assert.AreEqual("Index", result?.RouteValues["action"]);    // Kiểm tra chuyển hướng
        }

        // Test Delete POST khi ID không hợp lệ
        [TestMethod]
        public void DeleteConfirmed_InvalidId_ReturnsNotFound()
        {
            // Arrange: Không tìm thấy chuDe trong db (trả về null)
            _mockDb.Setup(db => db.ChuDes.Find(999)).Returns((ChuDe)null);

            // Act: Gọi hành động DeleteConfirmed với id không hợp lệ
            var result = _controller.DeleteConfirmed(999);

            // Assert: Kiểm tra xem kết quả có phải là HttpNotFound hay không
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        // Test Edit GET khi tìm thấy chuDe
        [TestMethod]
        public void Edit_Get_ValidId_ReturnsViewWithChuDe()
        {
            // Arrange: Tạo chuDe giả và mock phương thức Find
            var chuDe = new ChuDe { MaChuDe = 1, TenChuDe = "Chủ đề cần sửa" };
            _mockDb.Setup(db => db.ChuDes.Find(1)).Returns(chuDe);

            // Act: Gọi hành động Edit (GET) với id hợp lệ
            var result = _controller.Edit(1) as ViewResult;

            // Assert: Kiểm tra xem kết quả trả về là View với model chuDe
            Assert.IsNotNull(result);
            Assert.AreEqual(chuDe, result.Model);
        }

        // Test Edit GET khi không tìm thấy chuDe
        [TestMethod]
        public void Edit_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange: Không tìm thấy chuDe trong db
            _mockDb.Setup(db => db.ChuDes.Find(999)).Returns((ChuDe)null);

            // Act: Gọi hành động Edit (GET) với id không hợp lệ
            var result = _controller.Edit(999);

            // Assert: Kiểm tra xem kết quả có phải là HttpNotFound hay không
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }
    }
}
