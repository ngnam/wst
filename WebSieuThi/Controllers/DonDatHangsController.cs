using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebSieuThi.Models;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace WebSieuThi.Controllers
{
    [RoutePrefix("api/donhang")]
    public class DonDatHangsController : ApiController
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        public class DonDatHangCuaNguoiDung
        {    
            [Required]
            public int SieuThiId { get; set; }
            [Required]
            public int MaUser { get; set; }
            [Required]
            public string HoTen { get; set; }
            [Required]
            public string SDT { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string DiaChi { get; set; }
            public int TongCong { get; set; }
            public ICollection<ChiTietDonHangNgDung> chitietdonhang { get; set; }
        }

        public class ChiTietDonHangNgDung
        {            
            public string MaMatHang { get; set; }
            public string TenMatHang { get; set; }
            public Nullable<int> GiaCa { get; set; }
            public Nullable<int> SoLuong { get; set; }
            public int DonHangId { get; set; }
        }

        public class PostDonHangResponse
        {
            public string Result { get; set; }
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
        }

        // GET: api/DonDatHangs
        public IQueryable<DonDatHang> GetDonDatHangs()
        {
            return db.DonDatHangs;
        }

        // GET: api/DonDatHangs/5
        
        public async Task<IHttpActionResult> GetDonDatHang(int id)
        {
            DonDatHang donDatHang = await db.DonDatHangs.FindAsync(id);
            if (donDatHang == null)
            {
                return NotFound();
            }

            return Ok(donDatHang);
        }
                
        // POST: api/DonDatHangs
        [Route("ThemDonHang")]
        [HttpPost]
        [ResponseType(typeof(DonDatHang))]
        public async Task<IHttpActionResult> PostDonDatHang(DonDatHangCuaNguoiDung donDatHang)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //db.DonDatHangs.Add(donDatHang);
                //db.SaveChanges();
                // đoạn này để lưu đơn hàng vào đây này
                // Đầu tiên tao lấy thông tin đơn hàng m gửi để tạo mới 1 bảng ghi trong bảng đơn hàng,
                // tạo xong tao lấy id để gửi nốt thông tin vào bẳng chi tiết, có được số lượng và id mat hàng để tính thành tiên 
                // Cập nhật trường tổng cộng vào bảng đơn đặt hàng.
                //int? _donhangId;
                //bool _insert = false;
                DonDatHang _dondathang = new DonDatHang();
                _dondathang.HoTen = donDatHang.HoTen;
                _dondathang.SDT = donDatHang.SDT;
                _dondathang.Email = donDatHang.Email;
                _dondathang.DiaChi = donDatHang.DiaChi;
                _dondathang.TongCong = donDatHang.TongCong;
                _dondathang.SieuThiId = donDatHang.SieuThiId;
                _dondathang.MaUser = donDatHang.MaUser;
                _dondathang.NTDonHang = DateTime.Now;
                db.DonDatHangs.Add(_dondathang);
                await db.SaveChangesAsync();
                int newID = _dondathang.DonHangId;
                ChiTietDonHang _chitietDonHang = new ChiTietDonHang();
                
                foreach (var item in donDatHang.chitietdonhang)
                {
                    _chitietDonHang.MaMatHang = item.MaMatHang;
                    _chitietDonHang.TenMatHang = item.TenMatHang;
                    _chitietDonHang.GiaCa = item.GiaCa;
                    _chitietDonHang.SoLuong = item.SoLuong;
                    _chitietDonHang.DonHangId = newID;
                    db.ChiTietDonHangs.Add(_chitietDonHang);
                    await db.SaveChangesAsync();
                }

                // GỬi thông tin đơn hàng qua email cho sieu thi trên
                try
                {
                    var strThongTinDH = string.Format("Mã đơn hàng: {0} <br/>. Họ tên: {1} <br/>. Số điện thoại: {2}<br />. Email: {3} <br />. Địa chỉ: {4} <br/>. Tổng cộng: {5} <br/>.", _dondathang.DonHangId, _dondathang.HoTen, _dondathang.SDT, _dondathang.Email, _dondathang.DiaChi, _dondathang.TongCong);
                    var _chitietDHs = db.ChiTietDonHangs.Where(x => x.DonHangId == _dondathang.DonHangId).ToList();
                    System.Text.StringBuilder chitietdhs = new System.Text.StringBuilder();
                    if (_chitietDHs.Count > 0)
                    {
                        foreach (var item in _chitietDHs)
                        {
                            var _str11 = string.Format("Mã mặt hàng: {0} <br/>. Tên mặt hàng: {1} <br/>. Giá cả: {2} <br />. Số lượng: {3}. <br/>", item.MaMatHang, item.TenMatHang, item.GiaCa, item.SoLuong);
                            chitietdhs.Append(_str11 + "<br/>");
                        }
                    }
                    string emailSieuthi = db.SieuThis.Find(_dondathang.SieuThiId).Email;
                    Helpers.Config.Sendmail(ConfigurationManager.AppSettings["yourEmail"], ConfigurationManager.AppSettings["yourPass"], emailSieuthi, "Thông tin đơn hàng " + _dondathang.HoTen, strThongTinDH + chitietdhs.ToString());
                
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath ("../" + "log.txt"));
                    sw.WriteLine(DateTime.Now.ToString() + ": " + ex.ToString() );
                    sw.Close();
                }
                
                return Ok(new { isSuccess = true });
            }
            catch
            {

                return Ok(new { isSuccess = false });
            }
            

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DonDatHangExists(int id)
        {
            return db.DonDatHangs.Count(e => e.DonHangId == id) > 0;
        }
    }
}