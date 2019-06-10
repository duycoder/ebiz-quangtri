using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Custom;
using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CHIASETAILIEU;
using Business.CommonModel.CONSTANT;
using Web.FwCore;
using Web.Areas.ChiaSeTaiLieuArea.Models;
using System.Configuration;
using Model.Entities;
using CommonHelper;

namespace Web.Areas.ChiaSeTaiLieuArea.Controllers
{
    public class ShareController : BaseController
    {
        // GET: ChiaSeTaiLieuArea/Request
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private ChiaSeTaiLieuBusiness ChiaSeTaiLieuBusiness;
        private UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Request(int pageIndex = 1, int pageSize = 20)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            ChiaSeTaiLieuViewModel model = new ChiaSeTaiLieuViewModel();
            var listUser = DM_NGUOIDUNGBusiness.GetByRole(ConfigurationManager.AppSettings["RoleShare"], user.ID);
            model.ListUserRequest = listUser;
            model.ListUserShare = listUser;
            var searchModel = new ChiaSeTaiLieuSearchModel();
            searchModel.USER_YEU_CAU = user.ID;
            model.PageList = ChiaSeTaiLieuBusiness.GetPage(searchModel, pageIndex, pageSize);
            model.ListStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Chờ phê duyệt yêu cầu",Value=SHARE_STATUS_CONSTANT.YEU_CAU_CHIA_SE.ToString() },
                new SelectListItem{Text="Chờ chia sẻ",Value=SHARE_STATUS_CONSTANT.PHE_DUYET_CHIA_SE.ToString() },
                new SelectListItem{Text="Đã chia sẻ",Value=SHARE_STATUS_CONSTANT.DA_CHIA_SE.ToString() },
            };
            SessionManager.Remove("ShareRequestSearchModel");
            return View(model);
        }
        public ActionResult Shared(int pageIndex = 1, int pageSize = 20)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            ChiaSeTaiLieuViewModel model = new ChiaSeTaiLieuViewModel();
            var listUser = DM_NGUOIDUNGBusiness.GetByRole(ConfigurationManager.AppSettings["RoleShare"], user.ID);
            model.ListUserRequest = listUser;
            model.ListUserShare = listUser;
            var searchModel = new ChiaSeTaiLieuSearchModel();
            searchModel.USER_CHIA_SE = user.ID;
            model.PageList = ChiaSeTaiLieuBusiness.GetPage(searchModel, pageIndex, pageSize);
            model.ListStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Chờ chia sẻ",Value=SHARE_STATUS_CONSTANT.PHE_DUYET_CHIA_SE.ToString() },
                new SelectListItem{Text="Đã chia sẻ",Value=SHARE_STATUS_CONSTANT.DA_CHIA_SE.ToString() },
            };
            SessionManager.Remove("SharedSearchModel");
            return View(model);
        }
        public ActionResult Approve(int pageIndex = 1, int pageSize = 20)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            ChiaSeTaiLieuViewModel model = new ChiaSeTaiLieuViewModel();
            var listUser = DM_NGUOIDUNGBusiness.GetByRole(ConfigurationManager.AppSettings["RoleShare"], user.ID);
            model.ListUserRequest = listUser;
            model.ListUserShare = listUser;
            model.PageList = ChiaSeTaiLieuBusiness.GetPage(null, pageIndex, pageSize);
            model.ListStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Chờ phê duyệt yêu cầu",Value=SHARE_STATUS_CONSTANT.YEU_CAU_CHIA_SE.ToString() },
                new SelectListItem{Text="Chờ chia sẻ",Value=SHARE_STATUS_CONSTANT.PHE_DUYET_CHIA_SE.ToString() },
                new SelectListItem{Text="Đã chia sẻ",Value=SHARE_STATUS_CONSTANT.DA_CHIA_SE.ToString() },
            };
            SessionManager.Remove("ShareApproveSearchModel");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchRequest(FormCollection form)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var searchModel = SessionManager.GetValue("ShareRequestSearchModel") as ChiaSeTaiLieuSearchModel;

            if (searchModel == null)
            {
                searchModel = new ChiaSeTaiLieuSearchModel();
                searchModel.pageSize = 20;
                searchModel.USER_YEU_CAU = user.ID;
            }
            searchModel.USER_CHIA_SE = form["USER_CHIA_SE"].ToIntOrNULL();
            searchModel.STATUS = form["STATUS"].ToIntOrNULL();
            searchModel.KEYWORD = form["KEYWORD"];
            SessionManager.SetValue("ShareRequestSearchModel", searchModel);
            var data = ChiaSeTaiLieuBusiness.GetPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchApprove(FormCollection form)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var searchModel = SessionManager.GetValue("ShareApproveSearchModel") as ChiaSeTaiLieuSearchModel;

            if (searchModel == null)
            {
                searchModel = new ChiaSeTaiLieuSearchModel();
                searchModel.pageSize = 20;
            }
            searchModel.USER_YEU_CAU = form["USER_YEU_CAU"].ToIntOrNULL();
            searchModel.STATUS = form["STATUS"].ToIntOrNULL();
            searchModel.KEYWORD = form["KEYWORD"];
            SessionManager.SetValue("ShareApproveSearchModel", searchModel);
            var data = ChiaSeTaiLieuBusiness.GetPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchShared(FormCollection form)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var searchModel = SessionManager.GetValue("SharedSearchModel") as ChiaSeTaiLieuSearchModel;

            if (searchModel == null)
            {
                searchModel = new ChiaSeTaiLieuSearchModel();
                searchModel.pageSize = 20;
                searchModel.USER_CHIA_SE = user.ID;
            }
            searchModel.USER_YEU_CAU = form["USER_YEU_CAU"].ToIntOrNULL();
            searchModel.STATUS = form["STATUS"].ToIntOrNULL();
            searchModel.KEYWORD = form["KEYWORD"];
            SessionManager.SetValue("SharedSearchModel", searchModel);
            var data = ChiaSeTaiLieuBusiness.GetPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveApprove(FormCollection form)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();

            var share = new CHIASE_TAILIEU();
            if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0)
            {
                share = ChiaSeTaiLieuBusiness.Find(Convert.ToInt32(form["ID"]));
            }
            share.USER_PHE_DUYET = user.ID;
            share.USER_CHIA_SE = form["USER_CHIA_SE"].ToIntOrNULL();
            share.NOIDUNG_PHEDUYET = form["NOIDUNG_PHEDUYET"];
            share.STATUS = SHARE_STATUS_CONSTANT.PHE_DUYET_CHIA_SE;
            share.DATE_PHE_DUYET = DateTime.Now;
            ChiaSeTaiLieuBusiness.Save(share);
            var result = new JsonResultBO(true);
            return Json(new { result}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveShare(FormCollection form)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();

            var share = new CHIASE_TAILIEU();
            if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0)
            {
                share = ChiaSeTaiLieuBusiness.Find(Convert.ToInt32(form["ID"]));
            }
            share.DATE_CHIA_SE = DateTime.Now;
            share.NOIDUNG_CHIASE = form["NOIDUNG_CHIASE"];
            share.STATUS = SHARE_STATUS_CONSTANT.DA_CHIA_SE;
            ChiaSeTaiLieuBusiness.Save(share);
            var result = new JsonResultBO(true);
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveRequest(FormCollection form)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();

            var share = new CHIASE_TAILIEU();
            share.DATE_YEU_CAU = DateTime.Now;
            share.USER_YEU_CAU = user.ID;
            share.STATUS = SHARE_STATUS_CONSTANT.YEU_CAU_CHIA_SE;
            if (!string.IsNullOrEmpty(form["ID"]) && Convert.ToInt32(form["ID"]) > 0)
            {
                share = ChiaSeTaiLieuBusiness.Find(Convert.ToInt32(form["ID"]));
            }
            var result = new JsonResultBO(true);
            result.Message = "Thêm mới yêu cầu chia sẻ tài liệu thành công!";
            if (share.ID > 0)
            {
                result.Message = "Cập nhật yêu cầu chia sẻ tài liệu thành công!";
            }
            share.TIEUDE = form["TieuDe"];
            share.NOIDUNG_YEUCAU = form["NOIDUNG_YEUCAU"];
            ChiaSeTaiLieuBusiness.Save(share);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getApproveRequest(int indexPage, string sortQuery, int pageSize)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var searchModel = SessionManager.GetValue("ShareApproveSearchModel") as ChiaSeTaiLieuSearchModel;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new ChiaSeTaiLieuSearchModel();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("ShareApproveSearchModel", searchModel);
            }
            var data = ChiaSeTaiLieuBusiness.GetPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getDataShared(int indexPage, string sortQuery, int pageSize)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var searchModel = SessionManager.GetValue("SharedSearchModel") as ChiaSeTaiLieuSearchModel;
            if (searchModel == null)
            {
                searchModel = new ChiaSeTaiLieuSearchModel();
                searchModel.USER_CHIA_SE = user.ID;
            }
            if (!string.IsNullOrEmpty(sortQuery))
            {
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("SharedSearchModel", searchModel);
            }
            var data = ChiaSeTaiLieuBusiness.GetPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        [HttpPost]
        public JsonResult getDataRequest(int indexPage, string sortQuery, int pageSize)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var searchModel = SessionManager.GetValue("ShareRequestSearchModel") as ChiaSeTaiLieuSearchModel;
            if (searchModel == null)
            {
                searchModel = new ChiaSeTaiLieuSearchModel();
                searchModel.USER_YEU_CAU = user.ID;
            }
            if (!string.IsNullOrEmpty(sortQuery))
            {
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("ShareRequestSearchModel", searchModel);
            }
            var data = ChiaSeTaiLieuBusiness.GetPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        public JsonResult delete(int? id = 0)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            ChiaSeTaiLieuBusiness.repository.Delete(id);
            ChiaSeTaiLieuBusiness.Save();
            return Json(new JsonResultBO(true), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult FormRequest(int? id = 0)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var model = new ShareRequestFormModel();
            model.Share = new CHIASE_TAILIEU();
            model.Share.DATE_YEU_CAU = DateTime.Now;
            if (id > 0)
            {
                model.Share = ChiaSeTaiLieuBusiness.Find(id);
            }
            return PartialView("_FormRequest", model);
        }

        public PartialViewResult FormApprove(int? id = 0)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var model = new ShareApproveFormModel();
            model.Share = new ChiaSeTaiLieuBO();
            model.Share = ChiaSeTaiLieuBusiness.GetBO(id);
            model.Share.DATE_PHE_DUYET = DateTime.Now;
            model.ListUser = DM_NGUOIDUNGBusiness.GetByRole(ConfigurationManager.AppSettings["RoleShare"], user.ID);
            return PartialView("_FormApprove", model);
        }

        public PartialViewResult FormShare(int? id = 0)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var model = new ShareApproveFormModel();
            model.Share = new ChiaSeTaiLieuBO();
            model.Share = ChiaSeTaiLieuBusiness.GetBO(id);
            model.Share.DATE_CHIA_SE = DateTime.Now;
            return PartialView("_FormShare", model);
        }

        public PartialViewResult DetailShare(int? id = 0)
        {
            ChiaSeTaiLieuBusiness = Get<ChiaSeTaiLieuBusiness>();
            var model = new ShareApproveFormModel();
            model.Share = new ChiaSeTaiLieuBO();
            model.Share = ChiaSeTaiLieuBusiness.GetBO(id);
            return PartialView("_DetailShare", model);
        }
    }
}