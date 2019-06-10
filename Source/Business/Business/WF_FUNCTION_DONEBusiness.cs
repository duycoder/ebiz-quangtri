using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
namespace Business.Business
{
    public class WF_FUNCTION_DONEBusiness:BaseBusiness<WF_FUNCTION_DONE>
    {
        public WF_FUNCTION_DONEBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

    
    }
}
