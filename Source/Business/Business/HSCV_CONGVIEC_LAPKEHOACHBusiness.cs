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
    public class HSCV_CONGVIEC_LAPKEHOACHBusiness : BaseBusiness<HSCV_CONGVIEC_LAPKEHOACH>
    {
        public HSCV_CONGVIEC_LAPKEHOACHBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<HSCV_CONGVIEC_LAPKEHOACH> GetData(List<long> Ids)
        {
            var result = from plan in this.context.HSCV_CONGVIEC_LAPKEHOACH.AsNoTracking()
                         where Ids.Contains(plan.CONGVIEC_ID)
                         select plan;
            return result.ToList();
        }
    }
}
