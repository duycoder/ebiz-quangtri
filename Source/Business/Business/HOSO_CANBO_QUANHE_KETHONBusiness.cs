using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Business;
using Model.Entities;
using Business.BaseBusiness;
namespace Business.Business
{
  public class HOSO_CANBO_QUANHE_KETHONBusiness : BaseBusiness<HOSO_CANBO_QUANHEKETHON>
    {
      public HOSO_CANBO_QUANHE_KETHONBusiness(UnitOfWork unitofwork)
          :base(unitofwork)
      {

      }
    }
}
