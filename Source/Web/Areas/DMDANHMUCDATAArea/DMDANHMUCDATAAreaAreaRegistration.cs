using System.Web.Mvc;
namespace Web.Areas.DMDANHMUCDATAArea
{
public class DMDANHMUCDATAAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "DMDANHMUCDATAArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("DMDANHMUCDATAArea_default","DMDANHMUCDATAArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
