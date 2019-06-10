using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;
using Model.Entities;
namespace Business.Business
{
    public class QUATRINH_LUONG_CANBOBusiness :BaseBusiness<HOSO_CANBO_QUATRINH_LUONG>
    {
        public QUATRINH_LUONG_CANBOBusiness(UnitOfWork unitofwork) 
            :base(unitofwork)
        {

        }
    }
}
