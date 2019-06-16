using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommonHelper;
using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Model.Entities;
using Web.Custom;
using Web.FwCore;
using Business.CommonModel.WFSTREAM;
using Web.Areas.WFSTREAMArea.Models;
using Web.Common.Elastic;
using System.Web;
using Web.Common;
using System.Web.Configuration;
using SMSDAL;
namespace Web.Areas.WFSTREAMArea.Controllers
{
    public class WFSTREAMController : BaseController
    {
        #region khaibao
        WF_STREAMBusiness WF_STREAMBusiness;
        WF_REVIEWBusiness WF_REVIEWBusiness;
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private const string targetScreen = "DetailSignDocScreen";
        #endregion
        private string SERVERADDRESS = WebConfigurationManager.AppSettings["SERVERADDRESS"];
        private string VbTrinhKyExtension = WebConfigurationManager.AppSettings["VbDenExtension"];
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private int VbTrinhKySize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        public ActionResult Index()
        {
            var model = new IndexVM();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var searchmodel = new WF_STREAM_SEARCHBO();
            SessionManager.SetValue("wfstreamSearchModel", null);
            model.LstLuong = WF_STREAMBusiness.GetDaTaByPage(null);
            return View(model);
        }
        public PartialViewResult GetAction(long idItem, string itemType)
        {
            AssignUserInfo();
            var model = new ActionFlowBO();
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var WF_STEPBusiness = Get<WF_STEPBusiness>();
            var WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
            var WF_FUNCTION_DONEBusiness = Get<WF_FUNCTION_DONEBusiness>();
            model.Process = WF_PROCESSBusiness.GetProcess(idItem, itemType) ?? new WF_PROCESS();
            // Kiểm tra xem có bản ghi nào bị reject hay không
            WF_REVIEWBusiness = Get<WF_REVIEWBusiness>();
            WF_REVIEW ReviewRejectObj = WF_REVIEWBusiness.repository.All().Where(x =>
                    x.ITEMID == model.Process.ITEM_ID && x.ITEMTYPE == model.Process.ITEM_TYPE).OrderByDescending(x => x.ID)
                .FirstOrDefault();
            model.ReviewObj = ReviewRejectObj;
            // kiểm tra xem bước xử lý có tồn tại không và kiểm tra xem có đang ở trạng thái pending 
            // nghĩa là đang gửi review hay không
            if (model.Process != null && (model.Process.IS_PENDING != true || (ReviewRejectObj != null && ReviewRejectObj.IS_REJECT == true)))
            {
                model.StartState = WF_STATEBusiness.Find(model.Process.CURRENT_STATE);
                //check xem người đang xử lý chính có phải người dùng hiện tại hay không
                if (model.Process.USER_ID == currentUser.ID)
                {
                    // Kiểm tra xem trạng thái hiện tại có phải trạng thái kết thúc hay chưa
                    if (model.StartState.IS_KETTHUC != true)
                    {
                        model.LstStep = WF_STEPBusiness.GetListNextStep(model.Process.WF_ID.Value, model.StartState.ID);
                        model.LstStepBack = WF_STEPBusiness.GetListNextStepBack(model.Process);
                    }

                    if (model.StartState != null)
                    {
                        model.Function = WF_STATE_FUNCTIONBusiness.CheckGetFunction(model.StartState.ID, idItem, model.Process.ITEM_TYPE);
                    }
                }
            }
            else if (model.Process.IS_PENDING == true)
            {
                // Kiểm tra xem có bản ghi review nào ở trạng thái chưa kết thúc hay không
                WF_REVIEW ReviewObj = WF_REVIEWBusiness.repository.All().Where(x =>
                        x.ITEMID == model.Process.ITEM_ID && x.ITEMTYPE == model.Process.ITEM_TYPE &&
                        x.IS_FINISH != true)
                    .FirstOrDefault();
                if (ReviewObj != null)
                {
                    var WfUserReview = Get<WF_REVIEW_USERBusiness>();
                    // cập nhật trạng thái Required review - nếu trạng thái này = true thì yêu cầu review
                    model.REQUIRED_REVIEW = WfUserReview.CheckReviewing(ReviewObj.ID, currentUser.ID);
                }
            }
            return PartialView("_ActionFlowPartial", model);
        }

