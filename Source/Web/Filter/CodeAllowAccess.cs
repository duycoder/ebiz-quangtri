using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FwCore;
using Business.CommonBusiness;
using System.Web.Routing;
using Newtonsoft.Json;
using CommonHelper;
namespace Web.Filter
{
    public class CodeAllowAccess : ActionFilterAttribute, IActionFilter
    {
        //public List<string> lstCode { get; set; }
        public string Code { get; set; }
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userinfo = SessionManager.GetUserInfo() as UserInfoBO;
            var isAccess = true;
            if (userinfo != null)
            {
                var lstCOde = userinfo.ListThaoTac.Select(x => x.MA_THAOTAC).ToList();
                if (!lstCOde.Contains(Code))
                {
                    isAccess = false;
                }
                                
            }
            else
            {
                isAccess = false;
            }

            if (!isAccess)
            {

                if (((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.ReturnType == typeof(JsonResult))
                {
                    var rs = new JsonResultBO(false);
                    rs.Message = "Bạn không có quyền truy cập";
                    var jsresult = new JsonResult();
                    jsresult.ContentType = "json";
                    //jsresult.Data = JsonConvert.SerializeObject(rs);
                    jsresult.Data = rs;
                    filterContext.Result = jsresult;
                }
                else if (((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.ReturnType == typeof(PartialViewResult))
                {
                    filterContext.Result = new RedirectToRouteResult(new
                     RouteValueDictionary(new { controller = "Home", action = "UnAuthorPartial", area = "" }));
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                     RouteValueDictionary(new { controller = "Home", action = "UnAuthor", area = "" }));
                }

            }
        }
    }
}