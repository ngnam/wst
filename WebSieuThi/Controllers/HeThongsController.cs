using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebSieuThi.Models;

namespace WebSieuThi.Controllers
{
    [RoutePrefix("api/hethong")]
    public class HeThongsController : ApiController
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        [HttpGet]
        [Route("{id:int}/sukien")]
        public async Task<IHttpActionResult> GetSuKienOfHeThong(int id)
        {
            var sukien = await db.SuKienChungs.Where(x => x.HeThongId == id).Select(x => new DsSuKien()
            {
                TDSuKien = x.TDSuKien,
                NDSuKien = x.NDSuKien,
                DsHinhAnh = x.DsAnh,
                NgayTao = x.NgayTao,
                NgayBD = x.NgayBD,
                NgayKT = x.NgatKT

            }).ToListAsync();

            sukien = sukien.OrderByDescending(x => x.NgayTao).Take(10).ToList();
            return Ok(sukien);
        }

        public class DsSuKien
        {
            public string TDSuKien { get; set; }
            public string NDSuKien { get; set; }
            public string DsHinhAnh { get; set; }
            public DateTime? NgayTao { get; set; }
            public DateTime? NgayBD { get; set; }
            public DateTime? NgayKT { get; set; }

        }
    }
}
