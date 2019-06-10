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
//using Business.CommonModel.CCTCTHANHPHAN;
using System.Collections;
using System.Web.Mvc;
using Business.CommonModel.LICH_CONGTAC;
using CommonHelper;
using CommonHelper.DateExtend;
using Business.CommonModel.CONSTANT;

namespace Business.Business
{
    public class LICHCONGTACBusiness : BaseBusiness<LICHCONGTAC>
    {
        public LICHCONGTACBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(LICHCONGTAC calendar)
        {
            try
            {
                if (calendar.ID == 0)
                {
                    calendar.IS_DELETE = false;
                    this.repository.Insert(calendar);
                }
                else
                {
                    this.repository.Update(calendar);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra trùng lịch cán bộ trong ngày
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public bool CheckIsDuplicate(long calendarId, long userId, DateTime day, int hour, int minute)
        {
            int count = this.context.LICHCONGTAC.Where(x => x.IS_DELETE != true)
                .Where(x => x.LANHDAO_ID == userId && x.NGAY_CONGTAC == day
                    && x.GIO_CONGTAC == hour && x.PHUT_CONGTAC == minute
                    && x.ID != calendarId && x.IS_LATTEST == true)
                .Count();
            bool existed = count > 0;
            return existed;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách lịch công tác
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<LICHCONGTAC_BO> GetListLichCongTacs(LICHCONGTAC_SEARCH searchModel)
        {
            IQueryable<LICHCONGTAC_BO> queryResult = (from calendar in this.context.LICHCONGTAC.Where(x => x.IS_DELETE != true
                                                      && x.IS_LATTEST == true)
                                                      join leader in this.context.DM_NGUOIDUNG
                                                      on calendar.LANHDAO_ID equals leader.ID
                                                      orderby calendar.NGAY_CONGTAC descending, calendar.GIO_CONGTAC descending
                                                      select new LICHCONGTAC_BO()
                                                      {
                                                          ID = calendar.ID,
                                                          TIEUDE = calendar.TIEUDE,
                                                          NGAY_CONGTAC = calendar.NGAY_CONGTAC,
                                                          GIO_CONGTAC = calendar.GIO_CONGTAC,
                                                          PHUT_CONGTAC = calendar.PHUT_CONGTAC,
                                                          DIADIEM = calendar.DIADIEM,
                                                          NGUOITAO = calendar.NGUOITAO ?? 0,
                                                          LICH_GOC_ID = calendar.LICH_GOC_ID,
                                                          LANHDAO_ID = calendar.LANHDAO_ID ?? 0,
                                                          TEN_LANHDAO = leader.HOTEN,
                                                          NGUOICHUTRI_ID = calendar.NGUOICHUTRI_ID,
                                                          GHICHU = calendar.GHICHU
                                                      });
            if (searchModel.startDate != null)
            {
                queryResult = queryResult.Where(x => x.NGAY_CONGTAC >= searchModel.startDate);
            }
            if (searchModel.endDate != null)
            {
                queryResult = queryResult.Where(x => x.NGAY_CONGTAC <= searchModel.endDate);
            }
            if (searchModel.leaderId != null)
            {
                queryResult = queryResult.Where(x => x.LANHDAO_ID == searchModel.leaderId);
            }

            if (searchModel.queryDeptId != null)
            {
                IQueryable<int> listSameParentDeptIds = this.context.CCTC_THANHPHAN.Where(x => x.PARENT_ID == searchModel.queryDeptId.Value)
                    .Select(x => x.ID);
                IQueryable<long> listUserIdsHasSameParentDepts = this.context.DM_NGUOIDUNG
                    .Where(x => x.DM_PHONGBAN_ID != null && listSameParentDeptIds.Contains(x.DM_PHONGBAN_ID.Value))
                    .Select(x => x.ID);
                queryResult = queryResult.Where(x => listUserIdsHasSameParentDepts.Contains(x.LANHDAO_ID.Value));
            }

            List<LICHCONGTAC_BO> result = queryResult.ToList();


            //danh sách lịch công tác có đã đăng ký xe
            List<long> registeredCarCalendars = (from calendar in this.context.LICHCONGTAC.Where(x => x.IS_DELETE != true)
                                                 join register in this.context.QL_DANGKY_XE.Where(x => x.IS_DELETE != true
                                                 && x.TRANGTHAI != TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID)
                                                 on calendar.ID equals register.LICHCONGTAC_ID
                                                 select calendar.ID).ToList();
            result.ForEach(x =>
            {
                x.IS_OLD_WEEK = x.NGAY_CONGTAC.IsOldWeek();
                x.IS_REGISTERED_CAR = registeredCarCalendars.Contains(x.ID);
            });
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách những lịch công tác đã bị xóa
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<LICHCONGTAC_BO> GetListDeletedLichCongTacs(LICHCONGTAC_SEARCH searchModel)
        {
            IQueryable<LICHCONGTAC_BO> queryResult = (from calendar in this.context.LICHCONGTAC.Where(x => x.IS_DELETE == true
                                                      && x.IS_LATTEST == true)
                                                      orderby calendar.NGAY_CONGTAC descending, calendar.GIO_CONGTAC descending
                                                      select new LICHCONGTAC_BO()
                                                      {
                                                          ID = calendar.ID,
                                                          TIEUDE = calendar.TIEUDE,
                                                          NGAY_CONGTAC = calendar.NGAY_CONGTAC,
                                                          GIO_CONGTAC = calendar.GIO_CONGTAC,
                                                          PHUT_CONGTAC = calendar.PHUT_CONGTAC,
                                                          DIADIEM = calendar.DIADIEM,
                                                          NGUOITAO = calendar.NGUOITAO ?? 0,
                                                          LICH_GOC_ID = calendar.LICH_GOC_ID,
                                                          GHICHU = calendar.GHICHU,
                                                          LANHDAO_ID = calendar.LANHDAO_ID
                                                      });
            if (searchModel.startDate != null)
            {
                queryResult = queryResult.Where(x => x.NGAY_CONGTAC >= searchModel.startDate);
            }
            if (searchModel.endDate != null)
            {
                queryResult = queryResult.Where(x => x.NGAY_CONGTAC <= searchModel.endDate);
            }
            if (searchModel.leaderId != null)
            {
                queryResult = queryResult.Where(x => x.LANHDAO_ID == searchModel.leaderId);
            }
            List<LICHCONGTAC_BO> result = queryResult.ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy các năm có lịch công tác
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public List<SelectListItem> GetListYears(DateTime selected)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            List<int> years = this.context.LICHCONGTAC.OrderByDescending(x => x.NGAY_CONGTAC.Year)
                .Select(x => x.NGAY_CONGTAC.Year).Distinct().ToList();
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

        /// <summary>
        /// @author: duynn
        /// @description: thông tin chi tiết lịch công tác
        /// @since: 15/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LICHCONGTAC_BO GetDetailLichCongTac(long id)
        {
            LICHCONGTAC_BO result = new LICHCONGTAC_BO();

            LICHCONGTAC dbEntity = this.context.LICHCONGTAC.Find(id);
            if (dbEntity != null)
            {
                result.ID = dbEntity.ID;
                result.TIEUDE = dbEntity.TIEUDE;
                result.DIADIEM = dbEntity.DIADIEM;
                result.GHICHU = dbEntity.GHICHU;
                result.NGAY_CONGTAC_TEXT = dbEntity.GIO_CONGTAC + ":" + dbEntity.PHUT_CONGTAC + " " + string.Format("{0:dd/MM/yyyy}", dbEntity.NGAY_CONGTAC);
                result.TEN_LANHDAO = this.context.DM_NGUOIDUNG.Where(x => x.ID == dbEntity.LANHDAO_ID).Select(x => x.HOTEN).FirstOrDefault();
            }
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy lịch công tác của người dùng theo từng ngày
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<LICHCONGTAC_BO> GetLichCongTacOfUserByWeek(long userId, DateTime date)
        {
            List<LICHCONGTAC_BO> result = (from calendar in this.context.LICHCONGTAC
                                           .Where(x => x.LANHDAO_ID == userId)
                                           .Where(x => x.IS_DELETE != true && x.IS_LATTEST == true)
                                           select new LICHCONGTAC_BO()
                                           {
                                               ID = calendar.ID,
                                               TIEUDE = calendar.TIEUDE,
                                               NGAY_CONGTAC = calendar.NGAY_CONGTAC,
                                               GIO_CONGTAC = calendar.GIO_CONGTAC,
                                               PHUT_CONGTAC = calendar.PHUT_CONGTAC,
                                               DIADIEM = calendar.DIADIEM,
                                               NGUOITAO = calendar.NGUOITAO ?? 0,
                                               GHICHU = calendar.GHICHU,
                                           }).ToList();
            result = result.Where(x => x.NGAY_CONGTAC.GetWeekNumber() == date.GetWeekNumber()).ToList();
            result = result.OrderBy(x => x.GIO_CONGTAC)
                .ThenBy(x => x.PHUT_CONGTAC)
                .ToList();
            return result;
        }
    }
}

