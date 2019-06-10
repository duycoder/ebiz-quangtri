using System.Web.Mvc;

namespace Web.Areas.SYSTINNHANArea
{
    public class SYSTINNHANAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SYSTINNHANArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SYSTINNHANArea_default",
                "SYSTINNHANArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}