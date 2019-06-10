using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVVANBANDEN;
using Business.CommonModel.LICH_CONGTAC;
using Business.CommonModel.WFSTEP;
using CommonHelper;
using CommonHelper.DateExtend;
using CommonHelper.Excel;
using Model.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.HSCV_VANBANDENArea.Models;
using Web.Areas.HSVanBanDiArea.Models;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Common;
using Web.Common.Elastic;
using Web.Custom;
using Web.Filter;
using Web.FwCore;
using Web.Models;

namespace Web.Areas.HSCV_VANBANDENArea.Controllers
{
    public class HSCV_VANBANDENController : BaseController
    {
        private LICHCONGTACBusiness lichCongTacBusiness;
        private DM_DANHMUC_DATABusiness dmDanhMucDataBusiness;
        private CCTC_THANHPHANBusiness cctcThanhPhanBusiness;
        private HSCV_VANBANDENBusiness hscvVanBanDenBusiness;
        private WF_PROCESSBusiness wfProcessBusiness;
        private WF_ITEM_USER_PROCESSBusiness wfItemUserProcess;
        private DM_NGUOIDUNGBusiness dmNguoiDungBusiness;
        private WF_REVIEW_USERBusiness wfReviewUserBusiness;
        private WF_REVIEWBusiness wfReviewBusiness;
        private WF_STATEBusiness wfStateBusiness;
        private WF_STEPBusiness wfStepBusiness;
        private WF_STATE_FUNCTIONBusiness wfStateFunctionBusiness;
        private TAILIEUDINHKEMBusiness attachmentBusiness;
        private THUMUC_LUUTRUBusiness storeFolderBusiness;
        //private QL_NGUOINHAN_VANBANBusiness recipientBusiness;

        private string extensionOfVanBanDen = WebConfigurationManager.AppSettings["VbDenExtension"];
        private int maxSizeOfVanBanDen = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private string uploadFolder = WebConfigurationManager.AppSettings["FileUpload"];
        private int defaultPageSize = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);

        // GET: HSCV_VANBANDENArea/HSCV_VANBANDEN

        [ActionAudit]
        public ActionResult Index()
        {
            return RedirectToAction("ChuaXuLy");
            //return null;
        }

