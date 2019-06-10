using Business.BaseBusiness;
using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVVANBANDEN;
using Business.CommonModel.HSCVVANBANDI;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;
using Web.FwCore;

namespace Web.Custom
{
    public class BaseController : Controller
    {
        protected UserInfoBO currentUser;
        //Context per request
        protected DBEntities Context;
        protected Dictionary<string, object> ListBusiness = new Dictionary<string, object>();
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        /// <summary>
        /// default execute action
        /// </summary>
        /// <param name="filterContext"></param>
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    //CAUHINH_HETHONG sc = Get<CauhinhHethongBusiness>().repository.All().Where(o => o.MA_CAU_HINH == "MAINTAIN_TIME_QL").FirstOrDefault();
        //    //if (sc != null && sc.GIA_TRI != null && !filterContext.HttpContext.Request.IsLocal)
        //    //{
        //    //    if (filterContext.ActionDescriptor.ActionName != "ServerMaintenance")
        //    //    {
        //    //        this.Response.Redirect("~/Home/ServerMaintenance");
        //    //    }
        //    //}
        //}
        /// <summary>
        /// Create new context if null
        /// </summary>
        public DBEntities GetContext()
        {
            if (Context == null)
            {
                Context = new DBEntities();
            }
            return Context;
        }

        protected void AssignUserInfo()
        {
            currentUser = this.GetUserInfo();
        }

        public UserInfoBO GetUserInfo()
        {
            return (UserInfoBO)SessionManager.GetUserInfo();
        }

        public B Get<B>()
        {
            if (ListBusiness == null)
            {
                ListBusiness = new Dictionary<string, object>();
            }

            var type = typeof(B);
            if (ListBusiness.ContainsKey(type.Name))
            {
                return (B)ListBusiness[type.Name];
            }
            try
            {
                B instance = (B)Activator.CreateInstance(type, unitOfWork);
                ListBusiness.Add(type.Name, instance);
                return instance;
            }
            catch (Exception)
            {

                return default(B);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
               || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
                if (!skipAuthorization)
                {

                    if (filterContext.HttpContext.Session.IsNewSession || filterContext.HttpContext.Session["UserInfo"] == null)
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {

                            if (((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.ReturnType == typeof(JsonResult))
                            {
                                var rs = new JsonResultBO(false);
                                rs.Message = "Phiên làm việc của bạn đã hết";
                                filterContext.Result = Json(rs);
                            }
                            else if (((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.ReturnType == typeof(PartialViewResult))
                            {
                                filterContext.Result =
                                RedirectToAction("TimeOutSession", "home", new { area = "" });
                            }

                        }
                        else
                        {
                            filterContext.Result =
                           RedirectToAction("login", "account", new { area = "" });
                        }

                        return;

                    }
                    else if (filterContext.HttpContext.Session["UserInfo"] != null)
                    {
                        AssignUserInfo();
                        var hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
                        var hscvVanBanDiBusiness = Get<HSCV_VANBANDIBusiness>();

                        HSCV_VANBANDEN_SEARCH searchVanBanDen = new HSCV_VANBANDEN_SEARCH();
                        searchVanBanDen.USER_ID = currentUser.ID;
                        searchVanBanDen.ITEM_TYPE = MODULE_CONSTANT.VANBANDEN;
                        var resultVanBanDen = hscvVanBanDenBusiness.GetListInProcess(searchVanBanDen, 10, 1);

                        HSCV_VANBANDI_SEARCH searchVanBanDi = new HSCV_VANBANDI_SEARCH();
                        searchVanBanDi.USER_ID = currentUser.ID;
                        searchVanBanDi.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
                        var resultVanBanDi = hscvVanBanDiBusiness.GetListProcessing(searchVanBanDi, 10, 1);

                        SessionManager.SetValue("ProcessingVanBanDenNumber", resultVanBanDen.Count);
                        SessionManager.SetValue("ProcessingVanBanDiNumber", resultVanBanDi.Count);
                    }
                }

            }
            base.OnActionExecuting(filterContext);
        }
        public static bool IsInActivities(List<DM_THAOTAC> list_thaotac, string ma_thaotac)
        {
            if (list_thaotac != null && list_thaotac.Count > 0)
            {
                var thaotac = list_thaotac.Where(o => o.MA_THAOTAC.ToUpper() == ma_thaotac.ToUpper()).FirstOrDefault();
                return thaotac != null && thaotac.DM_THAOTAC_ID > 0;
            }
            return false;
        }
        public void SetViewDataActionAudit(IDictionary<string, object> dic)
        {
            ViewData[CommonKey.AuditActionKey.OldJsonObject] = dic[UIConstant.ACTION_AUDIT_OLD_JSON];
            ViewData[CommonKey.AuditActionKey.NewJsonObject] = dic[UIConstant.ACTION_AUDIT_NEW_JSON];
            ViewData[CommonKey.AuditActionKey.ObjectID] = dic[UIConstant.ACTION_AUDIT_OBJECTID];
            ViewData[CommonKey.AuditActionKey.Description] = dic[UIConstant.ACTION_AUDIT_DESCRIPTION];
        }
        public void SetViewDataActionAudit(string oldJson, string newJson, string objectID, string description)
        {
            ViewData[CommonKey.AuditActionKey.OldJsonObject] = oldJson;
            ViewData[CommonKey.AuditActionKey.NewJsonObject] = newJson;
            ViewData[CommonKey.AuditActionKey.ObjectID] = objectID;
            ViewData[CommonKey.AuditActionKey.Description] = description;
        }

    }
}