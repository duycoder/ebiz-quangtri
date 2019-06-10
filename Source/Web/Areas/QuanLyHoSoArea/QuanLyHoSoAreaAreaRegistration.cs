using System.Web.Mvc;

namespace Web.Areas.QuanLyHoSoArea
{
    public class QuanLyHoSoAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QuanLyHoSoArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QuanLyHoSoArea_default",
                "QuanLyHoSoArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
