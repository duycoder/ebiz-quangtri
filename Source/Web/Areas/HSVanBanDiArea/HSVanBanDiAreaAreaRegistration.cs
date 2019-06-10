using System.Web.Mvc;

namespace Web.Areas.HSVanBanDiArea
{
    public class HSVanBanDiAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HSVanBanDiArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HSVanBanDiArea_default",
                "HSVanBanDiArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}