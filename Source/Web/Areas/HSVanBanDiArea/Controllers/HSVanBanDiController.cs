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
        public ActionResult CreateVB(long id = 0,
            long idVanBanDen = 0,
            int docType = VANBANDI_CONSTANT.CHUA_XULY,
            bool isAllowPublish = false,
            bool isInternal = false)
        {
            AssignUserInfo();

            //khai báo
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            QL_NGUOINHAN_VANBANBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();

            var entityVanBanDi = HSCV_VANBANDIBusiness.Find(id) ?? new HSCV_VANBANDI();
            var entityVanBanDen = HSCV_VANBANDENBusiness.Find(idVanBanDen) ?? new HSCV_VANBANDEN();

            //gán phòng ban cha choi người dùng hiện tại
            currentUser.DeptParentID = currentUser.DeptParentID ?? 0;

            //gán vai trò lãnh đạo đơn vị
            var roleLanhDaoDonVi = DM_VAITROBusiness.GetRoleByCode(CODE_ROLE_LANHDAODONVI);
            int LANHDAODONVI = roleLanhDaoDonVi != null ? roleLanhDaoDonVi.DM_VAITRO_ID : ROLE_LANHDAODONVI.ToIntOrZero();

            //khởi tạo view model
            VanBanDiVM model = new VanBanDiVM()
            {
                IsAllowPublish = isAllowPublish,
                IsInternal = isInternal,
                docType = docType,
                LstNguoiKyVanBan = DM_NGUOIDUNGBusiness.GetUserByRoleAndParentDept(LANHDAODONVI, currentUser.DeptParentID.Value, entityVanBanDi.NGUOIKY_ID.GetValueOrDefault()),
                Recipients = QL_NGUOINHAN_VANBANBusiness.GetRecipientGroups(currentUser.DeptParentID.GetValueOrDefault()),

                LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.DOUUTIEN, currentUser.ID, entityVanBanDi.DOUUTIEN_ID ?? 0),
                LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.DOQUANTRONG, currentUser.ID, entityVanBanDi.DOKHAN_ID ?? 0),
                LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LINHVUCVANBAN, currentUser.ID, entityVanBanDi.LINHVUCVANBAN_ID ?? 0),
                LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LOAIVANBAN, currentUser.ID, entityVanBanDi.LOAIVANBAN_ID ?? 0),
                GroupDeptTypes = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LOAI_COQUAN, currentUser.ID, entityVanBanDi.LOAI_COQUAN_ID ?? 0),
                GroupDocTypes = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.PHANLOAI_VANBAN, currentUser.ID, entityVanBanDi.THONGTIN_LOAI_ID ?? 0),
                GroupDocAuthors = DM_NGUOIDUNGBusiness.GetDanhSachTacGia(currentUser.DeptParentID.Value, entityVanBanDi.TACGIA_ID ?? 0),
                VanBan = entityVanBanDi,
                VanBanDen = entityVanBanDen,
                ListTaiLieu = TAILIEUDINHKEMBusiness.GetNewestData(id, LOAITAILIEU.VANBAN),
                ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(entityVanBanDi.DONVINHAN_INTERNAL_ID?.ToListInt(',')),
                LstReceiveDirectly = DM_NGUOIDUNGBusiness.GetDanhSachByListIds(entityVanBanDi.USER_RECEIVE_DIRECTLY?.ToListLong(','))
            };

            //lấy danh sách người dùng có thể nhận trực tiếp
            model.UsersReceived = new List<long>();

            //hiển thị người nhận văn bản
            //if (model.IsInternal)
            //{
            //    List<DM_NGUOIDUNG_BO> usersReceiveInternal = DM_NGUOIDUNGBusiness.GetUsersReceiveInternal(currentUser);
            //    model.GroupUsersReceiveInternal = usersReceiveInternal.Select(x => new SelectListItem()
            //    {
            //        Value = x.ID.ToString(),
            //        Text = x.HOTEN
            //    }).ToList();
            //    model.GroupUserIdsReceiveInternal = new List<long>();
            //}

            //model.LstReceiveDirectly = new List<SelectListItem>();
            //model.GroupUserIdsReceiveDirectly = new List<long>();
            //HSCV_VANBANDI VanBan = new HSCV_VANBANDI();
            //if (id > 0)
            //{
            //    if (!string.IsNullOrEmpty(VanBan.USER_RECEIVE_DIRECTLY))
            //    {
            //        model.GroupUserIdsReceiveDirectly = VanBan.USER_RECEIVE_DIRECTLY.ToListLong(',');
            //        model.LstReceiveDirectly = DM_NGUOIDUNGBusiness.GetDanhSachByListIds(model.GroupUserIdsReceiveDirectly);
            //    }
            //}
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
        public ActionResult SaveVanBanDi(FormCollection form, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            AssignUserInfo();
            SMSDAL.SendSMSDAL sms = new SMSDAL.SendSMSDAL();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();

            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();

            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            LogSMSBusiness = Get<LogSMSBusiness>();

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
            myModel.Recipients = QL_NGUOINHAN_VANBANBusiness.GetRecipientGroups(currentUser.DeptParentID.GetValueOrDefault(), myModel.GroupUsersReceiveDirectly.Select(x => x.ID).ToList());
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
                }
                else
                {
                    entityVanBanDi.USER_RECEIVE_DIRECTLY = form["USERS_RECEIVE_SPECIAL"];
                }
                HSCV_VANBANDIBusiness.Save(entityVanBanDi);
                this.SaveVanBanPhatHanhToCaNhan(entityVanBanDi, sms);
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
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            LogSMSBusiness = Get<LogSMSBusiness>();

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
                }
                else
                {
                    entityVanBanDi.DONVINHAN_INTERNAL_ID = form["chonphongban"];
                }
                HSCV_VANBANDIBusiness.Save(entityVanBanDi);

                //gửi đơn vị nhận văn bản bên ngoài
                SaveVanBanPhatHanhToDonVi(entityVanBanDi, sms);
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
        /// <returns></returns>
        public bool SaveVanBanPhatHanhToDonVi(HSCV_VANBANDI entityVanBanDi, SMSDAL.SendSMSDAL sms)
        {
            bool result = true;
            try
            {
                DM_NHOMDANHMUC entityNhomDanhMuc = DM_NHOMDANHMUCBusiness.repository.All()
                    .FirstOrDefault(x => x.GROUP_CODE == DMLOAI_CONSTANT.SOVANBANDEN);

                DM_NGUOIDUNG entityNguoiKy = DM_NGUOIDUNGBusiness.Find(entityVanBanDi.NGUOIKY_ID);

                List<TAILIEUDINHKEM> groupFiles = TAILIEUDINHKEMBusiness.GetNewestData(entityVanBanDi.ID, LOAITAILIEU.VANBAN);

                List<int> idsDonViNhan = new List<int>();

                if (!string.IsNullOrEmpty(entityVanBanDi.DONVINHAN_INTERNAL_ID))
                {
                    var idsDonViDaNhan = entityVanBanDi.DONVINHAN_INTERNAL_ID.ToListInt(',');
                    idsDonViNhan.AddRange(idsDonViDaNhan);
                    entityVanBanDi.DONVINHAN_INTERNAL_ID = string.Join(",", idsDonViNhan.ToArray());
                }

                List<CCTC_THANHPHAN> groupDonViNhan = CCTC_THANHPHANBusiness.repository.AllNoTracking
                    .Where(x => idsDonViNhan.Contains(x.ID)).ToList();
                List<LOGSMS> groupLogSMS = new List<LOGSMS>();
                List<TAILIEUDINHKEM> groupForwardFiles = new List<TAILIEUDINHKEM>();
                foreach (var dept in groupDonViNhan)
                {
                    WF_STATE firstState = null;

                    //kiểm tra có phải là gửi nội bộ hay không?
                    bool isSendInternal = true;
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

                    /**
                     *kiểm tra vai trò nhận của trạng thái đầu tiên trong luồng xử lý 
                     */
                    if (firstState == null || firstState.VAITRO_ID == null)
                    {
                        continue;
                    }

                    /**
                     * lấy thông tin người nhận văn bản này
                     */
                    var receiver = DM_NGUOIDUNGBusiness.GetUserByRoleAndDeptId(firstState.VAITRO_ID.Value, dept.ID).FirstOrDefault();
                    if (receiver == null)
                    {
                        continue;
                    }

                    /**
                     * lấy thông tin người xử lý
                     */
                    UserInfoBO processor = DM_NGUOIDUNGBusiness.GetNewUserInfo(receiver.Value.ToLongOrZero());
                    if (processor == null)
                    {
                        continue;
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


                    if (isSendInternal)
                    {
                        //gửi thông tin văn bản đi nội bộ
                        WF_PROCESSBusiness.AddFlow(entityVanBanDen.ID, MODULE_CONSTANT.VANBANDENNOIBO, processor);
                    }
                    else
                    {
                        //gửi thông tin văn bản cho phòng ban khác
                        WF_PROCESSBusiness.AddFlow(entityVanBanDen.ID, MODULE_CONSTANT.VANBANDEN, processor);

                        ElasticSearch.updateListUser(entityVanBanDen.ID.ToString(), new List<long> { processor.ID }, ElasticType.VanBanDen);

                        //gửi email
                        if (!string.IsNullOrEmpty(processor.EMAIL))
                        {
                            var ContentEmail = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến <a href='" + SERVERADDRESS + "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + entityVanBanDen.ID.ToString() + "'>" + entityVanBanDen.SOHIEU + "</a>";
                            EmailProvider.SendMailTemplate(currentUser, ContentEmail, ContentEmail, new List<string> { processor.EMAIL });
                        }

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

                    TAILIEUDINHKEMBusiness.repository.InsertRange(groupForwardFiles);
                    LogSMSBusiness.repository.InsertRange(groupLogSMS);

                    TAILIEUDINHKEMBusiness.repository.Save();
                }
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
        /// <returns></returns>
        public bool SaveVanBanPhatHanhToCaNhan(HSCV_VANBANDI entityVanBanDi, SMSDAL.SendSMSDAL sms)
        {
            List<TAILIEUDINHKEM> groupForwardFiles = new List<TAILIEUDINHKEM>();
            List<LOGSMS> groupLogSMSs = new List<LOGSMS>();
            var idsNguoiNhanDichDanh = entityVanBanDi.USER_RECEIVE_DIRECTLY.ToListLong(',');
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

    }
}