using System.Web.Mvc;
namespace Web.Areas.DMNHOMDANHMUCArea
{
public class DMNHOMDANHMUCAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "DMNHOMDANHMUCArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("DMNHOMDANHMUCArea_default","DMNHOMDANHMUCArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
