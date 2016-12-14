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
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace WebSieuThi.Controllers
{
    public class SukiensController : Controller
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();
        // GET: Sukiens
        [Authorize(Roles = "hethong")]
        public ActionResult HeThongAddNewSuKien()
        {
            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.hethongid = user.HeThongId;
            return View();
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongAddNewSuKien(SuKienChungModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sự kiện.";
                return RedirectToRoute("HethongAddNewSKC");
            }
            // check sự kiện limit và thời gian gửi
            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var timeDelay = ConfigurationManager.AppSettings["TimeDelayEvent"].ToString();
            if (user.SoLuotSukien == 0)
            {
                TempData["Error"] = ConfigurationManager.AppSettings["strThongBaoHT"].ToString();
                return RedirectToRoute("HethongAddNewSKC");
            }

            var getsukienmoinhat = db.SuKienChungs.OrderByDescending(x => x.NgayTao).ToList().FirstOrDefault();
            if (getsukienmoinhat != null)
            {
                DateTime _ngay1 = (DateTime)getsukienmoinhat.NgayTao;
                double _totalSeconds = (DateTime.Now - _ngay1).TotalSeconds;
                if (_totalSeconds < Convert.ToInt32(timeDelay))
                {
                    TempData["Error"] = ConfigurationManager.AppSettings["strErrorSukien"].ToString();
                    return RedirectToRoute("HethongAddNewSKC");
                }
            }

            try
            {
                string strThongBao = "";
                SuKienChung _newSkc = new SuKienChung();
                _newSkc.HeThongId = model.HeThongId ?? null;
                _newSkc.TDSuKien = model.TDSuKien ?? null;
                _newSkc.NDSuKien = model.NDSuKien ?? null;
                _newSkc.NgayTao = DateTime.Now;
                _newSkc.DaThongBao = model.ConfirmSend;
                _newSkc.NgayBD = model.NgayBD ?? null;
                _newSkc.NgatKT = model.NgayKT ?? null;
                _newSkc.DsAnh = model.DsAnh ?? null;

                //YourString = YourString.Remove(YourString.Length - 1);
                //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
                //_newSkc.DsAnh = _dsanh != "" ? _dsanh : null;

                db.SuKienChungs.Add(_newSkc);
                await db.SaveChangesAsync();

                if (model.ConfirmSend == true)
                {
                    // Gửi thông báo tới người đăng ký nhận thông báo
                    var dssieuthi = db.SieuThis.Where(x => x.HeThongId == model.HeThongId).Select(x => x.SieuThiId).ToList();
                    try
                    {
                        string _fcmAppId = ConfigurationManager.AppSettings["fcmAppId"];
                        string _fcmSenderId = ConfigurationManager.AppSettings["fcmSenderId"];
                        if (GuiThongBaoFcm(_fcmAppId, _fcmSenderId, model.TDSuKien, dssieuthi, model.DsAnh, "HT_" + model.HeThongId.ToString()))
                        {
                            strThongBao = "Đã thêm mới sự kiện và gửi thông báo đến người dùng thành công.";
                            TempData["Updated"] = strThongBao;
                            // trừ đi 1 lượt sự kiên
                            user.SoLuotSukien -= 1;
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
                            return RedirectToRoute("HethongAddNewSKC");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Có lỗi xảy ra gửi thông báo.";
                        return RedirectToRoute("HethongAddNewSKC");
                    }
                }
                else
                {
                    strThongBao = "Đã thêm mới sự kiện " + model.TDSuKien ?? "" + " vào hệ thống";
                    TempData["Updated"] = strThongBao;
                }

            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm sự kiện.";
                return RedirectToRoute("HethongAddNewSKC");
            }
            return RedirectToRoute("HethongAddNewSKC");
        }

        //HeThongListSuKien
        [Authorize(Roles = "hethong")]
        public ActionResult HeThongListSuKien(int? pg, string search)
        {
            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            var data = db.SuKienChungs.Where(x => x.HeThongId == user.HeThongId).Select(x => x);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TDSuKien.Contains(search) || x.NDSuKien.Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderByDescending(x => x.NgayTao);

            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        #region Siêu thị
        //SieuthiAddNewSuKien
        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiAddNewSuKien()
        {
            var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.sieuthiid = user.SieuThiId;
            return View();
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiAddNewSuKien(SuKienModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sự kiện.";
                return RedirectToRoute("SieuthiAddNewSK");
            }

            var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            //var limitEvents = ConfigurationManager.AppSettings["LimitEvents"].ToString();
            var timeDelay = ConfigurationManager.AppSettings["TimeDelayEvent"].ToString();
            //if (db.SuKiens.Where(x => x.SieuThiId == user.SieuThiId).Count() + db.SuKienChungs.Where(x => x.HeThongId == user.HeThongId).Count() > Convert.ToInt32(limitEvents))
            //{
            //    TempData["Error"] = ConfigurationManager.AppSettings["strThongBaoST"].ToString();
            //    return RedirectToRoute("SieuthiAddNewSK");
            //}

            var getSukien1 = db.SuKiens.OrderByDescending(x => x.NgayTao).ToList().FirstOrDefault();
            if (getSukien1 != null)
            {
                DateTime _ngay1 = (DateTime)getSukien1.NgayTao;
                double _totalSeconds = (DateTime.Now - _ngay1).TotalSeconds;
                if (_totalSeconds < Convert.ToInt32(timeDelay))
                {
                    TempData["Error"] = ConfigurationManager.AppSettings["strErrorSukien"].ToString();
                    return RedirectToRoute("SieuthiAddNewSK");
                }
            }

            try
            {
                string strThongbao = "";
                SuKien _newSk = new SuKien();
                _newSk.SieuThiId = model.SieuThiId ?? null;
                _newSk.TDSuKien = model.TDSuKien ?? null;
                _newSk.NDSuKien = model.NDSuKien ?? null;                
                _newSk.NgayTao = DateTime.Now;
                _newSk.DaThongBao = model.ConfirmSend;
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
                
                if (model.indivanh5 != null && model.indivanh5 != "")
                {
                    _dsanh += model.indivanh5 + ",";
                }
                if (_dsanh != "")
                {
                    _dsanh = _dsanh.Remove(_dsanh.Length - 1);
                }  
                //YourString = YourString.Remove(YourString.Length - 1);
                //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
                _newSk.DsAnh = _dsanh != "" ? _dsanh : null;
                _newSk.NgayBD = model.NgayBD ?? null;
                _newSk.NgayKT = model.NgayKT ?? null;
                db.SuKiens.Add(_newSk);
                await db.SaveChangesAsync();

                if (model.ConfirmSend == true)
                {
                    // Gửi thông báo tới người đăng ký nhận thông báo
                    var dssieuthi = db.SieuThis.Where(x => x.SieuThiId == model.SieuThiId).Select(x => x.SieuThiId).ToList();
                    try
                    {
                        string _fcmAppId = ConfigurationManager.AppSettings["fcmAppId"];
                        string _fcmSenderId = ConfigurationManager.AppSettings["fcmSenderId"];
                        if (GuiThongBaoFcm(_fcmAppId, _fcmSenderId, model.TDSuKien, dssieuthi, _dsanh, model.SieuThiId.ToString()))
                        {
                            strThongbao = "Đã thêm mới sự kiện và gửi thông báo.";
                            TempData["Updated"] = strThongbao;
                        }
                        else
                        {
                            TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
                            return RedirectToRoute("SieuthiAddNewSK");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
                        Helpers.Config.SaveToLog(ex.ToString());
                        return RedirectToRoute("SieuthiAddNewSK");
                    }
                }
                else
                {
                    strThongbao = "Đã thêm mới sự kiện " + model.TDSuKien ?? "";
                    TempData["Updated"] = strThongbao;
                }
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm sự kiện.";
                return RedirectToRoute("SieuthiAddNewSK");
            }
            return RedirectToRoute("SieuthiAddNewSK");
        }

        //HeThongEditSukien
        //[Authorize(Roles = "hethong")]
        //public async Task<ActionResult> HeThongEditSukien(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //    SuKienChung _sukien = await db.SuKienChungs.FindAsync(id);
        //    if (_sukien == null)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //    List<string> danhsachanh = new List<string>();
        //    if (_sukien.DsAnh != null)
        //    {
        //        danhsachanh.AddRange(_sukien.DsAnh.Split(','));
        //    }           
        //    if (danhsachanh.Count == 1)
        //    {
        //        danhsachanh.AddRange(new string[] { "", "", "", "" });
        //    }
        //    if (danhsachanh.Count == 2)
        //    {
        //        danhsachanh.AddRange(new string[] { "", "", "" });
        //    }
        //    if (danhsachanh.Count == 3)
        //    {
        //        danhsachanh.AddRange(new string[] { "", "" });
        //    }
        //    if (danhsachanh.Count == 4)
        //    {
        //        danhsachanh.AddRange(new string[] { "" });
        //    }
        //    if (danhsachanh.Count == 0)
        //    {
        //        danhsachanh.AddRange(new string[] { "", "", "", "", "" });
        //    }
        //    var arrayAnh = danhsachanh.ToArray();
        //    var getSuKien = new SuKienChungModel()
        //    {
        //        SuKienChungId = _sukien.SuKienChungId,
        //        HeThongId = _sukien.HeThongId,
        //        ConfirmSend = (bool)_sukien.DaThongBao,
        //        TDSuKien = _sukien.TDSuKien,
        //        NDSuKien = _sukien.NDSuKien,
        //        NgayBD = _sukien.NgayBD,
        //        NgayKT = _sukien.NgatKT
        //    };

        //    return View(getSuKien);
        //}

        //[Authorize(Roles = "hethong")]
        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> HeThongEditSukien(SuKienChungModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return RedirectToRoute("HethongEditSKC", new { id = model.SuKienChungId });
        //    }
        //    var _sukien = await db.SuKienChungs.FindAsync(model.SuKienChungId);

        //    try
        //    {
        //        if (_sukien != null)
        //        {
        //            string strThongbao = "";
        //            _sukien.TDSuKien = model.TDSuKien ?? null;
        //            _sukien.NDSuKien = model.NDSuKien ?? null;
        //            _sukien.NgayTao = DateTime.Now;
        //            _sukien.HeThongId = model.HeThongId;
        //            _sukien.NgayBD = model.NgayBD ?? null;
        //            _sukien.NgatKT = model.NgayKT ?? null;
        //            string _dsanh = "";
        //            if (model.indivanh1 != null && model.indivanh1 != "")
        //            {
        //                _dsanh += model.indivanh1 + ",";
        //            }
                    
        //            if (model.indivanh2 != null && model.indivanh2 != "")
        //            {
        //                _dsanh += model.indivanh2 + ",";
        //            }
                    
        //            if (model.indivanh3 != null && model.indivanh3 != "")
        //            {
        //                _dsanh += model.indivanh3 + ",";
        //            }
                    
        //            if (model.indivanh4 != null && model.indivanh4 != "")
        //            {
        //                _dsanh += model.indivanh4 + ",";
        //            }
                    
        //            if (model.indivanh5 != null && model.indivanh5 != "")
        //            {
        //                _dsanh += model.indivanh5 + ",";
        //            }
        //            if (_dsanh != "")
        //            {
        //                _dsanh = _dsanh.Remove(_dsanh.Length - 1);
        //            }  
        //            //YourString = YourString.Remove(YourString.Length - 1);
        //            //_dsanh = _dsanh.Remove(_dsanh.Length - 1);
        //            _sukien.DsAnh = _dsanh != "" ? _dsanh : null;
        //            db.Entry(_sukien).State = EntityState.Modified;
                    
        //            if (model.ConfirmSend == true)
        //            {
        //                if (_sukien.DaThongBao == true)
        //                {
        //                    strThongbao = "Đã cập nhật sự kiện " + model.TDSuKien ?? "";
        //                    TempData["Updated"] = strThongbao;
        //                    TempData["Error"] = "Sự kiện này đã được gửi thông báo.";
        //                    await db.SaveChangesAsync();
        //                    return RedirectToRoute("HethongEditSKC");
        //                }
        //                else
        //                {
        //                    _sukien.DaThongBao = true;
        //                    await db.SaveChangesAsync();
        //                    // Gửi thông báo tới người đăng ký nhận thông báo
        //                    var dssieuthi = db.SieuThis.Where(x => x.SieuThiId == model.HeThongId).Select(x => x.SieuThiId).ToList();
        //                    try
        //                    {
        //                        string _fcmAppId = ConfigurationManager.AppSettings["fcmAppId"];
        //                        string _fcmSenderId = ConfigurationManager.AppSettings["fcmSenderId"];
        //                        if (GuiThongBaoFcm(_fcmAppId, _fcmSenderId, model.TDSuKien, dssieuthi, _dsanh, "HT_"+model.HeThongId.ToString()))
        //                        {
        //                            strThongbao = "Đã thêm mới sự kiện " + model.TDSuKien ?? "" + " và gửi thông báo.";
        //                            TempData["Updated"] = strThongbao;
        //                            var user = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
        //                            user.SoLuotSukien -= 1;
        //                            db.Entry(user).State = EntityState.Modified;
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
        //                            await db.SaveChangesAsync();
        //                            return RedirectToRoute("HethongEditSKC");
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
        //                        return RedirectToRoute("HethongEditSKC");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                strThongbao = "Đã cập nhật sự kiện " + model.TDSuKien ?? "";
        //                TempData["Updated"] = strThongbao;
        //            }
                    
                   
        //        }
        //        else
        //        {
        //            return RedirectToRoute("AdminPanel");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Có lỗi xảy ra khi sửa sự kiện");
        //        return View(model);
        //    }
                       
        //    return RedirectToRoute("HethongEditSKC", new { id = model.SuKienChungId });
        //}

        //HethongDeleteSKC
        //[Authorize(Roles = "hethong")]
        //public ActionResult HeThongDeleteSuKien(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //    SuKienChung _sukien = db.SuKienChungs.Find(id);
        //    if (_sukien == null)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //    return View(_sukien);
        //}

        //[Authorize(Roles = "hethong")]
        //[HttpPost, ActionName("HeThongDeleteSuKien")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> HeThongDeleteSuKienConfirmed(int? id)
        //{
        //    SuKienChung _sukien = await db.SuKienChungs.FindAsync(id);
        //    if (_sukien == null)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }

        //    try
        //    {
        //        db.SuKienChungs.Remove(_sukien);
        //        await db.SaveChangesAsync();
        //        TempData["Updated"] = "Xóa sự kiện thành công";
        //        return RedirectToRoute("HethongListSKC");
        //    }
        //    catch
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //}

        #endregion

        [Authorize(Roles = "sieuthi")]
        public ActionResult SieuthiListSuKien(int? pg, string search)
        {
            var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            var data = db.SuKiens.Where(x => x.SieuThiId == user.SieuThiId).Select(x => x);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TDSuKien.Contains(search) || x.NDSuKien.Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderByDescending(x => x.NgayTao);

            return View(data.ToList().ToPagedList(pageNumber, pageSize));
        }

        //SieuthiEditSukien
        [Authorize(Roles = "sieuthi")]
        public async Task<ActionResult> SieuthiEditSukien(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            SuKien _sukien = await db.SuKiens.FindAsync(id);
            if (_sukien == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            List<string> danhsachanh = new List<string>();
            if (_sukien.DsAnh != null)
            {
                danhsachanh.AddRange(_sukien.DsAnh.Split(','));
            }
            if (danhsachanh.Count == 1)
            {
                danhsachanh.AddRange(new string[] { "", "", "", "" });
            }
            if (danhsachanh.Count == 2)
            {
                danhsachanh.AddRange(new string[] { "", "", "" });
            }
            if (danhsachanh.Count == 3)
            {
                danhsachanh.AddRange(new string[] { "", "" });
            }
            if (danhsachanh.Count == 4)
            {
                danhsachanh.AddRange(new string[] { "" });
            }
            if (danhsachanh.Count == 0)
            {
                danhsachanh.AddRange(new string[] { "", "", "", "", "" });
            }
            var arrayAnh = danhsachanh.ToArray();
            var getSuKien = new SuKienModel()
            {
                SuKienId = _sukien.SuKienId,
                SieuThiId = _sukien.SieuThiId,
                ConfirmSend = (bool)_sukien.DaThongBao,
                TDSuKien = _sukien.TDSuKien,
                NDSuKien = _sukien.NDSuKien,
                indivanh1 = arrayAnh[0],
                indivanh2 = arrayAnh[1],
                indivanh3 = arrayAnh[2],
                indivanh4 = arrayAnh[3],
                indivanh5 = arrayAnh[4],
                NgayBD = _sukien.NgayBD,
                NgayKT = _sukien.NgayKT
            };

            return View(getSuKien);
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SieuthiEditSukien(SuKienModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin.";
                return RedirectToRoute("SieuthiEditSK", new { id = model.SuKienId });
            }
            var _sukien = await db.SuKiens.FindAsync(model.SuKienId);

            try
            {
                if (_sukien != null)
                {
                    string strThongbao = "";
                    _sukien.TDSuKien = model.TDSuKien ?? null;
                    _sukien.NDSuKien = model.NDSuKien ?? null;
                    _sukien.NgayTao = DateTime.Now;
                    _sukien.SieuThiId = model.SieuThiId;
                    _sukien.NgayBD = model.NgayBD ?? null;
                    _sukien.NgayKT = model.NgayKT ?? null;
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
                    
                    if (model.indivanh5 != null && model.indivanh5 != "")
                    {
                        _dsanh += model.indivanh5 + ",";
                    }
                    
                    //YourString = YourString.Remove(YourString.Length - 1);
                    if (_dsanh != "")
                    {
                        _dsanh = _dsanh.Remove(_dsanh.Length - 1);
                    }                   
                    _sukien.DsAnh = _dsanh != "" ? _dsanh : null;
                    db.Entry(_sukien).State = EntityState.Modified;
                    
                    if (model.ConfirmSend == true)
                    {
                        if (_sukien.DaThongBao == true)
                        {
                            strThongbao = "Đã cập nhật sự kiện " + model.TDSuKien ?? "";
                            TempData["Updated"] = strThongbao;
                            TempData["Error"] = "Sự kiện này đã được gửi thông báo.";
                            await db.SaveChangesAsync();
                            return RedirectToRoute("SieuthiEditSK", new { id = model.SuKienId });
                        }
                        else
                        {
                            _sukien.DaThongBao = true;
                            await db.SaveChangesAsync();
                            // Gửi thông báo tới người đăng ký nhận thông báo
                            var dssieuthi = db.SieuThis.Where(x => x.SieuThiId == model.SieuThiId).Select(x => x.SieuThiId).ToList();
                            try
                            {
                                string _fcmAppId = ConfigurationManager.AppSettings["fcmAppId"];
                                string _fcmSenderId = ConfigurationManager.AppSettings["fcmSenderId"];
                                if (GuiThongBaoFcm(_fcmAppId, _fcmSenderId, model.TDSuKien, dssieuthi, _dsanh, model.SieuThiId.ToString()))
                                {
                                    strThongbao = "Đã thêm mới sự kiện " + model.TDSuKien ?? "" + " và gửi thông báo.";
                                    TempData["Updated"] = strThongbao;
                                    //var user = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                                   
                                }
                                else
                                {
                                    TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
                                    return RedirectToRoute("SieuthiEditSK");
                                }
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo.";
                                return RedirectToRoute("SieuthiEditSK");
                            }
                        }
                    }
                    else
                    {
                        strThongbao = "Đã cập nhật sự kiện " + model.TDSuKien ?? "";
                        TempData["Updated"] = strThongbao;
                        await db.SaveChangesAsync();
                    }
                   

                }
                else
                {
                    return RedirectToRoute("AdminPanel");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa sự kiện");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật mặt hàng thành công";
            return RedirectToRoute("SieuthiEditSK", new { id = model.SuKienId });
        }

        //[Authorize(Roles = "sieuthi")]
        //public ActionResult SieuthiDeleteSuKien(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //    SuKien _sukien = db.SuKiens.Find(id);
        //    if (_sukien == null)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //    return View(_sukien);
        //}

        //[Authorize(Roles = "sieuthi")]
        //[HttpPost, ActionName("SieuthiDeleteSuKien")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SieuthiDeleteSuKienConfirmed(int? id)
        //{
        //    SuKien _sukien = await db.SuKiens.FindAsync(id);
        //    if (_sukien == null)
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }

        //    try
        //    {
        //        db.SuKiens.Remove(_sukien);
        //        await db.SaveChangesAsync();
        //        TempData["Updated"] = "Xóa sự kiện thành công";
        //        return RedirectToRoute("SieuthiListSK");
        //    }
        //    catch
        //    {
        //        return RedirectToRoute("AdminPanel");
        //    }
        //}

        public bool GuiThongBaoFcm(string fcmAppid, string fcmSenderId, string title, List<int> dssieuthi, string dsanh, string strhethongst)
        {
            try
            {
                bool sended = false;
                //lấy danh sách Registration Id
                List<User> ListUser = new List<User>();
                if (dssieuthi.Count > 0)
                {
                    var _userAnroid = db.Users.Where(x => x.RegId != null && !string.IsNullOrEmpty(x.DSSieuThiThongBao)).ToList();
                    if (_userAnroid != null)
                    {
                        foreach (var item in dssieuthi)
                        {
                            foreach (var item2 in _userAnroid)
                            {
                                var dsstnhanTB = item2.DSSieuThiThongBao.Split(',');
                                var ds2 = (from s in dsstnhanTB select s.Trim()).ToArray();
                                if (ds2.Contains(item.ToString()))
                                {
                                    ListUser.Add(item2);
                                }
                            }
                        }
                    }
                }
                ListUser = ListUser.Distinct().ToList();
                if (ListUser.Count > 0)
                {
                    string[] arrRegid = (from r in ListUser select r.RegId).ToArray();

                    //thiết lập GCM send
                    WebRequest tRequest;
                    tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "POST";
                    tRequest.UseDefaultCredentials = true;
                    tRequest.PreAuthenticate = true;
                    tRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
                    //định dạng JSON
                    tRequest.ContentType = "application/json";
                    //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmAppid));

                    tRequest.Headers.Add(string.Format("Sender: id={0}", fcmSenderId));

                    string RegArr = string.Empty;

                    RegArr = string.Join("\",\"", arrRegid);
                    string hinhanh = dsanh != null && dsanh != "" ? dsanh : "";
                    string base64hinhanh = dsanh != "" ? Helpers.Config.Base64Encode(dsanh) : null;
                    //string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + title + ";" + hinhanh + "\",\"id\":\"" + strhethongst + "\"}}"; //"\",\"dsanh\":\"" + dsanh +
                    string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + title + "\",\"hinhanh\": \"" + base64hinhanh + "\",\"id\": \"" + strhethongst + "\",\"collapse_key\":\"" + "xx" + "\"}}";

                    //string postData = Convert.ToBase64String(fileBytes);
                    
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    tRequest.ContentLength = byteArray.Length;

                    Stream dataStream = tRequest.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse tResponse = tRequest.GetResponse();

                    dataStream = tResponse.GetResponseStream();

                    StreamReader tReader = new StreamReader(dataStream);


                    string sResponseFromServer = tReader.ReadToEnd();

                    tReader.Close();
                    dataStream.Close();
                    tResponse.Close();

                    var json = JObject.Parse(sResponseFromServer);  //Turns your raw string into a key value lookup
                    var xyz = json["success"].ToString();

                    if (xyz != "0")
                    {
                        sended = true;
                    }
                }
                return sended;
            }
            catch (Exception ex)
            {
                return false;
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