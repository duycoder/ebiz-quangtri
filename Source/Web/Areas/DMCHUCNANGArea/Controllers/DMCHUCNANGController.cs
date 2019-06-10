using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonHelper;
using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using Web.Custom;
using Web.FwCore;
using Business.CommonModel.DMCHUCNANG;
using Web.Areas.DMCHUCNANGArea.Models;
using Web.Filter;


namespace Web.Areas.DMCHUCNANGArea.Controllers
{
    public class DMCHUCNANGController : BaseController
    {
        #region khaibao
        DM_CHUCNANGBusiness DM_CHUCNANGBusiness;
        #endregion
        [CodeAllowAccess(Code = "DsChucNang")]
        public ActionResult Index()
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var searchmodel = new DM_CHUCNANG_SEARCHBO();
            SessionManager.SetValue("dmchucnangSearchModel", null);
            var data = DM_CHUCNANGBusiness.GetDaTaByPage(null);
            return View(data);
        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var searchModel = SessionManager.GetValue("dmchucnangSearchModel") as DM_CHUCNANG_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new DM_CHUCNANG_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("dmchucnangSearchModel", searchModel);
            }
            var data = DM_CHUCNANGBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        public PartialViewResult Create()
        {
            return PartialView("_CreatePartial");
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new DM_CHUCNANG();
                myobj.TT_HIENTHI = collection["TT_HIENTHI"].ToIntOrZero();
                myobj.IS_HIENTHI = collection["IS_HIENTHI"].ToBoolByOnOff();
                myobj.NGAYTAO = DateTime.Now;
                myobj.MA_CHUCNANG = collection["MA_CHUCNANG"].ToString();
                myobj.TEN_CHUCNANG = collection["TEN_CHUCNANG"].ToString();
                myobj.URL = collection["URL"].ToString();
                myobj.ICONPATH = collection["ICONPATH"].ToString();
                myobj.CSSCLASS = collection["CSSCLASS"].ToString();
                DM_CHUCNANGBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không thêm mới được";
            }
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var searchModel = SessionManager.GetValue("dmchucnangSearchModel") as DM_CHUCNANG_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new DM_CHUCNANG_SEARCHBO();
                searchModel.pageSize = 20;
            }

            searchModel.QR_MA = form["QR_MA"];
            searchModel.QR_CHUCNANG = form["QR_CHUCNANG"];
            SessionManager.SetValue("dmchucnangSearchModel", searchModel);

            var data = DM_CHUCNANGBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }

        [HttpPost]
        public JsonResult CheckExsitCode(long idchucnang, string code)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            return Json(DM_CHUCNANGBusiness.checkExistCode(code, idchucnang));
        }
        public PartialViewResult Edit(long id)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var myModel = new EditVM();
            myModel.objModel = DM_CHUCNANGBusiness.repository.Find(id);
            return PartialView("_EditPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = DM_CHUCNANGBusiness.Find(id);
                myobj.TT_HIENTHI = collection["TT_HIENTHI"].ToIntOrZero();
                myobj.IS_HIENTHI = collection["IS_HIENTHI"].ToBoolByOnOff();
                myobj.NGAYSUA = DateTime.Now;
                myobj.MA_CHUCNANG = collection["MA_CHUCNANG"].ToString();
                myobj.TEN_CHUCNANG = collection["TEN_CHUCNANG"].ToString();
                myobj.URL = collection["URL"].ToString();
                myobj.ICONPATH = collection["ICONPATH"].ToString();
                myobj.CSSCLASS = collection["CSSCLASS"].ToString();
                DM_CHUCNANGBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            DM_CHUCNANGBusiness.repository.Delete(id);
            DM_CHUCNANGBusiness.Save();
            return Json(result);
        }

    }
}

