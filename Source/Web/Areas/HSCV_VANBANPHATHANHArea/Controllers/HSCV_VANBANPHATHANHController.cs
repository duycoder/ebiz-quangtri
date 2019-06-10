using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVVANBANDEN;
using CommonHelper;
using Model.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.HSCV_VANBANPHATHANHArea.Models;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Common;
using Web.Custom;
using Web.FwCore;
using Web.Models;

namespace Web.Areas.HSCV_VANBANPHATHANHArea.Controllers
{
    public class HSCV_VANBANPHATHANHController : BaseController
    {
        // GET: HSCV_VANBANPHATHANHArea/HSCV_VANBANPHATHANH
        private HSCV_VANBANDENBusiness HSCV_VANBANDENBusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string VbDenExtension = WebConfigurationManager.AppSettings["VbDenExtension"];
        private int VbDenSize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private HSCV_VANBANDI_DONVINHANBusiness HSCV_VANBANDI_DONVINHANBusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private THUMUC_LUUTRUBusiness THUMUC_LUUTRUBusiness;
        private HSCV_VANBANDEN_DONVINHANBusiness HSCV_VANBANDEN_DONVINHANBusiness;
        #region Các hàm ActionResult
        public ActionResult Index()
        {
            HscvVanBanPhatHanhModel model = new HscvVanBanPhatHanhModel();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            //model.ListDoMat = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOMAT);
            model.ListDoKhan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOKHAN);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOUUTIEN);
            model.ListLoaiVanBan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.LOAI_VANBAN);
            model.ListLinhVucVanBan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.LINHVUCVANBAN);
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            HSCV_VANBANDEN_SEARCH searchModel = new HSCV_VANBANDEN_SEARCH();
            AssignUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            List<int> Ids = new List<int>();
            if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.DONVI))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                if (DonVi != null && DonVi.PARENT_ID.HasValue && DonVi.PARENT_ID.Value > 0)
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(DonVi.PARENT_ID.Value).Select(x => x.ID).ToList();
                    Ids.Add(DonVi.PARENT_ID.Value);
                    model.TreeData = CCTC_THANHPHANBusiness.GetTree(DonVi.PARENT_ID.Value);
                    searchModel.DONVI_ID = DonVi.PARENT_ID.Value;
                }
                else
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                    Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                    searchModel.DONVI_ID = currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0;
                }
            }
            //Nếu là quản lý phòng ban
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.PHONGBAN))
            {
                Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                searchModel.DONVI_ID = currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0;
            }
            else
            {
                Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                //searchModel.USER_ID = currentUser.ID;
            }
            searchModel.ListDonVi = Ids;
            if (model.TreeData == null)
            {
                model.TreeData = CCTC_THANHPHANBusiness.GetTree(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
            }
            searchModel.pageSize = MaxPerpage;
            searchModel.pageIndex = 1;
            searchModel.ListIds = initIds(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
            model.ListResult = HSCV_VANBANDENBusiness.GetDaTaByPage(searchModel, MaxPerpage);
            SessionManager.SetValue("VanBanDenSearch", searchModel);
            //model.ListVanBan = initVanBanDi();
            model.UserInfoBO = currentUser;
            return View(model);
        }
        public ActionResult Create()
        {
            HscvVanBanPhatHanhModel model = new HscvVanBanPhatHanhModel();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            //model.ListDoMat = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOMAT);
            model.ListDoKhan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOKHAN);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOUUTIEN);
            model.ListLoaiVanBan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.LOAI_VANBAN);
            model.ListLinhVucVanBan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.LINHVUCVANBAN);
            model.ListTaiLieu = new List<TAILIEUDINHKEM>();
            model.VanBan = new HSCV_VANBANDEN();
            model.Extension = VbDenExtension;
            model.MaxSize = VbDenSize;
            model.ListDonVi = new List<CCTC_THANHPHAN>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            model.TreeDonVi = CCTC_THANHPHANBusiness.GetTree(0);
            return View(model);
        }
        public ActionResult Edit(long id)
        {
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            var VanBan = HSCV_VANBANDENBusiness.Find(id);
            AssignUserInfo();
            if (VanBan == null || currentUser.ID != VanBan.NGUOITAO)
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
            HscvVanBanPhatHanhModel model = new HscvVanBanPhatHanhModel();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            //model.ListDoMat = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOMAT);
            model.ListDoKhan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOKHAN);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.DOUUTIEN);
            model.ListLoaiVanBan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.LOAI_VANBAN);
            model.ListLinhVucVanBan = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.LINHVUCVANBAN);
            model.Extension = VbDenExtension;
            model.MaxSize = VbDenSize;
            model.ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.VANBANDEN);
            model.VanBan = VanBan;
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            model.TreeDonVi = CCTC_THANHPHANBusiness.GetTree(0);
            HSCV_VANBANDEN_DONVINHANBusiness = Get<HSCV_VANBANDEN_DONVINHANBusiness>();
            var ListDonViId = HSCV_VANBANDEN_DONVINHANBusiness.GetData(id);
            model.ListDonVi = model.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(ListDonViId.Where(o => o.DONVI_ID.HasValue).Select(x => x.DONVI_ID.Value).ToList());
            return View("Create", model);
        }
        public ActionResult Detail(long id)
        {
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            var VanBanBO = HSCV_VANBANDENBusiness.FindById(id);
            if (VanBanBO == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
            AssignUserInfo();
            HSCV_VANBANDEN_DONVINHANBusiness = Get<HSCV_VANBANDEN_DONVINHANBusiness>();
            var ListDonViId = HSCV_VANBANDEN_DONVINHANBusiness.GetData(id);
            //Nếu là quản lý đơn vị
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.DONVI))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                List<int> Ids = new List<int>();
                if (DonVi != null && DonVi.PARENT_ID.HasValue && DonVi.PARENT_ID.Value > 0)
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(DonVi.PARENT_ID.Value).Select(x => x.ID).ToList();
                    Ids.Add(DonVi.PARENT_ID.Value);
                }
                else
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                    Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                }
                if (ListDonViId.Where(x => Ids.Contains(x.DONVI_ID.Value)).Count() == 0)
                {
                    return RedirectToAction("NotFound", "Home", new { area = "" });
                }
                //if (!VanBanBO.DONVI_ID.HasValue || !Ids.Contains(VanBanBO.DONVI_ID.Value))
                //{
                //    return RedirectToAction("NotFound", "Home", new { area = "" });
                //}
            }
            //Nếu là quản lý phòng ban
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.PHONGBAN))
            {
                List<int> Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                if (ListDonViId.Where(x => Ids.Contains(x.DONVI_ID.Value)).Count() == 0)
                {
                    return RedirectToAction("NotFound", "Home", new { area = "" });
                }
                //if (!VanBanBO.DONVI_ID.HasValue || !Ids.Contains(VanBanBO.DONVI_ID.Value))
                //{
                //    return RedirectToAction("NotFound", "Home", new { area = "" });
                //}
            }
            else if (currentUser.ID != VanBanBO.NGUOITAO && !ListDonViId.Select(x => x.DONVI_ID).Contains(currentUser.DM_PHONGBAN_ID))
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
            //List<long> Ids = initIds(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
            //if (VanBanBO.VANBANDI_ID.HasValue && !Ids.Contains(VanBanBO.VANBANDI_ID.Value))
            //{
            //    return RedirectToAction("NotFound", "Home");
            //}

            HscvVanBanPhatHanhModel model = new HscvVanBanPhatHanhModel();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            model.ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.VANBANDEN);
            model.VanBanBO = VanBanBO;

            model.ListDonVi = model.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(ListDonViId.Where(o => o.DONVI_ID.HasValue).Select(x => x.DONVI_ID.Value).ToList());
            return View(model);
        }
        #endregion
        #region Các hàm private
       
        private List<CommonError> IsValid(HSCV_VANBANDEN VanBan)
        {
            List<CommonError> ListError = new List<CommonError>();
            CommonError error = new CommonError();
            if (string.IsNullOrEmpty(VanBan.SOHIEU))
            {
                error.Field = "SOHIEU";
                error.Message = "Bạn chưa nhập số/ ký hiệu";
                ListError.Add(error);
            }
            else if (VanBan.SOHIEU.Length > 50)
            {
                error.Field = "SOHIEU";
                error.Message = "Số/ ký hiệu không được lớn hơn 50 ký tự";
                ListError.Add(error);
            }
            if (string.IsNullOrEmpty(VanBan.NGUOIKY))
            {
                error.Field = "NGUOIKY";
                error.Message = "Bạn chưa nhập người ký";
                ListError.Add(error);
            }
            if (string.IsNullOrEmpty(VanBan.TRICHYEU))
            {
                error = new CommonError();
                error.Field = "SOHIEU";
                error.Message = "Bạn chưa nhập số/ ký hiệu";
                ListError.Add(error);
            }
            if (!VanBan.LOAIVANBAN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "LOAIVANBAN_ID";
                error.Message = "Bạn chưa chọn loại văn bản";
                ListError.Add(error);
            }
            if (!VanBan.LINHVUCVANBAN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "LINHVUCVANBAN_ID";
                error.Message = "Bạn chưa chọn lĩnh vực văn bản";
                ListError.Add(error);
            }
            if (!VanBan.DOKHAN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "DOKHAN_ID";
                error.Message = "Bạn chưa chọn độ khẩn văn bản";
                ListError.Add(error);
            }
            if (!VanBan.DOMAT_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "DOMAT_ID";
                error.Message = "Bạn chưa chọn độ mật văn bản";
                ListError.Add(error);
            }
         
            return ListError;
        }
        private List<HSCV_VANBANDI> initVanBanDi()
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            return HSCV_VANBANDIBusiness.GetData();
        }
        /// <summary>
        /// Danh sách mã id văn bản trình ký mà đơn vị đang đăng nhập có thể xem
        /// </summary>
        /// <returns></returns>
        private List<long> initIds(int DonViId)
        {
            HSCV_VANBANDI_DONVINHANBusiness = Get<HSCV_VANBANDI_DONVINHANBusiness>();
            return HSCV_VANBANDI_DONVINHANBusiness.GetIdsByDonVi(DonViId);
        }
        private List<int> initDonVi(UserInfoBO currentUser)
        {
            if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.DONVI))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                List<int> Ids = new List<int>();
                if (DonVi != null && DonVi.PARENT_ID.HasValue && DonVi.PARENT_ID.Value > 0)
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(DonVi.PARENT_ID.Value).Select(x => x.ID).ToList();
                    Ids.Add(DonVi.PARENT_ID.Value);
                }
                else
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                    Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                }
                return Ids;
            }
            //Nếu là quản lý phòng ban
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.PHONGBAN))
            {
                List<int> Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                return Ids;
            }
            return new List<int>();
        }
        #endregion
        #region Các hàm Partialview

        #endregion
        #region Các hàm json
        [ValidateInput(false)]
        public JsonResult SaveObj(HSCV_VANBANDEN VanBan, FormCollection col, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            try
            {
                List<CommonError> ListError = new List<CommonError>();
                Regex regex = new Regex(@"\d{2}/\d{2}/\d{4}");
                #region Gán dữ liệu string
                string NGUOIKY = string.Empty;
                string CHUCVU = string.Empty;
                string TRICHYEU = string.Empty;
                string NOIDUNG = string.Empty;
                string SOHIEU = string.Empty;
                string SOTHEOSO = string.Empty;
                string YKIENCHIDAO = string.Empty;
                if (!string.IsNullOrEmpty(col["NGUOIKY"]))
                {
                    NGUOIKY = col["NGUOIKY"].Trim();
                }
                if (!string.IsNullOrEmpty(col["CHUCVU"]))
                {
                    CHUCVU = col["CHUCVU"].Trim();
                }
                if (!string.IsNullOrEmpty(col["TRICHYEU"]))
                {
                    TRICHYEU = col["TRICHYEU"].Trim();
                }
                if (!string.IsNullOrEmpty(col["NOIDUNG"]))
                {
                    NOIDUNG = col["NOIDUNG"].Trim();
                }
                if (!string.IsNullOrEmpty(col["SOHIEU"]))
                {
                    SOHIEU = col["SOHIEU"].Trim();
                }
                CommonError commonError;
                if (!string.IsNullOrEmpty(col["NGAY_HIEULUC"]))
                {
                    if (regex.IsMatch(col["NGAY_HIEULUC"]))
                    {
                        VanBan.NGAY_HIEULUC = col["NGAY_HIEULUC"].ToDataTime();
                        if (!VanBan.NGAY_HIEULUC.HasValue)
                        {
                            commonError = new CommonError();
                            commonError.Field = "NGAY_HIEULUC";
                            commonError.Message = "Ngày hiệu lực không tồn tại";
                            ListError.Add(commonError);
                        }
                    }
                    else
                    {
                        commonError = new CommonError();
                        commonError.Field = "NGAY_HIEULUC";
                        commonError.Message = "Ngày hiệu lực không đúng định dạng";
                        ListError.Add(commonError);
                    }
                }
                if (!string.IsNullOrEmpty(col["NGAYHET_HIEULUC"]))
                {
                    if (regex.IsMatch(col["NGAYHET_HIEULUC"]))
                    {
                        VanBan.NGAYHET_HIEULUC = col["NGAYHET_HIEULUC"].ToDataTime();
                        if (!VanBan.NGAYHET_HIEULUC.HasValue)
                        {
                            commonError = new CommonError();
                            commonError.Field = "NGAYHET_HIEULUC";
                            commonError.Message = "Ngày hết hiệu lực không tồn tại";
                            ListError.Add(commonError);
                        }
                    }
                    else
                    {
                        commonError = new CommonError();
                        commonError.Field = "NGAYHET_HIEULUC";
                        commonError.Message = "Ngày hết hiệu lực không đúng định dạng";
                        ListError.Add(commonError);
                    }
                }
                if (!string.IsNullOrEmpty(col["NGAY_VANBAN"]))
                {
                    if (regex.IsMatch(col["NGAY_VANBAN"]))
                    {
                        VanBan.NGAY_VANBAN = col["NGAY_VANBAN"].ToDataTime();
                        if (!VanBan.NGAY_VANBAN.HasValue)
                        {
                            commonError = new CommonError();
                            commonError.Field = "NGAY_VANBAN";
                            commonError.Message = "Ngày văn bản không tồn tại";
                            ListError.Add(commonError);
                        }
                    }
                    else
                    {
                        commonError = new CommonError();
                        commonError.Field = "NGAY_VANBAN";
                        commonError.Message = "Ngày văn bản không đúng định dạng";
                        ListError.Add(commonError);
                    }
                }
                if (VanBan.NGAY_HIEULUC > VanBan.NGAYHET_HIEULUC)
                {
                    commonError = new CommonError();
                    commonError.Field = "NGAY_HIEULUC";
                    commonError.Message = "Ngày có hiệu lực phải nhỏ hơn hoặc bằng ngày hết hiệu lực";
                    ListError.Add(commonError);
                }
                if (!string.IsNullOrEmpty(col["NGAY_BANHANH"]))
                {
                    if (regex.IsMatch(col["NGAY_BANHANH"]))
                    {
                        VanBan.NGAY_BANHANH = col["NGAY_BANHANH"].ToDataTime();
                        if (!VanBan.NGAY_BANHANH.HasValue)
                        {
                            commonError = new CommonError();
                            commonError.Field = "NGAY_BANHANH";
                            commonError.Message = "Ngày ban hành không tồn tại";
                            ListError.Add(commonError);
                        }
                    }
                    else
                    {
                        commonError = new CommonError();
                        commonError.Field = "NGAY_BANHANH";
                        commonError.Message = "Ngày ban hành không đúng định dạng";
                        ListError.Add(commonError);
                    }
                }
                else
                {
                    commonError = new CommonError();
                    commonError.Field = "NGAY_BANHANH";
                    commonError.Message = "Bạn chưa chọn ngày ban hành";
                    ListError.Add(commonError);
                }
                //if (!string.IsNullOrEmpty(col["SOTHEOSO"]))
                //{
                //    SOTHEOSO = col["SOTHEOSO"].Trim();
                //}
                //if (!string.IsNullOrEmpty(col["YKIENCHIDAO"]))
                //{
                //    YKIENCHIDAO = col["YKIENCHIDAO"].Trim();
                //}
                #endregion
                VanBan.NGUOIKY = NGUOIKY;
                VanBan.CHUCVU = CHUCVU;
                VanBan.TRICHYEU = TRICHYEU;
                VanBan.NOIDUNG = NOIDUNG;
                VanBan.SOHIEU = SOHIEU;
                //VanBan.SOTHEOSO = SOTHEOSO;
                //VanBan.YKIENCHIDAO = YKIENCHIDAO;
                ListError.AddRange(IsValid(VanBan));
                if (ListError.Count > 0)
                {
                    return Json(new { Type = "INVALID", Message = ListError });
                }
                AssignUserInfo();
                HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

                UploadFileTool tool = new UploadFileTool();
                if (VanBan.ID > 0)
                {
                    #region Cập nhật văn bản
                    var result = HSCV_VANBANDENBusiness.Find(VanBan.ID);
                    if (result == null || currentUser.ID != result.NGUOITAO)
                    {
                        return Json(new { Type = "ERROR", Message = "Không tìm thấy văn bản phát hành cần cập nhật" });
                    }
                    result.CHUCVU = VanBan.CHUCVU;
                    result.DOKHAN_ID = VanBan.DOKHAN_ID;
                    result.DOMAT_ID = VanBan.DOMAT_ID;
                    //result.DOUUTIEN_ID = VanBan.DOUUTIEN_ID;
                    result.LINHVUCVANBAN_ID = VanBan.LINHVUCVANBAN_ID;
                    result.LOAIVANBAN_ID = VanBan.LOAIVANBAN_ID;
                    result.NGAYSUA = DateTime.Now;
                    result.NGUOIKY = VanBan.NGUOIKY;
                    result.NGUOISUA = currentUser.ID;
                    result.NOIDUNG = VanBan.NOIDUNG;
                    //result.SOBANSAO = VanBan.SOBANSAO;
                    result.SOHIEU = VanBan.SOHIEU;
                    //result.SOTHEOSO = VanBan.SOTHEOSO;
                    //result.SOTO = VanBan.SOTO;
                    result.SOTRANG = VanBan.SOTRANG;
                    result.TRICHYEU = VanBan.TRICHYEU;
                    //result.YKIENCHIDAO = VanBan.YKIENCHIDAO;
                    result.NGAY_BANHANH = VanBan.NGAY_BANHANH;
                    result.NGAY_HIEULUC = VanBan.NGAY_HIEULUC;
                    result.NGAY_VANBAN = VanBan.NGAY_VANBAN;
                    result.NGAYHET_HIEULUC = VanBan.NGAYHET_HIEULUC;
                    HSCV_VANBANDENBusiness.Save(result);
                    tool.UploadCustomFileVer3(filebase, true, VbDenExtension, URL_FOLDER, VbDenSize, FOLDER_ID, filename, result.ID, LOAITAILIEU.VANBANDEN, "Văn bản đến", currentUser);
                    return Json(new { Type = "SUCCESS", Message = "Cập nhật văn bản phát hành thành công" });
                    #endregion
                }
                else
                {
                    #region Thêm mới văn bản
                    VanBan.NGAYTAO = DateTime.Now;
                    VanBan.NGUOITAO = currentUser.ID;
                    VanBan.DONVI_ID = currentUser.DM_PHONGBAN_ID;
                    HSCV_VANBANDENBusiness.Save(VanBan);
                    tool.UploadCustomFileVer3(filebase, true, VbDenExtension, URL_FOLDER, VbDenSize, FOLDER_ID, filename, VanBan.ID, LOAITAILIEU.VANBANDEN, "Văn bản đến", currentUser);
                    #endregion
                }
                #region Thêm đơn vị nhận
                HSCV_VANBANDEN_DONVINHANBusiness = Get<HSCV_VANBANDEN_DONVINHANBusiness>();
                var ListDonVi = HSCV_VANBANDEN_DONVINHANBusiness.GetData(VanBan.ID);
                foreach (var item in ListDonVi)
                {
                    HSCV_VANBANDEN_DONVINHANBusiness.repository.Delete(item.ID);
                }
                HSCV_VANBANDEN_DONVINHANBusiness.Save();
                var selectedDept = col["department-choose"];
                if (!string.IsNullOrEmpty(selectedDept))
                {
                    var Ids = selectedDept.ToListInt(',');
                    foreach (var item in Ids)
                    {
                        HSCV_VANBANDEN_DONVINHAN DonVi = new HSCV_VANBANDEN_DONVINHAN();
                        DonVi.DONVI_ID = item;
                        DonVi.VANBANDEN_ID = VanBan.ID;
                        HSCV_VANBANDEN_DONVINHANBusiness.Save(DonVi);
                    }
                }
                #endregion
                return Json(new { Type = "SUCCESS", Message = "Thêm mới văn bản phát hành thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Type = "ERROR", Message = "Đã có lỗi xảy ra. Vui lòng kiểm tra lại" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            var searchModel = SessionManager.GetValue("VanBanDenSearch") as HSCV_VANBANDEN_SEARCH;

            if (searchModel == null)
            {
                searchModel = new HSCV_VANBANDEN_SEARCH();
                searchModel.pageSize = MaxPerpage;
            }
            int DONVI_ID = form["DONVI_ID"].ToIntOrZero();
            if (DONVI_ID > 0)
            {
                AssignUserInfo();
                CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                List<int> Ids = new List<int>();
                if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.DONVI))
                {
                    CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                    Ids = new List<int>();
                    if (DonVi != null && DonVi.PARENT_ID.HasValue && DonVi.PARENT_ID.Value > 0)
                    {
                        Ids = CCTC_THANHPHANBusiness.GetDSChild(DonVi.PARENT_ID.Value).Select(x => x.ID).ToList();
                        Ids.Add(DonVi.PARENT_ID.Value);
                    }
                    else
                    {
                        Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                        Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                    }
                }
                else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.PHONGBAN))
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0).Select(x => x.ID).ToList();
                    Ids.Add(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                }
                if (Ids.Contains(DONVI_ID))
                {
                    Ids = CCTC_THANHPHANBusiness.GetDSChild(DONVI_ID).Select(x => x.ID).ToList();
                    Ids.Add(DONVI_ID);
                    searchModel.ListDonVi = Ids;
                }
                else
                {
                    searchModel.ListDonVi = Ids;
                }
            }
            searchModel.SOHIEU = form["SOHIEU"];
            searchModel.TRICHYEU = form["TRICHYEU"];
            searchModel.DOKHAN_ID = form["DOKHAN_ID"].ToIntOrNULL();
            searchModel.DOMAT_ID = form["DOMAT_ID"].ToIntOrNULL();
            searchModel.LINHVUCVANBAN_ID = form["LINHVUCVANBAN_ID"].ToIntOrNULL();
            searchModel.LOAIVANBAN_ID = form["LOAIVANBAN_ID"].ToIntOrNULL();
            searchModel.NGUOIKY = form["NGUOIKY"];
            searchModel.NGAYBANHANH_TU = form["BANHANH_TU"].ToDateTime();
            searchModel.NGAYBANHANH_DEN = form["BANHANH_DEN"].ToDateTime();
            SessionManager.SetValue("VanBanDenSearch", searchModel);
            var data = HSCV_VANBANDENBusiness.GetDaTaByPage(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            var searchModel = SessionManager.GetValue("VanBanDenSearch") as HSCV_VANBANDEN_SEARCH;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new HSCV_VANBANDEN_SEARCH();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("VanBanDenSearch", searchModel);
            }
            var data = HSCV_VANBANDENBusiness.GetDaTaByPage(searchModel, pageSize, indexPage);
            return Json(data);
        }
        public JsonResult Delete(long id)
        {
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            HSCV_VANBANDEN VanBan = HSCV_VANBANDENBusiness.Find(id);
            AssignUserInfo();
            if (VanBan == null || currentUser.ID != VanBan.NGUOITAO)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy văn bản phát hành cần xóa" });
            }
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            FileUltilities file = new FileUltilities();
            List<TAILIEUDINHKEM> ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.VANBANDEN);
            foreach (var item in ListTaiLieu)
            {
                TAILIEUDINHKEMBusiness.repository.Delete(item.TAILIEU_ID);
                file.RemoveFile(URL_FOLDER + item.DUONGDAN_FILE);
                if (!string.IsNullOrEmpty(item.PDF_VERSION))
                {
                    file.RemoveFile(URL_FOLDER + item.PDF_VERSION);
                }
            }
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.GetDataByNam(id.ToString(), ThuMucLuuTruConstant.DefaultVbDen);
            if (ThuMuc != null)
            {
                ThuMuc.IS_DELETE = true;
                THUMUC_LUUTRUBusiness.Save(ThuMuc);
            }
            TAILIEUDINHKEMBusiness.Save();
            HSCV_VANBANDENBusiness.repository.Delete(id);
            HSCV_VANBANDENBusiness.Save();
            #region Xóa đơn vị nhận văn bản
            HSCV_VANBANDEN_DONVINHANBusiness = Get<HSCV_VANBANDEN_DONVINHANBusiness>();
            var ListDonVi = HSCV_VANBANDEN_DONVINHANBusiness.GetData(id);
            if (ListDonVi.Any())
            {
                foreach (var item in ListDonVi)
                {
                    HSCV_VANBANDEN_DONVINHANBusiness.repository.Delete(item.ID);
                }
                HSCV_VANBANDEN_DONVINHANBusiness.Save();
            }
            #endregion
            return Json(new { Type = "SUCCESS", Message = "Xóa văn bản phát hành thành công" });
        }
        #endregion
        #region Các hàm khác
        public PartialViewResult ShowDepartment(long id)
        {
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            HSCV_VANBANDEN VanBan = HSCV_VANBANDENBusiness.Find(id);
            if (VanBan == null)
            {
                VanBan = new HSCV_VANBANDEN();
            }
            DonViNhanModel model = new DonViNhanModel();
            HSCV_VANBANDEN_DONVINHANBusiness = Get<HSCV_VANBANDEN_DONVINHANBusiness>();
            model.VanBan = VanBan;
            model.ListDonVi = HSCV_VANBANDEN_DONVINHANBusiness.GetDataBO(id);
            return PartialView("_ShowDepartment", model);
        }
        #endregion
    }
}