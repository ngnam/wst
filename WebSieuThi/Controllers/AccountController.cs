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
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace WebSieuThi.Controllers
{
    public class AccountController : Controller
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        //public void init()
        //{
        //    if (!new UserManager().checkuserlogin1(User.Identity.Name))
        //    {
        //        HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //        Response.RedirectToRoute("LoginAccount");
        //    }
        //}
        //void init();

        //Flickr flickr = new Flickr(ConfigurationManager.AppSettings["flickApiKey"], ConfigurationManager.AppSettings["flickApiSecret"]);
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("AdminPanel");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật nhập không đúng")]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Mã bảo mật nhập không đúng");
                return View();
            }
            //FormsAuthentication.SetAuthCookie(model.Email, true);
            if (new UserManager().IsValid(model.Email, model.Password, model.TypeAccount))
            {
                var ident = new ClaimsIdentity(
                  new[] { 
                  // adding following 2 claim just for supporting default antiforgery provider
                  new Claim(ClaimTypes.NameIdentifier, model.Email),
                  new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                  new Claim(ClaimTypes.Name, model.Email),
                  // optionally you could add roles if any
                  model.TypeAccount == true ? new Claim(ClaimTypes.Role, "hethong") : new Claim(ClaimTypes.Role, "sieuthi"),
                  }, DefaultAuthenticationTypes.ApplicationCookie);

                HttpContext.GetOwinContext().Authentication.SignIn(
                   new AuthenticationProperties { IsPersistent = false }, ident);
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl); // auth succeed 
                }
                return RedirectToRoute("AdminPanel"); // auth succeed 
            }

            // invalid username or password
            ModelState.AddModelError("", "Tên đăng nhập, mật khẩu không đúng hoặc tài khoản chưa được kích hoạt.");
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.lstLoaiHinhKinhDoanh = ListLoaiHinhKD();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin đăng ký.";
                return RedirectToAction("Register");
            }
            if (new UserManager().IsEmailExist(model.Email))
            {
                TempData["Error"] = "Địa chỉ email đã được sử dụng.";
                return RedirectToAction("Register");
            }

            try
            {
                HeThong _newHeThong = new HeThong();
                _newHeThong.Email = model.Email ?? null;
                _newHeThong.Country = model.Country ?? null;
                _newHeThong.CreatedDate = DateTime.Now;
                _newHeThong.AnhIcon = model.AnhIcon ?? null;
                _newHeThong.Actived = false;
                _newHeThong.Picture_GP_KD = model.Picture_GP_KD != null ? "http://" + Request.Url.Host + model.Picture_GP_KD : null;
                _newHeThong.Pass = model.Pass != null ? Helpers.Config.Encrypt(model.Pass) : null;
                _newHeThong.TenHeThong = model.TenHeThong ?? null;
                _newHeThong.SDT = model.SDT ?? null;
                _newHeThong.LoaiHinhKD = model.LoaiHinhKD ?? null;
                _newHeThong.SoLuotSukien = 100;// mac dinh tao
                db.HeThongs.Add(_newHeThong);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Tài khoản đăng ký đang được xử lý.";
                StringBuilder strBody = new StringBuilder();
                strBody.AppendFormat("Thông tin đăng ký tài khoản {0} <br />: Địa chỉ Email - {1}. <br /> Quốc gia: {2}. <br /> Ảnh giấy phép kinh doanh: {3}. <br /> Số điện thoại: {4} <br /> Loại hình kinh doanh {5}.", model.TenHeThong, model.Email, model.Country, Request.Url.Host + model.Picture_GP_KD, model.SDT, model.LoaiHinhKD);
                strBody.AppendLine("Vui lòng kích hoạt tài khoản cho tôi.");
                strBody.AppendLine("Hà Nội, Ngày " + DateTime.Now.ToString() + ".");
                string _bodyMail = strBody.ToString();
                string _topicMail = string.Format("Thông tin đăng ký tài khoản {0}", model.TenHeThong);
                if (Helpers.Config.Sendmail(ConfigurationManager.AppSettings["yourEmail"], ConfigurationManager.AppSettings["yourPass"], ConfigurationManager.AppSettings["adminEmail"], _topicMail, _bodyMail)) {
                    TempData["Updated"] = "Thông tin đăng ký đã được gửi tới ban quản trị. Ban quản trị sẽ xem xét để kích hoạt tài khoản của bạn.";
                };
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi đăng ký tài khoản.";
                return RedirectToAction("Register");
            }
            return RedirectToAction("Register");
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");
            return RedirectToRoute("AdminPanel");
        }

        public ActionResult AdminLogout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");
            return RedirectToRoute("AdminPanel");
        }

        //[Authorize]
        //public ActionResult GetAuthenticateToken()
        //{
        //    string frob = Request.QueryString["frob"].ToString();
        //    CookieStore.SetCookie("auth_user_frob", frob, TimeSpan.FromDays(1));
        //    string script = "<script type='text/javascript'>setInterval(function() {console.log('xx');}, 1000)</script>";
        //    return Content(script);
        //}

        [AllowAnonymous]
        public ActionResult SaveAnhLocal()
        {
            bool isSavedSuccessfully = true;
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}store", Server.MapPath(@"\")));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString();
                        string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), strDay);

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/store/" + strDay + "/" + _fileName;
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult SaveAnhImgur()
        {
            bool isSavedSuccessfully = true;
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        // upanh tới imgur                   
                        
                        using (var w = new WebClient())
                        {
                            byte[] fileBytes = new byte[file.ContentLength];
                            file.InputStream.Read(fileBytes, 0, fileBytes.Length);
                            file.InputStream.Close();
                            string postData = Convert.ToBase64String(fileBytes);
                            var values = new NameValueCollection
                            {
                                {"image", postData},
                                {"type", "base64"}
                            };

                            w.UseDefaultCredentials = true;
                            w.Credentials = CredentialCache.DefaultNetworkCredentials;

                            w.Headers.Add("Authorization", "Client-ID " + ConfigurationManager.AppSettings["ImgurClientID"]);
                            //w.Headers.Add("Sender", "Client-Secret" + ConfigurationManager.AppSettings["ImgurClientSecret"]);
                           
                            var response = w.UploadValues("https://api.imgur.com/3/image", values);

                            Stream stream = new MemoryStream(response);
                            StreamReader responseReader = new StreamReader(stream);

                            string responseString = responseReader.ReadToEnd();

                            var json = JObject.Parse(responseString);  //Turns your raw string into a key value lookup
                            fName = json["data"]["link"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" }, JsonRequestBehavior.AllowGet);
            }
        }

        //up more image
        [Authorize]
        public ActionResult SaveMoreAnhImgur()
        {
            bool isSavedSuccessfully = true;
            List<string> fName = new List<string>();
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        // upanh tới imgur                 
                        using (var w = new WebClient())
                        {
                            byte[] fileBytes = new byte[file.ContentLength];
                            file.InputStream.Read(fileBytes, 0, fileBytes.Length);
                            file.InputStream.Close();
                            string postData = Convert.ToBase64String(fileBytes);
                            var values = new NameValueCollection
                            {
                                {"image", postData},
                                {"type", "base64"}
                            };
                            w.UseDefaultCredentials = true;
                            w.Credentials = CredentialCache.DefaultNetworkCredentials;
                            w.Headers.Add("Authorization", "Client-ID " + ConfigurationManager.AppSettings["ImgurClientID"]);
                            //w.Headers.Add("Sender", "Client-Secret" + ConfigurationManager.AppSettings["ImgurClientSecret"]);

                            var response = w.UploadValues("https://api.imgur.com/3/image", values);

                            Stream stream = new MemoryStream(response);
                            StreamReader responseReader = new StreamReader(stream);

                            string responseString = responseReader.ReadToEnd();

                            var json = JObject.Parse(responseString);  //Turns your raw string into a key value lookup
                            string url_link = json["data"]["link"].ToString();
                            fName.Add(url_link);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" }, JsonRequestBehavior.AllowGet);
            }
        }
        // upanh imgur
        

        //[Authorize]
        //public ActionResult SaveAnh1()
        //{
        //    bool isSavedSuccessfully = true;
        //    var fName = "";
        //    try
        //    {
        //        string authToken = CookieStore.GetCookie("auth_user_frob");
        //        if (authToken != null && authToken != "")
        //        {
        //            Flickr flickr = new Flickr(ConfigurationManager.AppSettings["flickApiKey"], ConfigurationManager.AppSettings["flickApiSecret"]);
        //            FlickrNet.Cache.CacheDisabled = true;
        //            Auth auth = flickr.AuthGetToken(authToken);
        //            auth.Permissions = AuthLevel.Write;
        //            foreach (string fileName in Request.Files)
        //            {
        //                HttpPostedFileBase file = Request.Files[fileName];
        //                //Save file content goes here
        //                if (file != null && file.ContentLength > 0)
        //                {
        //                    string FileuploadedID = flickr.UploadPicture(file.InputStream, file.FileName, file.FileName, "", "", true, false, false, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.Hidden);
        //                    PhotoInfo oPhotoInfo = flickr.PhotosGetInfo(FileuploadedID);
        //                    fName = oPhotoInfo.LargeUrl;
        //                }
        //            }
        //            CookieStore.DeleteCookie("auth_user_frob");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        isSavedSuccessfully = false;
        //    }
        //    if (isSavedSuccessfully)
        //    {
        //        return Json(new { Message = fName }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Message = "Có lỗi khi lưu tệp tin" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Authorize]
        //public ActionResult SaveMoreImage()
        //{
        //    bool isSavedSuccessfully = true;
        //    List<string> fName = new List<string>();
        //    try
        //    {
        //        string authToken = CookieStore.GetCookie("auth_user_frob");
        //        if (authToken != null && authToken != "")
        //        {
        //            Flickr flickr = new Flickr(ConfigurationManager.AppSettings["flickApiKey"], ConfigurationManager.AppSettings["flickApiSecret"]);
        //            FlickrNet.Cache.CacheDisabled = true;
        //            Auth auth = flickr.AuthGetToken(authToken);
        //            auth.Permissions = AuthLevel.Write;
        //            foreach (string fileName in Request.Files)
        //            {
        //                HttpPostedFileBase file = Request.Files[fileName];
        //                //Save file content goes here
        //                if (file != null && file.ContentLength > 0)
        //                {
        //                    //var _fileName = Guid.NewGuid().ToString("N");                        
        //                    string FileuploadedID = flickr.UploadPicture(file.InputStream, file.FileName, file.FileName, "", "", true, false, false, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.Hidden);
        //                    PhotoInfo oPhotoInfo = flickr.PhotosGetInfo(FileuploadedID);
        //                    fName.Add(oPhotoInfo.LargeUrl);
        //                }
        //            }
        //            //CookieStore.DeleteCookie("auth_user_frob");
        //        }
                
                
        //    }
        //    catch (Exception ex)
        //    {
        //        isSavedSuccessfully = false;
        //    }
        //    if (isSavedSuccessfully)
        //    {
        //        return Json(new { Message = fName }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Message = "Có lỗi khi lưu tệp tin" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        //public ActionResult authorized()
        //{
        //    try
        //    {        //[Authorize]
        //public ActionResult FlickrCall()
        //{
        //    Flickr flickr = new Flickr(ConfigurationManager.AppSettings["flickApiKey"], ConfigurationManager.AppSettings["flickApiSecret"]);
        //    FlickrNet.Cache.CacheDisabled = true;
        //    string flickrUrl = flickr.AuthCalcWebUrl(AuthLevel.Write);
        //    //return Redirect(flickrUrl);
        //    return Json(flickrUrl, JsonRequestBehavior.AllowGet);
        //}
        //        string frob = Request.QueryString["frob"];
        //        Auth auth = flickr.AuthGetToken(frob);
        //        auth.Permissions = AuthLevel.Write;
        //        string file = Session["filepath"].ToString();
        //        string FileuploadedID = flickr.UploadPicture(file, Path.GetFileName(file), "");
        //        PhotoInfo oPhotoInfo = flickr.PhotosGetInfo(FileuploadedID);
                
        //        return Json (new { Message = oPhotoInfo.LargeUrl }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = false });
        //    }
        //}



        [Authorize(Roles = "hethong")]
        public ActionResult ManagerHethong()
        {
            var _hethong = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (_hethong == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToRoute("LoginAccount");
            }
            var getHeThong = new HeThongModel()
            {
                HethongId = _hethong.HeThongId,
                Email = _hethong.Email,
                Country = _hethong.Country,
                AnhIcon = _hethong.AnhIcon,
                Picture_GP_KD = _hethong.Picture_GP_KD,
                TenHeThong = _hethong.TenHeThong,
                SDT = _hethong.SDT,
                LoaiHinhKD = _hethong.LoaiHinhKD
            };
            ViewBag.lstLoaiHinhKinhDoanh = ListLoaiHinhKD();
            return View(getHeThong);
        }

        //UpdateProfileHeThong
        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfileHeThong(HeThongModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin.";
                return RedirectToRoute("ManagerUserHethong");
            }
            var _hethong = await db.HeThongs.FindAsync(model.HethongId);
            if (_hethong != null)
            {
                if (new UserManager().IsEmailExist(model.Email) && _hethong.Email != model.Email )
                {
                    TempData["Error"] = "Địa chỉ email đã được sử dụng.";
                    return RedirectToRoute("ManagerUserHethong");
                }
                _hethong.Email = model.Email ?? null;
                _hethong.AnhIcon = model.AnhIcon ?? null;
                _hethong.Picture_GP_KD = model.Picture_GP_KD != null ? "http://" + Request.Url.Host + model.Picture_GP_KD : null;
                _hethong.Country = model.Country ?? null;
                _hethong.TenHeThong = model.TenHeThong ?? null;
                _hethong.SDT = model.SDT ?? null;
                _hethong.LoaiHinhKD = model.LoaiHinhKD ?? null;
                db.Entry(_hethong).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (User.Identity.Name.ToString() != _hethong.Email)
                {
                    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToRoute("LoginAccount");
                }
                TempData["Updated"] = "Cập nhật thông tin hệ thống thành công";
                return RedirectToRoute("ManagerUserHethong");
            }
            return View();
        }

        [Authorize(Roles = "hethong")]
        public ActionResult ChangePasswordHeThong()
        {
            return View();
        }

        //ChangePasswordHeThong
        [Authorize(Roles = "hethong")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePasswordHeThong(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (new UserManager().CheckPassUserHeThong(User.Identity.Name, model.OldPassword))
            {
                var _hethong = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                if (_hethong != null)
                {
                    _hethong.Pass = model.NewPassword != null ? Helpers.Config.Encrypt(model.NewPassword) : null;
                    db.Entry(_hethong).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Updated"] = "Cập nhật mật khẩu mới thành công!";
                    var ident = new ClaimsIdentity(
                      new[] { 
                      // adding following 2 claim just for supporting default antiforgery provider
                      new Claim(ClaimTypes.NameIdentifier, _hethong.Email),
                      new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                      new Claim(ClaimTypes.Name, _hethong.Email),
                      // optionally you could add roles if any
                      new Claim(ClaimTypes.Role, "hethong"),
                      }, DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties { IsPersistent = false }, ident);
                }
                return RedirectToRoute("ChangePassUserHeThong");
            }
            ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
            return View();
        }

        [Authorize(Roles="sieuthi")]
        public ActionResult ManagerSieuThi()
        {
            var _sieuthi = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (_sieuthi == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToRoute("LoginAccount");
            }
            var getSieuthi = new SieuThiModel()
            {
                SieuThiId = _sieuthi.SieuThiId,
                TenSieuThi = _sieuthi.TenSieuThi,
                DiaChi = _sieuthi.DiaChi,
                DienThoai = _sieuthi.DienThoai,
                CuocPhiVC = _sieuthi.CuocPhiVC,
                Email = _sieuthi.Email,
                longlat = _sieuthi.longlat,
                GioMoCua = _sieuthi.GioMoCua
            };
            
            return View(getSieuthi);
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfileSieuThi(SieuThiModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin";
                return RedirectToRoute("ManagerUserSieuthi");
            }
            try
            {
                var _sieuthi = await db.SieuThis.FindAsync(model.SieuThiId);
                if (_sieuthi != null)
                {
                    if (new UserManager().IsEmailExist(model.Email) && _sieuthi.Email != model.Email)
                    {
                        TempData["Error"] = "Địa chỉ email đã được sử dụng.";
                        return RedirectToRoute("ManagerUserSieuthi");
                    }
                    _sieuthi.TenSieuThi = model.TenSieuThi ?? null;
                    _sieuthi.DiaChi = model.DiaChi ?? null;
                    _sieuthi.DienThoai = model.DienThoai ?? null;
                    _sieuthi.Email = model.Email ?? null;
                    _sieuthi.CuocPhiVC = model.CuocPhiVC ?? null;
                    _sieuthi.longlat = model.longlat ?? null;
                    _sieuthi.GioMoCua = model.GioMoCua ?? null;
                    db.Entry(_sieuthi).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    if (User.Identity.Name.ToString() != _sieuthi.Email)
                    {
                        HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return RedirectToRoute("LoginAccount");
                    }
                    TempData["Updated"] = "Cập nhật thông tin siêu thị thành công";
                    return RedirectToRoute("ManagerUserSieuthi");
                }
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(Server.MapPath("../" + "log.txt"));
                sw.WriteLine(ex.ToString());
                sw.Close();
            }
            
            return View();
        }

        

        [Authorize(Roles="sieuthi")]
        public ActionResult ChangePasswordSieuThi()
        {
            return View();
        }

        [Authorize(Roles = "sieuthi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePasswordSieuThi(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (new UserManager().CheckPassUserSieuthi(User.Identity.Name, model.OldPassword))
            {
                var _sieuthi = db.SieuThis.Where(x=>x.Email == User.Identity.Name).FirstOrDefault();
                if (_sieuthi != null)
	            {
                    _sieuthi.pass = model.NewPassword != null ? Helpers.Config.Encrypt(model.NewPassword) : null;
                    db.Entry(_sieuthi).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Updated"] = "Cập nhật mật khẩu mới thành công!";
                    var ident = new ClaimsIdentity(
                      new[] { 
                      // adding following 2 claim just for supporting default antiforgery provider
                      new Claim(ClaimTypes.NameIdentifier, _sieuthi.Email),
                      new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                      new Claim(ClaimTypes.Name, _sieuthi.Email),
                      // optionally you could add roles if any
                      new Claim(ClaimTypes.Role, "sieuthi"),
                      }, DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties { IsPersistent = false }, ident);
                }                
                return RedirectToRoute("ChangePassUserSieuthi");
            }
            ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
            return View();
        }

        [Authorize(Roles="hethong")]
        public ActionResult HeThongAddNewSieuThi()
        {
            return View();
        }

        //HeThongAddNewSieuThi
        [Authorize(Roles="hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongAddNewSieuThi(NewSieuThiModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin đăng ký.";
                return RedirectToRoute("HethongAddNewST");
            }
            if (new UserManager().IsEmailExist(model.Email))
            {
                TempData["Error"] = "Địa chỉ email đã được sử dụng.";
                return RedirectToRoute("HethongAddNewST");
            }
            int HeThongId = 0;
            var userLogin = User.Identity.Name != null ? db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault() : null;
            if (userLogin != null)
            {
                HeThongId = userLogin.HeThongId;
            }
            try
            {
                int idsieuthimoi = 0;
                SieuThi _newsieuthi = new SieuThi();
                _newsieuthi.TenSieuThi = model.TenSieuThi ?? null;
                _newsieuthi.DiaChi = model.DiaChi ?? null;
                _newsieuthi.DienThoai = model.DienThoai ?? null;
                _newsieuthi.Email = model.Email ?? null;
                _newsieuthi.CuocPhiVC = model.CuocPhiVC ?? null;
                _newsieuthi.longlat = model.longlat ?? null;
                _newsieuthi.pass = model.Pass != null ? Helpers.Config.Encrypt(model.Pass) : null;
                _newsieuthi.HeThongId = HeThongId != 0 ? HeThongId : 0;
                _newsieuthi.GioMoCua = model.GioMoCua ?? null;
                db.SieuThis.Add(_newsieuthi);
                idsieuthimoi = _newsieuthi.SieuThiId;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới siêu thị " + model.TenSieuThi ?? "" + " vào hệ thống";

                // copy gian hàng chung hệ thống vào cho siêu thị vừa tạo
                try
                {
                    var ghc = db.GianHangChungs.Where(x => x.HeThongId == HeThongId).ToList();
                    if (ghc.Count > 0)
                    {
                        foreach (var item in ghc)
                        {
                            GianHang gh = new GianHang();
                            gh.SieuThiId = idsieuthimoi;
                            gh.MaGianHang = item.MaGianHangChung;
                            gh.TenGianHang = item.TenGianHangChung;
                            gh.AnhGianHang = item.AnhGianHang;
                            db.GianHangs.Add(gh);
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.Config.SaveToLog(ex.ToString());
                }
                

            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi đăng ký tài khoản.";
                return RedirectToRoute("HethongAddNewST");
            }
            return RedirectToRoute("HethongListST");
        }

        #region Danh sách siêu thị
        //HeThongListSieuThi
        [Authorize(Roles = "hethong")]
        public ActionResult HeThongListSieuThi(int? pg, string search, string loaihinhkd, string diachi)
        {
            var userlogin = User.Identity.Name != null ? db.HeThongs.Where(x=>x.Email == User.Identity.Name).FirstOrDefault() : null;
            if (userlogin == null)
	        {
		       return RedirectToRoute("LoginAccount");
	        }
            var data = db.SieuThis.Where(x => x.HeThongId == userlogin.HeThongId).Select(x=>x);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenSieuThi.Contains(search) || x.Email.Contains(search) || x.DienThoai.Contains(search));
                ViewBag.search = search;
            }

            if (loaihinhkd == null) loaihinhkd = ""; if (diachi == null) diachi = "";
            if (loaihinhkd != null && loaihinhkd != "")
            {
                data = data.Where(t => t.HeThong.LoaiHinhKD == loaihinhkd);
                ViewBag.loaihinhkd = loaihinhkd;
            }

            if (diachi != null && diachi != "")
            {
                data = data.Where(s => s.DiaChi == diachi);
                ViewBag.diachi = diachi;
            }

            data = data.OrderByDescending(d => d.TenSieuThi);
            ViewBag.lstloaihinhkd = ListLoaiHinhKD();
            ViewBag.lstdiachi = db.SieuThis.Where(x => x.DiaChi != null).Select(x=>x.DiaChi).ToList().Distinct().Select(x => new SelectListItem()
            {
                Value = x,
                Text = x
            }).ToList();

            return View(data.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        

        [Authorize(Roles="hethong")]
        public async Task<ActionResult> HeThongEditSieuThi(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            SieuThi _sieuthi = await db.SieuThis.FindAsync(id);
            if (_sieuthi == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            var getSieuthi = new SieuThiModel()
            {
                SieuThiId = _sieuthi.SieuThiId,
                TenSieuThi = _sieuthi.TenSieuThi,               
                DiaChi = _sieuthi.DiaChi,
                DienThoai = _sieuthi.DienThoai,
                CuocPhiVC = _sieuthi.CuocPhiVC,
                Email = _sieuthi.Email,
                longlat = _sieuthi.longlat,
                HeThongId = (int)_sieuthi.HeThongId,
                GioMoCua = _sieuthi.GioMoCua
            };
            return View(getSieuthi);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongEditSieuThi(SieuThiModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("HethongEditST", new { id = model.SieuThiId });
            }
            var _sieuthi = await db.SieuThis.FindAsync(model.SieuThiId);
            
            try
            {
                if (_sieuthi != null)
                {
                    _sieuthi.TenSieuThi = model.TenSieuThi ?? null;
                    _sieuthi.HeThongId = model.HeThongId != null ? (int?)model.HeThongId : (int?)null;                    
                    _sieuthi.longlat = model.longlat ?? null;
                    _sieuthi.DiaChi = model.DiaChi ?? null;
                    _sieuthi.DienThoai = model.DienThoai ?? null;
                    if (model.Email == _sieuthi.Email)
                    {
                        _sieuthi.Email = model.Email ?? "";
                    }
                    else if(new UserManager().IsEmailExist(model.Email))
                    {
                        ModelState.AddModelError("", "Địa chỉ email đã được sử dụng.");
                        return View(model);
                    }
                    else
                    {
                        _sieuthi.Email = model.Email ?? "";
                    }
                    _sieuthi.CuocPhiVC = model.CuocPhiVC ?? null;
                    _sieuthi.GioMoCua = model.GioMoCua ?? null;
                    
                    db.Entry(_sieuthi).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    return RedirectToRoute("AdminPanel");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa siêu thị");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật thông tin siêu thị thành công";
            return RedirectToRoute("HethongListST");
        }

        [Authorize(Roles="hethong")]
        public ActionResult HeThongDeleteSieuThi(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("AdminPanel");
            }
            SieuThi _sieuthi = db.SieuThis.Find(id);
            if (_sieuthi == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            return View(_sieuthi);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost, ActionName("HeThongDeleteSieuThi")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HeThongDeleteSieuThiConfirmed(int? id)
        {
            SieuThi _sieuthi = await db.SieuThis.FindAsync(id);
            if (_sieuthi == null)
            {
                return RedirectToRoute("AdminPanel");
            }

            try
            {
                if (_sieuthi.DonDatHangs.Count > 0)
                {
                    TempData["Updated"] = "Siêu thị đang chứa đơn hàng không thể xóa.";
                    return RedirectToRoute("HethongDeleteST", new { id = _sieuthi.SieuThiId });
                }
                if (_sieuthi.GianHangs.Count > 0)
                {
                    TempData["Updated"] = "Siêu thị đang chứa gian hàng không thể xóa.";
                    return RedirectToRoute("HethongDeleteST", new { id = _sieuthi.SieuThiId });
                }
                if (_sieuthi.SuKiens.Count > 0)
                {
                    TempData["Updated"] = "Siêu thị đang chứa sự kiện không thể xóa.";
                    return RedirectToRoute("HethongDeleteST", new { id = _sieuthi.SieuThiId });
                }

                db.SieuThis.Remove(_sieuthi);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa siêu thị thành công";
                 return RedirectToRoute("HethongListST");
            }
            catch
            {
                return RedirectToRoute("AdminPanel");
            }           
        }

        [Authorize(Roles = "hethong")]
        public ActionResult HeThongChangePassSieuThi(int? id)
        {
            if (id == default(int) || id == 0)
            {
                 return RedirectToRoute("AdminPanel");
            }
            SieuThi _sieuthi = db.SieuThis.Find(id);
            if (_sieuthi == null)
            {
                return RedirectToRoute("AdminPanel");
            }
            var getSieuthi = new ChangePasswordSieuthiViewModel()
            {
                SieuthiId = _sieuthi.SieuThiId
            };
            ViewBag.TenSieuThi = _sieuthi.TenSieuThi;
            return View(getSieuthi);
        }

        [Authorize(Roles = "hethong")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HeThongChangePassSieuThi(ChangePasswordSieuthiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (new UserManager().CheckPassUserSieuthi(User.Identity.Name, model.OldPassword))
            {
                var _sieuthi = db.SieuThis.Find(model.SieuthiId);
                if (_sieuthi != null)
                {
                    _sieuthi.pass = model.NewPassword != null ? Helpers.Config.Encrypt(model.NewPassword) : null;
                    db.Entry(_sieuthi).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Updated"] = "Cập nhật mật khẩu mới thành công!";
                }
                return RedirectToRoute("HethongChangePassST", new { id = model.SieuthiId });
            }
            ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
            return View();
        }

        public List<SelectListItem> ListLoaiHinhKD()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "Trung tâm thương mại", Text = "Trung tâm thương mại" });
            newList.Add(new SelectListItem() { Value = "Siêu thị bán lẻ", Text = "Siêu thị bán lẻ" });
            newList.Add(new SelectListItem() { Value = "Siêu thị điện máy", Text = "Siêu thị điện máy" });
            newList.Add(new SelectListItem() { Value = "Thời trang", Text = "Thời trang" });
            newList.Add(new SelectListItem() { Value = "Ẩm thực", Text = "Ẩm thực" });
            return newList;
        }

        public void ExportToExcel()
        {
            try
            {
                var model = (from s in db.SieuThis select s);
                if (model != null)
                {
                    Export export = new Export();
                    export.ToExcel(Response, model.ToList(), "DSSieuthi_"+DateTime.Now.ToString("yyyyMMdd")+".xls");
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

        public void ImportToExcel()
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