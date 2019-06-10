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
using Business.CommonModel.DMNGUOIDUNGTHAOTAC;



namespace Business.Business
{
    public class DM_NGUOIDUNG_THAOTACBusiness : BaseBusiness<DM_NGUOIDUNG_THAOTAC>
    {
        public DM_NGUOIDUNG_THAOTACBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public List<DM_NGUOIDUNG_THAOTAC> GetByUserID(long nguoidungID)
        {
            var query = this.context.DM_NGUOIDUNG_THAOTAC.Where(x => x.DM_NGUOIDUNG_ID == nguoidungID).ToList();
            return query;

        }

        public JsonResultBO SaveQuyenNguoiDung(long nguoidungid, List<long> ArrThaoTac, List<int> ArrTrangThai)
        {
            var result = new JsonResultBO(true);
            var listDB = this.context.DM_NGUOIDUNG_THAOTAC.Where(x => x.DM_NGUOIDUNG_ID == nguoidungid).ToList();
            var listDBTT = listDB.Select(x => x.DM_THAOTAC).ToList();
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {

                    for (int i = 0; i < ArrThaoTac.Count; i++)
                    {
                        // kiểm tra dữ liệu đã lưu
                        var obj = listDB.Where(x => x.DM_THAOTAC == ArrThaoTac[i]).FirstOrDefault();
                        if (obj != null)
                        {
                            // Khi thao tác ngày đã được lưu
                            switch (ArrTrangThai[i])
                            {
                                case 2:
                                    context.DM_NGUOIDUNG_THAOTAC.Remove(obj);
                                    context.SaveChanges();
                                    break;
                                case 0:
                                    obj.TRANGTHAI = false;
                                    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                    repository.Save();
                                    break;
                                case 1:
                                    obj.TRANGTHAI = true;
                                    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                    repository.Save();
                                    break;

                            }

                        }
                        else
                        {
                            //KHi thao tác chưa được lưu
                            if (ArrTrangThai[i] < 2)
                            {
                                var objNew = new DM_NGUOIDUNG_THAOTAC();
                                objNew.DM_NGUOIDUNG_ID = nguoidungid;
                                objNew.DM_THAOTAC = ArrThaoTac[i];
                                objNew.NGAYTAO = DateTime.Now;
                                objNew.TRANGTHAI = ArrTrangThai[i] == 1 ? true : false;
                                context.DM_NGUOIDUNG_THAOTAC.Add(objNew);
                                context.SaveChanges();
                            }

                        }
                       
                    }
                    transaction.Commit();

                }
                catch
                {

                    result.Status = false;
                    result.Message = "Không cập nhật được quyền";
                    transaction.Rollback();
                }
            }
            return result;
        }

        public void Save(DM_NGUOIDUNG_THAOTAC item)
        {
            try
            {

                if (item.DM_NGUOIDUNG_THAOTAC_ID == 0)
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


        public PageListResultBO<DM_NGUOIDUNG_THAOTAC_BO> GetDaTaByPage(DM_NGUOIDUNG_THAOTAC_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG_THAOTAC
                        select new DM_NGUOIDUNG_THAOTAC_BO
                           {
                               DM_NGUOIDUNG_THAOTAC_ID = tbl.DM_NGUOIDUNG_THAOTAC_ID,
                               DM_NGUOIDUNG_ID = tbl.DM_NGUOIDUNG_ID,
                               DM_THAOTAC = tbl.DM_THAOTAC,
                               NGUOITAO = tbl.NGUOITAO,
                               NGAYTAO = tbl.NGAYTAO,
                               NGUOISUA = tbl.NGUOISUA,
                               NGAYSUA = tbl.NGAYSUA,
                           };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.DM_NGUOIDUNG_THAOTAC_ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.DM_NGUOIDUNG_THAOTAC_ID);
            }
            var resultmodel = new PageListResultBO<DM_NGUOIDUNG_THAOTAC_BO>();
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
        public DM_NGUOIDUNG_THAOTAC_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG_THAOTAC
                        where tbl.DM_NGUOIDUNG_THAOTAC_ID == ID
                        select new DM_NGUOIDUNG_THAOTAC_BO
                           {
                               DM_NGUOIDUNG_THAOTAC_ID = tbl.DM_NGUOIDUNG_THAOTAC_ID,
                               DM_NGUOIDUNG_ID = tbl.DM_NGUOIDUNG_ID,
                               DM_THAOTAC = tbl.DM_THAOTAC,
                               NGUOITAO = tbl.NGUOITAO,
                               NGAYTAO = tbl.NGAYTAO,
                               NGUOISUA = tbl.NGUOISUA,
                               NGAYSUA = tbl.NGAYSUA,
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
    }
}

