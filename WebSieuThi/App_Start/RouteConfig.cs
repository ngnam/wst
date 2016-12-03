using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebSieuThi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "UPloadAnh",
                "upanh",
                new { controller = "Home", action = "Upanh" }
            );

            routes.MapRoute(
               "AdminPanel",
               "admin/dashboard",
               new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
               "RegisterAccount",
               "sign-up",
               new { controller = "Account", action = "Register" }
            );

            ///admin/logout
            routes.MapRoute(
               "adminlogout",
               "admin/logout",
               new { controller = "Account", action = "AdminLogout" }
            );

            routes.MapRoute(
              "LoginAccount",
              "sign-in",
              new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
                "ManagerUserSieuthi",
                "admin/sieuthi/profile",
                new { controller = "Account", action = "ManagerSieuThi" }
            );

            routes.MapRoute(
                "ChangePassUserSieuthi",
                "admin/sieuthi/changepass",
                new { controller = "Account", action = "ChangePasswordSieuThi" }
            );

            routes.MapRoute(
                "ChangePassUserHeThong",
                "admin/hethong/changepass",
                new { controller = "Account", action = "ChangePasswordHeThong" }
            );
            

            routes.MapRoute(
               "ManagerUserHethong",
               "admin/hethong/profile",
               new { controller = "Account", action = "ManagerHethong" }
            );

            routes.MapRoute(
                "HethongAddNewST",
                "admin/hethong/themsieuthi",
                new { controller = "Account", action = "HeThongAddNewSieuThi" }
            );

            routes.MapRoute(
                "HethongListST",
                "admin/hethong/dssieuthi",
                new { controller = "Account", action = "HeThongListSieuThi" }
            );

            routes.MapRoute(
                "HethongEditST",
                "admin/hethong/sieuthi/{id}/edit",
                new { controller = "Account", action = "HeThongEditSieuThi", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongChangePassST",
                "admin/hethong/sieuthi/{id}/changepass",
                new { controller = "Account", action = "HeThongChangePassSieuThi", id = UrlParameter.Optional }
            );
            //

            routes.MapRoute(
                "HethongDeleteST",
                "admin/hethong/sieuthi/{id}/delete",
                new { controller = "Account", action = "HeThongDeleteSieuThi", id = UrlParameter.Optional }
            );

            //hethong
            #region Hệ thống
            routes.MapRoute(
                "HethongAddNewGHC",
                "admin/hethong/themgianhang",
                new { controller = "GianHangs", action = "HethongAddNewGianHangChung" }
            );

            routes.MapRoute(
                "HethongListGHC",
                "admin/hethong/dsgianhang",
                new { controller = "GianHangs", action = "HethongListGianHangChung" }
            );

            routes.MapRoute(
                "HethongEditGHC",
                "admin/hethong/gianhang/{id}/edit",
                new { controller = "GianHangs", action = "HeThongEditGianHangChung", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongDeleteGHC",
                "admin/hethong/gianhang/{id}/delete",
                new { controller = "GianHangs", action = "HeThongDeleteGianHangChung", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongAddNewMHC",
                "admin/hethong/themmathang",
                new { controller = "MatHangs", action = "HeThongAddNewMatHang" }
            );

            routes.MapRoute(
                "HethongListMHC",
                "admin/hethong/dsmathang",
                new { controller = "MatHangs", action = "HeThongListMatHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongEditMHC",
                "admin/hethong/mathang/{id}/edit",
                new { controller = "MatHangs", action = "HeThongEditMatHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongDeleteMHC",
                "admin/hethong/mathang/{id}/delete",
                new { controller = "MatHangs", action = "HeThongDeleteMatHang", id = UrlParameter.Optional }
            );

            // Su kiện chung
            routes.MapRoute(
                "HethongAddNewSKC",
                "admin/hethong/themsukien",
                new { controller = "SuKiens", action = "HeThongAddNewSuKien" }
            );

            routes.MapRoute(
                "HethongListSKC",
                "admin/hethong/dssukien",
                new { controller = "SuKiens", action = "HeThongListSuKien", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongEditSKC",
                "admin/hethong/sukien/{id}/edit",
                new { controller = "SuKiens", action = "HeThongEditSukien", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "HethongDeleteSKC",
                "admin/hethong/sukien/{id}/delete",
                new { controller = "SuKiens", action = "HeThongDeleteSuKien", id = UrlParameter.Optional }
            );



            #endregion

            #region siêu thị

            //sieuthi
            routes.MapRoute(
                "SieuthiAddNewGH",
                "admin/sieuthi/themgianhang",
                new { controller = "GianHangs", action = "SieuthiAddNewGianHang" }
            );

            routes.MapRoute(
                "SieuthiDonHangs",
                "admin/sieuthi/listorders",
                new { controller = "Gianhangs", action = "ListDonHangs" }
            );

            routes.MapRoute(
                "SieuthiListGH",
                "admin/sieuthi/dsgianhang",
                new { controller = "GianHangs", action = "SieuThiListGianHang" }
            );

            routes.MapRoute(
                "SieuthiEditGH",
                "admin/sieuthi/gianhang/{id}/edit",
                new { controller = "GianHangs", action = "SieuThiEditGianHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiDeleteGH",
                "admin/sieuthi/gianhang/{id}/delete",
                new { controller = "GianHangs", action = "SieuthiDeleteGianHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiAddNewMH",
                "admin/sieuthi/themmathang",
                new { controller = "MatHangs", action = "SieuthiAddNewMatHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiListMH",
                "admin/sieuthi/dsmathang",
                new { controller = "MatHangs", action = "SieuthiListMatHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiEditMH",
                "admin/sieuthi/mathang/{id}/edit",
                new { controller = "MatHangs", action = "SieuthiEditMatHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiDeleteMH",
                "admin/sieuthi/mathang/{id}/delete",
                new { controller = "MatHangs", action = "SieuthiDeleteMatHang", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiAddNewSK",
                "admin/sieuthi/themsukien",
                new { controller = "SuKiens", action = "SieuthiAddNewSuKien" }
            );

            routes.MapRoute(
               "SieuthiListSK",
               "admin/sieuthi/dssukien",
               new { controller = "SuKiens", action = "SieuthiListSuKien" }
           );

            routes.MapRoute(
                "SieuthiEditSK",
                "admin/sieuthi/sukien/{id}/edit",
                new { controller = "SuKiens", action = "SieuthiEditSukien", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "SieuthiDeleteSK",
                "admin/sieuthi/sukien/{id}/delete",
                new { controller = "SuKiens", action = "SieuthiDeleteSuKien", id = UrlParameter.Optional }
            );

            #endregion

            #region Authorizetion
            routes.MapRoute(
                "AuthorizeFlickr",
                "auth/flickr",
                new { controller = "Account", action = "FlickrCall" }
            );

            #endregion

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
