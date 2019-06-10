using System.Web.Mvc;

namespace Web.Areas.HSCV_VANBANPHATHANHArea
{
    public class HSCV_VANBANPHATHANHAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HSCV_VANBANPHATHANHArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HSCV_VANBANPHATHANHArea_default",
                "HSCV_VANBANPHATHANHArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}