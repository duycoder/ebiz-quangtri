using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return View();
            return Redirect("/dashboard");
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

        [AllowAnonymous]
        public ActionResult UnAuthor(){
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult UnAuthorPartial()
        {
            return PartialView("UnAuthorPartial");
        }

        [AllowAnonymous]
        public PartialViewResult TimeOutSession()
        {
            return PartialView("TimeOutSession");
        }
        [AllowAnonymous]
        public ActionResult NotFound()
        {
            return View();
        }
    }
}