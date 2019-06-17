using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.WFSTREAM;
using CommonHelper;
using NPOI.SS.Formula.Functions;

namespace Business.Business
{
    public class WF_PROCESSBusiness : BaseBusiness<WF_PROCESS>
    {
        public WF_PROCESSBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(WF_PROCESS item)
        {
            try
            {
                var type = item.GetType();
                var property = type.GetProperty("ID");
                if (property != null)
                {
                    var getvalue = property.GetValue(item).ToString();
                    if (getvalue == "0")
                    {
                        item.UPDATED_AT = DateTime.Now;
                        this.repository.Insert(item);
                    }
                    else
                    {
                        item.UPDATED_AT = DateTime.Now;
                        this.repository.Update(item);
                    }
                }
                else
                {
                    throw new Exception("Không tồn tại ID");
                }

                this.repository.Save();
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
                throw new Exception(ex.Message);
            }
        }
        public bool CheckPermissionProcess(long ItemId, string itemType, long userId)
        {
            var result = this.context.WF_PROCESS.Where(x => x.ITEM_ID == ItemId && x.ITEM_TYPE == itemType && x.USER_ID == userId).FirstOrDefault();
            return result != null;
        }
        public JsonResultBO AddFlow(long ItemId, string itemType, UserInfoBO userId, int customState = 0)
        {
            var result = new JsonResultBO(true);
            //Lấy thông tin module của loại đối tượng
            var module = this.context.WF_MODULE.Where(x => x.MODULE_CODE.Equals(itemType)).FirstOrDefault();
            if (module == null || module.WF_STREAM_ID == null)
            {
                result.MessageFail("Không tìm thấy luồng xử lý");
                return result;
            }
            //Lấy thông tin luồng xử lý
            var DeptObj = this.context.CCTC_THANHPHAN.Find(userId.DM_PHONGBAN_ID);
            if (DeptObj == null)
            {
                result.MessageFail("Không tìm thấy luồng xử lý do chưa set đúng phòng ban");
                return result;
            }

            var LstWFIds = module.WF_STREAM_ID.ToListInt(',');

            var flow = this.context.WF_STREAM.Where(x => x.LEVEL_ID == DeptObj.CATEGORY && LstWFIds.Contains(x.ID)).FirstOrDefault();
            if (flow == null)
            {
                result.MessageFail("Không tìm thấy luồng xử lý");
                return result;

            }
            //Lấy danh sách bước
            var lstStep = this.context.WF_STATE.Where(x => x.WF_ID == flow.ID).OrderBy(x => x.ID).ToList();
            if (lstStep == null && !lstStep.Any())
            {
                result.MessageFail("Luồng xử lý chưa được cấu hình");
                return result;

            }
            var stateID = GetState(flow.ID, userId);

            /*
             * @author: duynn
             * @description: "trạng thái" tùy chỉnh cho phép sử dụng trạng thái vượt cấp
             * */
            if (customState > 0)
            {
                stateID = customState;
            }

            var stateCurrent = this.context.WF_STATE.Find(stateID);

            var process = new WF_PROCESS();
            process.ITEM_ID = ItemId;
            process.ITEM_TYPE = itemType;
            process.USER_ID = userId.ID;
            process.WF_ID = flow.ID;
            process.CURRENT_STATE = stateCurrent.ID;
            process.CURRENT_STATE_NAME = stateCurrent.STATE_NAME;
            if (customState > 0)
            {
                process.IS_END = stateCurrent.IS_KETTHUC.GetValueOrDefault();
            }
            this.Save(process);

            var userProcess = new WF_ITEM_USER_PROCESS();
            userProcess.ITEM_ID = ItemId;
            userProcess.ITEM_TYPE = itemType;
            userProcess.IS_XULYCHINH = true;
            userProcess.USER_ID = userId.ID;
            userProcess.create_at = DateTime.Now;
            userProcess.create_by = userId.ID;
            if (customState > 0)
            {
                userProcess.DAXULY = true;
            }
            this.context.WF_ITEM_USER_PROCESS.Add(userProcess);
            var log = new WF_LOG();
            log.ITEM_ID = process.ITEM_ID;
            log.ITEM_TYPE = process.ITEM_TYPE;
            log.NGUOIXULY_ID = userId.ID;
            log.WF_ID = process.WF_ID;
            if (customState > 0)
            {
                log.MESSAGE = "<div class='label label-info'>" + stateCurrent.STATE_NAME + "</div>";
            }
            else
            {
                log.MESSAGE = "<div class='label label-info'>Khởi tạo</div>";
            }
            log.STEP_ID = null;
            log.create_at = DateTime.Now;
            log.create_by = userId.ID;
            this.context.WF_LOG.Add(log);

            if(customState > 0)
            {
                var function = this.context.WF_STATE_FUNCTION
                    .Where(x => x.WF_STATE_ID == customState)
                    .FirstOrDefault();
                if(function != null)
                {
                    var functionDone = new WF_FUNCTION_DONE();
                    functionDone.ITEM_TYPE = itemType;
                    functionDone.ITEM_ID = ItemId;
                    functionDone.STATE = customState;
                    functionDone.FUNCTION_STATE = function.ID;
                    functionDone.create_at = DateTime.Now;
                    functionDone.create_by = userId.ID;
                    this.context.WF_FUNCTION_DONE.Add(functionDone);
                }
            }

            this.context.SaveChanges();
            return result;
        }
        /// <summary>
        /// Phương thức để xác nhận đối tượng sẽ nhận trạng thái nào khi được tạo với các vai trò và chức vụ khác nhau
        /// </summary>
        /// <returns></returns>
        public int GetState(int wfid, UserInfoBO userId)
        {
            var lstStep = this.context.WF_STEP.Where(x => x.WF_ID == wfid).ToList();
            var BeginState = this.context.WF_STATE.Where(x => x.WF_ID == wfid && x.IS_START == true).FirstOrDefault();
            if (BeginState != null)
            {
                var isFirst = true;
                if (BeginState.CHUCVU_ID.HasValue)
                {
                    if (userId.CHUCVU_ID != BeginState.CHUCVU_ID)
                    {
                        isFirst = false;
                    }

                }
                if (BeginState.VAITRO_ID.HasValue)
                {
                    if (!userId.ListVaiTro.Any(x => x.DM_VAITRO_ID == BeginState.VAITRO_ID.GetValueOrDefault()))
                    {
                        isFirst = false;
                    }
                }
                if (isFirst)
                {
                    return BeginState.ID;
                }
            }
            int currentState;
            foreach (var item in lstStep)
            {
                var config = this.context.WF_STEP_USER_PROCESS.Where(x => x.IS_XULYCHINH == true && x.WF_STEP_ID == item.ID).FirstOrDefault();
                if (config != null)
                {

                    var checkUser = CheckUserStep(userId, item, config);
                    if (checkUser)
                    {
                        currentState = item.STATE_END.GetValueOrDefault();
                        return currentState;
                    }

                }
            }
            return BeginState.ID;
        }
        /// <summary>
        /// Kiểm tra 1 user có thỏa mãn 1 step config hay không
        /// </summary>
        /// <param name="user"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool CheckUserStep(UserInfoBO user, WF_STEP step, WF_STEP_USER_PROCESS config)
        {
            var isResult = true;
            if (config.CHUCVU_ID.HasValue)
            {
                if (user.CHUCVU_ID != config.CHUCVU_ID)
                {
                    isResult = false;
                    return isResult;
                }
            }
            if (config.VAITRO_ID.HasValue)
            {
                if (!user.ListVaiTro.Any(x => x.DM_VAITRO_ID == config.VAITRO_ID))
                {
                    isResult = false;
                    return isResult;
                }
            }
            if (config.PHONGBAN_ID.HasValue)
            {
                if (user.DM_PHONGBAN_ID != config.PHONGBAN_ID)
                {
                    isResult = false;
                    return isResult;
                }
            }
            if (!config.IS_CUNGPHONG.HasValue)
            {
                return false;
            }

            return isResult;
        }

