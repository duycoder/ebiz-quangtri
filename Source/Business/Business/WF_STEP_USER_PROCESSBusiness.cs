using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.Business
{
    public class WF_STEP_USER_PROCESSBusiness : BaseBusiness<WF_STEP_USER_PROCESS>
    {
        public WF_STEP_USER_PROCESSBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public WF_STEP_USER_PROCESS GetMainProcess(int stepID)
        {
            var query = this.context.WF_STEP_USER_PROCESS.Where(x => x.IS_XULYCHINH == true && x.WF_STEP_ID == stepID).FirstOrDefault();
            return query;
        }
        public WF_STEP_USER_PROCESS GetJoinProcess(int stepID)
        {
            var query = this.context.WF_STEP_USER_PROCESS.Where(x => x.IS_XULYCHINH == false && x.WF_STEP_ID == stepID).FirstOrDefault();
            return query;
        }
        public void DeleteProcess(int stepId, bool IsMain)
        {
            var query = this.context.WF_STEP_USER_PROCESS.Where(x => x.IS_XULYCHINH == IsMain && x.WF_STEP_ID == stepId).FirstOrDefault();
            if (query != null)
            {
                this.repository.Delete(query.ID);
                Save();
            }

        }

    }
}
