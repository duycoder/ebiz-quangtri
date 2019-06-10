using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonHelper;
using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using Web.Custom;
using Web.FwCore;
using Business.CommonModel.DMVAITRO;
using Web.Areas.DMVAITROArea.Models;
using Web.Filter;

namespace Web.Areas.DMVAITROArea.Controllers
{
    public class DMVAITROController : BaseController
    {
        #region khaibao
        DM_VAITROBusiness DM_VAITROBusiness;
        DM_CHUCNANGBusiness DM_CHUCNANGBusiness;
        DM_THAOTACBusiness DM_THAOTACBusiness;
        NGUOIDUNG_VAITROBusiness NGUOIDUNG_VAITROBusiness;
        VAITRO_THAOTACBusiness VAITRO_THAOTACBusiness;
        #endregion
        [CodeAllowAccess(Code = "DsVaiTro")]
        public ActionResult Index()
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var searchmodel = new DM_VAITRO_SEARCHBO();
            SessionManager.SetValue("dmvaitroSearchModel", null);
            var data = DM_VAITROBusiness.GetDaTaByPage(null);
            return View(data);
        }

        public JsonResult SaveNguoiDungVaiTro(long idNguoiDung, List<string> listVaiTro)
        {

            NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            var listVaiTroInt = listVaiTro.Select(x => x.ToIntOrZero()).ToList();

            return Json(NGUOIDUNG_VAITROBusiness.UpdateVaiTroNguoiDung(idNguoiDung, listVaiTroInt));
        }
        public JsonResult SaveDataVaiTro(int idData, List<string> listVaiTro)
        {
            var DM_DANHMUC_DATA_VAITROBusiness = Get<DM_DANHMUC_DATA_VAITROBusiness>();
            var listVaiTroInt = listVaiTro.Select(x => x.ToIntOrZero()).ToList();

            return Json(DM_DANHMUC_DATA_VAITROBusiness.UpdateVaiTroNguoiDung(idData, listVaiTroInt));
        }


        [HttpPost]
        public JsonResult CheckExsitCode(long id, string code)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            return Json(DM_VAITROBusiness.checkExistCode(code, id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var searchModel = SessionManager.GetValue("dmvaitroSearchModel") as DM_VAITRO_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new DM_VAITRO_SEARCHBO();
                searchModel.pageSize = 20;
            }

            searchModel.QR_MA = form["QR_MA"];
            searchModel.QR_VAITRO = form["QR_VAITRO"];
            SessionManager.SetValue("dmvaitroSearchModel", searchModel);

            var data = DM_VAITROBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }


