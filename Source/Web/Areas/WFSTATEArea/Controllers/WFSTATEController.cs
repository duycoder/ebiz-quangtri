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
using Business.CommonModel.WFSTATE;
using Web.Areas.WFSTATEArea.Models;
using Business.CommonModel.CONSTANT;


namespace Web.Areas.WFSTATEArea.Controllers
{
    public class WFSTATEController : BaseController
    {
        #region khaibao
        WF_STATEBusiness WF_STATEBusiness;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id luồng xử lý</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var model = new IndexVM();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var searchmodel = new WF_STATE_SEARCHBO();
            SessionManager.SetValue("wfstateSearchModel", null);
            model.LuongXuLy = WF_STREAMBusiness.GetDaTaByID(id);
            model.LstState = WF_STATEBusiness.GetDaTaByPage(id, null);
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveFunction(WF_STATE_FUNCTION state)
        {
            AssignUserInfo();
            var result = new JsonResultBO(true);
            var WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
            try
            {
                if (state.ID == 0)
                {
                    if (state.ACTION != null)
                    {
                        state.create_at = DateTime.Now;
                        state.create_by = currentUser.ID;
                        WF_STATE_FUNCTIONBusiness.Save(state);
                    }

                }
                else
                {

                    var updatestate = WF_STATE_FUNCTIONBusiness.Find(state.ID);
                    updatestate.ACTION = state.ACTION;
                    updatestate.IS_BREAK = state.IS_BREAK;
                    updatestate.edit_at = DateTime.Now;
                    updatestate.edit_by = currentUser.ID;
                    WF_STATE_FUNCTIONBusiness.Save(updatestate);


                }

            }
            catch (Exception)
            {
                result.MessageFail("Không cập nhật được");
            }
            return Json(result);
        }
        public PartialViewResult FunctionState(int idStream, int state)
        {
            var model = new FunctionStateBO();
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
            var WF_FUNCTIONBusiness = Get<WF_FUNCTIONBusiness>();
            model.State = WF_STATEBusiness.Find(state);
            model.StateFunction = WF_STATE_FUNCTIONBusiness.GetStateFunction(state);
            model.DsFunction = WF_FUNCTIONBusiness.GetDsFunction((model.StateFunction != null ? model.StateFunction.ACTION ?? 0 : 0));
            model.DsFunction.Add(new SelectListItem()
            {
                Text = "Chọn hành động",
                Value = "",
                Selected = model.StateFunction == null || model.StateFunction.ACTION == null

            });
            return PartialView("FunctionState", model);
        }

        [HttpPost]
        public JsonResult getData(int idStream, int indexPage, string sortQuery, int pageSize)
        {
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var searchModel = SessionManager.GetValue("wfstateSearchModel") as WF_STATE_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new WF_STATE_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("wfstateSearchModel", searchModel);
            }
            var data = WF_STATEBusiness.GetDaTaByPage(idStream, searchModel, indexPage, pageSize);
            return Json(data);
        }
        /// <summary>
        /// Tạo mới trạng thái trong luồng xử lý
        /// </summary>
        /// <param name="id">id luồng xử lý</param>
        /// <returns></returns>
        public PartialViewResult Create(int id)
        {
            var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var model = new CreateVM();
            model.DsVaiTro = DM_VAITROBusiness.DsVaiTro(null);
            model.DsChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull(DMLOAI_CONSTANT.CHUCVU);
            model.LuongXuLy = WF_STREAMBusiness.Find(id);

            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            AssignUserInfo();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new WF_STATE();
                myobj.WF_ID = collection["WF_ID"].ToIntOrZero();
                myobj.create_at = DateTime.Now;
                myobj.IS_KETTHUC = collection["IS_KETTHUC"].ToBoolByOnOff();
                myobj.IS_START = collection["IS_START"].ToBoolByOnOff();
                if (myobj.IS_START == true)
                {
                    WF_STATEBusiness.ClearBatDau(myobj.WF_ID.Value);
                    myobj.CHUCVU_ID = collection["CHUCVU_ID"].ToIntOrNULL();
                    myobj.VAITRO_ID = collection["VAITRO_ID"].ToIntOrNULL();
                }

                myobj.create_by = currentUser.ID;
                myobj.STATE_NAME = collection["STATE_NAME"].ToString();
                myobj.GHICHU = collection["GHICHU"].ToString();
                if (myobj.IS_KETTHUC == true)
                {
                    WF_STATEBusiness.ClearKetThuc(myobj.WF_ID.Value);
                }
                WF_STATEBusiness.Save(myobj);
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
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var myModel = new EditVM();
            myModel.objModel = WF_STATEBusiness.repository.Find(id);
            var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            List<int> listVaiTro = null;
            if (myModel.objModel.VAITRO_ID != null)
            {
                listVaiTro = new List<int>();
                listVaiTro.Add(myModel.objModel.VAITRO_ID.GetValueOrDefault());
            }
            myModel.DsVaiTro = DM_VAITROBusiness.DsVaiTro(listVaiTro);
            myModel.DsChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull(DMLOAI_CONSTANT.CHUCVU, myModel.objModel.CHUCVU_ID.GetValueOrDefault());
            return PartialView("_EditPartial", myModel);
        }

        public PartialViewResult Detail(int id)
        {
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var myModel = WF_STATEBusiness.GetDaTaByID(id);
            return PartialView("_DetailPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            AssignUserInfo();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = WF_STATEBusiness.Find(id);
                myobj.edit_at = DateTime.Now;
                myobj.IS_KETTHUC = collection["IS_KETTHUC"].ToBoolByOnOff();
                myobj.IS_START = collection["IS_START"].ToBoolByOnOff();
                if (myobj.IS_START == true)
                {
                    WF_STATEBusiness.ClearBatDau(myobj.WF_ID.Value);
                    myobj.CHUCVU_ID = collection["CHUCVU_ID"].ToIntOrNULL();
                    myobj.VAITRO_ID = collection["VAITRO_ID"].ToIntOrNULL();
                }
                else
                {
                    myobj.CHUCVU_ID = null;
                    myobj.VAITRO_ID = null;
                }
                myobj.edit_by = currentUser.ID;
                myobj.STATE_NAME = collection["STATE_NAME"].ToString();
                myobj.GHICHU = collection["GHICHU"].ToString();
                if (myobj.IS_KETTHUC == true)
                {
                    WF_STATEBusiness.ClearKetThuc(myobj.WF_ID.Value);
                }
                WF_STATEBusiness.Save(myobj);
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
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            var searchModel = SessionManager.GetValue("wfstateSearchModel") as WF_STATE_SEARCHBO;
            if (searchModel == null)
            {
                searchModel = new WF_STATE_SEARCHBO();
                searchModel.pageSize = 20;
            }
            var idStream = form["QR_WF_ID"].ToIntOrZero();
            searchModel.QR_STATE_NAME = form["QR_STATE_NAME"].ToString();
            searchModel.QR_GHICHU = form["QR_GHICHU"].ToString();
            SessionManager.SetValue("wfstateSearchModel", searchModel);
            var data = WF_STATEBusiness.GetDaTaByPage(idStream, searchModel, 1, searchModel.pageSize);
            return Json(data);

        }
        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            WF_STATEBusiness.repository.Delete(id);
            WF_STATEBusiness.Save();
            return Json(result);
        }

    }
}

