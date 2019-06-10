using System.Web.Mvc;
namespace Web.Areas.DMVAITROArea
{
public class DMVAITROAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "DMVAITROArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("DMVAITROArea_default","DMVAITROArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
