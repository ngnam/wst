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
    
    public partial class MatHang
    {
        public int MatHangId { get; set; }
        public string MaMatHang { get; set; }
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
        public string MaGianHang { get; set; }
        public Nullable<int> GianHangId { get; set; }
    
        public virtual GianHang GianHang { get; set; }
    }
}