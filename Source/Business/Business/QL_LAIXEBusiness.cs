using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLLAIXE;
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
    public class QL_LAIXEBusiness : BaseBusiness<QL_LAIXE>
    {
        public QL_LAIXEBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        /// <summary>
        /// @author: duynn
        /// @since: 27/08/2018
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<LaiXeBO> GetDataByPage(LaiXeSearchBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            IQueryable<LaiXeBO> queryResult = (from driver in this.context.QL_LAIXE.Where(x => x.IS_DELETE != true)
                                               select new LaiXeBO()
                                               {
                                                   ID = driver.ID,
                                                   GIOITINH = driver.GIOITINH,
                                                   HOTEN = driver.HOTEN,
                                                   CMND = driver.CMND,
                                                   SODIENTHOAI = driver.SODIENTHOAI,
                                                   EMAIL = driver.EMAIL,
                                                   NGAYSUA = driver.NGAYSUA,
                                                   CCTC_THANHPHAN_ID = driver.CCTC_THANHPHAN_ID,
                                               });
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.HOTEN))
                {
                    searchModel.HOTEN = searchModel.HOTEN.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.HOTEN) && x.HOTEN.Trim().ToLower().Contains(searchModel.HOTEN));
                }
                if (!string.IsNullOrEmpty(searchModel.CMND))
                {
                    searchModel.CMND = searchModel.CMND.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.CMND) && x.CMND.Trim().ToLower().Contains(searchModel.CMND));
                }

                if (!string.IsNullOrEmpty(searchModel.SODIENTHOAI))
                {
                    searchModel.SODIENTHOAI = searchModel.SODIENTHOAI.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.SODIENTHOAI) && x.SODIENTHOAI.Trim().ToLower().Contains(searchModel.SODIENTHOAI));
                }

                if (!string.IsNullOrEmpty(searchModel.EMAIL))
                {
                    searchModel.EMAIL = searchModel.EMAIL.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.EMAIL) && x.EMAIL.Trim().ToLower().Contains(searchModel.EMAIL));

                }

                if (searchModel.GIOITINH != null)
                {
                    queryResult = queryResult.Where(x => x.GIOITINH == searchModel.GIOITINH.Value);

                }
                if(searchModel.CCTC_THANHPHAN_ID != null)
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

            PageListResultBO<LaiXeBO> result = new PageListResultBO<LaiXeBO>();
            IPagedList<LaiXeBO> pagedList = queryResult.ToPagedList(pageIndex, pageSize);
            result.Count = pagedList.TotalItemCount;
            result.TotalPage = pagedList.PageCount;
            result.ListItem = pagedList.ToList();

            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @since: 28/08/2018
        /// @description: lấy danh sách tài xế đang có sẵn
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownAvailableDrivers()
        {
            List<SelectListItem> result = (from drivers in this.context.QL_LAIXE.Where(x => x.IS_DELETE != true)
                                           select new SelectListItem()
                                           {
                                               Value = drivers.ID.ToString(),
                                               Text = drivers.HOTEN
                                           }).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách lái xe đang có sẵn để phục vụ cho chuyến
        /// </summary>
        /// <param name="registerId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownAvailableDriversForTrip(long registerId)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            QL_DANGKY_XE registration = this.context.QL_DANGKY_XE.Find(registerId);
            if(registration != null && registration.IS_DELETE != true && registration.TRANGTHAI != TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID)
            {
                //lấy danh sách mã lái xe đang chạy
                List<int> drivingDriverIds = this.context.QL_DANGKYXE_LAIXE
                    .Where(x=>x.CCTC_THANHPHAN_ID == registration.CCTC_THANHPHAN_ID)
                    .Where(x => x.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_ID && x.LAIXE_ID != null)
                    .Select(x => x.LAIXE_ID.Value)
                    .ToList();

                //lấy danh sách mã lái xe có cùng giờ và ngày chạy chuyến
                List<int> sameTimeDriverIds = (from driver in this.context.QL_LAIXE.Where(x => x.CCTC_THANHPHAN_ID == registration.CCTC_THANHPHAN_ID)
                                               join trip in this.context.QL_DANGKYXE_LAIXE
                                               on driver.ID equals trip.LAIXE_ID
                                               join register in this.context.QL_DANGKY_XE.Where(x => x.NGAY_XUATPHAT != null && x.GIO_XUATPHAT != null)
                                               .Where(x => x.NGAY_XUATPHAT == registration.NGAY_XUATPHAT && x.GIO_XUATPHAT == x.GIO_XUATPHAT)
                                               on trip.QL_DANGKY_XE_ID equals register.ID
                                               select driver.ID).ToList();
                List<int> notAvailableDriverIds = new List<int>();
                notAvailableDriverIds.AddRange(drivingDriverIds);
                notAvailableDriverIds.AddRange(sameTimeDriverIds);

                result = (from drivers in this.context.QL_LAIXE.Where(x => x.CCTC_THANHPHAN_ID == registration.CCTC_THANHPHAN_ID)
                          .Where(x => x.IS_DELETE != true && notAvailableDriverIds.Contains(x.ID) == false)
                          select new SelectListItem()
                          {
                              Value = drivers.ID.ToString(),
                              Text = drivers.HOTEN
                          }).ToList();
            }
            return result;
        }
    }
}
