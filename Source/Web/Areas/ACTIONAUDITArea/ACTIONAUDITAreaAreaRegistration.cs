using System.Web.Mvc;

namespace Web.Areas.ACTIONAUDITArea
{
    public class ACTIONAUDITAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ACTIONAUDITArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ACTIONAUDITArea_default",
                "ACTIONAUDITArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}