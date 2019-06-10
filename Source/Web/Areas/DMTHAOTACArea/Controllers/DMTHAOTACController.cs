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
using Business.CommonModel.DMTHAOTAC;
using Web.Areas.DMTHAOTACArea.Models;
using Web.Filter;


namespace Web.Areas.DMTHAOTACArea.Controllers
{
    public class DMTHAOTACController : BaseController
    {
        #region khaibao
        DM_THAOTACBusiness DM_THAOTACBusiness;
        DM_CHUCNANGBusiness DM_CHUCNANGBusiness;
        #endregion
        /// <summary>
        /// Xem danh sách thao tác theo chức năng 
        /// </summary>
        /// <param name="id">id chức năng</param>
        /// <returns></returns>
        /// 
        [CodeAllowAccess(Code = "DsThaoTac")]
        public ActionResult Index(int id)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var searchmodel = new DM_THAOTAC_SEARCHBO();
            SessionManager.SetValue("dmthaotacSearchModel", null);
            var data = new IndexVM();
            data.ListThaoTac = DM_THAOTACBusiness.GetDaTaByPage(id, null);
            data.ChucNang = DM_CHUCNANGBusiness.Find(id);
            return View(data);

        }
        [HttpPost]
        public JsonResult CheckExsitCode(long idThaotac, string code)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            return Json(DM_THAOTACBusiness.checkExistCode(code, idThaotac));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            var searchModel = SessionManager.GetValue("dmthaotacSearchModel") as DM_THAOTAC_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new DM_THAOTAC_SEARCHBO();
                searchModel.pageSize = 20;
            }
            
            searchModel.QR_MA = form["QR_MA"];
            searchModel.QR_THAOTAC = form["QR_THAOTAC"];
            searchModel.QR_CHUCNANGID = form["QR_ID"].ToIntOrZero();
            SessionManager.SetValue("dmthaotacSearchModel", searchModel);

            var data = DM_THAOTACBusiness.GetDaTaByPage(searchModel.QR_CHUCNANGID, searchModel, 1, searchModel.pageSize);
            return Json(data);
        }


        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize, int idchucnang)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            var searchModel = SessionManager.GetValue("dmthaotacSearchModel") as DM_THAOTAC_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new DM_THAOTAC_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("dmthaotacSearchModel", searchModel);
            }
            var data = DM_THAOTACBusiness.GetDaTaByPage(idchucnang, searchModel, indexPage, pageSize);
            return Json(data);
        }
        /// <summary>
        /// Tạo thao tác của chức năng
        /// </summary>
        /// <param name="id">id là id chức năng</param>
        /// <returns></returns>
        public PartialViewResult Create(int id)
        {
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            var chucnang = DM_CHUCNANGBusiness.Find(id);
            return PartialView("_CreatePartial", chucnang);
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new DM_THAOTAC();
                myobj.TRANGTHAI = collection["TRANGTHAI"].ToIntOrZero();
                myobj.DM_CHUCNANG_ID = collection["DM_CHUCNANG_ID"].ToIntOrZero();
                myobj.TT_HIENTHI = collection["TT_HIENTHI"].ToIntOrZero();
                myobj.NGAYTAO = DateTime.Now;
                myobj.IS_HIENTHI = collection["IS_HIENTHI"].ToBoolByOnOff();
                myobj.MA_THAOTAC = collection["MA_THAOTAC"].ToString();
                myobj.TEN_THAOTAC = collection["TEN_THAOTAC"].ToString();
                myobj.MENU_LINK = collection["MENU_LINK"].ToString();
                myobj.ICONPATH = collection["ICONPATH"].ToString();
                myobj.CSSCLASS = collection["CSSCLASS"].ToString();
                DM_THAOTACBusiness.Save(myobj);
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
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            var myModel = new EditVM();
            myModel.objModel = DM_THAOTACBusiness.repository.Find(id);
            return PartialView("_EditPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = DM_THAOTACBusiness.Find(id);
                myobj.TRANGTHAI = collection["TRANGTHAI"].ToIntOrZero();
                myobj.TT_HIENTHI = collection["TT_HIENTHI"].ToIntOrZero();
                myobj.NGAYSUA = DateTime.Now;
                myobj.IS_HIENTHI = collection["IS_HIENTHI"].ToBoolByOnOff();
                myobj.MA_THAOTAC = collection["MA_THAOTAC"].ToString();
                myobj.TEN_THAOTAC = collection["TEN_THAOTAC"].ToString();
                myobj.MENU_LINK = collection["MENU_LINK"].ToString();
                myobj.ICONPATH = collection["ICONPATH"].ToString();
                myobj.CSSCLASS = collection["CSSCLASS"].ToString();
                DM_THAOTACBusiness.Save(myobj);
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
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            DM_THAOTACBusiness.repository.Delete(id);
            DM_THAOTACBusiness.Save();
            return Json(result);
        }

    }
}

