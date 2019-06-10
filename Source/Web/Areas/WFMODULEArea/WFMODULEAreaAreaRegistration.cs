using System.Web.Mvc;
namespace Web.Areas.WFMODULEArea
{
public class WFMODULEAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "WFMODULEArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("WFMODULEArea_default","WFMODULEArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
