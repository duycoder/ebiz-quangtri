using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Business;
using Business.CommonModel.ACTIONAUDIT;
using CommonHelper;
using Web.Areas.ACTIONAUDITArea.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.ACTIONAUDITArea.Controllers
{
    public class ActionAuditController : BaseController
    {
        // GET: ACTIONAUDITArea/ActionAudit
        public ActionResult Index()
        {
            AssignUserInfo();
            var ActionAuditBusiness = Get<ActionAuditBusiness>();
            var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            ActionAuditSearchVM searchModel = new ActionAuditSearchVM();
            ActionAuditLogVM model = new ActionAuditLogVM();
            model.ListResult = ActionAuditBusiness.GetLog();
            model.ListUser = DM_NGUOIDUNGBusiness.repository.All().AsNoTracking().Select(x => new SelectListItem()
            {
                Value = x.TENDANGNHAP,
                Text = x.TENDANGNHAP
            }).ToList();
            SessionManager.SetValue("searchModel", searchModel);
            return View(model);
        }

        public JsonResult searchData(FormCollection coll)
        {
            var ActionAuditBusiness = Get<ActionAuditBusiness>();
            var searchModel = SessionManager.GetValue("searchModel") as ActionAuditSearchVM;
            if (!string.IsNullOrEmpty(coll["TENDANGNHAP"]))
            {
                searchModel.TENDANGNHAP = coll["TENDANGNHAP"];
            }
            else
            {
                searchModel.TENDANGNHAP = "";
            }
            
            SessionManager.SetValue("searchModel", searchModel);
            var data = ActionAuditBusiness.GetLog(searchModel);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            var ActionAuditBusiness = Get<ActionAuditBusiness>();
            var searchModel = SessionManager.GetValue("searchModel") as ActionAuditSearchVM;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new ActionAuditSearchVM();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("VanBanDiSearch", searchModel);
            }
            var data = ActionAuditBusiness.GetLog(searchModel, pageSize, indexPage);
            return Json(data);
        }
    }
}