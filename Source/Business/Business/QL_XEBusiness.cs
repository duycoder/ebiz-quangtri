using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLXE;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Business.Business
{
    public class QL_XEBusiness : BaseBusiness<QL_XE>
    {
        public QL_XEBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách xe
        /// </summary>
        /// <returns></returns>
        public PageListResultBO<XeBO> GetDataByPage(XeSearchBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            IQueryable<XeBO> queryResult = (from car in this.context.QL_XE.Where(x => x.IS_DELETE != true)
                                            join trip in this.context.QL_DANGKYXE_LAIXE
                                            on car.ID equals trip.XE_ID
                                            into group1
                                            select new XeBO()
                                            {
                                                ID = car.ID,
                                                IMAGE_PATH = car.IMAGE_PATH,
                                                TENXE = car.TENXE,
                                                BIENSO = car.BIENSO,
                                                SOCHO = car.SOCHO,
                                                NGAYSUA = car.NGAYSUA,
                                                CCTC_THANHPHAN_ID = car.CCTC_THANHPHAN_ID,
                                                TRANGTHAI = group1.OrderByDescending(x=>x.ID).Select(x=>x.TRANGTHAI).FirstOrDefault() ?? TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_ID,
                                            });
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.TENXE))
                {
                    searchModel.TENXE = searchModel.TENXE.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.TENXE) && x.TENXE.Trim().ToLower().Contains(searchModel.TENXE));
                }
                if (!string.IsNullOrEmpty(searchModel.BIENSO))
                {
                    searchModel.BIENSO = searchModel.BIENSO.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.BIENSO) && x.BIENSO.Trim().ToLower().Contains(searchModel.BIENSO));
                }

                if (searchModel.querySoChoStart != null)
                {
                    queryResult = queryResult.Where(x => x.SOCHO >= searchModel.querySoChoStart);
                }
                if (searchModel.querySoChoEnd != null)
                {
                    queryResult = queryResult.Where(x => x.SOCHO <= searchModel.querySoChoEnd);
                }

                if (searchModel.CCTC_THANHPHAN_ID != null)
                {
                    queryResult = queryResult.Where(x => x.CCTC_THANHPHAN_ID == searchModel.CCTC_THANHPHAN_ID.Value);
                }
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    queryResult = queryResult.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    queryResult = queryResult.OrderByDescending(x => x.ID).ThenByDescending(x => x.NGAYSUA);
                }
            }
            PageListResultBO<XeBO> result = new PageListResultBO<XeBO>();
            IPagedList<XeBO> pagedList = queryResult.ToPagedList(pageIndex, pageSize);
            result.Count = pagedList.TotalItemCount;
            result.TotalPage = pagedList.PageCount;
            result.ListItem = pagedList.ToList();
            return result;
        }


        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách xe đang có sẵn để phục vụ
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownAvailableCars(int carType = 0)
        {
            IQueryable<QL_XE> queryResult = (from car in this.context
                                           .QL_XE.Where(x => x.IS_DELETE != true)
                                             select car);
            //if (carType > 0)
            //{
            //    
            //}
            List<SelectListItem> result = queryResult
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.TENXE
                }).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: Lấy danh sách xe đang có sẵn để phục vụ cho yêu cầu đăng ký
        /// </summary>
        /// <param name="carType"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownAvailableCarsForTrip(long registrationId, int carType = 0)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            QL_DANGKY_XE registration = this.context.QL_DANGKY_XE.Find(registrationId);
            if (registration != null && registration.IS_DELETE != true && registration.TRANGTHAI != TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID)
            {
                //lấy danh sách mã xe đang trong chuyến
                List<int> drivingCarIds = this.context.QL_DANGKYXE_LAIXE.Where(x => x.CCTC_THANHPHAN_ID == registration.CCTC_THANHPHAN_ID)
                    .Where(x => x.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_ID && x.XE_ID != null)
                    .Select(x => x.XE_ID.Value).ToList();
                //lấy danh sách mã xe mới tạo chuyến cùng ngày và giờ với lịch đăng ký
                List<int> sameTimeCarIds = (from car in this.context.QL_XE.Where(x => x.CCTC_THANHPHAN_ID == registration.CCTC_THANHPHAN_ID)
                                            join trip in this.context.QL_DANGKYXE_LAIXE.Where(x=>x.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.MOITAO_ID)
                                            on car.ID equals trip.XE_ID
                                            join register in this.context.QL_DANGKY_XE
                                                .Where(x=>x.NGAY_XUATPHAT != null && x.GIO_XUATPHAT != null)
                                                .Where(x => x.NGAY_XUATPHAT == registration.NGAY_XUATPHAT && x.GIO_XUATPHAT ==  x.GIO_XUATPHAT)
                                            on trip.QL_DANGKY_XE_ID equals register.ID
                                            select car.ID).ToList();

                List<int> notAvailableCarIds = new List<int>();
                notAvailableCarIds.AddRange(drivingCarIds);
                notAvailableCarIds.AddRange(sameTimeCarIds);

                IQueryable<QL_XE> queryResult = (from car in this.context.QL_XE.Where(x => x.CCTC_THANHPHAN_ID == registration.CCTC_THANHPHAN_ID)
                                                 .Where(x => x.IS_DELETE != true && notAvailableCarIds.Contains(x.ID) == false)
                                                 select car);

                result = queryResult.Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.TENXE
                }).ToList();
            }
            return result;
        }
    }
}
