using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebSieuThi.Models;

namespace WebSieuThi.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private sieuthiapiEntities db = new sieuthiapiEntities();

        // GET: api/Users  
        [ResponseType(typeof(UserDTO))]
        [Route("GetAll")]
        [HttpGet]
        public IQueryable<UserDTO> GetUsers()
        {
            var users = from u in db.Users
                        select
                            new UserDTO()
                            {
                                MaUser = u.MaUser,
                                UserEmail = u.UserEmail,
                                RegId = u.RegId,
                                IdSieuThiThongBao = u.DSSieuThiThongBao
                            };
            return users;
        }

        public class UserInfo
        {
            [Required]
            public string userEmail { get; set; }
            public string regId { get; set; }
            public string DeviceId { get; set; }
        }

        public class UserGetID
        {
            public int MaUser { get; set; }
        }

        // POST: api/Users
        [ResponseType(typeof(UserInfo))]
        [Route("insertRegId")]
        [HttpPost]
        public async Task<IHttpActionResult> PostUser([FromBody]UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                 return BadRequest("Vui lòng chèn vào userId và regId");
            }
            var MaUser = default(int);
            //int sended = 0;
            var user = db.Users.Where(x => x.UserEmail == userInfo.userEmail).FirstOrDefault();
            if (user != null)
            {
                user.UserEmail = userInfo.userEmail ?? null;
                user.RegId = userInfo.regId != "" ? userInfo.regId : null;
                user.DeviceId = userInfo.DeviceId != "" ? userInfo.DeviceId : null;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                MaUser = user.MaUser;
                //sended = 1;
            }
            else
            {
                WebSieuThi.Models.User _user = new WebSieuThi.Models.User();
                _user.UserEmail = userInfo.userEmail ?? null;
                _user.RegId = userInfo.regId != "" ? userInfo.regId : null; ;
                _user.DeviceId = userInfo.DeviceId != "" ? userInfo.DeviceId : null;
                _user.DSSieuThiThongBao = null;
                _user.DSSieuThiThongBao = null;
                db.Users.Add(_user);
                await db.SaveChangesAsync();
                MaUser = _user.MaUser;
                //sended = 1;
            }

            return Ok(MaUser);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        [Route("delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        public class ThongTinUser
        {
            [Required]
            public int MaUser { get; set; }
            public string DsSieuThiThongBao { get; set; }
        }

        public class TTMatHangUaThich
        {
            [Required]
            public int MaUser { get; set; }
            public string DSMatHangUaThich { get; set; }
        }

        [HttpPost]
        [Route("PostMatHangUaThich")]
        [ResponseType(typeof(ThongTinUser))]
        public async Task<IHttpActionResult> PostMatHangUaThich(TTMatHangUaThich ttmh)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new { isSuccess = false });
                }
                var user = await db.Users.Where(x => x.MaUser == ttmh.MaUser).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.DSMatHangUaThich = ttmh.DSMatHangUaThich != null && ttmh.DSMatHangUaThich != "" ? ttmh.DSMatHangUaThich : null;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return Ok(new { isSuccess = true });
            }
            catch
            {
                return Ok(new { isSuccess = false });
            }
        }

        [HttpPost]
        [Route("PostSieuThiThongBao")]
        [ResponseType(typeof(ThongTinUser))]
        public async Task<IHttpActionResult> PostSieuThiThongBao(ThongTinUser thongtinUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new { isSuccess = false });
                }
                var user = await db.Users.Where(x => x.MaUser == thongtinUser.MaUser).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.DSSieuThiThongBao = thongtinUser.DsSieuThiThongBao != null && thongtinUser.DsSieuThiThongBao != "" ? thongtinUser.DsSieuThiThongBao : null;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                return Ok(new { isSuccess = true });
            }
            catch
            {
                return Ok(new { isSuccess = false });
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

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.MaUser == id) > 0;
        }
    }
}