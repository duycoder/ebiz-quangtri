using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.TAILIEUTHUOCTINH;
using Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using PagedList;
using Business.CommonModel.LOAITAILIEUTHUOCTINH;
using Business.CommonModel.TAILIEUDINHKEM;
using System;
using Business.CommonModel.EFILECHIASE;
using Business.CommonModel.THUMUCLUUTRU;

namespace Business.Business
{
    public class EFILE_CHIASEBusiness : BaseBusiness<EFILE_CHIASE>
    {
        public EFILE_CHIASEBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<EFILE_CHIASE_BO> GetData(long id, bool isFolder)
        {
            if (isFolder)
            {
                var result = from chiase in this.context.EFILE_CHIASE
                             join nguoidung in this.context.DM_NGUOIDUNG
                             on chiase.USER_ID equals nguoidung.ID
                             into group1
                             from g1 in group1.DefaultIfEmpty()
                             join folder in this.context.THUMUC_LUUTRU
                             on chiase.ITEM_ID equals folder.ID
                             into group2
                             from g2 in group2.DefaultIfEmpty()
                             where chiase.ITEM_ID == id && chiase.IS_FOLDER.HasValue && chiase.IS_FOLDER.Value == isFolder
                             select new EFILE_CHIASE_BO
                             {
                                 DENNGAY = chiase.DENNGAY,
                                 GHICHU = chiase.GHICHU,
                                 ID = chiase.ID,
                                 IS_FOLDER = chiase.IS_FOLDER,
                                 ITEM_ID = chiase.ITEM_ID,
                                 NGAY_CHIASE = chiase.NGAY_CHIASE,
                                 SHARING_BY = chiase.SHARING_BY,
                                 TEN_NGUOIDUNG = g1.HOTEN,
                                 TEN_TAILIEU = "",
                                 TEN_THUMUC = g2.TENTHUMUC,
                                 TUNGAY = chiase.TUNGAY,
                                 USER_ID = chiase.USER_ID,
                                 DONVI_ID = chiase.DONVI_ID,
                                 PERMISSION = chiase.PERMISSION
                             };
                return result.ToList();
            }
            else
            {
                var result = from chiase in this.context.EFILE_CHIASE
                             join nguoidung in this.context.DM_NGUOIDUNG
                             on chiase.USER_ID equals nguoidung.ID
                             into group1
                             from g1 in group1.DefaultIfEmpty()
                             join file in this.context.TAILIEUDINHKEM
                             on chiase.ITEM_ID equals file.TAILIEU_ID
                             into group2
                             from g2 in group2.DefaultIfEmpty()
                             where chiase.ITEM_ID == id && chiase.IS_FOLDER.HasValue && chiase.IS_FOLDER.Value == isFolder
                             select new EFILE_CHIASE_BO
                             {
                                 DENNGAY = chiase.DENNGAY,
                                 GHICHU = chiase.GHICHU,
                                 ID = chiase.ID,
                                 IS_FOLDER = chiase.IS_FOLDER,
                                 ITEM_ID = chiase.ITEM_ID,
                                 NGAY_CHIASE = chiase.NGAY_CHIASE,
                                 SHARING_BY = chiase.SHARING_BY,
                                 TEN_NGUOIDUNG = g1.HOTEN,
                                 TEN_TAILIEU = "",
                                 TEN_THUMUC = g2.TENTAILIEU,
                                 TUNGAY = chiase.TUNGAY,
                                 USER_ID = chiase.USER_ID
                             };
                return result.ToList();
            }
        }
        public PageListResultBO<THUMUC_LUUTRU_BO> GetDaTaByPage(EFILE_CHIASE_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from chiase in this.context.EFILE_CHIASE
                        join nguoidung in this.context.DM_NGUOIDUNG
                        on chiase.SHARING_BY equals nguoidung.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()
                        join donvi in this.context.CCTC_THANHPHAN
                        on chiase.DONVI_ID equals donvi.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()
                        join folder in this.context.THUMUC_LUUTRU
                        on chiase.ITEM_ID equals folder.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()
                        join file in this.context.TAILIEUDINHKEM
                        on chiase.ITEM_ID equals file.TAILIEU_ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()
                        where chiase.USER_ID.HasValue && chiase.USER_ID == searchModel.USER_ID
                        && (!chiase.TUNGAY.HasValue || (chiase.TUNGAY.HasValue && chiase.TUNGAY.Value <= DateTime.Today))
                        && (!chiase.DENNGAY.HasValue || (chiase.DENNGAY.HasValue && chiase.DENNGAY.Value >= DateTime.Today))
                        select new THUMUC_LUUTRU_BO
                        {
                            ACCESS_MODIFIER = g2.ACCESS_MODIFIER,
                            TENTHUMUC = chiase.IS_FOLDER.HasValue && !chiase.IS_FOLDER.Value ? g3.TENTAILIEU : g2.TENTHUMUC,
                            ID = chiase.ITEM_ID.HasValue ? chiase.ITEM_ID.Value : 0,
                            IS_THUMUC = chiase.IS_FOLDER.HasValue && !chiase.IS_FOLDER.Value ? false : true,
                            THUMUCCHA = g3.DINHDANG_FILE,
                            TEN_NGUOITAO = g1.HOTEN,
                            TEN_DONVI = g4.NAME,
                            DONVI_ID = chiase.DONVI_ID,
                            USER_ID = chiase.USER_ID,
                            NGAYTAO = chiase.NGAY_CHIASE,
                            //PERMISSION = chiase.IS_FOLDER.HasValue && !chiase.IS_FOLDER.Value ? g3.PERMISSION : g2.PERMISSION
                            PERMISSION = chiase.PERMISSION
                            //DENNGAY = chiase.DENNGAY,
                            //GHICHU = chiase.GHICHU,
                            //ID = chiase.ID,
                            //ITEM_ID = chiase.ITEM_ID,
                            //IS_FOLDER = chiase.IS_FOLDER,
                            //NGAY_CHIASE = chiase.NGAY_CHIASE,
                            //SHARING_BY = chiase.SHARING_BY,
                            //TEN_NGUOIDUNG = g1.HOTEN,
                            //TEN_TAILIEU = (chiase.IS_FOLDER.HasValue && !chiase.IS_FOLDER.Value ? g3.TENTAILIEU : ""),
                            //TEN_THUMUC = (chiase.IS_FOLDER.HasValue && chiase.IS_FOLDER.Value ? g2.TENTHUMUC : ""),
                            //TUNGAY = chiase.TUNGAY,
                            //USER_ID = chiase.USER_ID
                        };
            query = query.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderBy(x => x.TENTHUMUC);
                }
            }
            else
            {
                query = query.OrderBy(x => x.TENTHUMUC);
            }
            var resultmodel = new PageListResultBO<THUMUC_LUUTRU_BO>();
            if (pageSize == -1)
            {
                var dataPageList = query.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = query.ToPagedList(pageIndex, pageSize);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
        public List<EFILE_CHIASE> GetFolderByUserId(long id)
        {
            var result = from chiase in this.context.EFILE_CHIASE
                         where chiase.USER_ID.HasValue && chiase.USER_ID == id && chiase.IS_FOLDER.HasValue
                         && chiase.IS_FOLDER.Value
                         select chiase;
            return result.ToList();
        }
    }
}
