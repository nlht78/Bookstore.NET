using Microsoft.VisualStudio.TestTools.UnitTesting;
using Websitebansach.Controllers;
using Websitebansach.Models;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using PagedList;
using Moq;
using System.Data.Entity;

namespace Websitebansach.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<QuanLyBanSachEntities> _mockDb;
        private HomeController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock QuanLyBanSachEntities
            _mockDb = new Mock<QuanLyBanSachEntities>();

            // Tạo controller với mock db context
            _controller = new HomeController
            {
                db = _mockDb.Object
            };
        }

        [TestMethod]
        public void Index_SortByPriceAsc_ReturnsCorrectOrder()
        {
            // Arrange: Tạo dữ liệu sách
            var mockBooks = new List<Sach>
            {
                new Sach { MaSach = 1, TenSach = "Book A", GiaBan = 100 },
                new Sach { MaSach = 2, TenSach = "Book B", GiaBan = 200 },
                new Sach { MaSach = 3, TenSach = "Book C", GiaBan = 150 }
            };

            // Mock DbSet<Sach> trả về dữ liệu sách
            var mockDbSet = new Mock<DbSet<Sach>>();
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.Provider).Returns(mockBooks.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.Expression).Returns(mockBooks.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.ElementType).Returns(mockBooks.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.GetEnumerator()).Returns(mockBooks.GetEnumerator());

            // Mock db.Saches để trả về mock DbSet<Sach>
            _mockDb.Setup(db => db.Saches).Returns(mockDbSet.Object);

            // Act: Gọi hành động Index với sortOption "PriceAsc"
            var result = _controller.Index(1, "PriceAsc") as ViewResult;
            var model = result?.Model as IPagedList<Sach>;

            // Assert: Kiểm tra sắp xếp
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Count);
            Assert.AreEqual(100, model[0].GiaBan);
            Assert.AreEqual(150, model[1].GiaBan);
            Assert.AreEqual(200, model[2].GiaBan);
        }

        [TestMethod]
        public void Index_SortByAZ_ReturnsCorrectOrder()
        {
            // Arrange: Tạo dữ liệu sách
            var mockBooks = new List<Sach>
            {
                new Sach { MaSach = 1, TenSach = "Book C", GiaBan = 100 },
                new Sach { MaSach = 2, TenSach = "Book A", GiaBan = 200 },
                new Sach { MaSach = 3, TenSach = "Book B", GiaBan = 150 }
            };

            // Mock DbSet<Sach> trả về dữ liệu sách
            var mockDbSet = new Mock<DbSet<Sach>>();
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.Provider).Returns(mockBooks.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.Expression).Returns(mockBooks.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.ElementType).Returns(mockBooks.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Sach>>().Setup(m => m.GetEnumerator()).Returns(mockBooks.GetEnumerator());

            // Mock db.Saches để trả về mock DbSet<Sach>
            _mockDb.Setup(db => db.Saches).Returns(mockDbSet.Object);

            // Act: Gọi hành động Index với sortOption "AZ"
            var result = _controller.Index(1, "AZ") as ViewResult;
            var model = result?.Model as IPagedList<Sach>;

            // Assert: Kiểm tra sắp xếp
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Count);
            Assert.AreEqual("Book A", model[0].TenSach);
            Assert.AreEqual("Book B", model[1].TenSach);
            Assert.AreEqual("Book C", model[2].TenSach);
        }
    }
}
