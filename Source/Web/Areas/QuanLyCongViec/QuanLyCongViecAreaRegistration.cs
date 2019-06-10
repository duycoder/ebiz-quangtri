using System.Web.Mvc;

namespace Web.Areas.QuanLyCongViec
{
    public class QuanLyCongViecAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QuanLyCongViec";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QuanLyCongViec_default",
                "QuanLyCongViec/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}