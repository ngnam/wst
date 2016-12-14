using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSieuThi.Models
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tài khoản")]
        [Display(Name = "Chọn tài khoản?")]
        public bool TypeAccount { get; set; }
    }

    public class RegisterViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} không được để trống.")]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhất {2} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Pass { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Pass", ErrorMessage = "Mật khẩu xác nhận không đúng.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Ảnh giấy phép kinh doanh")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string Picture_GP_KD { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public string AnhIcon { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Actived { get; set; }
        [Display(Name = "Địa chỉ/quốc gia")]
        public string Country { get; set; }
        [Display(Name = "Tên hệ thống")]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        public string TenHeThong { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string SDT { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Loại hình kinh doanh")]
        public string LoaiHinhKD { get; set; }
    }

    public class HeThongModel
    {
        public int HethongId { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }        
        [Display(Name = "Ảnh giấy phép kinh doanh")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string Picture_GP_KD { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public string AnhIcon { get; set; }        
        [Display(Name = "Địa chỉ/quốc gia")]
        public string Country { get; set; }
        [Display(Name = "Tên hệ thống")]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        public string TenHeThong { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string SDT { get; set; }
        [Required(ErrorMessage = "{0} không được để trống.")]
        [Display(Name = "Loại hình kinh doanh")]
        public string LoaiHinhKD { get; set; }
    }

    public class SieuThiModel
    {
        public int SieuThiId { get; set; }
       
        [Display(Name = "Tên siêu thị")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string TenSieuThi { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập vào địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        public string DienThoai { get; set; }

        [Display(Name = "Cước phí vận chuyển")]
        public string CuocPhiVC { get; set; }

        [Display(Name = "Vị trí longlat")]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        public string longlat { get; set; }

        public int HeThongId { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email {get; set;}

        [Display(Name = "Giờ mở cửa")]
        public string GioMoCua { get; set; }

    }

    public class NewSieuThiModel
    {
        public int SieuThiId { get; set; }

        [Display(Name = "Tên siêu thị")]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        public string TenSieuThi { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string DienThoai { get; set; }

        [Display(Name = "Cước phí vận chuyển")]
        public string CuocPhiVC { get; set; }

        [Required(ErrorMessage = "{0} không được để trống.")]
        [Display(Name="Vị trí chính xác")]
        public string longlat { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhất {2} ký tự.", MinimumLength = 6)]
        [Display(Name = "Mật khẩu")]
        public string Pass { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Pass", ErrorMessage = "Mật khẩu xác nhận không đúng.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Giờ mở cửa")]
        public string GioMoCua { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mât khẩu hiện tại.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không chính xác.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordSieuthiViewModel
    {
        public int SieuthiId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mât khẩu hiện tại.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không chính xác.")]
        public string ConfirmPassword { get; set; }
    }

    public class GianHangChungModel
    {
        public int GianHangChungId { get; set; }
        [Display(Name = "Mã gian hàng")]
        [Required(ErrorMessage="{0} không được để trống")]
        public string MaGianHangChung { get; set; }
        [Display(Name = "Tên gian hàng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string TenGianHangChung { get; set; }
        public int? HeThongId { get; set; }
        [Display(Name = "Ảnh gian hàng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string AnhGianHang { get; set; }
    }

    public class GianHangModel
    {
        public int GianHangId { get; set; }
        [Display(Name = "Mã gian hàng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string MaGianHang { get; set; }
        [Display(Name = "Tên gian hàng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string TenGianHang { get; set; }
        public int? SieuthiId { get; set; }
        [Display(Name = "Ảnh gian hàng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string AnhGianHang { get; set; }
    }

    public class MatHangChungModel
    {
        public int HeThongId { get; set; }
        public int MatHangChungId { get; set; }
        [Display(Name = "Mã mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string MaMatHangChung { get; set; }
        [Display(Name = "Tên mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string TenMatHang { get; set; }
        [Display(Name = "Ảnh mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string AnhDaiDien { get; set; }
        [Display(Name="Danh sách hình ảnh")]
        public string DsHinhAnh { get; set; }
        [Display(Name = "Thông tin mô tả")]
        public string MoTa { get; set; }
        [Display(Name = "Bắt đầu từ")]
        [DataType(DataType.Date)]
        public DateTime? NgayBDKM { get; set; }
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? NgayKTKM { get; set; }
        [Display(Name = "Giá cả mặt hàng")]
        public int GiaCa { get; set; }
        [Display(Name = "Trạng thái mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string TrangThai { get; set; }
        [Required(ErrorMessage="{0} không được để trống.")]
        [Display(Name = "Loại hàng")]
        public string LoaiHang { get; set; }
        [Display(Name = "Phần trăm khuyến mại")]
        public int? PhanTramKM { get; set; }        
        [Required(ErrorMessage="Gian hàng không được để trống.")]
        public string strGianHangChung { get; set; }  //idghchung,maghchung  
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một hình ảnh.")]
        public string indivanh1 { get; set; }        
        public string indivanh2 { get; set; }
        public string indivanh3 { get; set; }
        public string indivanh4 { get; set; }
    }

    public class MatHangModel
    {
        public int HeThongId { get; set; }
        public int MatHangId { get; set; }
        [Display(Name = "Mã mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string MaMatHang { get; set; }
        [Display(Name = "Tên mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string TenMatHang { get; set; }
        [Display(Name = "Ảnh mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string AnhDaiDien { get; set; }
        public string DsHinhAnh { get; set; }
        [Display(Name = "Thông tin mô tả")]
        public string MoTa { get; set; }
        [Display(Name = "Bắt đầu từ")]
        [DataType(DataType.Date)]
        public DateTime? NgayBDKM { get; set; }
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? NgayKTKM { get; set; }
        [Display(Name = "Giá cả mặt hàng")]
        public int GiaCa { get; set; }
        [Display(Name = "Trạng thái mặt hàng")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string TrangThai { get; set; }
        [Required(ErrorMessage="{0} không được để trống.")]
        [Display(Name = "Loại hàng")]
        public string LoaiHang { get; set; }
        [Display(Name = "Phần trăm khuyến mại")]
        public int? PhanTramKM { get; set; }
        [Required(ErrorMessage="Gian hàng không được để trống.")]
        public string strGianHang { get; set; }  //idgh,magh
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một hình ảnh.")]
        public string indivanh1 { get; set; }
        public string indivanh2 { get; set; }
        public string indivanh3 { get; set; }
        public string indivanh4 { get; set; }
    }

    public class SuKienChungModel
    {
        public int? HeThongId { get; set; }
        public int SuKienChungId { get; set; }
        [Display(Name = "Danh sách hình ảnh")]
        public string DsAnh { get; set; }
        [Display(Name = "Tên sự kiện")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string TDSuKien { get; set; }
        [Display(Name = "Sự kiện")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string NDSuKien { get; set; }
       
        public bool ConfirmSend { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? NgayBD { get; set; }
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? NgayKT { get; set; }
    }

    public class SuKienModel
    {
        public int SuKienId { get; set; }
        public int? SieuThiId { get; set; }
        [Display(Name = "Danh sách hình ảnh")]
        public string DsAnh { get; set; }
        [Display(Name = "Tên sự kiện")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string TDSuKien { get; set; }
        [Display(Name = "Sự kiện")]
        [Required(ErrorMessage="{0} không được để trống.")]
        public string NDSuKien { get; set; }        
        public bool ConfirmSend { get; set; }
        public string indivanh1 { get; set; }
        public string indivanh2 { get; set; }
        public string indivanh3 { get; set; }
        public string indivanh4 { get; set; }
        public string indivanh5 { get; set; }
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? NgayBD { get; set; }
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? NgayKT { get; set; }
    }

}