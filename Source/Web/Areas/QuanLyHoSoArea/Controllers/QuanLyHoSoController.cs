using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QUANLYHOSO;
using Business.CommonModel.QUANLYVANBAN;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.QuanLyHoSoArea.Models;
using Web.Common;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.QuanLyHoSoArea.Controllers
{
    public class QuanLyHoSoController : BaseController
    {
        //
        // GET: /QuanLyHoSoArea/QuanLyHoSo/
        private QuanLyHoSoBusiness QuanLyHoSoBusiness;

        private QuanLyVanBanBusiness QuanLyVanBanBusiness;
        private TAILIEUDINHKEMBusiness TaiLieuDinhKemBusiness;
        private QuanLyHoSoNguoiNhapBusiness QuanLyHoSoNguoiNhapBusiness;
        private string UrlPath = WebConfigurationManager.AppSettings["FileUpload"];

        private string UrlImport = WebConfigurationManager.AppSettings["ImportFile"];

        private const int Limit = 20;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private int pageSize = 10;
        private int pageNumber = 1;

        private string URLPath = WebConfigurationManager.AppSettings["FileUpload"];
        private string FileAllowUpload = WebConfigurationManager.AppSettings["VANBAN_FileAllowUpload"];
        private string MaxFileSizeUpload = WebConfigurationManager.AppSettings["VANBAN_MaxSizeUpload"];

        public ActionResult Index()
        {
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            QuanLyHoSoIndexViewModel model = new QuanLyHoSoIndexViewModel();
            model.Source = QuanLyHoSoBusiness.GetPage(null, pageNumber, pageSize);
            model.ListHoSoNam = GetListNam();
            model.ListNamChinhLy = GetListNam();
            model.ListKho = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_KHO);
            model.ListPhongBan = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_PHONG);
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            SessionManager.Remove("ListHoSo");
            SessionManager.SetValue("ListHoSo", model.Source);
            return View(model);
        }

        public PartialViewResult FormVanBanIndex(long? hoSoId = 0, long? id = 0)
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
            model.HOSO_ID = hoSoId;
            model.ListCoQuanBanHanh = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_CO_QUAN_BAN_HANH, VanBan.COQUAN_BANHANH_ID);
            model.ListDoMat = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.DOMAT, VanBan.DOMAT_ID);
            model.ListHoSo = QuanLyHoSoBusiness.GetDropDow(hoSoId);
            model.ListLinhVuc = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.LINHVUCVANBAN, VanBan.LINHVUC_ID);
            model.ListLoaiVanBan = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.LOAI_VANBAN, VanBan.LOAI_VANBAN_ID);
            model.ListMucDoTruyCap = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_MUC_DO_TRUY_CAP, VanBan.MUCDO_TRUYCAP);
            model.ListNgonNgu = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_NGON_NGU, VanBan.NGONNGU_ID);
            model.ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(VanBan.ID, LOAITAILIEU.VANBAN);
            model.ListTinhTrangVatLy = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_TINH_TRANG_VAT_LY, VanBan.TINHTRANG_VATLY);
            return PartialView("_FormVanBanIndex", model);
        }
        public PartialViewResult FormVanBan(long? hoSoId = 0, long? id = 0)
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
            model.HOSO_ID = hoSoId;
            model.ListCoQuanBanHanh = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_CO_QUAN_BAN_HANH, VanBan.COQUAN_BANHANH_ID);
            model.ListDoMat = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.DOMAT, VanBan.DOMAT_ID);
            model.ListHoSo = QuanLyHoSoBusiness.GetDropDow(hoSoId);
            model.ListLinhVuc = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.LINHVUCVANBAN, VanBan.LINHVUC_ID);
            model.ListLoaiVanBan = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.LOAI_VANBAN, VanBan.LOAI_VANBAN_ID);
            model.ListMucDoTruyCap = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_MUC_DO_TRUY_CAP, VanBan.MUCDO_TRUYCAP);
            model.ListNgonNgu = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_NGON_NGU, VanBan.NGONNGU_ID);
            model.ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(VanBan.ID, LOAITAILIEU.VANBAN);
            model.ListTinhTrangVatLy = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_TINH_TRANG_VAT_LY, VanBan.TINHTRANG_VATLY);
            return PartialView("_FormVanBan", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveVanBan(FormCollection form, List<HttpPostedFileBase> filebase, string[] FOLDER_ID, string[] filename)
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
            if (filebase != null && filebase.Count() > 0)
            {

                if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0 && filebase[0] != null)
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

                    #endregion Xóa tài liệu đi kèm
                }
                UploadFileTool tool = new UploadFileTool();
                var exited = tool.UploadCustomFile(filebase, true, FileAllowUpload, URLPath, MaxFileSizeUpload.ToIntOrZero(), FOLDER_ID, filename, SaveVanBan.ID, LOAITAILIEU.VANBAN, "Văn Bản");
            }
            return Json(new { VanBan = QuanLyVanBanBusiness.GetByHoSo(SaveVanBan.HOSO_ID), message = title }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            var searchModel = SessionManager.GetValue("HoSoSearchModel") as QuanLyHoSoSearchModel;

            if (searchModel == null)
            {
                searchModel = new QuanLyHoSoSearchModel();
                searchModel.pageSize = 20;
            }

            searchModel.HoSoNam = form["HOSO_NAM"].ToIntOrZero();
            searchModel.NamChinhLy = form["NAM_CHINHLY"].ToIntOrZero();
            searchModel.KhoId = form["KHO_ID"].ToIntOrZero();
            searchModel.PhongId = form["PHONG_ID"].ToIntOrZero();
            searchModel.FTS = form["FTS"];
            SessionManager.SetValue("HoSoSearchModel", searchModel);

            var data = QuanLyHoSoBusiness.GetPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }

        public PartialViewResult ReloadGrid(int? HOSO_NAM = 0, int? NAM_CHINHLY = 0, int? KHO_ID = 0, int? PHONG_ID = 0, string FTS = "")
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            QuanLyHoSoIndexViewModel model = new QuanLyHoSoIndexViewModel();
            model.Source = QuanLyHoSoBusiness.GetPage(null, pageNumber, pageSize);
            ViewBag.CurrentSort = string.Empty;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            return PartialView("_SearchResult", model);
        }

        public PartialViewResult DetailVanBan(long? id = 0)
        {
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            TaiLieuDinhKemBusiness = Get<TAILIEUDINHKEMBusiness>();
            VanBanDetailViewModel model = new VanBanDetailViewModel();
            model.VanBan = QuanLyVanBanBusiness.GetDetail(id);
            model.ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(id.Value, LOAITAILIEU.VANBAN);
            return PartialView("_DetailVanBan", model);
        }

        public PartialViewResult DetailHoSo(long? id = 0)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyHoSoNguoiNhapBusiness = Get<QuanLyHoSoNguoiNhapBusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();

            QuanLyHoSoDetailViewModel model = new QuanLyHoSoDetailViewModel();
            model.HoSo = QuanLyHoSoBusiness.GetDetailBO(id);
            model.NguoiNhap = QuanLyHoSoNguoiNhapBusiness.GetText(id);
            var listVanBan = new ListVanBanBO();
            listVanBan.ListVanBan = QuanLyVanBanBusiness.GetByHoSo(id);
            listVanBan.HoSoID = id;
            model.ListVanBan = listVanBan;
            return PartialView("_DetailHoSo", model);
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

            #endregion Xóa tài liệu đi kèm

            return Json(new { message = "Xóa văn bản thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TieuDeExist(string tieuDe = "", int id = 0)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            return Json(QuanLyHoSoBusiness.TieuDeExisted(tieuDe, id));
        }

        public JsonResult HoSoSoExist(string hoSoSo = "", int id = 0)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            return Json(QuanLyHoSoBusiness.HoSoSoExisted(hoSoSo, id));
        }

        public PartialViewResult GetListVanBan(long hoSoId)
        {
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            var model = new ListVanBanBO();
            model.HoSoID = hoSoId;
            model.ListVanBan = QuanLyVanBanBusiness.GetByHoSo(hoSoId);
            return PartialView("_ListVanBan", model);
        }


        #region Các hàm Partialview

        public PartialViewResult FormHoSo(long? id = 0)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            QuanLyHoSoNguoiNhapBusiness = Get<QuanLyHoSoNguoiNhapBusiness>();
            QuanLyHoSoModel model = new QuanLyHoSoModel();
            var HoSo = new QUANLY_HOSO();
            model.ListNamHoSo = GetListNam(DateTime.Now.Year - 20, DateTime.Now.Year + 20, DateTime.Now.Year);
            model.NamChinhLy = GetListNam();
            if (id > 0)
            {
                HoSo = QuanLyHoSoBusiness.Find(id);
                if (HoSo.HOSO_NAM.HasValue)
                {
                    model.ListNamHoSo = GetListNam(HoSo.HOSO_NAM.Value - 20, HoSo.HOSO_NAM.Value + 20, HoSo.HOSO_NAM);
                }
                if (HoSo.NAM_CHINHLY.HasValue)
                {
                    model.NamChinhLy = GetListNam(HoSo.NAM_CHINHLY.Value - 20, HoSo.NAM_CHINHLY.Value + 20, HoSo.NAM_CHINHLY);
                }
                model.ListNguoiNhap = DM_NGUOIDUNGBusiness.GetDropDow(QuanLyHoSoNguoiNhapBusiness.GetByHoSo(id.Value));
            }
            else
            {
                model.ListNguoiNhap = DM_NGUOIDUNGBusiness.GetDropDow();
            }
            model.HoSo = HoSo;
            model.ListThoiHan = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_THOI_HAN_BAO_QUAN, HoSo.THOIHAN_BAOQUAN_ID);
            model.ListAccessModifier = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_MUC_DO_TRUY_CAP, HoSo.MUCDO_TRUYCAP);
            model.ListKho = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_KHO, HoSo.KHO_ID);
            model.ListPhong = DM_DANHMUC_DATABusiness.GetDropDow(DMLOAI_CONSTANT.QLHS_PHONG, HoSo.PHONG_ID);

            return PartialView("_FormHoSo", model);
        }

        #endregion Các hàm Partialview

        #region Các hàm Jsonresult

        public JsonResult DeleteHoSo(long id)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyVanBanBusiness = Get<QuanLyVanBanBusiness>();
            TaiLieuDinhKemBusiness = Get<TAILIEUDINHKEMBusiness>();
            QUANLY_HOSO HoSo = QuanLyHoSoBusiness.Find(id);
            if (HoSo == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy hồ sơ cần xóa" });
            }

            #region Xóa các văn bản đi kèm

            List<QUANLY_VANBAN> ListVanBan = QuanLyVanBanBusiness.GetData(id);
            foreach (var item in ListVanBan)
            {
                QuanLyVanBanBusiness.Delete(item.ID);
            }
            QuanLyVanBanBusiness.Save();

            #endregion Xóa các văn bản đi kèm

            #region Xóa tài liệu đi kèm

            List<TAILIEUDINHKEM> ListTaiLieu = TaiLieuDinhKemBusiness.GetDataByItemID(id, LOAITAILIEU.QUANLY_HOSO);
            ListTaiLieu.AddRange(TaiLieuDinhKemBusiness.GetDataByItemID(ListVanBan.Select(x => x.ID).ToList(), LOAITAILIEU.QUANLY_VANBAN));
            FileUltilities file = new FileUltilities();
            foreach (var item in ListTaiLieu)
            {
                file.RemoveFile(UrlPath + "\\" + item.DUONGDAN_FILE);
                TaiLieuDinhKemBusiness.Delete(item.TAILIEU_ID);
            }
            TaiLieuDinhKemBusiness.Save();

            #endregion Xóa tài liệu đi kèm

            #region Xóa người nhận

            QuanLyHoSoNguoiNhapBusiness = Get<QuanLyHoSoNguoiNhapBusiness>();
            QuanLyHoSoNguoiNhapBusiness.DeleteByHoSo(id);
            QuanLyHoSoNguoiNhapBusiness.Save();

            #endregion Xóa người nhận

            QuanLyHoSoBusiness.Delete(id);
            QuanLyHoSoBusiness.Save();
            return Json(new { Status = true, Message = "Xóa hồ sơ thành công" });
        }

        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            var searchModel = SessionManager.GetValue("HoSoSearchModel") as QuanLyHoSoSearchModel;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new QuanLyHoSoSearchModel();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("HoSoSearchModel", searchModel);
            }
            var data = QuanLyHoSoBusiness.GetPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveHoSo(FormCollection form)
        {
            QuanLyHoSoBusiness = Get<QuanLyHoSoBusiness>();
            QuanLyHoSoNguoiNhapBusiness = Get<QuanLyHoSoNguoiNhapBusiness>();
            var HoSo = new QUANLY_HOSO();
            string title = "Thêm mới hồ sơ thành công";
            if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0)
            {
                HoSo = QuanLyHoSoBusiness.Find(Convert.ToInt32(form["ID"]));
                title = "Cập nhật hồ sơ thành công";
            }
            HoSo.KHO_ID = form["KHO_ID"].ToIntOrNULL();
            HoSo.PHONG_ID = form["PHONG_ID"].ToIntOrNULL();
            HoSo.PHONGSO = form["PHONGSO"];
            HoSo.MUCLUC_SO = form["MUCLUC_SO"];
            HoSo.HOPSO = form["HOPSO"];
            HoSo.HOSO_SO = form["HOSO_SO"];
            HoSo.TIEUDE = form["TIEUDE"];
            HoSo.THOIGIAN_TAILIEU = form["THOIGIAN_TAILIEU"];
            HoSo.CHUGIAI = form["CHUGIAI"];
            HoSo.SOLUONG_TO = form["SOLUONG_TO"];
            HoSo.THOIHAN_BAOQUAN_ID = form["THOIHAN_BAOQUAN_ID"].ToIntOrNULL();
            HoSo.NAM_CHINHLY = form["NAM_CHINHLY"].ToIntOrNULL();
            HoSo.MUCDO_TRUYCAP = form["MUCDO_TRUYCAP"].ToIntOrNULL();
            HoSo.HOSO_NAM = form["HOSO_NAM"].ToIntOrNULL();
            HoSo.FTS = HoSo.PHONGSO + " " + HoSo.MUCLUC_SO + " " + HoSo.HOPSO + " " + HoSo.HOSO_SO + " " + HoSo.TIEUDE.ConvertToVN() + " " + HoSo.SOLUONG_TO;
            QuanLyHoSoBusiness.Save(HoSo);
            QuanLyHoSoNguoiNhapBusiness.DeleteByHoSo(HoSo.ID);
            if (!string.IsNullOrEmpty(form["NGUOINHAP"]))
            {
                var NguoiNhap = form["NGUOINHAP"].ToListInt(',');
                foreach (var item in NguoiNhap)
                {
                    QUANLY_HOSO_NGUOINHAP SaveNguoiNhap = new QUANLY_HOSO_NGUOINHAP();
                    SaveNguoiNhap.HOSO_ID = HoSo.ID;
                    SaveNguoiNhap.USER_ID = item;
                    QuanLyHoSoNguoiNhapBusiness.Save(SaveNguoiNhap);
                }
            }
            return Json(new { Type = "SUCCESS", Message = title });
        }

        #endregion Các hàm Jsonresult

        public List<SelectListItem> GetListNam(int? yearStart = 0, int? yearEnd = 0, int? selected = 0)
        {
            List<SelectListItem> listYear = new List<SelectListItem>();
            if (yearStart <= 0)
            {
                yearStart = DateTime.Now.Year - 20;
            }
            if (yearEnd <= 0)
            {
                yearEnd = DateTime.Now.Year + 20;
            }
            for (int i = yearStart.Value; i < yearEnd; i++)
            {
                listYear.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == selected) });
            }
            return listYear;
        }
    }
}