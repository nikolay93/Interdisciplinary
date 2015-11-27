using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security;
using InterdisciplinaryDomainModel.Database;

namespace Interdisciplinary.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize(Roles ="Administrator")]
        public ActionResult Index()
        {
            User.IsInRole("Administrator");
            var DB = new MysenseiEntities();
            var model = DB.Roles.ToList();
            return View(model);
        }
    }
}