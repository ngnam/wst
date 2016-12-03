using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using AdamTibi.Web.Security;

namespace WebSieuThi
{
    public class CookieStore
    {
        //CookieStore.SetCookie("currency", "GBP", TimeSpan.FromDays(1)); // here 1 is no of days for cookie to live
        //string currency= CookieStore.GetCookie("currency");
        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            //HttpCookie encodedCookie = HttpSecureCookie.Encode(new HttpCookie(key, value));
            HttpCookie encodedCookie = new HttpCookie(key, value);

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                // For security purpose, we need to encrypt the value.
                //HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
                HttpCookie clonedCookie = new HttpCookie(cookie.Name, cookie.Value);
                clonedCookie.Domain = cookie.Domain;
                clonedCookie.Expires = cookie.Expires;
                clonedCookie.HttpOnly = cookie.HttpOnly;
                clonedCookie.Path = cookie.Path;
                clonedCookie.Secure = cookie.Secure;
                value = clonedCookie.Value;
            }
            return value;
        }

        public static void DeleteCookie(string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            cookie.Expires = DateTime.Now.Add(TimeSpan.FromDays(-1));
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Response.Cookies.Remove("key");
        }

        public static string Encode(string text, CookieProtection cookieProtection)
        {
            if (string.IsNullOrEmpty(text) || cookieProtection == CookieProtection.None)
            {
                return text;
            }
            byte[] buf = Encoding.UTF8.GetBytes(text);
            return CookieProtectionHelperWrapper.Encode(cookieProtection, buf, buf.Length);
        }

        public static string Decode(string text, CookieProtection cookieProtection)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            byte[] buf;
            try
            {
                buf = CookieProtectionHelperWrapper.Decode(cookieProtection, text);
            }
            catch (Exception ex)
            {
                throw new InvalidCypherTextException(
                    "Unable to decode the text", ex.InnerException);
            }
            if (buf == null || buf.Length == 0)
            {
                throw new InvalidCypherTextException(
                    "Unable to decode the text");
            }
            return Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

    }



}