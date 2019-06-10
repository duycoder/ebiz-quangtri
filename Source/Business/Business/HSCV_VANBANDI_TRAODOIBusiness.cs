using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonBusiness;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using PagedList;
using Business.CommonModel.CCTCTHANHPHAN;
using System.Collections;
using System.Web.Mvc;



namespace Business.Business
{
    public class HSCV_VANBANDI_TRAODOIBusiness : BaseBusiness<HSCV_VANBANDI_TRAODOI>
    {
        public HSCV_VANBANDI_TRAODOIBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<UserComment> GetListCommentByVanBanDiId(long ID)
        {
            var result = (from noidungtraodoi in this.context.HSCV_VANBANDI_TRAODOI
                join nguoidung in this.context.DM_NGUOIDUNG
                    on noidungtraodoi.NGUOITAO.Value equals nguoidung.ID
                    into group1
                from g1 in group1.DefaultIfEmpty()
                where noidungtraodoi.VANBANDI_ID == ID
                select new UserComment()
                {
                    UserAvatar = g1.ANH_DAIDIEN,
                    FullName = g1.HOTEN,
                    UserId = noidungtraodoi.NGUOITAO,
                    CONGVIEC_ID = noidungtraodoi.VANBANDI_ID,
                    ID = noidungtraodoi.ID,
                    NGAYTAO = noidungtraodoi.NGAYTAO,
                    NOIDUNG = noidungtraodoi.NOIDUNGTRAODOI,
                    REPLY_ID = noidungtraodoi.PARENT_ID,
                }
            ).ToList();
            return result;
        }

        /// <summary>
        /// @description: lấy danh sách comment cha của văn bản
        /// @author: duynn
        /// @since: 13/06/2018
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UserComment> GetRootCommentsOfVanBan(long id,int pageIndex, int pageSize)
        {
            var queryResult = (from noidungtraodoi in this.context.HSCV_VANBANDI_TRAODOI
                          join nguoidung in this.context.DM_NGUOIDUNG
                              on noidungtraodoi.NGUOITAO.Value equals nguoidung.ID
                              into group1
                          from g1 in group1.DefaultIfEmpty()
                          where noidungtraodoi.VANBANDI_ID == id  && noidungtraodoi.PARENT_ID == null
                               select new UserComment()
                          {
                              UserAvatar = g1.ANH_DAIDIEN,
                              FullName = g1.HOTEN,
                              UserId = noidungtraodoi.NGUOITAO,
                              CONGVIEC_ID = noidungtraodoi.VANBANDI_ID,
                              ID = noidungtraodoi.ID,
                              NGAYTAO = noidungtraodoi.NGAYTAO,
                              NOIDUNG = noidungtraodoi.NOIDUNGTRAODOI,
                              REPLY_ID = noidungtraodoi.PARENT_ID,
                          });
            var result = queryResult.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// @description: lấy danh sách trả lời comment của một văn bản
        /// @author: duynn
        /// @since: 06/06/2018
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UserComment> GetRepliesOfComment(long commentId, int pageIndex = 1, int pageSize = 20)
        {
            var queryResult = (from noidungtraodoi in this.context.HSCV_VANBANDI_TRAODOI
                               join nguoidung in this.context.DM_NGUOIDUNG
                                   on noidungtraodoi.NGUOITAO.Value equals nguoidung.ID
                                   into group1
                               from g1 in group1.DefaultIfEmpty()
                               where noidungtraodoi.PARENT_ID == commentId
                               select new UserComment()
                               {
                                   UserAvatar = g1.ANH_DAIDIEN,
                                   FullName = g1.HOTEN,
                                   UserId = noidungtraodoi.NGUOITAO,
                                   CONGVIEC_ID = noidungtraodoi.VANBANDI_ID,
                                   ID = noidungtraodoi.ID,
                                   NGAYTAO = noidungtraodoi.NGAYTAO,
                                   NOIDUNG = noidungtraodoi.NOIDUNGTRAODOI,
                                   REPLY_ID = noidungtraodoi.PARENT_ID,
                               });
            var result = queryResult.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}

