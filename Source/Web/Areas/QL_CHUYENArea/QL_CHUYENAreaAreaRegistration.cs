using System.Web.Mvc;

namespace Web.Areas.QL_CHUYENArea
{
    public class QL_CHUYENAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QL_CHUYENArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QL_CHUYENArea_default",
                "QL_CHUYENArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}