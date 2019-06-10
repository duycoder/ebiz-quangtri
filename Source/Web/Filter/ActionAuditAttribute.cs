using System;
using System.Linq;
using System.Web.Mvc;
using Business.CommonBusiness;
using Web.FwCore;
using System.Net;
using System.Web.Helpers;
using Model.Entities;
using Web.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Web.Filter
{
    public class ActionAuditAttribute : FilterAttribute, IActionFilter
    {
        ACTION_AUDIT actionAudit;

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            actionAudit = new ACTION_AUDIT();

            actionAudit.CONTROLLER = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            actionAudit.ACTION = filterContext.ActionDescriptor.ActionName;
            actionAudit.USER_AGENT = filterContext.HttpContext.Request.UserAgent.Substring(0, 60);

            actionAudit.BEGIN_AUDIT_TIME = DateTime.Now;
            actionAudit.IP = filterContext.HttpContext.Request.UserHostAddress;
            actionAudit.DESCRIPTION = Dns.GetHostName();
            if (filterContext.ActionParameters.Count > 0)
            {
                var ParameterFirst = filterContext.ActionParameters.First();
                if (!typeof(LoginViewModel).IsInstanceOfType(ParameterFirst.Value))
                {
                    if (typeof(System.Web.HttpPostedFileBase[]).IsInstanceOfType(ParameterFirst.Value))
                    {
                        var fileFirst = (System.Web.HttpPostedFileBase[])ParameterFirst.Value;
                        if (fileFirst.First() != null)
                        {
                            var fileObj = fileFirst.First();
                            var tmpObj = new
                            {
                                FileName = fileObj.FileName,
                                ContentLength = fileObj.ContentLength,
                                ContentType = fileObj.ContentType
                            };
                            var DataObject = Json.Encode(tmpObj);
                            actionAudit.DESCRIPTION = actionAudit.DESCRIPTION + ". Thông tin Object:" + DataObject.ToString();
                        }
                    }
                    else
                    {
                        var DataObject = Json.Encode(ParameterFirst.Value);
                        actionAudit.DESCRIPTION = actionAudit.DESCRIPTION + ". Thông tin Object:" + DataObject.ToString();
                    }

                }
            }
            string clientIP = filterContext.HttpContext.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrWhiteSpace(clientIP))
            {
                actionAudit.IP = clientIP;
            }
            UserInfoBO loggedUser = filterContext.HttpContext.Session[SessionManager.USER_INFO] as UserInfoBO;
            if (loggedUser != null)
            {
                actionAudit.USER_NAME = loggedUser.TENDANGNHAP;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            UserInfoBO loggedUser = filterContext.HttpContext.Session[SessionManager.USER_INFO] as UserInfoBO;

            if (actionAudit == null) { return; }

            if (loggedUser != null && string.IsNullOrWhiteSpace(actionAudit.USER_NAME) && !string.IsNullOrWhiteSpace(loggedUser.TENDANGNHAP))
            {
                actionAudit.USER_NAME = loggedUser.TENDANGNHAP;
            }


            if (actionAudit.USER_NAME == null)
            {
                if (actionAudit.ACTION != "ExecuteChangePass")
                {
                    return;
                }
                else
                {
                    actionAudit.USER_NAME = "Error";
                }
            }

            actionAudit.END_AUDIT_TIME = DateTime.Now;
            LogAdapter.Insert(actionAudit);
        }
    }
}