using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.FakeDb;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Websitebansach.Controllers;
using Websitebansach.Models;

namespace Websitebansach.Tests.Controllers
{
    [TestClass]
    public class TacGiasControllerTests
    {
        private Mock<QuanLyBanSachEntities> _mockContext;
        private Mock<DbSet<TacGia>> _mockTacGiaSet;
        private Mock<DbSet<ThamGia>> _mockThamGiaSet;
        private TacGiasController _controller;

        [TestInitialize]
        public void TestInitialize()
        {



            // Mock DbSet<TacGia>
            _mockTacGiaSet = new Mock<DbSet<TacGia>>();
            var tacGiaData = new List<TacGia>
            {
                new TacGia { MaTacGia = 1, TenTacGia = "Author 1", DiaChi = "Address 1", DienThoai = "123456" },
                new TacGia { MaTacGia = 2, TenTacGia = "Author 2", DiaChi = "Address 2", DienThoai = "654321" }
            }.AsQueryable();

            _mockTacGiaSet.As<IQueryable<TacGia>>().Setup(m => m.Provider).Returns(tacGiaData.Provider);
            _mockTacGiaSet.As<IQueryable<TacGia>>().Setup(m => m.Expression).Returns(tacGiaData.Expression);
            _mockTacGiaSet.As<IQueryable<TacGia>>().Setup(m => m.ElementType).Returns(tacGiaData.ElementType);
            _mockTacGiaSet.As<IQueryable<TacGia>>().Setup(m => m.GetEnumerator()).Returns(tacGiaData.GetEnumerator());
            // Mock DbSet<ThamGia>
            _mockThamGiaSet = new Mock<DbSet<ThamGia>>();
            var thamGiaData = new List<ThamGia>
            {
                new ThamGia { MaTacGia = 1, MaSach = 1, VaiTro = "Author", ViTri = "Primary" },
                new ThamGia { MaTacGia = 1, MaSach = 2, VaiTro = "Editor", ViTri = "Secondary" }
            }.AsQueryable();

            _mockThamGiaSet.As<IQueryable<ThamGia>>().Setup(m => m.Provider).Returns(thamGiaData.Provider);
            _mockThamGiaSet.As<IQueryable<ThamGia>>().Setup(m => m.Expression).Returns(thamGiaData.Expression);
            _mockThamGiaSet.As<IQueryable<ThamGia>>().Setup(m => m.ElementType).Returns(thamGiaData.ElementType);
            _mockThamGiaSet.As<IQueryable<ThamGia>>().Setup(m => m.GetEnumerator()).Returns(thamGiaData.GetEnumerator());



            // Mock Context
            _mockContext = new Mock<QuanLyBanSachEntities>();
            _mockContext.Setup(c => c.TacGias).Returns(_mockTacGiaSet.Object);
            _mockContext.Setup(c => c.ThamGias).Returns(_mockThamGiaSet.Object);



            // Inject mock context vào controller
            _controller = new TacGiasController(_mockContext.Object);
        }


        [TestMethod]
        public void Index_ReturnsAllTacGias()
        {
            // Act
            var result = _controller.Index() as ViewResult;
            var model = result.Model as List<TacGia>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        public void SachDaThamGia_ReturnsCorrectBooks()
        {
            // Act
            var result = _controller.SachDaThamGia(1) as ViewResult;
            var model = result.Model as List<SachThamGiaViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Author", model[0].VaiTro);
        }

        [TestMethod]
        public void Create_SavesNewTacGia()
        {
            // Arrange
            var newTacGia = new TacGia
            {
                MaTacGia = 3,
                TenTacGia = "Author 3",
                DiaChi = "Address 3",
                DienThoai = "111222"
            };

            // Act
            var result = _controller.Create(newTacGia) as RedirectToRouteResult;

            // Assert
            _mockTacGiaSet.Verify(m => m.Add(It.IsAny<TacGia>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void EditVaiTro_UpdatesRoleSuccessfully()
        {
            // Arrange
            var updatedThamGia = new ThamGia
            {
                MaTacGia = 1,
                MaSach = 1,
                VaiTro = "UpdatedRole",
                ViTri = "UpdatedPosition"
            };

            // Act
            var result = _controller.EditVaiTro(updatedThamGia) as RedirectToRouteResult;

            // Assert
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.AreEqual("SachDaThamGia", result.RouteValues["action"]);
            Assert.AreEqual(1, result.RouteValues["id"]);
        }
    }
}
