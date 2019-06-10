using System.Web.Mvc;

namespace Web.Areas.HoSoCanBoArea
{
    public class HoSoCanBoAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HoSoCanBoArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HoSoCanBoArea_default",
                "HoSoCanBoArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}