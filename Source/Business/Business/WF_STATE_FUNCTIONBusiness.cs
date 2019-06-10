using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonModel.CONSTANT;

namespace Business.Business
{
    public class WF_STATE_FUNCTIONBusiness : BaseBusiness<WF_STATE_FUNCTION>
    {
        public WF_STATE_FUNCTIONBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public WF_STATE_FUNCTION GetStateFunction(int idState)
        {
            var query = this.context.WF_STATE_FUNCTION.Where(x => x.WF_STATE_ID == idState).FirstOrDefault();
            return query;
        }
        public WF_FUNCTION CheckGetFunction(int idState, long itemId,string ItemType)
        {
            var stateFunction = this.context.WF_STATE_FUNCTION.Where(x => x.WF_STATE_ID == idState).FirstOrDefault();
            if (stateFunction != null)
            {
                //kiểm tra xem function đã thực hiện chưa
                var done = this.context.WF_FUNCTION_DONE.Where(x => x.STATE == idState && x.ITEM_TYPE == ItemType && x.FUNCTION_STATE == stateFunction.ID && x.ITEM_ID == itemId).Any();
                if (done)
                {
                    //function đã thực hiện
                    return null;
                }
                else
                {
                    var query = (from statefunction in this.context.WF_STATE_FUNCTION
                                 where statefunction.WF_STATE_ID == idState
                                 join tblfunction in this.context.WF_FUNCTION on statefunction.ACTION equals tblfunction.ID
                                 select tblfunction).FirstOrDefault();
                    return query;
                }
            }
            else
            {
                return null;
            }
           
        }

        public bool CheckFunctionNextState(int idState, long itemId, string ItemType)
        {
            var stateFunction = this.context.WF_STATE_FUNCTION.Where(x => x.WF_STATE_ID == idState&&x.ACTION!=null).FirstOrDefault();
            if (stateFunction != null)
            {
                //kiểm tra xem function đã thực hiện chưa
                var done = this.context.WF_FUNCTION_DONE.Where(x => x.STATE == idState && x.ITEM_TYPE == ItemType && x.FUNCTION_STATE == stateFunction.ID && x.ITEM_ID == itemId).Any();
                if (done)
                {
                    //function đã thực hiện
                    return true;
                }
                else
                {
                    if (stateFunction.IS_BREAK==true)
                    {
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }

        }
    }
}
