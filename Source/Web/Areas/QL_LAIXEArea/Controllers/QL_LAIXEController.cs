using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.QLLAIXE;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.QL_LAIXEArea.Models;
using Web.Custom;
using Web.Filter;
using Web.FwCore;

namespace Web.Areas.QL_LAIXEArea.Controllers
{
    public class QL_LAIXEController : BaseController
    {
        private QL_LAIXEBusiness qlLaiXeBusiness;

        [ActionAudit]
        public ActionResult Index()
        {
            AssignUserInfo();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            LaiXeBenhVienIndexViewModel viewModel = new LaiXeBenhVienIndexViewModel();
            LaiXeSearchBO searchModel = new LaiXeSearchBO();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            viewModel.listLaiXeBenhViens = qlLaiXeBusiness.GetDataByPage(searchModel);
            SessionManager.SetValue("SearchLaiXeBenhVien", searchModel);
            return View(viewModel);
        }

        [ActionAudit]
        public JsonResult GetData(int pageIndex, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            LaiXeSearchBO searchModel = (LaiXeSearchBO)SessionManager.GetValue("SearchLaiXeBenhVien");
            if (searchModel == null)
            {
                searchModel = new LaiXeSearchBO();
            }
            searchModel.sortQuery = sortQuery;
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            PageListResultBO<LaiXeBO> data = qlLaiXeBusiness.GetDataByPage(searchModel);
            return Json(data);
        }

        [ActionAudit]
        [HttpPost]
        public JsonResult SearchData(FormCollection fc)
        {
            AssignUserInfo();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            var searchModel = (LaiXeSearchBO)SessionManager.GetValue("SearchLaiXeBenhVien");
            searchModel.HOTEN = fc["HOTEN"];
            searchModel.CMND = fc["CMND"];
            searchModel.SODIENTHOAI = fc["SODIENTHOAI"];
            searchModel.EMAIL = fc["EMAIL"];
            searchModel.GIOITINH = string.IsNullOrEmpty(fc["GIOITINH"]) ? null : (bool?)bool.Parse(fc["GIOITINH"]);
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            SessionManager.SetValue("SearchLaiXeBenhVien", searchModel);
            var result = qlLaiXeBusiness.GetDataByPage(searchModel);
            return Json(result);
        }

        [ActionAudit]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// @author: duynn
        /// @description: tạo mới lái xe
        /// @since: 27/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult Create()
        {
            LaiXeBenhVienEditViewModel model = new LaiXeBenhVienEditViewModel();
            return PartialView("_Edit", model);
        }

        // POST: QL_XEArea/QL_XE/Create
        [HttpPost]
        [ActionAudit]
        public JsonResult Save(FormCollection collection)
        {
            AssignUserInfo();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            JsonResultBO result = new JsonResultBO(true);
            try
            {
                QL_LAIXE laiXeEntity = new QL_LAIXE();
                laiXeEntity.HOTEN = collection["HOTEN"].Trim();
                laiXeEntity.CMND = collection["CMND"].Trim();
                laiXeEntity.SODIENTHOAI = collection["SODIENTHOAI"].Trim();
                laiXeEntity.EMAIL = collection["EMAIL"].Trim();
                laiXeEntity.GIOITINH = bool.Parse(collection["GIOITINH"]);
                laiXeEntity.NGUOISUA = currentUser.ID;
                laiXeEntity.NGAYSUA = DateTime.Now;
                laiXeEntity.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
                int ID = collection["ID"].ToIntOrZero();
                if (ID > 0)
                {
                    QL_LAIXE dbEntity = qlLaiXeBusiness.Find(ID);
                    if (dbEntity != null)
                    {
                        dbEntity.HOTEN = laiXeEntity.HOTEN;
                        dbEntity.CMND = laiXeEntity.CMND;
                        dbEntity.SODIENTHOAI = laiXeEntity.SODIENTHOAI;
                        dbEntity.EMAIL = laiXeEntity.EMAIL;
                        dbEntity.GIOITINH = laiXeEntity.GIOITINH;
                        dbEntity.CCTC_THANHPHAN_ID = laiXeEntity.CCTC_THANHPHAN_ID;
                        QL_LAIXE existedDriver = qlLaiXeBusiness.context.QL_LAIXE.Where(x => x.IS_DELETE != true 
                            && x.CMND == laiXeEntity.CMND 
                            && x.ID != dbEntity.ID).FirstOrDefault();

                        if (existedDriver != null)
                        {
                            result.Status = false;
                            result.Message = "CMND của lái xe đã tồn tại";
                            return Json(result);
                        }
                        qlLaiXeBusiness.Save(dbEntity);
                        result.Message = "Cập nhật thông tin lái xe thành công";
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Thông tin lái xe không tồn tại";
                        return Json(result);
                    }
                }
                else
                {
                    QL_LAIXE existedDriver = qlLaiXeBusiness.context.QL_LAIXE.Where(x => x.IS_DELETE != true && x.CMND == laiXeEntity.CMND).FirstOrDefault();
                    if(existedDriver != null)
                    {
                        result.Status = false;
                        result.Message = "CMND của lái xe đã tồn tại";
                        return Json(result);
                    }
                    laiXeEntity.NGUOITAO = currentUser.ID;
                    laiXeEntity.NGAYSUA = DateTime.Now;
                    qlLaiXeBusiness.Save(laiXeEntity);
                    result.Message = "Thêm mới thông tin lái xe thành công";
                }
                return Json(result);
            }
            catch(Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
                return Json(result);
            }
        }

        [ActionAudit]
        public PartialViewResult Edit(int id)
        {
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            QL_LAIXE entity = qlLaiXeBusiness.Find(id) ?? new QL_LAIXE();
            LaiXeBenhVienEditViewModel model = new LaiXeBenhVienEditViewModel(entity);
            return PartialView("_Edit", model);
        }

        [ActionAudit]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            JsonResultBO result = new JsonResultBO(true);
            QL_LAIXE dbEntity = qlLaiXeBusiness.Find(id);
            if (dbEntity != null)
            {
                dbEntity.IS_DELETE = true;
                qlLaiXeBusiness.Save(dbEntity);
            }
            else
            {
                result.Status = false;
                result.Message = "Không tìm thấy lái xe";
            }
            return Json(result);
        }
    }
}
