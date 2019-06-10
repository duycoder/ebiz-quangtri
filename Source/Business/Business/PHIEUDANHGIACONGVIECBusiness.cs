using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class PHIEUDANHGIACONGVIECBusiness : BaseBusiness<PHIEUDANHGIACONGVIEC>
    {
        public PHIEUDANHGIACONGVIECBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }        
        public PHIEUDANHGIACONGVIEC GetData(long id)
        {
            var result = from phieu in this.context.PHIEUDANHGIACONGVIEC
                         where id == phieu.CONGVIEC_ID
                         select phieu;
            return result.FirstOrDefault();
        }
    }
}
