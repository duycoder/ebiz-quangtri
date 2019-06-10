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
using Business.CommonModel.DMNHOMDANHMUC;
using Web.Areas.DMNHOMDANHMUCArea.Models;


namespace Web.Areas.DMNHOMDANHMUCArea.Controllers
{
    public class DMNHOMDANHMUCController : BaseController
    {
        #region khaibao
        DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;
        #endregion
        public ActionResult Index()
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var searchmodel = new DM_NHOMDANHMUC_SEARCHBO();
            SessionManager.SetValue("dmnhomdanhmucSearchModel", null);
            var data = DM_NHOMDANHMUCBusiness.GetDaTaByPage(null);
            return View(data);
        }

        [HttpPost]
        public JsonResult CheckCode(int id, string code)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            return Json(DM_NHOMDANHMUCBusiness.checkExistCode(code, id));
        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var searchModel = SessionManager.GetValue("dmnhomdanhmucSearchModel") as DM_NHOMDANHMUC_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new DM_NHOMDANHMUC_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("dmnhomdanhmucSearchModel", searchModel);
            }
            var data = DM_NHOMDANHMUCBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        public PartialViewResult Create()
        {
            return PartialView("_CreatePartial");
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new DM_NHOMDANHMUC();
                myobj.TYPE = collection["TYPE"].ToIntOrZero();
                myobj.GROUP_CODE = collection["GROUP_CODE"].ToString();
                myobj.GROUP_NAME = collection["GROUP_NAME"].ToString();
                DM_NHOMDANHMUCBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không thêm mới được";
            }
            return Json(result);
        }

        public PartialViewResult Edit(long id)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var myModel = new EditVM();
            myModel.objModel = DM_NHOMDANHMUCBusiness.repository.Find(id);
            return PartialView("_EditPartial", myModel);
        }

        public PartialViewResult Detail(int id)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var myModel = DM_NHOMDANHMUCBusiness.GetDaTaByID(id);
            return PartialView("_DetailPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = DM_NHOMDANHMUCBusiness.Find(id);
                myobj.TYPE = collection["TYPE"].ToIntOrZero();
                myobj.GROUP_CODE = collection["GROUP_CODE"].ToString();
                myobj.GROUP_NAME = collection["GROUP_NAME"].ToString();
                DM_NHOMDANHMUCBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var result = new JsonResultBO(true);
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            DM_NHOMDANHMUCBusiness.repository.Delete(id);
            DM_NHOMDANHMUCBusiness.Save();
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var searchModel = SessionManager.GetValue("dmnhomdanhmucSearchModel") as DM_NHOMDANHMUC_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new DM_NHOMDANHMUC_SEARCHBO();
                searchModel.pageSize = 20;
            }
            

            searchModel.QR_CODE = form["QR_CODE"];
            searchModel.QR_NAME = form["QR_NAME"];
            SessionManager.SetValue("dmnhomdanhmucSearchModel", searchModel);

            var data = DM_NHOMDANHMUCBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }
    }
}

