using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLCHUYEN;
using CommonHelper;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class QL_DANGKYXE_LAIXEBusiness : BaseBusiness<QL_DANGKYXE_LAIXE>
    {
        public QL_DANGKYXE_LAIXEBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách chuyến
        /// @since: 28/08/2018
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<ChuyenBO> GetDataByPage(ChuyenSearchBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            IQueryable<ChuyenBO> queryResult = (from trip in this.context.QL_DANGKYXE_LAIXE
                                                join registration in this.context.QL_DANGKY_XE
                                                on trip.QL_DANGKY_XE_ID equals registration.ID

                                                join car in this.context.QL_XE
                                                on trip.XE_ID equals car.ID

                                                join driver in this.context.QL_LAIXE
                                                on trip.LAIXE_ID equals driver.ID
                                                orderby trip.ID descending
                                                select new ChuyenBO
                                                {
                                                    ID = trip.ID,
                                                    TEN_CHUYEN = trip.TEN_CHUYEN,
                                                    MUCDICH = registration.MUCDICH,
                                                    XE_ID = car.ID,
                                                    TENXE = car.TENXE,
                                                    LAIXE_ID = driver.ID,
                                                    TEN_LAIXE = driver.HOTEN,
                                                    TRANGTHAI = trip.TRANGTHAI ?? 0,
                                                    NGAY_XUATPHAT = registration.NGAY_XUATPHAT,
                                                    GIO_XUATPHAT = registration.GIO_XUATPHAT,
                                                    PHUT_XUATPHAT = registration.PHUT_XUATPHAT,
                                                    NGAYSUA = registration.NGAYSUA,
                                                    NGUOITAO = trip.NGUOITAO,
                                                    CCTC_THANHPHAN_ID = trip.CCTC_THANHPHAN_ID,
                                                });
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.TEN_CHUYEN))
                {
                    searchModel.TEN_CHUYEN = searchModel.TEN_CHUYEN.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.TEN_CHUYEN) && x.TEN_CHUYEN.Trim().ToLower().Contains(searchModel.TEN_CHUYEN));
                }

                if (searchModel.CCTC_THANHPHAN_ID != null)
                {
                    queryResult = queryResult.Where(x => x.CCTC_THANHPHAN_ID == searchModel.CCTC_THANHPHAN_ID.Value);
                }

                if (searchModel.queryTimeStart != null)
                {
                    queryResult = queryResult.Where(x => x.NGAY_XUATPHAT >= searchModel.queryTimeStart);
                }

                if (searchModel.queryTimeEnd != null)
                {
                    queryResult = queryResult.Where(x => x.NGAY_XUATPHAT <= searchModel.queryTimeEnd);
                }
                if (searchModel.XE_ID != null)
                {
                    queryResult = queryResult.Where(x => x.XE_ID == searchModel.XE_ID);
                }

                if (searchModel.LAIXE_ID != null)
                {
                    queryResult = queryResult.Where(x => x.LAIXE_ID == searchModel.LAIXE_ID);
                }

                if (string.IsNullOrEmpty(searchModel.sortQuery) == false)
                {
                    queryResult = queryResult.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    queryResult = queryResult.OrderByDescending(x => x.NGAYSUA).ThenByDescending(x => x.ID);
                }
            }
            PageListResultBO<ChuyenBO> result = new PageListResultBO<ChuyenBO>();
            IPagedList<ChuyenBO> pagedList = queryResult.ToPagedList(pageIndex, pageSize);
            result.Count = pagedList.TotalItemCount;
            result.TotalPage = pagedList.PageCount;
            result.ListItem = pagedList.ToList();
            result.ListItem.ForEach(x =>
            {
                if (x.NGAY_XUATPHAT != null)
                {
                    x.THOIGIAN_XUATPHAT = string.Format("{0:dd/MM/yyyy}", x.NGAY_XUATPHAT);
                    if (x.GIO_XUATPHAT != null)
                    {
                        x.THOIGIAN_XUATPHAT += " " + x.GIO_XUATPHAT.Value.ToString("D2") + "h";

                        if (x.PHUT_XUATPHAT != null)
                        {
                            x.THOIGIAN_XUATPHAT += x.PHUT_XUATPHAT.Value.ToString("D2");
                        }
                    }
                }
                switch (x.TRANGTHAI.Value)
                {
                    case TRANGTHAI_CHUYEN_CONSTANT.MOITAO_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.MOITAO_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.MOITAO_COLOR;
                        break;
                    case TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_COLOR;
                        break;
                    case TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_COLOR;
                        break;
                    default:
                        break;
                }
            });
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: chi tiết chuyến
        /// @since: 28/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChuyenBO GetDetail(long id)
        {
            ChuyenBO result = (from trip in this.context.QL_DANGKYXE_LAIXE.Where(x => x.ID == id)
                               join registration in this.context.QL_DANGKY_XE
                               on trip.QL_DANGKY_XE_ID equals registration.ID

                               join car in this.context.QL_XE
                               on trip.XE_ID equals car.ID

                               join driver in this.context.QL_LAIXE
                               on trip.LAIXE_ID equals driver.ID

                               join leader in this.context.DM_NGUOIDUNG
                               on registration.CANBO_ID equals leader.ID
                               into groupLeader
                               from grLeader in groupLeader.DefaultIfEmpty()

                               join calendar in this.context.LICHCONGTAC
                               on registration.LICHCONGTAC_ID equals calendar.ID

                               into groupCalendar
                               from grCal in groupCalendar.DefaultIfEmpty()
                               orderby trip.ID descending
                               select new ChuyenBO
                               {
                                   ID = trip.ID,
                                   TEN_CHUYEN = trip.TEN_CHUYEN,
                                   MUCDICH = registration.MUCDICH,
                                   NOIDUNG = registration.NOIDUNG,
                                   XE_ID = car.ID,
                                   TENXE = car.TENXE,
                                   LAIXE_ID = driver.ID,
                                   TEN_LAIXE = driver.HOTEN,
                                   DIENTHOAI_LAIXE = driver.SODIENTHOAI,
                                   TRANGTHAI = trip.TRANGTHAI ?? 0,
                                   NGAY_XUATPHAT = registration.NGAY_XUATPHAT,
                                   GIO_XUATPHAT = registration.GIO_XUATPHAT,
                                   PHUT_XUATPHAT = registration.PHUT_XUATPHAT,
                                   DIEM_XUATPHAT = registration.DIEM_XUATPHAT,
                                   NGAYSUA = registration.NGAYSUA,
                                   NGUOITAO = registration.NGUOITAO,
                                   IS_BHYT = trip.IS_BHYT,
                                   LOAICHUYEN_ID = trip.LOAICHUYEN_ID,
                                   IS_TUVONG_TRENDUONG = trip.IS_TUVONG_TRENDUONG,
                                   TEN_CANBO = grLeader.HOTEN,
                                   TEN_BENHNHAN = registration.TEN_BENHNHAN,
                                   LICHCONGTAC_ID = grCal.ID,
                                   TEN_LICHCONGTAC = grCal.TIEUDE
                               }).FirstOrDefault();
            if (result != null)
            {
                if (result.LOAICHUYEN_ID != null)
                {
                    result.TEN_LOAICHUYEN = (result.LOAICHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN ?
                        TENLOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN : TENLOAICHUYEN_CONSTANT.CHUYEN_VE);
                }

                if (result.NGAY_XUATPHAT != null)
                {
                    result.THOIGIAN_XUATPHAT = string.Format("{0:dd/MM/yyyy}", result.NGAY_XUATPHAT);
                    if (result.GIO_XUATPHAT != null)
                    {
                        result.THOIGIAN_XUATPHAT += " " + result.GIO_XUATPHAT.Value.ToString("D2") + "h";

                        if (result.PHUT_XUATPHAT != null)
                        {
                            result.THOIGIAN_XUATPHAT += result.PHUT_XUATPHAT.Value.ToString("D2");
                        }
                    }
                }
                switch (result.TRANGTHAI.Value)
                {
                    case TRANGTHAI_CHUYEN_CONSTANT.MOITAO_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.MOITAO_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.MOITAO_COLOR;
                        break;
                    case TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_COLOR;
                        break;
                    case TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_COLOR;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: báo cáo chuyến
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<ChuyenReportBO> GetTripReportResult(ChuyenSearchBO searchModel)
        {
            List<UserInfoBO> users = (from user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)
                                        join dept in this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true && x.PARENT_ID == searchModel.CCTC_THANHPHAN_ID)
                                        on user.DM_PHONGBAN_ID equals dept.ID
                                        select new UserInfoBO()
                                        {
                                            ID = user.ID,
                                            HOTEN = user.HOTEN
                                        }).ToList();


            IQueryable<ChuyenBO> queryResult = (from user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)
                                                join dept in this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true && x.PARENT_ID == searchModel.CCTC_THANHPHAN_ID)
                                                on user.DM_PHONGBAN_ID equals dept.ID
                                                join registration in this.context.QL_DANGKY_XE.Where(x => x.IS_DELETE != true)
                                                on user.ID equals registration.CANBO_ID
                                                into group1
                                                from g1 in group1.DefaultIfEmpty()

                                                join trip in this.context.QL_DANGKYXE_LAIXE.Where(x => x.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_ID)
                                                on g1.ID equals trip.QL_DANGKY_XE_ID
                                                select new ChuyenBO()
                                                {
                                                    CANBO_ID = user.ID,
                                                    TEN_CANBO = user.HOTEN,
                                                    TEN_CHUYEN = trip.TEN_CHUYEN,
                                                    NGAY_XUATPHAT = g1.NGAY_XUATPHAT,
                                                    GIO_XUATPHAT = g1.GIO_XUATPHAT ?? 0,
                                                    PHUT_XUATPHAT = g1.PHUT_XUATPHAT ?? 0,
                                                    DIEM_XUATPHAT = g1.DIEM_XUATPHAT,
                                                    DIEM_KETTHUC = g1.DIEM_KETTHUC,
                                                    TONG_CHIPHI = trip.TONG_CHIPHI ?? 0,
                                                    NGAYSUA = trip.NGAYSUA,
                                                    QUANGDUONG_DICHUYEN = trip.QUANGDUONG_DICHUYEN ?? 0
                                                });
            if (searchModel.queryTimeStart != null)
            {
                searchModel.queryTimeStart = searchModel.queryTimeStart.Value.ToStartDay();
                queryResult = queryResult.Where(x => x.NGAY_XUATPHAT >= searchModel.queryTimeStart.Value);
            }

            if (searchModel.queryTimeEnd != null)
            {
                searchModel.queryTimeEnd = searchModel.queryTimeEnd.Value.ToEndDay();
                queryResult = queryResult.Where(x => x.NGAY_XUATPHAT <= searchModel.queryTimeEnd.Value);
            }

            if (searchModel.CANBO_IDs != null && searchModel.CANBO_IDs.Count > 0)
            {
                users = users.Where(x => searchModel.CANBO_IDs.Contains(x.ID)).ToList();
                queryResult = queryResult.Where(x => searchModel.CANBO_IDs.Contains(x.CANBO_ID));
            }

            List<ChuyenReportBO> result = new List<ChuyenReportBO>();
            
            foreach(var user in users)
            {
                ChuyenReportBO item = new ChuyenReportBO();
                item.TEN_CANBO = user.HOTEN;
                item.groupOfTrips = queryResult.Where(x => x.CANBO_ID == user.ID).ToList();
                result.Add(item);
            }
            return result;
        }

        ///// <summary>
        ///// @author: duynn
        ///// @description: lấy kết quả báo cáo công tác vận chuyển
        ///// </summary>
        ///// <param name="searchModel"></param>
        ///// <returns></returns>
        //public ChuyenReportBO GetTripReportResult(ChuyenSearchBO searchModel)
        //{
        //    IQueryable<ChuyenBO> queryCompletedTrips = (from trip in this.context.QL_DANGKYXE_LAIXE
        //                                     .Where(x => x.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_ID)
        //                                                join register in this.context.QL_DANGKY_XE
        //                                                on trip.QL_DANGKY_XE_ID equals register.ID
        //                                                select new ChuyenBO()
        //                                                {
        //                                                    NGAY_XUATPHAT = register.NGAY_XUATPHAT,
        //                                                    IS_BHYT = trip.IS_BHYT,
        //                                                    IS_TUVONG_TRENDUONG = trip.IS_TUVONG_TRENDUONG,
        //                                                    LOAICHUYEN_ID = trip.LOAICHUYEN_ID,
        //                                                });
        //    if (searchModel.queryTimeStart != null)
        //    {
        //        queryCompletedTrips = queryCompletedTrips.Where(x => x.NGAY_XUATPHAT >= searchModel.queryTimeStart);
        //    }

        //    if (searchModel.queryTimeEnd != null)
        //    {
        //        queryCompletedTrips = queryCompletedTrips.Where(x => x.NGAY_XUATPHAT <= searchModel.queryTimeEnd);
        //    }
        //    List<ChuyenBO> completedTrips = queryCompletedTrips.ToList();
        //    ChuyenReportBO result = new ChuyenReportBO();
        //    result.TONGSO_CHUYEN = completedTrips.Count();
        //    result.SOCHUYEN_HUONG_BHYT = completedTrips.Where(x => x.IS_BHYT == true).Count();
        //    result.SOCHUYEN_KHONGHUONG_BHYT = completedTrips.Where(x => x.IS_BHYT == false).Count();
        //    result.SOCHUYEN_NGANGTUYEN = completedTrips.Where(x => x.LOAICHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN).Count();
        //    result.SOCHUYEN_CHUYENVE = completedTrips.Where(x => x.LOAICHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_VE).Count();
        //    result.SOCHUYEN_TUVONG = completedTrips.Where(x => x.IS_TUVONG_TRENDUONG == true).Count();
        //    return result;
        //}
    }
}
