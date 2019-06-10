using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness : BaseBusiness<HSCV_CONGVIEC_NOIDUNGTRAODOI>
    {
        public HSCV_CONGVIEC_NOIDUNGTRAODOIBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(HSCV_CONGVIEC_NOIDUNGTRAODOI comment)
        {
            try
            {
                if (comment.ID == 0)
                {
                    this.repository.Insert(comment);
                }
                else
                {
                    this.repository.Update(comment);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<UserComment> GetListCommentByCongViecId(long CongViecId)
        {
            var result = (from noidungtraodoi in this.context.HSCV_CONGVIEC_NOIDUNGTRAODOI
                join nguoidung in this.context.DM_NGUOIDUNG
                    on noidungtraodoi.USER_ID.Value equals nguoidung.ID
                    into group1
                from g1 in group1.DefaultIfEmpty()
                where noidungtraodoi.CONGVIEC_ID == CongViecId
                select new UserComment()
                {
                    UserAvatar = g1.ANH_DAIDIEN,
                    FullName = g1.HOTEN,
                    UserId = noidungtraodoi.USER_ID,
                    CONGVIEC_ID = noidungtraodoi.CONGVIEC_ID,
                    ID = noidungtraodoi.ID,
                    NGAYTAO = noidungtraodoi.CREATED_AT,
                    NOIDUNG = noidungtraodoi.NOIDUNG,
                    REPLY_ID = noidungtraodoi.REPLY_ID,
                }
            ).ToList();
            return result;
        }

        /// <summary>
        /// @description: lấy các comment chính ở 1 công việc
        /// @author: duynn
        /// @since: 06/06/2018
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UserComment> GetRootCommentsOfTask(long taskId, int pageIndex = 1, int pageSize = 20)
        {
            var queryResult = (from content in this.context.HSCV_CONGVIEC_NOIDUNGTRAODOI
                               join user in this.context.DM_NGUOIDUNG
                               on content.USER_ID equals user.ID
                               into group1
                               from g1 in group1.DefaultIfEmpty()
                               where content.CONGVIEC_ID == taskId && content.REPLY_ID == null
                               select new UserComment()
                               {
                                   ID = content.ID,
                                   UserId = content.USER_ID,
                                   UserAvatar = g1.ANH_DAIDIEN,
                                   FullName = g1.HOTEN,
                                   CONGVIEC_ID = content.CONGVIEC_ID,
                                   NGAYTAO = content.CREATED_AT,
                                   NOIDUNG = content.NOIDUNG,
                                   REPLY_ID = content.REPLY_ID
                               });
            var result = queryResult.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// @description: lấy danh sách trả lời comment của một công việc
        /// @author: duynn
        /// @since: 06/06/2018
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UserComment> GetRepliesOfComment(long commentId, int pageIndex = 1, int pageSize = 20)
        {
            var queryResult = (from content in this.context.HSCV_CONGVIEC_NOIDUNGTRAODOI
                               join user in this.context.DM_NGUOIDUNG
                               on content.USER_ID equals user.ID
                               into group1
                               from g1 in group1.DefaultIfEmpty()
                               where  content.REPLY_ID == commentId
                               select new UserComment()
                               {
                                   ID = content.ID,
                                   UserId = content.USER_ID,
                                   UserAvatar = g1.ANH_DAIDIEN,
                                   FullName = g1.HOTEN,
                                   CONGVIEC_ID = content.CONGVIEC_ID,
                                   NGAYTAO = content.CREATED_AT,
                                   NOIDUNG = content.NOIDUNG,
                                   REPLY_ID = content.REPLY_ID
                               });
            var result = queryResult.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}
