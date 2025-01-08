using Microsoft.VisualStudio.TestTools.UnitTesting;
using Websitebansach.Controllers;
using Websitebansach.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Websitebansach.Tests
{
    [TestClass]
    public class ReviewControllerTests
    {
        private ReviewController _controller;

        // Khởi tạo controller trước mỗi lần kiểm thử
        [TestInitialize]
        public void TestInitialize()
        {
            _controller = new ReviewController();
        }

        // Kiểm tra phương thức Index trả về ViewResult và danh sách đánh giá
        [TestMethod]
        public void Index_Returns_ViewResult_With_Reviews()
        {
            // Act: Gọi action Index
            var result = _controller.Index() as ViewResult;

            // Assert: Kiểm tra ViewResult và model là danh sách Review
            Assert.IsNotNull(result);
            var model = result.Model as List<Review>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count > 0); // Kiểm tra ít nhất có một đánh giá
        }

        // Kiểm tra phương thức SubmitReview khi đánh giá hợp lệ
        [TestMethod]
        public void SubmitReview_ValidReview_Redirects_To_Index_With_SuccessMessage()
        {
            // Arrange: Tạo một review hợp lệ
            var review = new Review
            {
                UserName = "Nguyễn Văn A",
                Comment = "Sản phẩm tuyệt vời!",
                Rating = 5
            };

            // Act: Gửi POST yêu cầu với review hợp lệ
            var result = _controller.SubmitReview(review) as RedirectToRouteResult;

            // Assert: Kiểm tra việc chuyển hướng và thông báo thành công
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsTrue(_controller.TempData.ContainsKey("SuccessMessage"));
            Assert.AreEqual("Cảm ơn bạn đã đánh giá!", _controller.TempData["SuccessMessage"]);
        }

        // Kiểm tra khi gửi review không hợp lệ
        [TestMethod]
        public void SubmitReview_InvalidReview_Returns_To_Index()
        {
            // Arrange: Tạo review không hợp lệ (chưa nhập tên người dùng)
            var review = new Review
            {
                Comment = "Sản phẩm không tốt",
                Rating = 3
            };

            // Thêm lỗi vào ModelState để mô phỏng lỗi khi submit
            _controller.ModelState.AddModelError("UserName", "Tên không được để trống");

            // Act: Gửi POST yêu cầu với review không hợp lệ
            var result = _controller.SubmitReview(review) as ViewResult;

            // Assert: Kiểm tra quay lại trang Index mà không thay đổi
            Assert.IsNotNull(result);
            var model = result.Model as List<Review>;
            Assert.IsTrue(model.Count > 0); // Kiểm tra danh sách đánh giá không bị xóa
        }
    }
}
