using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVVANBANDI;
using CommonHelper;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace Business.Business
{
    public class HSCV_VANBANDIBusiness : BaseBusiness<HSCV_VANBANDI>
    {
        public HSCV_VANBANDIBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public void Save(HSCV_VANBANDI item)
        {
            try
            {
                if (item.ID == 0)
                {
                    this.repository.Insert(item);
                }
                else
                {
                    this.repository.Update(item);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<HSCV_VANBANDI> GetData()
        {
            var result = from vanban in this.context.HSCV_VANBANDI
                         orderby vanban.CREATED_AT descending
                         select vanban;
            return result.ToList();
        }
        public PageListResultBO<HSCV_VANBANDI_BO> GetDaTaByPage(HSCV_VANBANDI_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBAN_ID equals sovb.ID
                                                        into group7
                        from g7 in group7.DefaultIfEmpty()

                        join wfprocess in this.context.WF_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE)
                        on vanban.ID equals wfprocess.ITEM_ID
                        into groupwf
                        from jwf in groupwf.DefaultIfEmpty()

                        join user in this.context.DM_NGUOIDUNG
                        on vanban.CREATED_BY equals user.ID
                        into group6
                        from g6 in group6.DefaultIfEmpty()
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            NOI_NHAN = vanban.NOI_NHAN,
                            LANBANHANH = vanban.LANBANHANH,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g7.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO,
                            COLOR = g3.COLOR,
                            PROCESS_USER_ID = jwf.USER_ID ?? 0
                        };
            query = query.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }
        public PageListResultBO<HSCV_VANBANDI_BO> GetVanBanDaBanHanh(HSCV_VANBANDI_SEARCH searchModel, int DeptId, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
                        join loaivanban in this.context.DM_DANHMUC_DATA
                        on vanban.LOAIVANBAN_ID equals loaivanban.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join sovanban in this.context.DM_DANHMUC_DATA
                            on vanban.SOVANBAN_ID equals sovanban.ID
                            into group_svb
                        from g_svb in group_svb.DefaultIfEmpty()

                        join linhvuc in this.context.DM_DANHMUC_DATA
                        on vanban.LINHVUCVANBAN_ID equals linhvuc.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()

                        join dokhan in this.context.DM_DANHMUC_DATA
                        on vanban.DOKHAN_ID equals dokhan.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBAN_ID equals sovb.ID
                                                        into group6
                        from g6 in group6.DefaultIfEmpty()
                        join wfprocess in this.context.WF_PROCESS.Where(x => x.ITEM_TYPE == "MD_VANBANTRINHKY" && true == x.IS_END)
                        on vanban.ID equals wfprocess.ITEM_ID

                        where vanban.SOVANBAN_ID != null && vanban.DEPTID == DeptId
                        select new HSCV_VANBANDI_BO
                        {
                            TENSOVANBANDI = g_svb.TEXT,
                            CHUCVU = vanban.CHUCVU,
                            ID = vanban.ID,
                            LANBANHANH = vanban.LANBANHANH,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NOI_NHAN = vanban.NOI_NHAN,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,

                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,
                            
                            TENSOVANBAN = g6.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            YKIENCHIDAO = vanban.YKIENCHIDAO,
                            COLOR = g3.COLOR,
                            CREATED_AT = vanban.CREATED_AT,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID
                        };
            query = query.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }
        /// <summary>
        /// Danh sách văn bản đang xử lý
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDI_BO> GetListProcessing(HSCV_VANBANDI_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBAN_ID equals sovb.ID
                                                        into group6
                        from g6 in group6.DefaultIfEmpty()
                        join wf in this.context.WF_PROCESS
                        on vanban.ID equals wf.ITEM_ID
                        where wf.ITEM_TYPE == searchModel.ITEM_TYPE && wf.USER_ID == searchModel.USER_ID && wf.IS_END != true
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            LANBANHANH = vanban.LANBANHANH,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NOI_NHAN = vanban.NOI_NHAN,
                            NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g6.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO,
                            COLOR = g3.COLOR,
                            Update_At = wf.UPDATED_AT,
                            PROCESS_USER_ID = wf.USER_ID ?? 0
                        };
            query = query.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }
        /// <summary>
        /// Danh sách văn bản đã xử lý
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDI_BO> GetListProcessed(HSCV_VANBANDI_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join wfp in context.WF_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE)
                        on vanban.ID equals wfp.ITEM_ID
                        into group6

                        join sovb in this.context.DM_DANHMUC_DATA
                        on vanban.SOVANBAN_ID equals sovb.ID
                        into group7
                        from g7 in group7.DefaultIfEmpty()
                        from gwfp_vanban in group6.DefaultIfEmpty()

                        //duynn fix
                        join wf in context.WF_ITEM_USER_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE
                        && searchModel.USER_ID == x.USER_ID && true == x.IS_XULYCHINH && true == x.DAXULY)
                        on vanban.ID equals wf.ITEM_ID
                        where vanban.IS_INTERNAL != true
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            LANBANHANH = vanban.LANBANHANH,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            NOI_NHAN = vanban.NOI_NHAN,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g7.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO,
                            CURRENT_STATE = gwfp_vanban.CURRENT_STATE
                        };
            query = query.GroupBy(x => x.ID).Select(x => x.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }
        /// <summary>
        /// Danh sách văn bản cần review
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDI_BO> GetListReview(HSCV_VANBANDI_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBAN_ID equals sovb.ID
                                                        into group6
                        from g6 in group6.DefaultIfEmpty()
                        join wf in this.context.WF_USER_REVIEW.Where(x => x.ITEMTYPE.Equals(searchModel.ITEM_TYPE)
                        && searchModel.USER_ID == x.USER_ID && ((true == searchModel.IS_APPROVE) ? x.REVIEW_AT.HasValue : !x.REVIEW_AT.HasValue))
                        on vanban.ID equals wf.ITEMID
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            LANBANHANH = vanban.LANBANHANH,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            //NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g6.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO,
                            NGUOIKY_ID = wf.USER_ID
                        };
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }
        public HSCV_VANBANDI_BO FindById(long id)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                                                        on vanban.SOVANBAN_ID equals sovb.ID
                                                        into group6
                        from g6 in group6.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()
                        where vanban.ID == id
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            LANBANHANH = vanban.LANBANHANH,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g6.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO,
                            COLOR = g3.COLOR
                        };
            return query.FirstOrDefault();
        }
        /// <summary>
        /// Danh sách văn bản tham gia xử lý
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDI_BO> GetListJoinProcessing(HSCV_VANBANDI_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                        on vanban.SOVANBAN_ID equals sovb.ID
                        into group6
                        from g6 in group6.DefaultIfEmpty()
                        join wf in context.WF_ITEM_USER_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE
                        && searchModel.USER_ID == x.USER_ID && false == x.IS_XULYCHINH)
                        on vanban.ID equals wf.ITEM_ID
                        where vanban.IS_INTERNAL != true
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            NOI_NHAN = vanban.NOI_NHAN,
                            LANBANHANH = vanban.LANBANHANH,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g6.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO
                        };
            query = query.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách văn bản đi gửi nội bộ
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageListResultBO<HSCV_VANBANDI_BO> GetListInternal(HSCV_VANBANDI_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from vanban in this.context.HSCV_VANBANDI
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

                        join douutien in this.context.DM_DANHMUC_DATA
                        on vanban.DOUUTIEN_ID equals douutien.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        join nguoiky in this.context.DM_NGUOIDUNG
                        on vanban.NGUOIKY_ID equals nguoiky.ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        join sovb in this.context.DM_DANHMUC_DATA
                        on vanban.SOVANBAN_ID equals sovb.ID
                        into group6
                        from g6 in group6.DefaultIfEmpty()
                        join wf in context.WF_ITEM_USER_PROCESS.Where(x => x.ITEM_TYPE == searchModel.ITEM_TYPE
                        && searchModel.USER_ID == x.USER_ID && true == x.IS_XULYCHINH)
                        on vanban.ID equals wf.ITEM_ID
                        where vanban.IS_INTERNAL == true
                        select new HSCV_VANBANDI_BO
                        {
                            CHUCVU = vanban.CHUCVU,
                            LOAIVANBAN_ID = vanban.LOAIVANBAN_ID,
                            DOKHAN_ID = vanban.DOKHAN_ID,
                            LINHVUCVANBAN_ID = vanban.LINHVUCVANBAN_ID,
                            CREATED_AT = vanban.CREATED_AT,
                            CREATED_BY = vanban.CREATED_BY,
                            DONVINHAN_EXTERNAL_ID = vanban.DONVINHAN_EXTERNAL_ID,
                            DONVINHAN_INTERNAL_ID = vanban.DONVINHAN_INTERNAL_ID,
                            DONVINHAN_NGOAIHETHONG = vanban.DONVINHAN_NGOAIHETHONG,
                            DONVISOANTHAO_ID = vanban.DONVISOANTHAO_ID,
                            DOUUTIEN_ID = vanban.DOUUTIEN_ID,
                            ID = vanban.ID,
                            NOI_NHAN = vanban.NOI_NHAN,
                            LANBANHANH = vanban.LANBANHANH,
                            NGAYBANHANH = vanban.NGAYBANHANH,
                            NGAYCOHIEULUC = vanban.NGAYCOHIEULUC,
                            NGAYHETHIEULUC = vanban.NGAYHETHIEULUC,
                            NGAYVANBAN = vanban.NGAYVANBAN,
                            NGUOIKY_ID = vanban.NGUOIKY_ID,
                            NOIDUNG = vanban.NOIDUNG,
                            SOBANSAO = vanban.SOBANSAO,
                            SOHIEU = vanban.SOHIEU,
                            SOTHEOSO = vanban.SOTHEOSO,
                            SOTO = vanban.SOTO,
                            SOTRANG = vanban.SOTRANG,
                            SOVANBAN_ID = vanban.SOVANBAN_ID,
                            TEN_DOKHAN = g3.TEXT,
                            TEN_DOUUTIEN = g4.TEXT,
                            TEN_LINHVUC = g2.TEXT,
                            TEN_LOAIVANBAN = g1.TEXT,
                            ICON_DOKHAN = g3.ICON,
                            ICON_DOUUTIEN = g4.ICON,
                            ICON_LINHVUC = g2.ICON,
                            ICON_LOAIVANBAN = g1.ICON,
                            TEN_NGUOIKY = g5.HOTEN,

                            TENSOVANBAN = g6.TEXT,

                            THOIHANHOIBAO = vanban.THOIHANHOIBAO,
                            THOIHANXULY = vanban.THOIHANXULY,
                            TRICHYEU = vanban.TRICHYEU,
                            UPDATED_AT = vanban.UPDATED_AT,
                            UPDATED_BY = vanban.UPDATED_BY,
                            YKIENCHIDAO = vanban.YKIENCHIDAO
                        };
            query = query.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
            PageListResultBO<HSCV_VANBANDI_BO> result = this.GetListVanBanDiAfterPaging(searchModel, query, pageIndex, pageSize);
            return result;
        }
        public List<VanBanEmailBO> GetVanBanChuaXuLy()
        {
            var now = DateTime.Now;
            var curentDate = now.Date;
            var lstObj = (from process in this.context.WF_PROCESS
                          join nguoidung in this.context.DM_NGUOIDUNG
                          on process.USER_ID equals nguoidung.ID
                          join state in this.context.WF_STATE
                          on process.CURRENT_STATE equals state.ID
                          where process.ITEM_TYPE == "MD_VANBANTRINHKY"
                          && true != state.IS_KETTHUC
                          && DbFunctions.AddDays(now, 1) >= process.UPDATED_AT
                          group new { process, nguoidung } by nguoidung.ID into uservanban
                          select new VanBanEmailBO
                          {
                              title = "Văn bản trình ký chưa xử lý",
                              subtitle = "Vui lòng xử lý các văn bản này đúng hạn",
                              email = uservanban.FirstOrDefault().nguoidung.EMAIL,
                              userName = uservanban.FirstOrDefault().nguoidung.HOTEN,
                              LstVanBanIds = uservanban.Select(x => x.process.ITEM_ID.Value).ToList()
                          }
                         ).ToList();
            return lstObj;
        }
        public List<HSCV_VANBANDI> GetDataByIds(List<long> ids)
        {
            return this.repository.All().Where(x => ids.Contains(x.ID)).ToList();
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy kết quả thống kê văn bản theo loại văn bản
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetStatisticResultByCategory(int departmentId, string categoryCode, DateTime? queryDateStart, DateTime? queryDateEnd, int selected = 0)
        {
            IQueryable<HSCV_VANBANDI> queryResult = (from doc in this.context.HSCV_VANBANDI
                                                     join user in this.context.DM_NGUOIDUNG
                                                     on doc.CREATED_BY equals user.ID
                                                     join dept in this.context.CCTC_THANHPHAN
                                                     on user.DM_PHONGBAN_ID equals dept.ID
                                                     select doc);
            if (departmentId > 0)
            {
                queryResult = (from doc in this.context.HSCV_VANBANDI
                               join user in this.context.DM_NGUOIDUNG
                               on doc.CREATED_BY equals user.ID
                               join dept in this.context.CCTC_THANHPHAN.Where(x => x.ID == departmentId)
                               on user.DM_PHONGBAN_ID equals dept.ID
                               select doc);
            }
            if (queryDateStart != null)
            {
                queryResult = queryResult.Where(x => x.CREATED_AT >= queryDateStart);
            }

            if (queryDateEnd != null)
            {
                queryResult = queryResult.Where(x => x.CREATED_AT <= queryDateEnd);
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
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy số liệu thống kê theo đơn vị nhận
        /// </summary>
        /// <param name="queryDateStart"></param>
        /// <param name="queryDateEnd"></param>
        /// <returns></returns>
        public List<SelectListItem> GetStatisticResultByInteralDepartment(int departmentId, DateTime? queryDateStart, DateTime? queryDateEnd, int selected = 0)
        {
            IQueryable<HSCV_VANBANDI> queryResultDocs = (from doc in this.context.HSCV_VANBANDI
                                                         join user in this.context.DM_NGUOIDUNG
                                                          on doc.CREATED_BY equals user.ID

                                                         join dept in this.context.CCTC_THANHPHAN
                                                         on user.DM_PHONGBAN_ID equals dept.ID
                                                         select doc);
            if (departmentId > 0)
            {
                var SelectedDeptObj = this.context.CCTC_THANHPHAN.Find(departmentId);
                if (SelectedDeptObj.TYPE == 10)
                {
                    queryResultDocs = (from doc in this.context.HSCV_VANBANDI
                                       join user in this.context.DM_NGUOIDUNG
                                           on doc.CREATED_BY equals user.ID

                                       join dept in this.context.CCTC_THANHPHAN
                                           on user.DM_PHONGBAN_ID equals dept.ID
                                       where dept.PARENT_ID == departmentId
                                       select doc);
                }
                else
                {
                    var LstVanBanTrinhKyId = (from itemlog in this.context.WF_ITEM_USER_PROCESS
                                              where itemlog.ITEM_TYPE == "MD_VANBANTRINHKY"
                                              join user in this.context.DM_NGUOIDUNG
                                                  on itemlog.USER_ID equals user.ID into g1
                                              from group1 in g1.DefaultIfEmpty()
                                              where group1.DM_PHONGBAN_ID == departmentId && itemlog.create_at >= queryDateStart &&
                                                    itemlog.create_at <= queryDateEnd
                                              select itemlog.ITEM_ID).ToList();
                    if (LstVanBanTrinhKyId.Count > 0)
                    {
                        queryResultDocs = (from doc in this.context.HSCV_VANBANDI

                                           where LstVanBanTrinhKyId.Contains(doc.ID)
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
                queryResultDocs = queryResultDocs.Where(x => x.CREATED_AT >= queryDateStart);
            }

            if (queryDateEnd != null)
            {
                queryResultDocs = queryResultDocs.Where(x => x.CREATED_AT <= queryDateEnd);
            }
            List<string> interalDepartmentIdStrs = queryResultDocs.Where(x => x.DONVINHAN_INTERNAL_ID != null).Select(x => x.DONVINHAN_INTERNAL_ID).ToList();
            List<int> departmentIds = new List<int>();
            interalDepartmentIdStrs.ForEach(x =>
            {
                List<int> items = x.ToListInt(',');
                departmentIds.AddRange(items);
            });
            List<CCTC_THANHPHAN> depts = this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true).ToList();
            if (selected > 0)
            {
                depts = depts.Where(x => x.ID == selected).ToList();
            }

            List<SelectListItem> result = (from dept in depts
                                           select new SelectListItem()
                                           {
                                               Text = dept.NAME,
                                               Value = departmentIds.Where(x => x == dept.ID).Count().ToString(),
                                           }).OrderByDescending(x => int.Parse(x.Value)).ToList();
            return result;
        }

        private PageListResultBO<HSCV_VANBANDI_BO> GetListVanBanDiAfterPaging(HSCV_VANBANDI_SEARCH searchModel, IQueryable<HSCV_VANBANDI_BO> queryResult, int pageIndex, int pageSize)
        {
            if (searchModel != null)
            {
                #region Tim kiem
                if (searchModel.isMobileFilter)
                {
                    if (!string.IsNullOrEmpty(searchModel.mobileQuery))
                    {
                        queryResult = queryResult.Where(x => (x.SOHIEU != null && x.SOHIEU.ToLower().Contains(searchModel.mobileQuery.ToLower().Trim())) ||
                        (x.TRICHYEU != null && x.TRICHYEU.ToLower().Contains(searchModel.mobileQuery.ToLower().Trim())));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(searchModel.SOHIEU))
                    {
                        queryResult = queryResult.Where(x => x.SOHIEU.ToLower().Contains(searchModel.SOHIEU.ToLower().Trim()));
                    }
                    if (!string.IsNullOrEmpty(searchModel.TRICHYEU))
                    {
                        queryResult = queryResult.Where(x => x.TRICHYEU.ToLower().Contains(searchModel.TRICHYEU.ToLower().Trim()));
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
                    if (searchModel.DOUUTIEN_ID.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.DOUUTIEN_ID == x.DOUUTIEN_ID);
                    }
                    if (searchModel.SOVANBAN_ID.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.SOVANBAN_ID == x.SOVANBAN_ID);
                    }
                    //Ngày ban hành
                    if (searchModel.NGAYBANHANH_TU.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYBANHANH_TU <= x.NGAYBANHANH);
                    }
                    if (searchModel.NGAYBANHANH_DEN.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYBANHANH_DEN >= x.NGAYBANHANH);
                    }
                    //Hiệu lực từ ngày đến ngày
                    if (searchModel.NGAYHIEULUC_TU.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYHIEULUC_TU <= x.NGAYCOHIEULUC);
                    }
                    if (searchModel.NGAYHIEULUC_TU.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYHIEULUC_DEN >= x.NGAYCOHIEULUC);
                    }
                    //Hết hiệu lực từ ngày đến ngày
                    if (searchModel.NGAYHETHIEULUC_TU.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYHETHIEULUC_TU <= x.NGAYHETHIEULUC);
                    }
                    if (searchModel.NGAYHETHIEULUC_DEN.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYHETHIEULUC_DEN >= x.NGAYHETHIEULUC);
                    }
                    //Ngày văn bản từ ngày đến ngày
                    if (searchModel.NGAYVANBAN_TU.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYVANBAN_TU <= x.NGAYVANBAN);
                    }
                    if (searchModel.NGAYVANBAN_DEN.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYVANBAN_DEN >= x.NGAYVANBAN);
                    }
                    //Ngày tạo văn bản
                    if (searchModel.NGAYTAO_TU.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYTAO_TU <= x.CREATED_AT);
                    }
                    if (searchModel.NGAYTAO_DEN.HasValue)
                    {
                        queryResult = queryResult.Where(x => searchModel.NGAYTAO_DEN >= x.CREATED_AT);
                    }
                }
                #endregion
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    queryResult = queryResult.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    queryResult = queryResult.OrderByDescending(x => x.CREATED_AT);
                }
            }
            else
            {
                queryResult = queryResult.OrderByDescending(x => x.CREATED_AT);
            }
            var result = new PageListResultBO<HSCV_VANBANDI_BO>();
            if (pageIndex == -1)
            {
                int count = queryResult.Count() <= 0 ? 1 : queryResult.Count();
                var dataPageList = queryResult.ToPagedList(1, count);
                result.Count = count;
                result.TotalPage = 1;
                result.ListItem = queryResult.ToList();
                return result;
            }
            else
            {
                var dataPageList = queryResult.ToPagedList(pageIndex, pageSize);
                result.Count = dataPageList.TotalItemCount;
                result.TotalPage = dataPageList.PageCount;
                result.ListItem = dataPageList.ToList();
                return result;
            }
        }
    }
}
