using Business.BaseBusiness;
using Model.Entities;
using System.Linq;

namespace Business.Business
{
    public class WF_REVIEW_USERBusiness : BaseBusiness<WF_USER_REVIEW>
    {
        public WF_REVIEW_USERBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public bool CheckPermissionReview(long ItemId, string itemType, long userId, long reviewId = 0)
        {
            var result = this.context.WF_USER_REVIEW.Where(x => x.ITEMID == ItemId && x.ITEMTYPE == itemType && x.USER_ID == userId);
            if (reviewId > 0)
            {
                result = result.Where(x => x.REVIEW_ID == reviewId);
            }
            var resultObj = result.FirstOrDefault();
            return resultObj != null;
        }
        public bool CheckReviewing(long ReviewId, long userId)
        {
            var result = this.context.WF_USER_REVIEW.Where(x => x.REVIEW_ID == ReviewId && x.USER_ID == userId && x.IS_APPROVE == null).FirstOrDefault();
            return result != null;
        }

        public bool CheckFinishReview(long ReviewId)
        {
            var result = this.context.WF_USER_REVIEW.Where(x => x.REVIEW_ID == ReviewId && x.IS_APPROVE == null)
                .FirstOrDefault();
            return result == null;
        }
    }
}

