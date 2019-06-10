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
    public class HSCV_CONGVIEC_XINLUIHANBusiness : BaseBusiness<HSCV_CONGVIEC_XINLUIHAN>
    {
        public HSCV_CONGVIEC_XINLUIHANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<CongViecLuiHanBO> GetListByCongViec(long congviec_id)
        {
            var result = (from lh in this.context.HSCV_CONGVIEC_XINLUIHAN
                          join cv in this.context.HSCV_CONGVIEC
                              on lh.CONGVIEC_ID equals cv.ID
                          join user in this.context.DM_NGUOIDUNG
                              on lh.USER_ID equals user.ID
                          select new CongViecLuiHanBO
                          {
                              ID = lh.ID,
                              CONGVIEC_ID = lh.CONGVIEC_ID,
                              TENCONGVIEC = cv.TENCONGVIEC,
                              FullName = user.HOTEN,
                              NGAYTAO = lh.NGAYTAO,
                              HANKETHUC = lh.HANKETHUC,
                              IS_SENDREQUEST = lh.IS_SENDREQUEST,
                              IS_APPROVED = lh.IS_APPROVED,
                              TIEUDE = lh.TIEUDE,
                              NOIDUNG = lh.NOIDUNG,
                              USER_ID = lh.USER_ID,
                              NGUOIGIAOVIEC_ID = cv.NGUOIGIAOVIEC_ID,
                              BUTPHELANHDAO = lh.BUTPHELANHDAO,
                              HANKETHUCTRUOC = lh.HANKETTHUCTRUOC,
                              NGAYGUI = lh.NGAYGUI,
                              NGAYPHEDUYET = lh.NGAYPHEDUYET,
                              HANKETTHUC_LANHDAODUYET = lh.HANKETTHUC_LANHDAODUYET
                          });
            if (congviec_id > 0)
            {
                result = result.Where(o => o.CONGVIEC_ID == congviec_id);
            }
            return result.OrderByDescending(o => o.NGAYTAO).ToList();
        }

        /// <summary>
        /// @description: lấy danh sách lùi hạn công việc
        /// @author: duynn
        /// @since: 01/06/2018
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<CongViecLuiHanBO> GetListRescheduleTask(long taskId, string query, int pageIndex = 1, int pageSize = 20)
        {
        	var queryResult = (from lh in this.context.HSCV_CONGVIEC_XINLUIHAN
                          join cv in this.context.HSCV_CONGVIEC
                              on lh.CONGVIEC_ID equals cv.ID
                          join user in this.context.DM_NGUOIDUNG
                              on lh.USER_ID equals user.ID
                          where  lh.CONGVIEC_ID == taskId
                          select new CongViecLuiHanBO
                          {
                              ID = lh.ID,
                              CONGVIEC_ID = lh.CONGVIEC_ID,
                              TENCONGVIEC = cv.TENCONGVIEC,
                              FullName = user.HOTEN,
                              NGAYTAO = lh.NGAYTAO,
                              HANKETHUC = lh.HANKETHUC,
                              IS_SENDREQUEST = lh.IS_SENDREQUEST,
                              IS_APPROVED = lh.IS_APPROVED,
                              TIEUDE = lh.TIEUDE,
                              NOIDUNG = lh.NOIDUNG,
                              USER_ID = lh.USER_ID,
                              NGUOIGIAOVIEC_ID = cv.NGUOIGIAOVIEC_ID,
                              BUTPHELANHDAO = lh.BUTPHELANHDAO,
                              HANKETHUCTRUOC = lh.HANKETTHUCTRUOC,
                              HANKETTHUC_LANHDAODUYET = lh.HANKETTHUC_LANHDAODUYET,
                              NGAYGUI = lh.NGAYGUI,
                              NGAYPHEDUYET = lh.NGAYPHEDUYET,
                          });
        	if(!string.IsNullOrEmpty(query)){
        		query = query.Trim().ToLower();
                queryResult = queryResult.Where(x=> string.IsNullOrEmpty(x.FullName) && x.FullName.Trim().ToLower().Contains(query));
        	}

        	var result = queryResult.OrderByDescending(x=>x.ID).Skip((pageIndex -1) * pageSize).Take(pageSize).ToList();
        	return result;
        }
    }
}
