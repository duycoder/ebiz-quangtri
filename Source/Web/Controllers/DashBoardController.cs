using System.Web.Mvc;
using Web.Common.Elastic;
using Web.Custom;
using Web.Models;
using CommonHelper;
using Business.CommonModel.CONSTANT;
using System.Collections.Generic;

namespace Web.Controllers
{
    public class DashBoardController : BaseController
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            return View();
        }

    }
}