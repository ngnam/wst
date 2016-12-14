using WebSieuThi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace WebSieuThi.Controllers
{
    public class HomeController : Controller
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Hệ thống quản lý siêu thị";
            var data = db.HeThongs.ToList();
            return View(data);
        }

        public ActionResult Upanh()
        {
            return View();
        }

        public ActionResult upanhtoimg()
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

        // up load anh
        public ActionResult upnhieuanh()
        {
            bool isSavedSuccessfully = true;
            var fName = "";
            try
            {
                WebClient w = new WebClient();
                w.UseDefaultCredentials = true;
                w.Credentials = CredentialCache.DefaultNetworkCredentials;

                w.Headers.Add("Authorization", "Client-ID " + ConfigurationManager.AppSettings["ImgurClientID"]);
                //w.Headers.Add("Sender", "Client-Secret" + ConfigurationManager.AppSettings["ImgurClientSecret"]);

                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        // upanh tới imgur                  
                        byte[] fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, fileBytes.Length);
                        file.InputStream.Close();
                        string postData = Convert.ToBase64String(fileBytes);
                        var values = new NameValueCollection
                            {
                                {"image", postData},
                                {"type", "base64"}
                            };

                        var response = w.UploadValues("https://api.imgur.com/3/image", values);

                        Stream stream = new MemoryStream(response);
                        StreamReader responseReader = new StreamReader(stream);

                        string responseString = responseReader.ReadToEnd();

                        var json = JObject.Parse(responseString);  //Turns your raw string into a key value lookup
                        fName = json["data"]["link"].ToString();
                    }
                }
                w.Dispose();
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



//        public ActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Create(ViewModel vm)
//        {
//            try
//            {
//                //đây chính là API Key: (copy paste từ Google developer nhé)
//                //Server key
////AIzaSyBu5cVjT3tt118vKvpoxfYU4I-X2sTYDEk
////Sender ID
////497869913881
//                Configuration config = WebConfigurationManager.OpenWebConfiguration("~/");
//                //string applicationID = config.AppSettings.Settings["SettingAppId"].ToString();
//                string applicationID = "AIzaSyBu5cVjT3tt118vKvpoxfYU4I-X2sTYDEk";

//                //lấy danh sách Registration Id
//                string[] arrRegid = db.Users.Where(c=> c.DSSieuThiThongBao.Contains(vm.SieuThiId.ToString())).Select(c => c.RegId).ToArray();
//                //đây chính là Sender ID: (copy paste từ Google developer nhé)
//                string SENDER_ID = "497869913881";
//                //lấy nội dung thông báo
//                string value = vm.Title;
//                WebRequest tRequest;
//                //thiết lập GCM send
//                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
//                tRequest.Method = "POST";
//                tRequest.UseDefaultCredentials = true;
//                tRequest.PreAuthenticate = true;
//                tRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
//                //định dạng JSON
//                tRequest.ContentType = "application/json";
//                //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
//                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

//                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

//                string RegArr = string.Empty;

//                RegArr = string.Join("\",\"", arrRegid);

//                string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + value + "\",\"collapse_key\":\"" + value + "\"}}";

//                Console.WriteLine(postData);
//                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
//                tRequest.ContentLength = byteArray.Length;

//                Stream dataStream = tRequest.GetRequestStream();
//                dataStream.Write(byteArray, 0, byteArray.Length);
//                dataStream.Close();

//                WebResponse tResponse = tRequest.GetResponse();

//                dataStream = tResponse.GetResponseStream();

//                StreamReader tReader = new StreamReader(dataStream);

//                String sResponseFromServer = tReader.ReadToEnd();

//                ViewData["result"] = sResponseFromServer; //Lấy thông báo kết quả từ GCM server.
//                tReader.Close();
//                dataStream.Close();
//                tResponse.Close();


//            }
//            catch
//            {
//            }
//            return View();
//        }
    }
}
