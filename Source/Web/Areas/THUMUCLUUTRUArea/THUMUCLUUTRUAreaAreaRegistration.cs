using System.Web.Mvc;

namespace Web.Areas.THUMUCLUUTRUArea
{
    public class THUMUCLUUTRUAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "THUMUCLUUTRUArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "THUMUCLUUTRUArea_default",
                "THUMUCLUUTRUArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}