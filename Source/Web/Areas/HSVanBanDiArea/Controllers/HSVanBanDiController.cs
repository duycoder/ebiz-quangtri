using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.HSCVVANBANDI;
using CommonHelper;
using CommonHelper.DateExtend;
using CommonHelper.Excel;
using Model.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.HSVanBanDiArea.Models;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Common;
using Web.Common.Elastic;
using Web.Custom;
using Web.Filter;
using Web.FwCore;
using Web.Models;

namespace Web.Areas.HSVanBanDiArea.Controllers
{
    public class HSVanBanDiController : BaseController
    {
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private WF_STATEBusiness WF_STATEBusiness;
        private HSCV_VANBANDENBusiness HSCV_VANBANDENBusiness;
        private DM_VAITROBusiness DM_VAITROBusiness;
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private WF_PROCESSBusiness WF_PROCESSBusiness;
        private WF_ITEM_USER_PROCESSBusiness WF_ITEM_USER_PROCESSBusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;

        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private THUMUC_LUUTRUBusiness THUMUC_LUUTRUBusiness;
        private HSCVREADVANBANBusiness HSCVREADVANBANBusiness;
        private SYS_TINNHANBusiness SYS_TINNHANBusiness;
        private QL_NGUOINHAN_VANBANBusiness QL_NGUOINHAN_VANBANBusiness;
        private LogSMSBusiness LogSMSBusiness;
        private WF_MODULEBusiness WF_MODULEBusiness;
        private WF_STREAMBusiness WF_STREAMBusiness;
        private DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;


        private string ROLE_LANHDAODONVI = WebConfigurationManager.AppSettings["ROLE_LANHDAODONVI"];
        private string CODE_ROLE_LANHDAODONVI = WebConfigurationManager.AppSettings["CODE_ROLE_LANHDAODONVI"];
        private string CODE_ROLE_BANTONGGIAMDOC = WebConfigurationManager.AppSettings["CODE_ROLE_BANTONGGIAMDOC"];
        private int itemsPerPage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);

        private string VbTrinhKyExtension = WebConfigurationManager.AppSettings["VbDenExtension"];
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private int VbTrinhKySize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private string SERVERADDRESS = WebConfigurationManager.AppSettings["SERVERADDRESS"];
        private string DEPTTYLELABEL = WebConfigurationManager.AppSettings["DEPTTYLELABEL"];
        // GET: HSVanBanDiArea/HSVanBanDi
        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            VanBanDiVM viewModel = this.GetVanBanDiViewModel(VANBANDI_CONSTANT.TAT_CA);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi chưa xử lý
        /// </summary>
        /// <returns></returns>
        public ActionResult ChuaXuLy()
        {
            VanBanDiVM viewModel = this.GetVanBanDiViewModel(VANBANDI_CONSTANT.CHUA_XULY);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi đã xử lý
        /// </summary>
        /// <returns></returns>
        public ActionResult DaXuLy()
        {
            VanBanDiVM viewModel = this.GetVanBanDiViewModel(VANBANDI_CONSTANT.DA_XULY);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi tham gia xử lý
        /// </summary>
        /// <returns></returns>
        public ActionResult ThamGiaXuLy()
        {
            VanBanDiVM viewModel = this.GetVanBanDiViewModel(VANBANDI_CONSTANT.THAMGIA_XULY);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi tham gia xử lý
        /// </summary>
        /// <returns></returns>
        public ActionResult NoiBo()
        {
            VanBanDiVM viewModel = this.GetVanBanDiViewModel(VANBANDI_CONSTANT.NOIBO);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi đã ban hành
        /// </summary>
        /// <returns></returns>
        public ActionResult DaBanHanh()
        {
            VanBanDiVM viewModel = this.GetVanBanDiViewModel(VANBANDI_CONSTANT.DA_BANHANH);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy văn bản đi review
        /// </summary>
        /// <param name="type">loại văn bản</param>
        /// <returns></returns>
        private VanBanDiVM GetVanBanDiViewModel(int type)
        {
            AssignUserInfo();

            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();

            VanBanDiVM viewModel = new VanBanDiVM();
            viewModel.LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, 0);
            viewModel.LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            viewModel.LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
            viewModel.LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
            int dept = currentUser.DeptParentID.HasValue ? currentUser.DeptParentID.Value : 0;
            viewModel.LstSoVanBanDi = DM_DANHMUC_DATABusiness.DsByMaNhomByDept(DMLOAI_CONSTANT.SOVANBANDI, 0, dept);

            viewModel.docType = type;
            viewModel.UserInfoBO = currentUser;

            HSCV_VANBANDI_SEARCH searchModel = new HSCV_VANBANDI_SEARCH();
            searchModel.USER_ID = currentUser.ID;
            searchModel.pageSize = itemsPerPage;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            string searchSessionName = string.Empty;
            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    searchSessionName = "SessionVanBanDiChuaXuLy";
                    viewModel.listTitle = "Quản lý văn bản đi chưa xử lý";
                    viewModel.ListResult = HSCV_VANBANDIBusiness.GetListProcessing(searchModel, itemsPerPage);
                    break;
                case VANBANDI_CONSTANT.DA_XULY:
                    searchSessionName = "SessionVanBanDiDaXuLy";
                    viewModel.listTitle = "Quản lý văn bản đi đã xử lý";
                    viewModel.ListResult = HSCV_VANBANDIBusiness.GetListProcessed(searchModel, itemsPerPage);
                    break;
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    searchSessionName = "SessionVanBanDiThamGiaXuLy";
                    viewModel.listTitle = "Quản lý văn bản đi tham gia xử lý";
                    viewModel.ListResult = HSCV_VANBANDIBusiness.GetListJoinProcessing(searchModel, itemsPerPage);
                    break;
                case VANBANDI_CONSTANT.DA_BANHANH:
                    searchSessionName = "SessionVanBanDiDaBanHanh";
                    viewModel.listTitle = "Quản lý văn bản đi đã ban hành";
                    viewModel.ListResult = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.GetValueOrDefault(), itemsPerPage);
                    break;
                case VANBANDI_CONSTANT.NOIBO:
                    searchSessionName = "SessionVanBanDiNoiBo";
                    viewModel.listTitle = "Quản lý văn bản đi nội bộ";
                    viewModel.ListResult = HSCV_VANBANDIBusiness.GetListInternal(searchModel, itemsPerPage);
                    break;
                default:
                    searchSessionName = "SessionVanBanDi";
                    viewModel.listTitle = "Quản lý văn bản đi";
                    viewModel.ListResult = HSCV_VANBANDIBusiness.GetDaTaByPage(searchModel, itemsPerPage);
                    break;
            }
            SessionManager.SetValue(searchSessionName, searchModel);
            return viewModel;
        }

        /// <summary>
        /// @author: duynn
        /// @description: phân trang
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="sortQuery"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetData(int type, int index, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            string searchSessionName = string.Empty;
            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    searchSessionName = "SessionVanBanDiChuaXuLy";
                    break;
                case VANBANDI_CONSTANT.DA_XULY:
                    searchSessionName = "SessionVanBanDiDaXuLy";
                    break;
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    searchSessionName = "SessionVanBanDiThamGiaXuLy";
                    break;
                case VANBANDI_CONSTANT.DA_BANHANH:
                    searchSessionName = "SessionVanBanDiDaBanHanh";
                    break;
                case VANBANDI_CONSTANT.NOIBO:
                    searchSessionName = "SessionVanBanDiNoiBo";
                    break;
                default:
                    searchSessionName = "SessionVanBanDi";
                    break;
            }

            HSCV_VANBANDI_SEARCH searchModel = (HSCV_VANBANDI_SEARCH)SessionManager.GetValue(searchSessionName);
            if (searchModel == null)
            {
                searchModel = new HSCV_VANBANDI_SEARCH();
                searchModel.USER_ID = currentUser.ID;
            }
            searchModel.sortQuery = sortQuery;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            PageListResultBO<HSCV_VANBANDI_BO> result;
            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    result = HSCV_VANBANDIBusiness.GetListProcessing(searchModel, pageSize, index);
                    break;
                case VANBANDI_CONSTANT.DA_XULY:
                    result = HSCV_VANBANDIBusiness.GetListProcessed(searchModel, pageSize, index);
                    break;
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    result = HSCV_VANBANDIBusiness.GetListJoinProcessing(searchModel, pageSize, index);
                    break;
                case VANBANDI_CONSTANT.DA_BANHANH:
                    result = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.GetValueOrDefault(), pageSize, index);
                    break;
                case VANBANDI_CONSTANT.NOIBO:
                    result = HSCV_VANBANDIBusiness.GetListInternal(searchModel, pageSize, index);
                    break;
                default:
                    result = HSCV_VANBANDIBusiness.GetDaTaByPage(searchModel, pageSize, index);
                    break;
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: tìm kiếm danh sách văn bản đi
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchData(FormCollection form)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();

            HSCV_VANBANDI_SEARCH searchModel = new HSCV_VANBANDI_SEARCH();
            int type = form["DOC_TYPE"].ToIntOrZero();
            if (type != VANBANDI_CONSTANT.CHUA_XULY && type != VANBANDI_CONSTANT.NOIBO)
            {
                searchModel.SOHIEU = form["SOHIEU"];
                searchModel.SOVANBAN_ID = form["SOVANBANDI_ID"].ToIntOrNULL();
            }
            searchModel.TRICHYEU = form["TRICHYEU"];
            searchModel.DOKHAN_ID = form["DOKHAN_ID"].ToIntOrNULL();
            searchModel.DOUUTIEN_ID = form["DOUUTIEN_ID"].ToIntOrNULL();
            searchModel.LINHVUCVANBAN_ID = form["LINHVUCVANBAN_ID"].ToIntOrNULL();
            searchModel.LOAIVANBAN_ID = form["LOAIVANBAN_ID"].ToIntOrNULL();
            searchModel.NGAYBANHANH_TU = form["BANHANHTUNGAY"].ToDateTime();
            searchModel.NGAYBANHANH_DEN = form["BANHANHDENNGAY"].ToDateTime();
            searchModel.NGAYHIEULUC_TU = form["HIEULUCTUNGAY"].ToDateTime();
            searchModel.NGAYHIEULUC_DEN = form["HIEULUCDENNGAY"].ToDateTime();
            searchModel.NGAYHETHIEULUC_TU = form["HETHIEULUCTUNGAY"].ToDateTime();
            searchModel.NGAYHETHIEULUC_DEN = form["HETHIEULUCDENNGAY"].ToDateTime();
            searchModel.NGAYTAO_TU = form["NGAYTAO_TU"].ToDateTime();
            searchModel.NGAYTAO_DEN = form["NGAYTAO_DEN"].ToDateTime();

            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            searchModel.USER_ID = currentUser.ID;

            string searchSessionName = string.Empty;

            PageListResultBO<HSCV_VANBANDI_BO> result = new PageListResultBO<HSCV_VANBANDI_BO>();
            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    searchSessionName = "SessionVanBanDiChuaXuLy";
                    break;
                case VANBANDI_CONSTANT.DA_XULY:
                    searchSessionName = "SessionVanBanDiDaXuLy";
                    break;
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    searchSessionName = "SessionVanBanDiThamGiaXuLy";
                    break;
                case VANBANDI_CONSTANT.DA_BANHANH:
                    searchSessionName = "SessionVanBanDiDaBanHanh";
                    break;
                case VANBANDI_CONSTANT.NOIBO:
                    searchSessionName = "SessionVanBanDiNoiBo";
                    break;
                default:
                    searchSessionName = "SessionVanBanDi";
                    break;
            }
            if (!string.IsNullOrEmpty(searchSessionName))
            {
                SessionManager.SetValue(searchSessionName, searchModel);
                switch (type)
                {
                    case VANBANDI_CONSTANT.CHUA_XULY:
                        result = HSCV_VANBANDIBusiness.GetListProcessing(searchModel);
                        break;
                    case VANBANDI_CONSTANT.DA_XULY:
                        result = HSCV_VANBANDIBusiness.GetListProcessed(searchModel);
                        break;
                    case VANBANDI_CONSTANT.THAMGIA_XULY:
                        result = HSCV_VANBANDIBusiness.GetListJoinProcessing(searchModel);
                        break;
                    case VANBANDI_CONSTANT.DA_BANHANH:
                        result = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.GetValueOrDefault());
                        break;
                    case VANBANDI_CONSTANT.NOIBO:
                        result = HSCV_VANBANDIBusiness.GetListInternal(searchModel);
                        break;
                    default:
                        result = HSCV_VANBANDIBusiness.GetDaTaByPage(searchModel);
                        break;
                }
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description:tạo văn bản đi đã được phát hành
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult CreatePhatHanhVB()
        {
            return RedirectToAction("CreateVB", new { isAllowPublish = true });
        }

