using System.Web.Mvc;
namespace Web.Areas.WFSTEPArea
{
public class WFSTEPAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "WFSTEPArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("WFSTEPArea_default","WFSTEPArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
