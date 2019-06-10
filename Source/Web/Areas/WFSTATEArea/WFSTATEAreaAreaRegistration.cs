using System.Web.Mvc;
namespace Web.Areas.WFSTATEArea
{
public class WFSTATEAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "WFSTATEArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("WFSTATEArea_default","WFSTATEArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
