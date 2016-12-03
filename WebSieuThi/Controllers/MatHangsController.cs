using FlickrNet;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebSieuThi.Models;
using PagedList;
using PagedList.Mvc;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace WebSieuThi.Controllers
{
    public class MatHangsController : Controller
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();
        // GET: MatHangs

        #region Hệ thống

        [Authorize(Roles = "hethong")]
        public ActionResult HeThongAddNewMatHang()
        {
            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.gianhang = ListGianHang();
            ViewBag.hethongid = user.HeThongId;
            ViewBag.trangthaihang = ListTrangThaiHang();
            ViewBag.loaihang = ListLoaiHang();
            return View();
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongAddNewMatHang(MatHangChungModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin mặt hàng.";
                return RedirectToRoute("HethongAddNewMHC");
            }
            string _mamhcc = model.HeThongId.ToString() + "-" + model.MaMatHangChung;
            var _mhcc = db.MatHangChungs.Where(x => x.MaMatHangChung == _mamhcc).FirstOrDefault();
            if (_mhcc != null)
            {
                TempData["Error"] = "Mã mặt hàng đã tồn tại.";
                return RedirectToRoute("HethongAddNewMHC");
            }

            try
            {
                MatHangChung _newMhc = new MatHangChung();
                _newMhc.TenMatHang = model.TenMatHang ?? null;
                _newMhc.DSHinhAnh = model.DsHinhAnh ?? null;
                _newMhc.GiaCa = (int?)model.GiaCa ?? null;
                if (model.MaMatHangChung != null && model.HeThongId != default(int))
                {
                    _newMhc.MaMatHangChung = model.HeThongId.ToString() + "-" + model.MaMatHangChung;
                }
                string sghid = model.strGianHangChung != null ? model.strGianHangChung.Split('_')[0].ToString() : null;
                string mgh = model.strGianHangChung != null ? model.strGianHangChung.Split('_')[1].ToString() : null;
                _newMhc.GianHangChungId = sghid != null ? Convert.ToInt32(sghid) : (int?)null;
                _newMhc.MaGianHangChung = mgh ?? null;
                _newMhc.LoaiHang = model.LoaiHang ?? null;
                _newMhc.TrangThai = model.TrangThai ?? null;
                _newMhc.PhanTramKM = model.PhanTramKM ?? null;
                _newMhc.NgayBDKM = model.NgayBDKM ?? null;
                _newMhc.NgayKTKM = model.NgayKTKM ?? null;
                _newMhc.AnhDaiDien = model.AnhDaiDien ?? null;
                _newMhc.MoTa = model.MoTa ?? null;
                string _dsanh = "";
                if (model.indivanh1 != null && model.indivanh1 != "")
                {
                    _dsanh += model.indivanh1 + ",";
                }

                if (model.indivanh2 != null && model.indivanh2 != "")
                {
                    _dsanh += model.indivanh2 + ",";
                }

                if (model.indivanh3 != null && model.indivanh3 != "")
                {
                    _dsanh += model.indivanh3 + ",";
                }

                if (model.indivanh4 != null && model.indivanh4 != "")
                {
                    _dsanh += model.indivanh4 + ",";
                }
                if (_dsanh != "")
                {
                    _dsanh = _dsanh.Remove(_dsanh.Length - 1);
                }
                //YourString = YourString.Remove(YourString.Length - 1);
                //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
                _newMhc.DSHinhAnh = _dsanh != "" ? _dsanh : null;
                db.MatHangChungs.Add(_newMhc);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới mặt hàng " + model.TenMatHang ?? "" + " vào hệ thống";
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm mặt hàng hàng.";
                return RedirectToRoute("HethongAddNewMHC");
            }
            return RedirectToRoute("HethongListMHC");
        }

        [Authorize(Roles = "hethong")]
        public ActionResult HeThongListMatHang(int? pg, string search, string trangthai, string loaihang, string gianhang)
        {
            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.lstgianhang = db.GianHangChungs.Where(x => x.TenGianHangChung != null && x.MaGianHangChung != null && user.HeThongId == x.HeThongId).Select(x => new SelectListItem()
            {
                Value = x.GianHangChungId.ToString() + "_" + x.MaGianHangChung,
                Text = x.TenGianHangChung
            });

            ViewBag.lsttrangthaihang = ListTrangThaiHang();

            ViewBag.lstloaihang = ListLoaiHang();

            var data = db.MatHangChungs.Where(x => x.GianHangChung.HeThongId == user.HeThongId).Select(x => x);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.MaMatHangChung.Contains(search) || x.TenMatHang.Contains(search));
                ViewBag.search = search;
            }

            if (trangthai == null) trangthai = ""; if (loaihang == null) loaihang = ""; if (gianhang == null) gianhang = "";
            if (trangthai != null && trangthai != "")
            {
                data = data.Where(x => x.TrangThai == trangthai);
                ViewBag.trangthai = trangthai;
            }

            if (loaihang != null && loaihang != "")
            {
                data = data.Where(x => x.LoaiHang == loaihang);
                ViewBag.loaihang = loaihang;
            }

            if (gianhang != null && gianhang != "")
            {
                int idgh = Convert.ToInt32(gianhang.Split('_')[0]);
                string smgh = gianhang.Split('_')[1];
                data = data.Where(x => x.GianHangChungId == idgh && x.MaGianHangChung == smgh);
                ViewBag.gianhang = gianhang;
            }

            data = data.OrderBy(x => x.TenMatHang);

            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        //HeThongEditMatHang
        [Authorize(Roles = "hethong")]
        public async Task<ActionResult> HeThongEditMatHang(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            MatHangChung _mathang = await db.MatHangChungs.FindAsync(id);
            if (_mathang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            ViewBag.gianhang = ListGianHang();
            ViewBag.trangthaihang = ListTrangThaiHang();
            ViewBag.loaihang = ListLoaiHang();
            List<string> danhsachanh = new List<string>();
            if (_mathang.DSHinhAnh != null)
            {
                danhsachanh.AddRange(_mathang.DSHinhAnh.Split(','));
            }
            if (danhsachanh.Count == 1)
            {
                danhsachanh.AddRange(new string[] { "", "", "" });
            }
            if (danhsachanh.Count == 2)
            {
                danhsachanh.AddRange(new string[] { "", "" });
            }
            if (danhsachanh.Count == 3)
            {
                danhsachanh.Add("");
            }
            if (danhsachanh.Count == 0)
            {
                danhsachanh.AddRange(new string[] { "", "", "", "" });
            }
            var arrayAnh = danhsachanh.ToArray();

            var getMatHang = new MatHangChungModel()
            {
                MatHangChungId = _mathang.MatHangChungId,
                TenMatHang = _mathang.TenMatHang,
                AnhDaiDien = _mathang.AnhDaiDien,
                GiaCa = (int)_mathang.GiaCa,
                strGianHangChung = _mathang.GianHangChungId + "_" + _mathang.MaGianHangChung,
                LoaiHang = _mathang.LoaiHang,
                MoTa = _mathang.MoTa,
                NgayBDKM = _mathang.NgayBDKM,
                NgayKTKM = _mathang.NgayKTKM,
                PhanTramKM = _mathang.PhanTramKM,
                TrangThai = _mathang.TrangThai,
                MaMatHangChung = _mathang.MaMatHangChung != null ? _mathang.MaMatHangChung.Split('-')[1] : "",
                HeThongId = (int)_mathang.GianHangChung.HeThongId,
                indivanh1 = arrayAnh[0],
                indivanh2 = arrayAnh[1],
                indivanh3 = arrayAnh[2],
                indivanh4 = arrayAnh[3],
            };

            return View(getMatHang);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongEditMatHang(MatHangChungModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("HethongEditMHC", new { id = model.MatHangChungId });
            }
            var _mathang = await db.MatHangChungs.FindAsync(model.MatHangChungId);

            try
            {
                if (_mathang != null)
                {
                    _mathang.TenMatHang = model.TenMatHang ?? null;
                    _mathang.GiaCa = (int?)model.GiaCa ?? null;
                    if (model.MaMatHangChung != null && model.HeThongId != null)
                    {
                        _mathang.MaMatHangChung = model.HeThongId.ToString() + "-" + model.MaMatHangChung;
                    }
                    string sghid = model.strGianHangChung != null ? model.strGianHangChung.Split('_')[0].ToString() : null;
                    string mgh = model.strGianHangChung != null ? model.strGianHangChung.Split('_')[1].ToString() : null;
                    _mathang.GianHangChungId = sghid != null ? Convert.ToInt32(sghid) : (int?)null;
                    _mathang.MaGianHangChung = mgh ?? null;
                    _mathang.LoaiHang = model.LoaiHang ?? null;
                    _mathang.TrangThai = model.TrangThai ?? null;
                    _mathang.PhanTramKM = model.PhanTramKM ?? null;
                    _mathang.NgayBDKM = model.NgayBDKM ?? null;
                    _mathang.NgayKTKM = model.NgayKTKM ?? null;
                    _mathang.AnhDaiDien = model.AnhDaiDien ?? null;
                    _mathang.MoTa = model.MoTa ?? null;
                    string _dsanh = "";
                    if (model.indivanh1 != null && model.indivanh1 != "")
                    {
                        _dsanh += model.indivanh1 + ",";
                    }
                    if (model.indivanh2 != null && model.indivanh2 != "")
                    {
                        _dsanh += model.indivanh2 + ",";
                    }
                    if (model.indivanh3 != null && model.indivanh3 != "")
                    {
                        _dsanh += model.indivanh3 + ",";
                    }
                    if (model.indivanh4 != null && model.indivanh4 != "")
                    {
                        _dsanh += model.indivanh4 + ",";
                    }
                    if (_dsanh != "")
                    {
                        _dsanh = _dsanh.Remove(_dsanh.Length - 1);
                    }
                    //YourString = YourString.Remove(YourString.Length - 1);
                    //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
                    _mathang.DSHinhAnh = _dsanh != "" ? _dsanh : null;
                    db.Entry(_mathang).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    return RedirectToRoute("AdminPanel");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa gian hàng");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật mặt hàng thành công";
            return RedirectToRoute("HethongEditMHC", new { id = model.MatHangChungId });
        }

        [Authorize(Roles = "hethong")]
        public ActionResult HeThongDeleteMatHang(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            MatHangChung _mathang = db.MatHangChungs.Find(id);
            if (_mathang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            return View(_mathang);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ActionName("HeThongDeleteMatHang")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongDeleteMatHangConfirmed(int? id)
        {
            MatHangChung _mathang = await db.MatHangChungs.FindAsync(id);
            if (_mathang == null)
            {
                return RedirectToRoute("AdminPanel");
            }

            try
            {

                db.MatHangChungs.Remove(_mathang);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa mặt hàng hàng thành công";
                return RedirectToRoute("HethongListMHC");
            }
            catch
            {
                return RedirectToRoute("AdminPanel");
            }
        }

        public List<SelectListItem> ListGianHang()
        {
            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var newList = db.GianHangChungs.Where(x => x.TenGianHangChung != null && x.MaGianHangChung != null && user.HeThongId == x.HeThongId).Select(x => new SelectListItem()
            {
                Value = x.GianHangChungId.ToString() + "_" + x.MaGianHangChung,
                Text = x.TenGianHangChung
            }).ToList();
            return newList;
        }

        #endregion

        #region Siêu thị

        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiAddNewMatHang()
        {
            var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.gianhang = ListGianHangRieng();
            ViewBag.hethongid = user.HeThongId;
            ViewBag.trangthaihang = ListTrangThaiHang();
            ViewBag.loaihang = ListLoaiHang();
            return View();
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiAddNewMatHang(MatHangModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin mặt hàng.";
                return RedirectToRoute("SieuthiAddNewMH");
            }

            var userlogin = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            string _mamhcc = userlogin.HeThongId.ToString() + "-" + model.MaMatHang;
            var _mhcc = db.MatHangs.Where(x => x.MaMatHang == _mamhcc).FirstOrDefault();
            if (_mhcc != null)
            {
                TempData["Error"] = "Mã mặt hàng đã tồn tại.";
                return RedirectToRoute("SieuthiAddNewMH");
            }

            try
            {
                MatHang _newMh = new MatHang();
                _newMh.TenMatHang = model.TenMatHang ?? null;
                _newMh.GiaCa = (int?)model.GiaCa ?? null;
                if (model.MaMatHang != null && model.HeThongId != default(int))
                {
                    _newMh.MaMatHang = model.HeThongId + "-" + model.MaMatHang;
                }
                string sghid = model.strGianHang != null ? model.strGianHang.Split('_')[0].ToString() : null;
                string mgh = model.strGianHang != null ? model.strGianHang.Split('_')[1].ToString() : null;
                _newMh.GianHangId = sghid != null ? Convert.ToInt32(sghid) : (int?)null;
                _newMh.MaGianHang = mgh ?? null;
                _newMh.LoaiHang = model.LoaiHang ?? null;
                _newMh.TrangThai = model.TrangThai ?? null;
                _newMh.PhanTramKM = model.PhanTramKM ?? null;
                _newMh.NgayBDKM = model.NgayBDKM ?? null;
                _newMh.NgayKTKM = model.NgayKTKM ?? null;
                _newMh.AnhDaiDien = model.AnhDaiDien ?? null;
                _newMh.MoTa = model.MoTa ?? null;
                string _dsanh = "";
                if (model.indivanh1 != null && model.indivanh1 != "")
                {
                    _dsanh += model.indivanh1 + ",";
                }

                if (model.indivanh2 != null && model.indivanh2 != "")
                {
                    _dsanh += model.indivanh2 + ",";
                }

                if (model.indivanh3 != null && model.indivanh3 != "")
                {
                    _dsanh += model.indivanh3 + ",";
                }

                if (model.indivanh4 != null && model.indivanh4 != "")
                {
                    _dsanh += model.indivanh4 + ",";
                }
                if (_dsanh != "")
                {
                    _dsanh = _dsanh.Remove(_dsanh.Length - 1);
                }
                //YourString = YourString.Remove(YourString.Length - 1);
                //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
                _newMh.DSHinhAnh = _dsanh != "" ? _dsanh : null;
                db.MatHangs.Add(_newMh);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới mặt hàng " + model.TenMatHang ?? "" + " vào gian hàng của siêu thị";
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm mặt hàng.";
                return RedirectToRoute("SieuthiAddNewMH");
            }
            return RedirectToRoute("SieuthiListMH");
        }

        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiListMatHang(int? pg, string search, string trangthai, string loaihang, string gianhang)
        {
            var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.lstgianhang = ListGianHangRieng();

            ViewBag.lsttrangthaihang = ListTrangThaiHang();

            ViewBag.lstloaihang = ListLoaiHang();

            var data = db.MatHangs.Where(x => x.GianHang.SieuThiId == user.SieuThiId).Select(x => new getMatHang()
            {
                MatHangId = x.MatHangId,
                TenMatHang = x.TenMatHang,
                AnhDaiDien = x.AnhDaiDien,
                TrangThai = x.TrangThai,
                LoaiHang = x.LoaiHang,
                isMatHangChung = false,
                GianHangId = x.GianHangId,
                MaGianHang = x.MaGianHang
            });

            // hiện thêm mã mặt hàng chung
            var mathangchung = db.MatHangChungs.Where(x => x.GianHangChung.HeThongId == user.HeThongId).Select(x => new getMatHang()
            {
                MatHangId = x.MatHangChungId,
                TenMatHang = x.TenMatHang,
                AnhDaiDien = x.AnhDaiDien,
                TrangThai = x.TrangThai,
                LoaiHang = x.LoaiHang,
                isMatHangChung = true,
                GianHangId = x.GianHangChungId,
                MaGianHang = x.MaGianHangChung
            });

            data = data.Concat(mathangchung);

            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenMatHang.Contains(search));
                ViewBag.search = search;
            }

            if (trangthai == null) trangthai = ""; if (loaihang == null) loaihang = ""; if (gianhang == null) gianhang = "";
            if (trangthai != null && trangthai != "")
            {
                data = data.Where(x => x.TrangThai == trangthai);
                ViewBag.trangthai = trangthai;
            }

            if (loaihang != null && loaihang != "")
            {
                data = data.Where(x => x.LoaiHang == loaihang);
                ViewBag.loaihang = loaihang;
            }

            if (gianhang != null && gianhang != "")
            {
                int idgh = Convert.ToInt32(gianhang.Split('_')[0]);
                string smgh = gianhang.Split('_')[1];
                data = data.Where(x => x.GianHangId == idgh && x.MaGianHang == smgh);
                ViewBag.gianhang = gianhang;
            }

            data = data.OrderBy(x => x.TenMatHang);

            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "sieuthi")]
        public async Task<ActionResult> SieuthiEditMatHang(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            MatHang _mathang = await db.MatHangs.FindAsync(id);
            if (_mathang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            List<string> danhsachanh = new List<string>();
            if (_mathang.DSHinhAnh != null)
            {
                danhsachanh.AddRange(_mathang.DSHinhAnh.Split(','));
            }
            if (danhsachanh.Count == 1)
            {
                danhsachanh.AddRange(new string[] { "", "", "" });
            }
            if (danhsachanh.Count == 2)
            {
                danhsachanh.AddRange(new string[] { "", "" });
            }
            if (danhsachanh.Count == 3)
            {
                danhsachanh.Add("");
            }
            if (danhsachanh.Count == 0)
            {
                danhsachanh.AddRange(new string[] { "", "", "", "" });
            }
            var arrayAnh = danhsachanh.ToArray();
            var getMatHang = new MatHangModel()
            {
                MatHangId = _mathang.MatHangId,
                TenMatHang = _mathang.TenMatHang,
                AnhDaiDien = _mathang.AnhDaiDien,
                GiaCa = (int)_mathang.GiaCa,
                strGianHang = _mathang.GianHangId + "_" + _mathang.MaGianHang,
                LoaiHang = _mathang.LoaiHang,
                MoTa = _mathang.MoTa,
                NgayBDKM = _mathang.NgayBDKM,
                NgayKTKM = _mathang.NgayKTKM,
                PhanTramKM = _mathang.PhanTramKM,
                TrangThai = _mathang.TrangThai,
                MaMatHang = _mathang.MaMatHang != null ? _mathang.MaMatHang.Split('-')[1] : "",
                HeThongId = (int)_mathang.GianHang.SieuThi.HeThongId,
                indivanh1 = arrayAnh[0],
                indivanh2 = arrayAnh[1],
                indivanh3 = arrayAnh[2],
                indivanh4 = arrayAnh[3],

            };

            ViewBag.gianhang = ListGianHangRieng();
            ViewBag.trangthaihang = ListTrangThaiHang();
            ViewBag.loaihang = ListLoaiHang();
            return View(getMatHang);
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiEditMatHang(MatHangModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("SieuthiEditMH", new { id = model.MatHangId });
            }
            var _mathang = await db.MatHangs.FindAsync(model.MatHangId);

            try
            {
                if (_mathang != null)
                {
                    _mathang.TenMatHang = model.TenMatHang ?? null;
                    _mathang.GiaCa = (int?)model.GiaCa ?? null;
                    if (model.MaMatHang != null)
                    {
                        _mathang.MaMatHang = model.HeThongId.ToString() + "-" + model.MaMatHang;
                    }
                    string sghid = model.strGianHang != null ? model.strGianHang.Split('_')[0].ToString() : null;
                    string mgh = model.strGianHang != null ? model.strGianHang.Split('_')[1].ToString() : null;
                    _mathang.GianHangId = sghid != null ? Convert.ToInt32(sghid) : (int?)null;
                    _mathang.MaGianHang = mgh ?? null;
                    _mathang.LoaiHang = model.LoaiHang ?? null;
                    _mathang.TrangThai = model.TrangThai ?? null;
                    _mathang.PhanTramKM = model.PhanTramKM ?? null;
                    _mathang.NgayBDKM = model.NgayBDKM ?? null;
                    _mathang.NgayKTKM = model.NgayKTKM ?? null;
                    _mathang.AnhDaiDien = model.AnhDaiDien ?? null;
                    _mathang.MoTa = model.MoTa ?? null;
                    string _dsanh = "";
                    if (model.indivanh1 != null && model.indivanh1 != "")
                    {
                        _dsanh += model.indivanh1 + ",";
                    }

                    if (model.indivanh2 != null && model.indivanh2 != "")
                    {
                        _dsanh += model.indivanh2 + ",";
                    }

                    if (model.indivanh3 != null && model.indivanh3 != "")
                    {
                        _dsanh += model.indivanh3 + ",";
                    }

                    if (model.indivanh4 != null && model.indivanh4 != "")
                    {
                        _dsanh += model.indivanh4 + ",";
                    }
                    if (_dsanh != "")
                    {
                        _dsanh = _dsanh.Remove(_dsanh.Length - 1);
                    }
                    //YourString = YourString.Remove(YourString.Length - 1);
                    //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
                    _mathang.DSHinhAnh = _dsanh != "" ? _dsanh : null;
                    db.Entry(_mathang).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    return RedirectToRoute("AdminPanel");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa mặt hàng");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật mặt hàng thành công";
            return RedirectToRoute("SieuthiEditMH", new { id = model.MatHangId });
        }

        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiDeleteMatHang(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            MatHang _mathang = db.MatHangs.Find(id);
            if (_mathang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            return View(_mathang);
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ActionName("SieuthiDeleteMatHang")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiDeleteMatHangConfirmed(int? id)
        {
            MatHang _mathang = await db.MatHangs.FindAsync(id);
            if (_mathang == null)
            {
                return RedirectToRoute("AdminPanel");
            }

            try
            {
                db.MatHangs.Remove(_mathang);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa mặt hàng hàng thành công";
                return RedirectToRoute("SieuthiListMH");
            }
            catch
            {
                return RedirectToRoute("AdminPanel");
            }
        }

        public List<SelectListItem> ListGianHangRieng()
        {
            var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var newList = db.GianHangs.Where(x => x.TenGianHang != null && x.MaGianHang != null && user.SieuThiId == x.SieuThiId).Select(x => new SelectListItem()
            {
                Value = x.GianHangId.ToString() + "_" + x.MaGianHang,
                Text = x.TenGianHang
            }).ToList();
            return newList;
        }

        #endregion

        public List<SelectListItem> ListLoaiHang()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "1", Text = "Khuyến mại" });
            newList.Add(new SelectListItem() { Value = "2", Text = "Hàng mới" });
            newList.Add(new SelectListItem() { Value = "3", Text = "Phổ biến" });
            return newList;
        }

        public List<SelectListItem> ListTrangThaiHang()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "1", Text = "Còn hàng" });
            newList.Add(new SelectListItem() { Value = "2", Text = "Hết hàng" });
            newList.Add(new SelectListItem() { Value = "3", Text = "Sắp về" });
            return newList;
        }

        public void ExportToExcel()
        {
            try
            {
                var model = (from s in db.MatHangChungs select s);
                if (model != null)
                {
                    Export export = new Export();
                    export.ToExcel(Response, model.ToList(), "DSMatHang_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                }
                else
                {
                    Response.Write("Không có dữ liệu gì để export anh ơi.");
                }
            }
            catch (Exception ex)
            {
                Helpers.Config.SaveToLog(ex.ToString());
            }
        }

        public void ExportToExcel2()
        {
            try
            {
                var model = (from s in db.MatHangs select s);
                if (model != null)
                {
                    Export export = new Export();
                    export.ToExcel(Response, model.ToList(), "DSMatHang_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                }
                else
                {
                    Response.Write("Không có dữ liệu gì để export anh ơi.");
                }

            }
            catch (Exception ex)
            {
                Helpers.Config.SaveToLog(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult ImportToExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = System.IO.Path.GetExtension(file.FileName);

                        if (fileExtension == ".xls" || fileExtension == ".xlsx")
                        {
                            string fileLocation = Server.MapPath("~/Content/") + file.FileName;
                            if (System.IO.File.Exists(fileLocation))
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            file.SaveAs(fileLocation);
                            string excelConnectionString = string.Empty;
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            //connection String for xls file format.
                            if (fileExtension == ".xls")
                            {
                                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.12.0;Data Source=" +
                                fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                                //string excelconnectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +excelfilepath + ";"+"Extended Properties="Excel 12.0;HDR=Yes";
                            }
                            //connection String for xlsx file format.
                            else if (fileExtension == ".xlsx")
                            {
                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            //Create Connection to Excel work book and add oledb namespace
                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["MatHangChung"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                            }
                        }


                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string sql = @"insert into MatHangChung(MaMatHangChung, TenMatHang, AnhDaiDien, DSHinhAnh, MoTa, NgayBDKM, NgayKTKM, GiaCa,
                    TrangThai, LoaiHang, PhanTramKM, MaGianHangChung, GianHangChungId) values(
                    '" + ds.Tables[0].Rows[i][1].ToString() + "','"
                              + ds.Tables[0].Rows[i][2].ToString() + "','"
                              + ds.Tables[0].Rows[i][3].ToString() + "','"
                              + ds.Tables[0].Rows[i][4].ToString() + "','"
                              + ds.Tables[0].Rows[i][5].ToString() + "','"
                              + ds.Tables[0].Rows[i][6].ToString() + "','"
                              + ds.Tables[0].Rows[i][7].ToString() + "','"
                              + ds.Tables[0].Rows[i][8].ToString() + "','"
                              + ds.Tables[0].Rows[i][9].ToString() + "','"
                              + ds.Tables[0].Rows[i][10].ToString() + "','"
                              + ds.Tables[0].Rows[i][11].ToString() + "','"
                              + ds.Tables[0].Rows[i][12].ToString() + "','"
                              + ds.Tables[0].Rows[i][13].ToString() + "','"
                              + "')";
                            int noOfRowInserted = db.Database.ExecuteSqlCommand(sql);
                            if (noOfRowInserted == 0)
                            {
                                Helpers.Config.SaveToLog("Error: " + ds.Tables[0].Rows[i].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.Config.SaveToLog(ex.ToString());
            }

            return RedirectToRoute("HethongListMHC");
        }

        public void ImportToExcel2()
        {
            try
            {
                Response.Write("Chức năng này em đang làm anh ơi.");
            }
            catch (Exception ex)
            {
                Helpers.Config.SaveToLog(ex.ToString());
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

    }
}

//helper class
public class Export
{
    public void ToExcel(HttpResponseBase Response, object clientsList, string fileName)
    {
        try
        {
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = clientsList;
            grid.DataBind();
            Response.ClearContent();
            //var filename = "MatHang_" + DateTime.Now.ToString("yyyyMMdd")+".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }
        catch (Exception ex)
        {
            Helpers.Config.SaveToLog(ex.ToString());
        }
    }

    //import


}