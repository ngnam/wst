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
    
    public partial class ChiTietDonHang
    {
        public int IdChiTiet { get; set; }
        public string MaMatHang { get; set; }
        public string TenMatHang { get; set; }
        public Nullable<int> GiaCa { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public Nullable<int> DonHangId { get; set; }
    
        public virtual DonDatHang DonDatHang { get; set; }
    }
}