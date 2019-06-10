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
using Business.CommonModel.DMDANHMUCDATA;
using System.Web.Mvc;
using Business.CommonModel.ACTIONAUDIT;

namespace Business.Business
{
    public class ActionAuditBusiness : BaseBusiness<ACTION_AUDIT>
    {
        public ActionAuditBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(ACTION_AUDIT item)
        {
            try
            {
                if (item.ACTION_AUDIT_ID == 0)
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

        public PageListResultBO<ACTION_AUDIT> GetLog(ActionAuditSearchVM searchmodel = null, int pageSize = 20, int pageIndex = 1)
        {
            var query = from audit in this.context.ACTION_AUDIT
                select audit;
            if (searchmodel != null)
            {
                if (!string.IsNullOrEmpty(searchmodel.TENDANGNHAP))
                {
                    query = query.Where(x => x.USER_NAME == searchmodel.TENDANGNHAP);
                }
            }
            query = query.OrderByDescending(x => x.ACTION_AUDIT_ID);
            var resultmodel = new PageListResultBO<ACTION_AUDIT>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            return resultmodel;
        }
    }
}

