using System.Web.Mvc;

namespace Web.Areas.QL_DANGKY_XEArea
{
    public class QL_DANGKY_XEAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QL_DANGKY_XEArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QL_DANGKY_XEArea_default",
                "QL_DANGKY_XEArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}