using System.Web.Mvc;

namespace Web.Areas.LogSMSArea
{
    public class LogSMSAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LogSMSArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LogSMSArea_default",
                "LogSMSArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}