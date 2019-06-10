using Business.Business;
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
using Business.CommonBusiness;
using Web.Areas.CongViecArea.Models;
using Web.Areas.QuanLyCongViec.Models;
using Web.Common;
using Web.Custom;
using Web.FwCore;
using Web.Models;
using Web.Common.Elastic;
using Web.Filter;
using CommonHelper.Upload;
using CommonHelper.Excel;
using Business.CommonModel.HSCVSUBTASK;
using Business.CommonModel.DMNguoiDung;
using System.IO;

namespace Web.Areas.QuanLyCongViec.Controllers
{
    public class QuanLyCongViecController : BaseController
    {
        // GET: QuanLyCongViec/QuanLyCongViec
        #region khai báo business

        private HSCV_VANBANDENBusiness HSCV_VANBANDENBusiness;
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private HSCV_CONGVIECBusiness HSCV_CONGVIECBusiness;
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string CongViecExtension = WebConfigurationManager.AppSettings["CongViecExtension"];
        private int CongViecSize = int.Parse(WebConfigurationManager.AppSettings["CongViecSize"]);
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private PHIEUDANHGIACONGVIECBusiness PHIEUDANHGIACONGVIECBusiness;
        private HSCV_TRINHDUYETCONGVIECBusiness HSCV_TRINHDUYETCONGVIECBusiness;
        private HSCV_CONGVIEC_XINLUIHANBusiness HSCV_CONGVIEC_XINLUIHANBusiness;
        private HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness;
        private HSCV_SUBTASKBusiness HSCV_SUBTASKBusiness;
        private HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness;
        private HSCV_CAPNHATTIENDO_CVBusiness HSCV_CAPNHATTIENDO_CVBusiness;
        private SYS_TINNHANBusiness SYS_TINNHANBusiness;
        private DM_THAOTACBusiness DM_THAOTACBusiness;
        private VAITRO_THAOTACBusiness VAITRO_THAOTACBusiness;
        private DM_NGUOIDUNG_THAOTACBusiness DM_NGUOIDUNG_THAOTACBusiness;
        private NGUOIDUNG_VAITROBusiness NGUOIDUNG_VAITROBusiness;
        private HSCV_CONGVIEC_LAPKEHOACHBusiness HSCV_CONGVIEC_LAPKEHOACHBusiness;
        private const string targetScreen = "DetailTaskScreen";
        private string CODE_ROLE_BANTONGGIAMDOC = WebConfigurationManager.AppSettings["CODE_ROLE_BANTONGGIAMDOC"];
        private string UPLOADFOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string WEB_ADDRESS = WebConfigurationManager.AppSettings["WebAddress"];
        #endregion
        #region CRUD công việc
        [ActionAudit]
        public ActionResult Index()
        {
            CongViecIndexViewModel model = new CongViecIndexViewModel();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC_SEARCH searchModel = new HSCV_CONGVIEC_SEARCH();
            AssignUserInfo();
            searchModel.LOAI_CONGVIEC = LOAI_CONGVIEC.CANHAN;
            searchModel.USER_ID = currentUser.ID;
            searchModel.pageSize = MaxPerpage;
            SessionManager.SetValue("CongViecSearchModel", searchModel);
            var ListCongViec = HSCV_CONGVIECBusiness.GetDaTaByPage(searchModel, MaxPerpage);
            model.ListResult = ListCongViec;
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, 0);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.UserInfo = currentUser;
            return View(model);
        }
        /// <summary>
        /// Form tạo mới công việc
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult Create(int Type = 0, long Id = 0)
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CongViecViewModel model = new CongViecViewModel();
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, currentUser.ID);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID);
            model.UserInfo = currentUser;
            model.TYPE = Type; // văn bản đến hay văn bản đi
            if (Type == 1)
            {
                // văn bản đến
                HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
                if (Id > 0)
                {
                    var TmpVanBanDen = HSCV_VANBANDENBusiness.Find(Id);
                    if (TmpVanBanDen != null)
                    {
                        model.VanBanDenLienQuan = TmpVanBanDen;
                        model.IdVanBanLienQuan = Id;
                    }
                }
                
            }
            else if (Type == 2)
            {
                // văn bản đi
                HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                if (Id > 0)
                {
                    var TmpVanBanDi = HSCV_VANBANDIBusiness.FindById(Id);
                    if (TmpVanBanDi != null)
                    {
                        model.VanBanDiLienQuan = TmpVanBanDi;
                        model.IdVanBanLienQuan = Id;
                    }
                }
            }
            model.CongViec = new HSCV_CONGVIEC();
            model.ListTaiLieu = new List<TAILIEUDINHKEM>();
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            #region Check role
            if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_DONVI))
            {
                //Ban giám đốc
                model.ROLE = Permission.GIAOVIEC_DONVI;
                model.ROLE = "";
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_DONVI);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_PHONGBAN))
            {
                //trưởng đơn vị + giám đốc đơn vị
                model.ROLE = Permission.GIAOVIEC_PHONGBAN;
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_DONVI);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_CANHAN))
            {
                //Trưởng phòng
                model.ROLE = Permission.GIAOVIEC_CANHAN;
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_PHONGBAN);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            else
            {
                //Nhân viên
                model.ROLE = Permission.GIAOVIEC_CANHAN;
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_CANHAN);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            #endregion
            model.userInfo = currentUser;
            model.NguoiDung = new DM_NGUOIDUNG();
            return View(model);
        }

        [ActionAudit]
        public ActionResult Edit(long id)
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            CongViecViewModel model = new CongViecViewModel();
            model.UserInfo = currentUser;
            //model.TYPE = Type; // văn bản đến hay văn bản đi
            var CongViec = HSCV_CONGVIECBusiness.Find(id);
            if (CongViec == null || (currentUser.ID != CongViec.NGUOITAO && (CongViec.NGUOIGIAOVIEC_ID != null && currentUser.ID != CongViec.NGUOIGIAOVIEC_ID)))
            {
                CongViec = new HSCV_CONGVIEC();
            }
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, currentUser.ID, CongViec.DOKHAN.HasValue ? CongViec.DOKHAN.Value : 0);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID, CongViec.DOUU_TIEN.HasValue ? CongViec.DOUU_TIEN.Value : 0);
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            model.ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.CONGVIEC);
            #region Check role
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_DONVI))
            {
                //Ban giám đốc
                model.ROLE = Permission.GIAOVIEC_DONVI;
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_DONVI);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_PHONGBAN))
            {
                //trưởng đơn vị + giám đốc đơn vị
                model.ROLE = Permission.GIAOVIEC_PHONGBAN;
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_DONVI);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_CANHAN))
            {
                //Trưởng phòng
                model.ROLE = Permission.GIAOVIEC_CANHAN;
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_PHONGBAN);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            else
            {
                //Nhân viên
                model.ROLE = "";
                var ListThaoTac = DM_THAOTACBusiness.GetDataByCode(Permission.GIAOVIEC_CANHAN);
                var ListVaiTro = VAITRO_THAOTACBusiness.GetDataByThaoTacId(ListThaoTac.Select(x => x.DM_THAOTAC_ID).ToList());
                string role = "";
                foreach (var item in ListVaiTro)
                {
                    role += item.VAITRO_ID.Value + ",";
                }
                model.LIST_ROLE = role;
            }
            #endregion
            DM_NGUOIDUNG NguoiDung = new DM_NGUOIDUNG();
            if (CongViec.NGUOIGIAOVIEC_ID.HasValue)
            {
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                NguoiDung = DM_NGUOIDUNGBusiness.Find(CongViec.NGUOIGIAOVIEC_ID);
                if (NguoiDung == null)
                {
                    NguoiDung = new DM_NGUOIDUNG();
                }
            }
            model.NguoiDung = NguoiDung;
            model.CongViec = CongViec;
            model.userInfo = currentUser;
            return View("Create", model);
        }

        [ActionAudit]
        public ActionResult Detail(long id)
        {
            #region khoi tao business
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
            HSCV_CONGVIEC_XINLUIHANBusiness = Get<HSCV_CONGVIEC_XINLUIHANBusiness>();
            HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            HSCV_SUBTASKBusiness = Get<HSCV_SUBTASKBusiness>();
            HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness = Get<HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
            HSCV_TRINHDUYETCONGVIECBusiness = Get<HSCV_TRINHDUYETCONGVIECBusiness>();
            HSCV_CAPNHATTIENDO_CVBusiness = Get<HSCV_CAPNHATTIENDO_CVBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_CONGVIEC_LAPKEHOACHBusiness = Get<HSCV_CONGVIEC_LAPKEHOACHBusiness>();
            AssignUserInfo();
            #endregion
            CongViecModal cvModal = new CongViecModal();
            cvModal.CongViec = HSCV_CONGVIECBusiness.Find(id);
            #region kiểm tra người giao việc, người xử lý, người tham gia
            cvModal.IsNguoiGiaoViec = false;
            cvModal.IsNguoiThucHienChinh = false;
            cvModal.IsNguoiThamgia = false;
            if (cvModal.CongViec.NGUOIGIAOVIEC_ID == currentUser.ID)
            {
                cvModal.IsNguoiGiaoViec = true;
            }
            if (cvModal.CongViec.NGUOIXULYCHINH_ID == currentUser.ID)
            {
                cvModal.IsNguoiThucHienChinh = true;
            }

            var checkThamGia = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.repository.All().Where(x => x.CONGVIEC_ID == id).Select(x => x.USER_ID).ToList();
            if (checkThamGia.Contains(currentUser.ID))
            {
                cvModal.IsNguoiThamgia = true;
            }
            #endregion

            if (cvModal.CongViec == null)
            {
                return RedirectToAction("Index");
            }
            List<long> ListParentId = new List<long>();
            if (!string.IsNullOrEmpty(cvModal.CongViec.CONGVIEC_LIENQUAN_ID))
            {
                //danh sách công việc cha
                ListParentId = cvModal.CongViec.CONGVIEC_LIENQUAN_ID.ToListLong(',');
            }
            ListParentId.Add(id);
            if (ListParentId.Any() && currentUser.ID != cvModal.CongViec.NGUOIGIAOVIEC_ID
                && currentUser.ID != cvModal.CongViec.NGUOITAO)
            {
                //danh sách người tham gia xử lý của các công việc cha
                var ListThamGia = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.GetData(ListParentId, currentUser.ID);
                //danh sách người xử lý chính của các công việc cha
                var ListXuLyChinh = HSCV_CONGVIECBusiness.GetData(ListParentId, currentUser.ID);
                #region check quyền truy cập công việc
                // check có phải co quyen ban lanh dao tap doan hay ko
                bool checkRoleLanhDao = false;
                var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
                var roleBanLanhDaoTapDoan = DM_VAITROBusiness.repository.All().Where(x => x.MA_VAITRO == CODE_ROLE_BANTONGGIAMDOC).FirstOrDefault();
                if (roleBanLanhDaoTapDoan != null)
                {
                    var lstBanLanhDao = DM_NGUOIDUNGBusiness.GetUserByRoleBanLanhDao(roleBanLanhDaoTapDoan.DM_VAITRO_ID);
                    if (lstBanLanhDao != null)
                    {
                        checkRoleLanhDao = lstBanLanhDao.Where(x => x.Value == currentUser.ID.ToString()).FirstOrDefault() != null;
                    }
                }
                // check sau

                #endregion
                if (!ListThamGia.Any() && !ListXuLyChinh.Any() && !checkRoleLanhDao)
                {
                    return RedirectToAction("Index");
                }
            }
            #region danh sách tài liệu đính kèm
            cvModal.ListTaiLieuDinhKem = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.CONGVIEC);
            #endregion

            #region danh sách xin lùi hạn
            cvModal.LstXinLuiHan = HSCV_CONGVIEC_XINLUIHANBusiness.GetListByCongViec(id);
            #endregion

            #region người tham gia
            //var LstNguoiThamgia = DM_NGUOIDUNGBusiness.repository.All().Where(x => checkThamGia.Contains(x.ID)).Select(x => x.HOTEN + "(" + x.TENDANGNHAP + ")").ToList();
            //cvModal.LstNguoiThamGia = LstNguoiThamgia;
            cvModal.LstNewNguoiThamGia = DM_NGUOIDUNGBusiness.repository.All().Where(x => checkThamGia.Contains(x.ID)).ToList();
            #endregion

            #region người xử lý
            if (cvModal.CongViec.NGUOIXULYCHINH_ID != null)
            {
                var NguoiXuLyObj = DM_NGUOIDUNGBusiness.Find(cvModal.CongViec.NGUOIXULYCHINH_ID);
                cvModal.NGUOIXULYCHINH = NguoiXuLyObj.HOTEN + "(" + NguoiXuLyObj.TENDANGNHAP + ")";
            }
            #endregion

            #region thông tin người giao việc
            var NguoiGiao = DM_NGUOIDUNGBusiness.Find(cvModal.CongViec.NGUOIGIAOVIEC_ID);
            if (NguoiGiao != null)
            {
                cvModal.NGUOIGIAOVIEC = NguoiGiao.HOTEN + "(" + NguoiGiao.TENDANGNHAP + ")";
            }
            #endregion

            #region sub-task
            //cvModal.LstImportantTask = HSCV_SUBTASKBusiness.getSubTask(id, 1, 0).ToList();
            //cvModal.LstNormalTask = HSCV_SUBTASKBusiness.getSubTask(id, 0, 0).ToList();
            cvModal.LstTask = HSCV_SUBTASKBusiness.getSubTaskForJob(id, 0).ToList();
            cvModal.LstCompletedTask = HSCV_SUBTASKBusiness.getSubTask(id, 1).ToList();
            #endregion
            #region danh sách cập nhật tiến độ công việc
            cvModal.LstCapNhat = HSCV_CAPNHATTIENDO_CVBusiness.repository.All().Where(x => x.CONGVIEC_ID == id).ToList();
            #endregion

            #region nội dung trao đổi công việc
            cvModal.LstNoiDungTraoDoi = HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness.GetListCommentByCongViecId(id);
            cvModal.LstRootComment = cvModal.LstNoiDungTraoDoi.Where(x => x.REPLY_ID == null).OrderByDescending(x => x.NGAYTAO).ToList();
            List<long> LstRootCommentIds = cvModal.LstRootComment.Select(x => x.ID).ToList();
            var LstTaiLieuComment = TAILIEUDINHKEMBusiness.GetDataByItemID(LstRootCommentIds, LOAITAILIEU.NOIDUNGTRAODOICONGVIEC);
            cvModal.LstTaiLieuComment = LstTaiLieuComment;
            #endregion

            #region thông tin độ ưu tiên và độ khẩn

            var DoUuTien = DM_DANHMUC_DATABusiness.Find(cvModal.CongViec.DOUU_TIEN);
            if (DoUuTien != null)
            {
                cvModal.DOUUTIEN = DoUuTien.TEXT;
            }
            var DoKhan = DM_DANHMUC_DATABusiness.Find(cvModal.CongViec.DOKHAN);
            if (DoKhan != null)
            {
                cvModal.DOKHAN = DoKhan.TEXT;
            }
            #endregion

            #region thông tin kế hoạch công việc
            if (true == cvModal.CongViec.IS_HASPLAN)
            {
                cvModal.LstKeHoachCongViec = HSCV_CONGVIEC_LAPKEHOACHBusiness.repository.All().Where(x => x.CONGVIEC_ID == id).OrderByDescending(x => x.ID).ToList();
                var lstKeHoachIds = cvModal.LstKeHoachCongViec.Select(x => x.ID).ToList();
                cvModal.ListTaiLieuDinhKemKeHoach = TAILIEUDINHKEMBusiness.GetDataByItemID(lstKeHoachIds, LOAITAILIEU.PLANCONGVIEC);
                // Kiểm tra xem kế hoạch đã được trình hay chưa
                // Nếu tồn tại 1 bản ghi chưa được trình thì phải trình trước
                if (cvModal.LstKeHoachCongViec.Count > 0)
                {
                    var CheckTrinhPlan = cvModal.LstKeHoachCongViec.Where(x => x.NGAYTRINHKEHOACH == null)
                        .FirstOrDefault();
                    if (CheckTrinhPlan != null)
                    {
                        // Vẫn có bản ghi chưa trình thì trạng thái sẽ là chưa trình kế hoạch
                        cvModal.TrangThaiKeHoach = PLANJOB_CONSTANT.CHUATRINHKEHOACH;
                    }
                    else
                    {
                        // Đã trình hết rồi sẽ có 2 trường hợp: 1 - Lãnh đạo chưa phê duyệt - 2 lãnh đạo đã phê duyệt
                        var CheckTrangThai =
                            cvModal.LstKeHoachCongViec.Where(x => x.ISAPPROVE == null)
                                .FirstOrDefault(); // vẫn còn bản ghi chưa được duyệt
                        if (CheckTrangThai != null)
                        {
                            cvModal.TrangThaiKeHoach = PLANJOB_CONSTANT.DATRINHKEHOACH;
                        }
                        else
                        {
                            // Kiểm tra xem có plan nào được phê duyệt chưa ? nếu chưa thì cần lập lại plan
                            var CheckApprove = cvModal.LstKeHoachCongViec.Where(x => x.ISAPPROVE == true)
                                .FirstOrDefault();
                            if (CheckApprove != null)
                            {
                                cvModal.TrangThaiKeHoach = PLANJOB_CONSTANT.DAPHEDUYETKEHOACH;
                            }
                            else
                            {
                                cvModal.TrangThaiKeHoach = PLANJOB_CONSTANT.LAPLAIKEHOACH;
                            }

                        }
                    }
                }
                else
                {
                    cvModal.TrangThaiKeHoach = PLANJOB_CONSTANT.CHUALAPKEHOACH;
                    cvModal.LstKeHoachCongViec = new List<HSCV_CONGVIEC_LAPKEHOACH>();
                }
            }
            #endregion

            if (cvModal.CongViec.PHANTRAMHOANTHANH == 100)
            {
                #region lấy thông tin phiếu đánh giá
                cvModal.PhieuDanhGia = PHIEUDANHGIACONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == id).FirstOrDefault();
                #endregion
            }
            #region thông tin trình duyệt công việc
            cvModal.LstTrinhDuyet = HSCV_TRINHDUYETCONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == id).ToList();
            #endregion
            #region check quyền có được giao việc hay không ?
            cvModal.HasRoleAssignDepartment = false;
            cvModal.HasRoleAssignChuyenVien = false;
            cvModal.HasRoleAssignTask = false;
            if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_DONVI))
            {
                //Ban giám đốc
                cvModal.HasRoleAssignDepartment = true;
                cvModal.HasRoleAssignTask = true;
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_PHONGBAN))
            {
                //trưởng đơn vị + giám đốc đơn vị
                cvModal.HasRoleAssignDepartment = true;
                cvModal.HasRoleAssignTask = true;
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_CANHAN))
            {
                //Trưởng phòng
                cvModal.HasRoleAssignChuyenVien = true;
                cvModal.HasRoleAssignTask = true;
            }
            #endregion
            #region list độ khẩn và độ ưu tiên
            cvModal.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, currentUser.ID);
            cvModal.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID);
            #endregion
            #region danh gia cong viec
            cvModal.LstDanhGiaCongViec = DM_DANHMUC_DATABusiness.DsByMaNhom("DANHGIACONGVIEC", 0);
            cvModal.LstDanhGiaCongViecObj = DM_DANHMUC_DATABusiness.GetDataByCode("DANHGIACONGVIEC");
            #endregion

            #region Thông tin văn bản liên quan

            if (cvModal.CongViec.VANBANDI_ID != null && cvModal.CongViec.VANBANDI_ID > 0)
            {
                HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                var TmpVanBanDi = HSCV_VANBANDIBusiness.FindById(cvModal.CongViec.VANBANDI_ID.Value);
                if (TmpVanBanDi != null)
                {
                    cvModal.VanBanDiLienQuan = TmpVanBanDi;
                }
                
            }
            if (cvModal.CongViec.VANBANDEN_ID != null && cvModal.CongViec.VANBANDEN_ID > 0)
            {
                HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
                var TmpVanBanDen = HSCV_VANBANDENBusiness.Find(cvModal.CongViec.VANBANDEN_ID.Value);
                if (TmpVanBanDen != null)
                {
                    cvModal.VanBanDenLienQuan = TmpVanBanDen;
                }
            }
            #endregion
            return View(cvModal);
        }
        [ValidateInput(false)]
        [ActionAudit]
        public JsonResult SaveCongViec(HSCV_CONGVIEC CongViec, FormCollection col, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            AssignUserInfo();
            #region gán dữ liệu
            string NGAYBATDAU = "";
            string NGAYKETTHUC = col["NGAYKETTHUC"];
            string MUCTIEU_CONGVIEC = col["MUCTIEU_CONGVIEC"];
            string CACBUOC_THUCHIEN = col["CACBUOC_THUCHIEN"];
            bool IS_HASPLAN = col["IS_HASPLAN"].ToIntOrZero() == 1;
            bool IS_EMAIL = col["IS_EMAIL"].ToIntOrZero() == 1;
            bool IS_SMS = col["IS_SMS"].ToIntOrZero() == 1;
            bool IS_POPUP = col["IS_POPUP"].ToIntOrZero() == 1;
            int RELATEDTYPE = col["RELATEDTYPE"].ToIntOrZero();
            
            CongViec.IS_HASPLAN = IS_HASPLAN;
            CongViec.IS_EMAIL = IS_EMAIL;
            CongViec.IS_SMS = IS_SMS;
            CongViec.IS_POPUP = IS_POPUP;
            if (!string.IsNullOrEmpty(col["NGAYBATDAU"]))
            {
                NGAYBATDAU = col["NGAYBATDAU"].Trim();
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

            if (!string.IsNullOrEmpty(NGAYBATDAU))
            {
                CongViec.NGAY_NHANVIEC = NGAYBATDAU.ToDateTime();
            }

            CongViec.NGAYHOANTHANH_THEOMONGMUON = NGAYKETTHUC.ToDateTime();
            CongViec.IS_BATDAU = false;
            #endregion
            List<CommonError> ListError = IsValid(CongViec, NGAYKETTHUC);
            if (ListError.Any())
            {
                return Json(new { Type = "INVALID", Message = ListError }, JsonRequestBehavior.AllowGet);
            }
            if (0 < CongViec.NGUOIGIAOVIEC_ID)
            {
                if (CongViec.NGUOIXULYCHINH_ID == null)
                {
                    CongViec.NGUOIXULYCHINH_ID = currentUser.ID;
                }

                CongViec.TRANGTHAI_ID = TrangThaiCongViecConstant.PENDING;
                CongViec.IS_ASSIGNED = true;
                CongViec.DAGIAOVIEC = true;
            }
            else
            {
                CongViec.NGUOIGIAOVIEC_ID = currentUser.ID;
                //CongViec.NGUOIXULYCHINH_ID = currentUser.ID;
                CongViec.TRANGTHAI_ID = TrangThaiCongViecConstant.APPROVED;
            }

            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            UploadFileTool tool = new UploadFileTool();
            if (CongViec.ID > 0)
            {
                #region Cập nhật công việc cá nhân
                var result = HSCV_CONGVIECBusiness.Find(CongViec.ID);
                if (result == null || (currentUser.ID != result.NGUOITAO && result.NGUOIGIAOVIEC_ID != null && currentUser.ID != result.NGUOIGIAOVIEC_ID))
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
                result.NGUOIXULYCHINH_ID = CongViec.NGUOIXULYCHINH_ID;
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
                List<long> ListUser = new List<long>();
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(CongViec.NGUOIGIAOVIEC_ID);
                ListUser.Add(currentUser.ID);
                ListUser.Add(CongViec.NGUOIGIAOVIEC_ID.HasValue ? CongViec.NGUOIGIAOVIEC_ID.Value : 0);
                ElasticModel model = ElasticModel.ConvertJob(result, ListUser, NguoiDung != null ? NguoiDung.HOTEN : null);
                ElasticSearch.updateDocument(model, model.Id.ToString(), ElasticType.CongViec);
                return Json(new { Type = "SUCCESS", Message = "Cập nhật công việc cá nhân thành công", ID = result.ID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                #region Thêm mới công việc cá nhân
                CongViec.NGAYTAO = DateTime.Now;
                CongViec.NGUOITAO = currentUser.ID;
                CongViec.NGAYSUA = DateTime.Now;
                CongViec.HAS_FILE = filebase.Any() ? 1 : 0;
                CongViec.IS_MYJOB = true;
                // Cập nhật thông tin văn bản liên quan
                if (RELATEDTYPE == 1)
                {
                    HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
                    var VanBanDenId = col["RELATEDVANBAN"].ToLongOrZero();
                    if (VanBanDenId > 0)
                    {
                        var TmpVanBanDen = HSCV_VANBANDENBusiness.Find(VanBanDenId);
                        if (TmpVanBanDen != null)
                        {
                            CongViec.VANBANDEN_ID = VanBanDenId;
                        }
                    }

                }
                else if (RELATEDTYPE == 2)
                {
                    HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                    var VanBanDiId = col["RELATEDVANBAN"].ToLongOrZero();
                    if (VanBanDiId > 0)
                    {
                        var TmpVanBanDi = HSCV_VANBANDIBusiness.Find(VanBanDiId);
                        if (TmpVanBanDi != null)
                        {
                            CongViec.VANBANDI_ID = VanBanDiId;
                        }
                    }
                }
                // End
                HSCV_CONGVIECBusiness.Save(CongViec);
                tool.UploadCustomFileVer3(filebase, true, CongViecExtension, URL_FOLDER, CongViecSize, FOLDER_ID, filename, CongViec.ID, LOAITAILIEU.CONGVIEC, "Công việc", currentUser);
                #endregion
                List<long> ListUser = new List<long>();
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(CongViec.NGUOIGIAOVIEC_ID);
                ListUser.Add(currentUser.ID);
                ListUser.Add(CongViec.NGUOIGIAOVIEC_ID.HasValue ? CongViec.NGUOIGIAOVIEC_ID.Value : 0);
                ElasticModel model = ElasticModel.ConvertJob(CongViec, ListUser, NguoiDung != null ? NguoiDung.HOTEN : null);
                ElasticSearch.insertDocument(model, model.Id.ToString(), ElasticType.CongViec);
                if (currentUser.ID != CongViec.NGUOIGIAOVIEC_ID)
                {
                    sendNotification(CongViec.ID, currentUser.HOTEN + " đã tạo 1 công việc bạn giao xuống?", "THÔNG BÁO GIAO VIỆC", TargetDocType.ASSIGNED);
                }
            }
            return Json(new { Type = "SUCCESS", Message = "Thêm mới công việc cá nhân thành công", ID = CongViec.ID }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Các hàm private
        private List<CommonError> IsValid(HSCV_CONGVIEC CongViec, string NGAYKETTHUC)
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
            //if (string.IsNullOrEmpty(NGAYBATDAU))
            //{
            //    error = new CommonError();
            //    error.Field = "NGAYBATDAU";
            //    error.Message = "Bạn chưa nhập ngày bắt đầu";
            //    ListError.Add(error);
            //}
            //else if (!CongViec.NGAY_NHANVIEC.HasValue)
            //{
            //    error = new CommonError();
            //    error.Field = "NGAYBATDAU";
            //    error.Message = "Ngày bắt đầu không tồn tại hoặc không đúng định dạng";
            //    ListError.Add(error);
            //}
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
        #region Các hàm jsonresult
        [ActionAudit]
        public JsonResult deleteNguoiPhoiHop(long userid, long taskid)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC CongViec = HSCV_CONGVIECBusiness.Find(taskid);
            if (CongViec == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy công việc cần xóa" });
            }
            AssignUserInfo();
            if (currentUser.ID != CongViec.NGUOIGIAOVIEC_ID)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không phải người giao công việc này" });
            }
            HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            var DeleteObj = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.repository.All().Where(x => x.CONGVIEC_ID == taskid && x.USER_ID == userid).FirstOrDefault();
            if (DeleteObj != null)
            {
                HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.repository.Delete(DeleteObj.ID);
                HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.Save();
                return Json(new { Type = "SUCCESS", Message = "Xóa người tham gia thành công" });
            }
            return Json(new { Type = "ERROR", Message = "Người tham gia này đã bị xóa" });

        }
        [ActionAudit]
        public JsonResult deleteTask(long id)
        {
            HSCV_SUBTASKBusiness = Get<HSCV_SUBTASKBusiness>();
            HSCV_SUBTASK task = HSCV_SUBTASKBusiness.Find(id);
            if (task == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy công việc cần xóa" });
            }
            AssignUserInfo();
            if (task.NGUOITAO != null && currentUser.ID != task.NGUOITAO)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không có quyền xóa công việc này" });
            }
            if (true == task.DAGIAOVIEC)
            {
                return Json(new { Type = "ERROR", Message = "Công việc đã giao, bạn không thể xóa" });
            }
            HSCV_SUBTASKBusiness.repository.Delete(id);
            HSCV_SUBTASKBusiness.Save();
            return Json(new { Type = "SUCCESS", Message = "Xóa công việc thành công" });
        }
        [ActionAudit]
        public JsonResult Delete(long id)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC CongViec = HSCV_CONGVIECBusiness.Find(id);
            if (CongViec == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy công việc cần xóa" });
            }
            AssignUserInfo();
            if (currentUser.ID != CongViec.NGUOITAO)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không có quyền xóa công việc này" });
            }
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            List<TAILIEUDINHKEM> ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.CONGVIEC);
            FileUltilities file = new FileUltilities();
            foreach (var item in ListTaiLieu)
            {
                file.RemoveFile(URL_FOLDER + "/" + item.DUONGDAN_FILE);
                TAILIEUDINHKEMBusiness.repository.Delete(item.TAILIEU_ID);
            }
            TAILIEUDINHKEMBusiness.Save();
            HSCV_CONGVIECBusiness.repository.Delete(id);
            HSCV_CONGVIECBusiness.Save();
            ElasticSearch.deleteDocument(id.ToString(), ElasticType.CongViec);
            return Json(new { Type = "SUCCESS", Message = "Xóa công việc thành công" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var searchModel = SessionManager.GetValue("CongViecSearchModel") as HSCV_CONGVIEC_SEARCH;
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
            else
            {
                searchModel.DOKHAN = null;
            }
            if (!string.IsNullOrEmpty(DOMAT_ID))
            {
                searchModel.DO_UUTIEN = DOMAT_ID.ToLongOrNULL();
            }
            else
            {
                searchModel.DO_UUTIEN = null;
            }
            #endregion
            SessionManager.SetValue("CongViecSearchModel", searchModel);
            var data = HSCV_CONGVIECBusiness.GetDaTaByPage(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var searchModel = SessionManager.GetValue("CongViecSearchModel") as HSCV_CONGVIEC_SEARCH;
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
        #region các hàm bổ trợ trong công việc

        #region các hàm về comment công việc
        [HttpPost]
        [ActionAudit]
        public ActionResult SaveCommentRootLevel(IEnumerable<HttpPostedFileBase> FILECOMMENTFORTASK, string[] filename, FormCollection col)
        {
            AssignUserInfo();
            HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness = Get<HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            if (!string.IsNullOrEmpty(col["COMMENTFORTASK"]))
            {
                var CongViecId = col["COMMENTFORTASK"].ToIntOrZero();
                HSCV_CONGVIEC congViecObj = HSCV_CONGVIECBusiness.Find(CongViecId);
                if (congViecObj != null)
                {
                    int REPLY_ID = 0;
                    if (!string.IsNullOrEmpty(col["REPLY_ID"]))
                    {
                        REPLY_ID = col["REPLY_ID"].ToIntOrZero();
                    }
                    HSCV_CONGVIEC_NOIDUNGTRAODOI comment = new HSCV_CONGVIEC_NOIDUNGTRAODOI();
                    comment.CONGVIEC_ID = CongViecId;
                    comment.NOIDUNG = col["NOIDUNGTRAODOIFORTASK"];
                    comment.USER_ID = (long)currentUser.ID;
                    comment.CREATED_AT = DateTime.Now;
                    HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness.Save(comment);
                    UploadFileTool upload = new UploadFileTool();
                    upload.UploadCustomFile(FILECOMMENTFORTASK, true, CongViecExtension, URL_FOLDER, CongViecSize, null, filename, comment.ID, LOAITAILIEU.NOIDUNGTRAODOICONGVIEC, "Công việc");

                    #region notification
                    string msg = currentUser.HOTEN + " đã đăng trao đổi công việc #Công việc " + CongViecId.ToString();
                    sendNotification(CongViecId, msg, "Trao đổi công việc", TargetDocType.PRIVATE);
                    #endregion

                    return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + CongViecId.ToString());
                }
            }
            return RedirectToAction("Index");
        }
        [ActionAudit]
        public JsonResult SaveReplyForComment(long COMMENT_ID, string COMMENT, long TASK_ID)
        {
            if (string.IsNullOrEmpty(COMMENT) || COMMENT_ID <= 0 || TASK_ID <= 0)
            {
                return Json(false);
            }
            else
            {
                AssignUserInfo();
                HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness = Get<HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness>();
                HSCV_CONGVIEC_NOIDUNGTRAODOI comment = new HSCV_CONGVIEC_NOIDUNGTRAODOI();
                comment.CONGVIEC_ID = TASK_ID;
                comment.NOIDUNG = COMMENT;
                comment.USER_ID = (long)currentUser.ID;
                comment.CREATED_AT = DateTime.Now;
                comment.REPLY_ID = COMMENT_ID;
                HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness.Save(comment);
                #region notification
                string msg = currentUser.HOTEN + " đã đăng trao đổi công việc #Công việc " + TASK_ID.ToString();
                sendNotification(TASK_ID, msg, "Trao đổi công việc", TargetDocType.PRIVATE);
                #endregion
                return Json(true);
            }
        }
        #endregion
        #region cập nhật tiến độ công việc
        [ActionAudit]
        public ActionResult UpdateProgressTask(FormCollection col)
        {
            AssignUserInfo();
            HSCV_CAPNHATTIENDO_CVBusiness = Get<HSCV_CAPNHATTIENDO_CVBusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();

            if (!string.IsNullOrEmpty(col["BEGINTASKID"]))
            {
                var CongViecId = col["BEGINTASKID"].ToIntOrZero();
                HSCV_CONGVIEC congViecObj = HSCV_CONGVIECBusiness.Find(CongViecId);
                if (congViecObj != null)
                {
                    if (!string.IsNullOrEmpty(col["PHANTRAMHOANTHANH"]))
                    {
                        var PHANTRAMHOANTHANH = col["PHANTRAMHOANTHANH"].ToIntOrZero();
                        int ROOT_PHANTRAMHOANTHANH = 0;
                        if (congViecObj.PHANTRAMHOANTHANH != null && congViecObj.PHANTRAMHOANTHANH.Value > 0)
                        {
                            ROOT_PHANTRAMHOANTHANH = congViecObj.PHANTRAMHOANTHANH.Value;
                        }

                        if (PHANTRAMHOANTHANH > ROOT_PHANTRAMHOANTHANH)
                        {
                            congViecObj.PHANTRAMHOANTHANH_OLD = ROOT_PHANTRAMHOANTHANH;
                            congViecObj.PHANTRAMHOANTHANH = PHANTRAMHOANTHANH;
                            if (PHANTRAMHOANTHANH == 100)
                            {
                                HSCV_TRINHDUYETCONGVIECBusiness = Get<HSCV_TRINHDUYETCONGVIECBusiness>();
                                HSCV_TRINHDUYETCONGVIEC TrinhDuyet = new HSCV_TRINHDUYETCONGVIEC();
                                TrinhDuyet.CREATED_AT = DateTime.Now;
                                TrinhDuyet.CREATED_BY = currentUser.ID;
                                TrinhDuyet.CONGVIEC_ID = congViecObj.ID;
                                TrinhDuyet.NOIDUNG_TRINHDUYET = col["NOIDUNG_COMMENT"];
                                HSCV_TRINHDUYETCONGVIECBusiness.Save(TrinhDuyet);
                                congViecObj.NGAYKETTHUC_THUCTE = DateTime.Now;
                            }
                            HSCV_CONGVIECBusiness.Save(congViecObj);
                            HSCV_CAPNHATTIENDO_CV proccessCV = new HSCV_CAPNHATTIENDO_CV();
                            proccessCV.CONGVIEC_ID = CongViecId;
                            proccessCV.TIENDOCONGVIEC = PHANTRAMHOANTHANH;
                            proccessCV.NGAYCAPNHATTIENDO = DateTime.Now;
                            proccessCV.NGAYTAO = DateTime.Now;
                            proccessCV.NOIDUNG = col["NOIDUNG_COMMENT"];
                            proccessCV.NGUOITAO = currentUser.HOTEN + "( " + currentUser.TENDANGNHAP + ")";
                            HSCV_CAPNHATTIENDO_CVBusiness.Save(proccessCV);

                            #region notification
                            string msg = currentUser.HOTEN + " đã cập nhật tiến độ #Công việc " + CongViecId.ToString();
                            sendNotification(CongViecId, msg, "CẬP NHẬT TIẾN ĐỘ CÔNG VIỆC", TargetDocType.PRIVATE);
                            #endregion

                            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + CongViecId.ToString());
                        }
                        return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + CongViecId.ToString());
                    }
                    return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + CongViecId.ToString());
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region xin lùi hạn công việc
        [ActionAudit]
        public ActionResult SaveExtendTask(FormCollection col)
        {
            AssignUserInfo();
            HSCV_CONGVIEC_XINLUIHANBusiness = Get<HSCV_CONGVIEC_XINLUIHANBusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            if (string.IsNullOrEmpty(col["EXTEND_HANHOANTHANH"]) || string.IsNullOrEmpty(col["EXTEND_NOIDUNG_COMMENT"]) || string.IsNullOrEmpty(col["EXTEND_TASKID"]))
            {
                return RedirectToAction("Index");
            }

            long TASK_ID = col["EXTEND_TASKID"].ToLongOrZero();
            HSCV_CONGVIEC TaskObj = HSCV_CONGVIECBusiness.Find(TASK_ID);
            if (TaskObj == null)
            {
                return Redirect("/#QuanLyCongViec/QuanLyCongViec");
            }
            DateTime? HANHOANTHANH = col["EXTEND_HANHOANTHANH"].ToDateTime();

            HSCV_CONGVIEC_XINLUIHAN xinluihan = new HSCV_CONGVIEC_XINLUIHAN();
            xinluihan.CONGVIEC_ID = TASK_ID;
            xinluihan.USER_ID = currentUser.ID;
            xinluihan.TIEUDE = "Xin lùi hạn task " + TaskObj.ID.ToString();
            xinluihan.NOIDUNG = col["EXTEND_NOIDUNG_COMMENT"];
            if (TaskObj.IS_HASPLAN == true)
            {
                xinluihan.HANKETTHUCTRUOC = TaskObj.NGAYKETTHUC_KEHOACH;
            }
            else
            {
                xinluihan.HANKETTHUCTRUOC = TaskObj.NGAYHOANTHANH_THEOMONGMUON;
            }
            xinluihan.HANKETHUC = HANHOANTHANH;
            xinluihan.NGAYGUI = DateTime.Now;
            xinluihan.NGAYTAO = DateTime.Now;
            HSCV_CONGVIEC_XINLUIHANBusiness.Save(xinluihan);
            TaskObj.IS_EXTEND_TASK = true;
            HSCV_CONGVIECBusiness.Save(TaskObj);
            #region notification
            string msg = currentUser.HOTEN + " đã xin gia hạn #Công việc " + TaskObj.ID.ToString();
            sendNotification(TASK_ID, msg, "Xin gia hạn công việc", TargetDocType.PRIVATE);
            #endregion
            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString());
        }
        #endregion

        #region phê duyệt đơn xin lùi hạn công việc
        [ActionAudit]
        public JsonResult ApproveExtendTask(long ID, int STATUS, string Mess = "", string NgayExtend = "")
        {
            AssignUserInfo();
            HSCV_CONGVIEC_XINLUIHANBusiness = Get<HSCV_CONGVIEC_XINLUIHANBusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();

            HSCV_CONGVIEC_XINLUIHAN extendTask = HSCV_CONGVIEC_XINLUIHANBusiness.Find(ID);
            if (extendTask == null)
            {
                return Json(false);
            }
            var userID = currentUser.ID;
            #region kiểm tra xem user hiện tại có được quyền phê duyệt hay không
            HSCV_CONGVIEC taskObj = HSCV_CONGVIECBusiness.Find(extendTask.CONGVIEC_ID);
            if (taskObj == null)
            {
                return Json(false);
            }
            if (taskObj.NGUOIGIAOVIEC_ID != userID)
            {
                return Json(false);
            }
            if (STATUS == 0)
            {
                taskObj.IS_EXTEND_TASK = false;
                HSCV_CONGVIECBusiness.Save(taskObj);

                extendTask.IS_APPROVED = false; ;
                if (!string.IsNullOrEmpty(Mess))
                {
                    extendTask.BUTPHELANHDAO = Mess;
                }
                HSCV_CONGVIEC_XINLUIHANBusiness.Save(extendTask);
                #region notification
                string msg = currentUser.HOTEN + " đã từ chối gia hạn #Công việc " + taskObj.ID.ToString();
                sendNotification(taskObj.ID, msg, "TỪ CHỐI GIA HẠN CÔNG VIỆC", TargetDocType.PRIVATE);
                #endregion
            }
            else if (STATUS == 1)
            {
                if (!string.IsNullOrEmpty(NgayExtend))
                {
                    extendTask.HANKETTHUC_LANHDAODUYET = NgayExtend.ToDataTime();
                }
                else
                {
                    extendTask.HANKETTHUC_LANHDAODUYET = extendTask.HANKETHUC.Value;
                }
                if (taskObj.IS_HASPLAN == true)
                {
                    taskObj.NGAYKETTHUC_KEHOACH = extendTask.HANKETTHUC_LANHDAODUYET.Value;
                }
                else
                {
                    taskObj.NGAYHOANTHANH_THEOMONGMUON = extendTask.HANKETTHUC_LANHDAODUYET.Value;
                }

                taskObj.IS_EXTEND_TASK = false;
                HSCV_CONGVIECBusiness.Save(taskObj);
                extendTask.IS_APPROVED = true;
                if (!string.IsNullOrEmpty(Mess))
                {
                    extendTask.BUTPHELANHDAO = Mess;
                }
                HSCV_CONGVIEC_XINLUIHANBusiness.Save(extendTask);
                #region notification
                string msg = currentUser.HOTEN + " đã đồng ý gia hạn #Công việc " + taskObj.ID.ToString();
                sendNotification(taskObj.ID, msg, "ĐỒNG Ý GIA HẠN CÔNG VIỆC", TargetDocType.PRIVATE);
                #endregion
            }
            #endregion
            return Json(true);
        }
        #endregion

        #region Lập kế hoạch thực hiện công việc
        [ValidateInput(false)]
        public ActionResult LapKeHoachCongViec(FormCollection col, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            HSCV_CONGVIEC_LAPKEHOACHBusiness = Get<HSCV_CONGVIEC_LAPKEHOACHBusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();

            HSCV_CONGVIEC_LAPKEHOACH Plan = new HSCV_CONGVIEC_LAPKEHOACH();

            var CongViecID = col["LAPKEHOACHCONGVIEC_ID"].ToLongOrZero();
            HSCV_CONGVIEC CongViecObj = HSCV_CONGVIECBusiness.Find(CongViecID);
            if (CongViecObj != null)
            {
                AssignUserInfo();
                if (CongViecObj.NGUOIXULYCHINH_ID == currentUser.ID || (CongViecObj.NGUOIXULYCHINH_ID == null && CongViecObj.NGUOIGIAOVIEC_ID == currentUser.ID))
                {
                    Plan.CONGVIEC_ID = CongViecID;
                    if (!string.IsNullOrEmpty(col["NGAYBATDAU_KEHOACH"]))
                    {
                        var NGAYBATDAU_KEHOACH_STR = col["NGAYBATDAU_KEHOACH"].Trim();
                        var NGAYBATDAU_KEHOACH = NGAYBATDAU_KEHOACH_STR.ToDateTime();
                        if (NGAYBATDAU_KEHOACH != null)
                        {
                            Plan.NGAYBATDAUTHEOKEHOACH = NGAYBATDAU_KEHOACH.Value;
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                    if (!string.IsNullOrEmpty(col["NGAYKETTHUC_KEHOACH"]))
                    {
                        var NGAYKETTHUC_KEHOACH_STR = col["NGAYKETTHUC_KEHOACH"].Trim();
                        var NGAYKETTHUC_KEHOACH = NGAYKETTHUC_KEHOACH_STR.ToDateTime();
                        if (NGAYKETTHUC_KEHOACH != null)
                        {
                            Plan.NGAYHOANTHANHTHEOKEHOACH = NGAYKETTHUC_KEHOACH.Value;
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                    Plan.CACBUOCTHUCHIEN = col["CACBUOCTHUCHIEN"];
                    Plan.MUCTIEUCONGVIEC = col["MUCTIEU_CONGVIEC"];
                    Plan.CREATED_AT = DateTime.Now;
                    Plan.CREATED_BY = currentUser.ID;
                    HSCV_CONGVIEC_LAPKEHOACHBusiness.Save(Plan);
                    UploadFileTool tool = new UploadFileTool();
                    tool.UploadCustomFileVer3(filebase, true, CongViecExtension, URL_FOLDER, CongViecSize, FOLDER_ID, filename, Plan.ID, LOAITAILIEU.PLANCONGVIEC, "Kế hoạch Công việc", currentUser);


                    HSCV_CONGVIECBusiness.Save(CongViecObj);
                    return RedirectToAction("Detail", new { id = CongViecID });
                }

            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Bắt đầu xử lý công việc
        [ActionAudit]
        public JsonResult BeginProcess(long id)
        {
            AssignUserInfo();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC job = HSCV_CONGVIECBusiness.Find(id);
            if (job != null)
            {
                if (true != job.IS_BATDAU)
                {
                    // nếu công việc này đã được giao thì kiểm tra xem người xử lý chính và người hiện tại có trùng ko thì mới cho chuyển trạng thái
                    if (true == job.DAGIAOVIEC && job.NGUOIXULYCHINH_ID == currentUser.ID)
                    {
                        job.IS_BATDAU = true;
                        job.NGAYBATDAU_THUCTE = DateTime.Now;
                        job.PHANTRAMHOANTHANH = 0;
                        HSCV_CONGVIECBusiness.Save(job);
                        return Json(new { Type = "SUCCESS", Message = "Bạn đã bắt đầu thực hiện công việc" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (true != job.DAGIAOVIEC && job.NGUOIGIAOVIEC_ID == currentUser.ID)
                    {
                        // Công việc chưa được giao
                        // kiểm tra xem người hiện tại có phải người tạo việc ko
                        // nếu đúng cap nhật trạng thái đã xử lý, gán người xử lý là người hiện tại
                        job.IS_BATDAU = true;
                        job.DAGIAOVIEC = true;
                        job.NGUOIXULYCHINH_ID = currentUser.ID;
                        job.NGAYBATDAU_THUCTE = DateTime.Now;
                        job.PHANTRAMHOANTHANH = 0;
                        HSCV_CONGVIECBusiness.Save(job);
                        return Json(new { Type = "SUCCESS", Message = "Bạn đã bắt đầu thực hiện công việc" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { Type = "INVALID", Message = "Không thể bắt đầu được công việc này" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region trình kế hoạch
        [ActionAudit]
        public JsonResult trinhKeHoach(long id)
        {
            HSCV_CONGVIEC_LAPKEHOACHBusiness = Get<HSCV_CONGVIEC_LAPKEHOACHBusiness>();
            var PlanObj = HSCV_CONGVIEC_LAPKEHOACHBusiness.repository.All().Where(x => x.CONGVIEC_ID == id && x.NGAYTRINHKEHOACH == null).FirstOrDefault();
            if (PlanObj != null)
            {
                PlanObj.NGAYTRINHKEHOACH = DateTime.Now;
                HSCV_CONGVIEC_LAPKEHOACHBusiness.Save(PlanObj);
            }
            return Json(new { Type = "SUCCESS", Message = "Trình kế hoạch thành công" });
        }
        #endregion

        #region Duyệt kế hoạch
        [ActionAudit]
        public JsonResult approveKeHoach(long id, int status, string noidung)
        {
            HSCV_CONGVIEC_LAPKEHOACHBusiness = Get<HSCV_CONGVIEC_LAPKEHOACHBusiness>();
            var PlanObj = HSCV_CONGVIEC_LAPKEHOACHBusiness.repository.All().Where(x => x.CONGVIEC_ID == id && x.ISAPPROVE == null && x.NGAYTRINHKEHOACH != null).FirstOrDefault();
            if (PlanObj != null)
            {
                PlanObj.NGAYCAPTRENPHANHOI = DateTime.Now;
                TimeSpan Ts = DateTime.Now - PlanObj.NGAYTRINHKEHOACH.Value;
                PlanObj.SONGAYCHOPHANHOI = Ts.Days;
                PlanObj.ISAPPROVE = (status == 1 ? true : false);
                PlanObj.KETQUATRINHDUYET = (status == 1 ? "Đã phê duyệt " : "Từ chối phê duyệt ") + (!string.IsNullOrEmpty(noidung) ? ("- " + noidung) : "");
                HSCV_CONGVIEC_LAPKEHOACHBusiness.Save(PlanObj);
                HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
                #region Cập nhật công việc
                HSCV_CONGVIEC CongViec = HSCV_CONGVIECBusiness.Find(id);
                if (CongViec != null && status == 1)
                {
                    bool IsChange = false;
                    if (!string.IsNullOrEmpty(PlanObj.MUCTIEUCONGVIEC))
                    {
                        IsChange = true;
                        CongViec.MUCTIEU_CONGVIEC = PlanObj.MUCTIEUCONGVIEC;
                    }
                    if (!string.IsNullOrEmpty(PlanObj.CACBUOCTHUCHIEN))
                    {
                        IsChange = true;
                        CongViec.CACBUOC_THUCHIEN = PlanObj.CACBUOCTHUCHIEN;
                    }
                    if (PlanObj.NGAYBATDAUTHEOKEHOACH != null)
                    {
                        IsChange = true;
                        CongViec.NGAYBATDAU_KEHOACH = PlanObj.NGAYBATDAUTHEOKEHOACH;
                    }
                    if (PlanObj.NGAYHOANTHANHTHEOKEHOACH != null)
                    {
                        IsChange = true;
                        CongViec.NGAYKETTHUC_KEHOACH = PlanObj.NGAYHOANTHANHTHEOKEHOACH;
                    }
                    if (IsChange)
                    {
                        HSCV_CONGVIECBusiness.Save(CongViec);
                    }
                }
                #endregion
            }
            return Json(new { Type = "SUCCESS", Message = "Duyệt hoạch thành công" });
        }
        #endregion

        #region Duyệt kết quả trình duyệt công việc
        [ActionAudit]
        public ActionResult PhanHoiCongViec(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_TRINHDUYETCONGVIECBusiness = Get<HSCV_TRINHDUYETCONGVIECBusiness>();
            long CongViecId = form["PHANHOICONGVIEC_ID"].ToLongOrZero();
            HSCV_CONGVIEC CongViec = HSCV_CONGVIECBusiness.Find(CongViecId);
            if (CongViec != null)
            {
                HSCV_TRINHDUYETCONGVIEC TrinhDuyet = HSCV_TRINHDUYETCONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == CongViecId).OrderByDescending(x => x.ID).FirstOrDefault();
                if (TrinhDuyet != null)
                {
                    TrinhDuyet.NGAYPHANHOI = DateTime.Now;
                    TimeSpan Ts = TrinhDuyet.NGAYPHANHOI.Value - TrinhDuyet.CREATED_AT.Value;
                    TrinhDuyet.SONGAYCHOPHANHOI = Ts.Days;
                    TrinhDuyet.KETQUATRINHDUYET = form["PHANHOICONGVIEC"];
                    int PheDuyet = form["KETQUATIEPNHAN"].ToIntOrZero();
                    if (PheDuyet == 1)
                    {
                        TrinhDuyet.PHEDUYETKETQUA = true;
                        CongViec.NGUOIGIAOVIECDAPHANHOI = true;
                    }
                    else
                    {
                        TrinhDuyet.PHEDUYETKETQUA = false;
                        CongViec.NGAYKETTHUC_THUCTE = null;
                        CongViec.PHANTRAMHOANTHANH = CongViec.PHANTRAMHOANTHANH_OLD;
                    }
                    HSCV_TRINHDUYETCONGVIECBusiness.Save(TrinhDuyet);
                    var currentUser = (UserInfoBO)SessionManager.GetUserInfo();
                    string msg = currentUser.HOTEN + " đã phản hồi công việc #Công việc " + CongViecId.ToString();
                    sendNotification(CongViecId, msg, "PHÊ DUYỆT TIẾN ĐỘ HOÀN THÀNH CÔNG VIỆC", TargetDocType.PRIVATE);
                }
            }

            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + CongViecId.ToString());
        }
        #endregion

        #region đánh giá công việc
        [ActionAudit]
        public ActionResult TuDanhGiaCongViecTheoThangDiem(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
            var DanhMucBusiness = Get<DM_DANHMUC_DATABusiness>();

            var currentUser = (UserInfoBO)SessionManager.GetUserInfo();
            var TaskID = form["TDG_TASKID"].ToLongOrZero();
            if (string.IsNullOrEmpty(form["TUDANHGIATHEOTHANGDIEM"]))
            {
                return RedirectToAction("index");
            }

            var TUDANHGIATHEOTHANGDIEM = form["TUDANHGIATHEOTHANGDIEM"].ToIntOrZero();
            if (TUDANHGIATHEOTHANGDIEM == 0)
            {
                return RedirectToAction("index");
            }
            var DataObj = DanhMucBusiness.repository.Find(TUDANHGIATHEOTHANGDIEM);
            if (DataObj == null)
            {
                return RedirectToAction("index");
            }
            var TaskObj = HSCV_CONGVIECBusiness.Find(TaskID);
            TaskObj.DATUDANHGIA = true;
            HSCV_CONGVIECBusiness.Save(TaskObj);
            PHIEUDANHGIACONGVIEC DanhGia = new PHIEUDANHGIACONGVIEC();
            DanhGia.CONGVIEC_ID = TaskObj.ID;
            DanhGia.NGAYDANHGIA = DateTime.Now;
            DanhGia.TDG_XEPLOAI_ID = TUDANHGIATHEOTHANGDIEM;
            DanhGia.NGUOITUDANHGIA = currentUser.ID;
            //DanhGia.TONGDIEM = DataObj.DATA;
            DanhGia.TDG_TONGDIEM = DataObj.DATA;
            DanhGia.TDG_XEPLOAI = DataObj.TEXT;
            PHIEUDANHGIACONGVIECBusiness.Save(DanhGia);

            #region Gửi thông báo
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            AssignUserInfo();
            string url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString();
            string sup_title = "Đánh giá công việc";
            string sup_msg = currentUser.HOTEN + " đã gửi yêu cầu đánh giá công việc #Công việc " + TaskObj.ID.ToString();
            List<long> ListUser = new List<long>();
            ListUser.Add(TaskObj.NGUOIGIAOVIEC_ID.HasValue ? TaskObj.NGUOIGIAOVIEC_ID.Value : 0);
            SYS_TINNHANBusiness.sendMessageMultipleUsers(ListUser, currentUser, sup_title, sup_msg, url, targetScreen, true, TaskObj.ID, TargetDocType.ASSIGNED);
            #endregion

            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString());
        }
        [ActionAudit]
        public ActionResult TuDanhGiaCongViec(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
            var currentUser = (UserInfoBO)SessionManager.GetUserInfo();
            var TaskID = form["TDG_TASKID"].ToLongOrZero();
            var TaskObj = HSCV_CONGVIECBusiness.Find(TaskID);
            TaskObj.DATUDANHGIA = true;
            HSCV_CONGVIECBusiness.Save(TaskObj);
            PHIEUDANHGIACONGVIEC DanhGia = new PHIEUDANHGIACONGVIEC();
            DanhGia.CONGVIEC_ID = TaskObj.ID;
            DanhGia.NGAYDANHGIA = DateTime.Now;
            DanhGia.TDG_TUCHUCAO = form["TDG_TUCHUCAO"].ToIntOrZero();
            DanhGia.TDG_TRACHNHIEMLON = form["TDG_TRACHNHIEMLON"].ToIntOrZero();
            DanhGia.TDG_TUONGTACTOT = form["TDG_TUONGTACTOT"].ToIntOrZero();
            DanhGia.TDG_TOCDONHANH = form["TDG_TOCDONHANH"].ToIntOrZero();
            DanhGia.TDG_TIENBONHIEU = form["TDG_TIENBONHIEU"].ToIntOrZero();
            DanhGia.TDG_THANHTICHVUOT = form["TDG_THANHTICHVUOT"].ToIntOrZero();
            DanhGia.NGUOITUDANHGIA = currentUser.ID;
            PHIEUDANHGIACONGVIECBusiness.Save(DanhGia);
            #region Gửi thông báo
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            AssignUserInfo();
            string url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString();
            string sup_title = "Đánh giá công việc";
            string sup_msg = currentUser.HOTEN + " đã gửi yêu cầu đánh giá công việc #Công việc " + TaskObj.ID.ToString();
            List<long> ListUser = new List<long>();
            ListUser.Add(TaskObj.NGUOIGIAOVIEC_ID.HasValue ? TaskObj.NGUOIGIAOVIEC_ID.Value : 0);
            SYS_TINNHANBusiness.sendMessageMultipleUsers(ListUser, currentUser, sup_title, sup_msg, url, targetScreen, true, TaskObj.ID, TargetDocType.ASSIGNED);
            #endregion
            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString());
        }
        [ActionAudit]
        public ActionResult DuyetDanhGiaCongViecTheoThangDiem(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();

            var currentUser = (UserInfoBO)SessionManager.GetUserInfo();
            var TaskID = form["DD_TASKID"].ToLongOrZero();
            var TaskObj = HSCV_CONGVIECBusiness.Find(TaskID);
            TaskObj.NGUOIGIAOVIECDANHGIA = true;
            HSCV_CONGVIECBusiness.Save(TaskObj);
            PHIEUDANHGIACONGVIEC DanhGia = PHIEUDANHGIACONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == TaskID).FirstOrDefault();
            if (DanhGia != null)
            {
                if (string.IsNullOrEmpty(form["DUYETDANHGIATHEOTHANGDIEM"]))
                {
                    return RedirectToAction("index");
                }
                DanhGia.DD_XEPLOAI_ID = form["DUYETDANHGIATHEOTHANGDIEM"].ToIntOrZero();
                var DanhMucBusiness = Get<DM_DANHMUC_DATABusiness>();
                var DataXepLoai = DanhMucBusiness.repository.Find(DanhGia.DD_XEPLOAI_ID);
                if (DataXepLoai == null)
                {
                    return RedirectToAction("index");
                }
                DanhGia.KETLUAN = form["KETLUAN"];
                DanhGia.XEPLOAI = DataXepLoai.TEXT;
                DanhGia.XEPLOAIDUYET = DataXepLoai.TEXT;
                DanhGia.TONGDIEM = DataXepLoai.DATA;
                PHIEUDANHGIACONGVIECBusiness.Save(DanhGia);
                HSCV_CONGVIECBusiness.Save(TaskObj);
                return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString());
            }
            return RedirectToAction("index");
        }
        [ActionAudit]
        public ActionResult DuyetDanhGiaCongViec(FormCollection form)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
            var currentUser = (UserInfoBO)SessionManager.GetUserInfo();
            var TaskID = form["DD_TASKID"].ToLongOrZero();
            var TaskObj = HSCV_CONGVIECBusiness.Find(TaskID);
            TaskObj.NGUOIGIAOVIECDANHGIA = true;
            HSCV_CONGVIECBusiness.Save(TaskObj);
            PHIEUDANHGIACONGVIEC DanhGia = PHIEUDANHGIACONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == TaskID).FirstOrDefault();
            if (DanhGia != null)
            {
                DanhGia.DD_THANHTICHVUOT = form["DD_THANHTICHVUOT"].ToIntOrZero();
                DanhGia.DD_TIENBONHIEU = form["DD_TIENBONHIEU"].ToIntOrZero();
                DanhGia.DD_TOCDONHANH = form["DD_TOCDONHANH"].ToIntOrZero();
                DanhGia.DD_TRACHNHIEMLON = form["DD_TRACHNHIEMLON"].ToIntOrZero();
                DanhGia.DD_TUCHUCAO = form["DD_TUCHUCAO"].ToIntOrZero();
                DanhGia.DD_TUONGTACTOT = form["DD_TUONGTACTOT"].ToIntOrZero();
                DanhGia.KETLUAN = form["DD_KETLUAN"].ToString();
                DanhGia.NGUOIDUYET = (long)currentUser.ID;
                DanhGia.NGAYDUYET = DateTime.Now;
                DanhGia.TONGDIEM = DanhGia.DD_TUCHUCAO * 2 + DanhGia.DD_TRACHNHIEMLON * 2
                    + DanhGia.DD_TUONGTACTOT + DanhGia.DD_TOCDONHANH + DanhGia.DD_TIENBONHIEU
                    + DanhGia.DD_THANHTICHVUOT * 3;
                if (DanhGia.TONGDIEM >= 41)
                {
                    DanhGia.XEPLOAI = "Tốt";
                    TaskObj.XEPLOAICONGVIEC = XepLoaiCongViecConstant.TOT;
                }
                else if (DanhGia.TONGDIEM > 35 && DanhGia.TONGDIEM <= 40)
                {
                    DanhGia.XEPLOAI = "Khá";
                    TaskObj.XEPLOAICONGVIEC = XepLoaiCongViecConstant.KHA;
                }
                else if (DanhGia.TONGDIEM >= 26 && DanhGia.TONGDIEM <= 35)
                {
                    DanhGia.XEPLOAI = "Đạt";
                    TaskObj.XEPLOAICONGVIEC = XepLoaiCongViecConstant.DAT;
                }
                else
                {
                    DanhGia.XEPLOAI = "Không đạt";
                    TaskObj.XEPLOAICONGVIEC = XepLoaiCongViecConstant.KHONGDAT;
                }
                DanhGia.XEPLOAIDUYET = DanhGia.XEPLOAI;
                PHIEUDANHGIACONGVIECBusiness.Save(DanhGia);
                HSCV_CONGVIECBusiness.Save(TaskObj);
                #region Gửi thông báo
                AssignUserInfo();
                SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                string url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString();
                string sup_title = "Đánh giá công việc";
                string sup_msg = currentUser.HOTEN + " đã phê duyệt đánh giá công việc #Công việc " + TaskObj.ID.ToString();
                List<long> ListUser = new List<long>();
                ListUser.Add(DanhGia.NGUOITUDANHGIA.HasValue ? DanhGia.NGUOITUDANHGIA.Value : 0);
                SYS_TINNHANBusiness.sendMessageMultipleUsers(ListUser, currentUser, sup_title, sup_msg, url, targetScreen, true, TaskObj.ID, TargetDocType.ASSIGNED);
                #endregion
            }
            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString());
        }
        #endregion 
        #endregion
        #region các hàm gọi trong ajax
        public PartialViewResult ShowMemberToJoinTask(long TASKID)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            DM_NGUOIDUNG_THAOTACBusiness = Get<DM_NGUOIDUNG_THAOTACBusiness>();
            NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var TaskObj = HSCV_CONGVIECBusiness.repository.Find(TASKID);
            if (TaskObj == null)
            {
                return PartialView("pvError");
            }
            AssignUserInfo();
            AssignTask modal = new AssignTask();
            modal.AllowAssignDiffDept = false;
            int DeptId = currentUser.DM_PHONGBAN_ID.Value;
            #region check role assign task
            var LstThaoTac = currentUser.ListThaoTac;
            bool HasRoleAssignTask = false;
            bool HasRoleAssignDepartment = false;
            bool HasRoleAssignChuyenVien = false;
            DM_THAOTAC ThaoTac = new DM_THAOTAC();
            if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_DONVI))
            {
                //Ban giám đốc
                HasRoleAssignDepartment = true;
                HasRoleAssignTask = true;

                ThaoTac = DM_THAOTACBusiness.repository.All().Where(x => x.MA_THAOTAC == Permission.GIAOVIEC_PHONGBAN).FirstOrDefault();
                if (ThaoTac != null)
                {
                    var LstVaiTro = VAITRO_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC_ID == ThaoTac.DM_THAOTAC_ID).Select(x => x.VAITRO_ID).ToList();
                    var LstUserIds = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => LstVaiTro.Contains(x.VAITRO_ID)).Select(x => x.NGUOIDUNG_ID).ToList();
                    var LstNguoiDungIds = DM_NGUOIDUNG_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC == ThaoTac.DM_THAOTAC_ID).Select(x => x.DM_NGUOIDUNG_ID).ToList();
                    if (LstNguoiDungIds.Count > 0)
                    {
                        LstUserIds.AddRange(LstNguoiDungIds);
                    }
                    modal.LstUser = DM_NGUOIDUNGBusiness.repository.All().Where(x => LstUserIds.Contains(x.ID)).ToList();
                    if (modal.LstUser.Count > 0)
                    {
                        var lstDeptId = modal.LstUser.Select(x => x.DM_PHONGBAN_ID).ToList();
                        modal.LstDept = CCTC_THANHPHANBusiness.repository.All().Where(x => lstDeptId.Contains(x.ID)).ToList();
                    }

                }
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_PHONGBAN))
            {
                //trưởng đơn vị + giám đốc đơn vị
                HasRoleAssignDepartment = true;
                HasRoleAssignTask = true;
                // Lấy ra các deptid có cùng parent id hoặc có id = id hiện tại
                var TmpLstDept = CCTC_THANHPHANBusiness.repository.All().Where(x => (x.PARENT_ID == currentUser.DeptParentID) || (x.ID == currentUser.DM_PHONGBAN_ID)).ToList();
                if (TmpLstDept.Count > 0)
                {
                    modal.LstDept = TmpLstDept;
                    ThaoTac = DM_THAOTACBusiness.repository.All().Where(x => x.MA_THAOTAC == Permission.GIAOVIEC_CANHAN).FirstOrDefault();
                    if (ThaoTac != null)
                    {
                        var LstVaiTro = VAITRO_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC_ID == ThaoTac.DM_THAOTAC_ID).Select(x => x.VAITRO_ID).ToList();
                        var LstUserIds = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => LstVaiTro.Contains(x.VAITRO_ID)).Select(x => x.NGUOIDUNG_ID).ToList();
                        var LstNguoiDungIds = DM_NGUOIDUNG_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC == ThaoTac.DM_THAOTAC_ID).Select(x => x.DM_NGUOIDUNG_ID).ToList();
                        if (LstNguoiDungIds.Count > 0)
                        {
                            LstUserIds.AddRange(LstNguoiDungIds);
                        }
                        // Danh sách người dùng có vai trò, quyền phù hợp
                        modal.LstUser = DM_NGUOIDUNGBusiness.repository.All().Where(x => LstUserIds.Contains(x.ID)).ToList();
                        if (modal.LstUser.Count > 0)
                        {
                            //Danh sách phòng ban khác ko thuộc cùng đơn vị
                            var lstDeptId = modal.LstUser.Where(x => x.DM_PHONGBAN_ID != currentUser.DM_PHONGBAN_ID).Select(x => x.DM_PHONGBAN_ID).ToList();
                            modal.LstAddDept = CCTC_THANHPHANBusiness.repository.All().Where(x => lstDeptId.Contains(x.ID)).ToList();
                        }
                        modal.AllowAssignDiffDept = true;
                        modal.IsCapPhongBan = false;
                    }
                }
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_CANHAN))
            {
                //Trưởng phòng - lấy ra toàn bộ nhân viên của phòng ban mình, có thể giao việc chéo sang phòng ban khác thuộc cùng đơnv vị
                HasRoleAssignChuyenVien = true;
                HasRoleAssignTask = true;

                var lsThaoTac = DM_THAOTACBusiness.repository.All().Where(x => x.MA_THAOTAC == Permission.GIAOVIEC_PHONGBAN || x.MA_THAOTAC == Permission.GIAOVIEC_CANHAN || x.MA_THAOTAC == Permission.GIAOVIEC_DONVI).Select(x => x.DM_THAOTAC_ID).ToList();
                var LstVaiTro = VAITRO_THAOTACBusiness.repository.All().Where(x => lsThaoTac.Contains(x.DM_THAOTAC_ID.Value)).Select(x => x.VAITRO_ID).ToList();
                var LstUserIds = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => LstVaiTro.Contains(x.VAITRO_ID)).Select(x => x.NGUOIDUNG_ID).ToList();
                var LstNguoiDungIds = DM_NGUOIDUNG_THAOTACBusiness.repository.All().Where(x => lsThaoTac.Contains(x.DM_THAOTAC.Value)).Select(x => x.DM_NGUOIDUNG_ID).ToList();
                if (LstNguoiDungIds.Count > 0)
                {
                    LstUserIds.AddRange(LstNguoiDungIds);
                }
                // Lấy danh sách phòng ban thuộc cùng đơn vị
                modal.LstAddDept = CCTC_THANHPHANBusiness.repository.All().Where(x => x.PARENT_ID == currentUser.DeptParentID && x.ID != DeptId).ToList();
                var lstAddDepId = modal.LstAddDept.Select(x => x.ID).ToList();
                var LstUser = DM_NGUOIDUNGBusiness.repository.All().Where(x => (x.DM_PHONGBAN_ID == DeptId || lstAddDepId.Contains(x.DM_PHONGBAN_ID.Value)) && !LstUserIds.Contains(x.ID)).ToList();
                modal.LstUser = LstUser;
                modal.LstDept = CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID == DeptId).ToList();
                // Lấy danh sách phục vụ cho việc cần add thêm người từ phòng ban khác.
                modal.AllowAssignDiffDept = true;
                modal.IsCapPhongBan = true;
            }


            modal.HasRoleAssignTask = HasRoleAssignTask;
            modal.HasRoleAssignChuyenVien = HasRoleAssignChuyenVien;
            modal.HasRoleAssignDepartment = HasRoleAssignDepartment;
            #endregion
            modal.TASKID = TASKID;
            modal.SUBTASKID = 0;
            modal.IsAddMoreMember = true;
            var LstUserDaThamGia = new List<long>();
            if (TaskObj.NGUOIXULYCHINH_ID != null)
            {
                LstUserDaThamGia.Add(TaskObj.NGUOIXULYCHINH_ID.Value);
            }

            HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            var lstNguoiThamGia = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.repository.All().Where(x => x.CONGVIEC_ID == TASKID).Select(x => x.USER_ID.Value).ToList();
            if (lstNguoiThamGia != null)
            {
                LstUserDaThamGia.AddRange(lstNguoiThamGia);
            }
            modal.LstUser = modal.LstUser.Where(x => !LstUserDaThamGia.Contains(x.ID)).ToList();
            return PartialView("_AssignTask", modal);
        }
        public PartialViewResult ShowMemberToAssignTask(long TASKID, long SUBTASKID)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            DM_NGUOIDUNG_THAOTACBusiness = Get<DM_NGUOIDUNG_THAOTACBusiness>();
            NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            AssignUserInfo();
            AssignTask modal = new AssignTask();
            modal.AllowAssignDiffDept = false;
            int DeptId = currentUser.DM_PHONGBAN_ID.Value;
            #region check role assign task
            var LstThaoTac = currentUser.ListThaoTac;
            bool HasRoleAssignTask = false;
            bool HasRoleAssignDepartment = false;
            bool HasRoleAssignChuyenVien = false;
            DM_THAOTAC ThaoTac = new DM_THAOTAC();
            if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_DONVI))
            {
                //Ban giám đốc
                HasRoleAssignDepartment = true;
                HasRoleAssignTask = true;

                ThaoTac = DM_THAOTACBusiness.repository.All().Where(x => x.MA_THAOTAC == Permission.GIAOVIEC_PHONGBAN).FirstOrDefault();
                if (ThaoTac != null)
                {
                    var LstVaiTro = VAITRO_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC_ID == ThaoTac.DM_THAOTAC_ID).Select(x => x.VAITRO_ID).ToList();
                    var LstUserIds = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => LstVaiTro.Contains(x.VAITRO_ID)).Select(x => x.NGUOIDUNG_ID).ToList();
                    var LstNguoiDungIds = DM_NGUOIDUNG_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC == ThaoTac.DM_THAOTAC_ID).Select(x => x.DM_NGUOIDUNG_ID).ToList();
                    if (LstNguoiDungIds.Count > 0)
                    {
                        LstUserIds.AddRange(LstNguoiDungIds);
                    }
                    modal.LstUser = DM_NGUOIDUNGBusiness.repository.All().Where(x => LstUserIds.Contains(x.ID)).ToList();
                    if (modal.LstUser.Count > 0)
                    {
                        var lstDeptId = modal.LstUser.Select(x => x.DM_PHONGBAN_ID).ToList();
                        modal.LstDept = CCTC_THANHPHANBusiness.repository.All().Where(x => lstDeptId.Contains(x.ID)).ToList();
                    }

                }
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_PHONGBAN))
            {
                //trưởng đơn vị + giám đốc đơn vị
                HasRoleAssignDepartment = true;
                HasRoleAssignTask = true;
                // Lấy ra các deptid có cùng parent id hoặc có id = id hiện tại
                var TmpLstDept = CCTC_THANHPHANBusiness.repository.All().Where(x => (x.PARENT_ID == currentUser.DeptParentID) || (x.ID == currentUser.DM_PHONGBAN_ID)).ToList();
                if (TmpLstDept.Count > 0)
                {
                    modal.LstDept = TmpLstDept;
                    ThaoTac = DM_THAOTACBusiness.repository.All().Where(x => x.MA_THAOTAC == Permission.GIAOVIEC_CANHAN).FirstOrDefault();
                    if (ThaoTac != null)
                    {
                        var LstVaiTro = VAITRO_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC_ID == ThaoTac.DM_THAOTAC_ID).Select(x => x.VAITRO_ID).ToList();
                        var LstUserIds = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => LstVaiTro.Contains(x.VAITRO_ID)).Select(x => x.NGUOIDUNG_ID).ToList();
                        var LstNguoiDungIds = DM_NGUOIDUNG_THAOTACBusiness.repository.All().Where(x => x.DM_THAOTAC == ThaoTac.DM_THAOTAC_ID).Select(x => x.DM_NGUOIDUNG_ID).ToList();
                        if (LstNguoiDungIds.Count > 0)
                        {
                            LstUserIds.AddRange(LstNguoiDungIds);
                        }
                        // Danh sách người dùng có vai trò, quyền phù hợp
                        modal.LstUser = DM_NGUOIDUNGBusiness.repository.All().Where(x => LstUserIds.Contains(x.ID)).ToList();
                        if (modal.LstUser.Count > 0)
                        {
                            //Danh sách phòng ban khác ko thuộc cùng đơn vị
                            var lstDeptId = modal.LstUser.Where(x => x.DM_PHONGBAN_ID != currentUser.DM_PHONGBAN_ID).Select(x => x.DM_PHONGBAN_ID).ToList();
                            modal.LstAddDept = CCTC_THANHPHANBusiness.repository.All().Where(x => lstDeptId.Contains(x.ID)).ToList();
                        }
                        modal.AllowAssignDiffDept = true;
                        modal.IsCapPhongBan = false;
                    }
                }
            }
            else if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_CANHAN))
            {
                //Trưởng phòng - lấy ra toàn bộ nhân viên của phòng ban mình, có thể giao việc chéo sang phòng ban khác thuộc cùng đơnv vị
                HasRoleAssignChuyenVien = true;
                HasRoleAssignTask = true;

                var lsThaoTac = DM_THAOTACBusiness.repository.All().Where(x => x.MA_THAOTAC == Permission.GIAOVIEC_PHONGBAN || x.MA_THAOTAC == Permission.GIAOVIEC_CANHAN || x.MA_THAOTAC == Permission.GIAOVIEC_DONVI).Select(x => x.DM_THAOTAC_ID).ToList();
                var LstVaiTro = VAITRO_THAOTACBusiness.repository.All().Where(x => lsThaoTac.Contains(x.DM_THAOTAC_ID.Value)).Select(x => x.VAITRO_ID).ToList();
                var LstUserIds = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => LstVaiTro.Contains(x.VAITRO_ID)).Select(x => x.NGUOIDUNG_ID).ToList();
                var LstNguoiDungIds = DM_NGUOIDUNG_THAOTACBusiness.repository.All().Where(x => lsThaoTac.Contains(x.DM_THAOTAC.Value)).Select(x => x.DM_NGUOIDUNG_ID).ToList();
                if (LstNguoiDungIds.Count > 0)
                {
                    LstUserIds.AddRange(LstNguoiDungIds);
                }
                // Lấy danh sách phòng ban thuộc cùng đơn vị
                modal.LstAddDept = CCTC_THANHPHANBusiness.repository.All().Where(x => x.PARENT_ID == currentUser.DeptParentID && x.ID != DeptId).ToList();
                var lstAddDepId = modal.LstAddDept.Select(x => x.ID).ToList();
                var LstUser = DM_NGUOIDUNGBusiness.repository.All().Where(x => (x.DM_PHONGBAN_ID == DeptId || lstAddDepId.Contains(x.DM_PHONGBAN_ID.Value)) && !LstUserIds.Contains(x.ID)).ToList();
                modal.LstUser = LstUser;
                modal.LstDept = CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID == DeptId).ToList();
                // Lấy danh sách phục vụ cho việc cần add thêm người từ phòng ban khác.
                modal.AllowAssignDiffDept = true;
                modal.IsCapPhongBan = true;
            }


            modal.HasRoleAssignTask = HasRoleAssignTask;
            modal.HasRoleAssignChuyenVien = HasRoleAssignChuyenVien;
            modal.HasRoleAssignDepartment = HasRoleAssignDepartment;
            #endregion
            modal.TASKID = TASKID;
            modal.SUBTASKID = SUBTASKID;
            return PartialView("_AssignTask", modal);
        }
        #endregion

        #region function liên quan tới notification
        public void sendNotification(long ID, string Mess, string Title, int Type)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            AssignUserInfo();
            #region lấy toàn bộ người dùng liên quan tới công việc này
            HSCV_CONGVIEC TaskObj = HSCV_CONGVIECBusiness.Find(ID);
            List<long> LstUserIds = new List<long>();
            long CurrentUserId = (long)currentUser.ID;
            if (CurrentUserId != TaskObj.NGUOIGIAOVIEC_ID)
            {
                LstUserIds.Add(TaskObj.NGUOIGIAOVIEC_ID.Value);
            }
            if (CurrentUserId != TaskObj.NGUOIXULYCHINH_ID && TaskObj.NGUOIXULYCHINH_ID != null)
            {
                LstUserIds.Add(TaskObj.NGUOIXULYCHINH_ID.Value);
            }

            var LstThamGia = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.repository.All().Where(x => x.CONGVIEC_ID == ID && x.USER_ID != CurrentUserId).Select(x => x.USER_ID.Value).ToList();
            if (LstThamGia.Count > 0)
            {
                LstUserIds.AddRange(LstThamGia);
            }

            #endregion
            #region send notification
            string url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + ID.ToString();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            SYS_TINNHANBusiness.sendMessageMultipleUsers(LstUserIds, currentUser, Title, Mess, url, targetScreen, true, ID, Type);
            #endregion
        }
        #endregion
        #region công việc con
        [ActionAudit]
        public ActionResult CreateSubTaskNew(FormCollection col)
        {
            AssignUserInfo();
            HSCV_SUBTASKBusiness = Get<HSCV_SUBTASKBusiness>();

            if (!string.IsNullOrEmpty(col["BEGINTASKID"]) && !string.IsNullOrEmpty(col["TOTAL_CONGVIEC"]))
            {
                int TOTAL_CONGVIEC = col["TOTAL_CONGVIEC"].ToIntOrZero();
                long TaskId = col["BEGINTASKID"].ToLongOrZero();
                for (var idx = 1; idx <= TOTAL_CONGVIEC; idx++)
                {
                    if (!string.IsNullOrEmpty(col["TASKCONTENT-" + idx.ToString()]) && !string.IsNullOrEmpty(col["MUCDOUUTIEN-" + idx.ToString()]) && !string.IsNullOrEmpty(col["HANHOANTHANH-" + idx.ToString()]))
                    {

                        string TASKCONTENT = col["TASKCONTENT-" + idx.ToString()];
                        int MUCDOUUTIEN = col["MUCDOUUTIEN-" + idx.ToString()].ToIntOrZero();
                        int DOKHAN = col["DOKHAN-" + idx.ToString()].ToIntOrZero();
                        HSCV_SUBTASK subTask = new HSCV_SUBTASK();
                        subTask.CONGVIEC_ID = TaskId;
                        subTask.NOIDUNG = TASKCONTENT;
                        subTask.NGUOITAO = currentUser.ID;
                        subTask.TRANGTHAI_ID = 0;
                        subTask.MUCDOUUTIEN = MUCDOUUTIEN;
                        subTask.DOKHAN = DOKHAN;
                        subTask.NGAYTAO = DateTime.Now;
                        subTask.HANHOANTHANH = col["HANHOANTHANH-" + idx.ToString()].ToDateTime();
                        if (string.IsNullOrEmpty(col["IS_HASPLAN-" + idx.ToString()]))
                        {
                            subTask.IS_HASPLAN = false;
                        }
                        else
                        {
                            subTask.IS_HASPLAN = col["IS_HASPLAN-" + idx.ToString()].ToIntOrZero() == 1;
                        }

                        HSCV_SUBTASKBusiness.Save(subTask);

                    }
                }
                return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskId.ToString());
            }

            return Redirect("/#QuanLyCongViec/QuanLyCongViec");
        }
        [ActionAudit]
        public ActionResult CreateSubTask(FormCollection col)
        {
            AssignUserInfo();
            HSCV_SUBTASKBusiness = Get<HSCV_SUBTASKBusiness>();
            if (!string.IsNullOrEmpty(col["BEGINTASKID"]) && !string.IsNullOrEmpty(col["TASKCONTENT"]) && !string.IsNullOrEmpty(col["MUCDOUUTIEN"]) && !string.IsNullOrEmpty(col["HANHOANTHANH"]))
            {
                long TaskId = col["BEGINTASKID"].ToLongOrZero();
                string TASKCONTENT = col["TASKCONTENT"];
                int MUCDOUUTIEN = col["MUCDOUUTIEN"].ToIntOrZero();
                int DOKHAN = col["DOKHAN"].ToIntOrZero();
                HSCV_SUBTASK subTask = new HSCV_SUBTASK();
                subTask.CONGVIEC_ID = TaskId;
                subTask.NOIDUNG = TASKCONTENT;
                subTask.TRANGTHAI_ID = 0;
                subTask.MUCDOUUTIEN = MUCDOUUTIEN;
                subTask.NGUOITAO = currentUser.ID;
                subTask.DOKHAN = DOKHAN;
                subTask.NGAYTAO = DateTime.Now;
                subTask.HANHOANTHANH = col["HANHOANTHANH"].ToDateTime();
                if (string.IsNullOrEmpty(col["IS_HASPLAN"]))
                {
                    subTask.IS_HASPLAN = false;
                }
                else
                {
                    subTask.IS_HASPLAN = col["IS_HASPLAN"].ToIntOrZero() == 1;
                }

                HSCV_SUBTASKBusiness.Save(subTask);

                return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskId.ToString());
            }
            return Redirect("/#QuanLyCongViec/QuanLyCongViec");
        }
        [ActionAudit]
        public JsonResult FinishSubTask(long ID)
        {
            HSCV_SUBTASKBusiness = Get<HSCV_SUBTASKBusiness>();
            HSCV_SUBTASK subTask = HSCV_SUBTASKBusiness.Find(ID);
            if (subTask != null)
            {
                subTask.NGAYHOANTHANH = DateTime.Now;
                subTask.TRANGTHAI_ID = 1;
                HSCV_SUBTASKBusiness.Save(subTask);
            }
            return Json(true);
        }
        [ActionAudit]
        public ActionResult AssignTask(FormCollection col)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_SUBTASKBusiness = Get<HSCV_SUBTASKBusiness>();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            AssignUserInfo();
            HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();

            if (string.IsNullOrEmpty(col["ASSIGNTASK_TASKID"]) || string.IsNullOrEmpty(col["ASSIGNTASK_SUBTASKID"]))
            {
                return RedirectToAction("Index");
            }
            long TASKID = col["ASSIGNTASK_TASKID"].ToLongOrZero();
            long SUBTASKID = col["ASSIGNTASK_SUBTASKID"].ToLongOrZero();
            List<long> LstNguoiThamGia = col["NGUOITHAMGIAXULY"].ToListLong(',');
            #region get thông tin công việc chính
            HSCV_CONGVIEC jobObj = HSCV_CONGVIECBusiness.Find(TASKID);
            if (jobObj == null)
            {
                return RedirectToAction("Index");
            }
            #region
            List<long> ListUser = new List<long>();
            ListUser.AddRange(LstNguoiThamGia);
            ListUser.Add(SUBTASKID);
            ListUser.Add(currentUser.ID);
            ElasticModel model = ElasticModel.ConvertJob(jobObj, ListUser, "");
            ElasticSearch.updateDocument(model, model.Id.ToString(), ElasticType.CongViec);
            #endregion
            #endregion
            if (SUBTASKID == 0)
            {
                #region assign việc chính

                string url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + TASKID.ToString();
                if (col["NGUOIXULYCHINH"].ToLongOrZero() > 0)
                {
                    jobObj.NGUOIXULYCHINH_ID = col["NGUOIXULYCHINH"].ToLongOrZero();
                    jobObj.DAGIAOVIEC = true;
                    jobObj.IS_ASSIGNED = true;
                    jobObj.NGAY_NHANVIEC = DateTime.Now;
                    HSCV_CONGVIECBusiness.Save(jobObj);
                    List<long> listUserMainProcessIds = new List<long>();
                    listUserMainProcessIds.Add(jobObj.NGUOIXULYCHINH_ID.Value);
                    string title = "Xử lý chính #Công việc " + jobObj.ID.ToString();

                    string msg = currentUser.HOTEN + " đã giao cho bạn xử lý chính #Công việc " + TASKID.ToString();
                    SYS_TINNHANBusiness.sendMessageMultipleUsers(listUserMainProcessIds, currentUser, title, msg, url, targetScreen, true, TASKID, TargetDocType.ASSIGNED);

                    #region gửi email cho người xử lý chính
                    List<string> lstEmail = new List<string>();

                    var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                    var tmpObjNguoiXuLy = DM_NGUOIDUNGBusiness.repository.Find(jobObj.NGUOIXULYCHINH_ID);
                    if (tmpObjNguoiXuLy != null)
                    {
                        lstEmail.Add("duynt61089@gmail.com");
                        //lstEmail.Add(tmpObjNguoiXuLy.TENDANGNHAP);
                    }
                    if (lstEmail != null)
                    {
                        var ContentEmail = currentUser.HOTEN + " đã giao cho bạn xử lý chính <a href='http://d-office.doji.vn" + url + "'>#Công việc '" + jobObj.TENCONGVIEC.ToString() + "'</a>";
                        EmailProvider.SendMailTemplate(currentUser, ContentEmail, msg, lstEmail);
                    }
                    #endregion
                }
                #endregion
                #region nguoithamgia
                foreach (var supporter in LstNguoiThamGia)
                {
                    HSCV_CONGVIEC_NGUOITHAMGIAXULY supportObj = new HSCV_CONGVIEC_NGUOITHAMGIAXULY();
                    supportObj.CONGVIEC_ID = jobObj.ID;
                    supportObj.USER_ID = supporter;
                    HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.Save(supportObj);
                }
                string sup_title = "Tham gia xử lý #Công việc " + jobObj.ID.ToString();
                string sup_msg = currentUser.HOTEN + " đã giao cho bạn tham gia phối hợp xử lý chính #Công việc " + TASKID.ToString();
                SYS_TINNHANBusiness.sendMessageMultipleUsers(LstNguoiThamGia, currentUser, sup_title, sup_msg, url, targetScreen, true, TASKID, TargetDocType.ASSIGNED);
                #endregion
                return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TASKID.ToString());
            }
            else
            {
                #region assign việc nhỏ trong việc chính
                HSCV_SUBTASK subTask = HSCV_SUBTASKBusiness.Find(SUBTASKID);
                if (subTask == null)
                {
                    return RedirectToAction("Index");
                }
                HSCV_CONGVIEC taskObj = HSCV_CONGVIECBusiness.Find(TASKID);
                HSCV_CONGVIEC congViecObj = new HSCV_CONGVIEC();
                congViecObj.NGUOIGIAOVIEC_ID = currentUser.ID;
                congViecObj.TENCONGVIEC = subTask.NOIDUNG;
                congViecObj.NGAY_NHANVIEC = DateTime.Now;
                if (subTask.HANHOANTHANH != null)
                {
                    congViecObj.NGAYHOANTHANH_THEOMONGMUON = subTask.HANHOANTHANH.Value;
                }
                //congViecObj.TRANGTHAI_ID = 1;
                congViecObj.IS_POPUP = taskObj.IS_POPUP;
                congViecObj.IS_MESG = taskObj.IS_MESG;
                congViecObj.IS_EMAIL = taskObj.IS_EMAIL;
                congViecObj.IS_SMS = taskObj.IS_SMS;
                congViecObj.SONGAYNHACTRUOCHAN = taskObj.SONGAYNHACTRUOCHAN;
                congViecObj.HAS_NHACVIECDENHAN = taskObj.HAS_NHACVIECDENHAN;
                congViecObj.NOIDUNGCONGVIEC = subTask.NOIDUNG;
                congViecObj.CONGVIECGOC_ID = taskObj.ID;
                congViecObj.NGUOIXULYCHINH_ID = col["NGUOIXULYCHINH"].ToLongOrZero();
                congViecObj.NGUOIGIAOVIEC_ID = currentUser.ID;
                congViecObj.IS_SUBTASK = true;
                congViecObj.SUBTASK_ID = subTask.ID;
                congViecObj.DOKHAN = subTask.DOKHAN;
                congViecObj.DOUU_TIEN = subTask.MUCDOUUTIEN;
                congViecObj.DAGIAOVIEC = true;
                congViecObj.IS_ASSIGNED = true;
                congViecObj.IS_HASPLAN = subTask.IS_HASPLAN;
                HSCV_CONGVIECBusiness.Save(congViecObj);
                List<long> MainUserIds = new List<long>();
                MainUserIds.Add(congViecObj.NGUOIXULYCHINH_ID.Value);
                string title = "Xử lý chính #Công việc " + congViecObj.ID.ToString();
                string url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + congViecObj.ID.ToString();
                string msg = currentUser.HOTEN + " đã giao cho bạn xử lý chính #Công việc " + congViecObj.ID.ToString();
                SYS_TINNHANBusiness.sendMessageMultipleUsers(MainUserIds, currentUser, title, msg, url, targetScreen, true, TASKID, TargetDocType.ASSIGNED);
                subTask.DAGIAOVIEC = true;
                HSCV_SUBTASKBusiness.Save(subTask);
                #region gửi email cho người xử lý chính
                List<string> lstEmail = new List<string>();

                var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                var tmpObjNguoiXuLy = DM_NGUOIDUNGBusiness.repository.Find(congViecObj.NGUOIXULYCHINH_ID);
                if (tmpObjNguoiXuLy != null)
                {
                    lstEmail.Add("duynt61089@gmail.com");
                    //lstEmail.Add(tmpObjNguoiXuLy.TENDANGNHAP);
                }
                if (lstEmail != null)
                {
                    var ContentEmail = currentUser.HOTEN + " đã giao cho bạn xử lý chính <a href='http://d-office.doji.vn" + url + "'>#Công việc '" + congViecObj.NOIDUNGCONGVIEC + "'</a>";
                    EmailProvider.SendMailTemplate(currentUser, ContentEmail, msg, lstEmail);
                }
                #endregion
                #endregion
                #region nguoithamgia
                foreach (var supporter in LstNguoiThamGia)
                {
                    HSCV_CONGVIEC_NGUOITHAMGIAXULY supportObj = new HSCV_CONGVIEC_NGUOITHAMGIAXULY();
                    supportObj.CONGVIEC_ID = congViecObj.ID;
                    supportObj.USER_ID = supporter;
                    HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.Save(supportObj);
                }

                string sup_title = "Tham gia xử lý #Công việc " + congViecObj.ID.ToString();
                string sup_msg = currentUser.HOTEN + " đã giao cho bạn tham gia phối hợp xử lý chính #Công việc " + congViecObj.ID.ToString();
                SYS_TINNHANBusiness.sendMessageMultipleUsers(LstNguoiThamGia, currentUser, sup_title, sup_msg, url, targetScreen, true, TASKID, TargetDocType.COORDINATED);
                #endregion
                return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TASKID.ToString());
            }
        }
        public ActionResult GoToDetailSubTask(long ID)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();

            HSCV_CONGVIEC TaskObj = HSCV_CONGVIECBusiness.repository.All().Where(x => x.SUBTASK_ID == ID).FirstOrDefault();
            if (TaskObj == null)
            {
                return RedirectToAction("Index");
            }
            return Redirect("/QuanLyCongViec/QuanLyCongViec/Detail/" + TaskObj.ID.ToString());

        }
        #endregion

        #region Màn hình báo cáo
        public ActionResult ExportCongViec()
        {
            return View();
        }
        #endregion

        #region Theo dõi tiến độ công việc
        public PartialViewResult ShowAllTask(long id)
        {
            ReportModel model = new ReportModel();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            model.LstCongViec = HSCV_CONGVIECBusiness.GetDSChild(id);
            model.CongViecId = id;
            string html = "";
            string newhtml = "";
            var LstMaxSubTask = model.LstCongViec.Where(x => x.ParentId == model.CongViecId).ToList();
            int idx = 1;
            foreach (var item in LstMaxSubTask)
            {
                //html += "<div class='item' style='border-bottom: 1px solid #ddd; padding: 5px;'>";
                //html += "<p>";
                //html += "<span><strong>" + idx.ToString() + ".</strong></span>";
                //html += "<a target='_blank' href='/#QuanLyCongViec/QuanLyCongViec/Detail/" + item.ID + "' data-toggle='tooltip' title='' data-original-title='" + item.TENCONGVIEC + "'>";
                //html += item.TENCONGVIEC + "</a>";
                //html += "<i class='fa fa-bolt' style='margin-left:3px'></i> Tiến độ: <b>" + item.PHANTRAMHOANTHANH + "<span>%</span></b>";
                //html += "<i class='fa  fa-clock-o' style='margin-left:3px'></i> Hạn hoàn thành: <b>" + string.Format("{0:dd/MM/yyyy}", item.NGAYHOANTHANH_THEOMONGMUON) + "</b>";
                //html += "<i class='glyphicon glyphicon-user' style='margin-left:3px'></i>Người xử lý: <b>" + item.TEN_NGUOIXULYCHINH + "</b>";
                //html += "</p>";
                //html += "</div>";
                //html += RecursivePrinter(item, model, idx.ToString());
                //idx = idx + 1;

                newhtml += "<tr style='font-weight:bold'>";
                newhtml += "<td>";
                newhtml += "<i class='fa fa-folder-open'></i>";
                newhtml += "</td>";
                newhtml += "<td>";
                newhtml += "<a  target='_blank' href='/#QuanLyCongViec/QuanLyCongViec/Detail/" + item.ID + "' data-toggle='tooltip' title='' data-original-title='" + item.TENCONGVIEC + "'>";
                newhtml += item.TENCONGVIEC + "</a>";
                newhtml += "</td>";
                newhtml += "<td>";
                newhtml += item.TEN_NGUOIXULYCHINH;
                newhtml += "</td>";
                newhtml += "<td>";
                newhtml += string.Format("{0:dd/MM/yyyy}", item.NGAYHOANTHANH_THEOMONGMUON);
                newhtml += "</td>";
                newhtml += "<td>";
                newhtml += item.PHANTRAMHOANTHANH + "%";
                newhtml += "</td>";
                newhtml += "</tr>";
                newhtml += RecursivePrinter(item, model, idx.ToString());
                idx = idx + 1;

            }
            return PartialView("ShowAllTask", newhtml);
        }
        public string RecursivePrinter(CongViecBO qitem, ReportModel model, string idx)
        {
            var html = "";
            int stt = 1;
            var LstCongViec = model.LstCongViec.Where(x => x.ParentId == qitem.ID).ToList();
            if (LstCongViec.Count > 0)
            {
                foreach (var item in LstCongViec)
                {
                    //html += "<div class='item' style='border-bottom: 1px solid #ddd; padding: 5px;padding-left:20px'>";
                    //html += "<p>";
                    //html += "<span><strong> " + idx + "." + stt.ToString() + ". </strong></span>";
                    //html += "<a target='_blank' href='/#QuanLyCongViec/QuanLyCongViec/Detail/" + item.ID + "' data-toggle='tooltip' title='' data-original-title='" + item.TENCONGVIEC + "'>";
                    //html += item.TENCONGVIEC + "</a>";
                    //html += "<i class='fa fa-bolt' style='margin-left:3px'></i> Tiến độ: <b>" + item.PHANTRAMHOANTHANH + "<span>%</span></b>";
                    //html += "<i class='fa  fa-clock-o' style='margin-left:3px'></i> Hạn hoàn thành: <b>" + string.Format("{0:dd/MM/yyyy}", item.NGAYHOANTHANH_THEOMONGMUON) + "</b>";
                    //html += "<i class='glyphicon glyphicon-user' style='margin-left:3px'></i> Người xử lý: <b>" + item.TEN_NGUOIXULYCHINH + "</b>";
                    //html += "</p>";
                    //html += "</div>";
                    html += "<tr>";
                    html += "<td>";
                    html += "";
                    html += "</td>";
                    html += "<td>";
                    html += "<a  target='_blank' href='/#QuanLyCongViec/QuanLyCongViec/Detail/" + item.ID + "' data-toggle='tooltip' title='' data-original-title='" + item.TENCONGVIEC + "'>";
                    html += item.TENCONGVIEC + "</a>";
                    html += "</td>";
                    html += "<td>";
                    html += item.TEN_NGUOIXULYCHINH;
                    html += "</td>";
                    html += "<td>";
                    html += string.Format("{0:dd/MM/yyyy}", item.NGAYHOANTHANH_THEOMONGMUON);
                    html += "</td>";
                    html += "<td>";
                    html += item.PHANTRAMHOANTHANH + "%";
                    html += "</td>";
                    html += "</tr>";
                    html += RecursivePrinter(item, model, idx + "." + stt.ToString());
                    stt = stt + 1;
                }
            }
            return html;
        }
        #endregion


        public PartialViewResult LoadCongViecCon(long id, string RowNo, long RootId, string rowIndex)
        {
            CongViecIndexViewModel model = new CongViecIndexViewModel();
            AssignUserInfo();
            model.UserInfo = currentUser;
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            model.ListCongViec = HSCV_CONGVIECBusiness.GetDsCongViecCon(id, LOAI_CONGVIEC.CANHAN, currentUser.ID);
            model.ParentId = id;
            //model.BackgroundColor = BackgroundColor;
            //model.Color = Color;
            model.RowNo = RowNo;
            model.RootId = RootId;
            model.Level = rowIndex.ToListInt('.').Count;
            return PartialView("_CongViecCon", model);
        }

        public ActionResult convertCongViec()
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var lstJob = HSCV_CONGVIECBusiness.repository.All().Where(x => x.CONGVIECGOC_ID != null).ToList();
            foreach (var item in lstJob)
            {
                string tmpIds = "";
                var tmpJob = HSCV_CONGVIECBusiness.repository.All().Where(x => x.ID == item.CONGVIECGOC_ID).FirstOrDefault();
                if (tmpJob != null)
                {
                    if (tmpJob.CONGVIECGOC_ID != null)
                    {
                        tmpIds = tmpJob.ID.ToString() + "," + tmpJob.CONGVIECGOC_ID;
                    }
                    else
                    {
                        tmpIds = tmpJob.ID.ToString();
                    }

                }
                item.CONGVIEC_LIENQUAN_ID = tmpIds;
                HSCV_CONGVIECBusiness.Save(item);
            }
            return RedirectToAction("index");
        }
        public PartialViewResult generateFormSubTask(string idx)
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CongViecViewModel model = new CongViecViewModel();
            model.ListDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, currentUser.ID);
            model.ListDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID);
            model.IdxTr = idx;
            return PartialView("_generateFormSubTask", model);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 09/08/2018
        /// @description: màn hình import công việc
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportCongViec(string controllerName = "QuanLyCongViec")
        {
            ImportCongViecViewModel model = new ImportCongViecViewModel();
            model.controllerName = controllerName;
            model.importTemplatePath = Path.Combine(WEB_ADDRESS, WebConfigurationManager.AppSettings["ImportCongViecTemplate"]);
            return View(model);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 09/08/2018
        /// @description: màn hình import công việc con
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public ActionResult ImportCongViecCon(int taskId)
        {
            AssignUserInfo();
            var hscvCongViecBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC task = hscvCongViecBusiness.Find(taskId);
            if (task != null && currentUser.ID == task.NGUOIXULYCHINH_ID)
            {
                ImportCongViecViewModel model = new ImportCongViecViewModel();
                model.taskId = task.ID;
                model.importTemplatePath = Path.Combine(WEB_ADDRESS, WebConfigurationManager.AppSettings["ImportCongViecConTemplate"]);
                return View(model);
            }
            return Redirect("/Home/UnAuthor");
        }

        /// <summary>
        /// @author: duynn
        /// @since: 09/08/2018
        /// @description: import danh sách công việc con
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadImportSubTasks(HttpPostedFileBase fileImport, FormCollection fc)
        {
            AssignUserInfo();
            var hscvCongViecBusiness = Get<HSCV_CONGVIECBusiness>();
            var hscvSubTaskBusiness = Get<HSCV_SUBTASKBusiness>();
            var dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            JsonResultImportBO<HSCV_SUBTASK_BO> result = new JsonResultImportBO<HSCV_SUBTASK_BO>(true);
            int taskId = fc["CONGVIEC_ID"].ToIntOrZero();
            HSCV_CONGVIEC task = hscvCongViecBusiness.Find(taskId);

            if (task == null || fileImport == null)
            {
                result.Status = false;
                result.Message = (task == null) ? "Công việc cha không tồn tại" : "File tài liệu không tồn tại";
                return Json(result);
            }

            UploadResult uploadTempImportFileResult = UploadProvider.SaveFile(fileImport, fileImport.FileName, ".xls,.xlsx", null, "TempImportSubTasks", UPLOADFOLDER);
            if (!uploadTempImportFileResult.status)
            {
                result.Status = false;
                result.Message = uploadTempImportFileResult.message;
                return Json(result);
            }

            var helper = new ImportExcelHelper<HSCV_SUBTASK_BO>();
            helper.StartRow = 2;
            helper.StartCol = 1; //không có ý nghĩa vì trong hàm import luôn bắt đầu = 2
            helper.PathTemplate = uploadTempImportFileResult.fullPath;
            helper.ConfigColumn = new List<ConfigModule>()
            {
                new ConfigModule()
                {
                    columnName = "NOIDUNG",
                    require = true,
                    TypeValue = typeof(string).FullName
                },
                new ConfigModule()
                {
                    columnName = "TEN_MUCDOUUTIEN",
                    require = true,
                    TypeValue = typeof(string).FullName
                }, new ConfigModule()
                {
                    columnName = "TEN_DOKHAN",
                    require= true,
                    TypeValue = typeof(string).FullName
                }, new ConfigModule()
                {
                    columnName = "HANHOANTHANH",
                    require = true,
                    TypeValue = typeof(DateTime).FullName
                }, new ConfigModule()
                {
                    columnName = "TEXT_IS_HASPLAN",
                    require = false,
                    TypeValue = typeof(string).FullName
                }
            };

            var checkResult = helper.Import();

            if (checkResult.Status)
            {
                List<HSCV_SUBTASK_BO> validItems = checkResult.ListTrue;
                result.ListData = new List<HSCV_SUBTASK_BO>();
                List<SelectListItem> groupOfDoUuTien = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID);
                List<SelectListItem> groupOfDoKhan = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, currentUser.ID);

                foreach (var item in validItems)
                {
                    item.TEN_MUCDOUUTIEN = !string.IsNullOrEmpty(item.TEN_MUCDOUUTIEN) ? item.TEN_MUCDOUUTIEN.Trim().ToLower() : string.Empty;
                    item.TEN_DOKHAN = !string.IsNullOrEmpty(item.TEN_DOKHAN) ? item.TEN_DOKHAN.Trim().ToLower() : string.Empty;

                    SelectListItem itemMucDoUuTien = groupOfDoUuTien.FirstOrDefault(x => x.Text.Trim().ToLower().Equals(item.TEN_MUCDOUUTIEN));
                    SelectListItem itemDoKhan = groupOfDoKhan.FirstOrDefault(x => x.Text.ToLower().Equals(item.TEN_DOKHAN));

                    if (itemMucDoUuTien == null || itemDoKhan == null)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("0");
                        errors.Add(item.NOIDUNG);
                        errors.Add(item.TEN_MUCDOUUTIEN);
                        errors.Add(item.TEN_DOKHAN);
                        errors.Add(item.HANHOANTHANH != null ? string.Format("{0:dd/MM/yyyy}", item.HANHOANTHANH.Value) : string.Empty);
                        errors.Add(!string.IsNullOrEmpty(item.TEXT_IS_HASPLAN) ? "Có" : "Không");
                        checkResult.lstFalse.Add(errors);
                    }
                    else
                    {
                        item.HANHOANTHANH_TEXT = item.HANHOANTHANH != null ? string.Format("{0:dd/MM/yyyy}", item.HANHOANTHANH.Value) : string.Empty;
                        item.IS_HASPLAN = !string.IsNullOrEmpty(item.TEXT_IS_HASPLAN);
                        item.TEXT_IS_HASPLAN = !string.IsNullOrEmpty(item.TEXT_IS_HASPLAN) ? "Có" : "Không";
                        item.DOKHAN = itemDoKhan.Value.ToIntOrZero();
                        item.MUCDOUUTIEN = itemMucDoUuTien.Value.ToIntOrZero();
                        item.TRANGTHAI_ID = 0;
                        item.CONGVIEC_ID = taskId;
                        result.ListData.Add(item);
                    }
                    result.ListFalse = checkResult.lstFalse;
                }
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 09/08/2018
        /// @description: import danh sách công việc
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadImportTasks(HttpPostedFileBase fileImport, FormCollection fc)
        {
            AssignUserInfo();
            var hscvCongViecBusiness = Get<HSCV_CONGVIECBusiness>();
            var dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            var dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            var hscvCongViecNguoiThamGiaXuLyBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            JsonResultImportBO<CongViecBO> result = new JsonResultImportBO<CongViecBO>(true);
            result.ListData = new List<CongViecBO>();

            if (fileImport == null)
            {
                result.Status = false;
                result.Message = "File tài liệu không tồn tại";
                return Json(result);
            }

            UploadResult uploadTempImportFileResult = UploadProvider.SaveFile(fileImport, fileImport.FileName, ".xls,.xlsx", null, "TempImportTasks", UPLOADFOLDER);
            if (!uploadTempImportFileResult.status)
            {
                result.Status = false;
                result.Message = uploadTempImportFileResult.message;
                return Json(result);
            }

            var helper = new ImportExcelHelper<CongViecBO>();
            helper.StartRow = 2;
            helper.StartCol = 1;
            helper.PathTemplate = uploadTempImportFileResult.fullPath;
            helper.ConfigColumn = new List<ConfigModule>()
            {
                new ConfigModule()
                {
                    columnName = "TENCONGVIEC",
                    require = true,
                    TypeValue = typeof(string).FullName
                },
                new ConfigModule()
                {
                    columnName = "TEN_DOUUTIEN",
                    require = true,
                    TypeValue = typeof(string).FullName,
                }, new ConfigModule()
                {
                    columnName = "TEN_DOKHAN",
                    require = true,
                    TypeValue = typeof(string).FullName,
                }, new ConfigModule()
                {
                    columnName = "TEN_NGUOIGIAOVIEC",
                    require = false,
                    TypeValue = typeof(string).FullName
                }, new ConfigModule()
                {
                    columnName = "NGAY_NHANVIEC",
                    TypeValue = typeof(DateTime).FullName
                }, new ConfigModule()
                {
                    columnName = "NGAYHOANTHANH_THEOMONGMUON",
                    TypeValue = typeof(DateTime).FullName
                },  new ConfigModule()
                {
                    columnName = "TEN_NGUOIXULYCHINH",
                    TypeValue = typeof(string).FullName,
                },
                new ConfigModule()
                {
                    columnName = "NGUOI_THAMGIA_XULY",
                    TypeValue = typeof(string).FullName,
                },new ConfigModule()
                {
                    columnName = "SONGAYNHACTRUOCHAN",
                    TypeValue = typeof(int).FullName,
                }, new ConfigModule(){
                    columnName = "NOIDUNGCONGVIEC",
                    TypeValue = typeof(string).FullName
                }, new ConfigModule()
                {
                    columnName = "MUCTIEU_CONGVIEC",
                    TypeValue = typeof(string).FullName
                }
            };

            var checkResult = helper.Import();
            if (checkResult.Status)
            {
                List<DM_NGUOIDUNG_BO> users = dmNguoiDungBusiness.context.DM_NGUOIDUNG.Where(x => x.EMAIL != null)
                .Select(x => new DM_NGUOIDUNG_BO() { ID = x.ID, EMAIL = x.EMAIL }).ToList();

                List<SelectListItem> groupOfDoUuTien = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID);
                List<SelectListItem> groupOfDoKhan = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, currentUser.ID);

                List<CongViecBO> validItems = checkResult.ListTrue.ToList();

                foreach (var item in validItems)
                {
                    SelectListItem itemMucDoUuTien = groupOfDoUuTien.FirstOrDefault(x => x.Text.Trim().ToLower().Equals(item.TEN_DOUUTIEN.Trim().ToLower()));
                    SelectListItem itemDoKhan = groupOfDoKhan.FirstOrDefault(x => x.Text.Trim().ToLower().Equals(item.TEN_DOKHAN.Trim().ToLower()));

                    if (itemMucDoUuTien == null || itemDoKhan == null)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("0");
                        errors.Add(item.TENCONGVIEC);
                        errors.Add(item.TEN_DOUUTIEN);
                        errors.Add(item.TEN_DOKHAN);
                        errors.Add(item.TEN_NGUOIGIAOVIEC);
                        errors.Add(item.NGAY_NHANVIEC != null ? string.Format("{0:dd/MM/yyyy}", item.NGAY_NHANVIEC.Value) : string.Empty);
                        errors.Add(item.NGAYHOANTHANH_THEOMONGMUON != null ? string.Format("{0:dd/MM/yyyy}", item.NGAYHOANTHANH_THEOMONGMUON.Value) : string.Empty);
                        errors.Add(item.TEN_NGUOIXULYCHINH);
                        errors.Add(item.NGUOI_THAMGIA_XULY);
                        errors.Add(item.SONGAYNHACTRUOCHAN.GetValueOrDefault().ToString());
                        errors.Add(item.NOIDUNGCONGVIEC);
                        errors.Add(item.MUCTIEU_CONGVIEC);
                        checkResult.lstFalse.Add(errors);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.TEN_NGUOIGIAOVIEC))
                        {
                            DM_NGUOIDUNG entityNguoiGiaoViec = users.Where(x => x.EMAIL.Equals(item.TEN_NGUOIGIAOVIEC.Trim())).FirstOrDefault();
                            if (entityNguoiGiaoViec != null)
                            {
                                item.NGUOIGIAOVIEC_ID = entityNguoiGiaoViec.ID;
                            }
                        }
                        //else
                        //{
                        //    item.TEN_NGUOIGIAOVIEC = currentUser.EMAIL;
                        //    item.NGUOIGIAOVIEC_ID = currentUser.ID;
                        //}

                        if (!string.IsNullOrEmpty(item.TEN_NGUOIXULYCHINH))
                        {
                            DM_NGUOIDUNG entityNguoiXuLy = users.FirstOrDefault(x => x.EMAIL.Equals(item.TEN_NGUOIXULYCHINH.Trim()));
                            if (entityNguoiXuLy != null)
                            {
                                item.NGUOIXULYCHINH_ID = entityNguoiXuLy.ID;
                            }
                        }

                        if (!string.IsNullOrEmpty(item.NGUOI_THAMGIA_XULY))
                        {
                            List<string> entitiesOfThamGiaXuLy = item.NGUOI_THAMGIA_XULY.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (entitiesOfThamGiaXuLy.Any())
                            {
                                item.IDS_THAMGIA_XULY = new List<long>();
                                foreach (string entity in entitiesOfThamGiaXuLy)
                                {
                                    string entityStr = !string.IsNullOrEmpty(entity) ? entity.Trim() : string.Empty;
                                    DM_NGUOIDUNG user = users.FirstOrDefault(x => x.EMAIL.Equals(entityStr));
                                    if (user != null)
                                    {
                                        item.IDS_THAMGIA_XULY.Add(user.ID);
                                    }
                                }
                            }
                        }

                        item.DOKHAN = itemDoKhan.Value.ToIntOrZero();
                        item.DOUU_TIEN = itemMucDoUuTien.Value.ToIntOrZero();

                        if (((item.IDS_THAMGIA_XULY != null && item.IDS_THAMGIA_XULY.Count > 0) || item.NGUOIXULYCHINH_ID > 0) && item.NGAY_NHANVIEC == null)
                        {
                            item.NGAY_NHANVIEC = DateTime.Now;
                        }
                        item.NGAYHOANTHANH_THEOMONGMUON_TEXT = item.NGAYHOANTHANH_THEOMONGMUON != null ? string.Format("{0:dd/MM/yyyy}", item.NGAYHOANTHANH_THEOMONGMUON.Value) : string.Empty;
                        item.NGAY_NHANVIEC_TEXT = item.NGAY_NHANVIEC != null ? string.Format("{0:dd/MM/yyyy}", item.NGAY_NHANVIEC.Value) : string.Empty;
                        result.ListData.Add(item);
                    }
                }
                result.ListFalse = checkResult.lstFalse;
            }
            else
            {
                checkResult.Status = false;
                result.Message = checkResult.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: import công việc
        /// @since: 10/08/2018
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveImportTasks(List<List<string>> data)
        {
            AssignUserInfo();
            var hscvCongViecBusiness = Get<HSCV_CONGVIECBusiness>();
            var hscvCongViecNguoiThamGiaXuLyBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            var saveResult = new JsonResultBO(true);
            List<HSCV_CONGVIEC> groupTasksToInsert = new List<HSCV_CONGVIEC>();
            List<List<long>> taskJoiners = new List<List<long>>();
            try
            {
                foreach (var item in data)
                {
                    List<long> joiners = new List<long>();

                    HSCV_CONGVIEC task = new HSCV_CONGVIEC();
                    task.TENCONGVIEC = item[0].Trim();
                    task.DOUU_TIEN = item[1].ToIntOrZero();
                    task.DOKHAN = item[2].ToIntOrZero();
                    task.NGUOIGIAOVIEC_ID = item[3].ToIntOrNULL();
                    task.NGUOIXULYCHINH_ID = item[6].ToIntOrNULL();
                    if (task.NGUOIGIAOVIEC_ID > 0)
                    {
                        if (task.NGUOIXULYCHINH_ID == null)
                        {
                            task.NGUOIXULYCHINH_ID = currentUser.ID;
                        }

                        if (!hscvCongViecBusiness.CheckCanAssignOrJoinTask(task.NGUOIGIAOVIEC_ID.Value, task.NGUOIXULYCHINH_ID.Value))
                        {
                            continue;
                        }
                        else
                        {
                            //trường hợp người xử lý chính là người giao việc
                            if (task.NGUOIXULYCHINH_ID == task.NGUOIGIAOVIEC_ID)
                            {
                                task.NGUOIXULYCHINH_ID = null;
                                task.DAGIAOVIEC = false;
                            }
                            else
                            {
                                task.IS_ASSIGNED = true;
                                task.DAGIAOVIEC = true;

                                //lấy danh sách người tham gia xử lý
                                joiners = item[7].ToListLong(',');
                            }
                            task.TRANGTHAI_ID = TrangThaiCongViecConstant.PENDING;
                        }
                    }
                    else
                    {
                        task.NGUOIGIAOVIEC_ID = currentUser.ID;
                        task.TRANGTHAI_ID = TrangThaiCongViecConstant.APPROVED;
                    }
                    task.NGAY_NHANVIEC = item[4].ToDateTime();
                    task.NGAYHOANTHANH_THEOMONGMUON = item[5].ToDateTime();
                    task.SONGAYNHACTRUOCHAN = item[8].ToIntOrZero();
                    task.NOIDUNGCONGVIEC = item[9];
                    task.MUCTIEU_CONGVIEC = item[10];
                    groupTasksToInsert.Add(task);

                    //cập nhật người tham gia xử lý
                    taskJoiners.Add(joiners);
                }

                //using transaction to insert
                using (var transaction = hscvCongViecBusiness.context.Database.BeginTransaction())
                {
                    hscvCongViecBusiness.context.HSCV_CONGVIEC.AddRange(groupTasksToInsert);
                    hscvCongViecBusiness.context.SaveChanges();
                    transaction.Commit();
                }

                //execute HSCV_CONGVIEC_NGUOITHAMGIAXULY
                List<HSCV_CONGVIEC_NGUOITHAMGIAXULY> entitiesOfJoiner = new List<HSCV_CONGVIEC_NGUOITHAMGIAXULY>();

                if(taskJoiners.Count() > 0)
                {
                    foreach (var item in groupTasksToInsert)
                    {
                        int index = groupTasksToInsert.IndexOf(item);
                        List<long> joiners = taskJoiners[index];

                        foreach (var joinItem in joiners)
                        {
                            //kiểm tra có thể giao việc được không
                            if (hscvCongViecBusiness.CheckCanAssignOrJoinTask(item.NGUOIGIAOVIEC_ID.GetValueOrDefault(), joinItem))
                            {
                                HSCV_CONGVIEC_NGUOITHAMGIAXULY join = new HSCV_CONGVIEC_NGUOITHAMGIAXULY();
                                join.CONGVIEC_ID = item.ID;
                                join.USER_ID = joinItem;
                                entitiesOfJoiner.Add(join);
                            }
                        }
                    }
                }

                //using transaction to insert
                if(entitiesOfJoiner.Count > 0)
                {
                    using (var transaction = hscvCongViecNguoiThamGiaXuLyBusiness.context.Database.BeginTransaction())
                    {
                        hscvCongViecNguoiThamGiaXuLyBusiness.context.HSCV_CONGVIEC_NGUOITHAMGIAXULY.AddRange(entitiesOfJoiner);
                        hscvCongViecNguoiThamGiaXuLyBusiness.context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                saveResult.Status = false;
                saveResult.Message = "Lỗi dữ liệu, không thể import";
            }
            return Json(saveResult);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 10/08/2018
        /// @description: kết xuất file công việc bị lỗi
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportErrorTasks(List<List<string>> data)
        {
            ExportExcelHelper<CongViecBO> exPro = new CommonHelper.ExportExcelHelper<CongViecBO>();
            exPro.PathStore = Path.Combine(UPLOADFOLDER, "ErrorExport");
            exPro.PathTemplate = Path.Combine(UPLOADFOLDER, WebConfigurationManager.AppSettings["ImportCongViecTemplate"]);
            exPro.StartRow = 2;
            exPro.StartCol = 2;
            exPro.FileName = "ErrorExportCongViec";
            var result = exPro.ExportText(data);
            if (result.Status)
            {
                result.PathStore = Path.Combine(WEB_ADDRESS, "ErrorExport", result.FileName);
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 10/08/2018
        /// @description: kết xuất file công việc con bị lỗi
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportErrorSubTasks(List<List<string>> data)
        {
            ExportExcelHelper<HSCV_SUBTASK_BO> exPro = new CommonHelper.ExportExcelHelper<HSCV_SUBTASK_BO>();
            exPro.PathStore = Path.Combine(UPLOADFOLDER, "ErrorExport");
            exPro.PathTemplate = Path.Combine(UPLOADFOLDER, WebConfigurationManager.AppSettings["ImportCongViecConTemplate"]);
            exPro.StartRow = 2;
            exPro.StartCol = 2;
            exPro.FileName = "ErrorExportCongViecCon";
            var result = exPro.ExportText(data);
            if (result.Status)
            {
                result.PathStore = Path.Combine(WEB_ADDRESS, "ErrorExport", result.FileName);
            }
            return Json(result);
        }

        /// <summary>
        /// @description: import công việc con
        /// @author: duynn
        /// @since: 10/08/2018
        /// </summary>
        /// <param name="data"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveImportSubTasks(List<List<string>> data, long taskId)
        {
            AssignUserInfo();
            var hscvSubTaskBusiness = Get<HSCV_SUBTASKBusiness>();
            var saveResult = new JsonResultBO(true);
            List<HSCV_SUBTASK> groupSubTasksToInsert = new List<HSCV_SUBTASK>();
            try
            {
                foreach (var item in data)
                {
                    HSCV_SUBTASK subTask = new HSCV_SUBTASK();
                    subTask.CONGVIEC_ID = taskId;
                    subTask.NOIDUNG = item[0];
                    subTask.MUCDOUUTIEN = item[1].ToIntOrZero();
                    subTask.DOKHAN = item[2].ToIntOrZero();
                    subTask.HANHOANTHANH = item[3].ToDateTime();
                    subTask.IS_HASPLAN = item[4].Equals("true");
                    subTask.NGAYTAO = DateTime.Now;
                    subTask.NGUOITAO = currentUser.ID;
                    subTask.TRANGTHAI_ID = 0;
                    groupSubTasksToInsert.Add(subTask);
                }

                //using transaction to insert
                using (var transaction = hscvSubTaskBusiness.context.Database.BeginTransaction())
                {
                    hscvSubTaskBusiness.context.HSCV_SUBTASK.AddRange(groupSubTasksToInsert);
                    hscvSubTaskBusiness.context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                saveResult.Status = false;
                saveResult.Message = "Lỗi dữ liệu, không thể import";
            }
            return Json(saveResult);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 09/08/2018
        /// @description: trở về danh sách
        /// </summary>
        /// <returns></returns>
        public ActionResult NavigateBackToList(string controllerName = "QuanLyCongViec")
        {
            return RedirectToAction("Index", controllerName);
        }
    }
}