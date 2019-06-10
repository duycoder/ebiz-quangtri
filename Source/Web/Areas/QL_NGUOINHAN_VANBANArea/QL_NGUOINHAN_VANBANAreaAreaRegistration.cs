using System.Web.Mvc;

namespace Web.Areas.QL_NGUOINHAN_VANBANArea
{
    public class QL_NGUOINHAN_VANBANAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QL_NGUOINHAN_VANBANArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QL_NGUOINHAN_VANBANArea_default",
                "QL_NGUOINHAN_VANBANArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}