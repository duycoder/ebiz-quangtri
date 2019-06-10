using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.NGUOIDUNGVAITRO;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;



namespace Business.Business
{
    public class NGUOIDUNG_VAITROBusiness : BaseBusiness<NGUOIDUNG_VAITRO>
    {
        public NGUOIDUNG_VAITROBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public List<NGUOIDUNG_VAITRO> GetListByNguoiDung(long id)
        {
            var query = this.context.NGUOIDUNG_VAITRO.Where(x => x.NGUOIDUNG_ID == id && x.VAITRO_ID != null).ToList();
            return query;
        }
        public List<DM_VAITRO> GetVaiTroBYNguoiDung(long id)
        {
            var query = (from nguoidungvaitro in this.context.NGUOIDUNG_VAITRO.Where(x => x.NGUOIDUNG_ID == id)
                         join tbl_vaitro in this.context.DM_VAITRO on nguoidungvaitro.VAITRO_ID equals tbl_vaitro.DM_VAITRO_ID
                         select tbl_vaitro).ToList();
            return query;
        }

        public JsonResultBO UpdateVaiTroNguoiDung(long idNguoiDung, List<int> listVaiTro)
        {
            var result = new JsonResultBO(true);
            var listVaiTroNguoiDung = this.repository.All().Where(x => x.NGUOIDUNG_ID == idNguoiDung).ToList();
            var lstvaitroNguoiDungID = listVaiTroNguoiDung.Select(x => x.VAITRO_ID).ToList();
            using (var transaction = repository.Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in listVaiTro)
                    {
                        //Nếu chưa có vai trò này thì thêm mới
                        if (!lstvaitroNguoiDungID.Contains(item))
                        {
                            var ngdungvaitro = new NGUOIDUNG_VAITRO();
                            ngdungvaitro.NGUOIDUNG_ID = idNguoiDung;
                            ngdungvaitro.NGAYTAO = DateTime.Now;
                            ngdungvaitro.VAITRO_ID = item;
                            repository.Insert(ngdungvaitro);
                        }
                    }

                    foreach (var item in listVaiTroNguoiDung)
                    {
                        // Nếu vai trò đã được gán nhưng cập nhật k tồn tại thì xóa

                        if (!listVaiTro.Contains(item.VAITRO_ID.Value))
                        {
                            repository.Delete(item);
                            Save();
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    result.Status = false;
                    result.Message = "Không cập nhật được vai trò người dùng";
                    transaction.Rollback();
                }

            }
            return result;
        }
        public PageListResultBO<NGUOIDUNG_VAITRO_BO> GetDaTaByPage(NGUOIDUNG_VAITRO_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.NGUOIDUNG_VAITRO
                        select new NGUOIDUNG_VAITRO_BO
                           {
                               ID = tbl.ID,
                               VAITRO_ID = tbl.VAITRO_ID,
                               NGUOIDUNG_ID = tbl.NGUOIDUNG_ID,
                               NGAYTAO = tbl.NGAYTAO,
                               ROLE_DEFAULT = tbl.ROLE_DEFAULT,
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
            var resultmodel = new PageListResultBO<NGUOIDUNG_VAITRO_BO>();
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
        public NGUOIDUNG_VAITRO_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.NGUOIDUNG_VAITRO
                        where tbl.ID == ID
                        select new NGUOIDUNG_VAITRO_BO
                           {
                               ID = tbl.ID,
                               VAITRO_ID = tbl.VAITRO_ID,
                               NGUOIDUNG_ID = tbl.NGUOIDUNG_ID,
                               NGAYTAO = tbl.NGAYTAO,
                               ROLE_DEFAULT = tbl.ROLE_DEFAULT,
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
        public JsonResultBO saveImport(List<NGUOIDUNG_VAITRO> lstObj)
        {
            var result = new JsonResultBO(true);
            using (var transaction = repository.Context.Database.BeginTransaction())
            {
                try
                {

                    repository.Context.NGUOIDUNG_VAITRO.AddRange(lstObj);
                    repository.Context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    result.Status = false;
                    result.Message = "Không import được dữ liệu";
                    transaction.Rollback();
                }
            }
            return result;
        }
    }
}

