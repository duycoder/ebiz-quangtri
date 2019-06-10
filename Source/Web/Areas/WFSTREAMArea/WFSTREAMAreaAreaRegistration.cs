using System.Web.Mvc;
namespace Web.Areas.WFSTREAMArea
{
public class WFSTREAMAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "WFSTREAMArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("WFSTREAMArea_default","WFSTREAMArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
