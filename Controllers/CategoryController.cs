using InterdisciplinaryDomainModel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Interdisciplinary.Controllers
{
    public class CategoryController : Controller
    {
        private MysenseiEntities DB = new MysenseiEntities();
        // GET: Category
        public ActionResult Index()
        {
            var categories = DB.Categories.ToList();
            return View(categories);
        }
    }
}