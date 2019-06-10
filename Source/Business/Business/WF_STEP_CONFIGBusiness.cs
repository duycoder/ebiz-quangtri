using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;

namespace Business.Business
{
    public class WF_STEP_CONFIGBusiness:BaseBusiness<WF_STEP_CONFIG>
    {
        public WF_STEP_CONFIGBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public WF_STEP_CONFIG GetConfigStep(int idStep)
        {
            var query = this.context.WF_STEP_CONFIG.Where(x => x.WF_STEP_ID == idStep).FirstOrDefault();
            return query;
        }
    }
}