        /// <summary>
        /// Phân quyền cho vai trò
        /// </summary>
        /// <param name="id">id của vai trò</param>
        /// <returns></returns>
        public ActionResult PhanQuyen(int id)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            var model = new PhanQuyenVM();
            model.ListAllChucNang = DM_CHUCNANGBusiness.getChucNangThaoTac();
            model.ListChucNangVaiTro = VAITRO_THAOTACBusiness.getChucNangCuaVaiTro(id);
            model.VaiTro = DM_VAITROBusiness.Find(id);
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveVaiTroThaoTac(int idvaitro, List<long> ArrThaoTac)
        {
            DM_THAOTACBusiness = Get<DM_THAOTACBusiness>();
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            var listTaoTacDB = VAITRO_THAOTACBusiness.getListThaotacByVaiTro(idvaitro);
            var isdone = VAITRO_THAOTACBusiness.UpdatePermission(idvaitro, ArrThaoTac);
            var result = new JsonResultBO(true);
            if (!isdone)
            {
                result.Status = false;
                result.Message = "Không cập nhật được quyền cho vai trò";
            }
            return Json(result);

        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var searchModel = SessionManager.GetValue("dmvaitroSearchModel") as DM_VAITRO_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new DM_VAITRO_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("dmvaitroSearchModel", searchModel);
            }
            var data = DM_VAITROBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        [CodeAllowAccess(Code = "CREATE_ROLE")]
        public PartialViewResult Create()
        {
            return PartialView("_CreatePartial");
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new DM_VAITRO();
                myobj.NGAYTAO = DateTime.Now;
                myobj.MA_VAITRO = collection["MA_VAITRO"].ToString();
                myobj.TEN_VAITRO = collection["TEN_VAITRO"].ToString();
                myobj.IS_RECEIVE_DOC_DIRECTLY = collection["IS_RECEIVE_DOC_DIRECTLY"].ToIntOrZero() > 0;
                DM_VAITROBusiness.Save(myobj);
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
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var myModel = new EditVM();
            myModel.objModel = DM_VAITROBusiness.repository.Find(id);
            return PartialView("_EditPartial", myModel);
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = DM_VAITROBusiness.Find(id);

                myobj.NGAYSUA = DateTime.Now;

                myobj.MA_VAITRO = collection["MA_VAITRO"].ToString();
                myobj.TEN_VAITRO = collection["TEN_VAITRO"].ToString();
                myobj.IS_RECEIVE_DOC_DIRECTLY = collection["IS_RECEIVE_DOC_DIRECTLY"].ToIntOrZero() > 0;
                DM_VAITROBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            DM_VAITROBusiness.repository.Delete(id);
            DM_VAITROBusiness.Save();
            return Json(result);
        }
        public PartialViewResult ShowUserInRoleAction(int id)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            PhanQuyenVM model = new PhanQuyenVM();
            var ListUserInRole = DM_VAITROBusiness.GetListUserInRole(id);
            model.ListUserInRole = ListUserInRole;
            model.ListUserNotInRole = ListUserNotInRole(id);
            model.VaiTroID = id;
            return PartialView("_ShowUserInRoleAction", model);
        }
        public List<SelectListItem> ListUserNotInRole(int RoleID)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            return DM_VAITROBusiness.ListUserNotInRole(RoleID);
        }
        public JsonResult AddUserToRole(FormCollection formcol)
        {
            var result = new JsonResultBO(true);

            if (!string.IsNullOrEmpty(formcol["USER_ID"]))
            {
                var ListUserID = formcol["USER_ID"].Split(',');
                NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
                foreach (var item in ListUserID)
                {
                    NGUOIDUNG_VAITRO NguoiDungVaiTro = new NGUOIDUNG_VAITRO();
                    NguoiDungVaiTro.NGUOIDUNG_ID = item.ToIntOrZero();
                    NguoiDungVaiTro.VAITRO_ID = formcol["VAITRO_ID"].ToIntOrZero();
                    NguoiDungVaiTro.NGAYTAO = DateTime.Now;
                    NGUOIDUNG_VAITROBusiness.Save(NguoiDungVaiTro);
                }
            }
            return Json(result);
        }
        public PartialViewResult ReloadUserInRole(int RoleID)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            PhanQuyenVM model = new PhanQuyenVM();
            var ListUserInRole = DM_VAITROBusiness.GetListUserInRole(RoleID);
            model.ListUserInRole = ListUserInRole;
            return PartialView("_UpdateUserInRole", model);
        }
        public PartialViewResult ReloadUserNotInRole(int RoleID)
        {
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            PhanQuyenVM model = new PhanQuyenVM();
            model.ListUserNotInRole = ListUserNotInRole(RoleID);
            model.VaiTroID = RoleID;
            return PartialView("_UpdateUserNotInRole", model);
        }
        public JsonResult RemoveUsserInRole(int? UserRoleID)
        {
            var result = new JsonResultBO(true);

            if (UserRoleID > 0)
            {
                NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
                NGUOIDUNG_VAITROBusiness.repository.Delete(UserRoleID);
                NGUOIDUNG_VAITROBusiness.Save();
                result.Message = "Xóa người dùng thành công!";
            }
            else
            {
                result.Status = false;
                result.Message = "Không thể xóa người dùng khỏi vai trò!";
            }
            return Json(result);
        }
    }
}

