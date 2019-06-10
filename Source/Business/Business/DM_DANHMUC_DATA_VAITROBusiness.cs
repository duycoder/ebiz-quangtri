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



namespace Business.Business
{
    public class DM_DANHMUC_DATA_VAITROBusiness : BaseBusiness<DM_DANHMUC_DATA_VAITRO>
    {
        public DM_DANHMUC_DATA_VAITROBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<DM_DANHMUC_DATA_VAITRO> GetListByData(long id)
        {
            var query = this.context.DM_DANHMUC_DATA_VAITRO.Where(x => x.DATA_ID == id).ToList();
            return query;
        }
        public JsonResultBO UpdateVaiTroNguoiDung(int idData, List<int> listVaiTro)
        {
            var result = new JsonResultBO(true);
            var listVaiTroData = this.repository.All().Where(x => x.DATA_ID == idData).ToList();
            var lstvaitroDataID = listVaiTroData.Select(x => x.VAITRO_ID).ToList();
            using (var transaction = repository.Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in listVaiTro)
                    {
                        //Nếu chưa có vai trò này thì thêm mới
                        if (!lstvaitroDataID.Contains(item))
                        {
                            var datavaitro = new DM_DANHMUC_DATA_VAITRO();
                            datavaitro.DATA_ID = idData;
                            datavaitro.VAITRO_ID = item;
                            repository.Insert(datavaitro);
                        }
                    }

                    foreach (var item in listVaiTroData)
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
                    result.Message = "Không cập nhật được vai trò data";
                    transaction.Rollback();
                }

            }
            return result;
        }
    }
}

