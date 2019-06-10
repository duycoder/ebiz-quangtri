using System.Web.Mvc;

namespace Web.Areas.QLPHONGHOPArea
{
    public class QLPHONGHOPAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QLPHONGHOPArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QLPHONGHOPArea_default",
                "QLPHONGHOPArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}