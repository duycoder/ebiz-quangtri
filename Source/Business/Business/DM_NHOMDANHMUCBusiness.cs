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
using Business.CommonModel.DMNHOMDANHMUC;



namespace Business.Business
{
    public class DM_NHOMDANHMUCBusiness : BaseBusiness<DM_NHOMDANHMUC>
    {
        public DM_NHOMDANHMUCBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public JsonResultBO checkExistCode(string code, int id = 0)
        {
            var rs = new JsonResultBO(false);
            var obj = repository.All().Where(x => x.GROUP_CODE.ToUpper().Equals(code.ToUpper())).FirstOrDefault();
            //nếu đối tượng khác null thì code đã tồn tại
            rs.Status = obj != null ? true : false;
            //Nếu id >0 tức là đang cập nhật thì kiểm tra id obj nếu 
            if (id != 0 && obj != null)
            {
                rs.Status = obj.ID == id ? false : true;
            }
            return rs;
        }
        public PageListResultBO<DM_NHOMDANHMUC_BO> GetDaTaByPage(DM_NHOMDANHMUC_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.DM_NHOMDANHMUC
                        select new DM_NHOMDANHMUC_BO
                           {
                               ID = tbl.ID,
                               GROUP_CODE = tbl.GROUP_CODE,
                               GROUP_NAME = tbl.GROUP_NAME,
                               TYPE = tbl.TYPE,
                           };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.QR_CODE))
                {
                    query = query.Where(x => x.GROUP_CODE.Contains(searchModel.QR_CODE));
                }

                if (!string.IsNullOrEmpty(searchModel.QR_NAME))
                {
                    query = query.Where(x => x.GROUP_NAME.Contains(searchModel.QR_NAME));
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
            var resultmodel = new PageListResultBO<DM_NHOMDANHMUC_BO>();
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
        public DM_NHOMDANHMUC_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.DM_NHOMDANHMUC
                        where tbl.ID == ID
                        select new DM_NHOMDANHMUC_BO
                           {
                               ID = tbl.ID,
                               GROUP_CODE = tbl.GROUP_CODE,
                               GROUP_NAME = tbl.GROUP_NAME,
                               TYPE = tbl.TYPE,
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh mục theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DM_NHOMDANHMUC GetByCode(string code)
        {
            code = string.IsNullOrEmpty(code) ? string.Empty : code.ToLower();
            DM_NHOMDANHMUC result = this.context.DM_NHOMDANHMUC
                .Where(x => string.IsNullOrEmpty(x.GROUP_CODE) == false && x.GROUP_CODE.ToLower().Equals(code))
                .FirstOrDefault();
            return result;
        }
    }
}

