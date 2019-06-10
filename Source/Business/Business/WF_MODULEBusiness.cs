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
using Business.CommonModel.WFMODULE;



namespace Business.Business
{
    public class WF_MODULEBusiness : BaseBusiness<WF_MODULE>
    {
        public WF_MODULEBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public PageListResultBO<WF_MODULE_BO> GetDaTaByPage(WF_MODULE_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.WF_MODULE
                        //join tblLuong in this.context.WF_STREAM on tbl.WF_STREAM_ID equals tblLuong.ID into jLuong
                        //from luongxuly in jLuong.DefaultIfEmpty()
                        select new WF_MODULE_BO
                           {
                               ID = tbl.ID,
                               MODULE_CODE = tbl.MODULE_CODE,
                               MODULE_TITLE = tbl.MODULE_TITLE,
                               WF_STREAM_ID = tbl.WF_STREAM_ID,
                               create_at = tbl.create_at,
                               create_by = tbl.create_by,
                               edit_at = tbl.edit_at,
                               edit_by = tbl.edit_by,
                               //TenLuong = luongxuly != null ? luongxuly.WF_NAME : "Chưa thiết lập",
                           };
            if (searchModel != null)
            {
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
            var resultmodel = new PageListResultBO<WF_MODULE_BO>();
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
        public WF_MODULE_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.WF_MODULE
                        where tbl.ID == ID
                        select new WF_MODULE_BO
                           {
                               ID = tbl.ID,
                               MODULE_CODE = tbl.MODULE_CODE,
                               MODULE_TITLE = tbl.MODULE_TITLE,
                               WF_STREAM_ID = tbl.WF_STREAM_ID,
                               create_at = tbl.create_at,
                               create_by = tbl.create_by,
                               edit_at = tbl.edit_at,
                               edit_by = tbl.edit_by,
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }

        public bool ExistCode(string Code)
        {
            var result = false;
            var query = this.context.WF_MODULE.Where(x => x.MODULE_CODE.Equals(Code)).FirstOrDefault();
            result = query != null ? true : false;
            return result;
        }
    }
}

