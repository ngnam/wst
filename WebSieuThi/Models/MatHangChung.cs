//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebSieuThi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatHangChung
    {
        public int MatHangChungId { get; set; }
        public string MaMatHangChung { get; set; }
        public string TenMatHang { get; set; }
        public string AnhDaiDien { get; set; }
        public string DSHinhAnh { get; set; }
        public string MoTa { get; set; }
        public Nullable<System.DateTime> NgayBDKM { get; set; }
        public Nullable<System.DateTime> NgayKTKM { get; set; }
        public Nullable<int> GiaCa { get; set; }
        public string TrangThai { get; set; }
        public string LoaiHang { get; set; }
        public Nullable<int> PhanTramKM { get; set; }
        public string MaGianHangChung { get; set; }
        public Nullable<int> GianHangChungId { get; set; }
    
        public virtual GianHangChung GianHangChung { get; set; }
    }
}