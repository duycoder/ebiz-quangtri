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
using Business.CommonModel.DMCHUCNANG;



namespace Business.Business
{
    public class DM_CHUCNANGBusiness : BaseBusiness<DM_CHUCNANG>
    {
        public DM_CHUCNANGBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public void Save(DM_CHUCNANG item)
        {
            try
            {

                if (item.DM_CHUCNANG_ID == 0)
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

        public JsonResultBO checkExistCode(string code, long id = 0)
        {
            var result = new JsonResultBO(true);
            if (id > 0)
            {
                var exist = repository.All().Where(x => x.MA_CHUCNANG.ToUpper().Equals(code.ToUpper()) && x.DM_CHUCNANG_ID != id).Any();
                result.Status = exist;
            }
            else
            {
                var exist = repository.All().Where(x => x.MA_CHUCNANG.ToUpper().Equals(code.ToUpper())).Any();
                result.Status = exist;
            }
            return result;
        }
        public PageListResultBO<DM_CHUCNANG_BO> GetDaTaByPage(DM_CHUCNANG_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.DM_CHUCNANG
                        select new DM_CHUCNANG_BO
                           {
                               DM_CHUCNANG_ID = tbl.DM_CHUCNANG_ID,
                               MA_CHUCNANG = tbl.MA_CHUCNANG,
                               TEN_CHUCNANG = tbl.TEN_CHUCNANG,
                               TRANGTHAI = tbl.TRANGTHAI,
                               NGAYTAO = tbl.NGAYTAO,
                               NGAYSUA = tbl.NGAYSUA,
                               URL = tbl.URL,
                               TT_HIENTHI = tbl.TT_HIENTHI,
                               NGUOITAO = tbl.NGUOITAO,
                               NGUOISUA = tbl.NGUOISUA,
                               IS_HIENTHI = tbl.IS_HIENTHI,
                               ICONPATH = tbl.ICONPATH,
                               CSSCLASS = tbl.CSSCLASS,
                           };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.QR_MA))
                {
                    query = query.Where(x => x.MA_CHUCNANG.Contains(searchModel.QR_MA));
                }

                if (!string.IsNullOrEmpty(searchModel.QR_CHUCNANG))
                {
                    query = query.Where(x => x.TEN_CHUCNANG.Contains(searchModel.QR_CHUCNANG));
                }

                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.DM_CHUCNANG_ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.DM_CHUCNANG_ID);
            }
            var resultmodel = new PageListResultBO<DM_CHUCNANG_BO>();
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

        public List<DM_CHUCNANG_BO> getChucNangThaoTac()
        {
            var query = from tbl in this.context.DM_CHUCNANG
                        join tt in this.context.DM_THAOTAC on tbl.DM_CHUCNANG_ID equals tt.DM_CHUCNANG_ID into jtt
                        select new DM_CHUCNANG_BO
                        {
                            DM_CHUCNANG_ID = tbl.DM_CHUCNANG_ID,
                            MA_CHUCNANG = tbl.MA_CHUCNANG,
                            TEN_CHUCNANG = tbl.TEN_CHUCNANG,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            URL = tbl.URL,
                            TT_HIENTHI = tbl.TT_HIENTHI,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                            IS_HIENTHI = tbl.IS_HIENTHI,
                            ICONPATH = tbl.ICONPATH,
                            CSSCLASS = tbl.CSSCLASS,
                            ListThaoTac = jtt.ToList()
                        };
            return query.OrderBy(x => x.TEN_CHUCNANG).ToList();
        }
        public DM_CHUCNANG_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.DM_CHUCNANG
                        where tbl.DM_CHUCNANG_ID == ID
                        select new DM_CHUCNANG_BO
                           {
                               DM_CHUCNANG_ID = tbl.DM_CHUCNANG_ID,
                               MA_CHUCNANG = tbl.MA_CHUCNANG,
                               TEN_CHUCNANG = tbl.TEN_CHUCNANG,
                               TRANGTHAI = tbl.TRANGTHAI,
                               NGAYTAO = tbl.NGAYTAO,
                               NGAYSUA = tbl.NGAYSUA,
                               URL = tbl.URL,
                               TT_HIENTHI = tbl.TT_HIENTHI,
                               NGUOITAO = tbl.NGUOITAO,
                               NGUOISUA = tbl.NGUOISUA,
                               IS_HIENTHI = tbl.IS_HIENTHI,
                               ICONPATH = tbl.ICONPATH,
                               CSSCLASS = tbl.CSSCLASS,
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
    }
}

