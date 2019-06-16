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
        private DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;

        private WF_MODULEBusiness WF_MODULEBusiness;
        private WF_PROCESSBusiness WF_PROCESSBusiness;
        private WF_STREAMBusiness WF_STREAMBusiness;
        private WF_STATEBusiness WF_STATEBusiness;
        private WF_STATE_FUNCTIONBusiness WF_STATE_FUNCTIONBusiness;
        private WF_FUNCTION_DONEBusiness WF_FUNCTION_DONEBusiness;
        private WF_LOGBusiness WF_LOGBusiness;
        private WF_ITEM_USER_PROCESSBusiness WF_ITEM_USER_PROCESSBusiness;

        private SYS_TINNHANBusiness SYS_TINNHANBusiness;
        private LogSMSBusiness LogSMSBusiness;


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
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            LogSMSBusiness = Get<LogSMSBusiness>();

            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();


            WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();

            WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
            WF_STATEBusiness = Get<WF_STATEBusiness>();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();

            WF_FUNCTION_DONEBusiness = Get<WF_FUNCTION_DONEBusiness>();
            WF_LOGBusiness = Get<WF_LOGBusiness>();
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();

            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();

            string selectedDept = col["department-choose"];
            long? ID = col["ID"].ToLongOrNULL();
            if (0 >= ID)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy văn bản cần lưu sổ và phát hành" });
            }

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
            DM_DANHMUC_DATABusiness.UpdateSoVanBan(VanBanDi.SOVANBAN_ID.GetValueOrDefault(), numbSoDiTheoSo);

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

            List<TAILIEUDINHKEM> ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(ID.HasValue ? ID.Value : 0, LOAITAILIEU.VANBAN);
            DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(VanBanDi.NGUOIKY_ID);

            #endregion
            #region cập nhật wf function done

            var process = WF_PROCESSBusiness.GetProcess(VanBanDi.ID, MODULE_CONSTANT.VANBANTRINHKY);

            var function = WF_STATE_FUNCTIONBusiness.GetStateFunction((int)process.CURRENT_STATE);
            var functionDone = new WF_FUNCTION_DONE();
            functionDone.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            functionDone.ITEM_ID = VanBanDi.ID;
            functionDone.STATE = process.CURRENT_STATE;
            functionDone.FUNCTION_STATE = function.ID;
            functionDone.create_at = DateTime.Now;
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
            var state = WF_STATEBusiness.Find(process.CURRENT_STATE);
            if (state != null && state.IS_KETTHUC == true)
            {
                process.IS_END = true;
                WF_PROCESSBusiness.Save(process);
            }

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

            //gửi văn bản đi cho đơn vị
            SaveVanBanPhatHanhToDonVi(VanBanDi, sms);

            //gửi văn bản đi cho cá nhân
            SaveVanBanPhatHanhToCaNhan(VanBanDi, sms);

            return Json(new { Type = "SUCCESS", Message = "Lưu sổ và phát hành văn bản thành công" });
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