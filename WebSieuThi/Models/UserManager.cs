using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSieuThi.Models;
using Helpers;

namespace WebSieuThi
{
    public class UserManager
    {
        public bool IsValid(string Email, string Password, bool TypeAccount)
        {
            using (var db = new sieuthiapiEntities()) // use your DbConext
            {
                // if your users set name is Users
                string dpass = Config.Encrypt(Password);
                bool result = false;
                if (TypeAccount == true)
                {
                    result = db.HeThongs.Any(x => x.Email == Email && x.Pass == dpass && x.Actived == true);
                }
                else
                {
                    result = db.SieuThis.Any(x => x.Email == Email && x.pass == dpass);
                }
                return result;
            }
        }

        public bool checkuserlogin1(string email)
        {            
            using (var db = new sieuthiapiEntities())
            {
                bool result = true;
                var userlogin1 = db.HeThongs.Where(x => x.Email == email).FirstOrDefault();
                var userlogin2 = db.SieuThis.Where(x => x.Email == email).FirstOrDefault();
                if (userlogin1 == null && userlogin2 == null)
                {
                    result = false;
                }
                return result;
            }           
        }

        public bool IsEmailExist(string Email)
        {
            using (var db = new sieuthiapiEntities())
            {
                var beht = db.HeThongs.Any(x => x.Email == Email);
                var best = db.SieuThis.Any(x => x.Email == Email);
                return beht || best;
            }
        }

        public bool CheckPassUserSieuthi(string Email, string Password)
        {
            using (var db = new sieuthiapiEntities())
            {
                string dpass = Config.Encrypt(Password);
                bool validPass = false;
                var _sieuthi = db.SieuThis.Where(x => x.Email == Email && x.pass == dpass);
                if (_sieuthi != null)
                {
                    validPass = true;
                }
                return validPass;
            }           
            
        }

        public bool CheckPassUserHeThong(string Email, string Password)
        {
            using (var db = new sieuthiapiEntities())
            {
                string dpass = Config.Encrypt(Password);
                bool validPass = false;
                var _hethong = db.HeThongs.Where(x => x.Email == Email && x.Pass == dpass);
                if (_hethong != null)
                {
                    validPass = true;
                }
                return validPass;
            }

        }

        public bool CheckMaMatHangChung(string xxx) // xxx = sieuthiID + mamathang nhap
        {
            using (var db = new sieuthiapiEntities())
            {
                bool valid = false;
                var _mhc = db.MatHangChungs.Where(x => x.MaMatHangChung == xxx).FirstOrDefault();
                if (_mhc != null)
                {
                    valid = true;
                }
                return valid;
            }
        }

        public bool CheckMaMatHangRieng(string xxx) // xxx = sieuthiID + mamathang nhap
        {
            using (var db = new sieuthiapiEntities())
            {
                bool valid = false;
                var _mh = db.MatHangs.Where(x => x.MaMatHang == xxx).FirstOrDefault();
                if (_mh != null)
                {
                    valid = true;
                }
                return valid;
            }
        }

    }

}