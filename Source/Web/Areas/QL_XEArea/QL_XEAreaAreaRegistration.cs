using System.Web.Mvc;

namespace Web.Areas.QL_XEArea
{
    public class QL_XEAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QL_XEArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QL_XEArea_default",
                "QL_XEArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}