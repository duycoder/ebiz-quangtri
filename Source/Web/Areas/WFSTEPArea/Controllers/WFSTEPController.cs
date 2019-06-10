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
using Business.CommonModel.WFSTEP;
using Web.Areas.WFSTEPArea.Models;
using CommonHelper.Upload;
using System.Web.Configuration;
using System.IO;
using Business.CommonModel.CONSTANT;


namespace Web.Areas.WFSTEPArea.Controllers
{
    public class WFSTEPController : BaseController
    {
        #region khaibao
        WF_STEPBusiness WF_STEPBusiness;
        private string UPLOADFOLDER = WebConfigurationManager.AppSettings["UPLOADFOLDER"];
        private string HostUpload = WebConfigurationManager.AppSettings["HostUpload"];
        #endregion
        /// <summary>
        /// Danh sách bước chuyển trạng thái theo luồng xử lý
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var model = new IndexVM();
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var searchmodel = new WF_STEP_SEARCHBO();
            SessionManager.SetValue("wfstepSearchModel", null);
            model.LuongXuLy = WF_STREAMBusiness.Find(id);
            model.LstStep = WF_STEPBusiness.GetDaTaByPage(id, null);

            foreach (var item in model.LstStep.ListItem)
            {
                if (!string.IsNullOrEmpty(item.ICON))
                {
                    item.ICON = Path.Combine(HostUpload, item.ICON);
                }

            }
            model.GoData = WF_STEPBusiness.GetDataByStream(id);
            return View(model);
        }
        [HttpGet]
        public JsonResult GetChart(int id)
        {
            var WF_STEPBusiness = Get<WF_STEPBusiness>();
            var data = WF_STEPBusiness.GetDataByStream(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult SaveLocation(GoModel data)
        {
            var result = new JsonResultBO(true);
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            try
            {
                if (data.nodeDataArray != null && data.nodeDataArray.Any())
                {
                    foreach (var item in data.nodeDataArray)
                    {
                        var state = WF_STATEBusiness.Find(item.id);
                        state.LOCATION = item.loc;
                        WF_STATEBusiness.Save(state);
                    }
                }
            }
            catch
            {

                result.Status = false;
                result.Message = "Không cập nhật được dữ liệu";
            }
            return Json(result);

        }

        [HttpPost]
        public JsonResult getData(int idStream, int indexPage, string sortQuery, int pageSize)
        {
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var searchModel = SessionManager.GetValue("wfstepSearchModel") as WF_STEP_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new WF_STEP_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("wfstepSearchModel", searchModel);
            }
            var data = WF_STEPBusiness.GetDaTaByPage(idStream, searchModel, indexPage, pageSize);
            foreach (var item in data.ListItem)
            {
                if (!string.IsNullOrEmpty(item.ICON))
                {
                    item.ICON = Path.Combine(HostUpload, item.ICON);
                }
            }
            return Json(data);
        }

        public PartialViewResult ConfigStep(int id)
        {
            var model = new ConfigVM();
            var WF_STEPBusiness = Get<WF_STEPBusiness>();
            var CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var DMLoaiDonViBusiness = Get<DMLoaiDonViBusiness>();
            var WF_STEP_CONFIGBusiness = Get<WF_STEP_CONFIGBusiness>();
            var WF_STEP_USER_PROCESSBusiness = Get<WF_STEP_USER_PROCESSBusiness>();
            model.Step = WF_STEPBusiness.GetDaTaByID(id);
            model.ConfigStep = WF_STEP_CONFIGBusiness.GetConfigStep(id);
            model.MainProcess = WF_STEP_USER_PROCESSBusiness.GetMainProcess(id);
            model.JoinProcess = WF_STEP_USER_PROCESSBusiness.GetJoinProcess(id);
            model.DSPhongBan = CCTC_THANHPHANBusiness.GetDropDownList();
            model.DsVaiTro = DM_VAITROBusiness.DsVaiTro(null);
            model.DSChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull(DMLOAI_CONSTANT.CHUCVU);
            model.DSCap = DMLoaiDonViBusiness.DSLoaiDonVi();
            return PartialView("_ConfigStepPartial", model);
        }
        [HttpPost]
        public JsonResult SaveConfig(WF_STEP_CONFIG config, bool isBack, bool isMain, WF_STEP_USER_PROCESS mainProcess, bool isJoin, WF_STEP_USER_PROCESS joinProcess)
        {
            var result = new JsonResultBO(true);
            AssignUserInfo();
            var WF_STEP_CONFIGBusiness = Get<WF_STEP_CONFIGBusiness>();
            var WF_STEP_USER_PROCESSBusiness = Get<WF_STEP_USER_PROCESSBusiness>();
            try
            {
                if (config.ID == 0)
                {
                    config.create_at = DateTime.Now;
                    config.create_by = currentUser.ID;
                }
                else
                {
                    config.edit_at = DateTime.Now;
                    config.edit_by = currentUser.ID;

                }

                config.IS_BACK_USER = isBack;

                WF_STEP_CONFIGBusiness.Save(config);
                if (isMain)
                {
                    if (mainProcess.ID == 0)
                    {
                        mainProcess.create_at = DateTime.Now;
                        mainProcess.create_by = currentUser.ID;
                    }
                    else
                    {
                        mainProcess.edit_at = DateTime.Now;
                        mainProcess.edit_by = currentUser.ID;

                    }
                    WF_STEP_USER_PROCESSBusiness.Save(mainProcess);
                }
                else
                {
                    WF_STEP_USER_PROCESSBusiness.DeleteProcess(config.WF_STEP_ID.GetValueOrDefault(0), true);
                }


                if (isJoin)
                {
                    if (joinProcess.ID == 0)
                    {
                        joinProcess.create_at = DateTime.Now;
                        joinProcess.create_by = currentUser.ID;
                    }
                    else
                    {
                        joinProcess.edit_at = DateTime.Now;
                        joinProcess.edit_by = currentUser.ID;

                    }
                    WF_STEP_USER_PROCESSBusiness.Save(joinProcess);
                }
                else
                {
                    WF_STEP_USER_PROCESSBusiness.DeleteProcess(config.WF_STEP_ID.GetValueOrDefault(0), false);
                }
            }
            catch
            {

                result.Status = false;
                result.Message = "Không cập nhật được";
            }


            return Json(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id luong xử lý</param>
        /// <returns></returns>
        public PartialViewResult Create(int id)
        {
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var model = new CreateVM();
            model.LuongXuLy = WF_STREAMBusiness.Find(id);
            model.DsTrangThai = WF_STATEBusiness.GetDsTrangThai(id);
            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection, HttpPostedFileBase ICON)
        {
            AssignUserInfo();
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new WF_STEP();
                myobj.WF_ID = collection["WF_ID"].ToIntOrZero();
                myobj.STATE_BEGIN = collection["STATE_BEGIN"].ToIntOrZero();
                myobj.STATE_END = collection["STATE_END"].ToIntOrZero();
                myobj.create_at = DateTime.Now;
                myobj.IS_RETURN = collection["IS_RETURN"].ToBoolByOnOff();
                myobj.create_by = currentUser.ID;
                myobj.NAME = collection["NAME"].ToString();
                myobj.GHICHU = collection["GHICHU"].ToString();
                myobj.REQUIRED_REVIEW = collection["REQUIRED_REVIEW"].ToBoolByOnOff();
                if (ICON != null)
                {
                    var resultUpload = UploadProvider.SaveFile(ICON, null, ".jpg,.png,.jpeg", null, "IconWF", UPLOADFOLDER);
                    if (resultUpload.status)
                    {
                        myobj.ICON = resultUpload.path;
                    }
                    else
                    {
                        result.Message = resultUpload.message;
                    }
                }

                WF_STEPBusiness.Save(myobj);
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
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var myModel = new EditVM();
            myModel.objModel = WF_STEPBusiness.repository.Find(id);
            myModel.dsTrangThaiStart = WF_STATEBusiness.GetDsTrangThai(myModel.objModel.WF_ID.GetValueOrDefault(), myModel.objModel.STATE_BEGIN.GetValueOrDefault());
            myModel.dsTrangThaiEnd = WF_STATEBusiness.GetDsTrangThai(myModel.objModel.WF_ID.GetValueOrDefault(), myModel.objModel.STATE_END.GetValueOrDefault());
            return PartialView("_EditPartial", myModel);
        }

        public PartialViewResult Detail(int id)
        {
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var myModel = WF_STEPBusiness.GetDaTaByID(id);
            return PartialView("_DetailPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection, HttpPostedFileBase ICON)
        {
            AssignUserInfo();
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = WF_STEPBusiness.Find(id);
                myobj.STATE_BEGIN = collection["STATE_BEGIN"].ToIntOrZero();
                myobj.STATE_END = collection["STATE_END"].ToIntOrZero();
                myobj.create_at = DateTime.Now;
                myobj.IS_RETURN = collection["IS_RETURN"].ToBoolByOnOff();
                myobj.create_by = currentUser.ID;
                myobj.NAME = collection["NAME"].ToString();
                myobj.GHICHU = collection["GHICHU"].ToString();
                myobj.REQUIRED_REVIEW = collection["REQUIRED_REVIEW"].ToBoolByOnOff();
                if (ICON != null)
                {
                    var resultUpload = UploadProvider.SaveFile(ICON, null, ".jpg,.png,.jpeg", null, "IconWF", UPLOADFOLDER);
                    if (resultUpload.status)
                    {
                        myobj.ICON = resultUpload.path;
                    }
                    else
                    {
                        result.Message = resultUpload.message;
                    }
                }
                WF_STEPBusiness.Save(myobj);
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
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            var searchModel = SessionManager.GetValue("wfstepSearchModel") as WF_STEP_SEARCHBO;
            if (searchModel == null)
            {
                searchModel = new WF_STEP_SEARCHBO();
                searchModel.pageSize = 20;
            }
            var idStream = form["QR_WF_ID"].ToIntOrZero();
            searchModel.QR_STATE_BEGIN = form["QR_STATE_BEGIN"].ToIntOrZero();
            searchModel.QR_STATE_END = form["QR_STATE_END"].ToIntOrZero();

            searchModel.QR_IS_RETURN = form["QR_IS_RETURN"].ToBoolByOnOff();

            searchModel.QR_NAME = form["QR_NAME"].ToString();
            searchModel.QR_GHICHU = form["QR_GHICHU"].ToString();
            SessionManager.SetValue("wfstepSearchModel", searchModel);
            var data = WF_STEPBusiness.GetDaTaByPage(idStream, searchModel, 1, searchModel.pageSize);
            return Json(data);

        }
        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            WF_STEPBusiness = Get<WF_STEPBusiness>();
            WF_STEPBusiness.repository.Delete(id);
            WF_STEPBusiness.Save();
            return Json(result);
        }

    }
}

