using System.Web.Mvc;
namespace Web.Areas.CCTCTHANHPHANArea
{
public class CCTCTHANHPHANAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "CCTCTHANHPHANArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("CCTCTHANHPHANArea_default","CCTCTHANHPHANArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
