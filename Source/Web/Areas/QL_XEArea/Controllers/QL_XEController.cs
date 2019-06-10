using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.QLXE;
using CommonHelper;
using CommonHelper.Upload;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.QL_XEArea.Models;
using Web.Custom;
using Web.Filter;
using Web.FwCore;

namespace Web.Areas.QL_XEArea.Controllers
{
    public class QL_XEController : BaseController
    {
        private QL_XEBusiness qlXeBusiness;
        private string UPLOAD_PATH = WebConfigurationManager.AppSettings["FileUpload"];
        // GET: QL_XEArea/QL_XE
        [ActionAudit]
        public ActionResult Index()
        {
            AssignUserInfo();
            qlXeBusiness = Get<QL_XEBusiness>();
            XeBenhVienIndexViewModel viewModel = new XeBenhVienIndexViewModel();
            XeSearchBO searchModel = new XeSearchBO();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            viewModel.listXeBenhViens = qlXeBusiness.GetDataByPage(searchModel);
            SessionManager.SetValue("SearchXeBenhVien", searchModel);
            return View(viewModel);
        }

        [ActionAudit]
        public JsonResult GetData(int pageIndex, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            qlXeBusiness = Get<QL_XEBusiness>();
            XeSearchBO searchModel = (XeSearchBO)SessionManager.GetValue("SearchXeBenhVien");
            if (searchModel == null)
            {
                searchModel = new XeSearchBO();
            }
            searchModel.sortQuery = sortQuery;
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            PageListResultBO<XeBO> data = qlXeBusiness.GetDataByPage(searchModel);
            return Json(data);
        }

        [ActionAudit]
        [HttpPost]
        public JsonResult SearchData(FormCollection fc)
        {
            AssignUserInfo();
            qlXeBusiness = Get<QL_XEBusiness>();
            var searchModel = (XeSearchBO)SessionManager.GetValue("SearchXeBenhVien");
            searchModel.TENXE = fc["TENXE"];
            searchModel.BIENSO = fc["BIENSO"];
            searchModel.querySoChoEnd = fc["querySoChoEnd"].ToIntOrNULL();
            searchModel.querySoChoStart = fc["querySoChoStart"].ToIntOrNULL();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            SessionManager.SetValue("SearchXeBenhVien", searchModel);
            var result = qlXeBusiness.GetDataByPage(searchModel);
            return Json(result);
        }

        // GET: QL_XEArea/QL_XE/Details/5
        [ActionAudit]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: QL_XEArea/QL_XE/Create
        /// <summary>
        /// @author: duynn
        /// @description: tạo mới xe
        /// @since: 27/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult Create()
        {
            XeBenhVienEditViewModel model = new XeBenhVienEditViewModel();
            return PartialView("_Edit", model);
        }

