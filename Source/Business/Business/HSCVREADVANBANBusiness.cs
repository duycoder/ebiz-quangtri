using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.DMVAITRO;
using CommonHelper;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Configuration;
using System.Web.Mvc;


namespace Business.Business
{
    public class HSCVREADVANBANBusiness : BaseBusiness<HSCV_READVANBAN>
    {
        public HSCVREADVANBANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra các văn bản đến gửi nội bộ đã được đọc
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public bool CheckIsRead(string itemType, long itemId, CCTC_THANHPHAN department, UserInfoBO currentUser)
        {
            bool isRead = false;
            if (itemType == MODULE_CONSTANT.VANBANDENNOIBO)
            {
                WF_MODULE module = null;
                if (department.TYPE == 10)
                {
                    module = this.context.WF_MODULE.Where(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANDEN)
                        .FirstOrDefault();
                }
                else if (department.PARENT_ID == currentUser.DeptParentID)
                {
                    module = this.context.WF_MODULE.Where(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANDENNOIBO)
                        .FirstOrDefault();
                }

                if (module != null)
                {
                    var workFlowIds = module.WF_STREAM_ID.ToListInt(',');
                    WF_STREAM stream = this.context.WF_STREAM.Where(x => x.LEVEL_ID == department.CATEGORY && workFlowIds.Contains(x.ID)).FirstOrDefault();
                    WF_STATE state = this.context.WF_STATE.Where(x => x.IS_START == true && x.WF_ID == stream.ID).FirstOrDefault();

                    var userBusiness = new DM_NGUOIDUNGBusiness(new UnitOfWork());
                    List<long> userIds = new List<long>();

                    if (department.TYPE == 10)
                    {
                        userIds = userBusiness
                       .GetUserByRoleAndParentDept(state.VAITRO_ID.GetValueOrDefault(), department.ID)
                       .Select(x => long.Parse(x.Value)).ToList();
                    }
                    else if (department.PARENT_ID == currentUser.DeptParentID)
                    {
                        userIds = userBusiness
                        .GetUserByRoleAndDeptId(state.VAITRO_ID.GetValueOrDefault(), department.ID)
                        .Select(x => long.Parse(x.Value)).ToList();
                    }

                    if (!userIds.Any())
                    {
                        //lấy người dùng có vai trò cao nhất
                        var highestPriorityUser = userBusiness.GetUserHighestPriority(department.ID);
                        userIds.Add(highestPriorityUser.ID);
                    }

                    isRead = (from read in this.context.HSCV_READVANBAN.Where(x => x.TYPE == 1)
                              join vanban in this.context.HSCV_VANBANDEN.Where(x => x.VANBANDI_ID == itemId)
                              on read.VANBAN_ID equals vanban.ID
                              join user in this.context.DM_NGUOIDUNG
                              on read.USER_ID equals user.ID
                              join dept in this.context.CCTC_THANHPHAN
                              on user.DM_PHONGBAN_ID equals dept.ID
                              select read.USER_ID).Where(x => userIds.Contains(x)).Count() > 0;
                }
            }
            return isRead;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người đọc
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<DM_NGUOIDUNG_BO> GetUsersRead(string itemType, long itemId)
        {
            List<DM_NGUOIDUNG_BO> result = new List<DM_NGUOIDUNG_BO>();
            if (itemType == MODULE_CONSTANT.VANBANDENNOIBO)
            {
                HSCV_VANBANDI entity = this.context.HSCV_VANBANDI.Find(itemId);
                if (entity != null)
                {
                    result = (from user in this.context.DM_NGUOIDUNG
                              join read in this.context.HSCV_READVANBAN
                              on user.ID equals read.USER_ID
                              join dept in this.context.CCTC_THANHPHAN
                              on user.DM_PHONGBAN_ID equals dept.ID
                              join doc in this.context.HSCV_VANBANDEN.Where(x => x.VANBANDI_ID == itemId)
                              on read.VANBAN_ID equals doc.ID
                              where read.TYPE == 1
                              select new DM_NGUOIDUNG_BO()
                              {
                                  ID = user.ID,
                                  HOTEN = user.HOTEN,
                                  TenPhongBan = dept.NAME
                              }).ToList();
                }
            }
            else if (itemType == MODULE_CONSTANT.VANBANTRINHKY)
            {
                HSCV_VANBANDI entity = this.context.HSCV_VANBANDI.Find(itemId);
                if (entity != null)
                {
                    result = (from user in this.context.DM_NGUOIDUNG
                              join read in this.context.HSCV_READVANBAN
                              on user.ID equals read.USER_ID
                              join dept in this.context.CCTC_THANHPHAN
                              on user.DM_PHONGBAN_ID equals dept.ID
                              join doc in this.context.HSCV_VANBANDI.Where(x => x.ID == itemId)
                              on read.VANBAN_ID equals doc.ID
                              where read.TYPE == 2
                              select new DM_NGUOIDUNG_BO()
                              {
                                  ID = user.ID,
                                  HOTEN = user.HOTEN,
                                  TenPhongBan = dept.NAME
                              }).ToList();
                }
            }
            return result;
        }
    }
}

