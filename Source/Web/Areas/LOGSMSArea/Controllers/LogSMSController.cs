using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Custom;
using Business.Business;
using System.Web.Mvc;
using Model.Entities;
using Business.CommonModel.LOGSMS;
using Web.FwCore;
using CommonHelper;

namespace Web.Areas.LogSMSArea.Controllers
{
    public class LogSMSController:BaseController
    {
        LogSMSBusiness LogSMSBusiness;

        public ActionResult Index()
        
        {
            LogSMSBusiness = Get<LogSMSBusiness>();
            var searchmodel = new LOGSMS_SEARCHBO();
            SessionManager.SetValue("TimKiemSMS", null);
            var data = LogSMSBusiness.GetDaTaByPage(null);
            return View(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            LogSMSBusiness = Get<LogSMSBusiness>();
            var searchModel = SessionManager.GetValue("TimKiemSMS") as LOGSMS_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new LOGSMS_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("TimKiemSMS", searchModel);
            }

            var data = LogSMSBusiness.GetDaTaByPage(searchModel, pageSize, indexPage);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            LogSMSBusiness = Get<LogSMSBusiness>();
            var searchModel = SessionManager.GetValue("TimKiemSMS") as LOGSMS_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new LOGSMS_SEARCHBO();
                searchModel.pageSize = 20;
            }

            searchModel.NguoiNhan = form["sea_NguoiNhan"];
            searchModel.NguoiGui = form["sea_NguoiGui"];
            searchModel.DonViGui = form["sea_DonViGui"];
            searchModel.DonViNhan = form["sea_DonViNhan"];
            searchModel.SoDienThoai = form["sea_SoDienThoai"];
            searchModel.TuNgay = form["sea_TuNgay"].ToDataTime();
            searchModel.DenNgay = form["sea_DenNgay"].ToDataTime();
            SessionManager.SetValue("TimKiemSMS", searchModel);

            var data = LogSMSBusiness.GetDaTaByPage(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }

    }
}