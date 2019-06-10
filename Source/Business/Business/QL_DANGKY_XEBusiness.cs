using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLDANGKYXE;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class QL_DANGKY_XEBusiness : BaseBusiness<QL_DANGKY_XE>
    {
        public QL_DANGKY_XEBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public int? LOAIXECONSTANT { get; private set; }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách đăng ký xe
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<DangKyXeBO> GetDataByPage(DangKyXeSearchBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            IQueryable<DangKyXeBO> queryResult = (from register in this.context.QL_DANGKY_XE.Where(x => x.IS_DELETE != true)
                                                  join user in this.context.DM_NGUOIDUNG
                                                  on register.NGUOITAO equals user.ID
                                                  into group1
                                                  from g1 in group1.DefaultIfEmpty()
                                                  select new DangKyXeBO()
                                                  {
                                                      ID = register.ID,
                                                      SONGUOI = register.SONGUOI,
                                                      MUCDICH = register.MUCDICH,
                                                      NGAY_XUATPHAT = register.NGAY_XUATPHAT,
                                                      GIO_XUATPHAT = register.GIO_XUATPHAT,
                                                      PHUT_XUATPHAT = register.PHUT_XUATPHAT,
                                                      NGAYSUA = register.NGAYSUA,
                                                      TRANGTHAI = register.TRANGTHAI ?? 0,
                                                      NGUOITAO = register.NGUOITAO,
                                                      NGUOIDANGKY = g1.HOTEN,
                                                      CCTC_THANHPHAN_ID = register.CCTC_THANHPHAN_ID
                                                  });
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.MUCDICH))
                {
                    searchModel.MUCDICH = searchModel.MUCDICH.Trim().ToLower();
                    queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.MUCDICH) && x.MUCDICH.Trim().ToLower().Contains(searchModel.MUCDICH));
                }

                if (searchModel.queryThoiGianXuatPhatStart != null)
                {
                    queryResult = queryResult.Where(x => x.NGAY_XUATPHAT >= searchModel.queryThoiGianXuatPhatStart);
                }
                if (searchModel.queryThoiGianXuatPhatEnd != null)
                {
                    queryResult = queryResult.Where(x => x.NGAY_XUATPHAT <= searchModel.queryThoiGianXuatPhatEnd);
                }

                if (searchModel.querySoNguoiStart != null)
                {
                    queryResult = queryResult.Where(x => x.SONGUOI >= searchModel.querySoNguoiStart);
                }
                if (searchModel.querySoNguoiEnd != null)
                {
                    queryResult = queryResult.Where(x => x.SONGUOI <= searchModel.querySoNguoiEnd);
                }

                if (searchModel.CCTC_THANHPHAN_ID != null)
                {
                    queryResult = queryResult.Where(x => x.CCTC_THANHPHAN_ID == searchModel.CCTC_THANHPHAN_ID.Value);
                }

                if (searchModel.HAS_ROLE_CONFIRM)
                {
                    if (searchModel.TRANGTHAI != null && searchModel.USER_ID != null)
                    {
                        queryResult = queryResult.Where(x => x.TRANGTHAI != searchModel.TRANGTHAI || x.NGUOITAO == searchModel.USER_ID);
                    }
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
            PageListResultBO<DangKyXeBO> result = new PageListResultBO<DangKyXeBO>();
            IPagedList<DangKyXeBO> pagedList = queryResult.ToPagedList(pageIndex, pageSize);
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
                    case TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_ID:
                        x.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_TEXT;
                        x.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_COLOR;
                        break;
                    default:
                        break;
                }
            });
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy thông tin chi tiết của đăng ký xe
        /// @since: 28/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DangKyXeBO GetDetail(long id)
        {
            DangKyXeBO result = (from register in this.context.QL_DANGKY_XE
                                 .Where(x => x.IS_DELETE != true && x.ID == id)
                                 join user in this.context.DM_NGUOIDUNG
                                 on register.NGUOITAO equals user.ID
                                 into group1
                                 from g1 in group1.DefaultIfEmpty()

                                 join department in this.context.CCTC_THANHPHAN
                                 on register.PHONGBAN_ID equals department.ID
                                 into group2
                                 from g2 in group2.DefaultIfEmpty()

                                 join leader in this.context.DM_NGUOIDUNG
                                 on register.CANBO_ID equals leader.ID
                                 into group3
                                 from g3 in group3.DefaultIfEmpty()
                                 select new DangKyXeBO()
                                 {
                                     ID = register.ID,
                                     CANBO_ID = register.CANBO_ID,
                                     TEN_BENHNHAN = register.TEN_BENHNHAN,
                                     SONGUOI = register.SONGUOI,
                                     MUCDICH = register.MUCDICH,
                                     NOIDUNG = register.NOIDUNG,
                                     DIEM_XUATPHAT = register.DIEM_XUATPHAT,
                                     DIEM_KETTHUC = register.DIEM_KETTHUC,
                                     NGAY_XUATPHAT = register.NGAY_XUATPHAT,
                                     GIO_XUATPHAT = register.GIO_XUATPHAT,
                                     PHUT_XUATPHAT = register.PHUT_XUATPHAT,
                                     NGAYSUA = register.NGAYSUA,
                                     NGUOIDANGKY = g1.HOTEN,
                                     PHONGBAN_DANGKY = g2.NAME,
                                     IS_BHYT = register.IS_BHYT,
                                     LOAI_CHUYEN_ID = register.LOAI_CHUYEN_ID,
                                     TRANGTHAI = register.TRANGTHAI ?? 0,
                                     NGUOITAO = register.NGUOITAO,
                                     LICHCONGTAC_ID = register.LICHCONGTAC_ID,
                                     TEN_CANBO = g3.HOTEN,
                                     CCTC_THANHPHAN_ID = register.CCTC_THANHPHAN_ID
                                 }).FirstOrDefault();
            if (result != null)
            {
                if (result.LOAI_CHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN)
                {
                    result.TENLOAI_CHUYEN = TENLOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN;
                }
                else if (result.LOAI_CHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_VE)
                {
                    result.TENLOAI_CHUYEN = TENLOAICHUYEN_CONSTANT.CHUYEN_VE;
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
                    case TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_COLOR;
                        break;
                    case TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_ID:
                        result.TEN_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_TEXT;
                        result.MAU_TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_COLOR;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy các đăng ký xe có sẵn cho lịch công tác (chưa bị xóa - chưa bị hủy)
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public QL_DANGKY_XE GetAvailableRegistrationByCalendarId(long calendarId)
        {
            QL_DANGKY_XE result = this.context.QL_DANGKY_XE
                .Where(x => x.IS_DELETE != true
                && x.TRANGTHAI != TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID
                && x.LICHCONGTAC_ID == calendarId)
                .OrderByDescending(x => x.ID).FirstOrDefault();
            return result;
        }
    }
}
