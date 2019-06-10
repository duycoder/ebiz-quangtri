using Business.Business;
using Business.CommonModel.TAILIEUTHUOCTINH;
using CommonHelper;
using Model.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.THUMUCLUUTRUArea.Controllers
{
    public class TAILIEUTHUOCTINHController : BaseController
    {
        // GET: THUMUCLUUTRUArea/TAILIEUTHUOCTINH
        private LOAITAILIEU_THUOCTINHBusiness LOAITAILIEU_THUOCTINHBusiness;
        private DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        public ActionResult Index()
        {
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            SessionManager.SetValue("thuoctinhSearch", null);
            var data = LOAITAILIEU_THUOCTINHBusiness.GetDaTaByPage(null);
            TaiLieuThuocTinhModel model = new TaiLieuThuocTinhModel();
            model.ListLoaiTaiLieu = initDanhMuc();
            model.ListResult = data;
            return View(model);
        }
        #region Các hàm private
        private List<DM_DANHMUC_DATA> initDanhMuc()
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            return DM_DANHMUC_DATABusiness.GetData(ThuMucLuuTruConstant.LoaiTaiLieu);
        }
        private bool CanAccess()
        {
            return true;
        }
        #endregion
        #region Các hàm partialview
        public PartialViewResult Create()
        {
            TaiLieuThuocTinhModel model = new TaiLieuThuocTinhModel();
            model.ThuocTinh = new LOAITAILIEU_THUOCTINH();
            model.ListLoaiTaiLieu = initDanhMuc();
            return PartialView("_Create", model);
        }
        public PartialViewResult Edit(long id)
        {
            TaiLieuThuocTinhModel model = new TaiLieuThuocTinhModel();
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            LOAITAILIEU_THUOCTINH ThuocTinh = LOAITAILIEU_THUOCTINHBusiness.Find(id);
            if (ThuocTinh == null)
            {
                ThuocTinh = new LOAITAILIEU_THUOCTINH();
            }
            model.ThuocTinh = ThuocTinh;
            model.ListLoaiTaiLieu = initDanhMuc();
            return PartialView("_Create", model);
        }
        #endregion
        #region Các hàm json
        public JsonResult Delete(long id)
        {
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            LOAITAILIEU_THUOCTINH ThuocTinh = LOAITAILIEU_THUOCTINHBusiness.Find(id);
            if (ThuocTinh == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy thuộc tính cần xóa" });
            }
            LOAITAILIEU_THUOCTINHBusiness.repository.Delete(id);
            LOAITAILIEU_THUOCTINHBusiness.Save();
            return Json(new { Type = "SUCCESS", Message = "Xóa thuộc tính thành công" });
        }
        public JsonResult SaveItem(LOAITAILIEU_THUOCTINH ThuocTinh)
        {
            if (ThuocTinh.DANHMUC_ID == 0)
            {
                return Json(new { Type = "ERROR", Message = "Bạn chưa chọn loại tài liệu cho thuộc tính" }, JsonRequestBehavior.AllowGet);
            }
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            DM_DANHMUC_DATA DanhMuc = DM_DANHMUC_DATABusiness.Find(ThuocTinh.DANHMUC_ID);
            if (DanhMuc == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy loại tài liệu được chọn" }, JsonRequestBehavior.AllowGet);
            }
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            if (ThuocTinh.ID > 0)
            {
                #region Cập nhật thuộc tính
                var result = LOAITAILIEU_THUOCTINHBusiness.Find(ThuocTinh.ID);
                if (result == null)
                {
                    return Json(new { Type = "ERROR", Message = "Không tìm thấy thuộc tính cần cập nhật" }, JsonRequestBehavior.AllowGet);
                }
                result.DANHMUC_ID = ThuocTinh.DANHMUC_ID;
                result.MOTA = ThuocTinh.MOTA;
                result.TEN_THUOCTINH = ThuocTinh.TEN_THUOCTINH;
                result.TRANGTHAI = ThuocTinh.TRANGTHAI;
                LOAITAILIEU_THUOCTINHBusiness.Save(result);
                return Json(new { Type = "SUCCESS", Message = "Cập nhật thuộc tính thành công" }, JsonRequestBehavior.AllowGet);
                #endregion
            }
            else
            {
                #region Thêm mới thuộc tính
                LOAITAILIEU_THUOCTINHBusiness.Save(ThuocTinh);
                #endregion
            }
            return Json(new { Type = "SUCCESS", Message = "Thêm mới thuộc tính thành công" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            var searchModel = SessionManager.GetValue("thuoctinhSearch") as TAILIEU_THUOCTINH_SEARCH;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new TAILIEU_THUOCTINH_SEARCH();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("thuoctinhSearch", searchModel);
            }
            var data = LOAITAILIEU_THUOCTINHBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            var searchModel = SessionManager.GetValue("thuoctinhSearch") as TAILIEU_THUOCTINH_SEARCH;

            if (searchModel == null)
            {
                searchModel = new TAILIEU_THUOCTINH_SEARCH();
                searchModel.pageSize = 20;
            }

            searchModel.LOAI_TAILIEU = form["LOAI_TAILIEU"].ToIntOrZero();
            searchModel.TEN_THUOCTINH = form["TEN_THUOCTINH"];
            if (!string.IsNullOrEmpty(form["TRANGTHAI"]))
            {
                searchModel.TRANGTHAI = form["TRANGTHAI"].Equals("1");
            }
            else
            {
                searchModel.TRANGTHAI = null;
            }
            SessionManager.SetValue("thuoctinhSearch", searchModel);
            var data = LOAITAILIEU_THUOCTINHBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }
        #endregion
    }
}