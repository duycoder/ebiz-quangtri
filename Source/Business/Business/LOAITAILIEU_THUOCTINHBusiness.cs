using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.TAILIEUTHUOCTINH;
using Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using PagedList;
using Business.CommonModel.LOAITAILIEUTHUOCTINH;
using CommonHelper;

namespace Business.Business
{
    public class LOAITAILIEU_THUOCTINHBusiness : BaseBusiness<LOAITAILIEU_THUOCTINH>
    {
        public LOAITAILIEU_THUOCTINHBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<LOAITAILIEU_THUOCTINH> GetData(int id)
        {
            var result = from thuoctinh in this.context.LOAITAILIEU_THUOCTINH
                         where thuoctinh.DANHMUC_ID == id
                         select thuoctinh;
            return result.ToList();
        }
        public PageListResultBO<LOAITAILIEU_THUOCTINH_BO> GetDaTaByPage(TAILIEU_THUOCTINH_SEARCH searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from thuoctinh in this.context.LOAITAILIEU_THUOCTINH
                        join danhmuc in this.context.DM_DANHMUC_DATA
                        on thuoctinh.DANHMUC_ID equals danhmuc.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()
                        select new LOAITAILIEU_THUOCTINH_BO
                        {
                            ID = thuoctinh.ID,
                            DANHMUC_ID = thuoctinh.DANHMUC_ID,
                            MOTA = thuoctinh.MOTA,
                            TEN_DANHMUC = g1.TEXT,
                            TEN_THUOCTINH = thuoctinh.TEN_THUOCTINH,
                            TRANGTHAI = thuoctinh.TRANGTHAI
                        };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.TEN_DANHMUC);
                }
                if (!string.IsNullOrEmpty(searchModel.TEN_THUOCTINH))
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.TEN_THUOCTINH) && x.TEN_THUOCTINH.ToLower().Contains(searchModel.TEN_THUOCTINH.ToLower()));
                }
                if (searchModel.LOAI_TAILIEU > 0)
                {
                    query = query.Where(x => x.DANHMUC_ID == searchModel.LOAI_TAILIEU);
                }
                if (searchModel.TRANGTHAI.HasValue)
                {
                    query = query.Where(x => x.TRANGTHAI.HasValue && x.TRANGTHAI == searchModel.TRANGTHAI);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.TEN_DANHMUC);
            }
            var resultmodel = new PageListResultBO<LOAITAILIEU_THUOCTINH_BO>();
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
    }
}
