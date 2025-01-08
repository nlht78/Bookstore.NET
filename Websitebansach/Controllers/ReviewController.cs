using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Websitebansach.Models;

namespace Websitebansach.Controllers
{
    public class ReviewController : Controller
    {
        // Tạo danh sách tạm thời lưu đánh giá
        private static List<Review> reviews = new List<Review>();

        // GET: Review/Index
        public ActionResult Index()
        {
            // Kiểm tra nếu danh sách đánh giá trống, thêm một số bình luận mặc định
            if (!reviews.Any())
            {
                // Thêm một số đánh giá mặc định
                reviews.Add(new Review
                {
                    UserName = "Nguyễn Văn Thịnh",
                    Comment = "Sản phẩm tuyệt vời, tôi rất thích!",
                    Rating = 5,
                    CreatedAt = DateTime.Now.AddDays(-1) // Đánh giá cách đây 1 ngày
                });
                reviews.Add(new Review
                {
                    UserName = "Trần Thị Triều",
                    Comment = "Giá cả hợp lý và chất lượng tốt.",
                    Rating = 4,
                    CreatedAt = DateTime.Now.AddDays(-2) // Đánh giá cách đây 2 ngày
                });
                reviews.Add(new Review
                {
                    UserName = "Lê Hoàng Hiếu Thịnh",
                    Comment = "Dịch vụ giao hàng nhanh, sản phẩm đẹp.",
                    Rating = 4,
                    CreatedAt = DateTime.Now.AddDays(-3) // Đánh giá cách đây 3 ngày
                });
            }

            // Trả về danh sách đánh giá
            return View(reviews);
        }

        // POST: Review/SubmitReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReview(Review review)
        {
            if (ModelState.IsValid)
            {
                // Thiết lập thời gian tạo cho đánh giá
                review.CreatedAt = DateTime.Now;

                // Thêm đánh giá vào danh sách tạm thời
                reviews.Add(review);

                // Thông báo thành công
                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";

                // Chuyển hướng về trang danh sách đánh giá
                return RedirectToAction("Index");
            }

            // Nếu đánh giá không hợp lệ, quay lại form đánh giá
            return View("Index");
        }
    }
}
