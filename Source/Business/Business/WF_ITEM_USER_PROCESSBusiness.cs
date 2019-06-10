using Business.BaseBusiness;
using Model.Entities;
using System.Linq;
namespace Business.Business
{
    public class WF_ITEM_USER_PROCESSBusiness : BaseBusiness<WF_ITEM_USER_PROCESS>
    {
        public WF_ITEM_USER_PROCESSBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public bool CheckPermissionProcess(long ItemId, string itemType, long userId)
        {
            var result = this.context.WF_ITEM_USER_PROCESS.Where(x => x.ITEM_ID == ItemId && x.ITEM_TYPE == itemType && x.USER_ID == userId).FirstOrDefault();
            return result != null;
        }
    }
}