        /// <summary>
        /// @author: duynn
        /// @since: 06/08/2018
        /// @description: tạo mới văn bản đến
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult CreateVanBanDen(int type = VANBANDEN_CONSTANT.CHUA_XULY)
        {
            AssignUserInfo();
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            EditVanBanDenModel model = new EditVanBanDenModel();
            model.typeOfVanBanDen = type;
            model.groupDoKhans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOKHAN, 0);
            model.groupDoUuTiens = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.groupLoaiVanBans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
            model.groupLinhVucVanBans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
            model.groupSoVanBanDens = dmDanhMucDataBusiness.DsByMaNhomByDept(DMLOAI_CONSTANT.SOVANBANDEN, 0, currentUser.DeptParentID.GetValueOrDefault());
            model.groupDonViGuis = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DONVIGUI_VANBAN, 0);
            model.GroupDeptTypes = dmDanhMucDataBusiness.DsByMaNhom(VanBanConstant.LOAI_COQUAN, currentUser.ID);
            model.GroupDocTypes = dmDanhMucDataBusiness.DsByMaNhom(VanBanConstant.PHANLOAI_VANBAN, currentUser.ID);
            model.GroupDispatchTypes = dmDanhMucDataBusiness.DsByMaNhom(VanBanConstant.CONGVANDEN, currentUser.ID);
            model.groupTaiLieuDinhKems = new List<TAILIEUDINHKEM>();
            model.entityVanBanDen = new HSCV_VANBANDEN();

            model.fileExtension = extensionOfVanBanDen;
            model.fileMaxSize = maxSizeOfVanBanDen;
            return View(model);
        }

        /// <summary>
        /// @description: cập nhật văn bản đến
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult EditVanBanDen(long id, int type = VANBANDEN_CONSTANT.CHUA_XULY)
        {
            AssignUserInfo();
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            attachmentBusiness = Get<TAILIEUDINHKEMBusiness>();

            HSCV_VANBANDEN entityVanBanDen = hscvVanBanDenBusiness.Find(id);
            if (entityVanBanDen != null)
            {
                EditVanBanDenModel model = new EditVanBanDenModel(entityVanBanDen);
                model.typeOfVanBanDen = type;
                model.groupDonViGuis = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DONVIGUI_VANBAN, currentUser.ID, entityVanBanDen.DONVI_ID.GetValueOrDefault());
                model.groupDoKhans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOKHAN, currentUser.ID, entityVanBanDen.DOKHAN_ID.GetValueOrDefault());
                model.groupDoUuTiens = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, currentUser.ID, entityVanBanDen.DOMAT_ID.GetValueOrDefault());
                model.groupLoaiVanBans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, currentUser.ID, entityVanBanDen.LOAIVANBAN_ID.GetValueOrDefault());
                model.groupLinhVucVanBans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, currentUser.ID, entityVanBanDen.LINHVUCVANBAN_ID.GetValueOrDefault());
                model.groupSoVanBanDens = dmDanhMucDataBusiness.DsByMaNhomByDept(DMLOAI_CONSTANT.SOVANBANDEN, currentUser.ID, currentUser.DeptParentID.Value, entityVanBanDen.SOVANBANDEN_ID.GetValueOrDefault());
                model.groupTaiLieuDinhKems = attachmentBusiness.GetDataByItemID(entityVanBanDen.ID, LOAITAILIEU.VANBANDEN);
                model.entityVanBanDen = entityVanBanDen;
                model.fileExtension = extensionOfVanBanDen;
                model.fileMaxSize = maxSizeOfVanBanDen;

                model.UsersReceived = entityVanBanDen.NGUOINHAN_TRUCTIEP_IDS != null ? entityVanBanDen.NGUOINHAN_TRUCTIEP_IDS.ToListLong(',') : new List<long>();
                //model.Recipients = recipientBusiness.GetRecipientGroups(currentUser.DM_PHONGBAN_ID.GetValueOrDefault());

                model.GroupDeptTypes = dmDanhMucDataBusiness.DsByMaNhom(VanBanConstant.LOAI_COQUAN, currentUser.ID, entityVanBanDen.LOAI_COQUAN_ID.GetValueOrDefault());
                model.GroupDocTypes = dmDanhMucDataBusiness.DsByMaNhom(VanBanConstant.PHANLOAI_VANBAN, currentUser.ID, entityVanBanDen.THONGTIN_LOAI_ID.GetValueOrDefault());
                model.GroupDispatchTypes = dmDanhMucDataBusiness.DsByMaNhom(VanBanConstant.CONGVANDEN, currentUser.ID, entityVanBanDen.CONGVAN_DEN_ID.GetValueOrDefault());
                return View("CreateVanBanDen", model);
            }
            return Redirect("/Home/UnAuthor");
        }

        /// <summary>
        /// @description: lưu thông tin văn bản đến
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fc"></param>
        /// <param name="filebase"></param>
        /// <param name="filename"></param>
        /// <param name="FOLDER_ID"></param>
        /// <returns></returns>
        [ActionAudit]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveVanBanDen(HSCV_VANBANDEN entity, FormCollection fc, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            try
            {
                AssignUserInfo();

                UploadFileTool uploadFileTool = new UploadFileTool();
                hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
                wfProcessBusiness = Get<WF_PROCESSBusiness>();
                dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

                entity.SOHIEU = fc["SOHIEU"].Trim();
                entity.TRICHYEU = fc["TRICHYEU"].Trim();
                entity.SOTRANG = fc["SOTRANG"].ToIntOrZero();
                entity.LOAIVANBAN_ID = fc["LOAIVANBAN_ID"].ToIntOrZero();
                entity.DONVI_ID = fc["DONVI_ID"].ToIntOrZero();
                entity.LINHVUCVANBAN_ID = fc["LINHVUCVANBAN_ID"].ToIntOrZero();
                entity.DOKHAN_ID = fc["DOKHAN_ID"].ToIntOrZero();
                entity.NGAY_HIEULUC = fc["NGAY_HIEULUC"].ToDateTime();
                entity.NGAYHET_HIEULUC = fc["NGAYHET_HIEULUC"].ToDateTime();
                entity.NGAY_VANBAN = fc["NGAY_VANBAN"].ToDateTime();
                entity.NGAY_BANHANH = fc["NGAY_BANHANH"].ToDateTime();
                entity.NGUOIKY = !string.IsNullOrEmpty(fc["NGUOIKY"]) ? fc["NGUOIKY"].Trim() : string.Empty;
                entity.CHUCVU = !string.IsNullOrEmpty(fc["CHUCVU"]) ? fc["CHUCVU"].Trim() : string.Empty;
                entity.NOIDUNG = !string.IsNullOrEmpty(fc["NOIDUNGVANBAN"]) ? fc["NOIDUNGVANBAN"].Trim() : string.Empty;
                entity.DOMAT_ID = fc["DOMAT_ID"].ToIntOrZero();
                entity.SOVANBANDEN_ID = fc["SOVANBANDEN_ID"].ToIntOrZero();
                entity.SODITHEOSO = fc["SODITHEOSO"].Trim();
                entity.NGAYCONGTAC = fc["NGAYCONGTAC"].ToDateTime();
                entity.GIO_CONGTAC = fc["GIO_CONGTAC"].ToIntOrNULL();
                entity.PHUT_CONGTAC = fc["PHUT_CONGTAC"].ToIntOrNULL();
                entity.SODITHEOSO_NUMBER = int.Parse(entity.SODITHEOSO);
                entity.MA_DANGKY = fc["MA_DANGKY"];
                entity.LOAI_COQUAN_ID = fc["LOAI_COQUAN_ID"].ToIntOrNULL();
                entity.SO_BAN = fc["SO_BAN"].ToIntOrNULL();
                entity.THONGTIN_LOAI_ID = fc["THONGTIN_LOAI_ID"].ToIntOrNULL();
                entity.TACGIA = fc["TACGIA"];
                entity.CONGVAN_DEN_ID = fc["CONGVAN_DEN_ID"].ToIntOrNULL();
                entity.THOIHAN_GIAIQUYET = fc["THOIHAN_GIAIQUYET"].ToDateTime();
                List<long> ListUser = new List<long>();
                ListUser.Add(currentUser.ID);
                if (entity.ID == 0)
                {
                    entity.NGAYTAO = DateTime.Now;
                    entity.NGUOITAO = currentUser.ID;
                    entity.NGAYSUA = DateTime.Now;
                    entity.NGUOISUA = currentUser.ID;
                    hscvVanBanDenBusiness.Save(entity);
                    #region insert elastic search
                    ElasticModel model = ElasticModel.ConvertVanBanDen(entity, ListUser);
                    ElasticSearch.insertDocument(model, model.Id.ToString(), ElasticType.VanBanDen);
                    #endregion
                    JsonResultBO processResult = wfProcessBusiness.AddFlow(entity.ID, MODULE_CONSTANT.VANBANDEN, currentUser);
                    if (processResult.Status)
                    {
                        uploadFileTool.UploadFiles(filebase, extensionOfVanBanDen.Split(',').ToList(), uploadFolder, filename, entity.ID, LOAITAILIEU.VANBANDEN, maxSizeOfVanBanDen, currentUser);
                    }
                    //cập nhật số đi theo sổ
                    dmDanhMucDataBusiness.UpdateSoVanBan(entity.SOVANBANDEN_ID.GetValueOrDefault(), entity.SODITHEOSO.ToIntOrZero());
                }
                else
                {
                    HSCV_VANBANDEN dbEntity = hscvVanBanDenBusiness.Find(entity.ID);

                    dbEntity.SOHIEU = entity.SOHIEU;
                    dbEntity.TRICHYEU = entity.TRICHYEU;
                    dbEntity.SOTRANG = entity.SOTRANG;
                    dbEntity.DONVI_ID = entity.DONVI_ID;
                    dbEntity.LOAIVANBAN_ID = entity.LOAIVANBAN_ID;
                    dbEntity.LINHVUCVANBAN_ID = entity.LINHVUCVANBAN_ID;
                    dbEntity.DOKHAN_ID = entity.DOKHAN_ID;
                    dbEntity.NGAY_HIEULUC = entity.NGAY_HIEULUC;
                    dbEntity.NGAYHET_HIEULUC = entity.NGAYHET_HIEULUC;
                    dbEntity.NGAY_VANBAN = entity.NGAY_VANBAN;
                    dbEntity.NGAY_BANHANH = entity.NGAY_BANHANH;
                    dbEntity.NGUOIKY = entity.NGUOIKY;
                    dbEntity.CHUCVU = entity.CHUCVU;
                    dbEntity.NOIDUNG = entity.NOIDUNG;
                    dbEntity.DOMAT_ID = entity.DOMAT_ID;

                    dbEntity.NGAYCONGTAC = entity.NGAYCONGTAC;
                    dbEntity.SOVANBANDEN_ID = entity.SOVANBANDEN_ID;
                    dbEntity.SODITHEOSO = entity.SODITHEOSO;
                    dbEntity.SODITHEOSO_NUMBER = entity.SODITHEOSO_NUMBER;

                    dbEntity.GIO_CONGTAC = entity.GIO_CONGTAC;
                    dbEntity.PHUT_CONGTAC = entity.PHUT_CONGTAC;

                    dbEntity.MA_DANGKY = entity.MA_DANGKY;
                    dbEntity.LOAI_COQUAN_ID = entity.LOAI_COQUAN_ID;
                    dbEntity.SO_BAN = entity.SO_BAN;
                    dbEntity.THONGTIN_LOAI_ID = entity.THONGTIN_LOAI_ID;
                    dbEntity.TACGIA = entity.TACGIA;
                    dbEntity.CONGVAN_DEN_ID = entity.CONGVAN_DEN_ID;
                    dbEntity.THOIHAN_GIAIQUYET = entity.THOIHAN_GIAIQUYET;
                    dbEntity.NGUOINHAN_TRUCTIEP_IDS = entity.NGUOINHAN_TRUCTIEP_IDS;
                    dbEntity.NGAYSUA = DateTime.Now;
                    dbEntity.NGUOISUA = currentUser.ID;
                    hscvVanBanDenBusiness.Save(dbEntity);

                    uploadFileTool.UploadFiles(filebase, extensionOfVanBanDen.Split(',').ToList(), uploadFolder, filename, dbEntity.ID, LOAITAILIEU.VANBANDEN, maxSizeOfVanBanDen, currentUser);

                    #region update elastic
                    ElasticModel model = ElasticModel.ConvertVanBanDen(dbEntity, ListUser);
                    ElasticSearch.updateDocument(model, model.Id.ToString(), ElasticType.VanBanDen);
                    #endregion
                }
                return RedirectToAction("DetailVanBanDen", new { id = entity.ID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// @author: duynn
        /// @since: 06/08/2018
        /// @description: chi tiết văn bản đến
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult DetailVanBanDen(long id, int type = VANBANDEN_CONSTANT.CHUA_XULY)
        {
            AssignUserInfo();
            wfItemUserProcess = Get<WF_ITEM_USER_PROCESSBusiness>();
            wfReviewUserBusiness = Get<WF_REVIEW_USERBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            attachmentBusiness = Get<TAILIEUDINHKEMBusiness>();
            var HSCVREADVANBANBusiness = Get<HSCVREADVANBANBusiness>();
            HSCV_VANBANDEN entityVanBanDen = hscvVanBanDenBusiness.Find(id);
            bool canAccess = wfItemUserProcess.CheckPermissionProcess(id, true == entityVanBanDen.IS_NOIBO ? MODULE_CONSTANT.VANBANDENNOIBO : MODULE_CONSTANT.VANBANDEN, currentUser.ID);
            bool canReview = wfReviewUserBusiness.CheckPermissionReview(id, true == entityVanBanDen.IS_NOIBO ? MODULE_CONSTANT.VANBANDENNOIBO : MODULE_CONSTANT.VANBANDEN, currentUser.ID);

            if ((!canAccess && !canReview) || entityVanBanDen == null)
            {
                return Redirect("/Home/UnAuthor");
            }

            var checkread = HSCVREADVANBANBusiness.repository.All()
                .Where(x => x.USER_ID == currentUser.ID && x.TYPE == 1 && x.VANBAN_ID == id).FirstOrDefault();
            if (checkread == null)
            {
                HSCV_READVANBAN readObj = new HSCV_READVANBAN();
                readObj.TYPE = 1;
                readObj.USER_ID = currentUser.ID;
                readObj.VANBAN_ID = id;
                HSCVREADVANBANBusiness.Save(readObj);
            }

            DetailVanBanDenViewModel model = new DetailVanBanDenViewModel();
            DM_DANHMUC_DATA entityDonViGui = dmDanhMucDataBusiness.Find(entityVanBanDen.DONVI_ID);
            DM_DANHMUC_DATA entityDoKhan = dmDanhMucDataBusiness.Find(entityVanBanDen.DOKHAN_ID);
            DM_DANHMUC_DATA entityDoUuTien = dmDanhMucDataBusiness.Find(entityVanBanDen.DOMAT_ID);
            DM_DANHMUC_DATA entityLinhVucVanBan = dmDanhMucDataBusiness.Find(entityVanBanDen.LINHVUCVANBAN_ID);
            DM_DANHMUC_DATA entityLoaiVanBan = dmDanhMucDataBusiness.Find(entityVanBanDen.LOAIVANBAN_ID);
            DM_DANHMUC_DATA entityLoaiCoQuan = dmDanhMucDataBusiness.Find(entityVanBanDen.LOAI_COQUAN_ID);
            DM_DANHMUC_DATA entityThongTinLoaiVanBan = dmDanhMucDataBusiness.Find(entityVanBanDen.THONGTIN_LOAI_ID);
            DM_DANHMUC_DATA entityCongVanDen = dmDanhMucDataBusiness.Find(entityVanBanDen.CONGVAN_DEN_ID);

            model.currentUserId = currentUser.ID;
            model.entityVanBanDen = entityVanBanDen;
            model.nameOfDonViGui = entityDonViGui != null ? entityDonViGui.TEXT : string.Empty;
            model.nameOfDoKhan = entityDoKhan != null ? entityDoKhan.TEXT : string.Empty;
            model.nameOfDoUuTien = entityDoUuTien != null ? entityDoUuTien.TEXT : string.Empty;
            model.nameOfLinhVucVanBan = entityLinhVucVanBan != null ? entityLinhVucVanBan.TEXT : string.Empty;
            model.nameOfLoaiVanBan = entityLoaiVanBan != null ? entityLoaiVanBan.TEXT : string.Empty;
            model.nameOfLoaiCoQuan = entityLoaiCoQuan != null ? entityLoaiCoQuan.TEXT : string.Empty;
            model.nameOfThongTinLoaiBan = entityThongTinLoaiVanBan != null ? entityThongTinLoaiVanBan.TEXT : string.Empty;
            model.nameOfCongVanDen = entityCongVanDen != null ? entityCongVanDen.TEXT : string.Empty;

            model.groupOfTaiLieuDinhKems = attachmentBusiness.GetDataByItemID(entityVanBanDen.ID, LOAITAILIEU.VANBANDEN);
            model.typeOfVanBanDen = type;

            bool isFinish = hscvVanBanDenBusiness.CheckIsFinish(id);
            model.canCreateCalendar = (entityVanBanDen.NGUOITAO == currentUser.ID && isFinish);
            if (isFinish == false && entityVanBanDen.NGAYCONGTAC != null)
            {
                //kiểm tra người dùng có bước xử lý hay không?
                bool hasSteps = false;
                wfProcessBusiness = Get<WF_PROCESSBusiness>();
                wfReviewUserBusiness = Get<WF_REVIEW_USERBusiness>();
                wfReviewBusiness = Get<WF_REVIEWBusiness>();
                wfStateBusiness = Get<WF_STATEBusiness>();
                wfStepBusiness = Get<WF_STEPBusiness>();
                wfStateFunctionBusiness = Get<WF_STATE_FUNCTIONBusiness>();

                WF_PROCESS process = wfProcessBusiness.repository.All()
                    .Where(x => x.ITEM_ID == id && x.ITEM_TYPE == MODULE_CONSTANT.VANBANDEN
                    && x.USER_ID == currentUser.ID).FirstOrDefault();

                WF_REVIEW reviewReject = wfReviewBusiness.repository.All()
                    .Where(x => x.ITEMID == id && x.ITEMTYPE == MODULE_CONSTANT.VANBANDEN && x.IS_REJECT == true)
                    .OrderByDescending(x => x.ID).FirstOrDefault();

                WF_FUNCTION function = null;
                bool requireReview = false;
                List<WF_STEP> nextSteps = new List<WF_STEP>();
                List<StepBackBO> stepsBack = new List<StepBackBO>();

                if (process != null)
                {
                    if (process != null || reviewReject != null)
                    {
                        WF_STATE startState = wfStateBusiness.Find(process.CURRENT_STATE);
                        if (startState != null)
                        {
                            if (startState.IS_KETTHUC != true)
                            {
                                nextSteps = wfStepBusiness.GetListNextStep(process.WF_ID.GetValueOrDefault(), startState.ID);
                                stepsBack = wfStepBusiness.GetListNextStepBack(process);
                            }

                            function = wfStateFunctionBusiness.CheckGetFunction(startState.ID, entityVanBanDen.ID, MODULE_CONSTANT.VANBANDEN);
                        }
                    }
                    else if (process.IS_PENDING == true)
                    {
                        WF_REVIEW reviewFinish = wfReviewBusiness.repository.All()
                        .Where(x => x.ITEMID == id && x.ITEMTYPE == MODULE_CONSTANT.VANBANDEN && x.IS_FINISH != true)
                        .OrderByDescending(x => x.ID).FirstOrDefault();
                        if (reviewFinish != null)
                        {
                            requireReview = wfReviewUserBusiness.CheckReviewing(reviewFinish.ID, currentUser.ID);
                        }
                    }

                    hasSteps = (nextSteps.Any() || stepsBack.Any() || function != null || requireReview == true);
                }
                model.hasSteps = hasSteps;
                if (hasSteps)
                {
                    entityVanBanDen.NGAYCONGTAC = new DateTime(entityVanBanDen.NGAYCONGTAC.Value.Year, entityVanBanDen.NGAYCONGTAC.Value.Month, entityVanBanDen.NGAYCONGTAC.Value.Day);
                    lichCongTacBusiness = Get<LICHCONGTACBusiness>();
                    model.isDuplicateCalendar = lichCongTacBusiness.repository.All()
                        .Where(x => x.LANHDAO_ID == currentUser.ID
                        && x.NGAY_CONGTAC.Equals(entityVanBanDen.NGAYCONGTAC.Value)).Count() > 0;
                }
            }

            return View(model);
        }

        /// <summary>
        /// @description: danh sách văn bản đến chưa xử lý
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult ChuaXuLy()
        {
            AssignUserInfo();
            //var ContentEmail = currentUser.HOTEN + " đã gửi bạn một";
            //string NOIDUNG = "368754623875";
            //var lstEmail = new List<string>();
            //lstEmail.Add("duynt@hinetvietnam.com");
            //EmailProvider.SendMailTemplate(currentUser, ContentEmail, NOIDUNG, lstEmail);
            ListVanBanDenViewModel model = GetListViewModelOfVanBanDen(VANBANDEN_CONSTANT.CHUA_XULY);
            return View(model);
        }

        /// <summary>
        /// @description: danh sách văn bản đến đã xử lý
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult DaXuLy()
        {
            ListVanBanDenViewModel model = GetListViewModelOfVanBanDen(VANBANDEN_CONSTANT.DA_XULY);
            return View(model);
        }

        /// <summary>
        /// @description: danh sách văn bản đến chưa xử lý
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult NoiBoChuaXuLy()
        {
            ListVanBanDenViewModel model = GetListViewModelOfVanBanDen(VANBANDEN_CONSTANT.NOIBO_CHUAXULY);
            return View(model);
        }

        /// <summary>
        /// @description: danh sách văn bản đến đã xử lý
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult NoiBoDaXuLy()
        {
            ListVanBanDenViewModel model = GetListViewModelOfVanBanDen(VANBANDEN_CONSTANT.NOIBO_DAXULY);
            return View(model);
        }

        /// <summary>
        /// @description: danh sách văn bản đến tham gia xử lý
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult ThamGiaXuLy()
        {
            ListVanBanDenViewModel model = GetListViewModelOfVanBanDen(VANBANDEN_CONSTANT.THAMGIA_XULY);
            return View(model);
        }

        /// <summary>
        /// @description: view model văn bản đến
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <param name="typeOfVanBanDen"></param>
        /// <returns></returns>
        public ListVanBanDenViewModel GetListViewModelOfVanBanDen(int typeOfVanBanDen)
        {
            AssignUserInfo();
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            HSCV_VANBANDEN_SEARCH searchModel = new HSCV_VANBANDEN_SEARCH();
            searchModel.USER_ID = currentUser.ID;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANDEN;

            ListVanBanDenViewModel model = new ListVanBanDenViewModel();
            model.userInfo = currentUser;
            model.typeOfLoaiVanBan = typeOfVanBanDen;
            model.groupOfLoaiVanBans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
            model.groupOfLinhVucVanBans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
            model.groupOfDoKhans = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOKHAN, 0);
            model.groupOfDoUuTiens = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            int tmpdept = currentUser.DeptParentID.HasValue ? currentUser.DeptParentID.Value : 0;
            model.groupSoVanBanDens = dmDanhMucDataBusiness.DsByMaNhomByDept(DMLOAI_CONSTANT.SOVANBANDEN, 0, tmpdept);
            DM_THAOTAC userFunction = currentUser.ListThaoTac.Where(o => o.MA_THAOTAC.ToUpper() == "HSCV_VANBANDEN_CREATE").FirstOrDefault();
            model.canCreate = (userFunction != null && userFunction.DM_THAOTAC_ID > 0);
            string sessionName;
            switch (typeOfVanBanDen)
            {
                case VANBANDEN_CONSTANT.CHUA_XULY:
                    sessionName = "VanBanDenChuaXuLySearchModel";
                    model.title = "Quản lý văn bản đến chưa xử lý";
                    model.groupOfVanBanDens = hscvVanBanDenBusiness.GetListInProcess(searchModel);
                    break;
                case VANBANDEN_CONSTANT.DA_XULY:
                    sessionName = "VanBanDenDaXuLySearchModel";
                    model.title = "Quản lý văn bản đến đã xử lý";
                    model.groupOfVanBanDens = hscvVanBanDenBusiness.GetListProcessed(searchModel);
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    sessionName = "VanBanDenThamGiaXuLySearchModel";
                    model.title = "Quản lý văn bản đến tham gia xử lý";
                    model.groupOfVanBanDens = hscvVanBanDenBusiness.GetListJoinProcess(searchModel);
                    break;
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    sessionName = "VanBanDenNoiBoChuaXuLySearchModel";
                    searchModel.isInternal = true;
                    searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANDENNOIBO;
                    model.title = "Quản lý văn bản đến nội bộ chưa xử lý";
                    model.groupOfVanBanDens = hscvVanBanDenBusiness.GetListInProcess(searchModel);
                    break;
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    searchModel.isInternal = true;
                    searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANDENNOIBO;
                    sessionName = "VanBanDenNoiBoDaXuLySearchModel";
                    model.title = "Quản lý văn bản đến nội bộ đã xử lý";
                    model.groupOfVanBanDens = hscvVanBanDenBusiness.GetListProcessed(searchModel);
                    break;
                default:
                    sessionName = string.Empty;
                    break;
            }
            SessionManager.SetValue(sessionName, searchModel);
            return model;
        }

        /// <summary>
        /// @description: dữ liệu văn bản đến từng trang
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="sortQuery"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ActionAudit]
        public JsonResult GetData(int pageIndex, string sortQuery, int pageSize, int type)
        {
            AssignUserInfo();
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            string sessionName;
            HSCV_VANBANDEN_SEARCH searchModel = new HSCV_VANBANDEN_SEARCH();
            switch (type)
            {
                case VANBANDEN_CONSTANT.CHUA_XULY:
                    sessionName = "VanBanDenChuaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.DA_XULY:
                    sessionName = "VanBanDenDaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    sessionName = "VanBanDenThamGiaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    searchModel.isInternal = true;
                    searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANDENNOIBO;
                    sessionName = "VanBanDenNoiBoChuaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    searchModel.isInternal = true;
                    searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANDENNOIBO;
                    sessionName = "VanBanDenNoiBoDaXuLySearchModel";
                    break;
                default:
                    sessionName = string.Empty;
                    break;
            }
            PageListResultBO<HSCV_VANBANDEN_BO> data = new PageListResultBO<HSCV_VANBANDEN_BO>();
            if (!string.IsNullOrEmpty(sessionName))
            {
                searchModel = (HSCV_VANBANDEN_SEARCH)SessionManager.GetValue(sessionName);
                if (searchModel == null)
                {
                    searchModel = new HSCV_VANBANDEN_SEARCH();
                    searchModel.USER_ID = currentUser.ID;
                    searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANDEN;
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue(sessionName, searchModel);

                switch (type)
                {
                    case VANBANDEN_CONSTANT.CHUA_XULY:
                    case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                        data = hscvVanBanDenBusiness.GetListInProcess(searchModel, pageSize, pageIndex);
                        break;
                    case VANBANDEN_CONSTANT.DA_XULY:
                    case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                        data = hscvVanBanDenBusiness.GetListProcessed(searchModel, pageSize, pageIndex);
                        break;
                    case VANBANDEN_CONSTANT.THAMGIA_XULY:
                        data = hscvVanBanDenBusiness.GetListJoinProcess(searchModel, pageSize, pageIndex);
                        break;
                    default:
                        break;
                }
            }
            return Json(data);
        }

        /// <summary>
        /// @author: duynn
        /// @since: 06/08/2018
        /// @desc: tìm kiếm văn bản đến
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [ActionAudit]
        public JsonResult SearchData(FormCollection fc)
        {
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            string sessionName;
            int type = fc["TYPE"].ToIntOrZero();
            switch (type)
            {
                case VANBANDEN_CONSTANT.CHUA_XULY:
                    sessionName = "VanBanDenChuaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.DA_XULY:
                    sessionName = "VanBanDenDaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    sessionName = "VanBanDenThamGiaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    sessionName = "VanBanDenNoiBoChuaXuLySearchModel";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    sessionName = "VanBanDenNoiBoDaXuLySearchModel";
                    break;
                default:
                    sessionName = string.Empty;
                    break;
            }
            var searchModel = SessionManager.GetValue(sessionName) as HSCV_VANBANDEN_SEARCH;
            searchModel.isInternal = (type == VANBANDEN_CONSTANT.NOIBO_CHUAXULY || type == VANBANDEN_CONSTANT.NOIBO_DAXULY);
            searchModel.ITEM_TYPE = (type == VANBANDEN_CONSTANT.NOIBO_CHUAXULY || type == VANBANDEN_CONSTANT.NOIBO_DAXULY) ? MODULE_CONSTANT.VANBANDENNOIBO : MODULE_CONSTANT.VANBANDEN;
            searchModel.SOHIEU = fc["SOHIEU"];
            searchModel.TRICHYEU = fc["TRICHYEU"];
            searchModel.DOKHAN_ID = fc["DOKHAN_ID"].ToIntOrNULL();
            searchModel.DOMAT_ID = fc["DOMAT_ID"].ToIntOrNULL();
            searchModel.LINHVUCVANBAN_ID = fc["LINHVUCVANBAN_ID"].ToIntOrNULL();
            searchModel.LOAIVANBAN_ID = fc["LOAIVANBAN_ID"].ToIntOrNULL();
            searchModel.NGUOIKY = fc["NGUOIKY"];
            searchModel.SOVANBANDEN_ID = fc["SOVANBANDEN_ID"].ToIntOrNULL();
            searchModel.NGAYBANHANH_TU = fc["BANHANHTUNGAY"].ToDateTime();
            searchModel.NGAYBANHANH_DEN = fc["BANHANHDENNGAY"].ToDateTime();
            searchModel.NGAYHIEULUC_TU = fc["HIEULUCTUNGAY"].ToDateTime();
            searchModel.NGAYHIEULUC_DEN = fc["HIEULUCDENNGAY"].ToDateTime();
            searchModel.NGAYHETHIEULUC_TU = fc["HETHIEULUCTUNGAY"].ToDateTime();
            searchModel.NGAYHETHIEULUC_DEN = fc["HETHIEULUCDENNGAY"].ToDateTime();
            searchModel.NGAYVANBAN_TU = fc["NGAYVBTUNGAY"].ToDateTime();
            searchModel.NGAYVANBAN_DEN = fc["NGAYVBDENNGAY"].ToDateTime();
            SessionManager.SetValue(sessionName, searchModel);
            PageListResultBO<HSCV_VANBANDEN_BO> data = new PageListResultBO<HSCV_VANBANDEN_BO>();
            switch (type)
            {
                case VANBANDEN_CONSTANT.CHUA_XULY:
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    data = hscvVanBanDenBusiness.GetListInProcess(searchModel, defaultPageSize, 1);
                    break;
                case VANBANDEN_CONSTANT.DA_XULY:
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    data = hscvVanBanDenBusiness.GetListProcessed(searchModel, defaultPageSize, 1);
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    data = hscvVanBanDenBusiness.GetListJoinProcess(searchModel, defaultPageSize, 1);
                    break;
                default:
                    break;
            }
            return Json(data);
        }

        /// <summary>
        /// @description: xóa văn bản đến
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        public JsonResult Delete(long id)
        {
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            HSCV_VANBANDEN entityVanBanDen = hscvVanBanDenBusiness.Find(id);
            AssignUserInfo();
            if (entityVanBanDen == null || currentUser.ID != entityVanBanDen.NGUOITAO)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không có quyền xóa văn bản trình ký này" });
            }
            attachmentBusiness = Get<TAILIEUDINHKEMBusiness>();
            FileUltilities file = new FileUltilities();
            List<TAILIEUDINHKEM> ListTaiLieu = attachmentBusiness.GetDataByItemID(id, LOAITAILIEU.VANBANDEN);
            foreach (var item in ListTaiLieu)
            {
                attachmentBusiness.repository.Delete(item.TAILIEU_ID);
                file.RemoveFile(uploadFolder + item.DUONGDAN_FILE);
                if (!string.IsNullOrEmpty(item.PDF_VERSION))
                {
                    file.RemoveFile(uploadFolder + item.PDF_VERSION);
                }
            }
            storeFolderBusiness = Get<THUMUC_LUUTRUBusiness>();
            THUMUC_LUUTRU ThuMuc = storeFolderBusiness.GetDataByNam(id.ToString(), ThuMucLuuTruConstant.DefaultVanban);
            if (ThuMuc != null)
            {
                ThuMuc.IS_DELETE = true;
                storeFolderBusiness.Save(ThuMuc);
            }
            attachmentBusiness.Save();
            hscvVanBanDenBusiness.repository.Delete(id);
            #region xóa văn bản đến trong elastic

            ElasticSearch.deleteDocument(id.ToString(), ElasticType.VanBanDen);
            #endregion 
            return Json(new { Type = "SUCCESS", Message = "Xóa văn bản đến thành công" });
        }

        /// <summary>
        /// @description: điều hướng khi quay lại từ trang chi tiết
        /// @author: duynn
        /// @since: 07/08/2018
        /// </summary>
        /// <param name="typeOfVanBanDen"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionAudit]
        public ActionResult NavigateBackToList(int typeOfVanBanDen = VANBANDEN_CONSTANT.CHUA_XULY)
        {
            string actionName = "ChuaXuLy";
            switch (typeOfVanBanDen)
            {
                case VANBANDEN_CONSTANT.DA_XULY:
                    actionName = "DaXuLy";
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    actionName = "ThamGiaXuLy";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    actionName = "NoiBoChuaXuLy";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    actionName = "NoiBoDaXuLy";
                    break;
                default:
                    break;
            }
            return RedirectToAction(actionName);
        }


        /// <summary>
        /// @author: duynn
        /// @description: kết xuất excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportExcel(int type = VANBANDEN_CONSTANT.CHUA_XULY)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            properties.Add("STT", "STT");
            properties.Add("SOHIEU", "Số ký hiệu");

            properties.Add("TRICHYEU", "Trích yếu");
            properties.Add("SOBAN", "Số bản");
            properties.Add("NGAY_VANBAN", "Ngày VB");
            properties.Add("TEN_DONVI", "Tác giả");
            properties.Add("NGAYTAO", "Ngày đăng ký");
            properties.Add("CHUTRI", "Chủ trì + phối hợp");
            properties.Add("SOTRANG", "Số trang");
            properties.Add("LUU_HOSO", "Lưu hồ sơ");
            properties.Add("GHICHU", "Ghi chú");

            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            string actionName = "ChuaXuLy";
            string fileName = "BÁO CÁO THỐNG KÊ VĂN BẢN ĐẾN CHƯA XỬ LÝ";
            List<HSCV_VANBANDEN_BO> dataExport = new List<HSCV_VANBANDEN_BO>();
            string sessionName;
            switch (type)
            {
                case VANBANDEN_CONSTANT.CHUA_XULY:
                    actionName = "ChuaXuLy";
                    sessionName = "VanBanDenChuaXuLySearchModel";
                    fileName = "Báo cáo thống kê văn bản đến chưa xử lý";
                    break;
                case VANBANDEN_CONSTANT.DA_XULY:
                    actionName = "DaXuLy";
                    sessionName = "VanBanDenDaXuLySearchModel";
                    fileName = "Văn bản đến đã xử lý";
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    actionName = "ThamGiaXuLy";
                    sessionName = "VanBanDenThamGiaXuLySearchModel";
                    fileName = "Văn bản đến tham gia xử lý";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    actionName = "NoiBoChuaXuLy";
                    sessionName = "VanBanDenNoiBoChuaXuLySearchModel";
                    fileName = "Văn bản đến nội bộ chưa xử lý";
                    break;
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    actionName = "NoiBoDaXuLy";
                    sessionName = "VanBanDenNoiBoDaXuLySearchModel";
                    fileName = "Văn bản đến nội bộ đã xử lý";
                    break;
                default:
                    sessionName = string.Empty;
                    break;
            }
            var searchModel = SessionManager.GetValue(sessionName) as HSCV_VANBANDEN_SEARCH;

            PageListResultBO<HSCV_VANBANDEN_BO> data = new PageListResultBO<HSCV_VANBANDEN_BO>();
            switch (type)
            {
                case VANBANDEN_CONSTANT.CHUA_XULY:
                case VANBANDEN_CONSTANT.NOIBO_CHUAXULY:
                    data = hscvVanBanDenBusiness.GetListInProcess(searchModel, defaultPageSize, -1);
                    break;
                case VANBANDEN_CONSTANT.DA_XULY:
                case VANBANDEN_CONSTANT.NOIBO_DAXULY:
                    data = hscvVanBanDenBusiness.GetListProcessed(searchModel, defaultPageSize, -1);
                    break;
                case VANBANDEN_CONSTANT.THAMGIA_XULY:
                    data = hscvVanBanDenBusiness.GetListJoinProcess(searchModel, defaultPageSize, -1);
                    break;
                default:
                    break;
            }

            EPPlusSupplier<HSCV_VANBANDEN_BO> supplier = new EPPlusSupplier<HSCV_VANBANDEN_BO>();
            supplier.properties = properties;
            supplier.startColumn = 1;
            supplier.startRow = 5;
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
            sheet.DefaultColWidth = 20;
            sheet.Column(1).Width = 10;
            sheet.Column(3).Width = 50;
            sheet.Column(4).Width = 20;
            sheet.Column(5).Width = 30;
            sheet.Column(6).Width = 35;
            sheet.Cells.Style.Font.Name = "Times New Roman";

            ExcelRange titleRange = sheet.SelectedRange["A1:K1"];
            titleRange.Value = null;
            titleRange.Merge = true;
            titleRange.Value = "Công văn đến";
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

            return sheet;
        }
    }
}