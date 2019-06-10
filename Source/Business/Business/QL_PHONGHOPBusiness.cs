using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.BaseBusiness;
using System.Threading.Tasks;
using Model.Entities;
using CommonHelper;
using Business.CommonModel.DS_PHONGHOP;
using Business.CommonBusiness;
using PagedList;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace Business.Business
{
    public class QL_PHONGHOPBusiness : BaseBusiness<QL_PHONGHOP>
    {
        public QL_PHONGHOPBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {

        }

        public PageListResultBO<QL_PHONG_BO> GetDaTaByPage(int? depid, QLPHONGHOP_SEARCHBO searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from tblPhong in this.context.QL_PHONGHOP
                        where tblPhong.DEPID == depid
                        select new QL_PHONG_BO
                        {
                            ID = tblPhong.ID,
                            TENPHONG = tblPhong.TENPHONG,
                            MOTA = tblPhong.MOTA,
                            MAPHONG = tblPhong.MAPHONG,
                            DEPID = tblPhong.DEPID,
                            SOCHONGOI = tblPhong.SOCHONGOI,
                        };

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.TenPhong))
                {
                    query = query.Where(a => a.TENPHONG.Contains(searchModel.TenPhong));
                }
                if (!string.IsNullOrEmpty(searchModel.MaPhong))
                {
                    query = query.Where(a => a.MAPHONG.Contains(searchModel.MaPhong));
                }

                //Lọc tìm kiếm
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(a => a.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(a => a.ID);
            }

            //Gán nội dung trả về

            var resultmodel = new PageListResultBO<QL_PHONG_BO>();
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
        public List<QL_PHONG_BO> GetPhong(int? depID)
        {
            var query = (from tblphong in this.context.QL_PHONGHOP
                         where tblphong.DEPID == depID
                        select new QL_PHONG_BO{
                            ID = tblphong.ID,
                            TENPHONG = tblphong.TENPHONG,
                            MOTA = tblphong.MOTA,
                            MAPHONG = tblphong.MAPHONG,
                            DEPID = tblphong.DEPID,
                            SOCHONGOI = tblphong.SOCHONGOI,
                        }).ToList();
            return query;
        }

    }
}
