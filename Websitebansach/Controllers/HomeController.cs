using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Models;

namespace Websitebansach.Controllers
{
    public class HomeController : Controller
    {
        public QuanLyBanSachEntities db = new QuanLyBanSachEntities();

        public ActionResult Index(int? page, string sortOption)
        {
            int pageSize = 9; // Số sách hiển thị trên mỗi trang
            int pageNumber = page ?? 1;

            // Lấy danh sách sách
            var sachQuery = db.Saches.AsQueryable();

            // Xử lý sắp xếp theo lựa chọn
            switch (sortOption)
            {
                case "PriceAsc": // Giá từ thấp đến cao
                    sachQuery = sachQuery.OrderBy(s => s.GiaBan);
                    break;

                case "PriceDesc": // Giá từ cao đến thấp
                    sachQuery = sachQuery.OrderByDescending(s => s.GiaBan);
                    break;

                case "AZ": // Tên sách từ A-Z
                    sachQuery = sachQuery.OrderBy(s => s.TenSach);
                    break;

                case "BestSelling": // Sách bán chạy
                    var bestSellingBooks = db.ChiTietDonHangs
                                             .Where(ct => ct.DonHang.DaThanhToan == 1 && ct.DonHang.TinhTrangGiaoHang == 1)
                                             .GroupBy(ct => ct.MaSach)
                                             .Select(g => new { MaSach = g.Key, TotalSold = g.Sum(ct => ct.SoLuong ?? 0) })
                                             .OrderByDescending(g => g.TotalSold)
                                             .Select(g => g.MaSach)
                                             .ToList();

                    sachQuery = sachQuery.ToList()
                                         .Where(s => bestSellingBooks.Contains(s.MaSach))
                                         .OrderBy(s => bestSellingBooks.IndexOf(s.MaSach)) // Sắp xếp cần thiết
                                         .AsQueryable();
                    break;

                default: // Mặc định
                    sachQuery = sachQuery.Where(s => s.Moi == 1).OrderBy(s => s.TenSach); // Sắp xếp mặc định
                    break;
            }

            // Truyền dữ liệu sắp xếp sang ViewBag để giữ trạng thái
            ViewBag.SortOption = sortOption;

            // Trả về View với phân trang
            return View(sachQuery.ToPagedList(pageNumber, pageSize));
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        
       
    }
}