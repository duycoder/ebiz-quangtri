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
using Business.CommonModel.VAITROTHAOTAC;
using Business.CommonModel.DMCHUCNANG;



namespace Business.Business
{
    public class VAITRO_THAOTACBusiness : BaseBusiness<VAITRO_THAOTAC>
    {
        public VAITRO_THAOTACBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public PageListResultBO<VAITRO_THAOTAC_BO> GetDaTaByPage(VAITRO_THAOTAC_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.VAITRO_THAOTAC
                        select new VAITRO_THAOTAC_BO
                        {
                            VAITRO_THAOTAC_ID = tbl.VAITRO_THAOTAC_ID,
                            DM_THAOTAC_ID = tbl.DM_THAOTAC_ID,
                            VAITRO_ID = tbl.VAITRO_ID,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                        };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.VAITRO_THAOTAC_ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.VAITRO_THAOTAC_ID);
            }
            var resultmodel = new PageListResultBO<VAITRO_THAOTAC_BO>();
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

        public bool UpdatePermission(int idvaitro, List<long> listThaoTac)
        {
            var listThaoTacDB = getListThaotacByVaiTro(idvaitro);
            var listIDThaoTac = listThaoTacDB.Select(x => x.DM_THAOTAC_ID).ToList();
            var db = this.context;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in listThaoTac)
                    {
                        if (!listIDThaoTac.Contains(item))
                        {
                            var vttt = new VAITRO_THAOTAC();
                            vttt.DM_THAOTAC_ID = item;
                            vttt.NGAYTAO = DateTime.Now;
                            vttt.VAITRO_ID = idvaitro;
                            db.VAITRO_THAOTAC.Add(vttt);
                            db.SaveChanges();
                        }
                    }

                    foreach (var item in listThaoTacDB)
                    {
                        if (!listThaoTac.Contains(item.DM_THAOTAC_ID.Value))
                        {
                            db.VAITRO_THAOTAC.Remove(item);
                            db.SaveChanges();
                        }
                    }
                    transaction.Commit();
                }
                catch
                {

                    transaction.Rollback();
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Lấy thao tác của vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DM_CHUCNANG_BO> getChucNangCuaVaiTro(int id)
        {
            var query = from vaitrothaotac in this.context.VAITRO_THAOTAC
                        where vaitrothaotac.VAITRO_ID == id
                        join thaotac in this.context.DM_THAOTAC on vaitrothaotac.DM_THAOTAC_ID equals thaotac.DM_THAOTAC_ID
                        group thaotac by thaotac.DM_CHUCNANG_ID into gchucnang
                        select new DM_CHUCNANG_BO
                        {
                            DM_CHUCNANG_ID = gchucnang.Key.Value,
                            ListThaoTac = gchucnang.ToList()
                        };
            return query.ToList();

        }


        public List<VAITRO_THAOTAC> getListThaotacByVaiTro(int id)
        {
            var query = this.context.VAITRO_THAOTAC.Where(x => x.VAITRO_ID == id).ToList();
            return query;
        }
        public VAITRO_THAOTAC_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.VAITRO_THAOTAC
                        where tbl.VAITRO_THAOTAC_ID == ID
                        select new VAITRO_THAOTAC_BO
                        {
                            VAITRO_THAOTAC_ID = tbl.VAITRO_THAOTAC_ID,
                            DM_THAOTAC_ID = tbl.DM_THAOTAC_ID,
                            VAITRO_ID = tbl.VAITRO_ID,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                        };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
        public List<VAITRO_THAOTAC> GetDataByThaoTacId(List<long> Ids)
        {
            var result = from vaitro in this.context.VAITRO_THAOTAC.AsNoTracking()
                         where vaitro.VAITRO_ID.HasValue && vaitro.DM_THAOTAC_ID.HasValue && Ids.Contains(vaitro.DM_THAOTAC_ID.Value)
                         select vaitro;
            return result.ToList();
        }
    }
}

