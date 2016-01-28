using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEDALS_Ver01.DAL;

namespace TEDALS_Ver01.Controllers
{
    public class HomeController : Controller
    {
        private TedalsContext db = new TedalsContext();
        public void test()
        {



            
        }
        public ActionResult Index()
        {
            test();
            var user = db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name);
            ViewBag.UserName = user.UserName;
            var rights = new List<string>();
            if (user.IsAdmin)
                rights.Add("Admin");
            if (user.IsReader)
                rights.Add("Reader");
            if(user.IsEditor)
                rights.Add("Editor");
            
            if(user.IsExporter)
                rights.Add("Exporter");
            
            ViewBag.rights = rights;
            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}