        public WF_PROCESS GetProcess(long idItem, string ItemCode)
        {
            var query = this.context.WF_PROCESS.Where(x => x.ITEM_ID == idItem && x.ITEM_TYPE.Equals(ItemCode)).FirstOrDefault();
            return query;
        }

        /// <summary>
        /// Thực hiện chuyển trạng thái
        /// </summary>
        /// <returns></returns>
        public JsonResultBO SaveStepAction(long processID, int stepID, long mainUser, List<long> joinUser, string message, long currentUser)
        {
            var result = new JsonResultBO(true);
            long? ngxuly = 0;
            int? curentStep = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var process = this.context.WF_PROCESS.Find(processID);
                    if (process == null)
                    {
                        result.MessageFail("Đối tượng không trong luồng xử lý");
                        return result;
                    }

                    var step = this.context.WF_STEP.Find(stepID);
                    if (step == null)
                    {
                        result.MessageFail("Không tìm thấy bước chuyển");
                        return result;
                    }
                    long idUserBack = 0;
                    var stepConfig = this.context.WF_STEP_CONFIG.Where(x => x.WF_STEP_ID == step.ID).FirstOrDefault();
                    if (stepConfig != null && stepConfig.IS_BACK_USER == true)
                    {
                        var logLast = this.context.WF_LOG.Where(x => x.ITEM_TYPE == process.ITEM_TYPE && x.ITEM_ID == process.ITEM_ID).OrderByDescending(x => x.ID).FirstOrDefault();
                        if (logLast != null)
                        {
                            idUserBack = logLast.NGUOIXULY_ID.GetValueOrDefault(0);
                        }
                    }
                    ngxuly = process.USER_ID;
                    curentStep = step.ID;
                    #region Cập nhật trạng thái cho đối tượng
                    process.CURRENT_STATE = step.STATE_END;
                    var stateEnd = this.context.WF_STATE.Find(step.STATE_END);
                    process.CURRENT_STATE_NAME = stateEnd.STATE_NAME;
                    if (stateEnd.IS_KETTHUC == true)
                    {
                        //Kiểm tra xe process đã kết thúc chưa. các hành động đã thực hiện chưa
                        var isNoFunc = true;
                        var stateFunction = this.context.WF_STATE_FUNCTION.Where(x => x.WF_STATE_ID == stateEnd.ID).FirstOrDefault();
                        if (stateFunction != null)
                        {
                            //kiểm tra xem function đã thực hiện chưa
                            var done = this.context.WF_FUNCTION_DONE.Where(x => x.STATE == stateEnd.ID && x.ITEM_TYPE == process.ITEM_TYPE && x.FUNCTION_STATE == stateFunction.ID && x.ITEM_ID == process.ITEM_ID).Any();
                            if (!done)
                            {
                                //function đã thực hiện
                                isNoFunc = false;
                            }
                        }

                        if (isNoFunc)
                        {
                            process.IS_END = true;
                        }

                    }
                    if (idUserBack > 0)
                    {
                        mainUser = idUserBack;
                    }

                    #region cập nhật lại trạng thái đã xử lý của user cũ
                    var itemuserprocess = this.context.WF_ITEM_USER_PROCESS.Where(x =>
                        x.ITEM_TYPE == process.ITEM_TYPE && x.ITEM_ID == process.ITEM_ID &&
                        x.USER_ID == process.USER_ID && x.IS_XULYCHINH.Value == true).FirstOrDefault();
                    if (itemuserprocess != null)
                    {
                        itemuserprocess.DAXULY = true;
                        this.context.SaveChanges();
                    }
                    #endregion

                    if (mainUser > 0)
                    {
                        process.USER_ID = mainUser;
                    }

                    this.context.SaveChanges();
                    #endregion

                    #region Lưu người xử lý

                    if (mainUser > 0)
                    {
                        var wf_main_process = new WF_ITEM_USER_PROCESS();
                        wf_main_process.IS_XULYCHINH = true;
                        wf_main_process.ITEM_ID = process.ITEM_ID;
                        wf_main_process.ITEM_TYPE = process.ITEM_TYPE;
                        wf_main_process.STEP_ID = curentStep;
                        wf_main_process.create_at = DateTime.Now;
                        wf_main_process.create_by = currentUser;
                        wf_main_process.USER_ID = mainUser;
                        this.context.WF_ITEM_USER_PROCESS.Add(wf_main_process);
                        this.context.SaveChanges();
                    }

                    if (joinUser != null && joinUser.Any())
                    {
                        foreach (var item in joinUser)
                        {
                            var wf_join_process = new WF_ITEM_USER_PROCESS();
                            wf_join_process.IS_XULYCHINH = false;
                            wf_join_process.ITEM_ID = process.ITEM_ID;
                            wf_join_process.ITEM_TYPE = process.ITEM_TYPE;
                            wf_join_process.STEP_ID = curentStep;
                            wf_join_process.create_at = DateTime.Now;
                            wf_join_process.create_by = currentUser;
                            wf_join_process.USER_ID = item;
                            this.context.WF_ITEM_USER_PROCESS.Add(wf_join_process);
                            this.context.SaveChanges();
                        }
                    }

                    #endregion

                    #region ghi log xử lý

                    var log = new WF_LOG();
                    log.ITEM_ID = process.ITEM_ID;
                    log.ITEM_TYPE = process.ITEM_TYPE;
                    log.MESSAGE = message;
                    log.create_at = DateTime.Now;
                    log.NGUOIXULY_ID = ngxuly;
                    log.NGUONHAN_ID = process.USER_ID;
                    log.WF_ID = step.WF_ID;
                    log.STEP_ID = step.ID;

                    this.context.WF_LOG.Add(log);
                    this.context.SaveChanges();

                    #endregion
                    transaction.Commit();
                }
                catch
                {

                    transaction.Rollback();
                    result.MessageFail("Có lỗi xử lý. Không thể chuyển trạng thái");
                }
            }
            return result;

        }

        public JsonResultBO StepDenie(long processID, int stepid, string mess, long LogID)
        {
            var result = new JsonResultBO(true);
            long? ngxuly = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var process = this.context.WF_PROCESS.Find(processID);
                    if (process == null)
                    {
                        result.MessageFail("Đối tượng không trong luồng xử lý");
                        return result;
                    }
                    //var log = this.context.WF_LOG.Where(x => x.WF_ID == process.WF_ID && x.ITEM_TYPE.Equals(process.ITEM_TYPE) && x.ITEM_ID == process.ITEM_ID).OrderByDescending(x => x.ID).FirstOrDefault();
                    var log = this.context.WF_LOG.Find(LogID);

                    #region Cập nhật lại trạng thái

                    //if (log.STEP_ID == null)
                    //{
                    //    result.MessageFail("Không có trạng thái để quay lại");
                    //    return result;
                    //}

                    ngxuly = process.USER_ID;
                    var step = this.context.WF_STEP.Find(log.STEP_ID);
                    process.CURRENT_STATE = step.STATE_BEGIN;
                    var stepConfig = this.context.WF_STEP_CONFIG.Where(x => x.WF_STEP_ID == step.ID).FirstOrDefault();
                    var state = this.context.WF_STATE.Find(step.STATE_BEGIN);
                    process.CURRENT_STATE_NAME = state.STATE_NAME;
                    process.USER_ID = log.NGUOIXULY_ID;
                    this.context.SaveChanges();
                    #endregion

                    #region ghi log xử lý

                    var logNew = new WF_LOG();
                    logNew.ITEM_ID = process.ITEM_ID;
                    logNew.ITEM_TYPE = process.ITEM_TYPE;
                    logNew.MESSAGE = mess;
                    logNew.NGUOIXULY_ID = ngxuly;
                    logNew.NGUONHAN_ID = process.USER_ID;
                    logNew.WF_ID = step.WF_ID;
                    logNew.STEP_ID = stepid;
                    logNew.IS_RETURN = true;
                    logNew.create_at = DateTime.Now;
                    logNew.create_by = ngxuly;
                    this.context.WF_LOG.Add(logNew);
                    this.context.SaveChanges();
                    if (stepConfig != null && stepConfig.IS_NOTI_MESSAGE == true)
                    {
                        //var thongbao = new SYS_TINNHAN();
                        //thongbao.TO_USER_ID = ngxuly;
                        //thongbao.URL = MODULE_CONSTANT.GetNameModule(process.ITEM_TYPE, process.ITEM_ID.GetValueOrDefault()).Link;
                        //thongbao.NOIDUNG = "Trả về 1 " + MODULE_CONSTANT.GetNameModule(process.ITEM_TYPE, process.ITEM_ID.GetValueOrDefault()).Name + "Cần xử lý";
                        //thongbao.FROM_USER_ID = process.USER_ID;
                        //thongbao.NGAYTAO = DateTime.Now;
                        //thongbao.NGUOITAO = ngxuly;
                        //this.context.SYS_TINNHAN.Add(thongbao);
                        //context.SaveChanges();
                    }
                    #endregion
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    result.MessageFail("Có lỗi xử lý. Không thể chuyển trạng thái");
                }
            }
            return result;
        }

        public List<STATISTICVANBANBO> getStaticVanBanChart(long userId, DateTime date1, DateTime date2)
        {
            string query = "select CAST(wf.UPDATED_AT AS DATE) as NgayThongKe, wf.ITEM_TYPE, count(1) as SOLUONGVANBAN from WF_PROCESS as wf " +
                           "where wf.IS_END is null and CAST(wf.UPDATED_AT AS DATE)>=CAST(@start AS DATE) and CAST(wf.UPDATED_AT AS DATE)<=CAST(@end AS DATE) group by CAST(wf.UPDATED_AT AS DATE), wf.ITEM_TYPE";
            var result = this.context.Database.SqlQuery<STATISTICVANBANBO>(query, new SqlParameter("start", date1), new SqlParameter("end", date2)).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool CheckIsEnd(string itemType, long itemId)
        {
            bool isEnd = this.context.WF_PROCESS.Where(x => x.ITEM_TYPE == itemType && x.ITEM_ID == itemId && x.IS_END == true).Count() > 0;
            return isEnd;
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra item đã kết thúc bởi người dùng
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckIsEndByUser(string itemType, long itemId, long userId)
        {
            bool isEnd = this.context.WF_PROCESS.Where(x => x.ITEM_TYPE == itemType && x.ITEM_ID == itemId && x.IS_END == true && x.USER_ID == userId).FirstOrDefault() != null;
            return isEnd;
        }
    }
}
