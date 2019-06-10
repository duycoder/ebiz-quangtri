using Business.Business;
using Business.CommonModel.CONSTANT;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Business.CommonBusiness;
using Web.Areas.HSVanBanDiArea.Models;
using Web.Common;
using Web.Custom;
using Web.Common.Elastic;
using Business.CommonModel.DMNguoiDung;

namespace Web.Areas.WFSTREAMArea.Controllers
{
    public class FunctionStateController : BaseController
    {
        // GET: WFSTREAMArea/FunctionState
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private HSCV_VANBANDENBusiness HSCV_VANBANDENBusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private HSCV_VANBANDEN_DONVINHANBusiness HSCV_VANBANDEN_DONVINHANBusiness;
        private string DEPTTYLELABEL = WebConfigurationManager.AppSettings["DEPTTYLELABEL"];
        private string SERVERADDRESS = WebConfigurationManager.AppSettings["SERVERADDRESS"];
        private WF_PROCESSBusiness wfProcessBusiness;
        private QL_NGUOINHAN_VANBANBusiness recipientBusiness;
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult LuuSoPhatHanh(int id)
        {
            AssignUserInfo();
            var HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            var CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var DmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            recipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();

            VanBanDiVM model = new VanBanDiVM();
            model.TreeDonVi = CCTC_THANHPHANBusiness.GetTreeLabel(currentUser);
            model.VanBan = HSCV_VANBANDIBusiness.Find(id);

            if (!string.IsNullOrEmpty(model.VanBan.DONVINHAN_INTERNAL_ID))
            {
                model.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(model.VanBan.DONVINHAN_INTERNAL_ID.ToListInt(','));
            }
            if (model.ListDonVi == null)
            {
                model.ListDonVi = new List<CCTC_THANHPHAN>();
            }
            model.GroupUserIdsReceiveDirectly = new List<long>();
            if (!string.IsNullOrEmpty(model.VanBan.USER_RECEIVE_DIRECTLY))
            {
                model.GroupUserIdsReceiveDirectly = model.VanBan.USER_RECEIVE_DIRECTLY.ToListLong(',');
                model.LstReceiveDirectly = DM_NGUOIDUNGBusiness.GetDanhSachByListIds(model.GroupUserIdsReceiveDirectly);
            }
            else
            {
                model.LstReceiveDirectly = new List<SelectListItem>();
            }
            model.Recipients = recipientBusiness.GetRecipientGroups(currentUser.DeptParentID.GetValueOrDefault());
            model.LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.DOUUTIEN, 0, model.VanBan.DOUUTIEN_ID.HasValue ? model.VanBan.DOUUTIEN_ID.Value : 0);
            model.LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.DOQUANTRONG, 0, model.VanBan.DOKHAN_ID.HasValue ? model.VanBan.DOKHAN_ID.Value : 0);
            model.LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LINHVUCVANBAN, 0, model.VanBan.LINHVUCVANBAN_ID.HasValue ? model.VanBan.LINHVUCVANBAN_ID.Value : 0);
            model.LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.LOAIVANBAN, 0, model.VanBan.LOAIVANBAN_ID.HasValue ? model.VanBan.LOAIVANBAN_ID.Value : 0);
            model.ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.VANBAN);
            model.ListTaiLieu = model.ListTaiLieu.Where(x => x.IS_ACTIVE == 1)
                .GroupBy(x => x.TAILIEU_GOC_ID).Select(x => x.FirstOrDefault())
                .ToList();
            model.NguoiKy = DM_NGUOIDUNGBusiness.Find(model.VanBan.NGUOIKY_ID);
            model.LstSoVanBanDi = DM_DANHMUC_DATABusiness.DsByMaNhomByDept(VanBanConstant.SOVANBANDI, 0, currentUser.DeptParentID.Value);
            model.CodeDocumentType = (DM_DANHMUC_DATABusiness.Find(model.VanBan.LOAIVANBAN_ID) ?? new DM_DANHMUC_DATA()).CODE;
            model.CodePublishDepartment = (CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID) ?? new CCTC_THANHPHAN()).CODE;
            return PartialView("_LuuSoPhatHanh", model);
        }
        public JsonResult SavePhatHanhVanBan(FormCollection col)
        {
            AssignUserInfo();
            SMSDAL.SendSMSDAL sms = new SMSDAL.SendSMSDAL();
            var LogSMSBusiness = Get<LogSMSBusiness>();
            string selectedDept = col["department-choose"];
            long? ID = col["ID"].ToLongOrNULL();
            if (0 >= ID)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy văn bản cần lưu sổ và phát hành" });
            }
            var dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

            #region cập nhật thông tin văn bản đi
            HSCV_VANBANDI VanBanDi = HSCV_VANBANDIBusiness.Find(ID);
            if (VanBanDi == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy văn bản cần lưu sổ và phát hành" });
            }

            if (!string.IsNullOrEmpty(col["SOHIEU"]))
            {
                VanBanDi.SOHIEU = col["SOHIEU"];
            }
            VanBanDi.USER_RECEIVE_DIRECTLY = col["USERS_RECEIVE_SPECIAL"];
            VanBanDi.DONVINHAN_INTERNAL_ID = selectedDept;
            VanBanDi.SOTHEOSO = col["SOTHEOSO"];
            VanBanDi.SOVANBAN_ID = col["SOVANBAN_ID"].ToIntOrNULL();
            if (!string.IsNullOrEmpty(col["TRICHYEU"]))
            {
                VanBanDi.TRICHYEU = col["TRICHYEU"].Trim();
            }
            if (!string.IsNullOrEmpty(col["NGAY_HIEULUC"]))
            {
                try
                {
                    VanBanDi.NGAYCOHIEULUC = col["NGAY_HIEULUC"].ToDateTime();
                }
                catch (Exception e)
                {

                }
            }
            if (!string.IsNullOrEmpty(col["NGAYHET_HIEULUC"]))
            {
                try
                {
                    VanBanDi.NGAYHETHIEULUC = col["NGAYHET_HIEULUC"].ToDateTime();
                }
                catch (Exception e)
                {

                }
            }
            if (!string.IsNullOrEmpty(col["NGAYBANHANH"]))
            {
                try
                {
                    VanBanDi.NGAYBANHANH = col["NGAYBANHANH"].ToDateTime();
                }
                catch (Exception e)
                {

                }
            }
            HSCV_VANBANDIBusiness.Save(VanBanDi);
            #endregion

            //cập nhật số đi theo văn bản vào số văn bản hiện tại
            int numbSoDiTheoSo = VanBanDi.SOTHEOSO.GetPrefixNumber();
            dmDanhMucDataBusiness.UpdateSoVanBan(VanBanDi.SOVANBAN_ID.GetValueOrDefault(), numbSoDiTheoSo);

            #region Convert Văn bản
            List<int> ListDonVi = selectedDept.ToListInt(',');
            DateTime? NGAY_HIEULUC = col["NGAY_HIEULUC"].ToDateTime();
            DateTime? NGAYHET_HIEULUC = col["NGAYHET_HIEULUC"].ToDateTime();
            string TRICHYEU = col["TRICHYEU"];
            string SOHIEU = col["SOHIEU"];
            if (!string.IsNullOrEmpty(SOHIEU))
            {
                SOHIEU = SOHIEU.Trim();
            }
            if (!string.IsNullOrEmpty(TRICHYEU))
            {
                TRICHYEU = TRICHYEU.Trim();
            }

            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            List<TAILIEUDINHKEM> ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(ID.HasValue ? ID.Value : 0, LOAITAILIEU.VANBAN);
            var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(VanBanDi.NGUOIKY_ID);
            #region lấy toàn bộ người có vai trò nhận văn bản đến nội bộ của đơn vị
            var WFStateBusiness = Get<WF_STATEBusiness>();
            #region convert văn bản đi thành văn bản đến 
            // Lấy toàn bộ các đơn vị được gửi văn bản
            // Lấy thông tin đơn vị hiện tại
            var CCToChucBusiness = Get<CCTC_THANHPHANBusiness>();
            var WFStreamBusiness = Get<WF_STREAMBusiness>();
            var WFModuleBusiness = Get<WF_MODULEBusiness>();
            var wfProcessBusiness = Get<WF_PROCESSBusiness>();
            var dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();

            var SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            var CurrentDept = CCToChucBusiness.Find(currentUser.DM_PHONGBAN_ID);

            var LstObjDeptSend = CCToChucBusiness.repository.All().Where(x => ListDonVi.Contains(x.ID)).ToList();
            //lấy danh sách người nhận văn bản trực tiếp
            var LstNguoiNhanDichDanh = VanBanDi.USER_RECEIVE_DIRECTLY.ToListLong(',');
            LstNguoiNhanDichDanh = LstNguoiNhanDichDanh.Distinct().ToList();
            //var groupUsersReceiveDirectly = new List<DM_NGUOIDUNG_BO>();
            //if (VanBanDi.USER_RECEIVE_DIRECTLY != null)
            //{
            //    groupUsersReceiveDirectly = dmNguoiDungBusiness.GetUsersByGroupIds(VanBanDi.USER_RECEIVE_DIRECTLY.ToListLong(','));
            //}

            //kiểm tra có thể gửi tin nhắn
            #region gui den cac don vi
            foreach (var itemdept in LstObjDeptSend)
            {
                //- trường hợp gửi cho các ban ngành cấp tỉnh và các huyện xa -> thành văn bản đến của các đơnvị nhận được
                if (itemdept.TYPE == DEPTTYLELABEL.ToIntOrZero())
                {
                    //- trường hợp này là văn bản đến bình thường
                    var WFModuleObj = WFModuleBusiness.repository.All().Where(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANDEN)
                        .FirstOrDefault();
                    var LstWfIds = WFModuleObj.WF_STREAM_ID.ToListInt(',');
                    //- lấy luồng xử lý của từng đơn vị
                    var WFStreamObj = WFStreamBusiness.repository.All()
                        .Where(x => x.LEVEL_ID == itemdept.CATEGORY && LstWfIds.Contains(x.ID)).FirstOrDefault();
                    var StateObj = WFStateBusiness.repository.All().Where(x => x.IS_START == true && x.WF_ID == WFStreamObj.ID)
                        .FirstOrDefault();
                    if (StateObj != null)
                    {
                        if (StateObj.VAITRO_ID != null)
                        {
                            var NguoiNhan = DM_NGUOIDUNGBusiness.GetUserByRoleAndParentDept(StateObj.VAITRO_ID.Value, itemdept.ID).FirstOrDefault();
                            if (NguoiNhan != null)
                            {
                                UserInfoBO processUserBo = DM_NGUOIDUNGBusiness.GetNewUserInfo(NguoiNhan.Value.ToLongOrZero());
                                if (processUserBo != null)
                                {
                                    //- Sổ văn bản đến
                                    var SoDen = dmDanhMucDataBusiness.GetSoVanBan(DMLOAI_CONSTANT.SOVANBANDEN, DateTime.Now.Year, itemdept.ID);
                                    // - end
                                    if (SoDen != null)
                                    {
                                        #region lưu thông tin văn bản đến
                                        HSCV_VANBANDEN VanBanDen = new HSCV_VANBANDEN();
                                        VanBanDen.SOHIEU = VanBanDi.SOHIEU;
                                        VanBanDen.DOKHAN_ID = VanBanDi.DOKHAN_ID;
                                        VanBanDen.DOMAT_ID = VanBanDi.DOUUTIEN_ID;
                                        VanBanDen.LINHVUCVANBAN_ID = VanBanDi.LINHVUCVANBAN_ID;
                                        VanBanDen.LOAIVANBAN_ID = VanBanDi.LOAIVANBAN_ID;
                                        VanBanDen.NGAYHET_HIEULUC = NGAYHET_HIEULUC;
                                        VanBanDen.NGAYTAO = VanBanDi.CREATED_AT;
                                        VanBanDen.NGAY_HIEULUC = NGAY_HIEULUC;
                                        VanBanDen.NGUOITAO = VanBanDi.CREATED_BY;
                                        VanBanDen.NOIDUNG = VanBanDi.NOIDUNG;
                                        VanBanDen.SOHIEU = SOHIEU;
                                        VanBanDen.TRICHYEU = TRICHYEU;
                                        VanBanDen.VANBANDI_ID = VanBanDi.ID;
                                        VanBanDen.NGUOIKY = NguoiDung != null ? NguoiDung.HOTEN : null;
                                        VanBanDen.CHUCVU = VanBanDi.CHUCVU;
                                        VanBanDen.NGAY_BANHANH = DateTime.Now;
                                        VanBanDen.SOTRANG = VanBanDi.SOTO;
                                        VanBanDen.SOVANBANDEN_ID = SoDen.ID;
                                        VanBanDen.SODITHEOSO = SoDen.GHICHU.ToIntOrZero().ToString();
                                        VanBanDen.SODITHEOSO_NUMBER = SoDen.GHICHU.ToIntOrZero();
                                        //- Cập nhật số theo sổ
                                        SoDen.GHICHU = (SoDen.GHICHU.ToIntOrZero() + 1).ToString();
                                        dmDanhMucDataBusiness.Save(SoDen);
                                        // - End
                                        HSCV_VANBANDENBusiness.Save(VanBanDen);
                                        #endregion
                                        List<TAILIEUDINHKEM> ListTemp = ListTaiLieu;
                                        #region Thêm tài liệu đính kèm cho văn bản
                                        foreach (var file in ListTemp)
                                        {
                                            var tmpObj = new TAILIEUDINHKEM();
                                            tmpObj.IS_ACTIVE = file.IS_ACTIVE;
                                            tmpObj.ACCESS_MODIFIER = file.ACCESS_MODIFIER;
                                            tmpObj.CONTENT_CHANGE = file.CONTENT_CHANGE;
                                            tmpObj.DINHDANG_FILE = file.DINHDANG_FILE;
                                            tmpObj.DM_LOAITAILIEU_ID = file.DM_LOAITAILIEU_ID;
                                            tmpObj.DONVI_ID = file.DONVI_ID;
                                            tmpObj.DUONGDAN_FILE = file.DUONGDAN_FILE;
                                            tmpObj.FOLDER_ID = file.FOLDER_ID;
                                            tmpObj.IS_PHEDUYET = file.IS_PHEDUYET;
                                            tmpObj.IS_PRIVATE = file.IS_PRIVATE;
                                            tmpObj.IS_QLPHIENBAN = file.IS_QLPHIENBAN;
                                            tmpObj.KICHCO = file.KICHCO;
                                            tmpObj.MATAILIEU = file.MATAILIEU;
                                            tmpObj.MOTA = file.MOTA;
                                            tmpObj.SOLUONG_DOWNLOAD = file.SOLUONG_DOWNLOAD;
                                            tmpObj.NGAYTAO = DateTime.Now;
                                            tmpObj.NGAYPHATHANH = DateTime.Now;
                                            tmpObj.VERSION = file.VERSION;
                                            tmpObj.TENTACGIA = file.TENTACGIA;
                                            tmpObj.TENTAILIEU = file.TENTAILIEU;
                                            tmpObj.PDF_VERSION = file.PDF_VERSION;
                                            tmpObj.PERMISSION = file.PERMISSION;
                                            tmpObj.TRANGTHAI = file.TRANGTHAI;
                                            tmpObj.ITEM_ID = VanBanDen.ID;
                                            tmpObj.USER_ID = file.USER_ID;
                                            tmpObj.LOAI_TAILIEU = LOAITAILIEU.VANBANDEN;
                                            tmpObj.IS_LOCK = file.IS_LOCK;
                                            tmpObj.NGUOI_LOCK = file.NGUOI_LOCK;
                                            TAILIEUDINHKEMBusiness.Save(tmpObj);
                                        }
                                        #endregion
                                        JsonResultBO processResult = wfProcessBusiness.AddFlow(VanBanDen.ID, MODULE_CONSTANT.VANBANDEN, processUserBo);

                                        #region insert into elastic
                                        var lstMainUser = new List<long>();
                                        lstMainUser.Add(processUserBo.ID);
                                        ElasticSearch.updateListUser(VanBanDen.ID.ToString(), lstMainUser, ElasticType.VanBanDen);
                                        #endregion
                                        #region gửi email cho người xử lý chính
                                        List<string> lstEmail = new List<string>();
                                        lstEmail.Add(processUserBo.EMAIL);
                                        if (processUserBo.EMAIL != null && !string.IsNullOrEmpty(processUserBo.EMAIL))
                                        {
                                            var ContentEmail = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến <a href='" + SERVERADDRESS + "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + VanBanDen.ID.ToString() + "'>" + SOHIEU + "</a>";
                                            EmailProvider.SendMailTemplate(currentUser, ContentEmail, ContentEmail, lstEmail);
                                        }
                                        #endregion
                                        #region gửi sms cho người xử lý
                                        if (currentUser.CanSendSMS && VanBanDi.CAN_SEND_SMS == true)
                                        {
                                            if (processUserBo.DIENTHOAI != null && !String.IsNullOrEmpty(processUserBo.DIENTHOAI))
                                            {
                                                var ContentSMS = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến " + VanBanDen.SOHIEU;
                                                ContentSMS = sms.locDau(ContentSMS);
                                                var DoDaiSMS = ContentSMS.Length;
                                                string[] noiDung = new string[1];
                                                noiDung[0] = ContentSMS;
                                                string resultsend = sms.guiTinNhan(processUserBo.DIENTHOAI, "177403", noiDung);
                                                LOGSMS SmsObject = new LOGSMS();
                                                SmsObject.SODIENTHOAINHAN = processUserBo.DIENTHOAI;
                                                SmsObject.NOIDUNG = ContentSMS;
                                                SmsObject.SOKYTU = DoDaiSMS;
                                                SmsObject.KETQUA = resultsend;
                                                SmsObject.CREATED_AT = DateTime.Now;
                                                SmsObject.CREATED_BY = currentUser.ID;
                                                SmsObject.HOTENNGUOIGUI = currentUser.HOTEN;
                                                SmsObject.DONVI_GUI = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                                                SmsObject.ITEMTYPE = "VANBANDEN";
                                                SmsObject.ITEMID = VanBanDen.ID;
                                                SmsObject.DONVI_NHAN = processUserBo.DM_PHONGBAN_ID.GetValueOrDefault();
                                                SmsObject.HOTENNGUOINHAN = processUserBo.HOTEN;
                                                LogSMSBusiness.Save(SmsObject);
                                            }
                                        }
                                        #endregion
                                        #region gửi notification
                                        SYS_TINNHAN noti = new SYS_TINNHAN();
                                        noti.FROM_USERNAME = currentUser.HOTEN;
                                        noti.FROM_USER_ID = currentUser.ID;
                                        noti.NGAYTAO = DateTime.Now;
                                        noti.NOIDUNG = currentUser.TenPhongBan + " đã gửi đến bạn một văn bản đến";
                                        noti.URL = "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen/" + VanBanDen.ID.ToString();
                                        noti.TIEUDE = "Xử lý văn bản đến";
                                        noti.TO_USER_ID = processUserBo.ID;
                                        SYS_TINNHANBusiness.Save(noti, "", false, VanBanDen.ID, TargetDocType.COORDINATED);
                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                }
                else if (itemdept.PARENT_ID == CurrentDept.PARENT_ID)
                {
                    //- trường hợp gửi các phòng ban thuộc tỉnh ủy => thành văn bản đến nội bộ của tỉnh
                    var WFModuleObj = WFModuleBusiness.repository.All().Where(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANDENNOIBO)
                        .FirstOrDefault();
                    var LstWfIds = WFModuleObj.WF_STREAM_ID.ToListInt(',');
                    //- lấy luồng xử lý của từng đơn vị
                    var WFStreamObj = WFStreamBusiness.repository.All()
                        .Where(x => x.LEVEL_ID == itemdept.CATEGORY && LstWfIds.Contains(x.ID)).FirstOrDefault();
                    var StateObj = WFStateBusiness.repository.All().Where(x => x.IS_START == true && x.WF_ID == WFStreamObj.ID)
                        .FirstOrDefault();
                    if (StateObj != null)
                    {
                        if (StateObj.VAITRO_ID != null)
                        {
                            var NguoiNhan = DM_NGUOIDUNGBusiness.GetUserByRoleAndDeptId(StateObj.VAITRO_ID.Value, itemdept.ID).FirstOrDefault();
                            if (NguoiNhan != null)
                            {
                                UserInfoBO processUserBo = DM_NGUOIDUNGBusiness.GetNewUserInfo(NguoiNhan.Value.ToLongOrZero());
                                if (processUserBo != null)
                                {
                                    HSCV_VANBANDEN VanBanDen = new HSCV_VANBANDEN();
                                    VanBanDen.SOHIEU = VanBanDi.SOHIEU;
                                    VanBanDen.DOKHAN_ID = VanBanDi.DOKHAN_ID;
                                    VanBanDen.DOMAT_ID = VanBanDi.DOUUTIEN_ID;
                                    VanBanDen.LINHVUCVANBAN_ID = VanBanDi.LINHVUCVANBAN_ID;
                                    VanBanDen.LOAIVANBAN_ID = VanBanDi.LOAIVANBAN_ID;
                                    VanBanDen.NGAYHET_HIEULUC = NGAYHET_HIEULUC;
                                    VanBanDen.NGAYTAO = VanBanDi.CREATED_AT;
                                    VanBanDen.NGAY_HIEULUC = NGAY_HIEULUC;
                                    VanBanDen.NGUOITAO = VanBanDi.CREATED_BY;
                                    VanBanDen.NOIDUNG = VanBanDi.NOIDUNG;
                                    VanBanDen.SOHIEU = SOHIEU;
                                    VanBanDen.TRICHYEU = TRICHYEU;
                                    VanBanDen.VANBANDI_ID = VanBanDi.ID;
                                    VanBanDen.NGUOIKY = NguoiDung != null ? NguoiDung.HOTEN : null;
                                    VanBanDen.CHUCVU = VanBanDi.CHUCVU;
                                    VanBanDen.NGAY_BANHANH = DateTime.Now;
                                    VanBanDen.SOTRANG = VanBanDi.SOTO;
                                    VanBanDen.IS_NOIBO = true;

                                    HSCV_VANBANDENBusiness.Save(VanBanDen);
                                    List<TAILIEUDINHKEM> ListTemp = ListTaiLieu;
                                    #region Thêm tài liệu đính kèm cho văn bản
                                    foreach (var file in ListTemp)
                                    {
                                        var tmpObj = new TAILIEUDINHKEM();
                                        tmpObj.IS_ACTIVE = file.IS_ACTIVE;
                                        tmpObj.ACCESS_MODIFIER = file.ACCESS_MODIFIER;
                                        tmpObj.CONTENT_CHANGE = file.CONTENT_CHANGE;
                                        tmpObj.DINHDANG_FILE = file.DINHDANG_FILE;
                                        tmpObj.DM_LOAITAILIEU_ID = file.DM_LOAITAILIEU_ID;
                                        tmpObj.DONVI_ID = file.DONVI_ID;
                                        tmpObj.DUONGDAN_FILE = file.DUONGDAN_FILE;
                                        tmpObj.FOLDER_ID = file.FOLDER_ID;
                                        tmpObj.IS_PHEDUYET = file.IS_PHEDUYET;
                                        tmpObj.IS_PRIVATE = file.IS_PRIVATE;
                                        tmpObj.IS_QLPHIENBAN = file.IS_QLPHIENBAN;
                                        tmpObj.KICHCO = file.KICHCO;
                                        tmpObj.MATAILIEU = file.MATAILIEU;
                                        tmpObj.MOTA = file.MOTA;
                                        tmpObj.SOLUONG_DOWNLOAD = file.SOLUONG_DOWNLOAD;
                                        tmpObj.NGAYTAO = DateTime.Now;
                                        tmpObj.NGAYPHATHANH = DateTime.Now;
                                        tmpObj.VERSION = file.VERSION;
                                        tmpObj.TENTACGIA = file.TENTACGIA;
                                        tmpObj.TENTAILIEU = file.TENTAILIEU;
                                        tmpObj.PDF_VERSION = file.PDF_VERSION;
                                        tmpObj.PERMISSION = file.PERMISSION;
                                        tmpObj.TRANGTHAI = file.TRANGTHAI;
                                        tmpObj.ITEM_ID = VanBanDen.ID;
                                        tmpObj.USER_ID = file.USER_ID;
                                        tmpObj.LOAI_TAILIEU = LOAITAILIEU.VANBANDEN;
                                        tmpObj.IS_LOCK = file.IS_LOCK;
                                        tmpObj.NGUOI_LOCK = file.NGUOI_LOCK;
                                        TAILIEUDINHKEMBusiness.Save(tmpObj);
                                    }
                                    #endregion
                                    JsonResultBO processResult = wfProcessBusiness.AddFlow(VanBanDen.ID, MODULE_CONSTANT.VANBANDENNOIBO, processUserBo);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region  gửi cho người nhận đích danh văn bản

            wfProcessBusiness = Get<WF_PROCESSBusiness>();
            foreach (var userid in LstNguoiNhanDichDanh)
            {
                var tmpCurrUser = dmNguoiDungBusiness.GetNewUserInfo(userid);
                var SoDen = dmDanhMucDataBusiness.GetSoVanBan(DMLOAI_CONSTANT.SOVANBANDEN, DateTime.Now.Year, tmpCurrUser.DeptParentID.Value);
                if (SoDen != null)
                {
                    #region lưu thông tin văn bản đến
                    HSCV_VANBANDEN VanBanDen = new HSCV_VANBANDEN();
                    VanBanDen.SOHIEU = VanBanDi.SOHIEU;
                    VanBanDen.DOKHAN_ID = VanBanDi.DOKHAN_ID;
                    VanBanDen.DOMAT_ID = VanBanDi.DOUUTIEN_ID;
                    VanBanDen.LINHVUCVANBAN_ID = VanBanDi.LINHVUCVANBAN_ID;
                    VanBanDen.LOAIVANBAN_ID = VanBanDi.LOAIVANBAN_ID;
                    VanBanDen.NGAYHET_HIEULUC = NGAYHET_HIEULUC;
                    VanBanDen.NGAYTAO = VanBanDi.CREATED_AT;
                    VanBanDen.NGAY_HIEULUC = NGAY_HIEULUC;
                    VanBanDen.NGUOITAO = VanBanDi.CREATED_BY;
                    VanBanDen.NOIDUNG = VanBanDi.NOIDUNG;
                    VanBanDen.SOHIEU = SOHIEU;
                    VanBanDen.TRICHYEU = TRICHYEU;
                    VanBanDen.VANBANDI_ID = VanBanDi.ID;
                    VanBanDen.NGUOIKY = NguoiDung != null ? NguoiDung.HOTEN : null;
                    VanBanDen.CHUCVU = VanBanDi.CHUCVU;
                    VanBanDen.NGAY_BANHANH = DateTime.Now;
                    VanBanDen.SOTRANG = VanBanDi.SOTO;
                    VanBanDen.SOVANBANDEN_ID = SoDen.ID;
                    VanBanDen.SODITHEOSO = SoDen.GHICHU.ToIntOrZero().ToString();
                    VanBanDen.SODITHEOSO_NUMBER = SoDen.GHICHU.ToIntOrZero();
                    HSCV_VANBANDENBusiness.Save(VanBanDen);
                    #endregion
                    JsonResultBO processResult = wfProcessBusiness.AddFlow(VanBanDen.ID, MODULE_CONSTANT.VANBANDEN, tmpCurrUser);
                    if (processResult.Status)
                    {
                        //- Cập nhật số theo sổ
                        SoDen.GHICHU = (SoDen.GHICHU.ToIntOrZero() + 1).ToString();
                        dmDanhMucDataBusiness.Save(SoDen);
                        // - End
                        List<TAILIEUDINHKEM> ListTemp = ListTaiLieu;
                        #region Thêm tài liệu đính kèm cho văn bản
                        foreach (var file in ListTemp)
                        {
                            var tmpObj = new TAILIEUDINHKEM();
                            tmpObj.IS_ACTIVE = file.IS_ACTIVE;
                            tmpObj.ACCESS_MODIFIER = file.ACCESS_MODIFIER;
                            tmpObj.CONTENT_CHANGE = file.CONTENT_CHANGE;
                            tmpObj.DINHDANG_FILE = file.DINHDANG_FILE;
                            tmpObj.DM_LOAITAILIEU_ID = file.DM_LOAITAILIEU_ID;
                            tmpObj.DONVI_ID = file.DONVI_ID;
                            tmpObj.DUONGDAN_FILE = file.DUONGDAN_FILE;
                            tmpObj.FOLDER_ID = file.FOLDER_ID;
                            tmpObj.IS_PHEDUYET = file.IS_PHEDUYET;
                            tmpObj.IS_PRIVATE = file.IS_PRIVATE;
                            tmpObj.IS_QLPHIENBAN = file.IS_QLPHIENBAN;
                            tmpObj.KICHCO = file.KICHCO;
                            tmpObj.MATAILIEU = file.MATAILIEU;
                            tmpObj.MOTA = file.MOTA;
                            tmpObj.SOLUONG_DOWNLOAD = file.SOLUONG_DOWNLOAD;
                            tmpObj.NGAYTAO = DateTime.Now;
                            tmpObj.NGAYPHATHANH = DateTime.Now;
                            tmpObj.VERSION = file.VERSION;
                            tmpObj.TENTACGIA = file.TENTACGIA;
                            tmpObj.TENTAILIEU = file.TENTAILIEU;
                            tmpObj.PDF_VERSION = file.PDF_VERSION;
                            tmpObj.PERMISSION = file.PERMISSION;
                            tmpObj.TRANGTHAI = file.TRANGTHAI;
                            tmpObj.ITEM_ID = VanBanDen.ID;
                            tmpObj.USER_ID = file.USER_ID;
                            tmpObj.LOAI_TAILIEU = LOAITAILIEU.VANBANDEN;
                            tmpObj.IS_LOCK = file.IS_LOCK;
                            tmpObj.NGUOI_LOCK = file.NGUOI_LOCK;
                            TAILIEUDINHKEMBusiness.Save(tmpObj);
                        }
                        #endregion
                        #region insert into elastic
                        var lstMainUser = new List<long>();
                        lstMainUser.Add(tmpCurrUser.ID);
                        ElasticSearch.updateListUser(VanBanDen.ID.ToString(), lstMainUser, ElasticType.VanBanDen);
                        #endregion
                        #region gửi email cho người xử lý chính
                        List<string> lstEmail = new List<string>();
                        lstEmail.Add(tmpCurrUser.EMAIL);
                        if (tmpCurrUser.EMAIL != null && !string.IsNullOrEmpty(tmpCurrUser.EMAIL))
                        {
                            var ContentEmail = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến <a href='" + SERVERADDRESS + "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + VanBanDen.ID.ToString() + "'>" + SOHIEU + "</a>";
                            EmailProvider.SendMailTemplate(currentUser, ContentEmail, ContentEmail, lstEmail);
                        }
                        #endregion
                        #region gửi sms cho người xử lý

                        if (currentUser.CanSendSMS && VanBanDi.CAN_SEND_SMS == true)
                        {
                            if (tmpCurrUser.DIENTHOAI != null && !String.IsNullOrEmpty(tmpCurrUser.DIENTHOAI))
                            {
                                var ContentSMS = currentUser.TenPhongBan + " đã gửi bạn một văn bản đến " + VanBanDen.SOHIEU;
                                ContentSMS = sms.locDau(ContentSMS);
                                var DoDaiSMS = ContentSMS.Length;
                                string[] noiDung = new string[1];
                                noiDung[0] = ContentSMS;
                                string resultsend = sms.guiTinNhan(tmpCurrUser.DIENTHOAI, "177403", noiDung);
                                LOGSMS SmsObject = new LOGSMS();
                                SmsObject.SODIENTHOAINHAN = tmpCurrUser.DIENTHOAI;
                                SmsObject.NOIDUNG = ContentSMS;
                                SmsObject.SOKYTU = DoDaiSMS;
                                SmsObject.KETQUA = resultsend;
                                SmsObject.DONVI_GUI = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                                SmsObject.CREATED_AT = DateTime.Now;
                                SmsObject.CREATED_BY = currentUser.ID;
                                SmsObject.HOTENNGUOIGUI = currentUser.HOTEN;
                                SmsObject.ITEMTYPE = "VANBANDEN";
                                SmsObject.ITEMID = VanBanDen.ID;
                                SmsObject.HOTENNGUOINHAN = tmpCurrUser.HOTEN;
                                SmsObject.DONVI_NHAN = tmpCurrUser.DM_PHONGBAN_ID.GetValueOrDefault();
                                LogSMSBusiness.Save(SmsObject);
                            }
                        }
                        #endregion
                        #region gửi notification

                        SYS_TINNHAN noti = new SYS_TINNHAN();
                        noti.FROM_USERNAME = currentUser.HOTEN;
                        noti.FROM_USER_ID = currentUser.ID;
                        noti.NGAYTAO = DateTime.Now;
                        noti.NOIDUNG = currentUser.TenPhongBan + " đã gửi đến bạn một văn bản đến";
                        noti.URL = "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen/" + VanBanDen.ID.ToString();
                        noti.TIEUDE = "Xử lý văn bản đến";
                        noti.TO_USER_ID = tmpCurrUser.ID;
                        SYS_TINNHANBusiness.Save(noti, "", false, VanBanDen.ID, TargetDocType.COORDINATED);
                        #endregion
                    }
                    else
                    {
                        HSCV_VANBANDENBusiness.repository.Delete(VanBanDen);
                    }
                }
            }

            #endregion
            #endregion
            #endregion
            #endregion
            #region cập nhật wf function done
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var process = WF_PROCESSBusiness.GetProcess(VanBanDi.ID, MODULE_CONSTANT.VANBANTRINHKY);
            var WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
            var function = WF_STATE_FUNCTIONBusiness.GetStateFunction((int)process.CURRENT_STATE);
            var WF_FUNCTION_DONEBusiness = Get<WF_FUNCTION_DONEBusiness>();
            var WF_LOGBusiness = Get<WF_LOGBusiness>();
            var functionDone = new WF_FUNCTION_DONE();
            functionDone.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            functionDone.ITEM_ID = VanBanDi.ID;
            functionDone.STATE = process.CURRENT_STATE;
            functionDone.FUNCTION_STATE = function.ID;
            functionDone.create_at = DateTime.Now;
            AssignUserInfo();
            functionDone.create_by = currentUser.ID;
            WF_FUNCTION_DONEBusiness.Save(functionDone);
            var log = new WF_LOG();
            log.ITEM_ID = VanBanDi.ID;
            log.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            log.MESSAGE = "<div class='label label-info'>Đã phát hành</div>";
            log.WF_ID = process.WF_ID;
            log.create_at = DateTime.Now;
            log.create_by = currentUser.ID;
            log.NGUOIXULY_ID = currentUser.ID;
            WF_LOGBusiness.Save(log);
            //Ghi nhận luồng đã xử lý xong
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var state = WF_STATEBusiness.Find(process.CURRENT_STATE);
            if (state != null && state.IS_KETTHUC == true)
            {
                process.IS_END = true;
                WF_PROCESSBusiness.Save(process);
            }

            var WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
            var itemprocess = WF_ITEM_USER_PROCESSBusiness.repository.All().Where(x =>
                    x.ITEM_ID == process.ITEM_ID && x.ITEM_TYPE == process.ITEM_TYPE &&
                    x.STEP_ID == process.CURRENT_STATE)
                .FirstOrDefault();
            if (itemprocess != null)
            {
                itemprocess.DAXULY = true;
                WF_ITEM_USER_PROCESSBusiness.Save(itemprocess);
            }
            #endregion
            return Json(new { Type = "SUCCESS", Message = "Lưu sổ và phát hành văn bản thành công" });
        }

    }
}