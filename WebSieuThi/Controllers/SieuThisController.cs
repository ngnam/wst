using Helpers;
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

namespace WebSieuThi.Controllers
{
    
    [RoutePrefix("api/sieuthi")]
    public class SieuThisController : ApiController
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        public class sieuThiDTO
        {
            public int SieuThiId { get; set; }
            public string LoaiHinhKD { get; set; }
            public string TenSieuThi { get; set; }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }
            public string CuocPhiVC { get; set; }
        }

        // GET: api/SieuThis
        [HttpGet]
        [Route("getAll")]
        [ResponseType(typeof(sieuThiDTO))]
        public IEnumerable<sieuThiDTO> GetSieuThis()
        {
            var _sieuThi = from s in db.SieuThis
                           select new sieuThiDTO()
                           {
                               SieuThiId = s.SieuThiId,
                               LoaiHinhKD = s.HeThong.LoaiHinhKD,
                               TenSieuThi = s.TenSieuThi,
                               DiaChi = s.DiaChi,
                               DienThoai = s.DienThoai,
                               CuocPhiVC = s.CuocPhiVC,
                           };
            return _sieuThi;
        }

        public class sieuthithongbaodto
        {
            public int SieuThiId { get; set; }
            public string LoaiHinhKD { get; set; }
            public string TenSieuThi { get; set; }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }
            public string CuocPhiVC { get; set; }
            public string AnhIcon { get; set; }
            public string LongLat { get; set; }
            public bool NhanThongBao { get; set; }
            public double KhoangCachST { get; set; }
            public int HeThongId { get; set; }
            public string GioMoCua {get; set;}
            public string soluongsukien { get; set; }
            //public IEnumerable<ThongBaoOfST> ListThongBao { get; set; }
        }

        public class sieuthinhanthongbao
        {
            public int SieuThiId { get; set; }
            public string LoaiHinhKD { get; set; }
            public string TenSieuThi { get; set; }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }
            public string CuocPhiVC { get; set; }
            public string AnhIcon { get; set; }            
        }

        //public class ThongBaoOfST
        //{            
        //    public string TenSieuThiTb { get; set; }
        //    public string NDThongBao { get; set; }
        //    public string AnhSieuThiTb { get; set; }
        //}

        //[HttpPost]
        //[Route("xoaguithongbao/mauser={mauser:int}")]
        //public async Task<IHttpActionResult> XoaGuiThongBao(int mauser)
        //{
        //    try
        //    {
        //        var _guithongbao = await db.GuiThongBaos.Where(x => x.MaUser == mauser).ToListAsync();
        //        if (_guithongbao.Count > 0)
        //        {
        //            foreach (var item in _guithongbao)
        //            {
        //                db.GuiThongBaos.Remove(item);
        //                await db.SaveChangesAsync();
        //            }
        //        }
        //        return Ok(new { isSuccess = true });
        //    }
        //    catch
        //    {
        //        return Ok(new { isSuccess = false });
        //    }
        //}

        [HttpGet]
        [Route("ViTri={LatLongUser}/KhoangCach={kc:double}/mauser={mauser:int}")]
        [ResponseType(typeof(sieuthithongbaodto))]
        public IHttpActionResult GetSieuThi(string LatLongUser, double kc, int mauser)
        {
            var _user = db.Users.Where(x => x.MaUser == mauser).FirstOrDefault();
            if (_user == null)
            {
                return Ok(new List<sieuthithongbaodto> { });
            }
            String[] _lst2 = null;
            if (_user.DSSieuThiThongBao != "" && _user.DSSieuThiThongBao != null)
            {
                _lst2 = _user.DSSieuThiThongBao.Split(',');
                //return Ok(new List<sieuthithongbaodto> {});
            }
            
            double vtLat = Config.GetLongLat(LatLongUser).Latitude;
            double vtLong = Config.GetLongLat(LatLongUser).Longitude;
            var data = db.SieuThis.ToList();
            List<sieuthithongbaodto> _sieuthi = new List<sieuthithongbaodto>();
            
            foreach (var st in data)
            {
                double khoangcach = Config.TinhKhoangCachNguoiDungDenSieuThi(vtLat, vtLong, Config.GetLongLat(st.longlat).Latitude, Config.GetLongLat(st.longlat).Longitude);
                if (khoangcach <= kc * 1000)
                {
                    var sieuthi = new sieuthithongbaodto()
                    {
                        SieuThiId = st.SieuThiId,
                        LoaiHinhKD = st.HeThong.LoaiHinhKD,
                        TenSieuThi = st.TenSieuThi,
                        DiaChi = st.DiaChi,
                        DienThoai = st.DienThoai,
                        CuocPhiVC = st.CuocPhiVC,
                        AnhIcon = st.HeThong.AnhIcon,
                        LongLat = st.longlat,
                        NhanThongBao = _lst2 != null ? _lst2.Any(x=>x.Trim() == st.SieuThiId.ToString()) ? true : false : false,
                        KhoangCachST = khoangcach,
                        HeThongId = st.HeThongId != default(int) ? (int)st.HeThongId : 0,
                        GioMoCua = st.GioMoCua,
                        soluongsukien = (st.SuKiens.Where(x => DateTime.Now.AddDays(1).AddSeconds(-1) <= x.NgayKT).Count() + st.HeThong.SuKienChungs.Where(y => DateTime.Now.AddDays(1).AddSeconds(-1) <= y.NgatKT).Count()).ToString()
                    };
                    _sieuthi.Add(sieuthi);
                }
            }
            _sieuthi = _sieuthi.OrderBy(x => x.KhoangCachST).ToList();

            return Ok(_sieuthi);
        }



       // public IHttpActionResult GetThongBaos()


        public class MatHangST
        {
            public int? SieuThiId { get; set; }
            public string MaGianHang { get; set; }
            //public int? GianHangId { get; set; }
            public string TenSieuThi { get; set; }
            public string MaMatHang { get; set; }
            public string TenMatHang { get; set; }
            public string DSHinhAnh { get; set; }
            public string MoTa { get; set; }
            public string PhanTramKM { get; set; }
            public int? GiaCa { get; set; }
            public string TrangThai { get; set; }
            public string LoaiHang { get; set; }
            public bool? LaHangKM { get; set; }
        }

        private int Splitstring0(string x) {
            return int.Parse(x.Split('_')[0]);
        }

        private string Splitstring1(string x)
        {
            return x.Split('_')[1];
        }

        [HttpGet]
        [Route("MatHangUaThich/ofUser={mauser:int}")]
        [ResponseType(typeof(MatHangST))]
        public async Task<IHttpActionResult> GetMhUaThichOfUser(int mauser)
        {
            
            var _user = db.Users.Where(x => x.MaUser == mauser).FirstOrDefault();
            if (_user == null)
            {
                return Ok(new List<MatHangST> {});
            }

            var dsmathanguathich = _user.DSMatHangUaThich != null && _user.DSMatHangUaThich != "" ? _user.DSMatHangUaThich : "";
            if (dsmathanguathich == "")
            {
                return Ok(new List<MatHangST> { });
            }
            List<string> lstItem = new List<string>();
            List<int> lstSieuthiId = new List<int>();
            if (dsmathanguathich != null && dsmathanguathich != "")
            {
                //1_1-MHTPx, 1_1-MHTPy, 1_1-MHTP3 
                foreach (var item in dsmathanguathich.Split(','))
                {
                    lstItem.Add(item);
                    //lstSieuthiId.Add(int.Parse(item.Split('_')[0]));
                } 
            }

            List<MatHangST> dsmathang = new List<MatHangST>();
            List<MatHangST> dsmathangchung = new List<MatHangST>();
            foreach (var c in lstItem)
            {
                string sItem = c.Split('_')[1];          
                int istId = int.Parse(c.Split('_')[0]);
                var mh = db.MatHangs.Where(x => x.MaMatHang == sItem).FirstOrDefault();

                if (mh != null)
                {
                    if (db.SieuThis.Where(x => x.SieuThiId == istId).FirstOrDefault().GianHangs.Any(y => y.MatHangs.Any(z => z.MaMatHang == mh.MaMatHang)))
                    {
                        dsmathang.Add(new MatHangST()
                        {
                            SieuThiId = istId,
                            TenSieuThi = db.SieuThis.Where(xx => xx.SieuThiId == istId).FirstOrDefault().TenSieuThi,
                            MaGianHang = mh.MaGianHang,
                            MaMatHang = mh.MaMatHang,
                            TenMatHang = mh.TenMatHang,
                            DSHinhAnh = mh.DSHinhAnh != null ? (mh.AnhDaiDien != null ? mh.AnhDaiDien + "," + mh.DSHinhAnh : mh.DSHinhAnh) : (mh.AnhDaiDien != null ? mh.AnhDaiDien : ""),
                            MoTa = mh.MoTa,
                            GiaCa = mh.GiaCa,
                            TrangThai = mh.TrangThai,
                            LoaiHang = mh.LoaiHang,
                            PhanTramKM = mh.PhanTramKM.ToString(),
                            LaHangKM = mh.NgayBDKM <= DateTime.UtcNow && mh.NgayKTKM >= DateTime.UtcNow ? true : false
                        });
                    }
                    
                }

                
                var mhc = db.MatHangChungs.Where(y => y.MaMatHangChung == sItem).FirstOrDefault();
                if (mhc != null)
                {
                    dsmathangchung.Add(new MatHangST()
                    {
                        SieuThiId = istId,
                        MaGianHang = mhc.GianHangChung.MaGianHangChung,
                        TenSieuThi = db.SieuThis.Where(xx=>xx.SieuThiId == istId).FirstOrDefault().TenSieuThi,
                        MaMatHang = mhc.MaMatHangChung,
                        TenMatHang = mhc.TenMatHang,
                        DSHinhAnh = mhc.DSHinhAnh != null ? (mhc.AnhDaiDien != null ? mhc.AnhDaiDien + "," + mhc.DSHinhAnh : mhc.DSHinhAnh) : (mhc.AnhDaiDien != null ? mhc.AnhDaiDien : ""),
                        MoTa = mhc.MoTa,
                        GiaCa = mhc.GiaCa,
                        TrangThai = mhc.TrangThai,
                        LoaiHang = mhc.LoaiHang,
                        PhanTramKM = mhc.PhanTramKM.ToString(),
                        LaHangKM = mhc.NgayBDKM <= DateTime.UtcNow && mhc.NgayKTKM >= DateTime.UtcNow ? true : false
                    });
                }

            }

            var data = dsmathang.Union(dsmathangchung, new MatHangUComparer());
            //var data = dsmathang.Concat(dsmathangchung);

            return Ok(data);
        }


        class MatHangUComparer : IEqualityComparer<MatHangST>
        {
            public bool Equals(MatHangST x, MatHangST y)
            {
                return x.MaMatHang == y.MaMatHang && x.SieuThiId == y.SieuThiId;
            }

            public int GetHashCode(MatHangST obj)
            {
                return obj.MaMatHang.GetHashCode();
            }
        }

        //api/sieuthi/dssieuthithongbao/ofUser=1
        [HttpGet]
        [Route("dssieuthithongbao/ofUser={mauser:int}")]
        [ResponseType(typeof(sieuthinhanthongbao))]
        public async Task<IHttpActionResult> GetDsSTThongBaoOfUser(int mauser)
        {
            using (var db = new sieuthiapiEntities())
            {
                var _user = db.Users.Where(x => x.MaUser == mauser).FirstOrDefault();
                if (_user == null)
                {
                    return Ok(new List<User> { });
                }
                var dssieuthithongbao = _user.DSSieuThiThongBao != null && _user.DSMatHangUaThich != "" ? _user.DSSieuThiThongBao : "";
                if (dssieuthithongbao == "")
                {
                    return Ok(new List<User> { });
                }
                List<sieuthinhanthongbao> _sieuthi = new List<sieuthinhanthongbao>();
                foreach (var item in dssieuthithongbao.Split(','))
                {
                    int _id = Convert.ToInt32(item.Trim());
                    var st = await db.SieuThis.FindAsync(_id);
                    var getSieuthi = new sieuthinhanthongbao()
                    {
                        SieuThiId = st.SieuThiId,
                        LoaiHinhKD = st.HeThong.LoaiHinhKD,
                        TenSieuThi = st.TenSieuThi,
                        DiaChi = st.DiaChi,
                        DienThoai = st.DienThoai,
                        CuocPhiVC = st.CuocPhiVC,
                        AnhIcon = st.HeThong.AnhIcon
                    };
                    _sieuthi.Add(getSieuthi);
                }

                return Ok(_sieuthi);

            }          
            
        }

        //[HttpGet]
        //[Route("MatHangUaThich/ofUser={mauser:int}")]
        //public IEnumerable<MatHangST> GetMhUaThichOfUser(int mauser)
        //{
        //    var dataMhChung = (from us in db.Users
        //                       where us.MaUser == mauser
        //                       select us.MaUser into us1
        //                       join mhut in db.MatHangUaThiches on us1 equals mhut.MaUser into mhutofuser
        //                       from mhut2 in mhutofuser
        //                       join st in db.SieuThis on mhut2.SieuthiIdUaThich equals st.SieuThiId into listst
        //                       from st2 in listst
        //                       join gh in db.GianHangChungs on mhut2.GianHangIdUaThich equals gh.GianHangChungId into listgh
        //                       from gh2 in listgh
        //                       join mh in db.MatHangChungs on mhut2.IdMatHang equals mh.MatHangChungId
        //                       select new MatHangST()
        //                       {
        //                           SieuThiId = st2.SieuThiId,
        //                           GianHangId = gh2.GianHangChungId,
        //                           TenSieuThi = st2.TenSieuThi,
        //                           MatHangId = mh.MatHangChungId,
        //                           MaMathang = mh.MaMatHangChung,
        //                           TenMatHang = mh.TenMatHang,
        //                           DSHinhAnh = mh.DSHinhAnh,
        //                           MoTa = mh.MoTa,
        //                           GiaCa = mh.GiaCa,
        //                           TrangThai = mh.TrangThai,
        //                           LoaiHang = mh.LoaiHang,
        //                           LaHangKM = mh.NgayBDKM <= DateTime.UtcNow && mh.NgayKTKM >= DateTime.UtcNow ? true : false
        //                       }).ToList();

        //    var dataMhRieng = (from us in db.Users
        //                       where us.MaUser == mauser
        //                       select us.MaUser into us1
        //                       join mhut in db.MatHangUaThiches on us1 equals mhut.MaUser into mhutofuser
        //                       from mhut2 in mhutofuser
        //                       join st in db.SieuThis on mhut2.SieuthiIdUaThich equals st.SieuThiId into listst
        //                       from st2 in listst
        //                       join gh in db.GianHangs on mhut2.GianHangIdUaThich equals gh.GianHangId into listgh
        //                       from gh2 in listgh
        //                       join mh in db.MatHangs on mhut2.IdMatHang equals mh.MatHangId

        //                       select new MatHangST()
        //                       {
        //                           SieuThiId = st2.SieuThiId,
        //                           GianHangId = gh2.GianHangId,
        //                           TenSieuThi = st2.TenSieuThi,
        //                           MatHangId = mh.MatHangId,
        //                           MaMathang = mh.MaMatHang,
        //                           TenMatHang = mh.TenMatHang,
        //                           DSHinhAnh = mh.DSHinhAnh,
        //                           MoTa = mh.MoTa,
        //                           GiaCa = mh.GiaCa,
        //                           TrangThai = mh.TrangThai,
        //                           LoaiHang = mh.LoaiHang,
        //                           LaHangKM = mh.NgayBDKM <= DateTime.UtcNow && mh.NgayKTKM >= DateTime.UtcNow ? true : false
        //                       }).ToList();


        //    var data = dataMhChung.Concat(dataMhRieng);

        //    return data;
        //}

        public class sieuthigianhangDTO
        {
            public string TenSieuThi { get; set; }
            public IEnumerable<gianhangDTO> ListGianHang { get; set; }
        }

        public class HeThongianhangDTO
        {
            public string TenHeThong { get; set; }
            public IEnumerable<gianhangDTO> ListGianHang { get; set; }
        }

        public class gianhangsukien
        {
            public IEnumerable<gianhangDTO> DsGianHang { get; set; }
            public IEnumerable<DsSuKien> DsSuKien { get; set; }
        }

        public class gianhangDTO
        {
            public string MaGianHang { get; set; }
            public string TenGianHang { get; set; }
            public string AnhGianHang { get; set; }
        }

        public class DsSuKien
        {
            public string TDSuKien { get; set; }
            public string NDSuKien { get; set; }
            public string DsHinhAnh { get; set; }
            public DateTime? NgayTao { get; set; }
            public DateTime? NgayBD { get; set; }
            public DateTime? NgayKT { get; set; }
        }

        [HttpGet]
        [Route("{id:int}/sukien")]
        public async Task<IHttpActionResult> GetSuKienOfSieuThi(int id)
        {
            var sukien = await db.SuKiens.Where(x => x.SieuThiId == id).Select(x => new DsSuKien()
            {
                TDSuKien = x.TDSuKien,
                NDSuKien = x.NDSuKien,
                DsHinhAnh = x.DsAnh,
                NgayTao = x.NgayTao,
                NgayBD = x.NgayBD,
                NgayKT = x.NgayKT                
            }).ToListAsync();

            var sukienchung = await (from st in db.SieuThis
                               where st.SieuThiId == id
                               join ht in db.HeThongs on st.HeThongId equals ht.HeThongId into ht1
                               from x in ht1
                               join sk in db.SuKienChungs on x.HeThongId equals sk.HeThongId
                               select new DsSuKien()
                               {
                                   TDSuKien = sk.TDSuKien,
                                   NDSuKien = sk.NDSuKien,
                                   DsHinhAnh = sk.DsAnh,
                                   NgayTao = sk.NgayTao,
                                   NgayBD = sk.NgayBD,
                                   NgayKT = sk.NgatKT
                               }).ToListAsync();

            var data = sukien.Concat(sukienchung);
            data = data.OrderByDescending(x => x.NgayTao).Take(10).ToList();
            return Ok(data);
        }

       


        [HttpGet]
        [Route("{id:int}/gianhang")]
        public async Task<IHttpActionResult> GetGianHangOfSieuThi(int id)
        {
            var gianhang = await db.GianHangs.Where(x=>x.SieuThiId == id).Select(x=>new gianhangDTO() {
                MaGianHang = x.MaGianHang,
                TenGianHang = x.TenGianHang,
                AnhGianHang = x.AnhGianHang
            }).ToListAsync();

            var gianhangchung = await (from st in db.SieuThis
                                 where st.SieuThiId == id
                                 join ht in db.HeThongs on st.HeThongId equals ht.HeThongId into ht1
                                 from x in ht1
                                 join ghc in db.GianHangChungs on x.HeThongId equals ghc.HeThongId
                                 select new gianhangDTO()
                                 {
                                     MaGianHang = ghc.MaGianHangChung,
                                     TenGianHang = ghc.TenGianHangChung,
                                     AnhGianHang = ghc.AnhGianHang
                                 }).ToListAsync();

            var concatgh = gianhang.Union(gianhangchung, new GianHangComparer());
            return Ok(concatgh);
        }

        class GianHangComparer : IEqualityComparer<gianhangDTO>
        {
            public bool Equals(gianhangDTO x, gianhangDTO y)
            {
                return x.MaGianHang == y.MaGianHang;
            }

            public int GetHashCode(gianhangDTO obj)
            {
                return obj.MaGianHang.GetHashCode();
            }
        }

        public class sieuthigianhangDTO1
        {
            public string TenSieuThi { get; set; }
            public IEnumerable<gianhangDTO1> ListGianHang { get; set; }
        }

        public class gianhangDTO1
        {
            public int GianHangId { get; set; }
            public string TenGianHang { get; set; }
            public IEnumerable<MathangDTO> ListMatHang { get; set; }
        }

        public class MathangDTO
        {            
            public string MaMatHang { get; set; }
            public string MaGianHang { get; set; }
            public string TenMatHang { get; set; }
            public string DSHinhAnh { get; set; }
            public string MoTa { get; set; }
            public Nullable<int> GiaCa { get; set; }
            public string TrangThai { get; set; }
            public string LoaiHang { get; set; }
            public Nullable<int> PhanTramKM { get; set; }
            public bool? LaHangKM { get; set; }
        }

        [HttpGet]
        [Route("{id:int}/mathang/magianhang={magh}/loaihang={loaihang}")]
        [ResponseType(typeof(MathangDTO))]
        public async Task<IHttpActionResult> GetMatHangOfSieuThi(int id, string magh, string loaihang)
        {
            var mathang = await db.MatHangs
                .Select(x => new MathangDTO()
                {                   
                    MaMatHang = x.MaMatHang ?? "",
                    TenMatHang = x.TenMatHang ?? "",
                    DSHinhAnh = x.DSHinhAnh != null ? (x.AnhDaiDien != null ? x.AnhDaiDien + "," + x.DSHinhAnh : x.DSHinhAnh) : (x.AnhDaiDien != null ? x.AnhDaiDien : ""),
                    MoTa = x.MoTa ?? "",
                    GiaCa = x.GiaCa ?? 0,
                    PhanTramKM = x.PhanTramKM ?? 0,
                    LoaiHang = x.LoaiHang ?? "",
                    TrangThai = x.TrangThai ?? "",
                    LaHangKM = x.NgayBDKM <= DateTime.UtcNow && x.NgayKTKM >= DateTime.UtcNow ? true : false,
                    MaGianHang = x.MaGianHang ?? ""
                                   
                }).ToListAsync();

            //var _st = db.SieuThis.Where(x => x.SieuThiId == id).FirstOrDefault();
            //var _Listghc = _st.HeThong.GianHangChungs;
            //foreach (var _mhc in _Listghc)
            //{
            //    foreach (var item in _mhc.MatHangChungs)
            //    {
                    
            //    }
            //}

            var mathangchung = await (from st in db.SieuThis
                                      where st.SieuThiId == id
                                      join ht in db.HeThongs on st.HeThongId equals ht.HeThongId into ht1
                                      from x in ht1
                                      join ghc in db.GianHangChungs on x.HeThongId equals ghc.HeThongId into ghc1
                                      from y in ghc1
                                      join z in db.MatHangChungs on y.GianHangChungId equals z.GianHangChungId
                                      select new MathangDTO()
                                      {
                                          MaMatHang = z.MaMatHangChung ?? "",
                                          TenMatHang = z.TenMatHang ?? "",
                                          DSHinhAnh = z.DSHinhAnh != null ? (z.AnhDaiDien != null ? z.AnhDaiDien + "," + z.DSHinhAnh : z.DSHinhAnh) : (z.AnhDaiDien != null ? z.AnhDaiDien : ""),
                                          MoTa = z.MoTa ?? "",
                                          GiaCa = z.GiaCa ?? 0,
                                          PhanTramKM = z.PhanTramKM ?? 0,
                                          LoaiHang = z.LoaiHang ?? "",
                                          TrangThai = z.TrangThai ?? "",
                                          LaHangKM = z.NgayBDKM <= DateTime.Now && z.NgayKTKM >= DateTime.Now ? true : false,
                                          MaGianHang = z.MaGianHangChung ?? ""
                                      }).ToListAsync();
                      
            var query = mathang.Union(mathangchung, new MatHangComparer());
            if (magh != null && magh != "''")
            {
                query = query.Where(x => x.MaGianHang == magh).ToList();
            }

            if (loaihang != null && loaihang != "''")
            {
                query = query.Where(x => x.LoaiHang == loaihang).ToList();
            }
   
            if (query.Count() == 0)
            {
                return Ok(new List<MathangDTO> { });
            }
            return Ok(query);
        }

        class MatHangComparer : IEqualityComparer<MathangDTO>
        {
            public bool Equals(MathangDTO x, MathangDTO y)
            {
                return x.MaMatHang == y.MaMatHang;
            }

            public int GetHashCode(MathangDTO obj)
            {
                return obj.MaMatHang.GetHashCode();
            }
        }

        // PUT: api/SieuThis/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSieuThi(int id, SieuThi sieuThi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sieuThi.SieuThiId)
            {
                return BadRequest();
            }

            db.Entry(sieuThi).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SieuThiExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/SieuThis
        [ResponseType(typeof(SieuThi))]
        public async Task<IHttpActionResult> PostSieuThi(SieuThi sieuThi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SieuThis.Add(sieuThi);
            db.Entry(sieuThi).State = EntityState.Added;
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = sieuThi.SieuThiId }, sieuThi);
        }

        // DELETE: api/SieuThis/5
        [ResponseType(typeof(SieuThi))]
        public async Task<IHttpActionResult> DeleteSieuThi(int id)
        {
            SieuThi sieuThi = await db.SieuThis.FindAsync(id);
            if (sieuThi == null)
            {
                return NotFound();
            }

            db.SieuThis.Remove(sieuThi);
            db.Entry(sieuThi).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return Ok(sieuThi);
        }

        [Route("dsloaihinhkd")]
        [HttpGet]
        public IHttpActionResult DanhsachLoaiHinhKinhDoanh()
        {
            List<string> newLoaiHinh = new List<string>();
            newLoaiHinh.Add("Trung tâm thương mại");
            newLoaiHinh.Add("Siêu thị bán lẻ");
            newLoaiHinh.Add("Siêu thị điện máy");
            newLoaiHinh.Add("Thời trang");
            newLoaiHinh.Add("Ẩm thực");
            return Ok(newLoaiHinh);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SieuThiExists(int id)
        {
            return db.SieuThis.Count(e => e.SieuThiId == id) > 0;
        }
    }
}