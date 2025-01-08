using System;

namespace Websitebansach.Models
{
    public class Review
    {
        public int Id { get; set; }  // ID của đánh giá
        public string UserName { get; set; }  // Tên người đánh giá
        public string Comment { get; set; }  // Bình luận
        public int Rating { get; set; }  // Điểm đánh giá (ví dụ: từ 1 đến 5)
        public DateTime CreatedAt { get; set; }  // Thời gian đánh giá
    }
}
