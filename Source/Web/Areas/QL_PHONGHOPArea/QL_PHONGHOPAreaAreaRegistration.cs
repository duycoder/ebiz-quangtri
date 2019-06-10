using System.Web.Mvc;

namespace Web.Areas.QL_PHONGHOPArea
{
    public class QL_PHONGHOPAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QL_PHONGHOPArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QL_PHONGHOPArea_default",
                "QL_PHONGHOPArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}