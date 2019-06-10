using System.Web.Mvc;

namespace Web.Areas.ChiaSeTaiLieuArea
{
    public class ChiaSeTaiLieuAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ChiaSeTaiLieuArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ChiaSeTaiLieuArea_default",
                "ChiaSeTaiLieuArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}