        public PartialViewResult GetHistory(long idItem, string itemType)
        {
            AssignUserInfo();
            var model = new HistoryFlowBO();
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var WF_STEPBusiness = Get<WF_STEPBusiness>();
            var WF_LOGBusiness = Get<WF_LOGBusiness>();
            model.Process = WF_PROCESSBusiness.GetProcess(idItem, itemType);
            if (model.Process != null)
            {
                model.lstLog = WF_LOGBusiness.GetLstLog(idItem, itemType);
            }
            return PartialView("_historyPartial", model);
        }
        public PartialViewResult flow(int idprocess, int stepid, bool IsBack = false, long LogId = 0)
        {
            AssignUserInfo();
            var model = new FlowBO();
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var WF_STEPBusiness = Get<WF_STEPBusiness>();
            var WF_LOGBusiness = Get<WF_LOGBusiness>();
            model.Process = WF_PROCESSBusiness.Find(idprocess);
            model.Step = WF_STEPBusiness.GetDaTaByID(stepid);
            if (IsBack == true)
            {
                model.IsBack = true;
                model.Step.NAME = "Trả về";
                var log = WF_LOGBusiness.GetDataByID(LogId);
                model.Log = log;
            }
            else
            {
                if (model.Step.REQUIRED_REVIEW != true || model.Process.IS_PENDING == false)
                {
                    if (!(model.Step.ConfigStep != null && model.Step.ConfigStep.IS_BACK_USER == true))
                    {
                        if (model.Step.NguoiXuLyChinh != null)
                        {
                            model.dsNgNhanChinh = DM_NGUOIDUNGBusiness.GetNguoiDungFlow(model.Step.NguoiXuLyChinh, currentUser.ID);
                        }

                        if (model.Step.NguoiThamGiaXuLy != null)
                        {
                            model.dsNgThamGia = DM_NGUOIDUNGBusiness.GetNguoiDungFlow(model.Step.NguoiThamGiaXuLy, currentUser.ID);
                        }
                    }
                }
                else
                {
                    model.dsNgNhanChinh = DM_NGUOIDUNGBusiness.GetNguoiDungReview(currentUser.ID);
                    return PartialView("_reviewPartial", model);
                }
            }


            return PartialView("_flowshowPartial", model);
        }
        #region function liên quan tới review văn bản trình ký
        /// <summary>
        /// Phản hồi kết quả review
        /// </summary>
        /// <param name="phanHoiVanBan"></param>
        /// <param name="pheDuyetVanBan"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public ActionResult SaveUserReview(FormCollection coll, IEnumerable<HttpPostedFileBase> filebase, string[] filename, string[] FOLDER_ID)
        {
            AssignUserInfo();
            string phanHoiVanBan = coll["PHANHOIVANBAN"];
            int pheDuyetVanBan = coll["PHEDUYETVANBAN"].ToIntOrZero();
            long itemId = coll["itemId"].ToLongOrZero();
            string itemType = coll["itemType"];

            var result = new JsonResultBO(true);
            WF_REVIEW_USERBusiness WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();
            WF_REVIEWBusiness WF_REVIEWBusiness = Get<WF_REVIEWBusiness>();
            WF_REVIEW ReviewObj = WF_REVIEWBusiness.repository.All()
                .Where(x => x.ITEMID == itemId && x.ITEMTYPE == itemType && x.IS_FINISH != true).FirstOrDefault();
            if (ReviewObj != null)
            {
                // kiểm tra xem nội dung phản hồi có hay không và người dùng hiện tại có quyền review với văn bản này ko và phải là đợt review mới nhất
                WF_USER_REVIEW userReview = WF_REVIEW_USERBusiness.repository.All().Where(x =>
                    x.ITEMTYPE == itemType && x.ITEMID == itemId && x.USER_ID == currentUser.ID &&
                    x.REVIEW_ID == ReviewObj.ID).FirstOrDefault();
                if (!string.IsNullOrEmpty(phanHoiVanBan) && userReview != null)
                {
                    // luu log xu ly
                    var WF_LogBusiness = Get<WF_LOGBusiness>();
                    // Lấy log cuối cùng
                    WF_LOG lastLog = WF_LogBusiness.repository.All()
                        .Where(x => x.ITEM_ID == ReviewObj.ITEMID && x.ITEM_TYPE == ReviewObj.ITEMTYPE)
                        .OrderByDescending(x => x.ID).FirstOrDefault();
                    WF_LOG log = new WF_LOG();
                    if (lastLog != null)
                    {
                        log.MESSAGE = phanHoiVanBan;
                        log.NGUOIXULY_ID = currentUser.ID;
                        log.create_at = DateTime.Now;
                        log.create_by = currentUser.ID;
                        log.ITEM_TYPE = ReviewObj.ITEMTYPE;
                        log.ITEM_ID = ReviewObj.ITEMID;
                        log.NGUONHAN_ID = lastLog.NGUOIXULY_ID;
                        log.WF_ID = lastLog.WF_ID;
                        log.STEP_ID = lastLog.STEP_ID;
                    }
                    // end
                    userReview.COMMENT = phanHoiVanBan;
                    userReview.REVIEW_AT = DateTime.Now;
                    if (pheDuyetVanBan == 1)
                    {
                        userReview.IS_APPROVE = true;
                        log.MESSAGE += "- Đồng ý";
                    }
                    else
                    {
                        userReview.IS_APPROVE = false;
                        // cập nhật lại trạng thái reject, đã reject rồi sẽ ko cập nhật trạng thái pending của process vì chưa xử lý xong
                        ReviewObj.IS_REJECT = true;
                        log.MESSAGE += "- Không đồng ý";
                    }
                    if (lastLog != null)
                    {
                        WF_LogBusiness.Save(log);
                        UploadFileTool tool = new UploadFileTool();
                        tool.UploadCustomFileVer3(filebase, true, VbTrinhKyExtension, URL_FOLDER, VbTrinhKySize, FOLDER_ID, filename, log.ID, LOAITAILIEU.REVIEWVANBAN, "Review Văn bản trình ký", currentUser);
                    }
                    WF_REVIEW_USERBusiness.Save(userReview);
                    WF_REVIEWBusiness.Save(ReviewObj);
                    #region kiểm tra xem đã review hết chưa thì cập nhật lại trạng thái của bảng review là is finish
                    if (WF_REVIEW_USERBusiness.CheckFinishReview(ReviewObj.ID))
                    {
                        // nếu không còn bản ghi nào để review thì cập nhật trạng thái is finish
                        ReviewObj.IS_FINISH = true;
                        WF_REVIEWBusiness.Save(ReviewObj);
                        // nếu đã cập nhật hêt rồi mà ko có bản ghi nào bị từ chối thì update lại trạng thái pending của process
                        if (ReviewObj.IS_REJECT != true)
                        {
                            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
                            var ProcessObj = WF_PROCESSBusiness.repository.All()
                                .Where(x => x.ITEM_ID == itemId && x.ITEM_TYPE == itemType).OrderByDescending(x => x.ID)
                                .FirstOrDefault();
                            if (ProcessObj != null)
                            {
                                ProcessObj.IS_PENDING = false;
                                WF_PROCESSBusiness.Save(ProcessObj);
                            }
                        }
                    }
                    #endregion
                    #region gửi notification
                    var SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                    SYS_TINNHAN noti = new SYS_TINNHAN();
                    noti.FROM_USERNAME = currentUser.HOTEN;
                    noti.FROM_USER_ID = currentUser.ID;
                    noti.NGAYTAO = DateTime.Now;
                    noti.NOIDUNG = currentUser.HOTEN + " đã trả lời Review một văn bản trình ký";
                    if (ReviewObj.ITEMTYPE == MODULE_CONSTANT.VANBANTRINHKY)
                    {
                        noti.URL = "/HSVanBanDiArea/VanBanChuaXuLy/DetailVanBan/" + itemId.ToString();
                        noti.TIEUDE = "REVIEW VĂN BẢN TRÌNH KÝ";
                    }
                    noti.TO_USER_ID = userReview.CREATED_BY;
                    SYS_TINNHANBusiness.Save(noti, targetScreen, false, ReviewObj.ITEMID.HasValue ? ReviewObj.ITEMID.Value : 0, TargetDocType.COORDINATED);
                    #endregion
                }
                //else
                //{
                //    //result.MessageFail("Dữ liệu không hợp lệ hoặc bạn đang cố gắng hack hệ thống");
                //    //return Redirect("/HSVanBanDiArea/VanBanChuaXuLy/DetailVanBan/" + itemId.ToString());
                //}
            }
            //else
            //{
            //    result.MessageFail("Dữ liệu không hợp lệ hoặc bạn đang cố gắng hack hệ thống");
            //}
            return Redirect("/HSVanBanDiArea/VanBanChuaXuLy/DetailVanBan/" + itemId.ToString());
            //return Json(result);
        }
        /// <summary>
        /// Save Review văn bản
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="stepID"></param>
        /// <param name="joinUser"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public JsonResult SaveReview(long processID, int stepID, List<long> joinUser, string message)
        {
            AssignUserInfo();
            var result = new JsonResultBO(true);
            if (joinUser.Count > 0 && !joinUser.Contains(currentUser.ID))
            {
                var WF_STEPBusiness = Get<WF_STEPBusiness>();
                var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
                WF_REVIEWBusiness WF_REVIEWBusiness = Get<WF_REVIEWBusiness>();
                WF_REVIEW_USERBusiness WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();
                var SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                var step = WF_STEPBusiness.GetDaTaByID(stepID);
                var process = WF_PROCESSBusiness.Find(processID);
                #region cập nhật trạng thái review 
                process.IS_PENDING = true;
                WF_PROCESSBusiness.Save(process);
                #endregion
                #region lưu yêu cầu review - bảng này sẽ dùng để đếm số lần gửi yêu cầu review
                WF_REVIEW review = new WF_REVIEW();
                review.COMMENT = message;
                review.CREATED_AT = DateTime.Now;
                review.ITEMID = process.ITEM_ID;
                review.ITEMTYPE = process.ITEM_TYPE;
                review.CREATED_BY = currentUser.ID;
                WF_REVIEWBusiness.Save(review);
                #endregion
                #region lưu yêu cầu review đối với từng người được yêu cầu
                var WF_LogBusiness = Get<WF_LOGBusiness>();
                foreach (var item in joinUser)
                {
                    WF_USER_REVIEW itemreview = new WF_USER_REVIEW();
                    itemreview.ITEMID = process.ITEM_ID;
                    itemreview.ITEMTYPE = process.ITEM_TYPE;
                    itemreview.REVIEW_ID = review.ID;
                    itemreview.USER_ID = item;
                    itemreview.CREATED_AT = DateTime.Now;
                    itemreview.CREATED_BY = currentUser.ID;
                    WF_REVIEW_USERBusiness.Save(itemreview);
                    #region luu log
                    WF_LOG log = new WF_LOG();
                    log.ITEM_ID = process.ITEM_ID;
                    log.WF_ID = process.WF_ID;
                    log.ITEM_TYPE = process.ITEM_TYPE;
                    log.MESSAGE = message;
                    log.NGUOIXULY_ID = currentUser.ID;
                    log.NGUONHAN_ID = item;
                    log.create_at = DateTime.Now;
                    log.create_by = currentUser.ID;
                    WF_LogBusiness.Save(log);
                    #endregion
                    #region gửi notification
                    SYS_TINNHAN noti = new SYS_TINNHAN();
                    noti.FROM_USERNAME = currentUser.HOTEN;
                    noti.FROM_USER_ID = currentUser.ID;
                    noti.NGAYTAO = DateTime.Now;
                    noti.NOIDUNG = currentUser.HOTEN + " đã gửi bạn review một văn bản trình ký";
                    if (process.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY)
                    {
                        noti.URL = "/HSVanBanDiArea/VanBanChuaXuLy/DetailVanBan/" + process.ITEM_ID.ToString();
                        noti.TIEUDE = "REVIEW VĂN BẢN TRÌNH KÝ";
                    }
                    noti.TO_USER_ID = item;
                    SYS_TINNHANBusiness.Save(noti, targetScreen, false,
                        process.ITEM_ID.HasValue ? process.ITEM_ID.Value : 0, TargetDocType.COORDINATED);
                    #endregion
                }
                ElasticSearch.updateListUser(process.ITEM_ID.ToString(), joinUser, ElasticType.VanBanDi);

                #endregion
            }
            else
            {
                result.MessageFail("Không thể thực hiện được thao tác này");
            }
            return Json(result);
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="stepID"></param>
        /// <param name="mainUser"></param>
        /// <param name="joinUser"></param>
        /// <param name="message"></param>
        /// <param name="IsBack"></param>
        /// <param name="LogID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveFlow(long processID, int stepID, long mainUser, List<long> joinUser, string message, bool IsBack = false, long LogID = 0)
        {
            var result = new JsonResultBO(true);
            AssignUserInfo();
            var WF_STEPBusiness = Get<WF_STEPBusiness>();
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var WF_LOGBusiness = Get<WF_LOGBusiness>();
            var step = WF_STEPBusiness.GetDaTaByID(stepID);
            //SMSDAL.SendSMSDAL sms = new SMSDAL.SendSMSDAL();
            //var LogSMSBusiness = Get<LogSMSBusiness>();
            var lstMainUser = new List<long>();
            lstMainUser.Add(mainUser);
            var process = WF_PROCESSBusiness.Find(processID);
            string NOIDUNG = currentUser.HOTEN + (process.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY ? " đã gửi bạn một văn bản trình ký" : " đã gửi bạn một văn bản đến");
            string URL = process.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY ? "/HSVanBanDiArea/HSVanBanDi/DetailVanBan/" + process.ITEM_ID.ToString() : "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + process.ITEM_ID.ToString() + "&type=" + VANBANDEN_CONSTANT.CHUA_XULY;
            string itemName = process.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY ? "VĂN BẢN TRÌNH KÝ" : "VĂN BẢN ĐẾN";
            if (IsBack != true)
            {
                //Kiểm tra xem đã thực hiện function chưa?                
                var WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
                bool CanNext = false;
                if (process.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY)
                {
                    HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                    HSCV_VANBANDI VanBan = HSCV_VANBANDIBusiness.Find(process.ITEM_ID);
                    if (VanBan != null)
                    {
                        CanNext = VanBan.HAS_SIGNED.HasValue ? VanBan.HAS_SIGNED.Value : true;
                    }
                }
                var function = WF_STATE_FUNCTIONBusiness.CheckFunctionNextState(process.CURRENT_STATE.GetValueOrDefault(), process.ITEM_ID.GetValueOrDefault(), process.ITEM_TYPE);
                if (!function && CanNext)
                {
                    result.MessageFail("Bạn chưa thực hiện hành động của trạng thái");
                    return Json(result);
                }
                //Luồng chuyển đi
                result = WF_PROCESSBusiness.SaveStepAction(processID, stepID, mainUser, joinUser, message, currentUser.ID);
                #region gửi notification
                var SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();

                #region gửi email cho người xử lý chính
                List<string> lstEmail = new List<string>();
                var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                lstEmail = DM_NGUOIDUNGBusiness.repository.All().Where(x => lstMainUser.Contains(x.ID)).Select(x => x.EMAIL).ToList();
                if (lstEmail != null)
                {
                    var ContentEmail = currentUser.HOTEN + " đã gửi bạn một <a href='" + SERVERADDRESS + URL + "'>" + itemName + "</a>";
                    EmailProvider.SendMailTemplate(currentUser, ContentEmail, NOIDUNG, lstEmail);
                }
                #endregion
                #region gửi sms cho người xử lý
                //var LstUserNhan = DM_NGUOIDUNGBusiness.repository.All().Where(x => lstMainUser.Contains(x.ID)).ToList();
                //if (LstUserNhan != null)
                //{
                //    var ContentSMS = currentUser.HOTEN + " đã gửi bạn một " + itemName;
                //    ContentSMS = sms.locDau(ContentSMS);
                //    var DoDaiSMS = ContentSMS.Length;
                //    string[] noiDung = new string[1];
                //    noiDung[0] = ContentSMS;
                //    foreach (var user in LstUserNhan)
                //    {
                //        if (!string.IsNullOrEmpty(user.DIENTHOAI))
                //        {
                //            string resultsend = sms.guiTinNhan(user.DIENTHOAI, "177403", noiDung);
                //            LOGSMS SmsObject = new LOGSMS();
                //            SmsObject.SODIENTHOAINHAN = user.DIENTHOAI;
                //            SmsObject.NOIDUNG = ContentSMS;
                //            SmsObject.SOKYTU = DoDaiSMS;
                //            SmsObject.KETQUA = resultsend;
                //            SmsObject.CREATED_AT = DateTime.Now;
                //            SmsObject.CREATED_BY = currentUser.ID;
                //            SmsObject.HOTENNGUOIGUI = currentUser.HOTEN;
                //            SmsObject.ITEMTYPE = process.ITEM_TYPE;
                //            SmsObject.ITEMID = process.ITEM_ID;
                //            SmsObject.HOTENNGUOINHAN = user.HOTEN;
                //            LogSMSBusiness.Save(SmsObject);
                //        }
                //    }
                //}
                #endregion
                SYS_TINNHANBusiness.sendMessageMultipleUsers(lstMainUser, currentUser, itemName, NOIDUNG, URL, targetScreen, false, processID, TargetDocType.COORDINATED);
                if (joinUser != null)
                {
                    SYS_TINNHANBusiness.sendMessageMultipleUsers(joinUser, currentUser, itemName, NOIDUNG, URL, targetScreen, false, processID, TargetDocType.COORDINATED);
                }
                var lstTmpUserId = lstMainUser;
                if (joinUser != null)
                {
                    lstTmpUserId.AddRange(joinUser);
                }

                if (process.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY)
                {
                    ElasticSearch.updateListUser(process.ITEM_ID.ToString(), lstTmpUserId, ElasticType.VanBanDi);
                }
                else
                {
                    ElasticSearch.updateListUser(process.ITEM_ID.ToString(), lstTmpUserId, ElasticType.VanBanDen);
                }

                #endregion

            }
            else
            {
                result = WF_PROCESSBusiness.StepDenie(processID, stepID, message, LogID);
                var log = WF_LOGBusiness.GetDataByID(LogID);
                lstMainUser.Add(log.NGUOIXULY_ID.GetValueOrDefault());
                #region gửi notification
                var SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                SYS_TINNHANBusiness.sendMessageMultipleUsers(lstMainUser, currentUser, itemName, NOIDUNG, URL, targetScreen, false, processID, TargetDocType.COORDINATED);
                #endregion
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var searchModel = SessionManager.GetValue("wfstreamSearchModel") as WF_STREAM_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new WF_STREAM_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("wfstreamSearchModel", searchModel);
            }
            var data = WF_STREAMBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        public PartialViewResult Create()
        {
            AssignUserInfo();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var LstLevel = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCAPPHONGBAN", currentUser.ID);
            return PartialView("_CreatePartial", LstLevel);
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            AssignUserInfo();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new WF_STREAM();
                myobj.create_at = DateTime.Now;
                myobj.create_by = currentUser.ID;
                myobj.WF_NAME = collection["WF_NAME"].ToString();
                myobj.WF_DESCRIPTION = collection["WF_DESCRIPTION"].ToString();
                myobj.LEVEL_ID = collection["CATEGORY"].ToIntOrZero();
                WF_STREAMBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không thêm mới được";
            }
            return Json(result);
        }

        public PartialViewResult Edit(long id)
        {
            AssignUserInfo();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var myModel = new EditVM();
            myModel.objModel = WF_STREAMBusiness.repository.Find(id);
            myModel.LstLevel = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCAPPHONGBAN", currentUser.ID, myModel.objModel.LEVEL_ID.HasValue ? myModel.objModel.LEVEL_ID.Value : 0);
            return PartialView("_EditPartial", myModel);
        }

        public PartialViewResult Detail(int id)
        {
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var myModel = WF_STREAMBusiness.GetDaTaByID(id);
            return PartialView("_DetailPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            AssignUserInfo();
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = WF_STREAMBusiness.Find(id);
                myobj.edit_at = DateTime.Now;
                myobj.edit_by = currentUser.ID;
                myobj.WF_NAME = collection["WF_NAME"].ToString();
                myobj.WF_DESCRIPTION = collection["WF_DESCRIPTION"].ToString();
                myobj.LEVEL_ID = collection["CATEGORY"].ToIntOrZero();
                WF_STREAMBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            var searchModel = SessionManager.GetValue("wfstreamSearchModel") as WF_STREAM_SEARCHBO;
            if (searchModel == null)
            {
                searchModel = new WF_STREAM_SEARCHBO();
                searchModel.pageSize = 20;
            }
            searchModel.QR_WF_NAME = form["QR_WF_NAME"].ToString();
            searchModel.QR_WF_DESCRIPTION = form["QR_WF_DESCRIPTION"].ToString();
            SessionManager.SetValue("wfstreamSearchModel", searchModel);
            var data = WF_STREAMBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);

        }
        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            WF_STREAMBusiness = Get<WF_STREAMBusiness>();
            WF_STREAMBusiness.repository.Delete(id);
            WF_STREAMBusiness.Save();
            return Json(result);
        }
        //public PartialViewResult KeThuaFlow(int id)
        //{
        //    var model = new KeThuaVM();
        //    model.ToaNhaID = id;
        //    WF_STREAMBusiness = Get<WF_STREAMBusiness>();
        //    model.LstFlow = WF_STREAMBusiness.GetListKeThua(id);
        //    return PartialView("_KeThuaFlowPartial", model);
        //}
        //[HttpPost]
        //public JsonResult SaveKeThua(int idToaNha, int idFlow)
        //{
        //    AssignUserInfo();
        //    var result = new JsonResultBO(true);
        //    var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
        //    //var flow = WF_STREAMBusiness.Find(idFlow);
        //    //WF_STREAM newFlow = new WF_STREAM();
        //    //newFlow.TOANHA_ID = idToaNha;
        //    //newFlow.create_at = DateTime.Now;
        //    //newFlow.create_by = currentUser.ID;
        //    //newFlow.WF_DESCRIPTION = flow.WF_DESCRIPTION;
        //    //newFlow.WF_NAME = flow.WF_NAME;
        //    //WF_STREAMBusiness.Save(newFlow);
        //    result = WF_STREAMBusiness.CloneFlow(idToaNha, idFlow);
        //    return Json(result);
        //}
        /// <summary>
        /// Thực hiện ký duyệt văn bản
        /// </summary>
        /// <param name="id">id của văn bản cần ký duyệt</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveKyDuyetVanBan(long id)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDI VanBan = HSCV_VANBANDIBusiness.Find(id);
            if (VanBan == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy văn bản trình ký cần ký duyệt" });
            }
            //Nếu như văn bản cần ký duyệt
            if (true == VanBan.HAS_SIGNED)
            {
                try
                {
                    UploadFileTool tool = new UploadFileTool();
                    FileUltilities file = new FileUltilities();
                    List<string> ListExtension = tool.GetWordExtension();
                    List<string> ListPdfExtension = tool.GetPdfExtension();
                    var TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                    var ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(id, LOAITAILIEU.VANBAN);
                    var ListPdf = ListTaiLieu.Where(x => !string.IsNullOrEmpty(x.DINHDANG_FILE) && ListPdfExtension.Contains(x.DINHDANG_FILE.ToLower())).ToList();
                    var ListWord = ListTaiLieu.Where(x => !string.IsNullOrEmpty(x.DINHDANG_FILE) && ListExtension.Contains(x.DINHDANG_FILE.ToLower())).ToList();
                    var personalSign = "";
                    if (currentUser.signpath != null && currentUser.signpath != "")
                    {
                        personalSign = Server.MapPath("~/" + currentUser.signpath);
                    }

                    if (ListWord.Count > 0)
                    {
                        FileUltilities.CreateListWatermark("APPROVED", ListWord, "", personalSign);
                    }

                    if (ListPdf.Count > 0)
                    {
                        FileUltilities.CreateWaterMarkPdf(ListPdf, "APPROVED");
                    }
                    
                }
                catch (Exception ex)
                {

                }
            }
            #region cập nhật wf function done
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var process = WF_PROCESSBusiness.GetProcess(VanBan.ID, MODULE_CONSTANT.VANBANTRINHKY);
            var WF_STATE_FUNCTIONBusiness = Get<WF_STATE_FUNCTIONBusiness>();
            var function = WF_STATE_FUNCTIONBusiness.GetStateFunction((int)process.CURRENT_STATE);
            var WF_FUNCTION_DONEBusiness = Get<WF_FUNCTION_DONEBusiness>();
            var WF_LOGBusiness = Get<WF_LOGBusiness>();
            var functionDone = new WF_FUNCTION_DONE();
            functionDone.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            functionDone.ITEM_ID = VanBan.ID;
            functionDone.STATE = process.CURRENT_STATE;
            functionDone.FUNCTION_STATE = function.ID;
            functionDone.create_at = DateTime.Now;
            AssignUserInfo();
            functionDone.create_by = currentUser.ID;
            WF_FUNCTION_DONEBusiness.Save(functionDone);
            var log = new WF_LOG();
            log.ITEM_ID = VanBan.ID;
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
            #endregion
            return Json(new { Type = "SUCCESS", Message = "Ký duyệt văn bản thành công" });
        }
    }
}

