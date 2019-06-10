using Business.Business;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.CongViecArea.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.QuanLyCongViec.Controllers
{
    public class CongViecDuocGiaoController : BaseController
    {
        // GET: QuanLyCongViec/CongViecDuocGiao
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private HSCV_CONGVIECBusiness HSCV_CONGVIECBusiness;
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string CongViecExtension = WebConfigurationManager.AppSettings["CongViecExtension"];
        private int CongViecSize = int.Parse(WebConfigurationManager.AppSettings["CongViecSize"]);
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        public ActionResult Index()
        {
            CongViecIndexViewModel model = new CongViecIndexViewModel();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC_SEARCH searchModel = new HSCV_CONGVIEC_SEARCH();
            AssignUserInfo();
            searchModel.USER_ID = currentUser.ID;
            searchModel.LOAI_CONGVIEC = LOAI_CONGVIEC.DUOCGIAO;
            searchModel.pageSize = MaxPerpage;
            SessionManager.SetValue("CongViecDGSearchModel", searchModel);
            var ListCongViec = HSCV_CONGVIECBusiness.GetDaTaByPage(searchModel, MaxPerpage);
            model.ListResult = ListCongViec;
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, 0);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.UserInfo = currentUser;
            return View(model);
        }

        #region Các hàm jsonresult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var searchModel = SessionManager.GetValue("CongViecDGSearchModel") as HSCV_CONGVIEC_SEARCH;
            if (searchModel == null)
            {
                searchModel = new HSCV_CONGVIEC_SEARCH();
                searchModel.pageSize = MaxPerpage;
            }
            string TENCONGVIEC = form["TENCONGVIEC"];
            string NGAYBATDAU_FROM = form["NGAYBATDAU_FROM"];
            string NGAYBATDAU_TO = form["NGAYBATDAU_TO"];
            string NGAYKETTHUC_FROM = form["NGAYKETTHUC_FROM"];
            string NGAYKETTHUC_TO = form["NGAYKETTHUC_TO"];
            string DOKHAN_ID = form["DOKHAN_ID"];
            string DOMAT_ID = form["DOMAT_ID"];
            #region Gán giá trị
            if (!string.IsNullOrEmpty(TENCONGVIEC))
            {
                searchModel.TENCONGVIEC = TENCONGVIEC.Trim();
            }
            searchModel.NGAYBATDAU_FROM = NGAYBATDAU_FROM.ToDateTime();
            searchModel.NGAYBATDAU_TO = NGAYBATDAU_TO.ToDateTime();
            searchModel.NGAYKETTHUC_FROM = NGAYKETTHUC_FROM.ToDateTime();
            searchModel.NGAYKETTHUC_TO = NGAYKETTHUC_TO.ToDateTime();
            if (!string.IsNullOrEmpty(DOKHAN_ID))
            {
                searchModel.DOKHAN = DOKHAN_ID.ToLongOrNULL();
            }
            if (!string.IsNullOrEmpty(DOMAT_ID))
            {
                searchModel.DO_UUTIEN = DOMAT_ID.ToLongOrNULL();
            }
            #endregion
            SessionManager.SetValue("CongViecDGSearchModel", searchModel);
            var data = HSCV_CONGVIECBusiness.GetDaTaByPage(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var searchModel = SessionManager.GetValue("CongViecDGSearchModel") as HSCV_CONGVIEC_SEARCH;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new HSCV_CONGVIEC_SEARCH();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("CongViecSearchModel", searchModel);
            }
            var data = HSCV_CONGVIECBusiness.GetDaTaByPage(searchModel, pageSize, indexPage);
            return Json(data);
        }
        #endregion
    }
}