        // POST: QL_XEArea/QL_XE/Create
        [ActionAudit]
        [HttpPost]
        public ActionResult Save(FormCollection collection, IEnumerable<HttpPostedFileBase> filebase)
        {
            AssignUserInfo();
            qlXeBusiness = Get<QL_XEBusiness>();
            JsonResultBO result = new JsonResultBO(true);
            try
            {
                var file = filebase.FirstOrDefault();

                QL_XE xeEntity = new QL_XE();
                xeEntity.TENXE = collection["TENXE"].Trim();
                xeEntity.BIENSO = collection["BIENSO"].Trim();
                xeEntity.SOCHO = collection["SOCHO"].ToIntOrZero();
                xeEntity.GHICHU = collection["GHICHU"].Trim();
                xeEntity.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
                xeEntity.NGUOISUA = currentUser.ID;
                xeEntity.NGAYSUA = DateTime.Now;
                int ID = collection["ID"].ToIntOrZero();
                if (ID > 0)
                {
                    QL_XE dbEntity = qlXeBusiness.Find(ID);
                    if (dbEntity != null)
                    {
                        dbEntity.TENXE = xeEntity.TENXE;
                        dbEntity.BIENSO = xeEntity.BIENSO;
                        dbEntity.SOCHO = xeEntity.SOCHO;
                        dbEntity.GHICHU = xeEntity.GHICHU;
                        dbEntity.CCTC_THANHPHAN_ID = xeEntity.CCTC_THANHPHAN_ID;
                        QL_XE existedCar = qlXeBusiness.context.QL_XE.Where(x => x.IS_DELETE != true
                            && x.BIENSO == xeEntity.BIENSO && x.ID != dbEntity.ID)
                            .FirstOrDefault();
                        if (existedCar != null)
                        {
                            TempData["EditMessage"] = "Biển số xe đã tồn tại";
                            TempData["Status"] = false;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            existedCar = qlXeBusiness.context.QL_XE.Where(x => x.IS_DELETE != true
                            && x.TENXE == xeEntity.TENXE && x.ID != dbEntity.ID)
                            .FirstOrDefault();

                            if (existedCar != null)
                            {
                                TempData["EditMessage"] = "Tên xe đã tồn tại";
                                TempData["Status"] = false;
                                return RedirectToAction("Index");
                            }
                        }
                        qlXeBusiness.Save(dbEntity);

                        if (file != null && file.ContentLength > 0)
                        {
                            var saveFileResult = UploadProvider.SaveFile(file, null, ".png,.gif,.jpeg,.jpg,.PNG,.GIF,.JPEG,.JPG", null, "CarImage\\" + dbEntity.ID, UPLOAD_PATH);
                            if (saveFileResult.status)
                            {
                                dbEntity.IMAGE_PATH = saveFileResult.path;
                                qlXeBusiness.Save(dbEntity);
                            }
                        }

                        TempData["EditMessage"] = "Cập nhật xe thành công";
                        TempData["Status"] = true;
                    }
                    else
                    {
                        TempData["EditMessage"] = "Xe không tồn tại";
                        TempData["Status"] = false;
                    }
                }
                else
                {
                    QL_XE existedCar = qlXeBusiness.context.QL_XE.Where(x => x.IS_DELETE != true && x.BIENSO == xeEntity.BIENSO).FirstOrDefault();
                    if (existedCar != null)
                    {
                        TempData["EditMessage"] = "Biển số xe đã tồn tại";
                        TempData["Status"] = false;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        existedCar = qlXeBusiness.context.QL_XE.Where(x => x.IS_DELETE != true
                        && x.TENXE == xeEntity.TENXE)
                        .FirstOrDefault();

                        if (existedCar != null)
                        {
                            TempData["EditMessage"] = "Tên xe đã tồn tại";
                            TempData["Status"] = false;
                            return RedirectToAction("Index");
                        }
                    }

                    xeEntity.NGUOITAO = currentUser.ID;
                    xeEntity.NGAYSUA = DateTime.Now;
                    qlXeBusiness.Save(xeEntity);

                    if (file != null && file.ContentLength > 0)
                    {
                        var saveFileResult = UploadProvider.SaveFile(file, null, ".png,.gif,.jpeg,.jpg,.PNG,.GIF,.JPEG,.JPG", null, "CarImage\\" + xeEntity.ID, UPLOAD_PATH);
                        if (saveFileResult.status)
                        {
                            xeEntity.IMAGE_PATH = saveFileResult.path;
                            qlXeBusiness.Save(xeEntity);
                        }
                    }

                    TempData["EditMessage"] = "Thêm mới xe thành công";
                    TempData["Status"] = true;
                }
            }
            catch (Exception ex)
            {
                TempData["EditMessage"] = ex.Message;
                TempData["Status"] = false;
            }
            return RedirectToAction("Index");
        }

        // GET: QL_XEArea/QL_XE/Edit/5
        [ActionAudit]
        public PartialViewResult Edit(int id)
        {
            qlXeBusiness = Get<QL_XEBusiness>();
            QL_XE entity = qlXeBusiness.Find(id) ?? new QL_XE();
            XeBenhVienEditViewModel model = new XeBenhVienEditViewModel(entity);
            return PartialView("_Edit", model);
        }

        // GET: QL_XEArea/QL_XE/Delete/5
        [ActionAudit]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            qlXeBusiness = Get<QL_XEBusiness>();
            JsonResultBO result = new JsonResultBO(true);
            QL_XE dbEntity = qlXeBusiness.Find(id);
            if (dbEntity != null)
            {
                dbEntity.IS_DELETE = true;
                qlXeBusiness.Save(dbEntity);
            }
            else
            {
                result.Status = false;
                result.Message = "Không tìm thấy xe";
            }
            return Json(result);
        }
    }
}
