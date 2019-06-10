using System.Web.Mvc;
namespace Web.Areas.DMTHAOTACArea
{
public class DMTHAOTACAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "DMTHAOTACArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("DMTHAOTACArea_default","DMTHAOTACArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
