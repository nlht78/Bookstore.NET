using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Websitebansach.Models
{
    [MetadataTypeAttribute(typeof(SachMetadata))]
    public partial class Sach
    {
        internal sealed class SachMetadata
        {
            [Required(ErrorMessage = "Vui lòng nhập dữ liệu cho trường này")]
            [Display(Name = "Mã sách")]
            public int MaSach { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập dữ liệu cho trường này")]
            [Display(Name = "Tên sách")]
            public string TenSach { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập dữ liệu cho trường này")]
            [Display(Name = "Giá bán")]
            public Nullable<decimal> GiaBan { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập dữ liệu cho trường này")]
            [Display(Name = "Mô tả")]
            public string MoTa { get; set; }

            
            [Display(Name = "Ảnh bìa")]
            public string AnhBia { get; set; }

            [Display(Name = "Ngày Cập Nhật")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> NgayCapNhat { get; set; }


            [Required(ErrorMessage = "Vui lòng nhập dữ liệu cho trường này")]
            [Display(Name = "Số lượng tồn")]
            public Nullable<int> SoLuongTon { get; set; }

          
            [Display(Name = "Mã nhà xuất bản")]
            public Nullable<int> MaNXB { get; set; }

            [Display(Name = "Mã chủ đề")]
            public Nullable<int> MaChuDe { get; set; }

          
            [Display(Name = "Mới")]
            public Nullable<int> Moi { get; set; }

        }
    }
}