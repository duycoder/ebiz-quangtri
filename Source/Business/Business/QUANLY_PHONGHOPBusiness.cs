using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.BaseBusiness;
using System.Threading.Tasks;
using Model.Entities;
using CommonHelper;
using Business.CommonModel.QLPHONGHOP;
using System.Web.Mvc;

namespace Business.Business
{
    public class QUANLY_PHONGHOPBusiness : BaseBusiness<QUANLY_PHONGHOP>
    {
        public QUANLY_PHONGHOPBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {

        }

        public List<QUANLY_PHONGHOP> GetDatPhong(DateTime? NgayDat, int IDPhong)
        {
            var lstPhong = (from tblPhong in this.context.QUANLY_PHONGHOP
                            where tblPhong.NGAYDAT == NgayDat && tblPhong.PHONG_ID == IDPhong
                            select tblPhong).ToList();
            return lstPhong;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách đặt phòng trong ngày
        /// </summary>
        /// <param name="bookDay"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public List<QUANLY_PHONGHOP> GetBookingsOfRoomInDay(DateTime bookDay, int roomId)
        {
            var result = (from room in this.context.QUANLY_PHONGHOP.Where(x => x.IS_DELETE != true)
                          where room.NGAYDAT == bookDay
                          && room.PHONG_ID == roomId
                          select room)
                            .ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách đặt phòng trong ngày
        /// </summary>
        /// <param name="bookDay"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public List<QUANLY_PHONGHOP> GetBookingsOfUserInDay(DateTime bookDay, long userId)
        {
            var result = (from room in this.context.QUANLY_PHONGHOP.Where(x => x.IS_DELETE != true)
                          where room.NGAYDAT == bookDay
                          && room.USER_ID == userId
                          select room)
                            .ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: chi tiết lịch yêu cầu phòng họp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QLPHONGHOP_BO GetDetail(long id)
        {
            QLPHONGHOP_BO result = (from book in this.context.QUANLY_PHONGHOP.Where(x => x.ID == id)
                                    join room in this.context.QL_PHONGHOP
                                    on book.PHONG_ID equals room.ID
                                    into group1
                                    from g1 in group1.DefaultIfEmpty()

                                    join leader in this.context.DM_NGUOIDUNG
                                    on book.USER_ID equals leader.ID
                                    into group2
                                    from g2 in group2.DefaultIfEmpty()

                                    join user in this.context.DM_NGUOIDUNG
                                    on book.CREATED_BY equals user.ID
                                    into group3
                                    from g3 in group3.DefaultIfEmpty()

                                    select new QLPHONGHOP_BO()
                                    {
                                        ID = book.ID,
                                        MUCDICH = book.MUCDICH,
                                        THANHPHANTHAMDU = book.THANHPHANTHAMDU,
                                        NGAYDAT = book.NGAYDAT,
                                        GIOBATDAU = book.GIOBATDAU,
                                        PHUTBATDAU = book.PHUTBATDAU,
                                        GIOKETTHUC = book.GIOKETTHUC,
                                        PHUTKETTHUC = book.PHUTKETTHUC,
                                        TEN_NGUOITAO = g3.HOTEN,
                                        USER_ID = book.USER_ID,
                                        TEN_LANHDAO = g2.HOTEN,
                                        PHONG_ID = book.PHONG_ID,
                                        TENPHONG = g1.TENPHONG,
                                        CREATED_AT = book.CREATED_AT,
                                    }).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách lịch đặt phòng họp
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<QLPHONGHOP_BO> GetData(QLPHONGHOP_SEARCH searchModel)
        {
            IQueryable<QLPHONGHOP_BO> queryResult = (from cal in this.context.QUANLY_PHONGHOP.Where(x => x.IS_DELETE != true && x.NGAYDAT != null)
                                                     join user in this.context.DM_NGUOIDUNG
                                                     on cal.USER_ID equals user.ID
                                                     into group1
                                                     from g1 in group1.DefaultIfEmpty()

                                                     join room in this.context.QL_PHONGHOP
                                                     on cal.PHONG_ID equals room.ID
                                                     into group2
                                                     from g2 in group2.DefaultIfEmpty()

                                                     select new QLPHONGHOP_BO()
                                                     {
                                                         ID = cal.ID,
                                                         USER_ID = cal.USER_ID,
                                                         TEN_LANHDAO = g1.HOTEN,
                                                         PHONG_ID = cal.PHONG_ID,
                                                         MUCDICH = cal.MUCDICH ?? string.Empty,
                                                         THANHPHANTHAMDU = cal.THANHPHANTHAMDU,
                                                         NGAYDAT = cal.NGAYDAT,
                                                         GIOBATDAU = cal.GIOBATDAU ?? 0,
                                                         PHUTBATDAU = cal.PHUTBATDAU ?? 0,
                                                         GIOKETTHUC = cal.GIOKETTHUC ?? 0,
                                                         PHUTKETTHUC = cal.PHUTKETTHUC ?? 0,
                                                         CREATED_AT = cal.CREATED_AT,
                                                         CREATED_BY = cal.CREATED_BY,
                                                         CCTC_THANHPHAN_ID = cal.CCTC_THANHPHAN_ID ?? 0,
                                                     });
            if (searchModel != null)
            {
                if (searchModel.queryUserId != null)
                {
                    queryResult = queryResult.Where(x => x.USER_ID == searchModel.queryUserId.Value);
                }
                if (searchModel.queryDeptParentID != null)
                {
                    queryResult = queryResult.Where(x => x.CCTC_THANHPHAN_ID == searchModel.queryDeptParentID.Value);
                }
                if (searchModel.queryStartDate != null)
                {
                    queryResult = queryResult.Where(x => x.NGAYDAT >= searchModel.queryStartDate.Value);
                }
                if (searchModel.queryEndDate != null)
                {
                    searchModel.queryEndDate = searchModel.queryEndDate.ToEndDay();
                    queryResult = queryResult.Where(x => x.NGAYDAT <= searchModel.queryEndDate);
                }
            }
            List<QLPHONGHOP_BO> result = queryResult.ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy các năm có lịch đặt phòng
        /// @since: 15/09/2018
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public List<SelectListItem> GetListYears(DateTime selected)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            List<int> years = this.context.QUANLY_PHONGHOP.Where(x => x.NGAYDAT != null).OrderByDescending(x => x.NGAYDAT.Value.Year)
                .Select(x => x.NGAYDAT.Value.Year).Distinct().ToList();
            if (years.Any() == false)
            {
                result.Add(new SelectListItem()
                {
                    Value = selected.Year.ToString(),
                    Text = selected.Year.ToString(),
                    Selected = true
                });
            }
            else
            {
                if (years.Contains(selected.Year) == false)
                {
                    years.Add(selected.Year);
                }

                for (int i = years.Min(); i < years.Max(); i++)
                {
                    var nextYear = i + 1;
                    if (!years.Contains(nextYear))
                    {
                        years.Add(nextYear);
                    }
                }

                years.Sort();
                years.ForEach(x =>
                {
                    result.Add(new SelectListItem()
                    {
                        Value = x.ToString(),
                        Text = x.ToString(),
                        Selected = (x == selected.Year)
                    });
                });
            }
            return result;
        }
    }
}
