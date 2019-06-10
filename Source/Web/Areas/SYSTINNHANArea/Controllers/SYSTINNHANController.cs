using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.SYSTINNHAN;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.SYSTINNHANArea.Controllers
{
    public class SYSTINNHANController : BaseController
    {
        // GET: /SYSTINNHANArea/SYSTINNHAN
        private SYS_TINNHANBusiness SYS_TINNHANBusiness;
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        public ActionResult Index()
        {
            AssignUserInfo();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            SYS_TINNHAN_SEARCH searchModel = new SYS_TINNHAN_SEARCH();
            searchModel.USER_ID = currentUser.ID;
            searchModel.pageSize = MaxPerpage;
            var data = SYS_TINNHANBusiness.GetDaTaByPage(searchModel, MaxPerpage);
            SessionManager.SetValue("nofifSearch", searchModel);
            return View(data);
        }

        #region Các hàm Partialview
        public PartialViewResult Detail(long id)
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            SYS_TINNHAN_BO notif = SYS_TINNHANBusiness.GetInfoBO(id);
            AssignUserInfo();
            if (notif == null || currentUser.ID != notif.TO_USER_ID)
            {
                notif = new SYS_TINNHAN_BO();
            }
            else
            {
                SYS_TINNHAN TinNhan = notif.ToModel();
                if (false == TinNhan.IS_READ || !TinNhan.IS_READ.HasValue)
                {
                    TinNhan.IS_READ = true;
                    SYS_TINNHANBusiness.Save(TinNhan);
                }
            }
            return PartialView("_Detail", notif);
        }
        #endregion
        #region Các hàm json
        public JsonResult Delete(long id)
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            AssignUserInfo();
            SYS_TINNHAN Message = SYS_TINNHANBusiness.Find(id);
            if (Message == null || currentUser.ID != Message.TO_USER_ID)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không có quyền xóa thông báo này" });
            }
            SYS_TINNHANBusiness.repository.Delete(id);
            SYS_TINNHANBusiness.Save();
            return Json(new { Type = "SUCCESS", Message = "Xóa thông báo thành công" });
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            var searchModel = SessionManager.GetValue("nofifSearch") as SYS_TINNHAN_SEARCH;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new SYS_TINNHAN_SEARCH();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("thuoctinhSearch", searchModel);
            }
            var data = SYS_TINNHANBusiness.GetDaTaByPage(searchModel, pageSize, indexPage);
            return Json(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            var searchModel = SessionManager.GetValue("nofifSearch") as SYS_TINNHAN_SEARCH;

            if (searchModel == null)
            {
                searchModel = new SYS_TINNHAN_SEARCH();
                searchModel.pageSize = 20;
            }
            searchModel.TIEUDE = string.IsNullOrEmpty(form["TIEUDE"]) ? form["TIEUDE"] : form["TIEUDE"].Trim();
            if (!string.IsNullOrEmpty(form["TUNGAY"]))
            {
                searchModel.TUNGAY = form["TUNGAY"].ToDateTime();
            }
            if (!string.IsNullOrEmpty(form["DENNGAY"]))
            {
                searchModel.DENNGAY = form["DENNGAY"].ToDateTime();
            }
            if (!string.IsNullOrEmpty(form["TRANGTHAI"]))
            {
                searchModel.TRANGTHAI = form["TRANGTHAI"].Equals("1");
            }
            SessionManager.SetValue("thuoctinhSearch", searchModel);
            var data = SYS_TINNHANBusiness.GetDaTaByPage(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }
        #endregion

        public JsonResult ReadAll()
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            var result = new JsonResultBO(true);
            AssignUserInfo();
            try
            {
                var lstThongBao = SYS_TINNHANBusiness.repository.All().Where(x => x.TO_USER_ID == currentUser.ID && x.IS_READ == false).ToList();
                foreach (var item in lstThongBao)
                {
                    item.IS_READ = true;
                    SYS_TINNHANBusiness.Save(item);
                }
            }
            catch (Exception ex)
            {

                result.MessageFail(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}