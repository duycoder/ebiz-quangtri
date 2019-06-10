using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.TAILIEUTHUOCTINH;
using Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using PagedList;
using Business.CommonModel.LOAITAILIEUTHUOCTINH;
using Business.CommonModel.TAILIEUDINHKEM;
using System;

namespace Business.Business
{
    public class TAILIEU_THUOCTINHBusiness : BaseBusiness<TAILIEU_THUOCTINH>
    {
        public TAILIEU_THUOCTINHBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public bool Save(List<TAILIEU_THUOCTINH> ListThuocTinh)
        {
            try
            {
                foreach (var item in ListThuocTinh)
                {
                    if (item.ID == 0)
                    {
                        this.repository.Insert(item);
                    }
                    else
                    {
                        this.repository.Update(item);
                    }
                }
                this.repository.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<TAILIEUTHUOCTINH_BO> GetDataBO(long TAILIEU_ID)
        {
            var result = from tailieu in this.context.TAILIEU_THUOCTINH
                         join thuoctinh in this.context.LOAITAILIEU_THUOCTINH
                         on tailieu.THUOCTINH_ID equals thuoctinh.ID
                         into group1
                         from g1 in group1.DefaultIfEmpty()
                         where tailieu.TAILIEU_ID.HasValue && tailieu.TAILIEU_ID.Value == TAILIEU_ID
                         select new TAILIEUTHUOCTINH_BO
                         {
                             GIATRI = tailieu.GIATRI,
                             ID = tailieu.ID,
                             TAILIEU_ID = tailieu.TAILIEU_ID,
                             TEN_THUOCTINH = g1.TEN_THUOCTINH,
                             THUOCTINH_ID = tailieu.THUOCTINH_ID
                         };
            return result.ToList();
        }
        public List<TAILIEU_THUOCTINH> GetData(long TAILIEU_ID)
        {
            var result = from tailieu in this.context.TAILIEU_THUOCTINH
                         where tailieu.TAILIEU_ID.HasValue && tailieu.TAILIEU_ID.Value == TAILIEU_ID
                         select tailieu;
            return result.ToList();
        }
    }
}
