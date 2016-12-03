using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebSieuThi.Models
{
    // cai api 1 day
    public class ListSieuThi
    {
        public int SieuThiId { get; set; }
        public string LoaiHinhKD { get; set; }
        public string TenSieuThi { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string ThanhPho { get; set; }
        public string CuocPhiVC { get; set; }
        public string DSAnh { get; set; }
        public string email { get; set; }
        public string pass { get; set; }
        public double KhoangCach { get; set; }
        public string AnhIcon { get; set; }
        public IEnumerable<ThongBaoST> ListThongBao { get; set; }

        public class ThongBaoST
        {
            public int ThongBaoID { get; set; }
            public int MaUser { get; set; }
            public string TenSieuThi { get; set; }
            public string NDThongBao { get; set; }
        }
    }



    public class ListThongBao
    {
        public int IdThongBao { get; set; }
        public string TenSTThongBao { get; set; }
        public string NDThongBao { get; set; }
        public string AnhSTThong { get; set; }
        public int SieuThiDkNhanTB { get; set; }
    }

    //public class ListGianHang
    //{
    //    public int ghId { get; set; }
    //    public int TenGianHang { get; set; }
    //    public ICollection<GianHangST> ListGianHang { get; set; }
    //    public ICollection<MatHangST> ListMatHang { get; set; }
    //}

    public class GianHangST
    {
        public int IdGh { get; set; }
        public string MaGh { get; set; }
        public string TenGh { get; set; }
    }

    //public class ListMatHangUT
    //{
    //    public int? IdSieuThiUaThich { get; set; }
    //    public string TenSieuThiUaThich { get; set; }
    //    public string TenMatHangUaThich { get; set; }
    //    public int? GiaCaMHUT { get; set; }
    //    public string HinhAnhMHUT { get; set; }
    //    public string TrangThai { get; set; }
    //    public string CuocPhiVC { get; set; }

    //}

    //public class ListMatHangOfSieuThi
    //{
    //    public int? IdSieuThi { get; set; }
    //    public string TenSieuThi { get; set; }
    //    public int? idGianHang { get; set; }
    //    public string TenGianHang { get; set; }       
    //    public ICollection<MatHangST> ListMatHang { get; set; }
    //}

    public class Reg
    {
        public string RegId { get; set; }
    }

    public class ViewModel
    {
        public int SieuThiId { get; set; }
        public string Title { get; set; }
    }

    public class gcm
    {
        public string appId { get; set; }
        public string sendderId { get; set; }
    }

    public class Museum
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class UserDTO
    {
        public int MaUser { get; set; }
        [Required]
        public string UserEmail { get; set; }
        public string RegId { get; set; }
        public string IdSieuThiThongBao { get; set; }
    }

    public class getGianHang
    {
        public int GianHangId { get; set; }
        public string MaGianHang { get; set; }
        public string TenGianHang { get; set; }
        public Nullable<int> SieuThiId { get; set; }
        public string AnhGianHang { get; set; }
        public bool? isGianHangChung { get; set; }
    }

    public class getMatHang
    {
        public int MatHangId { get; set; }
        public string TenMatHang { get; set; }
        public string AnhDaiDien { get; set; }
        public string TrangThai { get; set; }
        public string LoaiHang { get; set; }
        public bool? isMatHangChung { get; set; }
        public int? GianHangId { get; set; }
        public string MaGianHang { get; set; }
    }

}