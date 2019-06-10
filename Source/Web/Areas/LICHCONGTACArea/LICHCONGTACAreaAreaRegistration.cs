using System.Web.Mvc;

namespace Web.Areas.LICHCONGTACArea
{
    public class LICHCONGTACAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LICHCONGTACArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LICHCONGTACArea_default",
                "LICHCONGTACArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}