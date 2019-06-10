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
using Business.CommonModel.WFSTREAM;
using System.Web.Mvc;



namespace Business.Business
{
    public class WF_STREAMBusiness : BaseBusiness<WF_STREAM>
    {
        public WF_STREAMBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public PageListResultBO<WF_STREAM_BO> GetDaTaByPage( WF_STREAM_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.WF_STREAM
                        join datalevel in this.context.DM_DANHMUC_DATA on tbl.LEVEL_ID equals datalevel.ID
                        into g1
                        from group1 in g1.DefaultIfEmpty()
                        select new WF_STREAM_BO
                           {
                               ID = tbl.ID,
                               WF_NAME = tbl.WF_NAME,
                               WF_DESCRIPTION = tbl.WF_DESCRIPTION,
                               create_at = tbl.create_at,
                               create_by = tbl.create_by,
                               edit_at = tbl.edit_at,
                               edit_by = tbl.edit_by,
                               TenCap = group1.TEXT
                           };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.QR_WF_NAME))
                {
                    query = query.Where(x => x.WF_NAME.Contains(searchModel.QR_WF_NAME));
                }

                if (!string.IsNullOrEmpty(searchModel.QR_WF_DESCRIPTION))
                {
                    query = query.Where(x => x.WF_DESCRIPTION.Contains(searchModel.QR_WF_DESCRIPTION));
                }

                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }
            var resultmodel = new PageListResultBO<WF_STREAM_BO>();
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
        public WF_STREAM_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.WF_STREAM
                        where tbl.ID == ID
                        select new WF_STREAM_BO
                           {
                               ID = tbl.ID,
                               WF_NAME = tbl.WF_NAME,
                               WF_DESCRIPTION = tbl.WF_DESCRIPTION,
                               create_at = tbl.create_at,
                               create_by = tbl.create_by,
                               edit_at = tbl.edit_at,
                               edit_by = tbl.edit_by,
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }

        public List<SelectListItem> DsLuong(int selectedItem = 0)
        {
            var query = this.context.WF_STREAM
                .Select(x => new SelectListItem()
                {
                    Text = x.WF_NAME,
                    Value = x.ID.ToString(),
                    Selected = selectedItem > 0 && x.ID == selectedItem
                }).ToList();
            return query;
        }
        public List<SelectListItem> DsLuongMultipe(List<int> lstselectedItem)
        {
            var query = this.context.WF_STREAM
                .Select(x => new SelectListItem()
                {
                    Text = x.WF_NAME,
                    Value = x.ID.ToString(),
                    Selected = lstselectedItem.Contains(x.ID)
                }).ToList();
            return query;
        }
    }
}

