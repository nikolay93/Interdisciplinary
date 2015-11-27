using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterdisciplinaryDomainModel.Database;

namespace Interdisciplinary.Controllers
{
    public class UserController : Controller
    {
        private MysenseiEntities DB = new MysenseiEntities();
        // GET: User
        [Authorize]
        public ActionResult Index()
        {
            var users = DB.Users.ToList();

            return View(users);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DB.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}