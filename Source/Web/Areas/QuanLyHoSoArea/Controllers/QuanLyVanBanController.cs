using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Custom;
using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using Web.Common;
using Web.Areas.QuanLyHoSoArea.Models;
using Business.CommonModel.QUANLYVANBAN;
using System.Web.Configuration;
using Web.FwCore;
using System.IO;
using VTUtils.Excel.Export;
using Business.CommonModel.CONSTANT;
using CommonHelper;

namespace Web.Areas.QuanLyHoSoArea.Controllers
{
    public class QuanLyVanBanController : BaseController
    {
        //
        // GET: /QuanLyHoSoArea/QuanLyVanBan/
        private QuanLyHoSoBusiness QuanLyHoSoBusiness;
        private QuanLyVanBanBusiness QuanLyVanBanBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private TAILIEUDINHKEMBusiness TaiLieuDinhKemBusiness;
        private CCTC_THANHPHANBusiness CCTCThanhPhanBusiness;
        private string URLPath = WebConfigurationManager.AppSettings["FileUpload"];
        private string FileAllowUpload = WebConfigurationManager.AppSettings["VANBAN_FileAllowUpload"];
        private string MaxFileSizeUpload = WebConfigurationManager.AppSettings["VANBAN_MaxSizeUpload"];

        int pageSize = 10;
        int pageNumber = 1;
        public ActionResult Index()
        {
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            QuanLyVanBanIndexViewModel model = new QuanLyVanBanIndexViewModel();
            model.Source = QuanLyHoSoBusiness.GetPageForVanBan(null,pageNumber, pageSize);
            model.ListHoSoNam = GetListNam();
            model.ListNamChinhLy = GetListNam();
            model.ListKho = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_KHO);
            model.ListPhongBan = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_CO_QUAN_BAN_HANH);
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            return View(model);
        }

        public PartialViewResult VanBanSorting(int? pageSize = 10, int? pageNumber = 1, string sortExpression = "", int? HOSO_NAM = 0, int? NAM_CHINHLY = 0, int? KHO_ID = 0, int? PHONG_ID = 0, string FTS = "")
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            QuanLyVanBanIndexViewModel model = new QuanLyVanBanIndexViewModel();
            model.Source = QuanLyHoSoBusiness.GetPageForVanBan(null, pageNumber.Value, pageSize.Value);
            ViewBag.CurrentSort = sortExpression;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            return PartialView("_SearchResult", model);
        }
        public PartialViewResult FormVanBan(long? id = 0, long? HoSoID = 0)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            TaiLieuDinhKemBusiness = Get<TAILIEUDINHKEMBusiness>();

            FormVanBanModel model = new FormVanBanModel();
            var VanBan = new QUANLY_VANBAN();
            if (id > 0)
            {
                VanBan = QuanLyVanBanBusiness.Find(id);
            }
            model.VanBan = VanBan;
            model.HOSO_ID = HoSoID;
            model.ListCoQuanBanHanh = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_CO_QUAN_BAN_HANH, VanBan.COQUAN_BANHANH_ID);
            model.ListDoMat = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.DOMAT, VanBan.DOMAT_ID);
            model.ListHoSo = QuanLyHoSoBusiness.GetDropDow(HoSoID);
            model.ListLinhVuc = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.LINHVUCVANBAN, VanBan.LINHVUC_ID);
            model.ListLoaiVanBan = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.LOAI_VANBAN, VanBan.LOAI_VANBAN_ID);
            model.ListMucDoTruyCap = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_MUC_DO_TRUY_CAP, VanBan.MUCDO_TRUYCAP);
            model.ListNgonNgu = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_NGON_NGU, VanBan.NGONNGU_ID);
            model.ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(VanBan.ID, LOAITAILIEU.VANBAN);
            model.ListTinhTrangVatLy = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_TINH_TRANG_VAT_LY, VanBan.TINHTRANG_VATLY);
            return PartialView("_FormVanBan", model);
        }

        public PartialViewResult Detail(long? id = 0)
        {
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            TaiLieuDinhKemBusiness = Get<TAILIEUDINHKEMBusiness>();
            VanBanDetailViewModel model = new VanBanDetailViewModel();
            model.VanBan = QuanLyVanBanBusiness.GetDetail(id);
            model.ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(id.Value, LOAITAILIEU.VANBAN);
            return PartialView("_Detail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveVanBan(FormCollection form, IEnumerable<HttpPostedFileBase> filebase, string[] FOLDER_ID, string[] filename)
        {
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            TaiLieuDinhKemBusiness = Get<TAILIEUDINHKEMBusiness>();

            var SaveVanBan = new QUANLY_VANBAN();
            string title = "Thêm mới văn bản thành công";
            if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0)
            {
                SaveVanBan = QuanLyVanBanBusiness.Find(Convert.ToInt32(form["ID"]));
                title = "Cập nhật văn bản thành công";
            }
            SaveVanBan.COQUAN_BANHANH_ID = form["COQUAN_BANHANH_ID"].ToIntOrNULL();
            SaveVanBan.DOMAT_ID = form["DOMAT_ID"].ToIntOrNULL();
            SaveVanBan.GHICHU = form["GHICHU"];
            SaveVanBan.HOSO_ID = form["HOSO_ID"].ToIntOrNULL();
            SaveVanBan.LINHVUC_ID = form["LINHVUC_ID"].ToIntOrNULL();
            SaveVanBan.LOAI_VANBAN_ID = form["LOAI_VANBAN_ID"].ToIntOrNULL();
            SaveVanBan.MUCDO_TRUYCAP = form["MUCDO_TRUYCAP"].ToIntOrNULL();
            SaveVanBan.NGAYBANHANH = form["NGAYBANHANH"].ToDateTime();
            SaveVanBan.NGONNGU_ID = form["NGONNGU_ID"].ToIntOrNULL();
            SaveVanBan.SO_KYHIEU = form["SO_KYHIEU"];
            SaveVanBan.TIEUDE = form["TIEUDE"];
            SaveVanBan.TINHTRANG_VATLY = form["TINHTRANG_VATLY"].ToIntOrNULL();
            SaveVanBan.TOSO = form["TOSO"].ToIntOrNULL();
            SaveVanBan.TRICHYEU_VANBAN = form["TRICHYEU_VANBAN"];
            QuanLyVanBanBusiness.Save(SaveVanBan);
            //Lưu file đính kèm
            if (filebase != null)
            {
                if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0)
                {
                    #region Xóa tài liệu đi kèm
                    List<TAILIEUDINHKEM> ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(SaveVanBan.ID, LOAITAILIEU.VANBAN);
                    FileUltilities file = new FileUltilities();
                    foreach (var item in ListTaiLieu)
                    {
                        file.RemoveFile(URLPath + "\\" + item.DUONGDAN_FILE);
                        TaiLieuDinhKemBusiness.Delete(item.TAILIEU_ID);
                    }
                    TaiLieuDinhKemBusiness.Save();
                    #endregion
                }
                UploadFileTool tool = new UploadFileTool();
                var exited = tool.UploadCustomFile(filebase, true, FileAllowUpload, URLPath, MaxFileSizeUpload.ToIntOrZero(), FOLDER_ID, filename, SaveVanBan.ID, LOAITAILIEU.VANBAN, "Văn Bản");
            }
            return Json(new { VanBan = QuanLyVanBanBusiness.GetByHoSo(SaveVanBan.HOSO_ID), message = title }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteVanBan(long? id = 0)
        {
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            TaiLieuDinhKemBusiness = Get<TAILIEUDINHKEMBusiness>();
            QuanLyVanBanBusiness.Delete(id);
            QuanLyVanBanBusiness.Save();
            #region Xóa tài liệu đi kèm
            List<TAILIEUDINHKEM> ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(id.Value, LOAITAILIEU.VANBAN);
            FileUltilities file = new FileUltilities();
            foreach (var item in ListTaiLieu)
            {
                file.RemoveFile(URLPath + "\\" + item.DUONGDAN_FILE);
                TaiLieuDinhKemBusiness.Delete(item.TAILIEU_ID);
            }
            TaiLieuDinhKemBusiness.Save();
            #endregion
            return Json(new { message = "Xóa văn bản thành công" }, JsonRequestBehavior.AllowGet);
        }
        public List<SelectListItem> GetListNam(int? selected = 0)
        {
            List<SelectListItem> listYear = new List<SelectListItem>();
            var yearStart = DateTime.Now.Year - 20;
            var yearEnd = DateTime.Now.Year + 20;
            for (int i = yearStart; i < yearEnd; i++)
            {
                listYear.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == selected) });
            }
            return listYear;
        }
    }
}
