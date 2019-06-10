using System.Web.Mvc;

namespace Web.Areas.DMNguoiDungArea
{
    public class DMNguoiDungAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DMNguoiDungArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DMNguoiDungArea_default",
                "DMNguoiDungArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}