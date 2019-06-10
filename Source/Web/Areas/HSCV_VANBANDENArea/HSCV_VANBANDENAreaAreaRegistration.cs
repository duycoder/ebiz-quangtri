using System.Web.Mvc;

namespace Web.Areas.HSCV_VANBANDENArea
{
    public class HSCV_VANBANDENAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HSCV_VANBANDENArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HSCV_VANBANDENArea_default",
                "HSCV_VANBANDENArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}