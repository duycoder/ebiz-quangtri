using Business.Business;
using Business.CommonBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Web.FwCore;

namespace Web.Custom
{
    /// <summary>
    /// NAMDV - 24/01/2014
    /// This class use for preventing access to unauthorized Action
    /// </summary>
    public class Authorize
    {
        public static bool ValidateRequest(HttpContextBase httpContext)
        {
            // Write Authorize code here
            UserInfoBO UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            if (UserInfo == null)
            {
                FormsAuthentication.SignOut();
                SessionManager.Clear();
                httpContext.Response.Redirect("/Account/Login");
                return true;
            }

            string username = httpContext.User.Identity.Name;

            var rd = httpContext.Request.RequestContext.RouteData;

            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentArea = rd.DataTokens["area"] as string;

            string ActionPath = string.Format("/{0}/{1}/{2}", currentArea, currentController, currentAction);
            string ActionPathShort = string.Format("/{0}/{1}", currentArea, currentController);

            UserInfoBO userinfo = (UserInfoBO)SessionManager.GetUserInfo();
            //if (ActionPath.ToLower() == "//Home/Index".ToLower())// nếu nhiều vai trò chưa chọn vai trò thì không được vào trang Index
            //{
            //    if (userinfo.RoleID < 1)
            //    {
            //        httpContext.Response.Redirect("/Home/OptionRole");
            //        return true;
            //    }
            //}
            //if (ActionPath.ToLower() == "//Home/OptionRole".ToLower())// nếu có 1 vai trò thì không được vào trang chọn vai trò
            //{
            //    if (userinfo.ListRole == null)
            //    {
            //        httpContext.Response.Redirect("/Home/UnAuthor");
            //        return true;
            //    }
            //}
            if (ActionPath == "//Home/Index" ||
                ActionPath == "//Home/UnAuthor" ||
                ActionPathShort == "//AttachFile" ||
                ActionPath == "//Account/ChangePassword")
            {
                return true;
            }



            return true;
            //commented by namdv
            //comment date: 09/06
            //reson: hệ thống kiểm tra thao tác có cho phép người dùng sử dụng hay không?
            List<ThaoTacBO> lstTT = userinfo.ListThaoTac;

            ThaoTacBO tt = lstTT.Where(o => o.THAOTAC.ToLower() == ActionPath.ToLower()).FirstOrDefault();
            if (tt == null)
            {
                return false;
            }
            else
            {
                if (tt.listThoiGian.Count == 0) return true;

                DateTime currentTime = DateTime.Now;
                for (int i = 0; i < tt.listThoiGian.Count - 1; i = i + 2)
                {
                    bool pass = true;

                    DateTime? start = tt.listThoiGian[i];
                    DateTime? end = tt.listThoiGian[i + 1];
                    if (end != null)
                    {
                        end = end.Value.AddDays(1);
                    }

                    if (start != null && start > currentTime)
                    {
                        pass = false;
                    }

                    if (end != null && end <= currentTime)
                    {
                        pass = false;
                    }

                    if (pass)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}