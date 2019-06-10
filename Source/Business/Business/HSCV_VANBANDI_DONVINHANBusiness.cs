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
using Business.CommonModel.DMVAITRO;
using System.Web.Mvc;


namespace Business.Business
{
    public class HSCV_VANBANDI_DONVINHANBusiness : BaseBusiness<HSCV_VANBANDI_DONVINHAN>
    {
        public HSCV_VANBANDI_DONVINHANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<HSCV_VANBANDI_DONVINHAN> GetData(long id)
        {
            var result = from donvi in this.context.HSCV_VANBANDI_DONVINHAN
                         where donvi.VANBANDI_ID.HasValue && donvi.VANBANDI_ID.Value == id
                         select donvi;
            return result.ToList();
        }
        public List<long> GetIdsByDonVi(int DonViId)
        {
            var result = from donvi in this.context.HSCV_VANBANDI_DONVINHAN
                         where DonViId == donvi.DONVI_ID.Value
                         && donvi.VANBANDI_ID.HasValue
                         select donvi.VANBANDI_ID.Value;
            return result.ToList();
        }
    }
}

