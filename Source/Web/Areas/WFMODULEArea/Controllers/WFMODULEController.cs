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
using Business.CommonModel.WFMODULE;
using Web.Areas.WFMODULEArea.Models;
using Business.CommonModel.CONSTANT;


namespace Web.Areas.WFMODULEArea.Controllers
{
    public class WFMODULEController : BaseController
    {
        #region khaibao
        WF_MODULEBusiness WF_MODULEBusiness;
        #endregion
        public ActionResult Index()
        {
            var model = new IndexVM();
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var searchmodel = new WF_MODULE_SEARCHBO();
            SessionManager.SetValue("wfmoduleSearchModel", null);
            model.LstModule = WF_MODULEBusiness.GetDaTaByPage(null);
            foreach (var item in model.LstModule.ListItem)
            {
                var LstIds = item.WF_STREAM_ID.ToListInt(',');
                var LstStreams = WF_STREAMBusiness.repository.All().Where(x => LstIds.Contains(x.ID)).ToList();
                foreach (var subitem in LstStreams)
                {
                    item.TenLuong += subitem.WF_NAME + ",";
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var searchModel = SessionManager.GetValue("wfmoduleSearchModel") as WF_MODULE_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new WF_MODULE_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("wfmoduleSearchModel", searchModel);
            }
            var data = WF_MODULEBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        public PartialViewResult Create()
        {
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var model = new CreateVM();
            model.DsModule = MODULE_CONSTANT.GetList();
            model.DsLuongXuLy = WF_STREAMBusiness.DsLuong();
            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            AssignUserInfo();
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new WF_MODULE();
                myobj.WF_STREAM_ID = collection["WF_STREAM_ID"];
                myobj.create_at = DateTime.Now;
                myobj.create_by = currentUser.ID;
                myobj.MODULE_CODE = collection["MODULE_CODE"].ToString();
                myobj.MODULE_TITLE = collection["MODULE_TITLE"].ToString();
                if (WF_MODULEBusiness.ExistCode(myobj.MODULE_CODE))
                {
                    result.Message = "Module này đã tồn tài";
                    result.Status = false;
                }
                else
                {
                    WF_MODULEBusiness.Save(myobj);
                }

            }
            catch
            {
                result.Status = false;
                result.Message = "Không thêm mới được";
            }
            return Json(result);
        }

        public PartialViewResult Edit(int id)
        {
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var myModel = new EditVM();
            myModel.objModel = WF_MODULEBusiness.repository.Find(id);
            var LstLuongId = myModel.objModel.WF_STREAM_ID.ToListInt(',');
            //myModel.DsLuongXuLy = WF_STREAMBusiness.DsLuong(myModel.objModel.WF_STREAM_ID.GetValueOrDefault(0));
            myModel.DsLuongXuLy = WF_STREAMBusiness.DsLuongMultipe(LstLuongId);
            return PartialView("_EditPartial", myModel);
        }

        public PartialViewResult Detail(int id)
        {
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var myModel = WF_MODULEBusiness.GetDaTaByID(id);
            return PartialView("_DetailPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            AssignUserInfo();
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = WF_MODULEBusiness.Find(id);
                myobj.WF_STREAM_ID = collection["WF_STREAM_ID"];
                myobj.edit_at = DateTime.Now;
                myobj.edit_by = currentUser.ID;
                myobj.MODULE_TITLE = collection["MODULE_TITLE"].ToString();
                WF_MODULEBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var searchModel = SessionManager.GetValue("wfmoduleSearchModel") as WF_MODULE_SEARCHBO;
            if (searchModel == null)
            {
                searchModel = new WF_MODULE_SEARCHBO();
                searchModel.pageSize = 20;
            }
            searchModel.QR_ID = form["QR_ID"].ToIntOrZero();
            //searchModel.QR_WF_STREAM_ID = form["QR_WF_STREAM_ID"].ToIntOrZero();
            var idToaNha = form["TOANHA_ID"].ToIntOrZero();
            searchModel.QR_create_by = form["QR_create_by"].ToLongOrZero();
            searchModel.QR_edit_by = form["QR_edit_by"].ToLongOrZero();
            searchModel.QR_MODULE_CODE = form["QR_MODULE_CODE"].ToString();
            searchModel.QR_MODULE_TITLE = form["QR_MODULE_TITLE"].ToString();
            SessionManager.SetValue("wfmoduleSearchModel", searchModel);
            var data = WF_MODULEBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);

        }
        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            WF_MODULEBusiness.repository.Delete(id);
            WF_MODULEBusiness.Save();
            return Json(result);
        }

    }
}