        /// <summary>
        /// @author: duynn
        /// @description:tạo văn bản đi phát hành nội bộ
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult CreateNoiBoVB()
        {
            return RedirectToAction("CreateVB", new { isInternal = true });
        }

        /// <summary>
        /// @author:duynn
        /// @description: thêm mới văn bản đến
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idVanBanDen">mã văn bản đến</param>
        /// <param name="docType">loại văn bản để trở về trang danh sách</param>
        /// <param name="isAllowPublish">đã phát hành chưa</param>
        /// <param name="isInternal">có phải là nội bộ không</param>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult CreateVB(long id = 0, long idVanBanDen = 0, int docType = VANBANDI_CONSTANT.CHUA_XULY, bool isAllowPublish = false, bool isInternal = false)
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            QL_NGUOINHAN_VANBANBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            VanBanDiVM model = new VanBanDiVM();
            model.IsAllowPublish = isAllowPublish;
            model.IsInternal = isInternal;
            model.docType = docType;
            var roleLanhDaoDonVi = DM_VAITROBusiness.repository.All().Where(x => x.MA_VAITRO == CODE_ROLE_LANHDAODONVI).FirstOrDefault();
            int LANHDAODONVI = ROLE_LANHDAODONVI.ToIntOrZero();

            if (roleLanhDaoDonVi != null)
            {
                LANHDAODONVI = roleLanhDaoDonVi.DM_VAITRO_ID;
            }
            if (currentUser.DeptParentID == null)
            {
                currentUser.DeptParentID = 0;
            }
            model.LstNguoiKyVanBan = DM_NGUOIDUNGBusiness.GetUserByRoleAndParentDept(LANHDAODONVI, currentUser.DeptParentID.Value);
            model.TreeDonVi = CCTC_THANHPHANBusiness.GetTreeLabel(currentUser);
            //lấy danh sách người dùng có thể nhận trực tiếp
            model.UsersReceived = new List<long>();
            model.Recipients = QL_NGUOINHAN_VANBANBusiness.GetRecipientGroups(currentUser.DeptParentID.GetValueOrDefault());
            //hiển thị người nhận văn bản

