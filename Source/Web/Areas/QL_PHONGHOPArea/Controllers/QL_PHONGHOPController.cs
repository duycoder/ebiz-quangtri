using Business.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Custom;
using Web.FwCore;
using Business.CommonModel.DS_PHONGHOP;
using Business.CommonBusiness;
using Model.Entities;
using Web.Areas.QL_PHONGHOPArea.Models;

namespace Web.Areas.QL_PHONGHOPArea.Controllers
{
    public class QL_PHONGHOPController : BaseController
    {

        QL_PHONGHOPBusiness QL_PHONGHOPBusiness;
        public ActionResult Index()
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();

           var data = QL_PHONGHOPBusiness.GetDaTaByPage(currentUser.DeptParentID ,null);
            SessionManager.SetValue("TimKiemPhong", null);
            return View(data);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            var searchModel = SessionManager.GetValue("TimKiemPhong") as QLPHONGHOP_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new QLPHONGHOP_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("TimKiemPhong", searchModel);
            }

            var data = QL_PHONGHOPBusiness.GetDaTaByPage(currentUser.DeptParentID,searchModel, pageSize, indexPage);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            var searchModel = SessionManager.GetValue("TimKiemPhong") as QLPHONGHOP_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new QLPHONGHOP_SEARCHBO();
                searchModel.pageSize = 20;
            }

            searchModel.TenPhong = form["sea_TenPhong"];
            searchModel.MaPhong = form["sea_MaPhong"];;
            SessionManager.SetValue("TimKiemPhong", searchModel);

            var data = QL_PHONGHOPBusiness.GetDaTaByPage(currentUser.DeptParentID, searchModel, searchModel.pageSize, 1);
            return Json(data);
        }

        public PartialViewResult Create()
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            var model = new CreateVM();
            model.Object = QL_PHONGHOPBusiness.GetPhong(currentUser.DeptParentID);
            return PartialView("_Create", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult CreatePhong(string tenphongid, string maphongid, string sochongoiid, string motaid)
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            var result = new JsonResultBO(true);

            try
            {
                var modelPhong = new QL_PHONGHOP();
                //Kiểm tra Input null không
                if (tenphongid == null || maphongid == null || sochongoiid == null || motaid == null)
                {
                    result.Status = false;
                    result.Message = "Không cập nhật được";
                }
                modelPhong.TENPHONG = tenphongid;
                modelPhong.MAPHONG = maphongid;
                modelPhong.SOCHONGOI = int.Parse(sochongoiid);
                modelPhong.MOTA = motaid;
                modelPhong.DEPID = currentUser.DeptParentID;

                QL_PHONGHOPBusiness.Save(modelPhong);

            }
            catch
            {
                result.Status = false;
                result.Message = "Không thêm mới được";
            }
            return Json(result);
        }

        public PartialViewResult Edit(long id)
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            var myModel = new EditVM();
            myModel.Object = QL_PHONGHOPBusiness.repository.Find(id);
            myModel.LstPhong = QL_PHONGHOPBusiness.GetPhong(currentUser.DeptParentID);

            return PartialView("_EditPartial", myModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditPhong(int id, string TenPhongIdEdit, string MaPhongIdEdit, string SoChoNgoiIdEdit, string MoTaIdEdit)
        {
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = QL_PHONGHOPBusiness.Find(id);
                if (TenPhongIdEdit == null || MaPhongIdEdit == null || MoTaIdEdit == null || SoChoNgoiIdEdit == null)
                {
                    result.Status = false;
                    result.Message = "Không cập nhật được";
                }
                
                    myobj.TENPHONG = TenPhongIdEdit;
                    myobj.MAPHONG = MaPhongIdEdit;
                    myobj.MOTA = MoTaIdEdit;
                    myobj.SOCHONGOI = int.Parse(SoChoNgoiIdEdit);

                    QL_PHONGHOPBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        
        }
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            QL_PHONGHOPBusiness.repository.Delete(id);
            QL_PHONGHOPBusiness.Save();
            return Json(result);
        }

    }
}