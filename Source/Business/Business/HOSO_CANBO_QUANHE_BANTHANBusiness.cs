using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;
using Model.Entities;
namespace Business.Business
{
  public class HOSO_CANBO_QUANHE_BANTHANBusiness : BaseBusiness<HOSO_CANBO_QUANHEBANTHAN>
    {
      public HOSO_CANBO_QUANHE_BANTHANBusiness(UnitOfWork  unitofwork)
          :base(unitofwork)
      {

      }
    }
}
