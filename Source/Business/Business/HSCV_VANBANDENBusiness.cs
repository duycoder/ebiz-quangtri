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
using Business.CommonModel.HSCVVANBANDEN;
using Business.CommonModel.HSCVVANBANDENDONVINHAN;
using System.Reflection;
using Business.CommonModel.CONSTANT;
using System.Web.Mvc;
using System.Data.Entity.SqlServer;

namespace Business.Business
{
    public class HSCV_VANBANDENBusiness : BaseBusiness<HSCV_VANBANDEN>
    {
        public HSCV_VANBANDENBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public PageListResultBO<HSCV_VANBANDEN_BO> GetDaTaByPage(HSCV_VANBANDEN_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var ListJobId = new List<long?>();
            var query = from vanban in this.context.HSCV_VANBANDEN
                        join loaivanban in this.context.DM_DANHMUC_DATA
                        on vanban.LOAIVANBAN_ID equals loaivanban.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join linhvuc in this.context.DM_DANHMUC_DATA
                        on vanban.LINHVUCVANBAN_ID equals linhvuc.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()

                        join dokhan in this.context.DM_DANHMUC_DATA
                        on vanban.DOKHAN_ID equals dokhan.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join domat in this.context.DM_DANHMUC_DATA
                        on vanban.DOMAT_ID equals domat.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoidung in this.context.DM_NGUOIDUNG
                        on vanban.NGUOITAO equals nguoidung.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        select new HSCV_VANBANDEN_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            DOMAT_ID = vanban.DOMAT_ID,
                            ID = vanban.ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            NGAYSUA = vanban.NGAYSUA,
                            NGAYTAO = vanban.NGAYTAO,
                            NGUOIKY = vanban.NGUOIKY,
                            NGUOISUA = vanban.NGUOISUA,
                            NGUOITAO = vanban.NGUOITAO,
                            NOIDUNG = vanban.NOIDUNG,
                            SOHIEU = vanban.SOHIEU,
                            SOTRANG = vanban.SOTRANG,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOMAT = g4.TEXT,
                            TEN_HINHTHUC = g1.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOMAT = g4.ICON,
                            ICON_HINHTHUC = g1.ICON,
                            ICON_LINHVUC = g2.ICON,
                            TEN_NGUOITAO = g5.HOTEN,
                            TRICHYEU = vanban.TRICHYEU,
                            DONVI_ID = vanban.DONVI_ID,
                            VANBANDI_ID = vanban.VANBANDI_ID,
                            NGAYHET_HIEULUC = vanban.NGAYHET_HIEULUC,
                            NGAY_BANHANH = vanban.NGAY_BANHANH,
                            NGAY_HIEULUC = vanban.NGAY_HIEULUC,
                            NGAY_VANBAN = vanban.NGAY_VANBAN,
                            COLOR = g3.COLOR,
                            ListDonViNhan = (from receiver in this.context.HSCV_VANBANDEN_DONVINHAN
                                             join unit in this.context.CCTC_THANHPHAN
                                             on receiver.DONVI_ID equals unit.ID
                                             where receiver.DONVI_ID.HasValue && receiver.VANBANDEN_ID == vanban.ID
                                             select new HSCV_VANBANDEN_DONVINHAN_BO
                                             {
                                                 DONVI_ID = receiver.DONVI_ID,
                                                 ID = receiver.ID,
                                                 TEN_DONVI = unit.NAME,
                                                 VANBANDEN_ID = receiver.VANBANDEN_ID
                                             }).ToList()
                            //TEN_DONVI = g6.NAME
                        };
            if (searchModel != null)
            {
                #region Tìm kiếm
                if (searchModel.isMobileFilter && !string.IsNullOrEmpty(searchModel.mobileQuery))
                {
                    query = query.Where(x => (x.SOHIEU != null && x.SOHIEU.ToLower().Contains(searchModel.mobileQuery.ToLower().Trim())) ||
                    (x.TRICHYEU != null && x.TRICHYEU.ToLower().Contains(searchModel.mobileQuery.ToLower().Trim())));
                }
                //Tìm kiếm theo đơn vị
                if (searchModel.ListDonVi != null && searchModel.ListDonVi.Any())
                {
                    ListJobId = (from receiver in this.context.HSCV_VANBANDEN_DONVINHAN
                                 where receiver.DONVI_ID.HasValue && searchModel.ListDonVi.Contains(receiver.DONVI_ID.Value)
                                 select receiver.VANBANDEN_ID).ToList();
                    //query = query.Where(x => (x.DONVI_ID.HasValue && searchModel.ListDonVi.Contains(x.DONVI_ID.Value)));
                    query = query.Where(x => ListJobId.Contains(x.ID));
                }
                else
                {
                    query = query.Where(x => x.NGUOITAO.HasValue && x.NGUOITAO.Value == searchModel.USER_ID && !x.VANBANDI_ID.HasValue);
                }
                if (!string.IsNullOrEmpty(searchModel.SOHIEU))
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.SOHIEU) && x.SOHIEU.ToLower().Trim().Contains(searchModel.SOHIEU.Trim()));
                }
                if (!string.IsNullOrEmpty(searchModel.TRICHYEU))
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.TRICHYEU) && x.TRICHYEU.ToLower().Trim().Contains(searchModel.TRICHYEU));
                }
                if (searchModel.LOAIVANBAN_ID.HasValue)
                {
                    query = query.Where(x => x.LOAIVANBAN_ID.HasValue && x.LOAIVANBAN_ID.Value == searchModel.LOAIVANBAN_ID);
                }
                if (searchModel.LINHVUCVANBAN_ID.HasValue)
                {
                    query = query.Where(x => x.LINHVUCVANBAN_ID.HasValue && x.LINHVUCVANBAN_ID.Value == searchModel.LINHVUCVANBAN_ID);
                }
                if (searchModel.DOMAT_ID.HasValue)
                {
                    query = query.Where(x => x.DOMAT_ID.HasValue && x.DOMAT_ID.Value == searchModel.DOMAT_ID);
                }
                if (searchModel.DOKHAN_ID.HasValue)
                {
                    query = query.Where(x => x.DOKHAN_ID.HasValue && x.DOKHAN_ID.Value == searchModel.DOKHAN_ID);
                }
                if (!string.IsNullOrEmpty(searchModel.NGUOIKY))
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.NGUOIKY) && x.NGUOIKY.ToLower().Trim().Contains(searchModel.NGUOIKY));
                }
                if (searchModel.NGAYBANHANH_TU.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYBANHANH_TU <= x.NGAY_BANHANH);
                }
                if (searchModel.NGAYBANHANH_DEN.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYBANHANH_DEN >= x.NGAY_BANHANH);
                }
                #endregion
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.NGAYTAO);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.NGAYTAO);
            }
            var resultmodel = new PageListResultBO<HSCV_VANBANDEN_BO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            //foreach (var item in resultmodel.ListItem)
            //{
            //    //Danh sách đơn vị nhận được văn bản này
            //    var ListDonVi = (from vanban in this.context.HSCV_VANBANDEN_DONVINHAN
            //                     join cctc in this.context.CCTC_THANHPHAN
            //                     on vanban.DONVI_ID equals cctc.ID
            //                     where vanban.VANBANDEN_ID == item.ID
            //                     select cctc).ToList();
            //    item.ListDonVi = ListDonVi;
            //    int length = ListDonVi.Count;
            //}
            return resultmodel;
        }
        public HSCV_VANBANDEN_BO FindById(long id)
        {
            var query = from vanban in this.context.HSCV_VANBANDEN
                        join loaivanban in this.context.DM_DANHMUC_DATA
                        on vanban.LOAIVANBAN_ID equals loaivanban.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join linhvuc in this.context.DM_DANHMUC_DATA
                        on vanban.LINHVUCVANBAN_ID equals linhvuc.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()

                        join dokhan in this.context.DM_DANHMUC_DATA
                        on vanban.DOKHAN_ID equals dokhan.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join domat in this.context.DM_DANHMUC_DATA
                        on vanban.DOMAT_ID equals domat.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoidung in this.context.DM_NGUOIDUNG
                        on vanban.NGUOITAO equals nguoidung.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()
                            //join vbdi in this.context.HSCV_VANBANDI
                            //on vanban.VANBANDI_ID equals vbdi.ID
                            //into group6
                            //from g6 in group6.DefaultIfEmpty()
                        where vanban.ID == id
                        select new HSCV_VANBANDEN_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            DOMAT_ID = vanban.DOMAT_ID,
                            ID = vanban.ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            NGAYSUA = vanban.NGAYSUA,
                            NGAYTAO = vanban.NGAYTAO,
                            NGUOIKY = vanban.NGUOIKY,
                            NGUOISUA = vanban.NGUOISUA,
                            NGUOITAO = vanban.NGUOITAO,
                            NOIDUNG = vanban.NOIDUNG,
                            SOHIEU = vanban.SOHIEU,
                            SOTRANG = vanban.SOTRANG,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOMAT = g4.TEXT,
                            TEN_HINHTHUC = g1.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_NGUOITAO = g5.HOTEN,
                            TRICHYEU = vanban.TRICHYEU,
                            DONVI_ID = vanban.DONVI_ID,
                            VANBANDI_ID = vanban.VANBANDI_ID,
                            //VANBANDI_SOHIEU = g6.SOHIEU,
                            //VANBANDI_TRICHYEU = g6.TRICHYEU,
                            NGAYHET_HIEULUC = vanban.NGAYHET_HIEULUC,
                            NGAY_BANHANH = vanban.NGAY_BANHANH,
                            NGAY_HIEULUC = vanban.NGAY_HIEULUC,
                            NGAY_VANBAN = vanban.NGAY_VANBAN,
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// @author: duynn
        /// @since: 08/08/2018
        /// @description: văn bản đến đang xử lý
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDEN_BO> GetListInProcess(HSCV_VANBANDEN_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            IQueryable<HSCV_VANBANDEN_BO> queryResult = from vanban in this.context.HSCV_VANBANDEN
                                                        join loaivanban in this.context.DM_DANHMUC_DATA
                                                        on vanban.LOAIVANBAN_ID equals loaivanban.ID
                                                        into group1
                                                        from g1 in group1.DefaultIfEmpty()

                                                        join douutien in this.context.DM_DANHMUC_DATA
                                                        on vanban.DOMAT_ID equals douutien.ID
                                                        into group2
                                                        from g2 in group2.DefaultIfEmpty()

                                                        join dokhan in this.context.DM_DANHMUC_DATA
                                                        on vanban.DOKHAN_ID equals dokhan.ID
                                                        into group3
                                                        from g3 in group3.DefaultIfEmpty()

                                                        join linhvuc in this.context.DM_DANHMUC_DATA
                                                        on vanban.LINHVUCVANBAN_ID equals linhvuc.ID
                                                        into group4
                                                        from g4 in group4.DefaultIfEmpty()

                                                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBANDEN_ID equals sovb.ID
                                                        into group5
                                                        from g5 in group5.DefaultIfEmpty()

                                                        join donvi in this.context.DM_DANHMUC_DATA
                                                        on vanban.DONVI_ID equals donvi.ID
                                                        into group6
                                                        from g6 in group6.DefaultIfEmpty()

                                                        join wf in this.context.WF_PROCESS
                                                        on vanban.ID equals wf.ITEM_ID
                                                        where wf.ITEM_TYPE == searchModel.ITEM_TYPE && searchModel.USER_ID == wf.USER_ID && wf.IS_END != true

                                                        select new HSCV_VANBANDEN_BO()
                                                        {
                                                            ID = vanban.ID,
                                                            SOHIEU = vanban.SOHIEU,
                                                            TRICHYEU = vanban.TRICHYEU,
                                                            NGAY_BANHANH = vanban.NGAY_BANHANH,
                                                            NGAY_VANBAN = vanban.NGAY_VANBAN,
                                                            NGAYHET_HIEULUC = vanban.NGAYHET_HIEULUC,
                                                            NGAY_HIEULUC = vanban.NGAY_HIEULUC,

                                                            TEN_HINHTHUC = g1.TEXT,
                                                            LOAIVANBAN_ID = g1.ID,

                                                            TEN_DOMAT = g2.TEXT,
                                                            DOMAT_ID = g2.ID,

                                                            TEN_DOKHAN = g3.TEXT,
                                                            DOKHAN_ID = g3.ID,
                                                            ICON_DOKHAN = g3.ICON,

                                                            TEN_LINHVUC = g4.TEXT,
                                                            LINHVUCVANBAN_ID = g4.ID,

                                                            TENSOVANBAN = g5.TEXT,
                                                            SODITHEOSO = vanban.SODITHEOSO,
                                                            SODITHEOSO_NUMBER = vanban.SODITHEOSO_NUMBER ?? 0,
                                                            SOVANBANDEN_ID = vanban.SOVANBANDEN_ID,
                                                            Update_At = wf.UPDATED_AT,

                                                            TEN_DONVI = g6.TEXT,
                                                            SOTRANG = vanban.SOTRANG,
                                                            NGUOIKY = vanban.NGUOIKY,
                                                            NGUOITAO = vanban.NGUOITAO,
                                                            NGAYTAO = vanban.NGAYTAO,
                                                            CHUCVU = vanban.CHUCVU,
                                                            IS_NOIBO = vanban.IS_NOIBO == true ? true : false,
                                                            PROCESS_USER_ID = wf.USER_ID,
                                                        };
            queryResult = queryResult.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDEN_BO> result = GetQueryResultAfterPaging(queryResult, searchModel, pageSize, pageIndex);
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @since: 06/08/2018
        /// @description: danh sách văn bản đến đã xử lý
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDEN_BO> GetListProcessed(HSCV_VANBANDEN_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            IQueryable<HSCV_VANBANDEN_BO> queryResult = from vanban in this.context.HSCV_VANBANDEN
                                                        join loaivanban in this.context.DM_DANHMUC_DATA
                                                        on vanban.LOAIVANBAN_ID equals loaivanban.ID
                                                        into group1
                                                        from g1 in group1.DefaultIfEmpty()

                                                        join douutien in this.context.DM_DANHMUC_DATA
                                                        on vanban.DOMAT_ID equals douutien.ID
                                                        into group2
                                                        from g2 in group2.DefaultIfEmpty()

                                                        join dokhan in this.context.DM_DANHMUC_DATA
                                                        on vanban.DOKHAN_ID equals dokhan.ID
                                                        into group3
                                                        from g3 in group3.DefaultIfEmpty()

                                                        join linhvuc in this.context.DM_DANHMUC_DATA
                                                        on vanban.LINHVUCVANBAN_ID equals linhvuc.ID
                                                        into group4
                                                        from g4 in group4.DefaultIfEmpty()

                                                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBANDEN_ID equals sovb.ID
                                                        into group5
                                                        from g5 in group5.DefaultIfEmpty()

                                                        join donvi in this.context.DM_DANHMUC_DATA
                                                        on vanban.DONVI_ID equals donvi.ID
                                                        into group6
                                                        from g6 in group6.DefaultIfEmpty()

                                                        join wfp in context.WF_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE)
                                                        on vanban.ID equals wfp.ITEM_ID
                                                        into group7
                                                        from gwfp_vanban in group7.DefaultIfEmpty()

                                                        join wf in context.WF_ITEM_USER_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE
                                                        && searchModel.USER_ID == x.USER_ID && true == x.IS_XULYCHINH && true == x.DAXULY)
                                                        on vanban.ID equals wf.ITEM_ID
                                                        select new HSCV_VANBANDEN_BO()
                                                        {
                                                            ID = vanban.ID,
                                                            SOHIEU = vanban.SOHIEU,
                                                            TRICHYEU = vanban.TRICHYEU,
                                                            TEN_HINHTHUC = g1.TEXT,
                                                            LOAIVANBAN_ID = g1.ID,

                                                            TEN_DOMAT = g2.TEXT,
                                                            DOMAT_ID = g2.ID,

                                                            TEN_DOKHAN = g3.TEXT,
                                                            DOKHAN_ID = g3.ID,
                                                            ICON_DOKHAN = g3.ICON,

                                                            TEN_LINHVUC = g4.TEXT,
                                                            LINHVUCVANBAN_ID = g4.ID,
                                                            SOTRANG = vanban.SOTRANG,
                                                            TENSOVANBAN = g5.TEXT,
                                                            TEN_DONVI = g6.TEXT,
                                                            SODITHEOSO = vanban.SODITHEOSO,
                                                            SODITHEOSO_NUMBER = vanban.SODITHEOSO_NUMBER ?? 0,
                                                            SOVANBANDEN_ID = vanban.SOVANBANDEN_ID,
                                                            NGUOIKY = vanban.NGUOIKY,
                                                            NGUOITAO = vanban.NGUOITAO,
                                                            NGAYTAO = vanban.NGAYTAO,
                                                            CHUCVU = vanban.CHUCVU,
                                                            PROCESS_USER_ID = wf.USER_ID,
                                                            IS_NOIBO = vanban.IS_NOIBO == true ? true : false,
                                                            NGAY_BANHANH = vanban.NGAY_BANHANH,
                                                            NGAY_VANBAN = vanban.NGAY_VANBAN,
                                                            NGAY_HIEULUC = vanban.NGAY_HIEULUC,
                                                            NGAYHET_HIEULUC = vanban.NGAYHET_HIEULUC
                                                        };
            queryResult = queryResult.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDEN_BO> result = GetQueryResultAfterPaging(queryResult, searchModel, pageSize, pageIndex);
            return result;
        }

        /// <summary>
        /// @description: danh sách văn bản đến tham gia xử lý
        /// @author: duynn
        /// @since: 06/08/2018
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDEN_BO> GetListJoinProcess(HSCV_VANBANDEN_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            IQueryable<HSCV_VANBANDEN_BO> queryResult = from vanban in this.context.HSCV_VANBANDEN
                                                        join loaivanban in this.context.DM_DANHMUC_DATA
                                                        on vanban.LOAIVANBAN_ID equals loaivanban.ID
                                                        into group1
                                                        from g1 in group1.DefaultIfEmpty()

                                                        join douutien in this.context.DM_DANHMUC_DATA
                                                        on vanban.DOMAT_ID equals douutien.ID
                                                        into group2
                                                        from g2 in group2.DefaultIfEmpty()

                                                        join dokhan in this.context.DM_DANHMUC_DATA
                                                        on vanban.DOKHAN_ID equals dokhan.ID
                                                        into group3
                                                        from g3 in group3.DefaultIfEmpty()

                                                        join linhvuc in this.context.DM_DANHMUC_DATA
                                                        on vanban.LINHVUCVANBAN_ID equals linhvuc.ID
                                                        into group4
                                                        from g4 in group4.DefaultIfEmpty()

                                                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBANDEN_ID equals sovb.ID
                                                        into group5
                                                        from g5 in group5.DefaultIfEmpty()

                                                        join donvi in this.context.DM_DANHMUC_DATA
                                                        on vanban.DONVI_ID equals donvi.ID
                                                        into group6
                                                        from g6 in group6.DefaultIfEmpty()

                                                        join wf in context.WF_ITEM_USER_PROCESS.Where(x => x.ITEM_TYPE.Equals(searchModel.ITEM_TYPE)
                                                        && searchModel.USER_ID == x.USER_ID && x.IS_XULYCHINH == false)
                                                        on vanban.ID equals wf.ITEM_ID
                                                        select new HSCV_VANBANDEN_BO()
                                                        {
                                                            ID = vanban.ID,
                                                            SOHIEU = vanban.SOHIEU,
                                                            TRICHYEU = vanban.TRICHYEU,
                                                            NGAY_BANHANH = vanban.NGAY_BANHANH,
                                                            NGAY_VANBAN = vanban.NGAY_VANBAN,
                                                            NGAY_HIEULUC = vanban.NGAY_HIEULUC,
                                                            NGAYHET_HIEULUC = vanban.NGAYHET_HIEULUC,

                                                            TEN_HINHTHUC = g1.TEXT,
                                                            LOAIVANBAN_ID = g1.ID,

                                                            TEN_DOMAT = g2.TEXT,
                                                            DOMAT_ID = g2.ID,
                                                            SOTRANG = vanban.SOTRANG,
                                                            TEN_DOKHAN = g3.TEXT,
                                                            DOKHAN_ID = g3.ID,
                                                            ICON_DOKHAN = g3.ICON,

                                                            TEN_LINHVUC = g4.TEXT,
                                                            LINHVUCVANBAN_ID = g4.ID,

                                                            TENSOVANBAN = g5.TEXT,
                                                            TEN_DONVI = g6.TEXT,
                                                            SODITHEOSO = vanban.SODITHEOSO,
                                                            SODITHEOSO_NUMBER = vanban.SODITHEOSO_NUMBER ?? 0,
                                                            NGUOIKY = vanban.NGUOIKY,
                                                            NGUOITAO = vanban.NGUOITAO,
                                                            NGAYTAO = vanban.NGAYTAO,
                                                            CHUCVU = vanban.CHUCVU,
                                                            SOVANBANDEN_ID = vanban.SOVANBANDEN_ID,
                                                            IS_NOIBO = vanban.IS_NOIBO == true ? true : false,
                                                            PROCESS_USER_ID = wf.USER_ID,
                                                        };
            queryResult = queryResult.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDEN_BO> result = GetQueryResultAfterPaging(queryResult, searchModel, pageSize, pageIndex);
            return result;
        }

        /// <summary>
        /// @description: hàm "temporary_fix" hiện tại hàm queryResult = queryResult.OrderBy(searchModel.sortQuery) không  hoạt động
        /// @author: duynn
        /// @since: 08/08/2018
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="sortCondition"></param>
        /// <returns></returns>
        public IQueryable<HSCV_VANBANDEN_BO> GetSortedQueryResult(IQueryable<HSCV_VANBANDEN_BO> queryResult, string sortCondition)
        {
            string[] arrOrderChars = sortCondition.Split(' ');
            if (arrOrderChars.Length > 1 && !string.IsNullOrEmpty(arrOrderChars[0]) && !string.IsNullOrEmpty(arrOrderChars[1]))
            {
                string orderProp = arrOrderChars[1];
                string propertyName = arrOrderChars[0];

                PropertyInfo property = typeof(HSCV_VANBANDEN_BO).GetProperty(propertyName);
                if (property != null && (orderProp.Equals("desc") || orderProp.Equals("asc")))
                {
                    if (orderProp.Equals("desc"))
                    {
                        queryResult = queryResult.OrderByDescending(x => property.GetValue(x, null));
                    }
                    else
                    {
                        queryResult = queryResult.OrderBy(x => property.GetValue(x, null));
                    }
                }
                else
                {
                    queryResult = queryResult.OrderByDescending(x => x.NGAYTAO);
                }
            }
            return queryResult;
        }

        /// <summary>
        /// @description: hàm lấy danh sách sau khi phân trang
        /// @author: duynn
        /// @since: 08/08/2018
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDEN_BO> GetQueryResultAfterPaging(IQueryable<HSCV_VANBANDEN_BO> queryResult, HSCV_VANBANDEN_SEARCH searchModel, int pageSize, int pageIndex)
        {
            if (searchModel != null && queryResult != null)
            {
                queryResult = queryResult.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
                if (!string.IsNullOrEmpty(searchModel.SOHIEU))
                {
                    queryResult = queryResult.Where(x => x.SOHIEU.ToLower().Contains(searchModel.SOHIEU.ToLower().Trim()));
                }

                if (!string.IsNullOrEmpty(searchModel.TRICHYEU))
                {
                    queryResult = queryResult.Where(x => x.TRICHYEU.ToLower().Contains(searchModel.TRICHYEU.ToLower().Trim()));
                }
                if (!string.IsNullOrEmpty(searchModel.NGUOIKY))
                {
                    queryResult = queryResult.Where(x => x.NGUOIKY.ToLower().Contains(searchModel.NGUOIKY.ToLower().Trim()));
                }

                if (searchModel.LOAIVANBAN_ID.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.LOAIVANBAN_ID == x.LOAIVANBAN_ID);
                }

                if (searchModel.LINHVUCVANBAN_ID.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.LINHVUCVANBAN_ID == x.LINHVUCVANBAN_ID);
                }
                if (searchModel.DOKHAN_ID.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.DOKHAN_ID == x.DOKHAN_ID);
                }
                if (searchModel.DOMAT_ID.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.DOMAT_ID == x.DOMAT_ID);
                }
                if (searchModel.SOVANBANDEN_ID.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.SOVANBANDEN_ID == x.SOVANBANDEN_ID);
                }
                //Ngày ban hành
                if (searchModel.NGAYBANHANH_TU.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYBANHANH_TU <= x.NGAY_BANHANH);
                }
                if (searchModel.NGAYBANHANH_DEN.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYBANHANH_DEN >= x.NGAY_BANHANH);
                }
                //Hiệu lực từ ngày đến ngày
                if (searchModel.NGAYHIEULUC_TU.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYHIEULUC_TU <= x.NGAY_HIEULUC);
                }
                if (searchModel.NGAYHIEULUC_TU.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYHIEULUC_DEN >= x.NGAY_HIEULUC);
                }
                //Hết hiệu lực từ ngày đến ngày
                if (searchModel.NGAYHETHIEULUC_TU.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYHETHIEULUC_TU <= x.NGAYHET_HIEULUC);
                }
                if (searchModel.NGAYHETHIEULUC_DEN.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYHETHIEULUC_DEN >= x.NGAYHET_HIEULUC);
                }
                //Ngày văn bản từ ngày đến ngày
                if (searchModel.NGAYVANBAN_TU.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYVANBAN_TU <= x.NGAY_VANBAN);
                }
                if (searchModel.NGAYVANBAN_DEN.HasValue)
                {
                    queryResult = queryResult.Where(x => searchModel.NGAYVANBAN_DEN >= x.NGAY_VANBAN);
                }
                
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    queryResult = queryResult.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    queryResult = queryResult.OrderByDescending(x => x.SOVANBANDEN_ID)
                        .ThenByDescending(x => x.SODITHEOSO_NUMBER);
                }
                queryResult = queryResult.Where(x => x.IS_NOIBO == searchModel.isInternal);
            }

            PageListResultBO<HSCV_VANBANDEN_BO> result = new PageListResultBO<HSCV_VANBANDEN_BO>();
            if (pageIndex == -1)
            {
                int count = queryResult.Count() <= 0 ? 1 : queryResult.Count();
                IPagedList<HSCV_VANBANDEN_BO> pagedList  = queryResult.ToPagedList(1, count);
                result.ListItem = queryResult.ToList();
                result.Count = count;
                result.TotalPage = count;
            }
            else
            {
                IPagedList<HSCV_VANBANDEN_BO> pagedList = queryResult.ToPagedList(pageIndex, pageSize);
                result.Count = pagedList.TotalItemCount;
                result.TotalPage = pagedList.PageCount;
                result.ListItem = pagedList.ToList();
                foreach (var item in result.ListItem)
                {
                    item.CAN_CREATE_CALENDAR = (item.NGUOITAO == searchModel.USER_ID && CheckIsFinish(item.ID));
                }
            }
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra văn bản đến đã kết thúc
        /// @since: 15/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckIsFinish(long id)
        {
            bool isFinish = (from doc in this.context.HSCV_VANBANDEN.Where(x => x.ID == id)
                             join log in this.context.WF_LOG.Where(x => x.ITEM_TYPE == MODULE_CONSTANT.VANBANDEN)
                             on doc.ID equals log.ITEM_ID

                             join step in this.context.WF_STEP
                             on log.STEP_ID equals step.ID

                             join state in this.context.WF_STATE
                             on step.STATE_END equals state.ID
                             where state.IS_KETTHUC == true
                             select doc.ID).Count() > 0;
            return isFinish;
        }

        public List<HSCV_VANBANDEN_BO> GetNgayTao()
        {
            var query = (from tblVbd in this.context.HSCV_VANBANDEN
                         join tblWfprocess in this.context.WF_PROCESS on tblVbd.ID equals tblWfprocess.ITEM_ID into jWfprocess
                         from ngaygui in jWfprocess.DefaultIfEmpty()
                         select new HSCV_VANBANDEN_BO
                         {
                             ID = tblVbd.ID,
                             Update_At = ngaygui.UPDATED_AT,
                         }).ToList();
            return query;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy kết quả thống kê văn bản theo loại văn bản
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetStatisticResultByCategory(int departmentId, string itemType, string categoryCode, DateTime? queryDateStart, DateTime? queryDateEnd, int selected = 0)
        {

            IQueryable<HSCV_VANBANDEN> queryResult = (from doc in this.context.HSCV_VANBANDEN
                                                      join user in this.context.DM_NGUOIDUNG
                                                      on doc.NGUOITAO equals user.ID

                                                      join dept in this.context.CCTC_THANHPHAN
                                                      on user.DM_PHONGBAN_ID equals dept.ID
                                                      select doc);
            if (departmentId > 0)
            {
                var SelectedDeptObj = this.context.CCTC_THANHPHAN.Find(departmentId);
                if (SelectedDeptObj.TYPE == 10)
                {
                    queryResult = (from doc in this.context.HSCV_VANBANDEN
                                   join user in this.context.DM_NGUOIDUNG
                                       on doc.NGUOITAO equals user.ID

                                   join dept in this.context.CCTC_THANHPHAN
                                       on user.DM_PHONGBAN_ID equals dept.ID
                                   where dept.PARENT_ID == departmentId
                                   select doc);
                }
                else
                {
                    var LstVanBanDenId = (from itemlog in this.context.WF_ITEM_USER_PROCESS
                                          where itemlog.ITEM_TYPE == "MD_VANBANDEN"
                                          join user in this.context.DM_NGUOIDUNG
                                              on itemlog.USER_ID equals user.ID into g1
                                          from group1 in g1.DefaultIfEmpty()
                                          where group1.DM_PHONGBAN_ID == departmentId && itemlog.create_at >= queryDateStart &&
                                                itemlog.create_at <= queryDateEnd
                                          select itemlog.ITEM_ID).ToList();
                    if (LstVanBanDenId.Count > 0)
                    {
                        queryResult = (from doc in this.context.HSCV_VANBANDEN

                                       where LstVanBanDenId.Contains(doc.ID)
                                       select doc);
                    }
                    else
                    {
                        return new List<SelectListItem>();
                    }

                }

            }

            if (queryDateStart != null)
            {
                queryResult = queryResult.Where(x => x.NGAYTAO >= queryDateStart);
            }

            if (queryDateEnd != null)
            {
                queryResult = queryResult.Where(x => x.NGAYTAO <= queryDateEnd);
            }

            List<SelectListItem> result = new List<SelectListItem>();
            List<DM_DANHMUC_DATA> categories = this.context.DM_DANHMUC_DATA.ToList();
            if (selected > 0)
            {
                categories = categories.Where(x => x.ID == selected).ToList();
            }
            if (categoryCode == DMLOAI_CONSTANT.LOAI_VANBAN)
            {
                result = (from category in categories
                          join type in this.context.DM_NHOMDANHMUC
                          .Where(x => x.GROUP_CODE == categoryCode)
                          on category.DM_NHOM_ID equals type.ID
                          select new SelectListItem()
                          {
                              Text = category.TEXT,
                              Value = queryResult
                              .Where(x => x.LOAIVANBAN_ID == category.ID).Count().ToString(),
                          }).OrderByDescending(x => int.Parse(x.Value)).ToList();
            }

            else if (categoryCode == DMLOAI_CONSTANT.LINHVUCVANBAN)
            {
                result = (from category in categories
                          join type in this.context.DM_NHOMDANHMUC
                          .Where(x => x.GROUP_CODE == categoryCode)
                          on category.DM_NHOM_ID equals type.ID

                          select new SelectListItem()
                          {
                              Text = category.TEXT,
                              Value = queryResult
                              .Where(x => x.LINHVUCVANBAN_ID == category.ID).Count().ToString(),
                          }).OrderByDescending(x => int.Parse(x.Value)).ToList();
            }
            else
            {
                result = (from category in categories
                          join type in this.context.DM_NHOMDANHMUC
                          .Where(x => x.GROUP_CODE == categoryCode)
                          on category.DM_NHOM_ID equals type.ID
                          select new SelectListItem()
                          {
                              Text = category.TEXT,
                              Value = queryResult
                              .Where(x => x.DONVI_ID == category.ID).Count().ToString(),
                          }).OrderByDescending(x => int.Parse(x.Value)).ToList();
            }
            return result;
        }
    }
}
