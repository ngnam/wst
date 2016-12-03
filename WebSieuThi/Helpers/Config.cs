using WebSieuThi.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net.Mail;
using System.Net;


namespace Helpers
{
    public static class Config
    {
        public static Museum GetLongLat(string strLongLat)
        {
            var result = new Museum();
            Char delimiter = ',';
            String[] x = strLongLat.Split(delimiter);
            result.Latitude = Convert.ToDouble(x[0].Trim());
            result.Longitude = Convert.ToDouble(x[1].Trim());
            return result;
        }

        public static double TinhKhoangCachNguoiDungDenSieuThi(double latA, double longA, double latB, double longB)
        {
            double iKhoangCach;
            try
            {
                // locA(21.0548635, 105.8884966), locB(20.9961054, 105.8618074)
                var locA = new GeoCoordinate(latA, longA);
                var locB = new GeoCoordinate(latB, longB);
                iKhoangCach = locA.GetDistanceTo(locB); // metres
                var x = Math.Round(iKhoangCach, 2, MidpointRounding.AwayFromZero);
                return x;
            }
            catch
            {
                return 0;
            }
        }

        public static IEnumerable<T> myEagerConcat<T>(this IEnumerable<T> first,
                                                    IEnumerable<T> second)
        {
            return (first ?? Enumerable.Empty<T>()).Concat(
                   (second ?? Enumerable.Empty<T>())).ToList();
        }

        public static IEnumerable<int> ConvertStringToInt(string listString)
        {
            var xx = listString.Split(',');
            var yy = from x in xx let a = Convert.ToInt32(x) select a;
            return yy;
        }

        public static IEnumerable<int> ConvertListStringToListInt(List<string> listString)
        {
            var yy = from x in listString let a = Convert.ToInt32(x) select a;
            return yy;
        }


        private const string EncryptionKey = "nguyenvannam2922";
        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static bool Sendmail(string from, string pass, string to, string topic, string content)
        {
            try
            {
                var fromAddress = from;
                var toAddress = to;
                //Password of your gmail address
                string fromPassword = pass;
                // Passing the values and make a email formate to display
                string subject = topic;
                string body = content;
                // smtp settings
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com";//"smtp.gmail.com";
                    smtp.Port = 587;// 465;//587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                    smtp.Timeout = 20000;
                }
                // Passing values to smtp object
                smtp.Send(message);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void SaveToLog(string log)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/" + "log.txt"), true))
            {
                sw.WriteLine(DateTime.Now.ToString() + ": " + log);
                sw.Close();
            }
        }

    }
}