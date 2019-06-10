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
    public class HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness : BaseBusiness<HSCV_CONGVIEC_NGUOITHAMGIAXULY>
    {
        public HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<HSCV_CONGVIEC_NGUOITHAMGIAXULY> GetData(List<long> Ids)
        {
            var result = from user in this.context.HSCV_CONGVIEC_NGUOITHAMGIAXULY.AsNoTracking()
                         where user.CONGVIEC_ID.HasValue && Ids.Contains(user.CONGVIEC_ID.Value)
                         && user.USER_ID.HasValue
                         select user;
            return result.ToList();
        }
        public List<HSCV_CONGVIEC_NGUOITHAMGIAXULY> GetData(List<long> Ids, long id)
        {
            var result = from user in this.context.HSCV_CONGVIEC_NGUOITHAMGIAXULY.AsNoTracking()
                         where user.CONGVIEC_ID.HasValue && Ids.Contains(user.CONGVIEC_ID.Value)
                         && id == user.USER_ID
                         select user;
            return result.ToList();
        }
    }
}
