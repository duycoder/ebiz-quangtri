using System.Web.Mvc;
namespace Web.Areas.DMCHUCNANGArea
{
public class DMCHUCNANGAreaAreaRegistration : AreaRegistration 
{
public override string AreaName
{get 
{
return "DMCHUCNANGArea";
}
}
 public override void RegisterArea(AreaRegistrationContext context) 
{
 context.MapRoute("DMCHUCNANGArea_default","DMCHUCNANGArea/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional } );
}
}
}
