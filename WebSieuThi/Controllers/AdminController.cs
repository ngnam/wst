using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSieuThi.Models;
using System.IO;

namespace WebSieuThi.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        // GET: Admin
        public ActionResult Index()
        {
            try
            {
                if (User.IsInRole("hethong"))
                {
                    ViewBag.dashboard = "dành cho hệ thống siêu thị";
                    var _hethong = db.HeThongs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                    if (_hethong != null)
                    {
                        ViewBag.TotalSieuThi = _hethong.SieuThis.Count;
                        ViewBag.TotalGianHangChung = _hethong.GianHangChungs.Count;
                        int _imh = 0;
                        foreach (var gh in _hethong.GianHangChungs)
                        {
                            _imh += gh.MatHangChungs.Count;
                        }
                        ViewBag.TotalMatHangChung = _imh;
                        ViewBag.TotalSuKienChung = _hethong.SuKienChungs.Count;

                    }

                }
                else if (User.IsInRole("sieuthi"))
                {
                    ViewBag.dashboard = "dành cho siêu thị";
                    var _sieuthi = db.SieuThis.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                    if (_sieuthi != null)
                    {
                        ViewBag.TotalGianHang = _sieuthi.GianHangs.Count;
                        int _imh = 0;
                        foreach (var mh in _sieuthi.GianHangs)
                        {
                            _imh += mh.MatHangs.Count;
                        }
                        ViewBag.TotalMatHang = _imh;
                        ViewBag.TotalDonHang = _sieuthi.DonDatHangs.Count;
                        ViewBag.TotalSuKien = _sieuthi.SuKiens.Count;

                    }

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