using Business.BaseBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class HOSOCANBOCONGTACBusiness : BaseBusiness<HOSO_CANBO_CONGTAC>
    {
        public HOSOCANBOCONGTACBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
    }
}
