using System.Web.Mvc;

namespace Web.Areas.QL_LAIXEArea
{
    public class QL_LAIXEAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QL_LAIXEArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QL_LAIXEArea_default",
                "QL_LAIXEArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}