            if (model.IsInternal)
            {
                List<DM_NGUOIDUNG_BO> usersReceiveInternal = DM_NGUOIDUNGBusiness.GetUsersReceiveInternal(currentUser);
                model.GroupUsersReceiveInternal = usersReceiveInternal.Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.HOTEN
                }).ToList();
                model.GroupUserIdsReceiveInternal = new List<long>();
            }
            model.LstReceiveDirectly = new List<SelectListItem>();
            model.GroupUserIdsReceiveDirectly = new List<long>();
            HSCV_VANBANDI VanBan = new HSCV_VANBANDI();
            if (id > 0)
            {
                HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                VanBan = HSCV_VANBANDIBusiness.Find(id);
                if (VanBan == null || currentUser.ID != VanBan.CREATED_BY)
                {
                    return Redirect("/Home/UnAuthor");
                }
                if (!string.IsNullOrEmpty(VanBan.DONVINHAN_INTERNAL_ID))
                {
                    model.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(VanBan.DONVINHAN_INTERNAL_ID.ToListInt(','));
                }
                model.LstNguoiKyVanBan = model.LstNguoiKyVanBan.Select(
                    x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value,
                        Selected = x.Value == (VanBan.NGUOIKY_ID == null ? "" : VanBan.NGUOIKY_ID.ToString())
                    }).ToList();
                if (!string.IsNullOrEmpty(VanBan.USER_RECEIVE_DIRECTLY))
                {
                    model.GroupUserIdsReceiveDirectly = VanBan.USER_RECEIVE_DIRECTLY.ToListLong(',');
                    model.LstReceiveDirectly = DM_NGUOIDUNGBusiness.GetDanhSachByListIds(model.GroupUserIdsReceiveDirectly);
                }
            }
            else
            {
                if (idVanBanDen > 0)
                {
                    HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
                    var TmpVanBanDen = HSCV_VANBANDENBusiness.Find(idVanBanDen);
                    if (TmpVanBanDen != null)
                    {
                        model.VanBanDen = TmpVanBanDen;
                    }
                }
            }

            if (model.ListDonVi == null)
            {
                model.ListDonVi = new List<CCTC_THANHPHAN>();
            }
            model.LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.DOUUTIEN, currentUser.ID, VanBan.DOUUTIEN_ID.HasValue ? VanBan.DOUUTIEN_ID.Value : 0);
            model.LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.DOQUANTRONG, currentUser.ID, VanBan.DOKHAN_ID.HasValue ? VanBan.DOKHAN_ID.Value : 0);
            model.LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LINHVUCVANBAN, currentUser.ID, VanBan.LINHVUCVANBAN_ID.HasValue ? VanBan.LINHVUCVANBAN_ID.Value : 0);
            model.LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LOAIVANBAN, currentUser.ID, VanBan.LOAIVANBAN_ID.HasValue ? VanBan.LOAIVANBAN_ID.Value : 0);
            model.GroupDeptTypes = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LOAI_COQUAN, currentUser.ID, VanBan.LOAI_COQUAN_ID.GetValueOrDefault());
            model.GroupDocTypes = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.PHANLOAI_VANBAN, currentUser.ID, VanBan.THONGTIN_LOAI_ID.GetValueOrDefault());
            model.GroupDocAuthors = DM_NGUOIDUNGBusiness.GetDanhSachTacGia(currentUser.DeptParentID.Value, VanBan.TACGIA_ID.GetValueOrDefault());
            model.LstSoVanBanDi =  DM_DANHMUC_DATABusiness.DsByMaNhomByDept(VanBanConstant.SOVANBANDI, 0, currentUser.DeptParentID.Value);
            model.VanBan = VanBan;
            if (id > 0)
            {
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                model.ListTaiLieu = TAILIEUDINHKEMBusiness.GetNewestData(id, LOAITAILIEU.VANBAN);
            }
            else
            {
                model.ListTaiLieu = new List<TAILIEUDINHKEM>();
            }
            return View(model);
        }

        [HttpPost]
        public PartialViewResult GetTreeDonVi(long idVanBanDi = 0)
        {
            AssignUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();

            VanBanDiVM model = new VanBanDiVM();
            if (idVanBanDi == 0)
            {
                model.TreeDonVi = CCTC_THANHPHANBusiness.GetTreeLabel(currentUser) ?? new CCTCItemTreeBO();
            }
            else
            {
                //myModel.VanBanTrinhKy = HSCV_VANBANDIBusiness.Find(ID);
                model.VanBan = HSCV_VANBANDIBusiness.Find(idVanBanDi);
                if (!string.IsNullOrEmpty(model.VanBan.DONVINHAN_INTERNAL_ID))
                {
                    model.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(model.VanBan.DONVINHAN_INTERNAL_ID.ToListInt(','));
                }
                else
                {
                    model.ListDonVi = new List<CCTC_THANHPHAN>();
                }

                //lấy danh sách các đơn vị chưa được gửi
                var idsDonViDaNhan = model.ListDonVi.Select(x => x.ID).ToList();
                model.TreeDonVi = CCTC_THANHPHANBusiness.GetTreeLabel(currentUser, idsDonViDaNhan);
                if (model.TreeDonVi == null)
                {
                    model.TreeDonVi = new CCTCItemTreeBO();
                }
            }

            return PartialView("_ChonDonVi", model.TreeDonVi);
        }

        public string GetChucVuNguoiKy(long id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var NguoiDungOBJ = DM_NGUOIDUNGBusiness.repository.Find(id);
            if (NguoiDungOBJ != null && NguoiDungOBJ.CHUCVU_ID != null)
            {
                var ChucVu = DM_DANHMUC_DATABusiness.GetDaTaByID(NguoiDungOBJ.CHUCVU_ID.Value);
                return ChucVu.TEXT;
            }
            return "";
        }

        /// <summary>
        /// Hàm tạo mới văn bản đi
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [ActionAudit]
        public ActionResult InsertVanBanDi(HSCV_VANBANDI VanBanDi, FormCollection form, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            AssignUserInfo();

            SMSDAL.SendSMSDAL sms = new SMSDAL.SendSMSDAL();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            LogSMSBusiness = Get<LogSMSBusiness>();

            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();

            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();

            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

            #region Check và tạo mới văn bản đi
            var STR_SOHIEU = string.Empty;
            bool HAS_SIGNED = false;
            bool ALLOW_PUBLISH = false;
            List<long> USERS_RECEIVE_INTERNAL = new List<long>();
            if (!string.IsNullOrEmpty(form["SOHIEU"]))
            {
                STR_SOHIEU = form["SOHIEU"].Trim();
            }
            var STR_TRICHYEU = string.Empty;
            if (!string.IsNullOrEmpty(form["TRICHYEU"]))
            {
                STR_TRICHYEU = form["TRICHYEU"].Trim();
            }
            var STR_CHUCVUNGUOIKY = string.Empty;
            if (!string.IsNullOrEmpty(form["CHUCVUNGUOIKY"]))
            {
                STR_CHUCVUNGUOIKY = form["CHUCVUNGUOIKY"].Trim();
            }
            var NOIDUNGVANBAN = string.Empty;
            if (!string.IsNullOrEmpty(form["NOIDUNGVANBAN"]))
            {
                NOIDUNGVANBAN = form["NOIDUNGVANBAN"].Trim();
            }
            if (!string.IsNullOrEmpty(form["HAS_SIGNED"]))
            {
                HAS_SIGNED = form["HAS_SIGNED"].Trim().ToIntOrZero() == 1;
            }

            //cho phép văn thư/chuyên viên có thể phát hành sau khi tạo mới
            if (!string.IsNullOrEmpty(form["ALLOW_PUBLISH"]))
            {
                ALLOW_PUBLISH = form["ALLOW_PUBLISH"].Trim().ToIntOrZero() == 1;
            }

            if (!string.IsNullOrEmpty(form["USERS_RECEIVE_INTERNAL"]))
            {
                USERS_RECEIVE_INTERNAL = form["USERS_RECEIVE_INTERNAL"].Trim().ToListLong(',');
            }

            var LOAIVANBAN_ID = form["LOAIVANBAN_ID"].ToIntOrZero();
            var LINHVUCVANBAN_ID = form["LINHVUCVANBAN_ID"].ToIntOrZero();
            var DOKHAN_ID = form["DOKHAN_ID"].ToIntOrZero();
            var DOUUTIEN_ID = form["DOUUTIEN_ID"].ToIntOrZero();
            var NGUOIKY_ID = form["TENNGUOIKY"].ToLongOrZero();
            var NOI_NHAN = form["NOI_NHAN"];
            var MA_DANGKY = form["MA_DANGKY"];
            var LOAI_COQUAN_ID = form["LOAI_COQUAN_ID"].ToIntOrNULL();
            var THONGTIN_LOAI_ID = form["THONGTIN_LOAI_ID"].ToIntOrNULL();
            var TACGIA_ID = form["TACGIA_ID"].ToIntOrNULL();
            var selectedDept = form["department-choose"];
            var usersReceiveDirectly = form["USERS_RECEIVE_SPECIAL"];
            #region Validate dữ liệu và gán giá trị
            VanBanDi.TRICHYEU = STR_TRICHYEU;
            VanBanDi.SOHIEU = STR_SOHIEU;
            VanBanDi.CHUCVU = STR_CHUCVUNGUOIKY;
            VanBanDi.NOIDUNG = NOIDUNGVANBAN;
            VanBanDi.NOI_NHAN = NOI_NHAN;
            VanBanDi.MA_DANGKY = MA_DANGKY;
            VanBanDi.LOAI_COQUAN_ID = LOAI_COQUAN_ID;
            VanBanDi.THONGTIN_LOAI_ID = THONGTIN_LOAI_ID;
            VanBanDi.TACGIA_ID = TACGIA_ID;
            VanBanDi.CAN_SEND_SMS = form["CAN_SEND_SMS"].Trim().ToIntOrZero() > 0;
            #endregion
            List<CommonError> ListError = IsValid(VanBanDi);
            if (ListError.Any())
            {
                return RedirectToAction("ChuaXuLy");
            }
            List<long> ListUser = new List<long>();
            ListUser.Add(currentUser.ID);
            if (!string.IsNullOrEmpty(STR_TRICHYEU))
            {
                try
                {
                    UploadFileTool tool = new UploadFileTool();
                    HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                    if (VanBanDi.ID > 0)
                    {
                        #region Cập nhật văn bản trình ký
                        var result = HSCV_VANBANDIBusiness.Find(VanBanDi.ID);
                        if (result == null || currentUser.ID != result.CREATED_BY)
                        {
                            return RedirectToAction("NotFound", "Home", new { area = "" });
                        }
                        result.CHUCVU = STR_CHUCVUNGUOIKY;
                        result.DOKHAN_ID = DOKHAN_ID;
                        result.DONVINHAN_EXTERNAL_ID = VanBanDi.DONVINHAN_EXTERNAL_ID;
                        result.DONVINHAN_INTERNAL_ID = selectedDept;
                        result.USER_RECEIVE_DIRECTLY = usersReceiveDirectly;
                        result.DONVINHAN_NGOAIHETHONG = VanBanDi.DONVINHAN_NGOAIHETHONG;
                        result.DONVISOANTHAO_ID = VanBanDi.DONVISOANTHAO_ID;
                        result.DOUUTIEN_ID = DOUUTIEN_ID;
                        result.LANBANHANH = VanBanDi.LANBANHANH;
                        result.LINHVUCVANBAN_ID = LINHVUCVANBAN_ID;
                        result.LOAIVANBAN_ID = LOAIVANBAN_ID;
                        result.NGAYBANHANH = VanBanDi.NGAYBANHANH;
                        result.NGAYCOHIEULUC = VanBanDi.NGAYCOHIEULUC;
                        result.NGAYHETHIEULUC = VanBanDi.NGAYHETHIEULUC;
                        result.NGAYVANBAN = VanBanDi.NGAYVANBAN;
                        result.NGUOIKY_ID = NGUOIKY_ID;
                        result.NOIDUNG = NOIDUNGVANBAN;
                        result.SOBANSAO = VanBanDi.SOBANSAO;
                        result.SOHIEU = STR_SOHIEU;
                        result.SOTHEOSO = VanBanDi.SOTHEOSO;
                        result.SOTO = VanBanDi.SOTO;
                        result.SOTRANG = VanBanDi.SOTRANG;
                        result.SOVANBAN_ID = VanBanDi.SOVANBAN_ID;
                        result.THOIHANHOIBAO = VanBanDi.THOIHANHOIBAO;
                        result.THOIHANXULY = VanBanDi.THOIHANXULY;
                        result.TRICHYEU = STR_TRICHYEU;
                        result.UPDATED_AT = DateTime.Now;
                        result.UPDATED_BY = currentUser.ID;
                        result.YKIENCHIDAO = VanBanDi.YKIENCHIDAO;
                        result.HAS_SIGNED = HAS_SIGNED;
                        result.NOI_NHAN = VanBanDi.NOI_NHAN;
                        result.MA_DANGKY = VanBanDi.MA_DANGKY;
                        result.LOAI_COQUAN_ID = VanBanDi.LOAI_COQUAN_ID;
                        result.THONGTIN_LOAI_ID = VanBanDi.THONGTIN_LOAI_ID;
                        result.TACGIA_ID = VanBanDi.TACGIA_ID;
                        result.CAN_SEND_SMS = VanBanDi.CAN_SEND_SMS;
                        HSCV_VANBANDIBusiness.Save(result);
                        #endregion
                        bool isSave = tool.UploadFiles(filebase, VbTrinhKyExtension.Split(',').ToList(), URL_FOLDER, filename, result.ID, LOAITAILIEU.VANBAN, VbTrinhKySize, currentUser);
                        if (isSave)
                        {
                            //cập nhật tài liệu gốc cho các tài liệu đính kèm mới
                            List<TAILIEUDINHKEM> attachments = TAILIEUDINHKEMBusiness.GetInsertedData(result.ID, LOAITAILIEU.VANBAN);
                            attachments.ForEach(x =>
                            {
                                x.TAILIEU_GOC_ID = x.TAILIEU_ID;
                                TAILIEUDINHKEMBusiness.Save(x);
                            });
                        }

                        DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                        DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(VanBanDi.NGUOIKY_ID);
                        ElasticModel model = ElasticModel.ConvertVanBan(result, ListUser, NguoiDung != null ? NguoiDung.HOTEN : "");
                        ElasticSearch.updateDocument(model, model.Id.ToString(), ElasticType.VanBanDi);
                    }
                    else
                    {
                        HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
                        var TmpVanBanDen = new HSCV_VANBANDEN();
                        if (!string.IsNullOrEmpty(form["VANBANDENLIENQUAN"]))
                        {
                            var VANBANDENLIENQUAN = form["VANBANDENLIENQUAN"].ToLongOrZero();
                            if (VANBANDENLIENQUAN > 0)
                            {
                                TmpVanBanDen = HSCV_VANBANDENBusiness.Find(VANBANDENLIENQUAN);
                                if (TmpVanBanDen != null)
                                {
                                    VanBanDi.VANBANDEN_ID = TmpVanBanDen.ID;
                                }
                            }
                        }

                        #region Thêm mới văn bản trình ký
                        VanBanDi.SOHIEU = STR_SOHIEU;
                        VanBanDi.TRICHYEU = STR_TRICHYEU;
                        VanBanDi.DOUUTIEN_ID = DOUUTIEN_ID;
                        VanBanDi.DOKHAN_ID = DOKHAN_ID;
                        VanBanDi.LOAIVANBAN_ID = LOAIVANBAN_ID;
                        VanBanDi.LINHVUCVANBAN_ID = LINHVUCVANBAN_ID;
                        VanBanDi.NGUOIKY_ID = NGUOIKY_ID;
                        VanBanDi.CHUCVU = STR_CHUCVUNGUOIKY;
                        VanBanDi.DONVINHAN_INTERNAL_ID = selectedDept;
                        VanBanDi.USER_RECEIVE_DIRECTLY = usersReceiveDirectly;
                        VanBanDi.CREATED_AT = DateTime.Now;
                        VanBanDi.CREATED_BY = currentUser.ID;
                        VanBanDi.HAS_SIGNED = HAS_SIGNED;
                        VanBanDi.DEPTID = currentUser.DeptParentID;
                        VanBanDi.NOI_NHAN = NOI_NHAN;
                        VanBanDi.IS_INTERNAL = USERS_RECEIVE_INTERNAL.Any();
                        VanBanDi.MA_DANGKY = MA_DANGKY;
                        VanBanDi.LOAI_COQUAN_ID = LOAI_COQUAN_ID;
                        VanBanDi.THONGTIN_LOAI_ID = THONGTIN_LOAI_ID;
                        VanBanDi.TACGIA_ID = TACGIA_ID;
                        HSCV_VANBANDIBusiness.Save(VanBanDi);
                        if (TmpVanBanDen.ID > 0)
                        {
                            TmpVanBanDen.VANBANLIENQUAN = TmpVanBanDen.VANBANLIENQUAN + "," + VanBanDi.ID;
                            HSCV_VANBANDENBusiness.Save(TmpVanBanDen);
                        }
                        #endregion
                        WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
                        int customState = 0;
                        if (ALLOW_PUBLISH)
                        {
                            WF_STATEBusiness = Get<WF_STATEBusiness>();
                            WF_STATE finalState = WF_STATEBusiness.GetFinalStateOfItem(MODULE_CONSTANT.VANBANTRINHKY, currentUser);
                            customState = finalState != null ? finalState.ID : 0;
                        }

                        //duynn
                        //lưu thông tin văn bản đi nội bộ
                        if (USERS_RECEIVE_INTERNAL.Any())
                        {
                            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
                            List<long> tempUserIds = USERS_RECEIVE_INTERNAL;
                            tempUserIds.Add(currentUser.ID);
                            foreach (var item in tempUserIds)
                            {
                                var userProcess = new WF_ITEM_USER_PROCESS();
                                userProcess.ITEM_ID = VanBanDi.ID;
                                userProcess.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
                                userProcess.IS_XULYCHINH = true;
                                userProcess.USER_ID = item;
                                userProcess.create_at = DateTime.Now;
                                userProcess.create_by = currentUser.ID;
                                WF_ITEM_USER_PROCESSBusiness.Save(userProcess);
                            }

                            //gửi thông báo
                            string itemName = "VĂN BẢN TRÌNH KÝ NỘI BỘ";
                            string content = currentUser.HOTEN + " đã gửi bạn một văn bản trình ký nội bộ";
                            string url = "/HSVanBanDiArea/HSVanBanDi/DetailVanBan/" + VanBanDi.ID.ToString() + "?type=" + VANBANDI_CONSTANT.NOIBO;
                            SYS_TINNHANBusiness.sendMessageMultipleUsers(USERS_RECEIVE_INTERNAL, currentUser, itemName, content, url, string.Empty, false, 0, TargetDocType.COORDINATED);
                        }

                        //duynn
                        //cập nhật tài liệu văn bản đi
                        if (USERS_RECEIVE_INTERNAL.Any())
                        {
                            bool isSave = tool.UploadFiles(filebase, VbTrinhKyExtension.Split(',').ToList(), URL_FOLDER, filename, VanBanDi.ID, LOAITAILIEU.VANBAN, VbTrinhKySize, currentUser);
                            if (isSave)
                            {
                                //cập nhật tài liệu gốc cho các tài liệu đính kèm mới
                                List<TAILIEUDINHKEM> attachments = TAILIEUDINHKEMBusiness.GetInsertedData(VanBanDi.ID, LOAITAILIEU.VANBAN);
                                attachments.ForEach(x =>
                                {
                                    x.TAILIEU_GOC_ID = x.TAILIEU_ID;
                                    TAILIEUDINHKEMBusiness.Save(x);
                                });
                            }
                        }
                        else
                        {
                            var isState = WF_PROCESSBusiness.AddFlow(VanBanDi.ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser, customState);
                            if (!isState.Status)
                            {
                                HSCV_VANBANDIBusiness.repository.Delete(VanBanDi.ID);
                                HSCV_VANBANDIBusiness.repository.Save();
                            }
                            else
                            {
                                bool isSave = tool.UploadFiles(filebase, VbTrinhKyExtension.Split(',').ToList(), URL_FOLDER, filename, VanBanDi.ID, LOAITAILIEU.VANBAN, VbTrinhKySize, currentUser);
                                if (isSave)
                                {
                                    //cập nhật tài liệu gốc cho các tài liệu đính kèm mới
                                    List<TAILIEUDINHKEM> attachments = TAILIEUDINHKEMBusiness.GetInsertedData(VanBanDi.ID, LOAITAILIEU.VANBAN);
                                    attachments.ForEach(x =>
                                    {
                                        x.TAILIEU_GOC_ID = x.TAILIEU_ID;
                                        TAILIEUDINHKEMBusiness.Save(x);
                                    });
                                }
                            }
                        }

                        if (ALLOW_PUBLISH)
                        {
                            VanBanDi.SOHIEU = form["SOHIEU"]?.Trim();
                            VanBanDi.SOVANBAN_ID = form["SOVANBAN_ID"].ToIntOrZero();
                            VanBanDi.SOTHEOSO = form["SOTHEOSO"]?.Trim();
                            VanBanDi.NGAYBANHANH = form["NGAYBANHANH"].ToDateTime();
                            VanBanDi.NGAYCOHIEULUC = form["NGAYCOHIEULUC"].ToDateTime();
                            VanBanDi.NGAYHETHIEULUC = form["NGAYHET_HIEULUC"].ToDateTime();
                            HSCV_VANBANDIBusiness.Save(VanBanDi);

                            int numbSoDiTheoSo = VanBanDi.SOTHEOSO.GetPrefixNumber();
                            DM_DANHMUC_DATABusiness.UpdateSoVanBan(VanBanDi.SOVANBAN_ID.GetValueOrDefault(), numbSoDiTheoSo);

                            SaveVanBanPhatHanhToCaNhan(VanBanDi, sms);
                            SaveVanBanPhatHanhToDonVi(VanBanDi, sms);
                        }

                        DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                        DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(VanBanDi.NGUOIKY_ID);
                        ElasticModel model = ElasticModel.ConvertVanBan(VanBanDi, ListUser, NguoiDung != null ? NguoiDung.HOTEN : "");
                        ElasticSearch.insertDocument(model, model.Id.ToString(), ElasticType.VanBanDi);
                    }
                    return RedirectToAction("DetailVanBan", new { ID = VanBanDi.ID });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ChuaXuLy");
                }
            }
            #endregion
            return RedirectToAction("ChuaXuLy");
        }

        public ActionResult DetailVanBan(long ID, int type = VANBANDI_CONSTANT.CHUA_XULY)
        {
            AssignUserInfo();
            #region check quyền truy cập của người dùng đến văn bản hiện tại
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            HSCVREADVANBANBusiness = Get<HSCVREADVANBANBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            QL_NGUOINHAN_VANBANBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            var WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();

            DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            // check quyền truy cập theo workflow
            bool HasPermissionAccess = WF_ITEM_USER_PROCESSBusiness.CheckPermissionProcess(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
            // check quyền truy cập với quyền review
            bool HasPermissionReview = WF_REVIEW_USERBusiness.CheckPermissionReview(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
            bool HasPermissionMainProcess = WF_PROCESSBusiness.CheckPermissionProcess(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
            if (!HasPermissionAccess && !HasPermissionReview)
            {
                return Redirect("/Home/UnAuthor");
            }
            ThongTinVanBanDiVM myModel = new ThongTinVanBanDiVM();
            myModel.DocType = type;
            myModel.CurrentUser = currentUser;
            myModel.VanBanTrinhKy = HSCV_VANBANDIBusiness.Find(ID);
            if (myModel.VanBanTrinhKy == null)
            {
                return Redirect("/Home/UnAuthor");
            }
            myModel.CanSendOthers = WF_PROCESSBusiness.CheckIsEndByUser(MODULE_CONSTANT.VANBANTRINHKY, myModel.VanBanTrinhKy.ID, currentUser.ID);
            myModel.HasPermissionMainProcess = HasPermissionMainProcess;
            #region lấy tên danh mục - do dữ liệu văn bản rất lớn cho nên không join với bảng danh mục mà thực hiện 3,4 câu sub-query, tốc độ nhanh hơn
            // chỉ áp dụng kiểu truy vấn này với màn hình chi tiết 1 văn bản
            // ở màn hình danh sách tuyệt đối không áp dụng query ntn
            var DanhMucDoKhan = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.DOKHAN_ID);
            if (DanhMucDoKhan != null)
            {
                myModel.STR_DOKHAN = DanhMucDoKhan.TEXT;
            }
            var DanhMucDoUuTien = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.DOUUTIEN_ID);
            if (DanhMucDoUuTien != null)
            {
                myModel.STR_DOUUTIEN = DanhMucDoUuTien.TEXT;
            }
            var DanhMucLinhVuc = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.LINHVUCVANBAN_ID);
            if (DanhMucLinhVuc != null)
            {
                myModel.STR_LINHVUCVANBAN = DanhMucLinhVuc.TEXT;
            }
            var DanhMucLoaiVanBan = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.LOAIVANBAN_ID);
            if (DanhMucLoaiVanBan != null)
            {
                myModel.STR_LOAIVANBAN = DanhMucLoaiVanBan.TEXT;
            }
            var NguoiKyVanBan = DM_NGUOIDUNGBusiness.Find(myModel.VanBanTrinhKy.NGUOIKY_ID);
            if (NguoiKyVanBan != null)
            {
                myModel.STR_NGUOIKY = NguoiKyVanBan.HOTEN;
            }
            var DanhMucLoaiCoQuan = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.LOAI_COQUAN_ID);
            if (DanhMucLoaiCoQuan != null)
            {
                myModel.STR_LOAI_COQUAN = DanhMucLoaiCoQuan.TEXT;
            }

            var DanhMucPhanLoaiVanBan = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.THONGTIN_LOAI_ID);
            if (DanhMucPhanLoaiVanBan != null)
            {
                myModel.STR_PHAN_LOAI_VANBAN = DanhMucPhanLoaiVanBan.TEXT;
            }

            var TacGiaVanBan = DM_NGUOIDUNGBusiness.Find(myModel.VanBanTrinhKy.TACGIA_ID);
            if (TacGiaVanBan != null)
            {
                myModel.STR_TACGIA = TacGiaVanBan.HOTEN;
            }
            //Danh sách tài liệu đính kèm của văn bản
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            myModel.ListTaiLieu = TAILIEUDINHKEMBusiness.GetNewestData(myModel.VanBanTrinhKy.ID, LOAITAILIEU.VANBAN);
            if (!string.IsNullOrEmpty(myModel.VanBanTrinhKy.DONVINHAN_INTERNAL_ID))
            {
                CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                myModel.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(myModel.VanBanTrinhKy.DONVINHAN_INTERNAL_ID.ToListInt(','));
            }
            else
            {
                myModel.ListDonVi = new List<CCTC_THANHPHAN>();
            }

            //lấy danh sách các đơn vị chưa được gửi
            //var idsDonViDaNhan = myModel.ListDonVi.Select(x => x.ID).ToList();
            //myModel.TreeDonVi = CCTC_THANHPHANBusiness.GetTreeLabel(currentUser, idsDonViDaNhan);
            //if (myModel.TreeDonVi == null)
            //{
            //    myModel.TreeDonVi = new CCTCItemTreeBO();
            //}
            if (!string.IsNullOrEmpty(myModel.VanBanTrinhKy.USER_RECEIVE_DIRECTLY))
            {
                myModel.GroupUsersReceiveDirectly = DM_NGUOIDUNGBusiness.GetUsersByGroupIds(myModel.VanBanTrinhKy.USER_RECEIVE_DIRECTLY.ToListLong(','));
            }
            else
            {
                myModel.GroupUsersReceiveDirectly = new List<DM_NGUOIDUNG_BO>();
            }
            //kiểm tra các đơn vị nhận văn bản đã đọc
            myModel.GroupDonViRead = new List<int>();

            //lấy ra nhóm người dùng nhận văn bản
            //myModel.Recipients = QL_NGUOINHAN_VANBANBusiness.GetRecipientGroups(currentUser.DeptParentID.GetValueOrDefault(), myModel.GroupUsersReceiveDirectly.Select(x => x.ID).ToList());
            foreach (var unit in myModel.ListDonVi)
            {
                bool isRead = HSCVREADVANBANBusiness.CheckIsRead(MODULE_CONSTANT.VANBANDENNOIBO, ID, unit, currentUser);
                if (isRead)
                {
                    myModel.GroupDonViRead.Add(unit.ID);
                }
            }

            #endregion
            #endregion
            #region danh sách comment
            var HscvVanBanDiTraoDoiBusiness = Get<HSCV_VANBANDI_TRAODOIBusiness>();
            myModel.LstNoiDungTraoDoi = HscvVanBanDiTraoDoiBusiness.GetListCommentByVanBanDiId(ID);
            myModel.LstRootComment = myModel.LstNoiDungTraoDoi.Where(x => x.REPLY_ID == null).OrderByDescending(x => x.NGAYTAO).ToList();
            List<long> LstRootCommentIds = myModel.LstRootComment.Select(x => x.ID).ToList();
            var LstTaiLieuComment = TAILIEUDINHKEMBusiness.GetDataForTaskByListItemId(LstRootCommentIds, LOAITAILIEU.NOIDUNGTRAODOIVANBANDI);
            myModel.LstTaiLieuComment = LstTaiLieuComment;
            myModel.GroupUsersRead = HSCVREADVANBANBusiness.GetUsersRead(MODULE_CONSTANT.VANBANDENNOIBO, ID);
            #endregion

            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            if (myModel.VanBanTrinhKy.VANBANDEN_ID != null && myModel.VanBanTrinhKy.VANBANDEN_ID.Value > 0)
            {
                var TmpVanBanDen = HSCV_VANBANDENBusiness.Find(myModel.VanBanTrinhKy.VANBANDEN_ID.Value);
                myModel.VanBanDen = TmpVanBanDen;
            }

            //hiển thị người nhận trong văn bản
            if (myModel.VanBanTrinhKy.IS_INTERNAL == true)
            {
                List<long> userIds = WF_ITEM_USER_PROCESSBusiness.context.WF_ITEM_USER_PROCESS
                    .Where(x => x.IS_XULYCHINH == true && x.ITEM_ID == myModel.VanBanTrinhKy.ID
                    && x.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY && x.USER_ID != null)
                    .Select(x => x.USER_ID.Value).ToList();
                myModel.GroupUsersReceiveInternal = DM_NGUOIDUNGBusiness.repository.All()
                    .Where(x => userIds.Contains(x.ID))
                    .Select(x => new DM_NGUOIDUNG_BO()
                    {
                        ID = x.ID,
                        HOTEN = x.HOTEN
                    }).ToList();
            }
            else
            {
                myModel.GroupUsersReceiveInternal = new List<DM_NGUOIDUNG_BO>();
            }
            var checkread = HSCVREADVANBANBusiness.repository.All()
                .Where(x => x.USER_ID == currentUser.ID && x.TYPE == 2 && x.VANBAN_ID == ID).FirstOrDefault();
            if (checkread == null)
            {
                HSCV_READVANBAN readObj = new HSCV_READVANBAN();
                readObj.TYPE = 2;
                readObj.USER_ID = currentUser.ID;
                readObj.VANBAN_ID = ID;
                HSCVREADVANBANBusiness.Save(readObj);
            }

            if (myModel.VanBanTrinhKy.IS_INTERNAL == true)
            {
                myModel.GroupUsersRead = HSCVREADVANBANBusiness.GetUsersRead(MODULE_CONSTANT.VANBANTRINHKY, ID);
            }
            return View(myModel);
        }
        #region Các hàm private
        private List<CommonError> IsValid(HSCV_VANBANDI VanBan)
        {
            List<CommonError> ListError = new List<CommonError>();
            CommonError error;
            if (string.IsNullOrEmpty(VanBan.TRICHYEU))
            {
                error = new CommonError();
                error.Field = "TRICHYEU";
                error.Message = "Bạn chưa nhập trích yếu";
            }
            if (!VanBan.LOAIVANBAN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "LOAIVANBAN_ID";
                error.Message = "Bạn chưa chọn hình thức văn bản";
            }
            if (!VanBan.LINHVUCVANBAN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "LINHVUCVANBAN_ID";
                error.Message = "Bạn chưa chọn lĩnh vực văn bản";
            }
            if (!VanBan.DOKHAN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "DOKHAN_ID";
                error.Message = "Bạn chưa chọn độ khẩn";
            }
            if (!VanBan.DOUUTIEN_ID.HasValue)
            {
                error = new CommonError();
                error.Field = "DOUUTIEN_ID";
                error.Message = "Bạn chưa chọn độ ưu tiên";
            }
            if (string.IsNullOrEmpty(VanBan.NOIDUNG))
            {
                error = new CommonError();
                error.Field = "NOIDUNG";
                error.Message = "Bạn chưa nhập nội dung";
            }
            return ListError;
        }
        #endregion
        #region Các hàm jsonresult
        [ActionAudit]
        public JsonResult Delete(long id)
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDI VanBan = HSCV_VANBANDIBusiness.Find(id);
            AssignUserInfo();
            if (VanBan == null || currentUser.ID != VanBan.CREATED_BY)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không có quyền xóa văn bản trình ký này" });
            }
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            FileUltilities file = new FileUltilities();
            List<TAILIEUDINHKEM> ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.VANBAN);
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
            THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.GetDataByNam(id.ToString(), ThuMucLuuTruConstant.DefaultVanban);
            if (ThuMuc != null)
            {
                ThuMuc.IS_DELETE = true;
                THUMUC_LUUTRUBusiness.Save(ThuMuc);
            }
            TAILIEUDINHKEMBusiness.Save();
            HSCV_VANBANDIBusiness.repository.Delete(id);
            ElasticSearch.deleteDocument(id.ToString(), ElasticType.VanBanDi);
            return Json(new { Type = "SUCCESS", Message = "Xóa văn bản trình ký thành công" });
        }

        /// <summary>
        /// @author: duynn
        /// @description: xóa văn bản
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateUsersReceive(long id, long userId)
        {
            JsonResultBO result = new JsonResultBO(true);
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDI entityVanBan = HSCV_VANBANDIBusiness.Find(id);
            if (entityVanBan != null)
            {
                if (string.IsNullOrEmpty(entityVanBan.USER_RECEIVE_DIRECTLY) == false)
                {
                    List<long> userIds = entityVanBan.USER_RECEIVE_DIRECTLY.ToListLong(',');
                    List<long> finalResult = new List<long>();
                    userIds.ForEach(x =>
                    {
                        if (x != userId)
                        {
                            finalResult.Add(x);
                        }
                    });

                    entityVanBan.USER_RECEIVE_DIRECTLY = string.Join(",", finalResult.ToArray());
                    HSCV_VANBANDIBusiness.Save(entityVanBan);
                    result.Message = "Xóa người dùng nhận văn bản thành công";
                }
            }
            else
            {
                result.Status = false;
                result.Message = "Không tìm thấy văn bản";
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: kết xuất excel
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportExcel(int type = VANBANDI_CONSTANT.CHUA_XULY)
        {
            AssignUserInfo();
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("STT", "STT");
            properties.Add("SOHIEU", "Số ký hiệu");
            properties.Add("TRICHYEU", "Trích yếu");
            properties.Add("NGAYVANBAN", "Ngày văn bản");
            properties.Add("TEN_NGUOIKY", "Người ký");
            properties.Add("NOI_NHAN", "Nơi nhận");
            properties.Add("SOTRANG", "Số trang");
            properties.Add("SOBANSAO", "Số bản");
            properties.Add("TEN_DOKHAN", "Độ mật");
            properties.Add("GHICHU", "Ghi chú");

            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            PageListResultBO<HSCV_VANBANDI_BO> data = new PageListResultBO<HSCV_VANBANDI_BO>();
            string sessionName = string.Empty;
            string fileName = string.Empty;
            string actionName = string.Empty;
            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    sessionName = "SessionVanBanDiChuaXuLy";
                    fileName = "Văn bản đi chưa xử lý";
                    actionName = "ChuaXuLy";
                    break;
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    sessionName = "SessionVanBanDiThamGiaXuLy";
                    fileName = "Văn bản đi tham gia xử lý";
                    actionName = "ThamGiaXuLy";
                    break;
                case VANBANDI_CONSTANT.DA_XULY:
                    sessionName = "SessionVanBanDiDaXuLy";
                    fileName = "Văn bản đi đã xử lý";
                    actionName = "DaXuLy";
                    break;
                case VANBANDI_CONSTANT.DA_BANHANH:
                    sessionName = "SessionVanBanDiDaBanHanh";
                    fileName = "Văn bản đi đã ban hành";
                    actionName = "DaBanHanh";
                    break;
                case VANBANDI_CONSTANT.NOIBO:
                    sessionName = "SessionVanBanDiNoiBo";
                    fileName = "Văn bản đi nội bộ";
                    actionName = "NoiBo";
                    break;
                default:
                    sessionName = "SessionVanBanDi";
                    fileName = "Văn bản đi";
                    actionName = "Index";
                    break;
            }

            HSCV_VANBANDI_SEARCH searchModel = (HSCV_VANBANDI_SEARCH)SessionManager.GetValue(sessionName);
            if (searchModel == null)
            {
                searchModel = new HSCV_VANBANDI_SEARCH();
            }

            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    data = HSCV_VANBANDIBusiness.GetListProcessing(searchModel, 0, -1);
                    break;
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    data = HSCV_VANBANDIBusiness.GetListJoinProcessing(searchModel, 0, -1);
                    break;
                case VANBANDI_CONSTANT.DA_XULY:
                    data = HSCV_VANBANDIBusiness.GetListProcessed(searchModel, 0, -1);
                    break;
                case VANBANDI_CONSTANT.DA_BANHANH:
                    data = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.GetValueOrDefault(), 0, -1);
                    break;
                case VANBANDI_CONSTANT.NOIBO:
                    data = HSCV_VANBANDIBusiness.GetListInternal(searchModel, 0, -1);
                    break;
                default:
                    data = HSCV_VANBANDIBusiness.GetDaTaByPage(searchModel, 0, -1);
                    break;
            }

            EPPlusSupplier<HSCV_VANBANDI_BO> supplier = new EPPlusSupplier<HSCV_VANBANDI_BO>();
            supplier.properties = properties;
            supplier.startColumn = 1;
            supplier.startRow = 6;
            supplier.fileName = fileName;

            var stream = supplier.CreateExcelFile(data.ListItem, FormatWorkSheet);
            var buffer = stream as MemoryStream;

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileName + ".xlsx"));
            Response.BinaryWrite(buffer.ToArray());
            Response.Flush();
            Response.End();

            return RedirectToAction(actionName);
        }

        /// <summary>
        /// @author: duynn
        /// @description: cập nhật worksheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public ExcelWorksheet FormatWorkSheet(ExcelWorksheet sheet, string title)
        {
            sheet.DefaultColWidth = 30;
            sheet.Column(3).Width = 50;
            sheet.Column(1).Width = 10;
            sheet.Column(4).Width = 20;
            sheet.Column(2).Width = 25;
            sheet.Column(5).Width = 25;
            sheet.Column(6).Width = 30;
            sheet.Column(7).Width = 20;
            sheet.Column(8).Width = 20;
            sheet.Column(9).Width = 20;

            sheet.Cells.Style.Font.Name = "Times New Roman";

            ExcelRange titleRange = sheet.SelectedRange["A1:J1"];
            titleRange.Value = null;
            titleRange.Merge = true;
            titleRange.Value = "Mục lục công văn bản đi";
            titleRange.Style.Font.Size = 22;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(1).Height = 30;


            ExcelRange userRange = sheet.SelectedRange["A3:J3"];
            userRange.Value = null;
            userRange.Merge = true;
            userRange.Value = "Cán bộ thống kê: " + currentUser.HOTEN + " - Loại: " + title;
            userRange.Style.Font.Size = 14;
            userRange.Style.Font.Bold = true;
            userRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            userRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            userRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(3).Height = 30;

            ExcelRange timeRange = sheet.SelectedRange["A4:J4"];
            timeRange.Value = null;
            timeRange.Merge = true;
            timeRange.Value = string.Format("(Ngày thống kê: {0})", DateTime.Now.ToVietnameseDateFormat());
            timeRange.Style.Font.Size = 14;
            timeRange.Style.Font.Bold = true;
            timeRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            timeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            timeRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(4).Height = 30;


            //sheet.SelectedRange["A4:H4"].Value = null;
            //sheet.SelectedRange["A4:H4"].Merge = true;
            //sheet.SelectedRange["A4:H4"].Value = string.Format("(Ngày thống kê: {0})", DateTime.Now.ToVietnameseDateFormat());
            //sheet.SelectedRange["A4:H4"].Style.Font.Size = 11;
            //sheet.SelectedRange["A4:H4"].Style.Font.Bold = true;
            //sheet.SelectedRange["A4:H4"].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(236, 29, 36));
            //sheet.SelectedRange["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //sheet.SelectedRange["A4:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //sheet.Row(4).Height = 30;

            //sheet.SelectedRange["A5:H5"].Style.Font.Size = 13;
            //sheet.SelectedRange["A5:H5"].Style.Font.Bold = true;
            //sheet.SelectedRange["A5:H5"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            //sheet.SelectedRange["A5:H5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //sheet.SelectedRange["A5:H5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 124, 194));
            //sheet.SelectedRange["A5:H5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //sheet.SelectedRange["A5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //sheet.Row(5).Height = 30;

            return sheet;
        }

        /// <summary>
        /// @author: duynn
        /// @description: hiển thị thông tin văn bản phát hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult ShowPublishInfo(long id)
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            HSCVREADVANBANBusiness = Get<HSCVREADVANBANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            ThongTinVanBanDiVM model = new ThongTinVanBanDiVM();
            model.VanBanTrinhKy = HSCV_VANBANDIBusiness.Find(id);
            model.ListDonVi = new List<CCTC_THANHPHAN>();
            if (!string.IsNullOrEmpty(model.VanBanTrinhKy.DONVINHAN_INTERNAL_ID))
            {
                model.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(model.VanBanTrinhKy.DONVINHAN_INTERNAL_ID.ToListInt(','));
            }
            //kiểm tra các đơn vị nhận văn bản đã đọc
            model.GroupDonViRead = new List<int>();
            foreach (var unit in model.ListDonVi)
            {
                bool isRead = HSCVREADVANBANBusiness.CheckIsRead(MODULE_CONSTANT.VANBANDENNOIBO, id, unit, currentUser);
                if (isRead)
                {
                    model.GroupDonViRead.Add(unit.ID);
                }
            }
            if (!string.IsNullOrEmpty(model.VanBanTrinhKy.USER_RECEIVE_DIRECTLY))
            {
                model.GroupUsersReceiveDirectly = DM_NGUOIDUNGBusiness.GetUsersByGroupIds(model.VanBanTrinhKy.USER_RECEIVE_DIRECTLY.ToListLong(','));
            }
            else
            {
                model.GroupUsersReceiveDirectly = new List<DM_NGUOIDUNG_BO>();
            }
            model.GroupUsersRead = HSCVREADVANBANBusiness.GetUsersRead(MODULE_CONSTANT.VANBANDENNOIBO, id);
            return PartialView("_PublishInfo", model);
        }

        /// <summary>
        /// @author: duynn
        /// @description: hiển thị thông tin người nhận văn bản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult ShowRecipientsInfo(long id)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCVREADVANBANBusiness = Get<HSCVREADVANBANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
            ThongTinVanBanDiVM model = new ThongTinVanBanDiVM();
            model.VanBanTrinhKy = HSCV_VANBANDIBusiness.Find(id);

            List<long> userIds = WF_ITEM_USER_PROCESSBusiness.context.WF_ITEM_USER_PROCESS
                    .Where(x => x.IS_XULYCHINH == true && x.ITEM_ID == model.VanBanTrinhKy.ID
                    && x.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY && x.USER_ID != null && x.USER_ID != currentUser.ID)
                    .Select(x => x.USER_ID.Value).ToList();
            model.GroupUsersReceiveInternal = DM_NGUOIDUNGBusiness.repository.All()
                .Where(x => userIds.Contains(x.ID))
                .Select(x => new DM_NGUOIDUNG_BO()
                {
                    ID = x.ID,
                    HOTEN = x.HOTEN
                }).ToList();
            model.GroupUsersRead = HSCVREADVANBANBusiness.GetUsersRead(MODULE_CONSTANT.VANBANTRINHKY, id);
            return PartialView("_RecipientsInfo", model);
        }


        /// <summary>
        /// @author: duynn
        /// @description: quay về trang danh sách
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult NavigateBackToList(int type = VANBANDI_CONSTANT.CHUA_XULY)
        {
            switch (type)
            {
                case VANBANDI_CONSTANT.CHUA_XULY:
                    return RedirectToAction("ChuaXuLy");
                case VANBANDI_CONSTANT.DA_XULY:
                    return RedirectToAction("DaXuLy");
                case VANBANDI_CONSTANT.THAMGIA_XULY:
                    return RedirectToAction("ThamGiaXuLy");
                case VANBANDI_CONSTANT.DA_BANHANH:
                    return RedirectToAction("DaBanHanh");
                case VANBANDI_CONSTANT.NOIBO:
                    return RedirectToAction("NoiBo");
                default:
                    return RedirectToAction("Index");
            }
        }

        //[HttpPost]
        //public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        //{
        //    HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
        //    var searchModel = SessionManager.GetValue("VanBanDiSearch") as HSCV_VANBANDI_SEARCH;
        //    if (!string.IsNullOrEmpty(sortQuery))
        //    {
        //        if (searchModel == null)
        //        {
        //            searchModel = new HSCV_VANBANDI_SEARCH();
        //        }
        //        searchModel.sortQuery = sortQuery;
        //        if (pageSize > 0)
        //        {
        //            searchModel.pageSize = pageSize;
        //        }
        //        SessionManager.SetValue("VanBanDiSearch", searchModel);
        //    }
        //    var data = HSCV_VANBANDIBusiness.GetDaTaByPage(searchModel, pageSize, indexPage);
        //    return Json(data);
        //}
        //public JsonResult searchData(FormCollection form)
        //{
        //    HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
        //    var searchModel = SessionManager.GetValue("VanBanDiSearch") as HSCV_VANBANDI_SEARCH;
        //    searchModel.SOHIEU = form["SOHIEU"];
        //    searchModel.TRICHYEU = form["TRICHYEU"];
        //    searchModel.DOKHAN_ID = form["DOKHAN_ID"].ToIntOrNULL();
        //    searchModel.DOUUTIEN_ID = form["DOMAT_ID"].ToIntOrNULL();
        //    searchModel.LINHVUCVANBAN_ID = form["LINHVUCVANBAN_ID"].ToIntOrNULL();
        //    searchModel.LOAIVANBAN_ID = form["LOAIVANBAN_ID"].ToIntOrNULL();
        //    SessionManager.SetValue("VbReview", searchModel);
        //    var data = HSCV_VANBANDIBusiness.GetDaTaByPage(searchModel, searchModel.pageSize, 1);
        //    return Json(data);
        //}
        #endregion

        #region save comment
        [HttpPost]
        [ActionAudit]
        public ActionResult SaveCommentRootLevel(IEnumerable<HttpPostedFileBase> FILECOMMENTFORTASK, string[] filename, FormCollection col)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();

            var HscvVanBanDiTraoDoiBusiness = Get<HSCV_VANBANDI_TRAODOIBusiness>();
            int docType = col["DOC_TYPE"].ToIntOrZero();
            if (!string.IsNullOrEmpty(col["COMMENTFORTASK"]))
            {
                var VanBanId = col["COMMENTFORTASK"].ToIntOrZero();
                HSCV_VANBANDI vanbanObj = HSCV_VANBANDIBusiness.Find(VanBanId);
                if (vanbanObj != null)
                {
                    int REPLY_ID = 0;
                    if (!string.IsNullOrEmpty(col["REPLY_ID"]))
                    {
                        REPLY_ID = col["REPLY_ID"].ToIntOrZero();
                    }
                    HSCV_VANBANDI_TRAODOI comment = new HSCV_VANBANDI_TRAODOI();
                    comment.VANBANDI_ID = VanBanId;
                    comment.NOIDUNGTRAODOI = col["NOIDUNGTRAODOIFORTASK"];
                    comment.NGUOITAO = (long)currentUser.ID;
                    comment.NGAYTAO = DateTime.Now;
                    HscvVanBanDiTraoDoiBusiness.Save(comment);
                    UploadFileTool upload = new UploadFileTool();
                    upload.UploadCustomFile(FILECOMMENTFORTASK, true, VbTrinhKyExtension, URL_FOLDER, VbTrinhKySize, null, filename, comment.ID, LOAITAILIEU.NOIDUNGTRAODOIVANBANDI, "Trao đổi văn bản đi");

                    #region gửi tin nhắn khi trao đổi văn bản nội bộ
                    if (docType == VANBANDI_CONSTANT.NOIBO)
                    {
                        string msg = currentUser.HOTEN + " đã đăng trao đổi công việc #Văn bản đi " + vanbanObj.ID.ToString();
                        string itemName = "VĂN BẢN TRÌNH KÝ NỘI BỘ";
                        string content = currentUser.HOTEN + " đã đăng trao đổi công việc #Văn bản đi " + vanbanObj.ID.ToString();
                        string url = "/HSVanBanDiArea/HSVanBanDi/DetailVanBan/" + vanbanObj.ID.ToString() + "?type=" + VANBANDI_CONSTANT.NOIBO;

                        List<long> userIds = WF_ITEM_USER_PROCESSBusiness.context.WF_ITEM_USER_PROCESS
                            .Where(x => x.IS_XULYCHINH == true && x.ITEM_ID == vanbanObj.ID
                            && x.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY && x.USER_ID != null && x.USER_ID != currentUser.ID)
                            .Select(x => x.USER_ID.Value).ToList();

                        SYS_TINNHANBusiness.sendMessageMultipleUsers(userIds, currentUser, itemName, content, url, string.Empty, false, 0, TargetDocType.COORDINATED);
                    }
                    #endregion
                    return RedirectToAction("DetailVanBan", new { ID = VanBanId, type = docType });
                }
            }
            return RedirectToAction("Index");
        }
        [ActionAudit]
        public JsonResult SaveReplyForComment(long COMMENT_ID, string COMMENT, long TASK_ID, int DOC_TYPE)
        {
            if (string.IsNullOrEmpty(COMMENT) || COMMENT_ID <= 0 || TASK_ID <= 0 || DOC_TYPE < 0)
            {
                return Json(false);
            }
            else
            {
                AssignUserInfo();
                HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
                SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();

                var HscvVanBanDiTraoDoiBusiness = Get<HSCV_VANBANDI_TRAODOIBusiness>();
                HSCV_VANBANDI_TRAODOI comment = new HSCV_VANBANDI_TRAODOI();
                comment.VANBANDI_ID = TASK_ID;
                comment.NOIDUNGTRAODOI = COMMENT;
                comment.NGUOITAO = (long)currentUser.ID;
                comment.NGAYTAO = DateTime.Now;
                comment.PARENT_ID = COMMENT_ID;
                HscvVanBanDiTraoDoiBusiness.Save(comment);
                #region notification
                #region gửi tin nhắn khi trao đổi văn bản nội bộ
                if (DOC_TYPE == VANBANDI_CONSTANT.NOIBO)
                {
                    HSCV_VANBANDI entity = HSCV_VANBANDIBusiness.Find(TASK_ID);

                    string msg = currentUser.HOTEN + " đã đăng trao đổi công việc #Văn bản đi " + entity.ID.ToString();
                    string itemName = "VĂN BẢN TRÌNH KÝ NỘI BỘ";
                    string content = currentUser.HOTEN + " đã đăng trao đổi công việc #Văn bản đi " + entity.ID.ToString();
                    string url = "/HSVanBanDiArea/HSVanBanDi/DetailVanBan/" + entity.ID.ToString() + "?type=" + VANBANDI_CONSTANT.NOIBO;

                    List<long> userIds = WF_ITEM_USER_PROCESSBusiness.context.WF_ITEM_USER_PROCESS
                        .Where(x => x.IS_XULYCHINH == true && x.ITEM_ID == entity.ID
                        && x.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY && x.USER_ID != null && x.USER_ID != currentUser.ID)
                        .Select(x => x.USER_ID.Value).ToList();

                    SYS_TINNHANBusiness.sendMessageMultipleUsers(userIds, currentUser, itemName, content, url, string.Empty, false, 0, TargetDocType.COORDINATED);
                }
                #endregion
                #endregion
                return Json(true);
            }
        }
        #endregion

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng nhận đích danh văn bản
        /// </summary>
        /// <param name="roleId">mã vai trò</param>
        /// <param name="docId">mã văn bản</param>
        /// <returns></returns>
        //public PartialViewResult GetUsersReceiveDirectly(int roleId, long docId, string selected)
        //{
        //    //HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
        //    //DM_VAITROBusiness = Get<DM_VAITROBusiness>();
        //    //HSCV_VANBANDI entityVanBanDi = HSCV_VANBANDIBusiness.Find(docId);

        //    //VanBanDiVM viewModel = new VanBanDiVM();
        //    //viewModel.GroupUsersInRole = DM_VAITROBusiness.GetListUserInRole(roleId);
        //    //viewModel.GroupUserIdsReceiveDirectly = new List<long>();
        //    //if (entityVanBanDi != null && entityVanBanDi.USER_RECEIVE_DIRECTLY != null)
        //    //{
        //    //    viewModel.GroupUserIdsReceiveDirectly = entityVanBanDi.USER_RECEIVE_DIRECTLY.ToListLong(',');
        //    //}
        //    //if(!string.IsNullOrEmpty(selected))
        //    //{
        //    //    viewModel.GroupUserIdsReceiveDirectly.AddRange(selected.ToListLong(','));
        //    //}
        //    //return PartialView("_UsersReceiveDirectly", viewModel);
        //    return 
        //}

        /// <summary>
        /// gửi cá nhân văn bản đã phát hành
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendVanBanPhatHanhToCaNhan(FormCollection form)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(true);
            try
            {
                SMSDAL.SendSMSDAL sms = new SMSDAL.SendSMSDAL();
                HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                LogSMSBusiness = Get<LogSMSBusiness>();
                HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
                SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();

                HSCV_VANBANDI entityVanBanDi = HSCV_VANBANDIBusiness.Find(form["VANBANDI_ID"].ToIntOrZero());
                var idsNguoiNhanDichDanh = form["USERS_RECEIVE_SPECIAL"].ToListLong(',');
                if (!string.IsNullOrEmpty(entityVanBanDi.USER_RECEIVE_DIRECTLY))
                {
                    var idsNguoiDaNhan = entityVanBanDi.USER_RECEIVE_DIRECTLY.ToListLong(',');
                    idsNguoiDaNhan.AddRange(idsNguoiNhanDichDanh);
                    entityVanBanDi.USER_RECEIVE_DIRECTLY = string.Join(",", idsNguoiDaNhan.ToArray());
                    HSCV_VANBANDIBusiness.Save(entityVanBanDi);
                    this.SaveVanBanPhatHanhToCaNhan(entityVanBanDi, sms, idsNguoiNhanDichDanh);
                }
                else
                {
                    entityVanBanDi.USER_RECEIVE_DIRECTLY = form["USERS_RECEIVE_SPECIAL"];
                    HSCV_VANBANDIBusiness.Save(entityVanBanDi);
                    this.SaveVanBanPhatHanhToCaNhan(entityVanBanDi, sms);
                }
                
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// gửi đơn vị các văn bản đã phát hành
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendVanBanPhatHanhToDonVi(FormCollection form)
        {
            AssignUserInfo();
            SMSDAL.SendSMSDAL sms = new SMSDAL.SendSMSDAL();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            LogSMSBusiness = Get<LogSMSBusiness>();

            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();

            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();

            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

            JsonResultBO result = new JsonResultBO(true);
            try
            {
                HSCV_VANBANDI entityVanBanDi = HSCV_VANBANDIBusiness.Find(form["VANBANDI_ID"].ToIntOrZero());
                DM_NGUOIDUNG entityNguoiKy = DM_NGUOIDUNGBusiness.Find(entityVanBanDi.NGUOIKY_ID);
                List<TAILIEUDINHKEM> groupFiles = TAILIEUDINHKEMBusiness.GetNewestData(entityVanBanDi.ID, LOAITAILIEU.VANBAN);

                List<int> idsDonViNhan = form["chonphongban"].ToListInt(',');

                if (!string.IsNullOrEmpty(entityVanBanDi.DONVINHAN_INTERNAL_ID))
                {
                    var idsDonViDaNhan = entityVanBanDi.DONVINHAN_INTERNAL_ID.ToListInt(',');
                    idsDonViDaNhan.AddRange(idsDonViNhan);
                    entityVanBanDi.DONVINHAN_INTERNAL_ID = string.Join(",", idsDonViDaNhan.ToArray());
                    HSCV_VANBANDIBusiness.Save(entityVanBanDi);
                    //gửi đơn vị nhận văn bản bên ngoài
                    this.SaveVanBanPhatHanhToDonVi(entityVanBanDi, sms, idsDonViNhan);
                }
                else
                {
                    entityVanBanDi.DONVINHAN_INTERNAL_ID = form["chonphongban"];
                    HSCV_VANBANDIBusiness.Save(entityVanBanDi);
                    //gửi đơn vị nhận văn bản bên ngoài
                    this.SaveVanBanPhatHanhToDonVi(entityVanBanDi, sms);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// @author:duynn
        /// @description: gửi đơn vị nhận bên ngoài
        /// @since: 10/06/2019
        /// </summary>
        /// <param name="entityVanBanDi"></param>
        /// <param name="sms"></param>
        /// <param name="otherDepts">gửi các đơn vị khác</param>
        /// <returns></returns>
        public bool SaveVanBanPhatHanhToDonVi(HSCV_VANBANDI entityVanBanDi, SMSDAL.SendSMSDAL sms, List<int> otherDepts = null)
        {
            bool result = true;
            try
            {
                DM_NHOMDANHMUC entityNhomDanhMuc = DM_NHOMDANHMUCBusiness.repository.All()
                    .FirstOrDefault(x => x.GROUP_CODE == DMLOAI_CONSTANT.SOVANBANDEN);

                DM_NGUOIDUNG entityNguoiKy = DM_NGUOIDUNGBusiness.Find(entityVanBanDi.NGUOIKY_ID);

                List<TAILIEUDINHKEM> groupFiles = TAILIEUDINHKEMBusiness.GetNewestData(entityVanBanDi.ID, LOAITAILIEU.VANBAN);

                List<int> idsDonViNhan = new List<int>();

                if(otherDepts != null && otherDepts.Any())
                {
                    idsDonViNhan = otherDepts;
                }
                else if (!string.IsNullOrEmpty(entityVanBanDi.DONVINHAN_INTERNAL_ID))
                {
                    var idsDonViDaNhan = entityVanBanDi.DONVINHAN_INTERNAL_ID.ToListInt(',');
                    idsDonViNhan.AddRange(idsDonViDaNhan);
                    entityVanBanDi.DONVINHAN_INTERNAL_ID = string.Join(",", idsDonViNhan.ToArray());
                }

                List<CCTC_THANHPHAN> groupDonViNhan = CCTC_THANHPHANBusiness.repository.AllNoTracking
                    .Where(x => idsDonViNhan.Contains(x.ID)).ToList();
                List<LOGSMS> groupLogSMS = new List<LOGSMS>();
                List<TAILIEUDINHKEM> groupForwardFiles = new List<TAILIEUDINHKEM>();
                List<WF_ITEM_USER_PROCESS> groupItemUserProcess = new List<WF_ITEM_USER_PROCESS>();
                foreach (var dept in groupDonViNhan)
                {
                    WF_STATE firstState = null;

                    //kiểm tra có phải là gửi nội bộ hay không?
                    bool isSendInternal = false;
                    //- trường hợp gửi cho các ban ngành cấp tỉnh và các huyện xa -> thành văn bản đến của các đơnvị nhận được
                    if (dept.TYPE == DEPTTYLELABEL.ToIntOrZero())
                    {
                        //- trường hợp này là văn bản đến bình thường
                        var workflowModule = WF_MODULEBusiness.repository
                            .All().FirstOrDefault(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANDEN);

                        var workflowStreamIds = workflowModule.WF_STREAM_ID.ToListInt(',');

                        //- lấy luồng xử lý của từng đơn vị
                        var workflowStream = WF_STREAMBusiness.repository.All()
                            .FirstOrDefault(x => x.LEVEL_ID == dept.CATEGORY && workflowStreamIds.Contains(x.ID));

                        //lấy trạng thái xử lý ban đầu cảu đơn vị
                        firstState = WF_STATEBusiness.repository.All()
                            .FirstOrDefault(x => x.IS_START == true && x.WF_ID == workflowStream.ID);
                    }
                    else if (dept.PARENT_ID == currentUser.DeptParentID)
                    {
                        isSendInternal = true;
                        //- trường hợp gửi các phòng ban thuộc tỉnh ủy => thành văn bản đến nội bộ của tỉnh
                        var workflowModule = WF_MODULEBusiness.repository.All()
                         .FirstOrDefault(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANDENNOIBO);

                        var workflowStreamIds = workflowModule.WF_STREAM_ID.ToListInt(',');

                        //- lấy luồng xử lý của từng đơn vị
                        var workflowStream = WF_STREAMBusiness.repository.All()
                            .FirstOrDefault(x => x.LEVEL_ID == dept.CATEGORY && workflowStreamIds.Contains(x.ID));

                        firstState = WF_STATEBusiness.repository.All()
                            .FirstOrDefault(x => x.IS_START == true && x.WF_ID == workflowStream.ID);
                    }


                    UserInfoBO processor = null;

                    /**
                     * kiểm tra vai trò nhận của trạng thái đầu tiên
                     */
                    if (firstState != null)
                    {
                        /**
                         * lấy người thuộc phòng ban có vai trò xử lý
                         */
                        var receiver = DM_NGUOIDUNGBusiness.GetUserByRoleAndDeptId(firstState.VAITRO_ID.GetValueOrDefault(), dept.ID).FirstOrDefault();
                        if (receiver != null)
                        {
                            processor = DM_NGUOIDUNGBusiness.GetNewUserInfo(receiver.Value.ToLongOrZero());
                        }
                    }


                    var dataSoVanBanDen = DM_DANHMUC_DATABusiness.GetSoVanBan(DMLOAI_CONSTANT.SOVANBANDEN, DateTime.Now.Year, dept.ID);
                    if (dataSoVanBanDen == null)
                    {
                        //tạo sổ văn bản đến
                        dataSoVanBanDen = new DM_DANHMUC_DATA()
                        {
                            DEPTID = dept.ID,
                            TEXT = "Sổ văn bản đến " + DateTime.Now.Year,
                            DATA = DateTime.Now.Year,
                            DM_NHOM_ID = entityNhomDanhMuc.ID
                        };
                        DM_DANHMUC_DATABusiness.Save(dataSoVanBanDen);
                    }

                    HSCV_VANBANDEN entityVanBanDen = this.ConvertToVanBanDen(entityVanBanDi, entityNguoiKy, dataSoVanBanDen);
                    entityVanBanDen.IS_NOIBO = isSendInternal ? true : false;
                    HSCV_VANBANDENBusiness.Save(entityVanBanDen);

                    /**
                     * cập nhật số văn bản đến
                     */
                    dataSoVanBanDen.GHICHU = (dataSoVanBanDen.GHICHU.ToIntOrZero() + 1).ToString();
                    DM_DANHMUC_DATABusiness.Save(dataSoVanBanDen);

                    /**
                     * cập nhật tài liệu đính kèm
                     */
                    var files = this.GenerateFiles(groupFiles, entityVanBanDen);
                    groupForwardFiles.AddRange(files);

                    /**
                     * cập nhật thông tin văn bản đến trong luồng xử lý
                     */
                    if (processor != null)
                    {
                        WF_PROCESSBusiness.AddFlow(entityVanBanDen.ID, isSendInternal ? MODULE_CONSTANT.VANBANDENNOIBO : MODULE_CONSTANT.VANBANDEN, processor);
                    }

                    /**
                     * nếu không có người phù hợp trong luồng xử lý ==> lấy người có vai trò cao nhất tại phòng
                     */
                    if (processor == null)
                    {
                        /**
                        * lấy ra người có vai trò cao nhất tại phòng ban
                        */
                        processor = DM_NGUOIDUNGBusiness.GetUserHighestPriority(dept.ID);

                        /**
                         * lưu thông in người nhận vào bảng  WF_ITEM_USER_PROCESS
                         */
                        var itemUserProcess = new WF_ITEM_USER_PROCESS();
                        itemUserProcess.ITEM_ID = entityVanBanDen.ID;
                        itemUserProcess.ITEM_TYPE = isSendInternal ? MODULE_CONSTANT.VANBANDENNOIBO : MODULE_CONSTANT.VANBANDEN;
                        itemUserProcess.IS_XULYCHINH = false;
                        itemUserProcess.USER_ID = processor.ID;
                        itemUserProcess.create_at = DateTime.Now;
                        itemUserProcess.create_by = currentUser.ID;
                        groupItemUserProcess.Add(itemUserProcess);
                    }


                    if (processor != null)
                    {
                        ElasticSearch.updateListUser(entityVanBanDen.ID.ToString(), new List<long> { processor.ID }, ElasticType.VanBanDen);

                        //gửi email
                        if (!string.IsNullOrEmpty(processor.EMAIL))
                        {
                            var ContentEmail = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến <a href='" + SERVERADDRESS + "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + entityVanBanDen.ID.ToString() + "'>" + entityVanBanDen.SOHIEU + "</a>";
                            EmailProvider.SendMailTemplate(currentUser, ContentEmail, ContentEmail, new List<string> { processor.EMAIL });
                        }
                        //gửi tin nhắn
                        if (currentUser.CanSendSMS && entityVanBanDi.CAN_SEND_SMS == true && processor.DIENTHOAI != null)
                        {
                            var ContentSMS = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến " + entityVanBanDen.SOHIEU;
                            ContentSMS = sms.locDau(ContentSMS);
                            var DoDaiSMS = ContentSMS.Length;
                            string[] noiDung = new string[1];
                            noiDung[0] = ContentSMS;
                            string resultsend = sms.guiTinNhan(processor.DIENTHOAI, "177403", noiDung);
                            LOGSMS SmsObject = new LOGSMS();
                            SmsObject.SODIENTHOAINHAN = processor.DIENTHOAI;
                            SmsObject.NOIDUNG = ContentSMS;
                            SmsObject.SOKYTU = DoDaiSMS;
                            SmsObject.KETQUA = resultsend;
                            SmsObject.CREATED_AT = DateTime.Now;
                            SmsObject.CREATED_BY = currentUser.ID;
                            SmsObject.HOTENNGUOIGUI = currentUser.HOTEN;
                            SmsObject.DONVI_GUI = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                            SmsObject.ITEMTYPE = "VANBANDEN";
                            SmsObject.ITEMID = entityVanBanDen.ID;
                            SmsObject.DONVI_NHAN = processor.DM_PHONGBAN_ID.GetValueOrDefault();
                            SmsObject.HOTENNGUOINHAN = processor.HOTEN;
                            groupLogSMS.Add(SmsObject);
                        }

                        //lưu thông báo
                        SYS_TINNHAN noti = new SYS_TINNHAN();
                        noti.FROM_USERNAME = currentUser.HOTEN;
                        noti.FROM_USER_ID = currentUser.ID;
                        noti.NGAYTAO = DateTime.Now;
                        noti.NOIDUNG = currentUser.TenPhongBan + " đã gửi đến bạn một văn bản đến";
                        noti.URL = "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen/" + entityVanBanDen.ID.ToString();
                        noti.TIEUDE = "Xử lý văn bản đến";
                        noti.TO_USER_ID = processor.ID;
                        SYS_TINNHANBusiness.Save(noti, "", false, entityVanBanDen.ID, TargetDocType.COORDINATED);
                    }
                }

                //lưu thông tin tài liệu, sms, luồng xử lý
                TAILIEUDINHKEMBusiness.repository.InsertRange(groupForwardFiles);
                LogSMSBusiness.repository.InsertRange(groupLogSMS);
                WF_ITEM_USER_PROCESSBusiness.repository.InsertRange(groupItemUserProcess);

                TAILIEUDINHKEMBusiness.repository.Save();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: gửi cá nhân nhận bên ngoài
        /// @since: 10/06/2019
        /// </summary>
        /// <param name="entityVanBanDi"></param>
        /// <param name="sms"></param>
        /// <param name="otherRecipients">người nhận mới</param>
        /// <returns></returns>
        public bool SaveVanBanPhatHanhToCaNhan(HSCV_VANBANDI entityVanBanDi, SMSDAL.SendSMSDAL sms, List<long> otherRecipients = null)
        {
            List<TAILIEUDINHKEM> groupForwardFiles = new List<TAILIEUDINHKEM>();
            List<LOGSMS> groupLogSMSs = new List<LOGSMS>();
            var idsNguoiNhanDichDanh = new List<long>();
            if (otherRecipients != null && otherRecipients.Any())
            {
                idsNguoiNhanDichDanh = otherRecipients;
            }
            else if(!string.IsNullOrEmpty(entityVanBanDi.USER_RECEIVE_DIRECTLY))
            {
                idsNguoiNhanDichDanh = entityVanBanDi.USER_RECEIVE_DIRECTLY.ToListLong(',');
            }

            bool result = true;

            try
            {
                /**
                 * nhóm sổ văn bản
                 */
                var dataNhomSoVanBanDen = DM_NHOMDANHMUCBusiness.repository.All()
                   .FirstOrDefault(x => x.GROUP_CODE == DMLOAI_CONSTANT.SOVANBANDEN);

                /**
                 * gửi từng người nhận đích danh
                 */
                foreach (var userId in idsNguoiNhanDichDanh)
                {
                    var entityNguoiNhan = DM_NGUOIDUNGBusiness.GetNewUserInfo(userId);
                    var dataSoVanBanDen = DM_DANHMUC_DATABusiness.GetSoVanBan(DMLOAI_CONSTANT.SOVANBANDEN, DateTime.Now.Year, entityNguoiNhan.DeptParentID.Value);

                    if (dataSoVanBanDen == null)
                    {
                        dataSoVanBanDen = new DM_DANHMUC_DATA()
                        {
                            DEPTID = entityNguoiNhan.DM_PHONGBAN_ID.GetValueOrDefault(),
                            TEXT = "Sổ văn bản đến " + DateTime.Now.Year,
                            DATA = DateTime.Now.Year,
                            DM_NHOM_ID = dataNhomSoVanBanDen.ID
                        };
                        DM_DANHMUC_DATABusiness.Save(dataSoVanBanDen);
                    }

                    DM_NGUOIDUNG entityNguoiKy = DM_NGUOIDUNGBusiness.Find(entityVanBanDi.NGUOIKY_ID);
                    HSCV_VANBANDEN entityVanBanDen = this.ConvertToVanBanDen(entityVanBanDi, entityNguoiKy, dataSoVanBanDen);
                    HSCV_VANBANDENBusiness.Save(entityVanBanDen);

                    JsonResultBO workflowResult = WF_PROCESSBusiness.AddFlow(entityVanBanDen.ID, MODULE_CONSTANT.VANBANDEN, entityNguoiNhan);
                    if (!workflowResult.Status)
                    {
                        continue;
                    }

                    /**
                         * Cập nhật số theo sổ
                         */
                    dataSoVanBanDen.GHICHU = (dataSoVanBanDen.GHICHU.ToIntOrZero() + 1).ToString();
                    DM_DANHMUC_DATABusiness.Save(dataSoVanBanDen);

                    /**
                     * cập nhật tài liệu đính kèm
                     */
                    List<TAILIEUDINHKEM> groupFiles = TAILIEUDINHKEMBusiness.GetNewestData(entityVanBanDi.ID, LOAITAILIEU.VANBAN);
                    groupForwardFiles.AddRange(this.GenerateFiles(groupFiles, entityVanBanDen));
                    /**
                     * cập nhật elastic search
                     */
                    ElasticSearch.updateListUser(entityVanBanDen.ID.ToString(), new List<long> { entityNguoiNhan.ID }, ElasticType.VanBanDen);

                    /**
                     * gửi email
                     */
                    if (entityNguoiNhan.EMAIL != null && !string.IsNullOrEmpty(entityNguoiNhan.EMAIL))
                    {
                        var ContentEmail = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến <a href='" + SERVERADDRESS + "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + entityVanBanDen.ID.ToString() + "'>" + entityVanBanDen.SOHIEU + "</a>";
                        EmailProvider.SendMailTemplate(currentUser, ContentEmail, ContentEmail, new List<string> { entityNguoiNhan.EMAIL });
                    }

                    //gửi sms
                    if (currentUser.CanSendSMS && entityVanBanDi.CAN_SEND_SMS == true)
                    {
                        if (!string.IsNullOrEmpty(entityNguoiNhan.DIENTHOAI))
                        {
                            var ContentSMS = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến " + entityVanBanDen.SOHIEU;
                            ContentSMS = sms.locDau(ContentSMS);
                            var DoDaiSMS = ContentSMS.Length;
                            string[] noiDung = new string[1];
                            noiDung[0] = ContentSMS;
                            string resultsend = sms.guiTinNhan(entityNguoiNhan.DIENTHOAI, "177403", noiDung);
                            LOGSMS SmsObject = new LOGSMS();
                            SmsObject.SODIENTHOAINHAN = entityNguoiNhan.DIENTHOAI;
                            SmsObject.NOIDUNG = ContentSMS;
                            SmsObject.SOKYTU = DoDaiSMS;
                            SmsObject.KETQUA = resultsend;
                            SmsObject.DONVI_GUI = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                            SmsObject.CREATED_AT = DateTime.Now;
                            SmsObject.CREATED_BY = currentUser.ID;
                            SmsObject.HOTENNGUOIGUI = currentUser.HOTEN;
                            SmsObject.ITEMTYPE = "VANBANDEN";
                            SmsObject.ITEMID = entityVanBanDen.ID;
                            SmsObject.HOTENNGUOINHAN = entityNguoiNhan.HOTEN;
                            SmsObject.DONVI_NHAN = entityNguoiNhan.DM_PHONGBAN_ID.GetValueOrDefault();
                            groupLogSMSs.Add(SmsObject);
                        }
                    }
                    //gửi tin nhắn

                    SYS_TINNHAN noti = new SYS_TINNHAN();
                    noti.FROM_USERNAME = currentUser.HOTEN;
                    noti.FROM_USER_ID = currentUser.ID;
                    noti.NGAYTAO = DateTime.Now;
                    noti.NOIDUNG = currentUser.TenPhongBan + " đã gửi đến bạn một văn bản đến";
                    noti.URL = "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen/" + entityVanBanDen.ID.ToString();
                    noti.TIEUDE = "Xử lý văn bản đến";
                    noti.TO_USER_ID = entityNguoiNhan.ID;
                    SYS_TINNHANBusiness.Save(noti, "", false, entityVanBanDen.ID, TargetDocType.COORDINATED);
                }

                /**
                 * lưu file và log SMS
                 */
                TAILIEUDINHKEMBusiness.repository.InsertRange(groupForwardFiles);
                LogSMSBusiness.repository.InsertRange(groupLogSMSs);
                TAILIEUDINHKEMBusiness.Save();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// @author:duynn
        /// @description: chuyển văn bản đi ==> văn bản đến
        /// </summary>
        /// <param name="entityVanBanDi"></param>
        /// <param name="signer"></param>
        /// <param name="dataSoVanBanDen"></param>
        /// <returns></returns>
        public HSCV_VANBANDEN ConvertToVanBanDen(HSCV_VANBANDI entityVanBanDi, DM_NGUOIDUNG signer, DM_DANHMUC_DATA dataSoVanBanDen)
        {
            HSCV_VANBANDEN result = new HSCV_VANBANDEN();
            result.SOHIEU = entityVanBanDi.SOHIEU;
            result.DOKHAN_ID = entityVanBanDi.DOKHAN_ID;
            result.DOMAT_ID = entityVanBanDi.DOUUTIEN_ID;
            result.LINHVUCVANBAN_ID = entityVanBanDi.LINHVUCVANBAN_ID;
            result.LOAIVANBAN_ID = entityVanBanDi.LOAIVANBAN_ID;
            result.NGAYHET_HIEULUC = entityVanBanDi.NGAYHETHIEULUC;
            result.NGAYTAO = entityVanBanDi.CREATED_AT;
            result.NGAY_HIEULUC = entityVanBanDi.NGAYCOHIEULUC;
            result.NGUOITAO = entityVanBanDi.CREATED_BY;
            result.NOIDUNG = entityVanBanDi.NOIDUNG;
            result.SOHIEU = entityVanBanDi.SOHIEU;
            result.TRICHYEU = entityVanBanDi.TRICHYEU;
            result.VANBANDI_ID = entityVanBanDi.ID;
            result.NGUOIKY = signer != null ? signer.HOTEN : null;
            result.CHUCVU = entityVanBanDi.CHUCVU;
            result.NGAY_BANHANH = DateTime.Now;
            result.SOTRANG = entityVanBanDi.SOTO;
            result.SOVANBANDEN_ID = dataSoVanBanDen.ID;
            result.SODITHEOSO = dataSoVanBanDen.GHICHU.ToIntOrZero().ToString();
            result.SODITHEOSO_NUMBER = dataSoVanBanDen.GHICHU.ToIntOrZero();
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: chuyển các file từ văn bản đi ==> thành văn bản đến
        /// </summary>
        /// <param name="groupFiles"></param>
        /// <param name="entityVanBanDen"></param>
        /// <returns></returns>
        public IEnumerable<TAILIEUDINHKEM> GenerateFiles(List<TAILIEUDINHKEM> groupFiles, HSCV_VANBANDEN entityVanBanDen)
        {
            foreach (var item in groupFiles)
            {
                var file = new TAILIEUDINHKEM();
                file.IS_ACTIVE = item.IS_ACTIVE;
                file.ACCESS_MODIFIER = item.ACCESS_MODIFIER;
                file.CONTENT_CHANGE = item.CONTENT_CHANGE;
                file.DINHDANG_FILE = item.DINHDANG_FILE;
                file.DM_LOAITAILIEU_ID = item.DM_LOAITAILIEU_ID;
                file.DONVI_ID = item.DONVI_ID;
                file.DUONGDAN_FILE = item.DUONGDAN_FILE;
                file.FOLDER_ID = item.FOLDER_ID;
                file.IS_PHEDUYET = item.IS_PHEDUYET;
                file.IS_PRIVATE = item.IS_PRIVATE;
                file.IS_QLPHIENBAN = item.IS_QLPHIENBAN;
                file.KICHCO = item.KICHCO;
                file.MATAILIEU = item.MATAILIEU;
                file.MOTA = item.MOTA;
                file.SOLUONG_DOWNLOAD = item.SOLUONG_DOWNLOAD;
                file.NGAYTAO = DateTime.Now;
                file.NGAYPHATHANH = DateTime.Now;
                file.VERSION = item.VERSION;
                file.TENTACGIA = item.TENTACGIA;
                file.TENTAILIEU = item.TENTAILIEU;
                file.PDF_VERSION = item.PDF_VERSION;
                file.PERMISSION = item.PERMISSION;
                file.TRANGTHAI = item.TRANGTHAI;
                file.ITEM_ID = entityVanBanDen.ID;
                file.USER_ID = file.USER_ID;
                file.LOAI_TAILIEU = LOAITAILIEU.VANBANDEN;
                file.IS_LOCK = file.IS_LOCK;
                file.NGUOI_LOCK = file.NGUOI_LOCK;
                yield return file;
            }
        }


        //màn hình gửi đơn vị khác
    }
}