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
using Business.CommonModel.DMTHAOTAC;



namespace Business.Business
{
    public class DM_THAOTACBusiness : BaseBusiness<DM_THAOTAC>
    {
        public DM_THAOTACBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }


        public JsonResultBO checkExistCode(string code, long id = 0)
        {
            var result = new JsonResultBO(true);
            if (id > 0)
            {
                var exist = repository.All().Where(x => x.MA_THAOTAC.ToUpper().Equals(code.ToUpper()) && x.DM_THAOTAC_ID != id).Any();
                result.Status = exist;
            }
            else
            {
                var exist = repository.All().Where(x => x.MA_THAOTAC.ToUpper().Equals(code.ToUpper())).Any();
                result.Status = exist;
            }
            return result;
        }
        public void Save(DM_THAOTAC item)
        {
            try
            {

                if (item.DM_THAOTAC_ID == 0)
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
                //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
                throw new Exception(ex.Message);
            }
        }

        public PageListResultBO<DM_THAOTAC_BO> GetDaTaByPage(int idchucnang, DM_THAOTAC_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.DM_THAOTAC.Where(x => x.DM_CHUCNANG_ID == idchucnang)
                        select new DM_THAOTAC_BO
                        {
                            DM_THAOTAC_ID = tbl.DM_THAOTAC_ID,
                            MA_THAOTAC = tbl.MA_THAOTAC,
                            TEN_THAOTAC = tbl.TEN_THAOTAC,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            DM_CHUCNANG_ID = tbl.DM_CHUCNANG_ID,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                            MENU_LINK = tbl.MENU_LINK,
                            TT_HIENTHI = tbl.TT_HIENTHI,
                            IS_HIENTHI = tbl.IS_HIENTHI,
                            ICONPATH = tbl.ICONPATH,
                            CSSCLASS = tbl.CSSCLASS,
                        };
            if (searchModel != null)
            {

                if (!string.IsNullOrEmpty(searchModel.QR_MA))
                {
                    query = query.Where(x => x.MA_THAOTAC.Contains(searchModel.QR_MA));
                }

                if (!string.IsNullOrEmpty(searchModel.QR_THAOTAC))
                {
                    query = query.Where(x => x.TEN_THAOTAC.Contains(searchModel.QR_THAOTAC));
                }

                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.DM_THAOTAC_ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.DM_THAOTAC_ID);
            }

            var resultmodel = new PageListResultBO<DM_THAOTAC_BO>();
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
        public DM_THAOTAC_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.DM_THAOTAC
                        where tbl.DM_THAOTAC_ID == ID
                        select new DM_THAOTAC_BO
                        {
                            DM_THAOTAC_ID = tbl.DM_THAOTAC_ID,
                            MA_THAOTAC = tbl.MA_THAOTAC,
                            TEN_THAOTAC = tbl.TEN_THAOTAC,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            DM_CHUCNANG_ID = tbl.DM_CHUCNANG_ID,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                            MENU_LINK = tbl.MENU_LINK,
                            TT_HIENTHI = tbl.TT_HIENTHI,
                            IS_HIENTHI = tbl.IS_HIENTHI,
                            ICONPATH = tbl.ICONPATH,
                            CSSCLASS = tbl.CSSCLASS,
                        };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
        public List<DM_THAOTAC> GetDataByCode(string code)
        {
            var result = from thaotac in this.context.DM_THAOTAC.AsNoTracking()
                         where thaotac.MA_THAOTAC.ToLower().Equals(code)
                         select thaotac;
            return result.ToList();
        }
    }
}

