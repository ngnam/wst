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

namespace WebSieuThi.Controllers
{
    public class GianHangsController : Controller
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        #region Hethong
        [Authorize(Roles = "hethong")]
        public ActionResult HethongListGianHangChung(int? pg, string search)
        {
            var userlogin = User.Identity.Name != null ? db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault() : null;
            if (userlogin == null)
            {
                return RedirectToRoute("LoginAccount");
            }
            var data = db.GianHangChungs.Where(x => x.HeThongId == userlogin.HeThongId).Select(x => x);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.MaGianHangChung.Contains(search) || x.TenGianHangChung.Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderBy(x => x.TenGianHangChung);

            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "sieuthi")]
        public ActionResult ListDonHangs(int? pg)
        {
            var _sieuthi = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var data = db.DonDatHangs.Where(x => x.SieuThiId == _sieuthi.SieuThiId).Select(x => x).OrderByDescending(x => x.NTDonHang);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: GianHangs
        [Authorize(Roles = "hethong")]
        public ActionResult HethongAddNewGianHangChung()
        {
            return View();
        }

        //public ActionResult CheckMaGianHangChung(string magh)
        //{
        //    bool isExist = false;
        //    if (magh != null && magh != "")
        //    {
        //        var _user = db.HeThongs
        //        var gianhang = db.GianHangChungs.Where(x => x.MaGianHangChung == magh).FirstOrDefault();
        //    }
        //    return Json(isExist, JsonRequestBehavior.AllowGet);
        //}

        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HethongAddNewGianHangChung(GianHangChungModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin gian hàng.";
                return RedirectToRoute("HethongAddNewGHC");
            }
            
            int HeThongId = 0;
            var userLogin = User.Identity.Name != null ? db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault() : null;
            if (userLogin != null)
            {
                HeThongId = userLogin.HeThongId;
            }
            string _ghcc = string.Format("{0}-{1}", HeThongId, model.MaGianHangChung);
            var _gh = db.GianHangChungs.Where(x => x.MaGianHangChung == _ghcc).FirstOrDefault();
            if (_gh != null)
            {
                TempData["Error"] = "Mã gian hàng đã tồn tại.";
                return RedirectToRoute("HethongAddNewGHC");
            }
            
            try
            {
                GianHangChung _newGhc = new GianHangChung();
                _newGhc.MaGianHangChung = model.MaGianHangChung != null ? string.Format("{0}-{1}", HeThongId, model.MaGianHangChung) : null;
                _newGhc.TenGianHangChung = model.TenGianHangChung ?? null;
                _newGhc.AnhGianHang = model.AnhGianHang ?? null;
                _newGhc.HeThongId = HeThongId != 0 ? HeThongId : (int?)null;
                db.GianHangChungs.Add(_newGhc);
                await db.SaveChangesAsync();

                // save a copy gianhangnew to gianhang cua tat ca sieuthi trong hethong.
                try
                {
                    var sieuthiid = userLogin.SieuThis.Select(x => x.SieuThiId).ToArray();
                    if (sieuthiid.Length > 0)
                    {
                        foreach (var item in sieuthiid)
                        {
                            GianHang _ghr = new GianHang();
                            _ghr.MaGianHang = _newGhc.MaGianHangChung;
                            _ghr.TenGianHang = _newGhc.TenGianHangChung;
                            _ghr.AnhGianHang = _newGhc.AnhGianHang;
                            _ghr.SieuThiId = item;
                            db.GianHangs.Add(_ghr);
                            await db.SaveChangesAsync();
                        }
                    }
                    

                }
                catch (Exception ex)
                {
                    Helpers.Config.SaveToLog(ex.ToString());
                }

                TempData["Updated"] = "Đã thêm mới gian hàng " + model.TenGianHangChung ?? "" + " vào hệ thống";
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm gian hàng.";
                return RedirectToRoute("HethongAddNewGHC");
            }
            return RedirectToRoute("HethongListGHC");
        }

        //HeThongEditGianHangChung
        [Authorize(Roles = "hethong")]
        public async Task<ActionResult> HeThongEditGianHangChung(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            GianHangChung _gianhang = await db.GianHangChungs.FindAsync(id);
            if (_gianhang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            var getGianHang = new GianHangChungModel()
            {
               GianHangChungId = _gianhang.GianHangChungId,
               HeThongId = _gianhang.HeThongId ?? 0,
               AnhGianHang = _gianhang.AnhGianHang ?? "",
               MaGianHangChung = _gianhang.MaGianHangChung != null ? _gianhang.MaGianHangChung.Split('-')[1] : "",
               TenGianHangChung = _gianhang.TenGianHangChung ?? ""              
            };
            return View(getGianHang);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongEditGianHangChung(GianHangChungModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("HethongEditGHC", new { id = model.GianHangChungId });
            }
            var _gianhang = await db.GianHangChungs.FindAsync(model.GianHangChungId);

            //string _ghcc = string.Format("{0}-{1}", model.HeThongId, model.MaGianHangChung);
            //var _gh = db.GianHangChungs.Where(x => x.MaGianHangChung == _ghcc).FirstOrDefault();
            //if (_gh != null)
            //{
            //    TempData["Error"] = "Mã gian hàng đã tồn tại.";
            //    return RedirectToRoute("HethongAddNewGHC");
            //}
            try
            {
                if (_gianhang != null)
                {
                    _gianhang.HeThongId = model.HeThongId ?? (int?)null;
                    _gianhang.MaGianHangChung = string.Format("{0}-{1}", model.HeThongId ?? 0, model.MaGianHangChung);
                    _gianhang.TenGianHangChung = model.TenGianHangChung ?? null;
                    _gianhang.AnhGianHang = model.AnhGianHang ?? null;

                    db.Entry(_gianhang).State = EntityState.Modified;
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

            TempData["Updated"] = "Cập nhật thông tin gian hàng thành công";
            return RedirectToRoute("HethongListGHC");
        }



        //HeThongDeleteGianHangChung
        [Authorize(Roles = "hethong")]
        public ActionResult HeThongDeleteGianHangChung(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            GianHangChung _gianhang = db.GianHangChungs.Find(id);
            if (_gianhang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            return View(_gianhang);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ActionName("HeThongDeleteGianHangChung")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongDeleteGianHangChungConfirmed(int? id)
        {
            GianHangChung _gianhang = await db.GianHangChungs.FindAsync(id);
            if (_gianhang == null)
            {
                return RedirectToRoute("AdminPanel");
            }

            try
            {
                if (_gianhang.MatHangChungs.Count > 0)
                {
                    TempData["Updated"] = "Gian hàng này đang có mặt hàng, bạn không thể xóa.";
                    return RedirectToRoute("HethongDeleteGHC", new { id = _gianhang.GianHangChungId });
                }

                db.GianHangChungs.Remove(_gianhang);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa gian hàng thành công";
                return RedirectToRoute("HethongListGHC");
            }
            catch
            {
                return RedirectToRoute("AdminPanel");
            }
        }
        #endregion

        
        

        #region sieuthi
        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuThiListGianHang(int? pg, string search)
        {
            var userlogin = User.Identity.Name != null ? db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault() : null;
            if (userlogin == null)
            {
                return RedirectToRoute("LoginAccount");
            }
            var data = db.GianHangs.Where(x => x.SieuThiId == userlogin.SieuThiId).Select(x => new getGianHang() { 
                GianHangId = x.GianHangId,
                isGianHangChung = false,
                AnhGianHang = x.AnhGianHang,
                MaGianHang = x.MaGianHang,
                SieuThiId = x.SieuThiId,
                TenGianHang = x.TenGianHang
            });

            //var gianhangchung = db.GianHangChungs.Where(x => x.HeThongId == userlogin.HeThongId).Select(x=>new getGianHang() {
            //    GianHangId = x.GianHangChungId,
            //    isGianHangChung = true,
            //    AnhGianHang = x.AnhGianHang,
            //    MaGianHang = x.MaGianHangChung,
            //    SieuThiId = null,
            //    TenGianHang = x.TenGianHangChung
            //});

            //data = data.Concat(gianhangchung);

            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.MaGianHang.Contains(search) || x.TenGianHang.Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderBy(x => x.TenGianHang);

            
            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: GianHangs
        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiAddNewGianHang()
        {
            return View();
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiAddNewGianHang(GianHangModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin gian hàng.";
                return RedirectToRoute("SieuthiAddNewGH");
            }

           
            var userLogin = User.Identity.Name != null ? db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault() : null;
            if (userLogin == null)
            {
                return RedirectToRoute("AdminPanel");
            }

            string _ghcc = string.Format("{0}-{1}", userLogin.HeThongId, model.MaGianHang);
            var _gh = db.GianHangs.Where(x => x.MaGianHang == _ghcc && x.SieuThiId == userLogin.SieuThiId).FirstOrDefault();
            if (_gh != null)
            {
                TempData["Error"] = "Mã gian hàng đã tồn tại.";
                return RedirectToRoute("SieuthiAddNewGH");
            }

            try
            {
                GianHang _newGh = new GianHang();
                _newGh.MaGianHang = model.MaGianHang != null ? string.Format("{0}-{1}", userLogin.HeThongId ?? 0, model.MaGianHang) : null;
                _newGh.TenGianHang = model.TenGianHang ?? null;
                _newGh.AnhGianHang = model.AnhGianHang ?? null;
                _newGh.SieuThiId = userLogin.SieuThiId != null ? userLogin.SieuThiId : (int?)null;
                db.GianHangs.Add(_newGh);

                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới gian hàng " + model.TenGianHang ?? "" + " vào siêu thị";
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm gian hàng.";
                return RedirectToRoute("SieuthiAddNewGH");
            }
            return RedirectToRoute("SieuthiListGH");
        }

        //SieuThiEditGianHang
        [Authorize(Roles = "sieuthi")]
        public async Task<ActionResult> SieuThiEditGianHang(int? id)
        {
            if (id == default(int) || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            GianHang _gianhang = await db.GianHangs.FindAsync(id);
            if (_gianhang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            var getGianHang = new GianHangModel()
            {
                GianHangId = _gianhang.GianHangId,
                SieuthiId = _gianhang.SieuThiId ?? 0,
                AnhGianHang = _gianhang.AnhGianHang ?? "",
                MaGianHang = _gianhang.MaGianHang != null ? _gianhang.MaGianHang.Split('-')[1] : "",
                TenGianHang = _gianhang.TenGianHang ?? ""
            };
            return View(getGianHang);
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuThiEditGianHang(GianHang model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("SieuthiEditGH", new { id = model.GianHangId });
            }
            var _gianhang = await db.GianHangs.FindAsync(model.GianHangId);

            try
            {
                if (_gianhang != null)
                {
                    _gianhang.SieuThiId = model.SieuThiId ?? (int?)null;
                    _gianhang.MaGianHang = string.Format("{0}-{1}", _gianhang.SieuThi.HeThongId ?? 0, model.MaGianHang);
                    _gianhang.TenGianHang = model.TenGianHang ?? null;
                    _gianhang.AnhGianHang = model.AnhGianHang ?? null;

                    db.Entry(_gianhang).State = EntityState.Modified;
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

            TempData["Updated"] = "Cập nhật thông tin gian hàng thành công";
            return RedirectToRoute("SieuthiListGH");
        }

        //SieuthiDeleteGH
        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiDeleteGianHang(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            GianHang _gianhang = db.GianHangs.Find(id);
            if (_gianhang == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            return View(_gianhang);
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ActionName("SieuthiDeleteGianHang")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiDeleteGianHangConfirmed(int? id)
        {
            GianHang _gianhang = await db.GianHangs.FindAsync(id);
            if (_gianhang == null)
            {
                return RedirectToRoute("AdminPanel");
            }

            try
            {
                if (_gianhang.MatHangs.Count > 0)
                {
                    TempData["Updated"] = "Gian hàng này đang có mặt hàng, bạn không thể xóa.";
                    return RedirectToRoute("SieuthiDeleteGH", new { id = _gianhang.GianHangId });
                }

                db.GianHangs.Remove(_gianhang);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa gian hàng thành công";
                return RedirectToRoute("SieuthiListGH");
            }
            catch
            {
                return RedirectToRoute("AdminPanel");
            }
        }
        #endregion

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