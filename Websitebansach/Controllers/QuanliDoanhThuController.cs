using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Websitebansach.Filters;
using Websitebansach.Models;

namespace Websitebansach.Controllers
{
    [AdminAuthorize]
    public class QuanliDoanhThuController : Controller
    {
        private QuanLyBanSachEntities db = new QuanLyBanSachEntities();




        // GET: QuanliDoanhThu
        public ActionResult ThongKeDoanhThu(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không có tham số, mặc định là từ đầu năm đến ngày hiện tại
            startDate = startDate ?? new DateTime(DateTime.Now.Year, 1, 1);
            endDate = endDate ?? DateTime.Now;

            // Tổng doanh thu
            var doanhThu = db.ChiTietDonHangs
                             .Where(ct => ct.DonHang.DaThanhToan == 1
                                          && ct.DonHang.NgayDat >= startDate
                                          && ct.DonHang.NgayDat <= endDate)
                             .Sum(ct => ct.SoLuong * ct.DonGia);

            // Tổng số đơn hàng
            var tongSoDonHang = db.DonHangs
                                  .Where(dh => dh.DaThanhToan == 1
                                               && dh.NgayDat >= startDate
                                               && dh.NgayDat <= endDate)
                                  .Count();

            // Tổng số lượng sách bán được
            var tongSoLuongSach = db.ChiTietDonHangs
                                    .Where(ct => ct.DonHang.DaThanhToan == 1
                                                 && ct.DonHang.NgayDat >= startDate
                                                 && ct.DonHang.NgayDat <= endDate)
                                    .Sum(ct => ct.SoLuong);

            ViewBag.DoanhThu = doanhThu;
            ViewBag.TongSoDonHang = tongSoDonHang;
            ViewBag.TongSoLuongSach = tongSoLuongSach;
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View();
        }

        // Chức năng xuất báo cáo doanh thu (CSV)
        public ActionResult XuatBaoCaoCSV(DateTime startDate, DateTime endDate)
        {
            // Tính toán dữ liệu thống kê như trong ThongKeDoanhThu
            var doanhThu = db.ChiTietDonHangs
                             .Where(ct => ct.DonHang.DaThanhToan == 1
                                          && ct.DonHang.NgayDat >= startDate
                                          && ct.DonHang.NgayDat <= endDate)
                             .Sum(ct => ct.SoLuong * ct.DonGia);

            var tongSoDonHang = db.DonHangs
                                  .Where(dh => dh.DaThanhToan == 1
                                               && dh.NgayDat >= startDate
                                               && dh.NgayDat <= endDate)
                                  .Count();

            var tongSoLuongSach = db.ChiTietDonHangs
                                    .Where(ct => ct.DonHang.DaThanhToan == 1
                                                 && ct.DonHang.NgayDat >= startDate
                                                 && ct.DonHang.NgayDat <= endDate)
                                    .Sum(ct => ct.SoLuong);

            // Xuất file CSV
            var fileName = $"BaoCaoDoanhThu_{startDate:yyyyMMdd}_den_{endDate:yyyyMMdd}.csv";
            var csv = new StringWriter();
            csv.WriteLine("Thời gian, Tổng Doanh Thu, Tổng Số Đơn Hàng, Tổng Số Lượng Sách");
            csv.WriteLine($"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}, {doanhThu}, {tongSoDonHang}, {tongSoLuongSach}");

            return File(new System.Text.UTF8Encoding().GetBytes(csv.ToString()), "text/csv", fileName);
        }

        // Chức năng xuất báo cáo doanh thu (Excel)
        public ActionResult XuatBaoCaoExcel(DateTime? startDate, DateTime? endDate)
        {
            // Cấu hình license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Lấy dữ liệu doanh thu
            startDate = startDate ?? new DateTime(DateTime.Now.Year, 1, 1);
            endDate = endDate ?? DateTime.Now;

            var doanhThu = db.ChiTietDonHangs
                             .Where(ct => ct.DonHang.DaThanhToan == 1
                                          && ct.DonHang.NgayDat >= startDate
                                          && ct.DonHang.NgayDat <= endDate)
                             .Sum(ct => ct.SoLuong * ct.DonGia);

            var tongSoDonHang = db.DonHangs
                                  .Count(dh => dh.DaThanhToan == 1
                                               && dh.NgayDat >= startDate
                                               && dh.NgayDat <= endDate);

            var tongSoLuongSach = db.ChiTietDonHangs
                                    .Where(ct => ct.DonHang.DaThanhToan == 1
                                                 && ct.DonHang.NgayDat >= startDate
                                                 && ct.DonHang.NgayDat <= endDate)
                                    .Sum(ct => ct.SoLuong);

            // Xuất Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo Cáo Doanh Thu");

                // Thêm tiêu đề
                worksheet.Cells["A1"].Value = "Báo Cáo Doanh Thu";
                worksheet.Cells["A2"].Value = $"Từ ngày {startDate:yyyy-MM-dd} đến ngày {endDate:yyyy-MM-dd}";

                // Header
                worksheet.Cells["A4"].Value = "Tổng Doanh Thu";
                worksheet.Cells["B4"].Value = doanhThu;

                worksheet.Cells["A5"].Value = "Tổng Số Đơn Hàng";
                worksheet.Cells["B5"].Value = tongSoDonHang;

                worksheet.Cells["A6"].Value = "Tổng Số Lượng Sách Bán Được";
                worksheet.Cells["B6"].Value = tongSoLuongSach;

                // Định dạng bảng
                worksheet.Cells["A1:B1"].Merge = true;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;

                // Xuất file Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}
