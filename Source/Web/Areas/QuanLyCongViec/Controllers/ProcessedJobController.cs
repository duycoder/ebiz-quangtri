using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.CongViecArea.Models;
using Web.Common;
using Web.Common.Elastic;
using Web.Custom;
using Web.FwCore;
using Web.Models;

namespace Web.Areas.QuanLyCongViec.Controllers
{
    public class ProcessedJobController : BaseController
    {
        // GET: QuanLyCongViec/ProcessedJob
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private HSCV_CONGVIECBusiness HSCV_CONGVIECBusiness;
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string CongViecExtension = WebConfigurationManager.AppSettings["CongViecExtension"];
        private int CongViecSize = int.Parse(WebConfigurationManager.AppSettings["CongViecSize"]);
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        //private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        public ActionResult Index()
        {
            CongViecIndexViewModel model = new CongViecIndexViewModel();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC_SEARCH searchModel = new HSCV_CONGVIEC_SEARCH();
            AssignUserInfo();
            searchModel.USER_ID = currentUser.ID;
            searchModel.pageSize = MaxPerpage;
            SessionManager.SetValue("ProcessedJobSearchModel", searchModel);
            var ListCongViec = HSCV_CONGVIECBusiness.GetListProcessedJob(searchModel, MaxPerpage);
            model.ListResult = ListCongViec;
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, 0);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.UserInfo = currentUser;
            SessionManager.SetValue("ProcessedJobSearch", ListCongViec.ListItem);
            return View(model);
        }
        #region Các hàm jsonresult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var searchModel = SessionManager.GetValue("ProcessedJobSearchModel") as HSCV_CONGVIEC_SEARCH;
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
            SessionManager.SetValue("ProcessedJobSearchModel", searchModel);
            var data = HSCV_CONGVIECBusiness.GetListProcessedJob(searchModel, searchModel.pageSize, 1);
            SessionManager.SetValue("ProcessedJobSearch", data.ListItem);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var searchModel = SessionManager.GetValue("ProcessedJobSearchModel") as HSCV_CONGVIEC_SEARCH;
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
                SessionManager.SetValue("ProcessedJobSearchModel", searchModel);
            }
            var data = HSCV_CONGVIECBusiness.GetListProcessedJob(searchModel, pageSize, indexPage);
            SessionManager.SetValue("ProcessedJobSearch", data.ListItem);
            return Json(data);
        }
        public JsonResult Approval(string id)
        {
            List<long> Ids = id.ToListLong(',');
            List<CongViecBO> ListCongViec = SessionManager.GetValue("ProcessedJobSearch") as List<CongViecBO>;
            Ids = Ids.Where(x => ListCongViec.Select(y => y.ID).Contains(x)).ToList();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            foreach (var item in Ids)
            {
                HSCV_CONGVIEC CongViec = HSCV_CONGVIECBusiness.Find(item);
                if (CongViec != null)
                {
                    CongViec.NGAYDUYET = DateTime.Now;
                    CongViec.TRANGTHAI_ID = TrangThaiCongViecConstant.APPROVED;
                    HSCV_CONGVIECBusiness.Save(CongViec);
                }
            }
            return Json(new { Type = "SUCCESS", Message = "Phê duyệt công việc đã giao thành công" });
        }
        public JsonResult UnApproval(string id, string COMMENT)
        {
            List<long> Ids = id.ToListLong(',');
            List<CongViecBO> ListCongViec = SessionManager.GetValue("ProcessedJobSearch") as List<CongViecBO>;
            Ids = Ids.Where(x => ListCongViec.Select(y => y.ID).Contains(x)).ToList();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            foreach (var item in Ids)
            {
                HSCV_CONGVIEC CongViec = HSCV_CONGVIECBusiness.Find(item);
                if (CongViec != null)
                {
                    CongViec.NGAYDUYET = DateTime.Now;
                    CongViec.NGUOIGIAOVIEC_ID = null;
                    CongViec.NGUOIGIAOVIEC_PHANHOI = COMMENT;
                    CongViec.NGUOIGIAOVIECDAPHANHOI = true;
                    CongViec.TRANGTHAI_ID = TrangThaiCongViecConstant.UNAPPROVAL;
                    HSCV_CONGVIECBusiness.Save(CongViec);
                }
            }
            return Json(new { Type = "SUCCESS", Message = "Không phê duyệt công việc đã giao thành công" });
        }
        #endregion
        public ActionResult Update(long id)
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            CongViecViewModel model = new CongViecViewModel();
            model.UserInfo = currentUser;
            var CongViec = HSCV_CONGVIECBusiness.Find(id);
            if (CongViec == null || currentUser.ID != CongViec.NGUOIGIAOVIEC_ID)
            {
                CongViec = new HSCV_CONGVIEC();
            }
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOKHAN, currentUser.ID, CongViec.DOKHAN.HasValue ? CongViec.DOKHAN.Value : 0);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID, CongViec.DOUU_TIEN.HasValue ? CongViec.DOUU_TIEN.Value : 0);
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            model.ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.CONGVIEC);
            model.CongViec = CongViec;
            model.userInfo = currentUser;
            return View(model);
        }
        #region Lưu lại công việc đã giao
        [ValidateInput(false)]
        public JsonResult SaveCongViec(HSCV_CONGVIEC CongViec, FormCollection col, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            AssignUserInfo();
            #region gán dữ liệu
            string NGAYBATDAU = col["NGAYBATDAU"];
            string NGAYKETTHUC = col["NGAYKETTHUC"];
            string MUCTIEU_CONGVIEC = col["MUCTIEU_CONGVIEC"];
            string CACBUOC_THUCHIEN = col["CACBUOC_THUCHIEN"];
            if (!string.IsNullOrEmpty(NGAYBATDAU))
            {
                NGAYBATDAU = NGAYBATDAU.Trim();
            }
            if (!string.IsNullOrEmpty(NGAYKETTHUC))
            {
                NGAYKETTHUC = NGAYKETTHUC.Trim();
            }
            if (!string.IsNullOrEmpty(col["NOIDUNGCONGVIEC"]))
            {
                CongViec.NOIDUNGCONGVIEC = col["NOIDUNGCONGVIEC"].Trim();
            }
            if (!string.IsNullOrEmpty(col["TENCONGVIEC"]))
            {
                CongViec.TENCONGVIEC = col["TENCONGVIEC"].Trim();
            }
            if (!string.IsNullOrEmpty(MUCTIEU_CONGVIEC))
            {
                CongViec.MUCTIEU_CONGVIEC = MUCTIEU_CONGVIEC.Trim();
            }
            if (!string.IsNullOrEmpty(CACBUOC_THUCHIEN))
            {
                CongViec.CACBUOC_THUCHIEN = CACBUOC_THUCHIEN.Trim();
            }
            CongViec.NGAY_NHANVIEC = NGAYBATDAU.ToDateTime();
            CongViec.NGAYHOANTHANH_THEOMONGMUON = NGAYKETTHUC.ToDateTime();
            CongViec.IS_BATDAU = false;
            #endregion
            List<CommonError> ListError = IsValid(CongViec, NGAYBATDAU, NGAYKETTHUC);
            if (ListError.Any())
            {
                return Json(new { Type = "INVALID", Message = ListError }, JsonRequestBehavior.AllowGet);
            }
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            UploadFileTool tool = new UploadFileTool();
            if (CongViec.ID > 0)
            {
                #region Cập nhật công việc cá nhân
                var result = HSCV_CONGVIECBusiness.Find(CongViec.ID);
                if (result == null || currentUser.ID != result.NGUOIGIAOVIEC_ID)
                {
                    return Json(new { Type = "ERROR", Message = "Không tìm thấy công việc cần cập nhật" }, JsonRequestBehavior.AllowGet);
                }
                result.DOKHAN = CongViec.DOKHAN;
                result.DOUU_TIEN = CongViec.DOUU_TIEN;
                result.HAS_FILE = filebase.Any() ? 1 : 0;
                result.HAS_NHACVIECDENHAN = CongViec.HAS_NHACVIECDENHAN;
                result.IS_EMAIL = CongViec.IS_EMAIL;
                result.IS_HASPLAN = CongViec.IS_HASPLAN;
                result.IS_MESG = CongViec.IS_MESG;
                result.IS_POPUP = CongViec.IS_POPUP;
                result.IS_SMS = CongViec.IS_SMS;
                result.IS_SUBTASK = CongViec.IS_SUBTASK;
                result.ITEMTYPE = CongViec.ITEMTYPE;
                result.ITEM_ID = CongViec.ITEM_ID;
                result.NGAY_NHANVIEC = CongViec.NGAY_NHANVIEC;
                result.NGAYHOANTHANH_THEOMONGMUON = CongViec.NGAYHOANTHANH_THEOMONGMUON;
                result.NGAYSUA = DateTime.Now;
                result.NGUOIGIAOVIECDANHGIA = CongViec.NGUOIGIAOVIECDANHGIA;
                result.NGUOIGIAOVIECDAPHANHOI = CongViec.NGUOIGIAOVIECDAPHANHOI;
                result.NOIDUNGCONGVIEC = CongViec.NOIDUNGCONGVIEC;
                result.PHANTRAMHOANTHANH = CongViec.PHANTRAMHOANTHANH;
                result.SONGAYNHACTRUOCHAN = CongViec.SONGAYNHACTRUOCHAN;
                result.SUBTASK_ID = CongViec.SUBTASK_ID;
                result.TENCONGVIEC = CongViec.TENCONGVIEC;
                result.TRANGTHAI_ID = CongViec.TRANGTHAI_ID;
                result.IS_BATDAU = CongViec.IS_BATDAU;
                result.CACBUOC_THUCHIEN = CongViec.CACBUOC_THUCHIEN;
                result.MUCTIEU_CONGVIEC = CongViec.MUCTIEU_CONGVIEC;
                HSCV_CONGVIECBusiness.Save(result);
                tool.UploadCustomFileVer3(filebase, true, CongViecExtension, URL_FOLDER, CongViecSize, FOLDER_ID, filename, result.ID, LOAITAILIEU.CONGVIEC, "Công việc", currentUser);
                #endregion
                #region Elastic search
                List<long> ListUser = new List<long>();
                ListUser.Add(currentUser.ID);
                ListUser.Add(result.NGUOITAO.HasValue ? result.NGUOITAO.Value : 0);
                ElasticModel model = ElasticModel.ConvertJob(result, ListUser, currentUser.HOTEN);
                ElasticSearch.updateDocument(model, model.Id.ToString(), ElasticType.CongViec);
                #endregion

                return Json(new { Type = "SUCCESS", Message = "Cập nhật công việc đã giao thành công" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Type = "ERROR", Message = "Không thể thực hiện thao tác này" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Các hàm private
        private List<CommonError> IsValid(HSCV_CONGVIEC CongViec, string NGAYBATDAU, string NGAYKETTHUC)
        {
            List<CommonError> ListError = new List<CommonError>();
            CommonError error;
            if (string.IsNullOrEmpty(CongViec.TENCONGVIEC))
            {
                error = new CommonError();
                error.Field = "TENCONGVIEC";
                error.Message = "Bạn chưa nhập tên công việc";
                ListError.Add(error);
            }
            if (string.IsNullOrEmpty(NGAYBATDAU))
            {
                error = new CommonError();
                error.Field = "NGAYBATDAU";
                error.Message = "Bạn chưa nhập ngày bắt đầu";
                ListError.Add(error);
            }
            else if (!CongViec.NGAY_NHANVIEC.HasValue)
            {
                error = new CommonError();
                error.Field = "NGAYBATDAU";
                error.Message = "Ngày bắt đầu không tồn tại hoặc không đúng định dạng";
                ListError.Add(error);
            }
            if (string.IsNullOrEmpty(NGAYKETTHUC))
            {
                error = new CommonError();
                error.Field = "NGAYKETTHUC";
                error.Message = "Bạn chưa nhập ngày kết thúc";
                ListError.Add(error);
            }
            else if (!CongViec.NGAYHOANTHANH_THEOMONGMUON.HasValue)
            {
                error = new CommonError();
                error.Field = "NGAYKETTHUC";
                error.Message = "Ngày kết thúc không tồn tại hoặc không đúng định dạng";
                ListError.Add(error);
            }
            return ListError;
        }
        #endregion
        public PartialViewResult PhanHoi()
        {
            return PartialView("_UnApproval");
        }
    }
}